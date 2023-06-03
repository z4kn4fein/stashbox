using Stashbox.Expressions;
using Stashbox.Lifetime;
using Stashbox.Registration;
using Stashbox.Resolution.Resolvers;
using Stashbox.Resolution.Wrappers;
using Stashbox.Utils;
using Stashbox.Utils.Data.Immutable;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Linq.Expressions;

namespace Stashbox.Resolution;

internal class ResolutionStrategy : IResolutionStrategy
{
    private readonly ParentContainerResolver parentContainerResolver;
    private ImmutableBucket<IResolver> resolverRepository;

    public ResolutionStrategy()
    {
        this.parentContainerResolver = new ParentContainerResolver();
        this.resolverRepository = new ImmutableBucket<IResolver>(new IResolver[]
        {
            new EnumerableWrapper(),
            new LazyWrapper(),
            new FuncWrapper(),
            new MetadataWrapper(),
            new KeyValueWrapper(),

            new ServiceProviderResolver(),
            new OptionalValueResolver(),
            new DefaultValueResolver(),
            this.parentContainerResolver,
            new UnknownTypeResolver(),
        });
    }
    
    public ServiceContext BuildExpressionForType(ResolutionContext resolutionContext, TypeInformation typeInformation)
    {
        if (typeInformation.Type == TypeCache<IDependencyResolver>.Type)
            return resolutionContext.CurrentScopeParameter.AsServiceContext();

        if (typeInformation.Type == TypeCache<IRequestContext>.Type)
        {
            resolutionContext.RequestConfiguration.RequiresRequestContext = true;
            return resolutionContext.RequestContextParameter.AsServiceContext();
        }

        if (!resolutionContext.IsTopRequest)
        {
            if (resolutionContext.ParameterExpressions.Length > 0)
            {
                var type = typeInformation.Type;
                var length = resolutionContext.ParameterExpressions.Length;
                for (var i = length; i-- > 0;)
                {
                    var parameters = resolutionContext.ParameterExpressions[i]
                        .Where(p => p.I2.Type == type ||
                                    p.I2.Type.Implements(type)).CastToArray();

                    if (parameters.Length == 0) continue;
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                        var selected = parameters.FirstOrDefault(parameter => !parameter.I1) ?? parameters[^1];
#else
                    var selected = parameters.FirstOrDefault(parameter => !parameter.I1) ?? parameters[parameters.Length - 1];
#endif

                    selected.I1 = true;
                    return selected.I2.AsServiceContext();
                }
            }

            var decorators = resolutionContext.RemainingDecorators.GetOrDefaultByRef(typeInformation.Type);
            if (decorators is { Length: > 0 })
                return BuildExpressionForDecorator(decorators.Front(),
                    resolutionContext.BeginDecoratingContext(typeInformation.Type, decorators), typeInformation, decorators).AsServiceContext();
        }

        var exprOverride = resolutionContext.ExpressionOverrides?.GetOrDefaultByValue(typeInformation.DependencyName ?? typeInformation.Type);
        if (exprOverride != null)
            return exprOverride.AsServiceContext();

        var registration = resolutionContext.ResolutionBehavior.Has(ResolutionBehavior.Current) 
            ? resolutionContext.CurrentContainerContext.RegistrationRepository
                .GetRegistrationOrDefault(typeInformation, resolutionContext)
            : null;

        var isResolutionCallRequired = registration?.Options.IsOn(RegistrationOption.IsResolutionCallRequired) ?? false;
        if (!resolutionContext.IsTopRequest && registration != null && isResolutionCallRequired)
            return resolutionContext.CurrentScopeParameter
                .ConvertTo(TypeCache<IDependencyResolver>.Type)
                .CallMethod(Constants.ResolveMethod, 
                    typeInformation.Type.AsConstant(), typeInformation.DependencyName.AsConstant(),
                    resolutionContext.ExpressionOverrides?.Walk().Select(c => c.Value).ToArray().AsConstant() ?? TypeCache.EmptyArray<object>().AsConstant(),
                    resolutionContext.ResolutionBehavior.AsConstant())
                .ConvertTo(typeInformation.Type)
                .AsServiceContext(registration);

        resolutionContext = resolutionContext.BeginSubDependencyContext();
        return registration != null
            ? this.BuildExpressionForRegistration(registration, resolutionContext, typeInformation)
            : this.BuildExpressionUsingWrappersOrResolvers(resolutionContext, typeInformation);
    }

