using Stashbox.Expressions;
using Stashbox.Lifetime;
using Stashbox.Registration;
using Stashbox.Resolution.Resolvers;
using Stashbox.Resolution.Wrappers;
using Stashbox.Utils;
using Stashbox.Utils.Data.Immutable;
using System;
using System.Collections.Generic;
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
        this.resolverRepository = new ImmutableBucket<IResolver>([
            new EnumerableWrapper(),
            new LazyWrapper(),
            new FuncWrapper(),
            new MetadataWrapper(),
            new KeyValueWrapper(),

            new ServiceProviderResolver(),
            new OptionalValueResolver(),
            new DefaultValueResolver(),
            this.parentContainerResolver,
            new UnknownTypeResolver()
        ]);
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

        if (typeInformation is { HasDependencyNameAttribute: true, Parent: not null })
            return typeInformation.Parent.DependencyName.AsConstant().ConvertTo(typeInformation.Type)
                .AsServiceContext();

        if (typeInformation.IsDependency)
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
                    var selected = Array.Find(parameters, parameter => !parameter.I1) ?? parameters[^1];
#else
                    var selected = Array.Find(parameters, parameter => !parameter.I1) ??
                                   parameters[parameters.Length - 1];
#endif

                    selected.I1 = true;
                    return selected.I2.AsServiceContext();
                }
            }

            var decorators = resolutionContext.RemainingDecorators.GetOrDefaultByRef(typeInformation.Type);
            if (decorators is { Length: > 0 })
                return BuildExpressionForDecorator(decorators.Front(),
                    resolutionContext.BeginDecoratingContext(typeInformation.Type, decorators), typeInformation,
                    decorators).AsServiceContext();
        }

        var exprOverride = resolutionContext.ExpressionOverrides?.GetOrDefault(typeInformation.Type);
        if (exprOverride != null)
        {
            var expression = typeInformation.DependencyName != null
                ? exprOverride.LastOrDefault(e => typeInformation.DependencyName.Equals(e.DependencyName))
                : exprOverride.LastOrDefault();

            if (expression != null)
                return expression.Instance.AsConstant().AsServiceContext();
        }

        var registration = resolutionContext.ResolutionBehavior.Has(ResolutionBehavior.Current)
            ? resolutionContext.CurrentContainerContext.RegistrationRepository
                .GetRegistrationOrDefault(typeInformation, resolutionContext)
            : null;

        var isResolutionCallRequired = registration?.Options.IsOn(RegistrationOption.IsResolutionCallRequired) ?? false;
        if (isResolutionCallRequired && typeInformation.IsDependency && registration != null)
            return resolutionContext.CurrentScopeParameter
                .ConvertTo(TypeCache<IDependencyResolver>.Type)
                .CallMethod(Constants.ResolveMethod,
                    typeInformation.Type.AsConstant(), typeInformation.DependencyName.AsConstant(),
                    resolutionContext.ExpressionOverrides?.Walk().SelectMany(c =>
                        c.Select(ov => ov)).ToArray().AsConstant() ?? TypeCache.EmptyArray<object>().AsConstant(),
                    resolutionContext.ResolutionBehavior.AsConstant())
                .ConvertTo(typeInformation.Type)
                .AsServiceContext(registration);

        return registration != null
            ? this.BuildExpressionForRegistration(registration, resolutionContext, typeInformation)
            : this.BuildExpressionUsingWrappersOrResolvers(resolutionContext, typeInformation);
    }

    public IEnumerable<ServiceContext> BuildExpressionsForEnumerableRequest(ResolutionContext resolutionContext,
        TypeInformation typeInformation)
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

        if (resolutionContext.ResolutionBehavior.Has(ResolutionBehavior.PreferEnumerableInCurrent) &&
            !resolutionContext.IsInParentContext)
            return expressions;
        
        if (!this.parentContainerResolver.CanUseForResolution(typeInformation, resolutionContext)) return expressions;

        var parentRegistrations =
            this.parentContainerResolver.GetExpressionsForEnumerableRequest(this, typeInformation,
                resolutionContext);
        return parentRegistrations.Concat(expressions);
    }

    public ServiceContext BuildExpressionForRegistration(ServiceRegistration serviceRegistration,
        ResolutionContext resolutionContext, TypeInformation typeInformation)
    {
        if (serviceRegistration is OpenGenericRegistration openGenericRegistration)
        {
            var genericType =
                serviceRegistration.ImplementationType.MakeGenericType(typeInformation.Type.GetGenericArguments());
            serviceRegistration = openGenericRegistration.ProduceClosedRegistration(genericType);
        }

        var decoratorContext = resolutionContext.RequestInitiatorResolutionBehavior.Has(ResolutionBehavior.Current)
            ? resolutionContext.RequestInitiatorContainerContext
            : resolutionContext.RequestInitiatorContainerContext.ParentContext;

        var decorators = resolutionContext.RequestInitiatorResolutionBehavior.Has(ResolutionBehavior.Parent) ||
                         resolutionContext.RequestInitiatorResolutionBehavior.Has(ResolutionBehavior.ParentDependency)
            ? CollectDecorators(serviceRegistration.ImplementationType, typeInformation,
                resolutionContext, decoratorContext)
            : SearchAndFilterDecorators(serviceRegistration.ImplementationType, typeInformation, resolutionContext,
                decoratorContext);

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

        if (resolutionContext.CurrentContainerContext.RegistrationRepository.ContainsRegistration(typeInformation.Type,
                typeInformation.DependencyName) ||
            this.IsWrappedTypeRegistered(typeInformation, resolutionContext))
            return true;

        var exprOverride = resolutionContext.ExpressionOverrides?.GetOrDefault(typeInformation.Type);
        if (exprOverride == null) return this.CanLookupService(typeInformation, resolutionContext);
        if (typeInformation.DependencyName != null)
            return exprOverride.LastOrDefault(exp => exp.DependencyName == typeInformation.DependencyName) != null;

        return true;
    }

    public void RegisterResolver(IResolver resolver) =>
        Swap.SwapValue(ref this.resolverRepository, (t1, _, _, _, repo) =>
                repo.Insert(repo.Length - 4, t1), resolver, Constants.DelegatePlaceholder,
            Constants.DelegatePlaceholder,
            Constants.DelegatePlaceholder);

    private ServiceContext BuildExpressionUsingWrappersOrResolvers(ResolutionContext resolutionContext,
        TypeInformation typeInformation)
    {
        var length = this.resolverRepository.Length;
        for (var i = 0; i < length; i++)
        {
            var wrapper = this.resolverRepository[i];

            switch (wrapper)
            {
                case IEnumerableWrapper enumerableWrapper
                    when enumerableWrapper.TryUnWrap(typeInformation.Type, out var unWrappedEnumerable):
                {
                    var unwrappedTypeInformation = typeInformation.Clone(unWrappedEnumerable);
                    var expressions = this.BuildExpressionsForEnumerableRequest(resolutionContext,
                        unwrappedTypeInformation);
                    if (resolutionContext.CurrentContainerContext.ContainerConfiguration.ExceptionOverEmptyCollectionEnabled && 
                        !typeInformation.Type.MapsToGenericTypeDefinition(TypeCache.EnumerableType) &&
                        expressions == TypeCache.EmptyArray<ServiceContext>()) return ServiceContext.Empty;
                    return enumerableWrapper
                        .WrapExpression(typeInformation, unwrappedTypeInformation, expressions)
                        .AsServiceContext();
                }
                case IServiceWrapper serviceWrapper
                    when serviceWrapper.TryUnWrap(typeInformation.Type, out var unWrappedServiceType):
                {
                    var unwrappedTypeInformation = typeInformation.Clone(unWrappedServiceType);
                    var serviceContext = this.BuildExpressionForType(resolutionContext, unwrappedTypeInformation);
                    return serviceContext.IsEmpty()
                        ? ServiceContext.Empty
                        : serviceWrapper
                            .WrapExpression(typeInformation, unwrappedTypeInformation, serviceContext)
                            .AsServiceContext(serviceContext.ServiceRegistration);
                }
                case IParameterizedWrapper parameterizedWrapper when parameterizedWrapper.TryUnWrap(
                    typeInformation.Type,
                    out var unWrappedParameterizedType, out var parameters):
                {
                    var unwrappedTypeInformation = typeInformation.Clone(unWrappedParameterizedType);
                    var parameterExpressions = parameters.Select(p => p.AsParameter()).CastToArray();
                    var serviceContext = this.BuildExpressionForType(
                        resolutionContext.BeginContextWithFunctionParameters(parameterExpressions),
                        unwrappedTypeInformation);
                    return serviceContext.IsEmpty()
                        ? ServiceContext.Empty
                        : parameterizedWrapper
                            .WrapExpression(typeInformation, unwrappedTypeInformation, serviceContext,
                                parameterExpressions)
                            .AsServiceContext(serviceContext.ServiceRegistration);
                }
                case IMetadataWrapper metadataWrapper when metadataWrapper.TryUnWrap(typeInformation.Type,
                    out var unWrappedType, out var unwrappedMetadataType):
                {
                    var unwrappedTypeInformation =
                        typeInformation.Clone(unWrappedType, metadataType: unwrappedMetadataType);
                    var serviceContext = this.BuildExpressionForType(resolutionContext, unwrappedTypeInformation);
                    return serviceContext.IsEmpty()
                        ? ServiceContext.Empty
                        : metadataWrapper
                            .WrapExpression(typeInformation, unwrappedTypeInformation, serviceContext)
                            .AsServiceContext(serviceContext.ServiceRegistration);
                }
            }
        }

        return this.BuildExpressionUsingResolvers(resolutionContext, typeInformation);
    }

    private IEnumerable<ServiceContext> BuildEnumerableExpressionUsingWrappersOrResolvers(
        ResolutionContext resolutionContext, TypeInformation typeInformation)
    {
        var length = this.resolverRepository.Length;
        for (var i = 0; i < length; i++)
        {
            var wrapper = this.resolverRepository[i];

            switch (wrapper)
            {
                case IServiceWrapper serviceWrapper
                    when serviceWrapper.TryUnWrap(typeInformation.Type, out var unWrappedServiceType):
                {
                    var unwrappedTypeInformation = typeInformation.Clone(unWrappedServiceType);
                    var serviceContexts = this.BuildExpressionsForEnumerableRequest(resolutionContext,
                        unwrappedTypeInformation);
                    return serviceContexts.Select(serviceContext => serviceWrapper
                        .WrapExpression(typeInformation, unwrappedTypeInformation, serviceContext)
                        .AsServiceContext(serviceContext.ServiceRegistration));
                }
                case IParameterizedWrapper parameterizedWrapper
                    when parameterizedWrapper.TryUnWrap(typeInformation.Type, out var unWrappedParameterizedType,
                        out var parameters):
                {
                    var unwrappedTypeInformation = typeInformation.Clone(unWrappedParameterizedType);
                    var parameterExpressions = parameters.Select(p => p.AsParameter()).CastToArray();
                    var serviceContexts = this.BuildExpressionsForEnumerableRequest(
                        resolutionContext.BeginContextWithFunctionParameters(parameterExpressions),
                        unwrappedTypeInformation);
                    return serviceContexts.Select(serviceContext => parameterizedWrapper
                        .WrapExpression(typeInformation, unwrappedTypeInformation, serviceContext, parameterExpressions)
                        .AsServiceContext(serviceContext.ServiceRegistration));
                }
                case IMetadataWrapper metadataWrapper when metadataWrapper.TryUnWrap(typeInformation.Type,
                    out var unWrappedType, out var unwrappedMetadataType):
                {
                    var unwrappedTypeInformation =
                        typeInformation.Clone(unWrappedType, metadataType: unwrappedMetadataType);
                    var serviceContexts = this.BuildExpressionsForEnumerableRequest(resolutionContext,
                        unwrappedTypeInformation);
                    return serviceContexts.Select(serviceContext => metadataWrapper
                        .WrapExpression(typeInformation, unwrappedTypeInformation, serviceContext)
                        .AsServiceContext(serviceContext.ServiceRegistration));
                }
            }
        }

        return this.BuildEnumerableExpressionsUsingResolvers(resolutionContext, typeInformation);
    }

    private ServiceContext BuildExpressionUsingResolvers(ResolutionContext resolutionContext,
        TypeInformation typeInformation)
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

    private IEnumerable<ServiceContext> BuildEnumerableExpressionsUsingResolvers(ResolutionContext resolutionContext,
        TypeInformation typeInformation)
    {
        var length = this.resolverRepository.Length;
        for (var i = 0; i < length; i++)
        {
            var item = this.resolverRepository[i];
            if (item is IEnumerableSupportedResolver resolver &&
                resolver.CanUseForResolution(typeInformation, resolutionContext))
                return resolver.GetExpressionsForEnumerableRequest(this, typeInformation,
                    resolutionContext);
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
                case IEnumerableWrapper enumerableWrapper
                    when enumerableWrapper.TryUnWrap(typeInformation.Type, out var unWrappedEnumerableType) &&
                    !resolutionContext.CurrentContainerContext.ContainerConfiguration
                        .ExceptionOverEmptyCollectionEnabled || typeInformation.Type.MapsToGenericTypeDefinition(
                                                                 TypeCache.EnumerableType)
                                                             || resolutionContext.CurrentContainerContext
                                                                 .RegistrationRepository
                                                                 .ContainsRegistration(unWrappedEnumerableType,
                                                                     typeInformation.DependencyName):
                case IServiceWrapper serviceWrapper
                    when serviceWrapper.TryUnWrap(typeInformation.Type, out var unWrappedServiceType) &&
                         resolutionContext.CurrentContainerContext.RegistrationRepository.ContainsRegistration(
                             unWrappedServiceType, typeInformation.DependencyName):
                case IParameterizedWrapper parameterizedWrapper
                    when parameterizedWrapper.TryUnWrap(typeInformation.Type, out var unWrappedParameterizedType,
                             out var _) &&
                         resolutionContext.CurrentContainerContext.RegistrationRepository.ContainsRegistration(
                             unWrappedParameterizedType, typeInformation.DependencyName):
                case IMetadataWrapper metadataWrapper
                    when metadataWrapper.TryUnWrap(typeInformation.Type, out var unWrappedType, out var _) &&
                         resolutionContext.CurrentContainerContext.RegistrationRepository.ContainsRegistration(
                             unWrappedType, typeInformation.DependencyName):
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

    private IEnumerable<ServiceRegistration>? CollectDecorators(Type implementationType,
        TypeInformation typeInformation, ResolutionContext resolutionContext, IContainerContext? decoratorContext)
    {
        if (decoratorContext == null) return null;
        var parentDecorators = CollectDecorators(implementationType,
            typeInformation, resolutionContext, decoratorContext.ParentContext);

        if (parentDecorators == null)
            return SearchAndFilterDecorators(implementationType,
                typeInformation, resolutionContext, decoratorContext);

        var currentDecorators = SearchAndFilterDecorators(implementationType,
            typeInformation, resolutionContext, decoratorContext);

        return currentDecorators == null ? parentDecorators : parentDecorators.Concat(currentDecorators);
    }

    private IEnumerable<ServiceRegistration>? SearchAndFilterDecorators(Type implementationType,
        TypeInformation typeInformation, ResolutionContext resolutionContext, IContainerContext? decoratorContext)
    {
        var decorators = decoratorContext?.DecoratorRepository.GetDecoratorsOrDefault(implementationType,
            typeInformation, resolutionContext);

        if (decorators == null) return null;

        var filtered = decorators.Where(d =>
        {
            var types = d.ImplementationType.GetPossibleDependencyTypes().ToArray();
            return types.Any(implementationType.Implements) ||
                   types.Any(t => TryUnwrapTypeFrom(t, out var unwrapped) && implementationType.Implements(unwrapped));
        }).ToArray();

        return filtered.Length == 0 ? null : filtered;
    }

    private bool TryUnwrapTypeFrom(Type wrapped, out Type unwrapped)
    {
        var length = this.resolverRepository.Length;
        for (var i = 0; i < length; i++)
        {
            var resolver = this.resolverRepository[i];
            switch (resolver)
            {
                case IEnumerableWrapper enumerableWrapper when enumerableWrapper.TryUnWrap(wrapped, out unwrapped):
                case IServiceWrapper serviceWrapper when serviceWrapper.TryUnWrap(wrapped, out unwrapped):
                case IParameterizedWrapper parameterizedWrapper
                    when parameterizedWrapper.TryUnWrap(wrapped, out unwrapped, out _):
                case IMetadataWrapper metadataWrapper when metadataWrapper.TryUnWrap(wrapped, out unwrapped, out _):
                    return true;
            }
        }

        unwrapped = TypeCache.EmptyType;
        return false;
    }

    private static Expression? BuildExpressionForDecorator(ServiceRegistration serviceRegistration,
        ResolutionContext resolutionContext, TypeInformation typeInformation,
        Utils.Data.Stack<ServiceRegistration> decorators)
    {
        if (serviceRegistration is OpenGenericRegistration openGenericRegistration)
        {
            var genericType =
                serviceRegistration.ImplementationType.MakeGenericType(typeInformation.Type.GetGenericArguments());
            serviceRegistration = openGenericRegistration.ProduceClosedRegistration(genericType);
        }

        return BuildExpressionAndApplyLifetime(serviceRegistration, resolutionContext,
            typeInformation, decorators.PeekBack()?.Lifetime);
    }

    private static Expression? BuildExpressionAndApplyLifetime(ServiceRegistration serviceRegistration,
        ResolutionContext resolutionContext, TypeInformation typeInformation,
        LifetimeDescriptor? secondaryLifetimeDescriptor = null)
    {
        var lifetimeDescriptor = secondaryLifetimeDescriptor != null && serviceRegistration.Lifetime is EmptyLifetime
            ? secondaryLifetimeDescriptor
            : serviceRegistration.Lifetime;

        var expression = !IsOutputLifetimeManageable(serviceRegistration) || lifetimeDescriptor is EmptyLifetime
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