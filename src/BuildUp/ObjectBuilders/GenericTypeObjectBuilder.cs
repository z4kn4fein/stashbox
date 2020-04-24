using Stashbox.Registration;
using Stashbox.Resolution;
using System;
using System.Linq.Expressions;

namespace Stashbox.BuildUp.ObjectBuilders
{
    internal class GenericTypeObjectBuilder : ObjectBuilderBase
    {
        private readonly IServiceRegistrator serviceRegistrator;
        private readonly IObjectBuilder defaultObjectBuilder;

        public GenericTypeObjectBuilder(IServiceRegistrator serviceRegistrator, IObjectBuilder defaultObjectBuilder)
        {
            this.serviceRegistrator = serviceRegistrator;
            this.defaultObjectBuilder = defaultObjectBuilder;
        }

        public override bool ResultShouldBeLifetimeManaged => false;

        protected override Expression GetExpressionInternal(IContainerContext containerContext, IServiceRegistration serviceRegistration, ResolutionContext resolutionContext, Type resolveType)
        {
            var genericType = serviceRegistration.ImplementationType.MakeGenericType(resolveType.GetGenericArguments());
            var registration = this.RegisterConcreteGenericType(containerContext, serviceRegistration, resolveType, genericType);
            return registration.GetExpression(containerContext, resolutionContext, resolveType);
        }

        private IServiceRegistration RegisterConcreteGenericType(IContainerContext containerContext, IServiceRegistration serviceRegistration, Type resolveType, Type genericType)
        {
            var newRegistration = serviceRegistration.Clone(genericType, this.defaultObjectBuilder);

            newRegistration.RegistrationContext.Name = null;
            this.serviceRegistrator.Register(containerContext, newRegistration, resolveType, false);
            return newRegistration;
        }
    }
}
