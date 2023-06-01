using System;
using System.Threading.Tasks;
using Xunit;

namespace Stashbox.Tests;

public class ChildContainerTests
{
    [Fact]
    public void ChildContainerTests_Dispose_Parent_Disposes_Child()
    {
        var test = new Test();
        var container = new StashboxContainer();
        var child = container.CreateChildContainer();
        child.RegisterInstance(test);
        
        container.Dispose();
        container.Dispose();
        
        Assert.True(test.Disposed);
    }
    
    [Fact]
    public void ChildContainerTests_Dispose_Parent_Not_Disposes_Child()
    {
        var test = new Test();
        var container = new StashboxContainer();
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
    [Fact]
    public async Task ChildContainerTests_Dispose_Parent_Disposes_Child_Async()
    {
        var test = new Test();
        var container = new StashboxContainer();
        var child = container.CreateChildContainer();
        child.RegisterInstance(test);
        
        await container.DisposeAsync();
        await container.DisposeAsync();
        
        Assert.True(test.Disposed);
    }
    
    [Fact]
    public async Task ChildContainerTests_Dispose_Parent_Not_Disposes_Child_Async()
    {
        var test = new Test();
        var container = new StashboxContainer();
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
}