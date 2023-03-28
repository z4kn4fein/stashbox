using System;
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

    interface IA
    { }
    
    class B
    { }
    
    abstract class C
    { }
    
    interface IA<TK, out TV> where TV : class
    { }

    class A<TK, TV> : IA<TK, TV> where TV : class
    { }
}