//using cspLib.PCL.Ioc.Build;
//using cspLib.PCL.Ioc.Overrides;
//using cspLib.PCL.Ioc.Registration;
//using System;

//namespace cspLib.PCL.Ioc.Infrastructure
//{
//    public static class ServiceRegistrationExtensions
//    {
//        public static ServiceRegistration ConfigureLifeTime(this ServiceRegistration registration, ILifeTimeManager manager)
//        {
//            registration.Context.SetLifeTime(manager);
//            return registration;
//        }

//        public static ServiceRegistration WithFactoryResolver(this ServiceRegistration registration, Func<object> factory)
//        {
//            registration.Context.SetObjectBuilder(new FactoryObjectBuilder(factory));
//            return registration;
//        }

//        public static ServiceRegistration WithFactoryResolver(this ServiceRegistration registration, Func<object, object> factory)
//        {
//            registration.Context.SetObjectBuilder(new FactoryObjectBuilder(factory));
//            return registration;
//        }

//        public static ServiceRegistration WithFactoryResolver(this ServiceRegistration registration, Func<object, object, object> factory)
//        {
//            registration.Context.SetObjectBuilder(new FactoryObjectBuilder(factory));
//            return registration;
//        }

//        public static ServiceRegistration WithFactoryResolver(this ServiceRegistration registration, Func<object, object, object, object> factory)
//        {
//            registration.Context.SetObjectBuilder(new FactoryObjectBuilder(factory));
//            return registration;
//        }

//        public static ServiceRegistration WithFactoryResolver(this ServiceRegistration registration, Func<object, object, object, object, object> factory)
//        {
//            registration.Context.SetObjectBuilder(new FactoryObjectBuilder(factory));
//            return registration;
//        }

//        public static void Register(this ServiceRegistration registration)
//        {
//            registration.Context.Container.ServiceRegistrations
//                .AddRegistration(registration.Context.MetaInfoProvider.KeyType, registration, registration.Name);
//        }

//        public static ServiceRegistration WithInjectionConstructor(this ServiceRegistration registration, params object[] parameters)
//        {
//            registration.Context.InjectionInfo.SetInjectionConstructor(parameters);
//            return registration;
//        }

//        public static ServiceRegistration WithInjectionConstructor<T>(this ServiceRegistration registration, T parameter)
//        {
//            registration.Context.InjectionInfo.SetInjectionConstructor<T>(parameter);
//            return registration;
//        }

//        public static ServiceRegistration WithInjectionConstructor<T1, T2>(this ServiceRegistration registration, T1 parameter1, T2 parameter2)
//        {
//            registration.Context.InjectionInfo.SetInjectionConstructor<T1, T2>(parameter1, parameter2);
//            return registration;
//        }

//        public static ServiceRegistration WithInjectionConstructor<T1, T2, T3>(this ServiceRegistration registration, T1 parameter1, T2 parameter2, T3 parameter3)
//        {
//            registration.Context.InjectionInfo.SetInjectionConstructor<T1, T2, T3>(parameter1, parameter2, parameter3);
//            return registration;
//        }

//        public static ServiceRegistration WithInjectionOverrides(this ServiceRegistration registration, params Override[] overrides)
//        {
//            registration.Context.InjectionInfo.AddOverrides(overrides);
//            return registration;
//        }

//        public static ServiceRegistration ConfigureBuildExtension(this ServiceRegistration registration, IBuildExtension extension)
//        {
//            registration.Context.ExtensionManager.AddBuildExtension(extension);
//            return registration;
//        }
//    }
//}