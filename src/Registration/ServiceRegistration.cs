using Stashbox.Configuration;
using Stashbox.Lifetime;
using Stashbox.Resolution;
using Stashbox.Utils.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace Stashbox.Registration
{
    /// <summary>
    /// Represents a service registration.
    /// </summary>
    public class ServiceRegistration
    {
        private static int GlobalRegistrationId = int.MinValue;

        private static int GlobalRegistrationOrder = int.MinValue;

        /// <summary>
        /// The registration id.
        /// </summary>
        public readonly int RegistrationId;

        /// <summary>
        /// True if the registration is a decorator.
        /// </summary>
        public readonly bool IsDecorator;

        /// <summary>
        /// Name of the registration.
        /// </summary>
        public object? Name { get; internal set; }

        /// <summary>
        /// Lifetime of the registration.
        /// </summary>
        public LifetimeDescriptor Lifetime { get; internal set; }

        /// <summary>
        /// The registration order indicator.
        /// </summary>
        public int RegistrationOrder { get; private set; }

        /// <summary>
        /// The implementation type.
        /// </summary>
        public Type ImplementationType { get; internal set; }

        /// <summary>
        /// Advanced options.
        /// </summary>
        public Dictionary<byte, object?>? Options { get; internal set; }

        internal ServiceRegistration(Type implementationType, object? name,
            LifetimeDescriptor lifetimeDescriptor, bool isDecorator, Dictionary<byte, object?>? options = null, 
            int? registrationId = null, int? order = null)
        {
            this.ImplementationType = implementationType;
            this.IsDecorator = isDecorator;
            this.Name = name;
            this.Lifetime = lifetimeDescriptor;
            this.Options = options;
            this.RegistrationId = registrationId ?? ReserveRegistrationId();
            this.RegistrationOrder = order ?? ReserveRegistrationOrder();
        }

        internal void Replaces(ServiceRegistration serviceRegistration) =>
            this.RegistrationOrder = serviceRegistration.RegistrationOrder;

        internal bool IsFactory() => Options.GetOrDefault(OptionIds.RegistrationTypeOptions) is FactoryOptions;

        internal bool IsInstance() => Options.GetOrDefault(OptionIds.RegistrationTypeOptions) is InstanceOptions;

        internal bool IsUsableForCurrentContext(TypeInformation typeInfo, ConditionOptions conditionOptions) =>
            HasParentTypeConditionAndMatch(typeInfo, conditionOptions) ||
            HasAttributeConditionAndMatch(typeInfo, conditionOptions) ||
            HasResolutionConditionAndMatch(typeInfo, conditionOptions);

        private bool HasParentTypeConditionAndMatch(TypeInformation typeInfo, ConditionOptions conditionOptions) =>
            conditionOptions.TargetTypeConditions != null &&
            typeInfo.ParentType != null &&
            conditionOptions.TargetTypeConditions.Contains(typeInfo.ParentType);

        private bool HasAttributeConditionAndMatch(TypeInformation typeInfo, ConditionOptions conditionOptions) =>
            conditionOptions.AttributeConditions != null &&
            typeInfo.CustomAttributes != null &&
            conditionOptions.AttributeConditions.Intersect(typeInfo.CustomAttributes.Select(attribute => attribute.GetType())).Any();

        private bool HasResolutionConditionAndMatch(TypeInformation typeInfo, ConditionOptions conditionOptions)
        {
            if (conditionOptions.ResolutionConditions == null)
                return false;
            var length = conditionOptions.ResolutionConditions.Length;
            for (var i = 0; i < length; i++)
            {
                if (conditionOptions.ResolutionConditions[i](typeInfo))
                    return true;
            }
            return false;
        }

        private static int ReserveRegistrationId() =>
            Interlocked.Increment(ref GlobalRegistrationId);

        private static int ReserveRegistrationOrder() =>
            Interlocked.Increment(ref GlobalRegistrationOrder);

    }

    internal static class OptionIds
    {
        public const byte IsResolutionCallRequired = 0;
        public const byte ConstructorOptions = 1;
        public const byte AutoMemberOptions = 2;
        public const byte DependencyBindings = 3;
        public const byte Finalizer = 4;
        public const byte Initializer = 5;
        public const byte AsyncInitializer = 6;
        public const byte IsLifetimeExternallyOwned = 7;
        public const byte DefinedScopeName = 8;
        public const byte ConstructorSelectionRule = 9;
        public const byte Metadata = 10;
        public const byte ReplaceExistingRegistration = 11;
        public const byte ReplaceExistingRegistrationOnlyIfExists = 12;
        public const byte AdditionalServiceTypes = 13;
        public const byte InjectionParameters = 14;
        public const byte ConditionOptions = 15;
        public const byte RegistrationTypeOptions = 16;
    }

    internal class FactoryOptions
    {
        public readonly Delegate Factory;
        public readonly Type[] FactoryParameters;
        public readonly bool IsFactoryDelegateACompiledLambda;

        internal FactoryOptions(Delegate factory, Type[] factoryParameters, bool isCompiledLambda)
        {
            this.Factory = factory;
            this.FactoryParameters = factoryParameters;
            this.IsFactoryDelegateACompiledLambda = isCompiledLambda;
        }
    }

    internal class InstanceOptions
    {
        public readonly bool IsWireUp;
        public readonly object ExistingInstance;

        internal InstanceOptions(object existingInstance, bool isWireUp)
        {
            this.IsWireUp = isWireUp;
            this.ExistingInstance = existingInstance;
        }
    }

    internal class AutoMemberOptions
    {
        public readonly Rules.AutoMemberInjectionRules AutoMemberInjectionRule;
        public readonly Func<MemberInfo, bool>? AutoMemberInjectionFilter;

        internal AutoMemberOptions(Rules.AutoMemberInjectionRules autoMemberInjectionRule, Func<MemberInfo, bool>? autoMemberInjectionFilter)
        {
            this.AutoMemberInjectionRule = autoMemberInjectionRule;
            this.AutoMemberInjectionFilter = autoMemberInjectionFilter;
        }
    }

    internal class ConstructorOptions
    {
        public readonly ConstructorInfo SelectedConstructor;

        public readonly object[]? ConstructorArguments;

        public ConstructorOptions(ConstructorInfo selectedConstructor, object[]? constructorArguments)
        {
            this.SelectedConstructor = selectedConstructor;
            this.ConstructorArguments = constructorArguments;
        }
    }

    internal class ConditionOptions
    {
        internal ExpandableArray<Type>? TargetTypeConditions { get; set; }
        internal ExpandableArray<Func<TypeInformation, bool>>? ResolutionConditions { get; set; }
        internal ExpandableArray<Type>? AttributeConditions { get; set; }
    }
}
