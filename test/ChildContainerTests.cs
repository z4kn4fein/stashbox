using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Stashbox.Configuration;
using Stashbox.Exceptions;
using Stashbox.Resolution;
using Stashbox.Tests.Utils;
using Stashbox.Utils.Data.Immutable;
using Xunit;

namespace Stashbox.Tests;

public class ChildContainerTests
{
    [Theory]
    [ClassData(typeof(CompilerTypeTestData))]
    public void ChildContainerTests_Dispose_Parent_Disposes_Child(CompilerType compilerType)
    {
        var test = new Test();
        IStashboxContainer container = new StashboxContainer(c => c.WithCompiler(compilerType));
        var child = container.CreateChildContainer();
        child.RegisterInstance(test);
        
        container.Dispose();
        container.Dispose();
        
        Assert.True(test.Disposed);
    }
    
    [Theory]
    [ClassData(typeof(CompilerTypeTestData))]
    public void ChildContainerTests_Dispose_Parent_Not_Disposes_Child(CompilerType compilerType)
    {
        var test = new Test();
        IStashboxContainer container = new StashboxContainer(c => c.WithCompiler(compilerType));
        var child = container.CreateChildContainer(attachToParent: false);
        child.RegisterInstance(test);
        
        container.Dispose();
        container.Dispose();
        
        Assert.False(test.Disposed);
        
        child.Dispose();
        child.Dispose();
        
        Assert.True(test.Disposed);
    }
    
#if HAS_ASYNC_DISPOSABLE
    [Theory]
    [ClassData(typeof(CompilerTypeTestData))]
    public async Task ChildContainerTests_Dispose_Parent_Disposes_Child_Async(CompilerType compilerType)
    {
        var test = new Test();
        IStashboxContainer container = new StashboxContainer(c => c.WithCompiler(compilerType));
        var child = container.CreateChildContainer(attachToParent: true);
        child.RegisterInstance(test);
        
        await container.DisposeAsync();
        await container.DisposeAsync();
        
        Assert.True(test.Disposed);
    }
    
    [Theory]
    [ClassData(typeof(CompilerTypeTestData))]
    public async Task ChildContainerTests_Dispose_Parent_Not_Disposes_Child_Async(CompilerType compilerType)
    {
        var test = new Test();
        IStashboxContainer container = new StashboxContainer(c => c.WithCompiler(compilerType));
        var child = container.CreateChildContainer(attachToParent: false);
        child.RegisterInstance(test);
        
        await container.DisposeAsync();
        await container.DisposeAsync();
        
        Assert.False(test.Disposed);
        
        child.Dispose();
        child.Dispose();
        
        Assert.True(test.Disposed);
    }
#endif
    
    [Theory]
    [ClassData(typeof(CompilerTypeTestData))]
    public void ChildContainerTests_ResolveAll_Parent_Current(CompilerType compilerType)
    {
        using var container = new StashboxContainer(c => c.WithCompiler(compilerType)).Register<IT, T1>().Register<IT, T2>();
        var child = container.CreateChildContainer().Register<IT, T3>().Register<IT, T4>();

        Assert.Equal(4, child.ResolveAll<IT>().Count());
        Assert.Equal(2, child.ResolveAll<IT>(name: null, dependencyOverrides: new []{new object()}, ResolutionBehavior.Current).Count());
        Assert.Equal(2, child.ResolveAll<IT>(dependencyOverrides: new []{new object()}, ResolutionBehavior.Current).Count());
        Assert.Equal(2, child.ResolveAll<IT>(name: null, ResolutionBehavior.Current).Count());
        Assert.Equal(2, child.ResolveAll(typeof(IT), name: null, dependencyOverrides: new []{new object()}, ResolutionBehavior.Parent).Count());
    }
    
