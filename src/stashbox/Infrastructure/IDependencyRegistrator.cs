using System;

namespace Stashbox.Infrastructure
{
    public interface IDependencyRegistrator
    {
        void RegisterType<TKey, TValue>(string name = null, ILifetime lifetime = null)
            where TKey : class
            where TValue : class, TKey;

        void RegisterType<TKey>(Type typeTo, string name = null, ILifetime lifetime = null)
            where TKey : class;

        void RegisterType(Type typeTo, Type typeFrom = null, string name = null, ILifetime lifetime = null);

        void RegisterType<TValue>(string name = null, ILifetime lifetime = null)
             where TValue : class;

        void RegisterInstance<TKey>(object instance, string name = null)
            where TKey : class;

        void RegisterInstance(object instance, Type type = null, string name = null);

        void BuildUp<TKey>(object instance, string name = null)
            where TKey : class;

        void BuildUp(object instance, Type type = null, string name = null);
    }
}
