using System;

namespace Stashbox.Infrastructure
{
    public interface IDependencyRegistrator
    {
        IRegistrationContext PrepareType<TFrom, TTo>()
            where TFrom : class
            where TTo : class, TFrom;

        IRegistrationContext PrepareType<TFrom>(Type typeTo)
            where TFrom : class;

        IRegistrationContext PrepareType(Type typeFrom, Type typeTo);

        IRegistrationContext PrepareType<TTo>()
             where TTo : class;

        IDependencyRegistrator RegisterType<TFrom, TTo>(string name = null)
            where TFrom : class
            where TTo : class, TFrom;

        IDependencyRegistrator RegisterType<TFrom>(Type typeTo, string name = null)
            where TFrom : class;

        IDependencyRegistrator RegisterType(Type typeFrom, Type typeTo, string name = null);

        IDependencyRegistrator RegisterType<TTo>(string name = null)
             where TTo : class;

        IDependencyRegistrator ReMap<TFrom, TTo>(string name = null)
            where TFrom : class
            where TTo : class, TFrom;

        IDependencyRegistrator ReMap<TFrom>(Type typeTo, string name = null)
            where TFrom : class;

        IDependencyRegistrator ReMap(Type typeFrom, Type typeTo, string name = null);

        IDependencyRegistrator ReMap<TTo>(string name = null)
             where TTo : class;

        IDependencyRegistrator RegisterInstance<TFrom>(object instance, string name = null)
            where TFrom : class;

        IDependencyRegistrator RegisterInstance(object instance, Type type = null, string name = null);

        IDependencyRegistrator BuildUp<TFrom>(object instance, string name = null)
            where TFrom : class;

        IDependencyRegistrator BuildUp(object instance, Type type = null, string name = null);
    }
}
