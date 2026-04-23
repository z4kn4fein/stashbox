using System;
using System.Linq;
using Stashbox.Attributes;
using Stashbox.Configuration;
using Stashbox.Exceptions;
using Stashbox.Resolution;
using Stashbox.Tests.IssueTests;
using Xunit;

namespace Stashbox.Tests;

public class KeyedTests
{
    private static object UniversalName = new();

    [Fact]
    public void ResolveKeyedService()
    {
        var service1 = new Service();
        var service2 = new Service();
        using var container = new StashboxContainer(config => config
            .WithDisposableTransientTracking()
            .WithRegistrationBehavior(Rules.RegistrationBehavior.PreserveDuplications));
        container.Register<IService>(c => c.WithInstance(service1).WithName("service1").WithSingletonLifetime());
        container.Register<IService>(c => c.WithInstance(service2).WithName("service2").WithSingletonLifetime());

        Assert.Null(container.ResolveOrDefault<IService>());
        Assert.Same(service1, container.ResolveOrDefault<IService>("service1"));
        Assert.Same(service2, container.ResolveOrDefault<IService>("service2"));
    }

    [Fact]
    public void ResolveKeyedOpenGenericService()
    {
        using var container = new StashboxContainer(config => config
            .WithDisposableTransientTracking()
            .WithRegistrationBehavior(Rules.RegistrationBehavior.PreserveDuplications));
        container.Register(typeof(IFakeOpenGenericService<>), typeof(FakeOpenGenericService<>),
            c => c.WithName("my-service"));
        container.RegisterSingleton<IFakeSingletonService, FakeService>();

        // Act
        var genericService = container.ResolveOrDefault<IFakeOpenGenericService<IFakeSingletonService>>("my-service");
        var singletonService = container.ResolveOrDefault<IFakeSingletonService>();

        // Assert
        Assert.Same(singletonService, genericService.Value);
    }

    [Fact]
    public void ResolveKeyedServices()
    {
        var service1 = new Service();
        var service2 = new Service();
        var service3 = new Service();
        var service4 = new Service();
        using var container = new StashboxContainer(config => config
            .WithDisposableTransientTracking()
            .WithRegistrationBehavior(Rules.RegistrationBehavior.PreserveDuplications));
        container.Register<IService>(c => c.WithName("first-service").WithInstance(service1).WithSingletonLifetime());
        container.Register<IService>(c => c.WithName("service").WithInstance(service2).WithSingletonLifetime());
        container.Register<IService>(c => c.WithName("service").WithInstance(service3).WithSingletonLifetime());
        container.Register<IService>(c => c.WithName("service").WithInstance(service4).WithSingletonLifetime());

        var firstSvc = container.ResolveAll<IService>("first-service").ToList();
        Assert.Single(firstSvc);
        Assert.Same(service1, firstSvc[0]);

        var services = container.ResolveAll<IService>("service").ToList();
        Assert.Equal(new[] { service2, service3, service4 }, services);
    }

    [Fact]
    public void ResolveKeyedGenericServices()
    {
        var service1 = new FakeService();
        var service2 = new FakeService();
        var service3 = new FakeService();
        var service4 = new FakeService();
        using var container = new StashboxContainer(config => config
            .WithDisposableTransientTracking()
            .WithRegistrationBehavior(Rules.RegistrationBehavior.PreserveDuplications));
        container.Register<IFakeOpenGenericService<PocoClass>>(c => c.WithName("first-service")
            .WithInstance(service1).WithSingletonLifetime());
        container.Register<IFakeOpenGenericService<PocoClass>>(c => c.WithName("service")
            .WithInstance(service2).WithSingletonLifetime());
        container.Register<IFakeOpenGenericService<PocoClass>>(c => c.WithName("service")
            .WithInstance(service3).WithSingletonLifetime());
        container.Register<IFakeOpenGenericService<PocoClass>>(c => c.WithName("service")
            .WithInstance(service4).WithSingletonLifetime());

        var firstSvc = container.ResolveAll<IFakeOpenGenericService<PocoClass>>("first-service").ToList();
        Assert.Single(firstSvc);
        Assert.Same(service1, firstSvc[0]);

        var services = container.ResolveAll<IFakeOpenGenericService<PocoClass>>("service").ToList();
        Assert.Equal(new[] { service2, service3, service4 }, services);
    }

