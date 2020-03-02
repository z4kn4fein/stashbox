using Stashbox.Configuration;
using Stashbox.Registration;
using System;
using System.Reflection;

namespace Stashbox.Entity
{
    /// <summary>
    /// Represents information about a member.
    /// </summary>
    public class MemberInformation
    {
        /// <summary>
        /// Stores the reflected member information.
        /// </summary>
        public MemberInfo MemberInfo { get; set; }

        /// <summary>
        /// Stores the type information about the member.
        /// </summary>
        public TypeInformation TypeInformation { get; set; }

        /// <summary>
        /// Determines that the member is injectable in the current context.
        /// </summary>
        /// <param name="configuration">The container configuration to determine that the container allows the auto injection or not.</param>
        /// <param name="contextData">The registration context to determine that the registration allows the auto injection or not.</param>
        /// <returns>True if the member is injectable, otherwise false.</returns>
        public bool CanInject(ContainerConfiguration configuration, RegistrationContext contextData)
        {
            var autoMemberInjectionEnabled = configuration.MemberInjectionWithoutAnnotationEnabled || contextData.AutoMemberInjectionEnabled;
            var autoMemberInjectionRule = contextData.AutoMemberInjectionEnabled ? contextData.AutoMemberInjectionRule :
                configuration.MemberInjectionWithoutAnnotationRule;

            if (autoMemberInjectionEnabled)
                return this.TypeInformation.ForcedDependency ||
                       this.TypeInformation.MemberType == MemberType.Field &&
                           (autoMemberInjectionRule & Rules.AutoMemberInjectionRules.PrivateFields) == Rules.AutoMemberInjectionRules.PrivateFields ||
                       this.TypeInformation.MemberType == MemberType.Property &&
                           ((autoMemberInjectionRule & Rules.AutoMemberInjectionRules.PropertiesWithPublicSetter) == Rules.AutoMemberInjectionRules.PropertiesWithPublicSetter &&
                           ((PropertyInfo)this.MemberInfo).HasSetMethod() ||
                           (autoMemberInjectionRule & Rules.AutoMemberInjectionRules.PropertiesWithLimitedAccess) == Rules.AutoMemberInjectionRules.PropertiesWithLimitedAccess);

            return this.TypeInformation.ForcedDependency;
        }

        internal MemberInformation Clone()
        {
            return new MemberInformation { MemberInfo = this.MemberInfo, TypeInformation = this.TypeInformation.Clone() };
        }
    }
}
