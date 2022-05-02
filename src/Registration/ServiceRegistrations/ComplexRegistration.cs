using Stashbox.Configuration;
using Stashbox.Resolution;
using Stashbox.Utils.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Stashbox.Registration.ServiceRegistrations
{
    /// <summary>
    /// 
    /// </summary>
    public class ComplexRegistration : ServiceRegistration
    {
        /// <summary>
        /// Determines whether the service's resolution should be handled by a dynamic <see cref="IDependencyResolver.Resolve(Type)"/> call on the current <see cref="IDependencyResolver"/> instead of a pre-built instantiation expression.
        /// </summary>
        public bool IsResolutionCallRequired { get; internal set; }

        /// <summary>
        /// The selected constructor if any was set.
        /// </summary>
        public ConstructorInfo? SelectedConstructor { get; internal set; }

        /// <summary>
        /// The arguments of the selected constructor if any was set.
        /// </summary>
        public object[]? ConstructorArguments { get; internal set; }

        /// <summary>
        /// Dependency names or types that are bound to named registrations.
        /// </summary>
        public Dictionary<object, object?>? DependencyBindings { get; internal set; }
        /// <summary>
        /// The cleanup delegate.
        /// </summary>
        public Action<object>? Finalizer { get; internal set; }

        /// <summary>
        /// The initializer delegate.
        /// </summary>
        public Delegate? Initializer { get; internal set; }

        /// <summary>
        /// The async initializer delegate.
        /// </summary>
        public Func<object, IDependencyResolver, CancellationToken, Task>? AsyncInitializer { get; internal set; }

        /// <summary>
        /// The auto member injection rule for the registration.
        /// </summary>
        public Rules.AutoMemberInjectionRules AutoMemberInjectionRule { get; internal set; }

        /// <summary>
        /// True if auto member injection is enabled on this instance.
        /// </summary>
        public bool AutoMemberInjectionEnabled { get; internal set; }

        /// <summary>
        /// True if the lifetime of the service is owned externally.
        /// </summary>
        public bool IsLifetimeExternallyOwned { get; internal set; }

        /// <summary>
        /// The name of the scope this registration defines.
        /// </summary>
        public object? DefinedScopeName { get; internal set; }

        /// <summary>
        /// The constructor selection rule.
        /// </summary>
        public Func<IEnumerable<ConstructorInfo>, IEnumerable<ConstructorInfo>>? ConstructorSelectionRule { get; internal set; }

        /// <summary>
        /// A filter delegate used to determine which members should be auto injected and which are not.
        /// </summary>
        public Func<MemberInfo, bool>? AutoMemberInjectionFilter { get; internal set; }

        /// <summary>
        /// The additional metadata.
        /// </summary>
        public object? Metadata { get; internal set; }

        internal bool ReplaceExistingRegistration { get; set; }
        internal bool ReplaceExistingRegistrationOnlyIfExists { get; set; }
        internal ExpandableArray<Type>? AdditionalServiceTypes { get; set; }
        internal ExpandableArray<KeyValuePair<string, object?>>? InjectionParameters { get; set; }
        internal bool HasScopeName { get; set; }
        internal ExpandableArray<Type>? TargetTypeConditions { get; set; }
        internal ExpandableArray<Func<TypeInformation, bool>>? ResolutionConditions { get; set; }
        internal ExpandableArray<Type>? AttributeConditions { get; set; }

        internal ComplexRegistration(ServiceRegistration baseRegistration)
            : base(baseRegistration.ImplementationType,
                  baseRegistration.Name,
                  baseRegistration.Lifetime,
                  baseRegistration.RegistrationId,
                  baseRegistration.RegistrationOrder,
                  baseRegistration.IsDecorator)
        { }

        internal ComplexRegistration(Type implementationType, object? name, ServiceRegistration baseRegistration)
            : base(implementationType,
                  name,
                  baseRegistration.Lifetime,
                  baseRegistration.IsDecorator)
        { }

        internal bool IsUsableForCurrentContext(TypeInformation typeInfo) =>
            HasParentTypeConditionAndMatch(typeInfo) ||
            HasAttributeConditionAndMatch(typeInfo) ||
            HasResolutionConditionAndMatch(typeInfo);

        private bool HasParentTypeConditionAndMatch(TypeInformation typeInfo) =>
            TargetTypeConditions != null &&
            typeInfo.ParentType != null &&
            TargetTypeConditions.Contains(typeInfo.ParentType);

        private bool HasAttributeConditionAndMatch(TypeInformation typeInfo) =>
            AttributeConditions != null &&
            typeInfo.CustomAttributes != null &&
            AttributeConditions.Intersect(typeInfo.CustomAttributes.Select(attribute => attribute.GetType())).Any();

        private bool HasResolutionConditionAndMatch(TypeInformation typeInfo)
        {
            if (ResolutionConditions == null)
                return false;

            var length = ResolutionConditions.Length;
            for (var i = 0; i < length; i++)
            {
                if (ResolutionConditions[i](typeInfo))
                    return true;
            }

            return false;
        }
    }
}