    [Fact]
    public void ResolveKeyedServiceSingletonInstance()
    {
        var service = new Service();
        using var container = new StashboxContainer(config => config
            .WithDisposableTransientTracking()
            .WithRegistrationBehavior(Rules.RegistrationBehavior.PreserveDuplications));
        container.Register<IService>(c => c.WithName("service1").WithInstance(service).WithSingletonLifetime());

        Assert.Null(container.ResolveOrDefault<IService>());
        Assert.Same(service, container.ResolveOrDefault<IService>("service1"));
    }

    [Fact]
    public void ResolveKeyedServiceSingletonInstanceWithKeyInjection()
    {
        var serviceKey = "this-is-my-service";
        using var container = new StashboxContainer(config => config
            .WithDisposableTransientTracking()
            .WithRegistrationBehavior(Rules.RegistrationBehavior.PreserveDuplications));
        container.RegisterSingleton<IService, Service>(serviceKey);

        Assert.Null(container.ResolveOrDefault<IService>());
        var svc = container.ResolveOrDefault<IService>(serviceKey);
        Assert.NotNull(svc);
        Assert.Equal(serviceKey, svc.ToString());
    }

    [Fact]
    public void ResolveKeyedServiceSingletonInstanceWithAnyKey()
    {
        using var container = new StashboxContainer(config => config
            .WithUniversalName(UniversalName)
            .WithDisposableTransientTracking()
            .WithRegistrationBehavior(Rules.RegistrationBehavior.PreserveDuplications));
        container.RegisterSingleton<IService, Service>(UniversalName);

        Assert.Null(container.ResolveOrDefault<IService>());

        var serviceKey1 = "some-key";
        var svc1 = container.ResolveOrDefault<IService>(serviceKey1);
        Assert.NotNull(svc1);
        Assert.Equal(serviceKey1, svc1.ToString());

        var serviceKey2 = "some-other-key";
        var svc2 = container.ResolveOrDefault<IService>(serviceKey2);
        Assert.NotNull(svc2);
        Assert.Equal(serviceKey2, svc2.ToString());
    }

    [Fact]
    public void ResolveKeyedServicesSingletonInstanceWithAnyKey()
    {
        var service1 = new FakeService();
        var service2 = new FakeService();

        using var container = new StashboxContainer(config => config
            .WithUniversalName(UniversalName)
            .WithDisposableTransientTracking()
            .WithRegistrationBehavior(Rules.RegistrationBehavior.PreserveDuplications));
        container.Register<IFakeOpenGenericService<PocoClass>>(c => c.WithName(UniversalName)
            .WithInstance(service1).WithSingletonLifetime());
        container.Register<IFakeOpenGenericService<PocoClass>>(c => c.WithName("some-key")
            .WithInstance(service2).WithSingletonLifetime());

        var services = container.ResolveAll<IFakeOpenGenericService<PocoClass>>("some-key").ToList();
        Assert.Equal(new[] { service2 }, services);
    }

    [Fact]
    public void ResolveKeyedServiceSingletonInstanceWithKeyedParameter()
    {
        using var container = new StashboxContainer(config => config
            .WithDisposableTransientTracking()
            .WithRegistrationBehavior(Rules.RegistrationBehavior.PreserveDuplications));
        container.RegisterSingleton<IService, Service>("service1");
        container.RegisterSingleton<IService, Service>("service2");
        container.RegisterSingleton<OtherService>();

        Assert.Null(container.ResolveOrDefault<IService>());
        var svc = container.ResolveOrDefault<OtherService>();
        Assert.NotNull(svc);
        Assert.Equal("service1", svc.Service1.ToString());
        Assert.Equal("service2", svc.Service2.ToString());
    }

