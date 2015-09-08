using Stashbox.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Stashbox.Overrides
{
    public class OverrideManager
    {
        private readonly HashSet<NamedOverride> nameOverrides;
        private readonly HashSet<TypeOverride> typeOverrides;

        public OverrideManager(IEnumerable<Override> overrides)
        {
            var overridesArray = overrides as Override[] ?? overrides.ToArray();

            if (!overridesArray.Any()) return;
            this.nameOverrides = new HashSet<NamedOverride>();
            this.typeOverrides = new HashSet<TypeOverride>();

            foreach (var item in overridesArray)
            {
                if (item is NamedOverride)
                    this.nameOverrides.Add(item as NamedOverride);
                if (item is TypeOverride)
                    this.typeOverrides.Add(item as TypeOverride);
            }
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
            var overridden = this.typeOverrides?.FirstOrDefault(t => t.OverrideType == type);
            return overridden?.Value;
        }

        private object GetNamedValue(string name)
        {
            var overridden = this.nameOverrides?.FirstOrDefault(t => t.OverrideName.Equals(name));
            return overridden?.Value;
        }
    }
}