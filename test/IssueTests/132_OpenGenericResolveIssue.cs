using System;
using Microsoft.Win32;
using Xunit;

namespace Stashbox.Tests.IssueTests;

public class OpenGenericResolveIssue 
{
    [Fact]
    public void Ensure_Generic_Class_Constraint_Works_With_Interface()
    {
        using var container = new StashboxContainer();
        container.Register(typeof(IA<,>), typeof(A<,>));
        
        Assert.NotNull(container.Resolve<IA<Type, IA>>());
        Assert.NotNull(container.Resolve<IA<Type, B>>());
        Assert.NotNull(container.Resolve<IA<Type, C>>());
    }
    
    [Fact]
    public void Ensure_Generic_Struct_Constraint_Works()
    {
        using var container = new StashboxContainer();
        container.Register(typeof(IS<>), typeof(S<>));
        
        Assert.NotNull(container.Resolve<IS<D>>());
    }

    interface IA;
    
    class B;
    
    abstract class C;
    
    interface IA<TK, out TV> where TV : class;

    class A<TK, TV> : IA<TK, TV> where TV : class;

    interface IS<T> where T : struct;

    class S<T> : IS<T> where T:struct;
    
    struct D;
}