    public IEnumerable<ServiceContext> BuildExpressionsForEnumerableRequest(ResolutionContext resolutionContext, TypeInformation typeInformation)
    {
        var registrations = resolutionContext.ResolutionBehavior.Has(ResolutionBehavior.Current) 
            ? resolutionContext.CurrentContainerContext.RegistrationRepository
                .GetRegistrationsOrDefault(typeInformation, resolutionContext)
            : null;

        if (registrations == null)
            return this.BuildEnumerableExpressionUsingWrappersOrResolvers(resolutionContext, typeInformation);

        var expressions = registrations.Select(registration =>
        {
            var decorators = resolutionContext.RemainingDecorators.GetOrDefaultByRef(typeInformation.Type);
            if (decorators == null || decorators.Length == 0)
                return this.BuildExpressionForRegistration(registration, resolutionContext, typeInformation);

            decorators.ReplaceBack(registration);
            return BuildExpressionForDecorator(decorators.Front(),
                resolutionContext.BeginDecoratingContext(typeInformation.Type, decorators),
                typeInformation, decorators).AsServiceContext(registration);
        });

        if (!this.parentContainerResolver.CanUseForResolution(typeInformation, resolutionContext)) return expressions;
        
        var parentRegistrations =
            this.parentContainerResolver.GetExpressionsForEnumerableRequest(this, typeInformation, resolutionContext);
        return expressions.Concat(parentRegistrations);
    }

    public ServiceContext BuildExpressionForRegistration(ServiceRegistration serviceRegistration,
        ResolutionContext resolutionContext, TypeInformation typeInformation)
    {
        if (serviceRegistration is OpenGenericRegistration openGenericRegistration)
        {
            var genericType = serviceRegistration.ImplementationType.MakeGenericType(typeInformation.Type.GetGenericArguments());
            serviceRegistration = openGenericRegistration.ProduceClosedRegistration(genericType);
        }

        var decoratorContext = resolutionContext.RequestInitiatorResolutionBehavior.Has(ResolutionBehavior.Current)
            ? resolutionContext.RequestInitiatorContainerContext
            : resolutionContext.RequestInitiatorContainerContext.ParentContext;

        var decorators = resolutionContext.RequestInitiatorResolutionBehavior.Has(ResolutionBehavior.Parent)
            ? CollectDecorators(serviceRegistration.ImplementationType, typeInformation, 
                resolutionContext, decoratorContext)
            : decoratorContext?.DecoratorRepository.GetDecoratorsOrDefault(serviceRegistration.ImplementationType, typeInformation, resolutionContext);

        if (decorators == null)
            return BuildExpressionAndApplyLifetime(serviceRegistration, resolutionContext, typeInformation)
                .AsServiceContext(serviceRegistration);

        var stack = decorators.AsStack();
        stack.PushBack(serviceRegistration);
        return BuildExpressionForDecorator(stack.Front(),
                resolutionContext.BeginDecoratingContext(typeInformation.Type, stack), typeInformation, stack)
            .AsServiceContext(serviceRegistration);
    }

    public bool IsTypeResolvable(ResolutionContext resolutionContext, TypeInformation typeInformation)
    {
        if (typeInformation.Type.IsGenericTypeDefinition)
            return false;

        if (typeInformation.Type == TypeCache<IDependencyResolver>.Type ||
            typeInformation.Type == TypeCache<IServiceProvider>.Type ||
            typeInformation.Type == TypeCache<IRequestContext>.Type)
            return true;

        if (resolutionContext.CurrentContainerContext.RegistrationRepository.ContainsRegistration(typeInformation.Type, typeInformation.DependencyName) ||
            this.IsWrappedTypeRegistered(typeInformation, resolutionContext))
            return true;

        var exprOverride = resolutionContext.ExpressionOverrides?.GetOrDefaultByValue(typeInformation.DependencyName ?? typeInformation.Type);
        return exprOverride != null || this.CanLookupService(typeInformation, resolutionContext);
    }

    public void RegisterResolver(IResolver resolver) =>
        Swap.SwapValue(ref this.resolverRepository, (t1, _, _, _, repo) =>
            repo.Insert(repo.Length - 4, t1), resolver, Constants.DelegatePlaceholder, Constants.DelegatePlaceholder, Constants.DelegatePlaceholder);