    [Fact]
    public void ResolveKeyedServiceSingletonInstanceWithKeyedParameterWithAdditionalAttribute()
    {
        using var container = new StashboxContainer(config => config
            .WithAdditionalDependencyAttribute<AdditionalDependencyAttribute>()
            .WithAdditionalDependencyNameAttribute<AdditionalNameAttribute>()
            .WithDisposableTransientTracking()
            .WithRegistrationBehavior(Rules.RegistrationBehavior.PreserveDuplications));
        container.RegisterSingleton<IService, Service2>("service1");
        container.RegisterSingleton<IService, Service2>("service2");
        container.RegisterSingleton<OtherService2>();

        Assert.Null(container.ResolveOrDefault<IService>());
        var svc = container.ResolveOrDefault<OtherService2>();
        Assert.NotNull(svc);
        Assert.Equal("service1", svc.Service1.ToString());
        Assert.Equal("service2", svc.Service2.ToString());
    }

    [Fact]
    public void ResolveKeyedServiceSingletonFactory()
    {
        var service = new Service();
        using var container = new StashboxContainer(config => config
            .WithDisposableTransientTracking()
            .WithRegistrationBehavior(Rules.RegistrationBehavior.PreserveDuplications));
        container.Register<IService>(c => c.WithName("service1")
            .WithFactory(() => service).WithSingletonLifetime());

        Assert.Null(container.ResolveOrDefault<IService>());
        Assert.Same(service, container.ResolveOrDefault<IService>("service1"));
    }

    [Fact]
    public void ResolveKeyedServiceSingletonFactoryWithAnyKey()
    {
        using var container = new StashboxContainer(config => config
            .WithUniversalName(UniversalName)
            .WithDisposableTransientTracking()
            .WithRegistrationBehavior(Rules.RegistrationBehavior.PreserveDuplications));
        container.Register<IService>(c => c.WithName(UniversalName)
            .WithFactory<TypeInformation>(t => new Service((string)t.DependencyName)).WithSingletonLifetime());

        Assert.Null(container.ResolveOrDefault<IService>());

        for (int i = 0; i < 3; i++)
        {
            var key = "service" + i;
            var s1 = container.ResolveOrDefault<IService>(key);
            var s2 = container.ResolveOrDefault<IService>(key);
            Assert.Same(s1, s2);
            Assert.Equal(key, s1.ToString());
        }
    }

    [Fact]
    public void ResolveKeyedServiceSingletonType()
    {
        using var container = new StashboxContainer(config => config
            .WithDisposableTransientTracking()
            .WithRegistrationBehavior(Rules.RegistrationBehavior.PreserveDuplications));
        container.RegisterSingleton<IService, Service>("service1");

        Assert.Null(container.ResolveOrDefault<IService>());
        Assert.Equal(typeof(Service), container.ResolveOrDefault<IService>("service1")!.GetType());
    }

    [Fact]
    public void ResolveKeyedServiceTransientFactory()
    {
        using var container = new StashboxContainer(config => config
            .WithDisposableTransientTracking()
            .WithRegistrationBehavior(Rules.RegistrationBehavior.PreserveDuplications));
        container.Register<IService>(c => c.WithName("service1")
            .WithFactory<TypeInformation>(t => new Service((string)t.DependencyName)));

        Assert.Null(container.ResolveOrDefault<IService>());
        var first = container.ResolveOrDefault<IService>("service1");
        var second = container.ResolveOrDefault<IService>("service1");
        Assert.NotSame(first, second);
        Assert.Equal("service1", first.ToString());
        Assert.Equal("service1", second.ToString());
    }

