using System;
using Xunit;

namespace Stashbox.Tests.IssueTests;

public class DoesScopeAttachToParentOnlyAffectDisposeBehaviour
{
    [Fact]
    public void Test_ScopeGraph_Named_Job_Scopes()
    {
        var container = new StashboxContainer()
            .Register<Cached1>(c => c.InNamedScope("A"))
            .Register<Cached2>(c => c.InNamedScope("A"))
            .Register<Job1>(c => c.InNamedScope("B"))
            .Register<Job2>(c => c.InNamedScope("B"));

        Job1 j1 = null;
        Job2 j2 = null;
        {
            using var a = container.BeginScope("A");

            {
                using var b = a.BeginScope("B");
                j1 = b.Resolve<Job1>();
                j2 = b.Resolve<Job2>();
            }

            Assert.Same(j1, j2.Job);

            Assert.True(j1.Disposed);
            Assert.True(j2.Disposed);

            Assert.Same(j1.Cached, j2.Cached1);

            Assert.False(j1.Cached.Disposed);
            Assert.False(j2.Cached1.Disposed);
            Assert.False(j2.Cached2.Disposed);
            Assert.False(j2.Job.Cached.Disposed);
        }

        Assert.True(j1.Cached.Disposed);
        Assert.True(j2.Cached1.Disposed);
        Assert.True(j2.Cached2.Disposed);
    }

    [Fact]
    public void Test_ScopeGraph_Nameless_Job_Scopes()
    {
        var container = new StashboxContainer()
            .Register<Cached1>(c => c.InNamedScope("A"))
            .Register<Cached2>(c => c.InNamedScope("A"))
            .RegisterScoped<Job1>()
            .RegisterScoped<Job2>();

        Job1 j1 = null;
        Job2 j2 = null;
        {
            using var a = container.BeginScope("A");

            {
                using var b = a.BeginScope();
                j1 = b.Resolve<Job1>();
                j2 = b.Resolve<Job2>();
            }

            Assert.Same(j1, j2.Job);

            Assert.True(j1.Disposed);
            Assert.True(j2.Disposed);

            Assert.Same(j1.Cached, j2.Cached1);

            Assert.False(j1.Cached.Disposed);
            Assert.False(j2.Cached1.Disposed);
            Assert.False(j2.Cached2.Disposed);
            Assert.False(j2.Job.Cached.Disposed);
        }

        Assert.True(j1.Cached.Disposed);
        Assert.True(j2.Cached1.Disposed);
        Assert.True(j2.Cached2.Disposed);
    }
}

class Cached1 : Disposable;

class Cached2 : Disposable;

class Job1 : Disposable
{
    public Job1(Cached1 cached)
    {
        Cached = cached;
    }

    public Cached1 Cached { get; }
}

class Job2 : Disposable
{
    public Job2(Job1 job, Cached2 cached2, Cached1 cached1)
    {
        Job = job;
        Cached2 = cached2;
        Cached1 = cached1;
    }

    public Job1 Job { get; }
    public Cached2 Cached2 { get; }
    public Cached1 Cached1 { get; }
}

abstract class Disposable : IDisposable
{
    public bool Disposed { get; private set; }

    public void Dispose()
    {
        if (this.Disposed)
        {
            throw new ObjectDisposedException(nameof(Disposable));
        }

        this.Disposed = true;
    }
}