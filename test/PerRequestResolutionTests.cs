using Stashbox.Configuration;
using Stashbox.Tests.Utils;
using System.Linq;
using Xunit;

namespace Stashbox.Tests;

public class PerRequestResolutionTests
{
    [Theory]
    [ClassData(typeof(CompilerTypeTestData))]
    public void FromScope_PerRequest(CompilerType compilerType)
    {
        using var container = new StashboxContainer(c => c.WithCompiler(compilerType));
        container.Register<IA, A1>(c => c.WithPerRequestLifetime()).Register<B>().Register<C>();

        using var scope = container.BeginScope();
        var c1 = scope.Resolve<C>();
        var c2 = scope.Resolve<C>();
        IA preA = new A2();
        var c3 = scope.Resolve<C>(dependencyOverrides: new[] { preA });

        Assert.IsType<A1>(c1.A);
        Assert.IsType<A1>(c1.B.A);
        Assert.IsType<A1>(c2.A);
        Assert.IsType<A1>(c2.B.A);
        Assert.IsType<A2>(c3.A);
        Assert.IsType<A2>(c3.B.A);

        Assert.Same(c1.A, c1.B.A);
        Assert.Same(c2.A, c2.B.A);
        Assert.NotSame(c1.A, c2.A);

        Assert.Same(c3.A, c3.B.A);
        Assert.Same(c3.A, preA);
    }

    [Theory]
    [ClassData(typeof(CompilerTypeTestData))]
    public void FromContainer_PerRequest(CompilerType compilerType)
    {
        using var container = new StashboxContainer(c => c.WithCompiler(compilerType));
        container.Register<IA, A1>(c => c.WithPerRequestLifetime()).Register<B>().Register<C>();

        var c1 = container.Resolve<C>();
        var c2 = container.Resolve<C>();
        IA preA = new A2();
        var c3 = container.Resolve<C>(dependencyOverrides: new[] { preA });

        Assert.IsType<A1>(c1.A);
        Assert.IsType<A1>(c1.B.A);
        Assert.IsType<A1>(c2.A);
        Assert.IsType<A1>(c2.B.A);
        Assert.IsType<A2>(c3.A);
        Assert.IsType<A2>(c3.B.A);

        Assert.Same(c1.A, c1.B.A);
        Assert.Same(c2.A, c2.B.A);
        Assert.NotSame(c1.A, c2.A);

        Assert.Same(c3.A, c3.B.A);
        Assert.Same(c3.A, preA);
    }

    [Theory]
    [ClassData(typeof(CompilerTypeTestData))]
    public void FromScope_PerRequest_Named(CompilerType compilerType)
    {
        using var container = new StashboxContainer(c => c.WithCompiler(compilerType));
        container.Register<IA, A1>(c => c.WithPerRequestLifetime().WithName("A1"))
            .Register<IA, A2>(c => c.WithPerRequestLifetime().WithName("A2"))
            .Register<B>(c => c.WithDependencyBinding("a", "A1"))
            .Register<C>(c => c.WithName("C").WithDependencyBinding("a", "A1"));

        using var scope = container.BeginScope();
        var c1 = scope.Resolve<C>("C");
        var c2 = scope.Resolve<C>("C");
        IA preA = new A2();
        var c3 = scope.Resolve<C>("C", dependencyOverrides: new[] { preA });

        Assert.IsType<A1>(c1.A);
        Assert.IsType<A1>(c1.B.A);
        Assert.IsType<A1>(c2.A);
        Assert.IsType<A1>(c2.B.A);
        Assert.IsType<A1>(c3.A);
        Assert.IsType<A1>(c3.B.A);

        Assert.Same(c1.A, c1.B.A);
        Assert.Same(c2.A, c2.B.A);
        Assert.NotSame(c1.A, c2.A);

        Assert.Same(c3.A, c3.B.A);
        Assert.NotSame(c3.A, preA);
    }

    [Theory]
    [ClassData(typeof(CompilerTypeTestData))]
    public void FromContainer_PerRequest_Named(CompilerType compilerType)
    {
        using var container = new StashboxContainer(c => c.WithCompiler(compilerType));
        container.Register<IA, A1>(c => c.WithPerRequestLifetime().WithName("A1"))
            .Register<IA, A2>(c => c.WithPerRequestLifetime().WithName("A2"))
            .Register<B>(c => c.WithDependencyBinding("a", "A1"))
            .Register<C>(c => c.WithName("C").WithDependencyBinding("a", "A1"));

        var c1 = container.Resolve<C>("C");
        var c2 = container.Resolve<C>("C");
        IA preA = new A2();
        var c3 = container.Resolve<C>("C", dependencyOverrides: new[] { preA });

        Assert.IsType<A1>(c1.A);
        Assert.IsType<A1>(c1.B.A);
        Assert.IsType<A1>(c2.A);
        Assert.IsType<A1>(c2.B.A);
        Assert.IsType<A1>(c3.A);
        Assert.IsType<A1>(c3.B.A);

        Assert.Same(c1.A, c1.B.A);
        Assert.Same(c2.A, c2.B.A);
        Assert.NotSame(c1.A, c2.A);

        Assert.Same(c3.A, c3.B.A);
        Assert.NotSame(c3.A, preA);
    }

