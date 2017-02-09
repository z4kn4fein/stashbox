using Stashbox.Exceptions;
using System;
using System.Collections.Generic;
using Stashbox.Configuration;

namespace Stashbox.BuildUp
{
    internal sealed class CircularDependencyBarrier : IDisposable
    {
        private readonly ISet<Type> set;
        private readonly Type type;

        public CircularDependencyBarrier(ISet<Type> set, Type type)
        {
            if (set.Contains(type))
                throw new CircularDependencyException(type.FullName);

            set.Add(type);
            this.set = set;
            this.type = type;
        }

        public void Dispose()
        {
            this.set?.Remove(this.type);
        }
    }
}
