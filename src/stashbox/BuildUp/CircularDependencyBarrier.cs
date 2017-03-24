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
            var existing = resolutionInfo.CircularDependencyBarrier.GetOrDefault(type.GetHashCode());
            if (existing != null)
                throw new CircularDependencyException(type.FullName);

            resolutionInfo.CircularDependencyBarrier.AddOrUpdate(type.GetHashCode(), type);
            this.resolutionInfo = resolutionInfo;
            this.type = type;
        }

        public void Dispose() =>
            this.resolutionInfo.CircularDependencyBarrier.AddOrUpdate(this.type.GetHashCode(), null, (oldValue, newValue) => newValue);
    }
}