    private ServiceContext BuildExpressionUsingWrappersOrResolvers(ResolutionContext resolutionContext, TypeInformation typeInformation)
    {
        var length = this.resolverRepository.Length;
        for (var i = 0; i < length; i++)
        {
            var wrapper = this.resolverRepository[i];

            switch (wrapper)
            {
                case IEnumerableWrapper enumerableWrapper when enumerableWrapper.TryUnWrap(typeInformation, out var unWrappedEnumerable):
                    return enumerableWrapper
                        .WrapExpression(typeInformation, unWrappedEnumerable, 
                            this.BuildExpressionsForEnumerableRequest(resolutionContext, unWrappedEnumerable))
                        .AsServiceContext();
                case IServiceWrapper serviceWrapper when serviceWrapper.TryUnWrap(typeInformation, out var unWrappedServiceType):
                {
                    var serviceContext = this.BuildExpressionForType(resolutionContext, unWrappedServiceType);
                    return serviceContext.IsEmpty() ? ServiceContext.Empty : serviceWrapper
                        .WrapExpression(typeInformation, unWrappedServiceType, serviceContext)
                        .AsServiceContext(serviceContext.ServiceRegistration);
                }
                case IParameterizedWrapper parameterizedWrapper when parameterizedWrapper.TryUnWrap(typeInformation, out var unWrappedParameterizedType, out var parameters):
                {
                    var parameterExpressions = parameters.Select(p => p.AsParameter()).CastToArray();
                    var serviceContext = this.BuildExpressionForType(resolutionContext.BeginContextWithFunctionParameters(parameterExpressions), unWrappedParameterizedType);
                    return serviceContext.IsEmpty() ? ServiceContext.Empty : parameterizedWrapper
                        .WrapExpression(typeInformation, unWrappedParameterizedType, serviceContext, parameterExpressions)
                        .AsServiceContext(serviceContext.ServiceRegistration);
                }
            }
        }

        return this.BuildExpressionUsingResolvers(resolutionContext, typeInformation);
    }

    private IEnumerable<ServiceContext> BuildEnumerableExpressionUsingWrappersOrResolvers(ResolutionContext resolutionContext, TypeInformation typeInformation)
    {
        var length = this.resolverRepository.Length;
        for (var i = 0; i < length; i++)
        {
            var wrapper = this.resolverRepository[i];

            switch (wrapper)
            {
                case IServiceWrapper serviceWrapper when serviceWrapper.TryUnWrap(typeInformation, out var unWrappedServiceType):
                {
                    var serviceContexts = this.BuildExpressionsForEnumerableRequest(resolutionContext, unWrappedServiceType);
                    return serviceContexts.Select(serviceContext => serviceWrapper
                        .WrapExpression(typeInformation, unWrappedServiceType, serviceContext)
                        .AsServiceContext(serviceContext.ServiceRegistration));
                }
                case IParameterizedWrapper parameterizedWrapper when parameterizedWrapper.TryUnWrap(typeInformation, out var unWrappedParameterizedType, out var parameters):
                {
                    var parameterExpressions = parameters.Select(p => p.AsParameter()).CastToArray();
                    var serviceContexts = this.BuildExpressionsForEnumerableRequest(resolutionContext.BeginContextWithFunctionParameters(parameterExpressions), unWrappedParameterizedType);
                    return serviceContexts.Select(serviceContext => parameterizedWrapper
                        .WrapExpression(typeInformation, unWrappedParameterizedType, serviceContext, parameterExpressions)
                        .AsServiceContext(serviceContext.ServiceRegistration));
                }
            }
        }

        return this.BuildEnumerableExpressionsUsingResolvers(resolutionContext, typeInformation);
    }

    private ServiceContext BuildExpressionUsingResolvers(ResolutionContext resolutionContext, TypeInformation typeInformation)
    {
        var length = this.resolverRepository.Length;
        for (var i = 0; i < length; i++)
        {
            var item = this.resolverRepository[i];
            if (item is IServiceResolver resolver && resolver.CanUseForResolution(typeInformation, resolutionContext))
                return resolver.GetExpression(this, typeInformation, resolutionContext);
        }

        return ServiceContext.Empty;
    }

