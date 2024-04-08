using Xunit;

namespace Stashbox.Tests.IssueTests;

public class GenericDecoratorsBroken 
{
    [Fact]
    public void Ensure_GenericDecorators_Working()
    {
        using var container = new StashboxContainer();
        container.RegisterSingleton(typeof(ICommand<Context>), typeof(Command<Context>));
        container.RegisterDecorator(typeof(ICommand<Context>), typeof(Decorator<Context>));

        var decorator = container.Resolve<ICommand<Context>>();
        
        Assert.IsType<Decorator<Context>>(decorator);
        Assert.IsType<Command<Context>>(((Decorator<Context>)decorator).Dep);
    }
    
    [Fact]
    public void Ensure_GenericDecorators_Working_Open()
    {
        using var container = new StashboxContainer();
        container.RegisterSingleton(typeof(ICommand<Context>), typeof(Command<Context>));
        container.RegisterDecorator(typeof(ICommand<>), typeof(Decorator<>));

        var decorator = container.Resolve<ICommand<Context>>();
        
        Assert.IsType<Decorator<Context>>(decorator);
        Assert.IsType<Command<Context>>(((Decorator<Context>)decorator).Dep);
    }

    interface ICommand<T>;
    
    class Command <T> : ICommand<T>;

    class Decorator<T> : ICommand<T>
    {
        public ICommand<T> Dep { get; }

        public Decorator(ICommand<T> dep)
        {
            Dep = dep;
        }
    }
    
    class Context;
}