    [Theory]
    [ClassData(typeof(CompilerTypeTestData))]
    public void FromScope_GetService(CompilerType compilerType)
    {
        using var container = new StashboxContainer(c => c.WithCompiler(compilerType));
        container.Register<IA, A1>()
            .Register<B>()
            .Register<C>();

        using var scope = container.BeginScope();

        var c1 = (C)scope.GetService(typeof(C));
        var c2 = (C)scope.GetService(typeof(C));

        Assert.NotSame(c1.A, c1.B.A);
        Assert.NotSame(c2.A, c2.B.A);
        Assert.NotSame(c1.A, c2.A);
    }

    [Theory]
    [ClassData(typeof(CompilerTypeTestData))]
    public void FromScope_GetService_Null(CompilerType compilerType)
    {
        using var container = new StashboxContainer(c => c.WithCompiler(compilerType));
        container.Register<C>();

        using var scope = container.BeginScope();
        Assert.Null(scope.GetService(typeof(C)));
    }


    [Theory]
    [ClassData(typeof(CompilerTypeTestData))]
    public void FromScope_PerRequest_GetService(CompilerType compilerType)
    {
        using var container = new StashboxContainer(c => c.WithCompiler(compilerType));
        container.Register<IA, A1>(c => c.WithPerRequestLifetime())
            .Register<B>()
            .Register<C>();

        using var scope = container.BeginScope();

        var c1 = (C)scope.GetService(typeof(C));
        var c2 = (C)scope.GetService(typeof(C));

        Assert.IsType<A1>(c1.A);
        Assert.IsType<A1>(c1.B.A);
        Assert.IsType<A1>(c2.A);
        Assert.IsType<A1>(c2.B.A);

        Assert.Same(c1.A, c1.B.A);
        Assert.Same(c2.A, c2.B.A);
        Assert.NotSame(c1.A, c2.A);
    }

    [Theory]
    [ClassData(typeof(CompilerTypeTestData))]
    public void FromContainer_GetService(CompilerType compilerType)
    {
        using var container = new StashboxContainer(c => c.WithCompiler(compilerType));
        container.Register<IA, A1>()
            .Register<B>()
            .Register<C>();

        var c1 = (C)container.GetService(typeof(C));
        var c2 = (C)container.GetService(typeof(C));

        Assert.NotSame(c1.A, c1.B.A);
        Assert.NotSame(c2.A, c2.B.A);
        Assert.NotSame(c1.A, c2.A);
    }

    [Theory]
    [ClassData(typeof(CompilerTypeTestData))]
    public void FromContainer_GetService_Null(CompilerType compilerType)
    {
        using var container = new StashboxContainer(c => c.WithCompiler(compilerType));
        container.Register<C>();

        Assert.Null(container.GetService(typeof(C)));
    }


    [Theory]
    [ClassData(typeof(CompilerTypeTestData))]
    public void FromContainer_PerRequest_GetService(CompilerType compilerType)
    {
        using var container = new StashboxContainer(c => c.WithCompiler(compilerType));
        container.Register<IA, A1>(c => c.WithPerRequestLifetime())
            .Register<B>()
            .Register<C>();

        var c1 = (C)container.GetService(typeof(C));
        var c2 = (C)container.GetService(typeof(C));

        Assert.IsType<A1>(c1.A);
        Assert.IsType<A1>(c1.B.A);
        Assert.IsType<A1>(c2.A);
        Assert.IsType<A1>(c2.B.A);

        Assert.Same(c1.A, c1.B.A);
        Assert.Same(c2.A, c2.B.A);
        Assert.NotSame(c1.A, c2.A);
    }

