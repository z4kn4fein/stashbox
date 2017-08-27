using Stashbox.Entity;
using Stashbox.Infrastructure;
using System;
using System.Linq.Expressions;
using Stashbox.Infrastructure.Registration;

namespace Stashbox.BuildUp
{
    internal class GenericTypeObjectBuilder : ObjectBuilderBase
    {
        public GenericTypeObjectBuilder(IContainerContext containerContext)
            : base(containerContext)
        { }

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

            var registration = base.ContainerContext.Container.ServiceRegistrator.PrepareContext(resolveType,
                genericType, newData).CreateServiceRegistration(serviceRegistration.IsDecorator);

            if (!serviceRegistration.IsDecorator)
            {
                this.ContainerContext.RegistrationRepository.AddOrUpdateRegistration(registration, false, false);
                return registration;
            }

            base.ContainerContext.DecoratorRepository.AddDecorator(resolveType, registration, false, false);
            return registration;
        }
    }
}
