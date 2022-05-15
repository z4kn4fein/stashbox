using Stashbox;
using System.Reflection;
using System.Runtime.CompilerServices;

try
{
    new A(new B());
    Console.WriteLine(Runner.Run());
}
catch (TypeLoadException ex)
{
    Console.WriteLine(ex.TypeName);
    Console.WriteLine(ex.Message);
}

class A
{
    public A(B b)
    {

    }
}

class B
{
    public B()
    {

    }
}

class Runner
{
    [MethodImpl((short)MethodImplAttributes.NoInlining)]
    public static object? Run()
    {
        IStashboxContainer container = new StashboxContainer();
        container.RegisterScoped<A>()
            .RegisterScoped<A>("A")
            .Register<B>("B")
            .Register<B>()
            .RegisterSingleton<B>()
            .RegisterSingleton<B>("BB")
            .Register(typeof(A))
            .Register(typeof(A), c => { })
            .Register<A>(typeof(A))
            .Register<A>(typeof(A), c => { })
            .Register<A, A>()
            .Register<A, A>(c => { })
            .Register<A>(c => { })
            .RegisterDecorator<A>()
            .RegisterDecorator<A>(c => { });
        
        container.Resolve<A>();
        container.Resolve<A>("A");
        container.Resolve(typeof(A));
        container.Resolve(typeof(A), "A");
        container.ResolveOrDefault<A>();
        container.ResolveOrDefault<A>("A");
        container.ResolveOrDefault(typeof(A));
        container.ResolveOrDefault(typeof(A), "A");
        container.ResolveAll<A>();
        container.ResolveAll<A>("A");
        container.ResolveAll(typeof(A));
        container.ResolveAll(typeof(A), "A");

        IDependencyResolver scope = container.BeginScope();
        scope.Resolve<A>();
        scope.Resolve<A>("A");
        scope.Resolve(typeof(A));
        scope.Resolve(typeof(A), "A");
        scope.ResolveOrDefault<A>();
        scope.ResolveOrDefault<A>("A");
        scope.ResolveOrDefault(typeof(A));
        scope.ResolveOrDefault(typeof(A), "A");
        scope.ResolveAll<A>();
        scope.ResolveAll<A>("A");
        scope.ResolveAll(typeof(A));
        scope.ResolveAll(typeof(A), "A");

        return scope.Resolve<A>();
    }
}