    [Theory]
    [ClassData(typeof(CompilerTypeTestData))]
    public void ChildContainerTests_ResolveAll_Scope_Parent_Current(CompilerType compilerType)
    {
        using var container = new StashboxContainer(c => c.WithCompiler(compilerType)).Register<IT, T1>().Register<IT, T2>();
        var child = container.CreateChildContainer().Register<IT, T3>().Register<IT, T4>();

        Assert.Equal(4, child.BeginScope().ResolveAll<IT>().Count());
        Assert.Equal(2, child.BeginScope().ResolveAll<IT>(name: null, dependencyOverrides: new []{new object()}, ResolutionBehavior.Current).Count());
        Assert.Equal(2, child.BeginScope().ResolveAll<IT>(dependencyOverrides: new []{new object()}, ResolutionBehavior.Current).Count());
        Assert.Equal(2, child.BeginScope().ResolveAll<IT>(name: null, ResolutionBehavior.Current).Count());
        Assert.Equal(2, child.BeginScope().ResolveAll(typeof(IT), name: null, dependencyOverrides: new []{new object()}, ResolutionBehavior.Parent).Count());
    }
    
    [Theory]
    [ClassData(typeof(CompilerTypeTestData))]
    public void ChildContainerTests_Resolve_Dependency_Parent_Current(CompilerType compilerType)
    {
        using var container = new StashboxContainer(c => c.WithCompiler(compilerType))
            .Register<IT, T1>()
            .Register<IT, T2>()
            .Register<D1>(c => c.WithInitializer((inst, _) => inst.Init("parent")))
            .Register<D2>(c => c.WithInitializer((inst, _) => inst.Init("parent")));
        var child = container.CreateChildContainer()
            .Register<IT, T3>()
            .Register<IT, T4>()
            .Register<D1>(c => c.WithInitializer((inst, _) => inst.Init("child")))
            .Register<D2>(c => c.WithInitializer((inst, _) => inst.Init("child")));

        Assert.IsType<T4>(child.Resolve<D1>().Dep);
        Assert.IsType<T4>(child.Resolve<D1>(ResolutionBehavior.Current).Dep);
        Assert.IsType<T2>(child.Resolve<D1>(ResolutionBehavior.Parent).Dep);
        
        Assert.Equal(4, child.Resolve<D2>().Dep.Count());
        Assert.Equal(2, child.Resolve<D2>(ResolutionBehavior.Current).Dep.Count());
        Assert.Equal(2, child.Resolve<D2>(ResolutionBehavior.Parent).Dep.Count());

        var deps = child.Resolve<D2>().Dep.Select(t => t.GetType()).ToArray();
        Assert.Contains(typeof(T1), deps);
        Assert.Contains(typeof(T2), deps);
        Assert.Contains(typeof(T3), deps);
        Assert.Contains(typeof(T4), deps);
        
        deps = child.Resolve<D2>(ResolutionBehavior.Current).Dep.Select(t => t.GetType()).ToArray();
        Assert.Contains(typeof(T3), deps);
        Assert.Contains(typeof(T4), deps);
        
        deps = child.Resolve<D2>(ResolutionBehavior.Parent).Dep.Select(t => t.GetType()).ToArray();
        Assert.Contains(typeof(T1), deps);
        Assert.Contains(typeof(T2), deps);
        
        Assert.Equal("child", child.Resolve<D1>().ID);
        Assert.Equal("child", child.Resolve<D1>(ResolutionBehavior.Current).ID);
        Assert.Equal("parent", child.Resolve<D1>(ResolutionBehavior.Parent).ID);
        
        Assert.Equal("child", child.Resolve<D2>().ID);
        Assert.Equal("child", child.Resolve<D2>(ResolutionBehavior.Current).ID);
        Assert.Equal("parent", child.Resolve<D2>(ResolutionBehavior.Parent).ID);
    }
    
