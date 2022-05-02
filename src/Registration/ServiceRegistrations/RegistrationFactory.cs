using System;

namespace Stashbox.Registration.ServiceRegistrations
{
    internal static class RegistrationFactory
    {
        public static FactoryRegistration EnsureFactory(Delegate factory, Type[] factoryParameters, bool isCompiledLambda, ServiceRegistration serviceRegistration)
        {
            if (serviceRegistration is FactoryRegistration factoryRegistration)
                return factoryRegistration;

            return SetComplexRegistrationProperties(serviceRegistration, new FactoryRegistration(factory, factoryParameters, isCompiledLambda, serviceRegistration));
        }

        public static InstanceRegistration EnsureInstance(object existingInstance, bool isWireUp, ServiceRegistration serviceRegistration)
        {
            if (serviceRegistration is InstanceRegistration instanceRegistration)
                return instanceRegistration;

            return SetComplexRegistrationProperties(serviceRegistration, new InstanceRegistration(existingInstance, isWireUp, serviceRegistration));
        }

        public static ComplexRegistration EnsureComplex(ServiceRegistration serviceRegistration)
        {
            if (serviceRegistration is ComplexRegistration complexRegistration)
                return complexRegistration;

            return SetComplexRegistrationProperties(serviceRegistration, new ComplexRegistration(serviceRegistration));
        }

        public static OpenGenericRegistration EnsureOpenGeneric(ServiceRegistration serviceRegistration)
        {
            if (serviceRegistration is OpenGenericRegistration openGenericRegistration)
                return openGenericRegistration;

            return SetComplexRegistrationProperties(serviceRegistration, new OpenGenericRegistration(serviceRegistration));
        }

        public static ServiceRegistration FromOpenGeneric(Type closedGenericType, ServiceRegistration serviceRegistration)
        {
            if (serviceRegistration is ComplexRegistration)
            {
                var result = new ComplexRegistration(closedGenericType, null, serviceRegistration);
                return SetComplexRegistrationProperties(serviceRegistration, result);
            }

            return new ServiceRegistration(closedGenericType, null, serviceRegistration.Lifetime, serviceRegistration.IsDecorator);
        }

        public static TRegistration SetComplexRegistrationProperties<TRegistration>(ServiceRegistration from, TRegistration to)
            where TRegistration : ComplexRegistration
        {
            if (from is ComplexRegistration fromComplex && to is ComplexRegistration toComplex)
            {
                toComplex.SelectedConstructor = fromComplex.SelectedConstructor;
                toComplex.ConstructorArguments = fromComplex.ConstructorArguments;
                toComplex.DependencyBindings = fromComplex.DependencyBindings;
                toComplex.Finalizer = fromComplex.Finalizer;
                toComplex.Initializer = fromComplex.Initializer;
                toComplex.AsyncInitializer = fromComplex.AsyncInitializer;
                toComplex.AutoMemberInjectionRule = fromComplex.AutoMemberInjectionRule;
                toComplex.AutoMemberInjectionEnabled = fromComplex.AutoMemberInjectionEnabled;
                toComplex.IsLifetimeExternallyOwned = fromComplex.IsLifetimeExternallyOwned;
                toComplex.DefinedScopeName = fromComplex.DefinedScopeName;
                toComplex.ConstructorSelectionRule = fromComplex.ConstructorSelectionRule;
                toComplex.AutoMemberInjectionFilter = fromComplex.AutoMemberInjectionFilter;
                toComplex.Metadata = fromComplex.Metadata;
                toComplex.TargetTypeConditions = fromComplex.TargetTypeConditions;
                toComplex.ResolutionConditions = fromComplex.ResolutionConditions;
                toComplex.AttributeConditions = fromComplex.AttributeConditions;
                toComplex.ReplaceExistingRegistration = fromComplex.ReplaceExistingRegistration;
                toComplex.ReplaceExistingRegistrationOnlyIfExists = fromComplex.ReplaceExistingRegistrationOnlyIfExists;
                toComplex.IsResolutionCallRequired = fromComplex.IsResolutionCallRequired;
                toComplex.AdditionalServiceTypes = fromComplex.AdditionalServiceTypes;
                toComplex.InjectionParameters = fromComplex.InjectionParameters;
            }

            return to;
        }
    }
}
