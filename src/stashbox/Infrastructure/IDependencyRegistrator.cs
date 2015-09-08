using Stashbox.Registration;
using System;

namespace Stashbox.Infrastructure
{
    public interface IDependencyRegistrator
    {
        ServiceRegistration CreateRegistration<TKey, TValue>(string name = null)
            where TKey : class
            where TValue : class, TKey;

        ServiceRegistration CreateRegistration<TKey>(Type typeTo, string name = null)
            where TKey : class;

        ServiceRegistration CreateRegistration(Type typeTo, Type typeFrom = null, string name = null);

        ServiceRegistration CreateRegistration<TValue>(string name = null)
             where TValue : class;

        void RegisterType<TKey, TValue>(string name = null)
            where TKey : class
            where TValue : class, TKey;

        void RegisterType<TKey>(Type typeTo, string name = null)
            where TKey : class;

        void RegisterType(Type typeTo, Type typeFrom = null, string name = null);

        void RegisterType<TValue>(string name = null)
             where TValue : class;

        void RegisterInstance<TKey>(object instance, string name = null)
            where TKey : class;

        void RegisterInstance(object instance, Type type = null, string name = null);

        void BuildUp<TKey>(object instance, string name = null)
            where TKey : class;

        void BuildUp(object instance, Type type = null, string name = null);
    }
}
