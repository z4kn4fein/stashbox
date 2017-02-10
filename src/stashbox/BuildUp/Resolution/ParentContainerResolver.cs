using Stashbox.Entity;
using Stashbox.Exceptions;
using Stashbox.Infrastructure;
using Stashbox.Infrastructure.Resolution;
using System.Linq;
using System.Linq.Expressions;

namespace Stashbox.BuildUp.Resolution
{
    internal class ParentContainerResolver : Resolver
    {
        private readonly IContainerContext parentContainerContext;
        public ParentContainerResolver(IContainerContext containerContext, TypeInformation typeInfo)
            : base(containerContext, typeInfo)
        {
            this.parentContainerContext = containerContext.Container.ParentContainer.ContainerContext;
        }

        public override bool CanUseForEnumerableArgumentResolution => true;

        public override Expression GetExpression(ResolutionInfo resolutionInfo)
        {
            var registration = this.parentContainerContext.RegistrationRepository.GetRegistrationOrDefault(base.TypeInfo, true);
            if (registration == null)
                throw new ResolutionFailedException(base.TypeInfo.Type.FullName);

            return registration.GetExpression(resolutionInfo, base.TypeInfo);
        }

        public override Expression[] GetEnumerableArgumentExpressions(ResolutionInfo resolutionInfo)
        {
            var registrations = this.parentContainerContext.RegistrationRepository.GetRegistrationsOrDefault(base.TypeInfo);
            if (registrations == null) return null;

            var serviceRegistrations = this.parentContainerContext.Container.ContainerConfiguration.EnumerableOrderRule(registrations).ToArray();
            var lenght = serviceRegistrations.Length;
            var expressions = new Expression[lenght];
            for (int i = 0; i < lenght; i++)
                expressions[i] = serviceRegistrations[i].GetExpression(resolutionInfo, base.TypeInfo);

            return expressions;
        }
    }
}