    [Theory]
    [ClassData(typeof(CompilerTypeTestData))]
    public void ChildContainerTests_Resolve_Decorator_Parent_Current(CompilerType compilerType)
    {
        using var container = new StashboxContainer(c => c.WithCompiler(compilerType))
            .Register<IT, T1>()
            .Register<IT, T2>()
            .RegisterDecorator<IT, T6>(c => c.WithInitializer((inst, _) => inst.Init("parent")));
        var child = container.CreateChildContainer()
            .Register<IT, T3>()
            .Register<IT, T4>()
            .RegisterDecorator<IT, T6>(c => c.WithInitializer((inst, _) => inst.Init("child")));

        Assert.IsType<T6>(((T6)child.Resolve<IT>()).Dep);
        Assert.IsType<T4>(((T6)child.Resolve<IT>(dependencyOverrides: new []{new object()}, ResolutionBehavior.Current)).Dep);
        Assert.IsType<T2>(((T6)child.Resolve<IT>(name: null, dependencyOverrides: new []{new object()}, ResolutionBehavior.Parent)).Dep);
        
        Assert.Equal("child", ((T6)child.Resolve<IT>()).ID);
        Assert.Equal("child", ((T6)child.Resolve<IT>(name: null, ResolutionBehavior.Current)).ID);
        Assert.Equal("parent", ((T6)child.Resolve(typeof(IT), ResolutionBehavior.Parent)).ID);
    }
    
    [Theory]
    [ClassData(typeof(CompilerTypeTestData))]
    public void ChildContainerTests_Resolve_Scope_Decorator_Parent_Current(CompilerType compilerType)
    {
        using var container = new StashboxContainer(c => c.WithCompiler(compilerType))
            .Register<IT, T1>()
            .Register<IT, T2>()
            .RegisterDecorator<IT, T6>(c => c.WithInitializer((inst, _) => inst.Init("parent")));
        var child = container.CreateChildContainer()
            .Register<IT, T3>()
            .Register<IT, T4>()
            .RegisterDecorator<IT, T6>(c => c.WithInitializer((inst, _) => inst.Init("child")));

        Assert.IsType<T6>(((T6)child.BeginScope().Resolve<IT>()).Dep);
        Assert.IsType<T4>(((T6)child.BeginScope().Resolve<IT>(dependencyOverrides: new []{new object()}, ResolutionBehavior.Current)).Dep);
        Assert.IsType<T2>(((T6)child.BeginScope().Resolve<IT>(name: null, dependencyOverrides: new []{new object()}, ResolutionBehavior.Parent)).Dep);
        
        Assert.Equal("child", ((T6)child.BeginScope().Resolve<IT>()).ID);
        Assert.Equal("child", ((T6)child.BeginScope().Resolve<IT>(name: null, ResolutionBehavior.Current)).ID);
        Assert.Equal("parent", ((T6)child.BeginScope().Resolve(typeof(IT), ResolutionBehavior.Parent)).ID);
    }
    
    [Theory]
    [ClassData(typeof(CompilerTypeTestData))]
    public void ChildContainerTests_ResolveOrDefault_Decorator_Parent_Current(CompilerType compilerType)
    {
        using var container = new StashboxContainer(c => c.WithCompiler(compilerType))
            .Register<IT, T1>()
            .Register<IT, T2>()
            .RegisterDecorator<IT, T6>(c => c.WithInitializer((inst, _) => inst.Init("parent")));
        var child = container.CreateChildContainer()
            .Register<IT, T3>()
            .Register<IT, T4>()
            .RegisterDecorator<IT, T6>(c => c.WithInitializer((inst, _) => inst.Init("child")));

        Assert.IsType<T6>(((T6)child.ResolveOrDefault<IT>()).Dep);
        Assert.IsType<T4>(((T6)child.ResolveOrDefault<IT>(dependencyOverrides: new []{new object()}, ResolutionBehavior.Current)).Dep);
        Assert.IsType<T2>(((T6)child.ResolveOrDefault<IT>(name: null, dependencyOverrides: new []{new object()}, ResolutionBehavior.Parent)).Dep);
        
        Assert.Equal("child", ((T6)child.ResolveOrDefault<IT>()).ID);
        Assert.Equal("child", ((T6)child.ResolveOrDefault<IT>(name: null, ResolutionBehavior.Current)).ID);
        Assert.Equal("parent", ((T6)child.ResolveOrDefault(typeof(IT), ResolutionBehavior.Parent)!).ID);
    }
    
