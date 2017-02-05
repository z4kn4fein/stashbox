using Stashbox.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Stashbox.Overrides
{
    /// <summary>
    /// Represents an override manager.
    /// </summary>
    public class OverrideManager
    {
        private readonly TypeOverride[] typeOverrides;

        /// <summary>
        /// Constructs an <see cref="OverrideManager"/>
        /// </summary>
        /// <param name="overrides">The initial overrides.</param>
        public OverrideManager(TypeOverride[] overrides)
        {
            if (overrides == null || !overrides.Any()) return;
            this.typeOverrides = overrides;
        }

        /// <summary>
        /// Gets an overridden value if has any.
        /// </summary>
        /// <param name="type">The type information.</param>
        /// <returns>The overridden value.</returns>
        public object GetOverriddenValue(Type type)
        {
            return GetTypedValue(type);
        }

        /// <summary>
        /// Returns all the overrides.
        /// </summary>
        /// <returns>The collection of the overrides.</returns>
        public IEnumerable<Override> GetOverrides()
        {
            return this.typeOverrides;
        }

        /// <summary>
        /// Checks that the <see cref="OverrideManager"/> contains an override for a service.
        /// </summary>
        /// <param name="parameter">The type information.</param>
        /// <returns>True if an override already exists for a service, otherwise false.</returns>
        public bool ContainsValue(TypeInformation parameter)
        {
            return this.typeOverrides != null && this.typeOverrides.Any(x => x.OverrideType == parameter.Type ||
                                                                             x.OverrideType.GetTypeInfo().IsAssignableFrom(parameter.Type.GetTypeInfo()));
        }
        
        private object GetTypedValue(Type type)
        {
            return this.typeOverrides?.FirstOrDefault(t => t.OverrideType == type)?.Value;
        }
    }
}