    private IEnumerable<ServiceContext> BuildEnumerableExpressionsUsingResolvers(ResolutionContext resolutionContext, TypeInformation typeInformation)
    {
        var length = this.resolverRepository.Length;
        for (var i = 0; i < length; i++)
        {
            var item = this.resolverRepository[i];
            if (item is IEnumerableSupportedResolver resolver && resolver.CanUseForResolution(typeInformation, resolutionContext))
                return resolver.GetExpressionsForEnumerableRequest(this, typeInformation, resolutionContext);
        }

        return TypeCache.EmptyArray<ServiceContext>();
    }

    private bool IsWrappedTypeRegistered(TypeInformation typeInformation, ResolutionContext resolutionContext)
    {
        var length = this.resolverRepository.Length;
        for (var i = 0; i < length; i++)
        {
            var middleware = this.resolverRepository[i];
            switch (middleware)
            {
                case IEnumerableWrapper enumerableWrapper when enumerableWrapper.TryUnWrap(typeInformation, out var _):
                case IServiceWrapper serviceWrapper when serviceWrapper.TryUnWrap(typeInformation, out var unWrappedServiceType) &&
                                                         resolutionContext.CurrentContainerContext.RegistrationRepository.ContainsRegistration(unWrappedServiceType.Type, typeInformation.DependencyName):
                case IParameterizedWrapper parameterizedWrapper when parameterizedWrapper.TryUnWrap(typeInformation, out var unWrappedParameterizedType, out var _) &&
                                                                     resolutionContext.CurrentContainerContext.RegistrationRepository.ContainsRegistration(unWrappedParameterizedType.Type, typeInformation.DependencyName):
                    return true;
            }
        }

        return false;
    }

    private bool CanLookupService(TypeInformation typeInfo, ResolutionContext resolutionContext)
    {
        var length = this.resolverRepository.Length;
        for (var i = 0; i < length; i++)
        {
            var item = this.resolverRepository[i];
            if (item is ILookup lookup && lookup.CanLookupService(typeInfo, resolutionContext))
                return true;
        }

        return false;
    }

    private static IEnumerable<ServiceRegistration>? CollectDecorators(Type implementationType,
        TypeInformation typeInformation, ResolutionContext resolutionContext, IContainerContext? decoratorContext)
    {
        if (decoratorContext == null) return null;
        var parentDecorators = CollectDecorators(implementationType, 
            typeInformation, resolutionContext, decoratorContext.ParentContext);
        
        if (parentDecorators == null)
            return decoratorContext.DecoratorRepository.GetDecoratorsOrDefault(implementationType, 
                typeInformation, resolutionContext);
        
        var currentDecorators = decoratorContext.DecoratorRepository.GetDecoratorsOrDefault(implementationType, 
            typeInformation, resolutionContext);

        return currentDecorators == null ? parentDecorators : parentDecorators.Concat(currentDecorators);
    }

    private static Expression? BuildExpressionForDecorator(ServiceRegistration serviceRegistration,
        ResolutionContext resolutionContext, TypeInformation typeInformation, Utils.Data.Stack<ServiceRegistration> decorators)
    {
        if (serviceRegistration is OpenGenericRegistration openGenericRegistration)
        {
            var genericType = serviceRegistration.ImplementationType.MakeGenericType(typeInformation.Type.GetGenericArguments());
            serviceRegistration = openGenericRegistration.ProduceClosedRegistration(genericType);
        }

        return BuildExpressionAndApplyLifetime(serviceRegistration, resolutionContext,
            typeInformation, decorators.PeekBack()?.Lifetime);
    }

    private static Expression? BuildExpressionAndApplyLifetime(ServiceRegistration serviceRegistration,
        ResolutionContext resolutionContext, TypeInformation typeInformation, LifetimeDescriptor? secondaryLifetimeDescriptor = null)
    {
        var lifetimeDescriptor = secondaryLifetimeDescriptor != null && serviceRegistration.Lifetime is EmptyLifetime
            ? secondaryLifetimeDescriptor
            : serviceRegistration.Lifetime;

        var expression = !IsOutputLifetimeManageable(serviceRegistration)
            ? ExpressionBuilder.BuildExpressionForRegistration(serviceRegistration, resolutionContext, typeInformation)
            : lifetimeDescriptor.ApplyLifetime(serviceRegistration, resolutionContext, typeInformation);

        return typeInformation.Type != expression?.Type
            ? expression?.ConvertTo(typeInformation.Type)
            : expression;
    }

    private static bool IsOutputLifetimeManageable(ServiceRegistration serviceRegistration) =>
        serviceRegistration is not OpenGenericRegistration &&
        !serviceRegistration.IsInstance();
}