    [Theory]
    [ClassData(typeof(CompilerTypeTestData))]
    public void FromScope_PerRequest_ResolveAll(CompilerType compilerType)
    {
        using var container = new StashboxContainer(c => c.WithCompiler(compilerType));
        container.Register<IA, A1>(c => c.WithPerRequestLifetime())
            .Register<B>()
            .Register<C>();

        using var scope = container.BeginScope();

        var each = scope.ResolveAll(typeof(C)).Cast<C>().ToArray();

        Assert.Same(each[0].A, each[0].B.A);

        IA preA = new A2();

        each = scope.ResolveAll(typeof(C), new[] { preA }).Cast<C>().ToArray();

        Assert.IsType<A2>(each[0].A);
        Assert.Same(each[0].A, each[0].B.A);
    }

    [Theory]
    [ClassData(typeof(CompilerTypeTestData))]
    public void FromScope_PerRequest_ResolveAll_Generic(CompilerType compilerType)
    {
        using var container = new StashboxContainer(c => c.WithCompiler(compilerType));
        container.Register<IA, A1>(c => c.WithPerRequestLifetime())
            .Register<B>()
            .Register<C>();

        using var scope = container.BeginScope();

        var each = scope.ResolveAll<C>().ToArray();

        Assert.Same(each[0].A, each[0].B.A);

        IA preA = new A2();

        each = scope.ResolveAll<C>(new[] { preA }).ToArray();

        Assert.IsType<A2>(each[0].A);
        Assert.Same(each[0].A, each[0].B.A);
    }

    [Theory]
    [ClassData(typeof(CompilerTypeTestData))]
    public void FromContainer_PerRequest_ResolveAll(CompilerType compilerType)
    {
        using var container = new StashboxContainer(c => c.WithCompiler(compilerType));
        container.Register<IA, A1>(c => c.WithPerRequestLifetime())
            .Register<B>()
            .Register<C>();

        var each = container.ResolveAll(typeof(C)).Cast<C>().ToArray();

        Assert.Same(each[0].A, each[0].B.A);

        IA preA = new A2();

        each = container.ResolveAll(typeof(C), new[] { preA }).Cast<C>().ToArray();

        Assert.IsType<A2>(each[0].A);
        Assert.Same(each[0].A, each[0].B.A);
    }

    [Theory]
    [ClassData(typeof(CompilerTypeTestData))]
    public void FromContainer_PerRequest_ResolveAll_Generic(CompilerType compilerType)
    {
        using var container = new StashboxContainer(c => c.WithCompiler(compilerType));
        container.Register<IA, A1>(c => c.WithPerRequestLifetime())
            .Register<B>()
            .Register<C>();

        var each = container.ResolveAll<C>().ToArray();

        Assert.Same(each[0].A, each[0].B.A);

        IA preA = new A2();

        each = container.ResolveAll<C>(new[] { preA }).ToArray();

        Assert.IsType<A2>(each[0].A);
        Assert.Same(each[0].A, each[0].B.A);
    }

    [Theory]
    [ClassData(typeof(CompilerTypeTestData))]
    public void FromScope_PerRequest_Activate(CompilerType compilerType)
    {
        using var container = new StashboxContainer(c => c.WithCompiler(compilerType));
        container.Register<IA, A1>(c => c.WithPerRequestLifetime())
            .Register<B>();

        using var scope = container.BeginScope();

        var c = scope.Activate<C>();

        Assert.IsType<A1>(c.A);
        Assert.IsType<A1>(c.B.A);

        Assert.Same(c.A, c.B.A);

        IA preA = new A2();

        c = scope.Activate<C>(new[] { preA });

        Assert.IsType<A2>(c.A);
        Assert.IsType<A2>(c.B.A);

        Assert.Same(c.A, c.B.A);
    }

    [Theory]
    [ClassData(typeof(CompilerTypeTestData))]
    public void FromContainer_PerRequest_Activate(CompilerType compilerType)
    {
        using var container = new StashboxContainer(c => c.WithCompiler(compilerType));
        container.Register<IA, A1>(c => c.WithPerRequestLifetime())
            .Register<B>();

        var c = container.Activate<C>();

        Assert.IsType<A1>(c.A);
        Assert.IsType<A1>(c.B.A);

        Assert.Same(c.A, c.B.A);

        IA preA = new A2();

        c = container.Activate<C>(new[] { preA });

        Assert.IsType<A2>(c.A);
        Assert.IsType<A2>(c.B.A);

        Assert.Same(c.A, c.B.A);
    }

    interface IA { }

    class A1 : IA { }

    class A2 : IA { }

    class B
    {
        public B(IA a)
        {
            A = a;
        }

        public IA A { get; }
    }

    class C
    {
        public C(B b, IA a)
        {
            B = b;
            A = a;
        }

        public B B { get; }
        public IA A { get; }
    }
}