    [Theory]
    [ClassData(typeof(CompilerTypeTestData))]
    public void ChildContainerTests_ResolveOrDefault_Scope_Decorator_Parent_Current(CompilerType compilerType)
    {
        using var container = new StashboxContainer(c => c.WithCompiler(compilerType))
            .Register<IT, T1>()
            .Register<IT, T2>()
            .RegisterDecorator<IT, T6>(c => c.WithInitializer((inst, _) => inst.Init("parent")));
        var child = container.CreateChildContainer()
            .Register<IT, T3>()
            .Register<IT, T4>()
            .RegisterDecorator<IT, T6>(c => c.WithInitializer((inst, _) => inst.Init("child")));

        Assert.IsType<T6>(((T6)child.BeginScope().ResolveOrDefault<IT>()).Dep);
        Assert.IsType<T4>(((T6)child.BeginScope().ResolveOrDefault<IT>(dependencyOverrides: new []{new object()}, ResolutionBehavior.Current)).Dep);
        Assert.IsType<T2>(((T6)child.BeginScope().ResolveOrDefault<IT>(name: null, dependencyOverrides: new []{new object()}, ResolutionBehavior.Parent)).Dep);
        
        Assert.Equal("child", ((T6)child.BeginScope().ResolveOrDefault<IT>()).ID);
        Assert.Equal("child", ((T6)child.BeginScope().ResolveOrDefault<IT>(name: null, ResolutionBehavior.Current)).ID);
        Assert.Equal("parent", ((T6)child.BeginScope().ResolveOrDefault(typeof(IT), ResolutionBehavior.Parent)!).ID);
    }
    
    [Theory]
    [ClassData(typeof(CompilerTypeTestData))]
    public void ChildContainerTests_Resolve_Decorator_Child_Service_Parent(CompilerType compilerType)
    {
        using var container = new StashboxContainer(c => c.WithCompiler(compilerType))
            .Register<IT, T1>();
        var child = container.CreateChildContainer()
            .RegisterDecorator<IT, T6>();

        Assert.IsType<T1>(((T6)child.Resolve<IT>()).Dep);
        Assert.IsType<T1>(child.Resolve<IT>(ResolutionBehavior.Parent));
        Assert.Throws<ResolutionFailedException>(() => child.Resolve<IT>(ResolutionBehavior.Current));
    }
    
    [Theory]
    [ClassData(typeof(CompilerTypeTestData))]
    public void ChildContainerTests_Resolve_Decorator_Parent_Service_Child(CompilerType compilerType)
    {
        using var container = new StashboxContainer(c => c.WithCompiler(compilerType))
            .RegisterDecorator<IT, T6>();
        var child = container.CreateChildContainer()
            .Register<IT, T1>();

        Assert.IsType<T1>(((T6)child.Resolve<IT>()).Dep);
        Assert.Throws<ResolutionFailedException>(() => child.Resolve<IT>(ResolutionBehavior.Parent));
        Assert.IsType<T1>(child.Resolve<IT>(ResolutionBehavior.Current));
    }
    
    [Theory]
    [ClassData(typeof(CompilerTypeTestData))]
    public void ChildContainerTests_Resolve_Decorator_Enumerable_Parent_Current(CompilerType compilerType)
    {
        using var container = new StashboxContainer(c => c.WithCompiler(compilerType)).Register<IT, T1>().Register<IT, T2>().RegisterDecorator<IT, T5>();
        var child = container.CreateChildContainer().Register<IT, T3>().Register<IT, T4>().RegisterDecorator<IT, T5>();

        Assert.Equal(4, ((T5)child.Resolve<IT>()).Dep.Count());
        Assert.Equal(2, ((T5)child.Resolve<IT>(ResolutionBehavior.Current)).Dep.Count());
        Assert.Equal(2, ((T5)child.Resolve<IT>(ResolutionBehavior.Parent)).Dep.Count());
        
        var deps = ((T5)child.Resolve<IT>()).Dep.SelectMany(t => ((T5)t).Dep.Select(td => td.GetType())).ToArray();
        Assert.Contains(typeof(T1), deps);
        Assert.Contains(typeof(T2), deps);
        Assert.Contains(typeof(T3), deps);
        Assert.Contains(typeof(T4), deps);
        
        deps = ((T5)child.Resolve<IT>(ResolutionBehavior.Current)).Dep.Select(t => t.GetType()).ToArray();
        Assert.Contains(typeof(T3), deps);
        Assert.Contains(typeof(T4), deps);
        
        deps = ((T5)child.Resolve<IT>(ResolutionBehavior.Parent)).Dep.Select(t => t.GetType()).ToArray();
        Assert.Contains(typeof(T1), deps);
        Assert.Contains(typeof(T2), deps);
    }
    
