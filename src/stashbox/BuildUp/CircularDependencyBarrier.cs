using Stashbox.Exceptions;
using System;
using Stashbox.Entity;

namespace Stashbox.BuildUp
{
    internal sealed class CircularDependencyBarrier : IDisposable
    {
        private readonly ResolutionInfo resolutionInfo;
        private readonly Type type;

        public CircularDependencyBarrier(ResolutionInfo resolutionInfo, Type type)
        {
            resolutionInfo.AddCircularDependencyCheck(type, out bool updated);
            if (updated)
                throw new CircularDependencyException(type.FullName);
            
            this.resolutionInfo = resolutionInfo;
            this.type = type;
        }

        public void Dispose() =>
            this.resolutionInfo.ClearCircularDependencyCheck(this.type);
    }
}
