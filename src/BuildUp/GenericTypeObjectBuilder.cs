using Stashbox.Registration;
using Stashbox.Resolution;
using System;
using System.Linq.Expressions;

namespace Stashbox.BuildUp
{
    internal class GenericTypeObjectBuilder : ObjectBuilderBase
    {
        private readonly IServiceRegistrator serviceRegistrator;
        private readonly IObjectBuilderSelector objectBuilderSelector;

        public GenericTypeObjectBuilder(IServiceRegistrator serviceRegistrator, IObjectBuilderSelector objectBuilderSelector)
        {
            this.serviceRegistrator = serviceRegistrator;
            this.objectBuilderSelector = objectBuilderSelector;
        }

        protected override Expression GetExpressionInternal(IContainerContext containerContext, IServiceRegistration serviceRegistration, ResolutionContext resolutionContext, Type resolveType)
        {
            var genericType = serviceRegistration.ImplementationType.MakeGenericType(resolveType.GetGenericArguments());
            var registration = this.RegisterConcreteGenericType(serviceRegistration, resolveType, genericType);
            return registration.GetExpression(containerContext, resolutionContext, resolveType);
        }

        private IServiceRegistration RegisterConcreteGenericType(IServiceRegistration serviceRegistration, Type resolveType, Type genericType)
        {
            var newRegistration = serviceRegistration.Clone(resolveType, genericType,
                this.objectBuilderSelector.Get(ObjectBuilder.Default));

            newRegistration.RegistrationContext.Name = null;
            this.serviceRegistrator.Register(newRegistration, false);
            return newRegistration;
        }
    }
}
