using Stashbox.Exceptions;
using System;
using Xunit;

namespace Stashbox.Tests.IssueTests;

public class UnknownTypeResolutionDoesNotWork
{
    [Fact]
    public void Ensure_Unknown_Type_Resolution_Works_With_Interface()
    {
        using var container = new StashboxContainer(c => c.WithUnknownTypeResolution(config =>
        {
            if (config.ServiceType == typeof(ITest))
                config.SetImplementationType(typeof(Test));
        }));

        var inst = container.Resolve<Test1>();

        Assert.NotNull(inst);
        Assert.NotNull(inst.Test);

    }

    [Fact]
    public void Unknown_Type_Resolution_With_Interface_Bad_Implementation()
    {
        Assert.Throws<ArgumentException>(() =>
        {
            using var container = new StashboxContainer(c => c.WithUnknownTypeResolution(config =>
            {
                if (config.ServiceType == typeof(ITest))
                    config.SetImplementationType(typeof(object));
            }));

            container.Resolve<Test1>();
        });

    }

    [Fact]
    public void Ensures_Registration_Validation_Works()
    {
        using var container = new StashboxContainer();
        Assert.Throws<InvalidRegistrationException>(() => container.Register<ITest>());
        Assert.Throws<InvalidRegistrationException>(() => container.Register<ITest>(typeof(Test1)));
    }

    [Fact]
    public void Ensures_Unknown_Registration_Does_Not_Activate_When_Unresolvable_And_Null_Enabled()
    {
        using var container = new StashboxContainer(c => c.WithUnknownTypeResolution(config => { }));
        Assert.Null(container.ResolveOrDefault<ITest>());
    }

    [Fact]
    public void Ensures_Unknown_Registration_Activate_When_Resolvable_And_Null_Enabled()
    {
        using var container = new StashboxContainer(c => c.WithUnknownTypeResolution(config =>
        {
            if (config.ServiceType == typeof(ITest))
                config.SetImplementationType(typeof(Test));
        }));
        Assert.NotNull(container.ResolveOrDefault<ITest>());
    }

    interface ITest
    { }

    class Test : ITest
    { }

    class Test1
    {
        public Test1(ITest test)
        {
            Test = test;
        }

        public ITest Test { get; }
    }
}