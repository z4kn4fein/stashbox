using Stashbox.Entity;
using System;
using System.Linq;
using System.Reflection;

namespace Stashbox.Overrides
{
    /// <summary>
    /// Represents an override manager.
    /// </summary>
    public class OverrideManager
    {
        private readonly NamedOverride[] nameOverrides;
        private readonly TypeOverride[] typeOverrides;

        /// <summary>
        /// Constructs an <see cref="OverrideManager"/>
        /// </summary>
        /// <param name="overrides">The initial overrides.</param>
        public OverrideManager(Override[] overrides)
        {
            if (overrides == null || !overrides.Any()) return;

            this.nameOverrides = overrides.OfType<NamedOverride>().ToArray();
            this.typeOverrides = overrides.OfType<TypeOverride>().ToArray();
        }

        /// <summary>
        /// Gets an overridden value if has any.
        /// </summary>
        /// <param name="parameter">The type information.</param>
        /// <returns>The overridden value.</returns>
        public object GetOverriddenValue(TypeInformation parameter)
        {
            return GetTypedValue(parameter.Type) ?? GetNamedValue(parameter.DependencyName);
        }

        /// <summary>
        /// Checks that the <see cref="OverrideManager"/> contains an override for a service.
        /// </summary>
        /// <param name="parameter">The type information.</param>
        /// <returns>True if an override already exists for a service, otherwise false.</returns>
        public bool ContainsValue(TypeInformation parameter)
        {
            return (this.typeOverrides != null && this.typeOverrides.Any(x => x.OverrideType == parameter.Type ||
                x.OverrideType.GetTypeInfo().IsAssignableFrom(parameter.Type.GetTypeInfo()))) ||
                (this.nameOverrides != null && this.nameOverrides.Any(x => x.OverrideName == parameter.DependencyName));
        }

        private object GetTypedValue(Type type)
        {
            return this.typeOverrides?.FirstOrDefault(t => t.OverrideType == type)?.Value;
        }

        private object GetNamedValue(string name)
        {
            return this.nameOverrides?.FirstOrDefault(t => t.OverrideName.Equals(name))?.Value;
        }
    }
}