    [Fact]
    public void ChildContainerTests_Get_NonExisting_Null()
    {
        var container = new StashboxContainer();

        Assert.Null(container.GetChildContainer("A"));
    }

    [Fact]
    public void ChildContainerTests_Configure()
    {
        var container = new StashboxContainer();
        container.Register<IA, A>();

        container.CreateChildContainer("A", c => c.Register<IA, B>());

        Assert.IsType<A>(container.Resolve<IA>());

        var tenant = container.GetChildContainer("A");

        Assert.NotNull(tenant);
        Assert.IsType<B>(tenant?.Resolve<IA>());
    }

    [Fact]
    public void ChildContainerTests_Configure_Dep()
    {
        var container = new StashboxContainer();
        container.Register<D>();
        container.Register<IA, A>();

        container.CreateChildContainer("A", c => c.Register<IA, B>());

        var tenant = container.GetChildContainer("A");

        Assert.IsType<A>(container.Resolve<D>().Ia);
        Assert.IsType<B>(tenant?.Resolve<D>().Ia);
    }

    [Fact]
    public void ChildContainerTests_Configure_Validate_Root_Throws()
    {
        var container = new StashboxContainer();
        container.Register<D>();

        container.CreateChildContainer("A", c => c.Register<IA, B>());

        var exception = Assert.Throws<AggregateException>(() => container.Validate());
        Assert.Single(exception.InnerExceptions);
    }

    [Fact]
    public void ChildContainerTests_Configure_Validate_Root_And_Tenants_Throws()
    {
        var container = new StashboxContainer();
        container.Register<D>();

        container.CreateChildContainer("A", c => c.Register<D>());

        var exception = Assert.Throws<AggregateException>(() => container.Validate());
        Assert.Equal(2, exception.InnerExceptions.Count);
    }

    [Fact]
    public void ChildContainerTests_Configure_Validate_Valid()
    {
        var container = new StashboxContainer();
        container.Register<IA, A>();

        container.CreateChildContainer("A", c => c.Register<D>());

        var exception = Record.Exception(() => container.Validate());
        Assert.Null(exception);
    }

    [Fact]
    public void ChildContainerTests_Dispose()
    {
        var container = new StashboxContainer(c => c.WithDisposableTransientTracking());
        container.Register<IA, C>();

        var tenant = container.CreateChildContainer("C", c => { });

        var inst = (C)tenant.Resolve<IA>();

        container.Dispose();

        Assert.True(inst.Disposed);
        Assert.Throws<ObjectDisposedException>(() => container.Resolve<IA>());
        Assert.Throws<ObjectDisposedException>(() => tenant.Resolve<IA>());
    }

    [Fact]
    public void ChildContainerTests_Dispose_Tenant()
    {
        var container = new StashboxContainer(c => c.WithDisposableTransientTracking());

        container.CreateChildContainer("C", c => c.Register<IA, C>());
        var tenant = container.GetChildContainer("C");

        var inst = (C)tenant?.Resolve<IA>();

        container.Dispose();

        Assert.True(inst?.Disposed);

        Assert.Throws<ObjectDisposedException>(() => container.Resolve<IA>());
        Assert.Throws<ObjectDisposedException>(() => tenant?.Resolve<IA>());
    }

    [Fact]
    public void ChildContainerTests_Dispose_Multiple()
    {
        var container = new StashboxContainer();
        container.Register<IA, C>();

        Assert.Null(Record.Exception(() => container.Dispose()));
        Assert.Null(Record.Exception(() => container.Dispose()));
    }
    
    [Fact]
    public void ChildContainerTests_Dispose_Removes_Child()
    {
        var container = new StashboxContainer();
        var a = container.CreateChildContainer("A");
        container.CreateChildContainer("B");
        var b = container.GetChildContainer("B");
        
        Assert.Equal(2, container.ChildContainers.Count());
        
        a.Dispose();

        Assert.Single(container.ChildContainers);
        
        b?.Dispose();
        
        Assert.Empty(container.ChildContainers);
    }
    
