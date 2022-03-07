using Stashbox.Configuration;
using Stashbox.Lifetime;
using Stashbox.Resolution;
using Stashbox.Utils.Data;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Stashbox.Registration
{
    internal class RegistrationContext
    {
        public object? Name;
        public Delegate? Factory;
        public Type[]? FactoryParameters;
        public bool IsFactoryDelegateACompiledLambda;
        public ConstructorInfo? SelectedConstructor;
        public object[]? ConstructorArguments;
        public LifetimeDescriptor? Lifetime;
        public Dictionary<object, object?>? DependencyBindings;
        public object? ExistingInstance;
        public Action<object>? Finalizer;
        public Delegate? Initializer;
        public Func<object, IDependencyResolver, CancellationToken, Task>? AsyncInitializer;
        public Rules.AutoMemberInjectionRules AutoMemberInjectionRule;
        public bool AutoMemberInjectionEnabled;
        public bool IsLifetimeExternallyOwned;
        public object? DefinedScopeName;
        public bool IsWireUp;
        public Func<IEnumerable<ConstructorInfo>, IEnumerable<ConstructorInfo>>? ConstructorSelectionRule;
        public Func<MemberInfo, bool>? AutoMemberInjectionFilter;
        public object? Metadata;
        public ExpandableArray<Type>? TargetTypeConditions;
        public ExpandableArray<Func<TypeInformation, bool>>? ResolutionConditions;
        public ExpandableArray<Type>? AttributeConditions;
        public bool ReplaceExistingRegistration;
        public bool ReplaceExistingRegistrationOnlyIfExists;
        public ExpandableArray<Type>? AdditionalServiceTypes;
        public ExpandableArray<KeyValuePair<string, object?>>? InjectionParameters;
    }
}
