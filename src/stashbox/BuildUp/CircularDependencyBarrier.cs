using Stashbox.Exceptions;
using System;
using Stashbox.Entity;
using Stashbox.Infrastructure.Registration;

namespace Stashbox.BuildUp
{
    internal sealed class CircularDependencyBarrier : IDisposable
    {
        private readonly ResolutionInfo resolutionInfo;
        private readonly int registrationNumber;

        public CircularDependencyBarrier(ResolutionInfo resolutionInfo, IServiceRegistration registration)
        {
            resolutionInfo.AddCircularDependencyCheck(registration.RegistrationNumber, out bool updated);
            if (updated)
                throw new CircularDependencyException(registration.ImplementationType);
            
            this.resolutionInfo = resolutionInfo;
            this.registrationNumber = registration.RegistrationNumber;
        }

        public void Dispose() =>
            this.resolutionInfo.ClearCircularDependencyCheck(this.registrationNumber);
    }
}