    [Fact]
    public void ChildContainerTests_Dispose_Parent()
    {
        var container = new StashboxContainer();
        container.CreateChildContainer("A");
        container.CreateChildContainer("B");
        
        Assert.Equal(2, container.ChildContainers.Count());
        
        container.Dispose();
        
        Assert.Empty(container.ChildContainers);
    }
    
#if HAS_ASYNC_DISPOSABLE
    [Fact]
    public async Task ChildContainerTests_Dispose_Async()
    {
        var container = new StashboxContainer(c => c.WithDisposableTransientTracking());
        container.Register<IA, C>();

        var tenant = container.CreateChildContainer("C", c => { });

        var inst = (C)tenant.Resolve<IA>();

        await container.DisposeAsync();

        Assert.True(inst.Disposed);
        Assert.Throws<ObjectDisposedException>(() => container.Resolve<IA>());
        Assert.Throws<ObjectDisposedException>(() => tenant.Resolve<IA>());
    }

    [Fact]
    public async Task MultitenantTests_Dispose_Tenant_Async()
    {
        var container = new StashboxContainer(c => c.WithDisposableTransientTracking());

        container.CreateChildContainer("C", c => c.Register<IA, C>());
        var tenant = container.GetChildContainer("C");

        var inst = (C)tenant?.Resolve<IA>();

        await container.DisposeAsync();

        Assert.True(inst?.Disposed);

        Assert.Throws<ObjectDisposedException>(() => container.Resolve<IA>());
        Assert.Throws<ObjectDisposedException>(() => tenant?.Resolve<IA>());
    }

    [Fact]
    public async Task MultitenantTests_Dispose_Multiple_Async()
    {
        var container = new StashboxContainer();
        container.Register<IA, C>();

        Assert.Null(await Record.ExceptionAsync(async () => await container.DisposeAsync()));
        Assert.Null(await Record.ExceptionAsync(async () => await container.DisposeAsync()));
    }
    
    [Fact]
    public async Task ChildContainerTests_Dispose_Removes_Child_Async()
    {
        var container = new StashboxContainer();
        var a = container.CreateChildContainer("A");
        container.CreateChildContainer("B");
        var b = container.GetChildContainer("B");
        
        Assert.Equal(2, container.ChildContainers.Count());
        
        await a.DisposeAsync();

        Assert.Single(container.ChildContainers);
        
        await b!.DisposeAsync();
        
        Assert.Empty(container.ChildContainers);
    }
#endif

    interface IT { }
    
    class T1 : IT { }
    class T2 : IT { }
    class T3 : IT { }
    class T4 : IT { }

    class T5 : IT
    {
        public IEnumerable<IT> Dep { get; }
        public T5(IEnumerable<IT> Dep) { this.Dep = Dep; }
    }
    
    class T6 : IT
    {
        public string ID { get; private set; }
        public IT Dep { get; }
        public T6(IT Dep) { this.Dep = Dep; }
        public void Init(string id) => ID = id;
    }

    class D1
    {
        public string ID { get; private set; }
        public IT Dep { get; }
        public D1(IT Dep) { this.Dep = Dep; }
        public void Init(string id) => ID = id;
    }
    
    class D2
    {
        public string ID { get; private set; }
        public IEnumerable<IT> Dep { get; }
        public D2(IEnumerable<IT> Dep) { this.Dep = Dep; }
        public void Init(string id) => ID = id;
    }

    class Test : IDisposable
    { 
        public bool Disposed { get; private set; }

        public void Dispose()
        {
            if (this.Disposed)
            {
                throw new ObjectDisposedException(nameof(Test));
            }

            this.Disposed = true;
        }
    }
    
    interface IA { }

    class A : IA { }

    class B : IA { }

    class C : IA, IDisposable
    {
        public bool Disposed { get; private set; }

        public void Dispose()
        {
            if (this.Disposed)
                throw new ObjectDisposedException(nameof(C));

            this.Disposed = true;
        }
    }

    class D
    {
        public D(IA ia)
        {
            Ia = ia;
        }

        public IA Ia { get; }
    }
}