    [Fact]
    public void ResolveKeyedServiceTransientType()
    {
        using var container = new StashboxContainer(config => config
            .WithDisposableTransientTracking()
            .WithRegistrationBehavior(Rules.RegistrationBehavior.PreserveDuplications));
        container.Register<IService, Service>("service1");

        Assert.Null(container.ResolveOrDefault<IService>());
        var first = container.ResolveOrDefault<IService>("service1");
        var second = container.ResolveOrDefault<IService>("service1");
        Assert.NotSame(first, second);
    }

    [Fact]
    public void ResolveKeyedServiceTransientTypeWithAnyKey()
    {
        using var container = new StashboxContainer(config => config
            .WithUniversalName(UniversalName)
            .WithDisposableTransientTracking()
            .WithRegistrationBehavior(Rules.RegistrationBehavior.PreserveDuplications));
        container.Register<IService, Service>(UniversalName);

        Assert.Null(container.ResolveOrDefault<IService>());
        var first = container.ResolveOrDefault<IService>("service1");
        var second = container.ResolveOrDefault<IService>("service1");
        Assert.NotSame(first, second);
    }

    [Fact]
    public void ResolveKeyedServicesAnyKey()
    {
        var service1 = new Service();
        var service2 = new Service();
        var service3 = new Service();
        var service4 = new Service();
        var service5 = new Service();
        var service6 = new Service();
        using var container = new StashboxContainer(config => config
            .WithUniversalName(UniversalName)
            .WithDisposableTransientTracking()
            .WithRegistrationBehavior(Rules.RegistrationBehavior.PreserveDuplications));
        container.Register<IService, Service>(c =>
            c.WithInstance(service1).WithName("first-service").WithSingletonLifetime());
        container.Register<IService, Service>(c =>
            c.WithInstance(service2).WithName("service").WithSingletonLifetime());
        container.Register<IService, Service>(c =>
            c.WithInstance(service3).WithName("service").WithSingletonLifetime());
        container.Register<IService, Service>(c =>
            c.WithInstance(service4).WithName("service").WithSingletonLifetime());
        container.Register<IService, Service>(c => c.WithInstance(service5).WithName(null).WithSingletonLifetime());
        container.Register<IService, Service>(c => c.WithInstance(service6).WithSingletonLifetime());

        // Return all services registered with a non null key
        var allServices = container.ResolveAll<IService>(UniversalName).ToList();
        Assert.Equal(4, allServices.Count);
        Assert.Equal(new[] { service1, service2, service3, service4 }, allServices);

        // Check again (caching)
        var allServices2 = container.ResolveAll<IService>(UniversalName).ToList();
        Assert.Equal(allServices, allServices2);
    }

    [Fact]
    public void ResolveKeyedServicesAnyKeyWithAnyKeyRegistration()
    {
        var service1 = new Service();
        var service2 = new Service();
        var service3 = new Service();
        var service4 = new Service();
        var service5 = new Service();
        var service6 = new Service();
        using var container = new StashboxContainer(config => config
            .WithUniversalName(UniversalName)
            .WithDisposableTransientTracking()
            .WithRegistrationBehavior(Rules.RegistrationBehavior.PreserveDuplications));
        container.Register<IService, Service>(c => c.WithFactory(() => new Service()).WithName(UniversalName));
        container.Register<IService, Service>(c =>
            c.WithInstance(service1).WithName("first-service").WithSingletonLifetime());
        container.Register<IService, Service>(c =>
            c.WithInstance(service2).WithName("service").WithSingletonLifetime());
        container.Register<IService, Service>(c =>
            c.WithInstance(service3).WithName("service").WithSingletonLifetime());
        container.Register<IService, Service>(c =>
            c.WithInstance(service4).WithName("service").WithSingletonLifetime());
        container.Register<IService, Service>(c => c.WithInstance(service5).WithName(null).WithSingletonLifetime());
        container.Register<IService, Service>(c => c.WithInstance(service6).WithSingletonLifetime());

        _ = container.Resolve<IService>("something-else");
        _ = container.Resolve<IService>("something-else-again");

        // Return all services registered with a non null key, but not the one "created" with KeyedService.AnyKey
        var allServices = container.ResolveAll<IService>(UniversalName).ToList();
        Assert.Equal(5, allServices.Count);
        Assert.Equal([service1, service2, service3, service4], allServices.Skip(1));
    }

