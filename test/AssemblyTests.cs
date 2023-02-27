using Stashbox.Configuration;
using Stashbox.Lifetime;
using System.Linq;
using TestAssembly;
using Xunit;

namespace Stashbox.Tests;

public class AssemblyTests
{
    [Fact]
    public void LoadTestAssembly()
    {
        using var container = new StashboxContainer()
            .RegisterAssembly(typeof(ITA_T1).Assembly);

        var regs = container.GetRegistrationDiagnostics().ToArray();

        Assert.Equal(34, regs.Length);
    }

    [Fact]
    public void LoadTestAssembly_Just_Interfaces_And_Self()
    {
        using var container = new StashboxContainer()
            .RegisterAssembly(typeof(ITA_T1).Assembly,
                serviceTypeSelector: Rules.ServiceRegistrationFilters.Interfaces);

        var regs = container.GetRegistrationDiagnostics().ToArray();

        Assert.Equal(32, regs.Length);
    }

    [Fact]
    public void LoadTestAssembly_Just_Abstarct_And_Self()
    {
        using var container = new StashboxContainer()
            .RegisterAssembly(typeof(ITA_T1).Assembly,
                serviceTypeSelector: Rules.ServiceRegistrationFilters.AbstractClasses);

        var regs = container.GetRegistrationDiagnostics().ToArray();

        Assert.Equal(18, regs.Length);
    }

    [Fact]
    public void LoadTestAssembly_Just_Abstarct_Without_Self()
    {
        using var container = new StashboxContainer()
            .RegisterAssembly(typeof(ITA_T1).Assembly,
                serviceTypeSelector: Rules.ServiceRegistrationFilters.AbstractClasses,
                registerSelf: false);

        var regs = container.GetRegistrationDiagnostics().ToArray();

        Assert.Equal(2, regs.Length);
    }

    [Fact]
    public void RegistersTests_RegisterAssembly()
    {
        using var container = new StashboxContainer();
        container.RegisterAssemblyContaining<ITA_T1>();

        var regs = container.ContainerContext.RegistrationRepository.GetRegistrationMappings().ToArray();

        Assert.Equal(34, regs.Length);
    }

    [Fact]
    public void RegistersTests_RegisterAssembly_AsSelf()
    {
        using var container = new StashboxContainer();
        container.RegisterAssemblyContaining<ITA_T1>();

        var regs = container.ContainerContext.RegistrationRepository.GetRegistrationMappings().ToArray();

        Assert.Equal(34, regs.Length);
        Assert.Contains(regs, reg => reg.Key == typeof(TA_T1));
    }

    [Fact]
    public void RegistersTests_RegisterAssemblies()
    {
        using var container = new StashboxContainer();

        var assembly1 = typeof(ITA_T1).Assembly;
        var assembly2 = typeof(IStashboxContainer).Assembly;

        container.RegisterAssemblies(new[] { assembly1, assembly2 }, t => t.FullName.Contains("TestAssembly") || t == typeof(StashboxContainer));

        var regs = container.ContainerContext.RegistrationRepository.GetRegistrationMappings();

        Assert.Contains(regs, r => r.Key == typeof(IStashboxContainer));
        Assert.Contains(regs, r => r.Key == typeof(ITA_T1));
    }

    [Fact]
    public void RegistersTests_RegisterAssemblies_AsSelf()
    {
        using var container = new StashboxContainer();

        var assembly1 = typeof(ITA_T1).Assembly;
        var assembly2 = typeof(IStashboxContainer).Assembly;

        container.RegisterAssemblies(new[] { assembly1, assembly2 });

        var regs = container.ContainerContext.RegistrationRepository.GetRegistrationMappings();

        Assert.Contains(regs, r => r.Key == typeof(StashboxContainer));
        Assert.Contains(regs, r => r.Key == typeof(TA_T1));
    }

    [Fact]
    public void RegistersTests_RegisterAssembly_Selector()
    {
        using var container = new StashboxContainer();
        container.RegisterAssemblyContaining<ITA_T1>(type => type == typeof(TA_T1));

        var regs = container.ContainerContext.RegistrationRepository.GetRegistrationMappings();

        Assert.Contains(regs, reg => reg.Key == typeof(ITA_T1));
        Assert.Contains(regs, reg => reg.Key == typeof(TA_T1));
    }

    [Fact]
    public void RegistersTests_RegisterAssembly_Configurator()
    {
        using var container = new StashboxContainer();
        container.RegisterAssemblyContaining<ITA_T1>(configurator: context =>
        {
            if (context.HasServiceType<ITA_T2>())
                context.WithScopedLifetime();
        });

        var regs = container.ContainerContext.RegistrationRepository.GetRegistrationMappings()
            .Where(r => r.Value.Lifetime is ScopedLifetime).ToArray();

        Assert.True(regs.Length > 0);
    }

    [Fact]
    public void RegistersTests_RegisterAssembly_Configurator_AsSelf()
    {
        using var container = new StashboxContainer();
        container.RegisterAssemblyContaining<ITA_T1>(configurator: context =>
        {
            if (context.ImplementationType == typeof(TA_T1))
                context.WithScopedLifetime();
        });

        var regs = container.ContainerContext.RegistrationRepository.GetRegistrationMappings()
            .Where(r => r.Value.Lifetime is ScopedLifetime).ToArray();

        Assert.True(regs.Length > 0);
    }
}