using Stashbox.Exceptions;
using Xunit;

namespace Stashbox.Tests;

public class ConstructorSelectionTests
{
    [Fact]
    public void ConstructorSelectionTests_ArgTypes()
    {
        using var container = new StashboxContainer(config => config.WithUnknownTypeResolution());
        container.Register<Test>(context => context.WithConstructorByArgumentTypes(typeof(Dep), typeof(Dep2)));
        Assert.NotNull(container.Resolve<Test>());
    }

    [Fact]
    public void ConstructorSelectionTests_Args()
    {
        using var container = new StashboxContainer();
        var dep = new Dep();
        var dep2 = new Dep2();

        container.Register<Test>(context => context.WithConstructorByArguments(dep, dep2));
        var test = container.Resolve<Test>();

        Assert.Same(dep, test.Dep);
        Assert.Same(dep2, test.Dep2);
    }

    [Fact]
    public void ConstructorSelectionTests_ArgTypes_Throws_ResolutionFailed()
    {
        using var container = new StashboxContainer();
        container.Register<Test>(context => context.WithConstructorByArgumentTypes(typeof(Dep), typeof(Dep2)));
        var exception = Assert.Throws<ResolutionFailedException>(() => container.Resolve<Test>());

        Assert.Equal(typeof(Test), exception.Type);
    }

    [Fact]
    public void ConstructorSelectionTests_ArgTypes_Throws_MissingConstructor()
    {
        using var container = new StashboxContainer();
        Assert.Throws<ConstructorNotFoundException>(() => container.Register<Test>(context => context.WithConstructorByArgumentTypes()));
    }

    [Fact]
    public void ConstructorSelectionTests_Args_Throws_MissingConstructor()
    {
        using var container = new StashboxContainer();
        Assert.Throws<ConstructorNotFoundException>(() =>
            container.Register<Test>(context => context.WithConstructorByArguments()));
    }

    [Fact]
    public void ConstructorSelectionTests_Args_Throws_MissingConstructor_OneParam()
    {
        using var container = new StashboxContainer();
        Assert.Throws<ConstructorNotFoundException>(() =>
            container.Register<Test>(context => context.WithConstructorByArguments(new object())));
    }

    [Fact]
    public void ConstructorSelectionTests_Args_Throws_MissingConstructor_MoreParams()
    {
        using var container = new StashboxContainer();
        Assert.Throws<ConstructorNotFoundException>(() =>
            container.Register<Test>(context =>
                context.WithConstructorByArguments(new object(), new object())));
    }

    [Fact]
    public void ConstructorSelectionTests_Decorator_ArgTypes()
    {
        using var container = new StashboxContainer(config => config.WithUnknownTypeResolution());
        container.RegisterDecorator<Dep, DepDecorator>(context => context.WithConstructorByArgumentTypes(typeof(Dep), typeof(Dep2)));
        var test = container.Resolve<Dep>();

        Assert.IsType<DepDecorator>(test);
    }

    [Fact]
    public void ConstructorSelectionTests_Decorator_Args()
    {
        using var container = new StashboxContainer(config => config.WithUnknownTypeResolution());
        var dep = new Dep();
        var dep2 = new Dep2();

        container.RegisterDecorator<Dep, DepDecorator>(context => context.WithConstructorByArguments(dep, dep2));
        var test = container.Resolve<Dep>();

        Assert.Same(dep, ((DepDecorator)test).Dep);
        Assert.Same(dep2, ((DepDecorator)test).Dep2);
    }

    [Fact]
    public void ConstructorSelectionTests_Arg_ByInterface()
    {
        using var container = new StashboxContainer();
        var arg = new Arg();
        var arg1 = new Arg1();

        container.Register<Test1>(context => context.WithConstructorByArguments(arg, arg1));
        var test = container.Resolve<Test1>();

        Assert.Same(arg, test.PArg);
        Assert.Same(arg1, test.PArg1);
    }

    class Dep
    { }

    class Dep2
    { }

    class Dep3
    { }

    class DepDecorator : Dep
    {
        public Dep Dep { get; }
        public Dep2 Dep2 { get; }

        public DepDecorator(Dep dep)
        {
            Assert.True(false, "wrong constructor");
        }

        public DepDecorator(Dep dep, Dep2 dep2)
        {
            this.Dep = dep;
            this.Dep2 = dep2;
        }

        public DepDecorator(Dep dep, Dep2 dep2, Dep3 dep3)
        {
            Assert.True(false, "wrong constructor");
        }
    }

    class Test
    {
        public Dep Dep { get; }
        public Dep2 Dep2 { get; }

        public Test(Dep dep)
        {
            Assert.True(false, "wrong constructor");
        }

        public Test(Dep dep, Dep2 dep2)
        {
            this.Dep = dep;
            this.Dep2 = dep2;
        }

        public Test(Dep dep, Dep2 dep2, Dep3 dep3)
        {
            Assert.True(false, "wrong constructor");
        }
    }

    interface IArg { }

    interface IArg1 { }

    class Arg : IArg { }

    class Arg1 : IArg1 { }

    class Test1
    {
        public IArg PArg { get; }
        public IArg1 PArg1 { get; }

        public Test1(IArg arg, IArg1 arg1)
        {
            this.PArg = arg;
            this.PArg1 = arg1;
        }
    }
}