    [Fact]
    public void CombinationalRegistration()
    {
        Service service1 = new();
        Service service2 = new();
        Service keyedService1 = new();
        Service keyedService2 = new();
        Service anykeyService1 = new();
        Service anykeyService2 = new();
        Service nullkeyService1 = new();
        Service nullkeyService2 = new();
        using var container = new StashboxContainer(config => config
            .WithUniversalName(UniversalName)
            .WithDisposableTransientTracking()
            .WithNamedDependencyResolutionForUnNamedRequests(false, false)
            .OverrideResolutionFailedExceptionWith<InvalidOperationException>()
            .WithIgnoreServicesWithUniversalNameForUniversalNamedRequests()
            .WithForceThrowWhenNamedDependencyIsNotResolvable()
            .WithRegistrationBehavior(Rules.RegistrationBehavior.PreserveDuplications));
        container.Register<IService, Service>(c => c.WithInstance(service1).WithSingletonLifetime());
        container.Register<IService, Service>(c => c.WithInstance(service2).WithSingletonLifetime());
        container.Register<IService, Service>(c =>
            c.WithInstance(nullkeyService1).WithName(null).WithSingletonLifetime());
        container.Register<IService, Service>(c =>
            c.WithInstance(nullkeyService2).WithName(null).WithSingletonLifetime());
        container.Register<IService, Service>(c =>
            c.WithInstance(anykeyService1).WithName(UniversalName).WithSingletonLifetime());
        container.Register<IService, Service>(c =>
            c.WithInstance(anykeyService2).WithName(UniversalName).WithSingletonLifetime());
        container.Register<IService, Service>(c =>
            c.WithInstance(keyedService1).WithName("keyedService").WithSingletonLifetime());
        container.Register<IService, Service>(c =>
            c.WithInstance(keyedService2).WithName("keyedService").WithSingletonLifetime());

        Assert.Equal(
            [service1, service2, nullkeyService1, nullkeyService2],
            container.ResolveAll<IService>());
        Assert.Equal(nullkeyService2, container.Resolve<IService>());
        Assert.Equal(
            [service1, service2, nullkeyService1, nullkeyService2],
            container.ResolveAll<IService>());
        Assert.Equal(nullkeyService2, container.Resolve<IService>());
        Assert.Equal(
            [keyedService1, keyedService2],
            container.ResolveAll<IService>(UniversalName));
        Assert.Throws<InvalidOperationException>(() => container.Resolve<IService>(UniversalName));
        Assert.Null(container.ResolveOrDefault<IService>(UniversalName));
        Assert.Equal(
            [keyedService1, keyedService2],
            container.ResolveAll<IService>("keyedService"));
        Assert.Equal(keyedService2, container.Resolve<IService>("keyedService"));
    }
    
    [Fact]
    public void ResolveKeyedServiceWithKeyedParameter_MissingRegistration_FirstParameter()
    {
        using var container = new StashboxContainer(config => config
            .WithUniversalName(UniversalName)
            .WithDisposableTransientTracking()
            .WithNamedDependencyResolutionForUnNamedRequests(false, false)
            .OverrideResolutionFailedExceptionWith<InvalidOperationException>()
            .WithIgnoreServicesWithUniversalNameForUniversalNamedRequests()
            .WithForceThrowWhenNamedDependencyIsNotResolvable()
            .WithRegistrationBehavior(Rules.RegistrationBehavior.PreserveDuplications));

        container.RegisterSingleton<OtherService>();
        
        Assert.Null(container.ResolveOrDefault<IService>());
        Assert.Null(container.ResolveOrDefault<OtherService>());
        Assert.Throws<InvalidOperationException>(container.Resolve<OtherService>);
    }
    
