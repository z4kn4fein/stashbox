using Stashbox.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Stashbox.Overrides
{
    public class OverrideManager
    {
        private readonly NamedOverride[] nameOverrides;
        private readonly TypeOverride[] typeOverrides;

        public OverrideManager(Override[] overrides)
        {
            if (overrides == null || !overrides.Any()) return;

            this.nameOverrides = overrides.OfType<NamedOverride>().ToArray();
            this.typeOverrides = overrides.OfType<TypeOverride>().ToArray();
        }

        public object GetOverriddenValue(Type type, string name)
        {
            var overridden = GetTypedValue(type) ?? GetNamedValue(name);

            return overridden;
        }

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