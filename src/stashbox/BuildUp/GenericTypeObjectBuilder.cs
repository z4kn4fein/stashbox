using Stashbox.Entity;
using Stashbox.Infrastructure;
using Stashbox.Infrastructure.Registration;
using System;
using System.Linq.Expressions;

namespace Stashbox.BuildUp
{
    internal class GenericTypeObjectBuilder : ObjectBuilderBase
    {
        protected override Expression GetExpressionInternal(IContainerContext containerContext, IServiceRegistration serviceRegistration, ResolutionInfo resolutionInfo, Type resolveType)
        {
            var genericType = serviceRegistration.ImplementationType.MakeGenericType(resolveType.GetGenericArguments());
            var registration = this.RegisterConcreteGenericType(containerContext, serviceRegistration, resolveType, genericType);
            return registration.GetExpression(containerContext, resolutionInfo, resolveType);
        }

        private IServiceRegistration RegisterConcreteGenericType(IContainerContext containerContext, IServiceRegistration serviceRegistration, Type resolveType, Type genericType)
        {
            var newData = serviceRegistration.RegistrationContext.CreateCopy();
            newData.Name = null;

            var registration = containerContext.Container.ServiceRegistrator.PrepareContext(resolveType,
                genericType, newData).CreateServiceRegistration(serviceRegistration.IsDecorator);

            if (!serviceRegistration.IsDecorator)
            {
                containerContext.RegistrationRepository.AddOrUpdateRegistration(registration, false, false);
                return registration;
            }

            containerContext.DecoratorRepository.AddDecorator(resolveType, registration, false, false);
            return registration;
        }
    }
}