    [Fact]
    public void ResolveKeyedServiceWithKeyedParameter_MissingRegistration_SecondParameter()
    {
        using var container = new StashboxContainer(config => config
            .WithUniversalName(UniversalName)
            .WithDisposableTransientTracking()
            .WithNamedDependencyResolutionForUnNamedRequests(false, false)
            .OverrideResolutionFailedExceptionWith<InvalidOperationException>()
            .WithIgnoreServicesWithUniversalNameForUniversalNamedRequests()
            .WithForceThrowWhenNamedDependencyIsNotResolvable()
            .WithRegistrationBehavior(Rules.RegistrationBehavior.PreserveDuplications));

        container.RegisterSingleton<OtherService>();

        container.RegisterSingleton<IService, Service>("service1");

        Assert.Null(container.ResolveOrDefault<IService>());
        Assert.Null(container.ResolveOrDefault<OtherService>());
    }
    
    [Fact]
    public void ResolveKeyedServiceWithKeyedParameter_MissingRegistrationButWithUnkeyedService()
    {
        using var container = new StashboxContainer(config => config
            .WithUniversalName(UniversalName)
            .WithDisposableTransientTracking()
            .WithNamedDependencyResolutionForUnNamedRequests(false, false)
            .OverrideResolutionFailedExceptionWith<InvalidOperationException>()
            .WithIgnoreServicesWithUniversalNameForUniversalNamedRequests()
            .WithForceThrowWhenNamedDependencyIsNotResolvable()
            .WithRegistrationBehavior(Rules.RegistrationBehavior.PreserveDuplications));

        container.RegisterSingleton<OtherService>();
        
        container.RegisterSingleton<IService, Service>();

        Assert.NotNull(container.ResolveOrDefault<IService>());
        Assert.Null(container.ResolveOrDefault<OtherService>());
    }
    
    [Fact]
    public void ResolveKeyedServiceSingletonFactoryWithAnyKeyIgnoreWrongType()
    {
        using var container = new StashboxContainer(config => config
            .WithUniversalName(UniversalName)
            .WithDisposableTransientTracking()
            .WithNamedDependencyResolutionForUnNamedRequests(false, false)
            .OverrideResolutionFailedExceptionWith<InvalidOperationException>()
            .WithIgnoreServicesWithUniversalNameForUniversalNamedRequests()
            .WithForceThrowWhenNamedDependencyIsNotResolvable()
            .WithRegistrationBehavior(Rules.RegistrationBehavior.PreserveDuplications));
        container.Register<IService, ServiceWithIntKey>(UniversalName);

        Assert.Null(container.ResolveOrDefault<IService>());
        Assert.NotNull(container.ResolveOrDefault<IService>(87));
        Assert.Null(container.ResolveOrDefault<IService>(new object()));
    }

    private interface IService;

    private class Service([DependencyName] string id) : IService
    {
        public Service() : this(Guid.NewGuid().ToString())
        {
        }

        public override string ToString() => id;
    }

    private class Service2([AdditionalName] string id) : IService
    {
        public Service2() : this(Guid.NewGuid().ToString())
        {
        }

        public override string ToString() => id;
    }

    private class OtherService(
        [Dependency("service1")] IService service1,
        [Dependency("service2")] IService service2)
    {
        public IService Service1 { get; } = service1;

        public IService Service2 { get; } = service2;
    }

    private class OtherService2(
        [AdditionalDependency("service1")] IService service1,
        [AdditionalDependency("service2")] IService service2)
    {
        public IService Service1 { get; } = service1;

        public IService Service2 { get; } = service2;
    }

    private class ServiceWithIntKey([DependencyName] int id) : IService
    {
        private readonly int id = id;
    }

    private class AdditionalNameAttribute : Attribute;

    private class AdditionalDependencyAttribute : Attribute
    {
        public AdditionalDependencyAttribute(string name)
        {
        }
    }
}