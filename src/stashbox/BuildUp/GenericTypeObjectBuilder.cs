using Stashbox.Entity;
using Stashbox.Infrastructure;
using System;
using System.Linq.Expressions;
using Stashbox.Infrastructure.Registration;

namespace Stashbox.BuildUp
{
    internal class GenericTypeObjectBuilder : ObjectBuilderBase
    {
        private readonly IContainerContext containerContext;

        public GenericTypeObjectBuilder(IContainerContext containerContext)
            : base(containerContext)
        {
            this.containerContext = containerContext;
        }

        protected override Expression GetExpressionInternal(IServiceRegistration serviceRegistration, ResolutionInfo resolutionInfo, Type resolveType)
        {
            var genericType = serviceRegistration.ImplementationType.MakeGenericType(resolveType.GetGenericArguments());
            var registration = this.RegisterConcreteGenericType(serviceRegistration, resolveType, genericType);
            return registration.GetExpression(resolutionInfo, resolveType);
        }

        private IServiceRegistration RegisterConcreteGenericType(IServiceRegistration serviceRegistration, Type resolveType, Type genericType)
        {
            var newData = serviceRegistration.RegistrationContext.CreateCopy();
            newData.Name = null;

            var registration = this.containerContext.Container.ServiceRegistrator.PrepareContext(resolveType,
                genericType, newData).CreateServiceRegistration(serviceRegistration.IsDecorator);

            if (!serviceRegistration.IsDecorator) return registration;
            
            this.containerContext.DecoratorRepository.AddDecorator(resolveType, registration);
            return registration;
        }
    }
}
