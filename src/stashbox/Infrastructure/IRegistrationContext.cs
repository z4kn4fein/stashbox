using Stashbox.Entity;
using System;

namespace Stashbox.Infrastructure
{
    public interface IRegistrationContext
    {
        Type TypeFrom { get; }
        Type TypeTo { get; }
        IContainerContext ContainerContext { get; }

        IRegistrationContext WithLifetime(ILifetime lifetime);
        IRegistrationContext WithName(string name);
        IRegistrationContext WithInjectionParameters(params InjectionParameter[] injectionParameters);
        IRegistrationContext WithFactoryParameters(Func<object, object> singleParameterFactory);
        IRegistrationContext WithFactoryParameters(Func<object, object, object> twoParametersFactory);
        IRegistrationContext WithFactoryParameters(Func<object, object, object, object> threeParametersFactory);
        IRegistrationContext WhenDependantIs<TTarget>(string dependencyName = null) where TTarget : class;
        IRegistrationContext WhenDependantIs(Type targetType, string dependencyName = null);
        IRegistrationContext WhenHas<TAttribute>() where TAttribute : Attribute;
        IRegistrationContext WhenHas(Type attributeType);
        IRegistrationContext When(Func<TypeInformation, bool> resolutionCondition);
        IStashboxContainer Register();
        IStashboxContainer ReMap();
    }
}
