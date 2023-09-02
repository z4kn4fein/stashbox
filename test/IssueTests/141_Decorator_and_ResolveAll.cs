using System.Collections.Generic;
using System.Linq;
using Stashbox.Configuration;
using Xunit;

namespace Stashbox.Tests.IssueTests;

public class DecoratorAndResolveAll 
{
    [Fact]
    public void Ensure_DecoratorAndResolveAll_Works()
    {
        var container = new StashboxContainer(c =>
        {
            c.WithRegistrationBehavior(Rules.RegistrationBehavior.PreserveDuplications);
        });

        container.Register<AnimalRepository>(c => c.WithSingletonLifetime().AsImplementedTypes());
        container.Register<CarRepository>(c => c.WithSingletonLifetime().AsImplementedTypes());
        container.Register<PhoneRepository>(c => c.WithSingletonLifetime().AsImplementedTypes());
        container.Register<HouseRepository>(c => c.WithSingletonLifetime().AsImplementedTypes());


        container.RegisterDecorator<CachedAnimalRepository>(c => c.AsImplementedTypes());
        container.RegisterDecorator<CachedCarRepository>(c => c.AsImplementedTypes());
        container.RegisterDecorator<CachedPhoneRepository>(c => c.AsImplementedTypes());

        container.Register<CollectionOfServices>();

        var services = container.Resolve<CollectionOfServices>().Services.ToArray();
        
        Assert.Equal(4, services.Length);
        Assert.IsType<CachedAnimalRepository>(services[0]);
        Assert.IsType<AnimalRepository>(((CachedAnimalRepository)services[0]).Service);
        Assert.IsType<CachedCarRepository>(services[1]);
        Assert.IsType<CarRepository>(((CachedCarRepository)services[1]).Service);
        Assert.IsType<CachedPhoneRepository>(services[2]);
        Assert.IsType<PhoneRepository>(((CachedPhoneRepository)services[2]).Service);
        Assert.IsType<HouseRepository>(services[3]);
    }
    
    interface IDataRepository
    {
        void CommonMethod();
    }
    
    interface IAnimalRepository : IDataRepository { }

    interface ICarRepository : IDataRepository { }

    interface IPhoneRepository : IDataRepository { }

    interface IHouseRepository : IDataRepository { }

    sealed class AnimalRepository : IAnimalRepository
    {
        public void CommonMethod() {}
    }

    sealed class CarRepository : ICarRepository
    {
        public void CommonMethod() {}
    }

    sealed class PhoneRepository : IPhoneRepository
    {
        public void CommonMethod() {}
    }

    sealed class HouseRepository : IHouseRepository
    {
        public void CommonMethod() {}
    }

    sealed class CachedAnimalRepository : IAnimalRepository
    {
        public IAnimalRepository Service { get; }

        public CachedAnimalRepository(IAnimalRepository service)
        {
            Service = service;
        }
        
        public void CommonMethod() {}
    }

    sealed class CachedCarRepository : ICarRepository
    {
        public ICarRepository Service { get; }

        public CachedCarRepository(ICarRepository service)
        {
            Service = service;
        }
        
        public void CommonMethod() {}
    }

    sealed class CachedPhoneRepository : IPhoneRepository
    {
        public IPhoneRepository Service { get; }

        public CachedPhoneRepository(IPhoneRepository service)
        {
            Service = service;
        }
        
        public void CommonMethod() {}
    }

    sealed class CollectionOfServices
    {
        public IEnumerable<IDataRepository> Services { get; }

        public CollectionOfServices(IEnumerable<IDataRepository> services)
        {
            Services = services;
        }
    }
}