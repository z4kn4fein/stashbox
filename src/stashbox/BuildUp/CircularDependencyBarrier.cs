using Stashbox.Exceptions;
using Stashbox.Infrastructure.Registration;
using Stashbox.Resolution;
using System;

namespace Stashbox.BuildUp
{
    internal sealed class CircularDependencyBarrier : IDisposable
    {
        private readonly ResolutionContext resolutionContext;
        private readonly int registrationNumber;

        public CircularDependencyBarrier(ResolutionContext resolutionContext, IServiceRegistration registration)
        {
            resolutionContext.AddCircularDependencyCheck(registration.RegistrationNumber, out bool updated);
            if (updated)
                throw new CircularDependencyException(registration.ImplementationType);

            this.resolutionContext = resolutionContext;
            this.registrationNumber = registration.RegistrationNumber;
        }

        public void Dispose() =>
            this.resolutionContext.ClearCircularDependencyCheck(this.registrationNumber);
    }
}
