using System;
using System.Collections.Generic;
using Stashbox.Tests.Utils;
using Stashbox.Utils.Data;
using Stashbox.Utils.Data.Immutable;
using Xunit;

namespace Stashbox.Tests.DataTests;

public class TreeTests
{
    [Fact]
    public void Test_Collision_Check()
    {
        var key1 = new H1();
        var key2 = new H2();
        
        var a = new A();
        var b = new B();
        var c = new C();
        var imTree = ImmutableTree<object, List<object>>.Empty;
        var tree = new HashTree<object, List<object>>();

        imTree = imTree.AddOrUpdate(key1, [a], false);
        imTree = imTree.AddOrUpdate(key2, [b], false);
        tree.Add(key1, [a]);
        tree.Add(key2, [b]);
        
        Assert.Equal([a], imTree.GetOrDefaultByValue(key1));
        Assert.Equal([b], imTree.GetOrDefaultByValue(key2));
        Assert.Equal([a], tree.GetOrDefault(key1));
        Assert.Equal([b], tree.GetOrDefault(key2));

        imTree = imTree.AddOrUpdate(key2, [c], false, (list, n) => [..list, ..n]);
        
        Assert.Equal([a], imTree.GetOrDefaultByValue(key1));
        Assert.Equal([b, c], imTree.GetOrDefaultByValue(key2));

        imTree = imTree.AddOrUpdate(key2, [b], false, true);
        
        Assert.Equal([a], imTree.GetOrDefaultByValue(key1));
        Assert.Equal([b], imTree.GetOrDefaultByValue(key2));
        
        imTree = imTree.UpdateIfExists(key2, false, list => [..list, c]);
        
        Assert.Equal([a], imTree.GetOrDefaultByValue(key1));
        Assert.Equal([b, c], imTree.GetOrDefaultByValue(key2));
        
        imTree = imTree.UpdateIfExists(key2, [b], false);
        
        Assert.Equal([a], imTree.GetOrDefaultByValue(key1));
        Assert.Equal([b], imTree.GetOrDefaultByValue(key2));
    }
    
    [Fact]
    public void Test_Ref_Collision_Check()
    {
        var (key1, key2) = TypeGen.GetCollidingTypes();

        var a = new A();
        var b = new B();
        var c = new C();
        var imTree = ImmutableTree<Type, List<object>>.Empty;
        var tree = new HashTree<Type, List<object>>();

        imTree = imTree.AddOrUpdate(key1, [a], true);
        imTree = imTree.AddOrUpdate(key2, [b], true);
        tree.Add(key1, [a]);
        tree.Add(key2, [b]);
        
        Assert.Equal([a], imTree.GetOrDefaultByRef(key1));
        Assert.Equal([b], imTree.GetOrDefaultByRef(key2));
        Assert.Equal([a], tree.GetOrDefault(key1));
        Assert.Equal([b], tree.GetOrDefault(key2));

        imTree = imTree.AddOrUpdate(key2, [c], true, (list, n) => [..list, ..n]);
        
        Assert.Equal([a], imTree.GetOrDefaultByRef(key1));
        Assert.Equal([b, c], imTree.GetOrDefaultByRef(key2));

        imTree = imTree.AddOrUpdate(key2, [b], true, true);
        
        Assert.Equal([a], imTree.GetOrDefaultByRef(key1));
        Assert.Equal([b], imTree.GetOrDefaultByRef(key2));
        
        imTree = imTree.UpdateIfExists(key2, true, list => [..list, c]);
        
        Assert.Equal([a], imTree.GetOrDefaultByRef(key1));
        Assert.Equal([b, c], imTree.GetOrDefaultByRef(key2));
        
        imTree = imTree.UpdateIfExists(key2, [b], true);
        
        Assert.Equal([a], imTree.GetOrDefaultByRef(key1));
        Assert.Equal([b], imTree.GetOrDefaultByRef(key2));
    }
    
    [Fact]
    public void Test_Remove_Keep_Collisions()
    {
        var key1 = new H1();
        var key2 = new H2();
        var key3 = new H3();
        
        var a = new A();
        var b = new B();
        var c = new C();
        var imTree = ImmutableTree<object, object>.Empty;

        imTree = imTree.AddOrUpdate(key1, a, false);
        imTree = imTree.AddOrUpdate(key2, b, false);
        imTree = imTree.AddOrUpdate(key3, c, false);

        imTree = imTree.Remove(key1, false);
        
        Assert.False(imTree.IsEmpty);
        Assert.Null(imTree.GetOrDefaultByValue(key1));
        Assert.Equal(b, imTree.GetOrDefaultByValue(key2));
        Assert.Equal(c, imTree.GetOrDefaultByValue(key3));
        
        imTree = imTree.Remove(key2, false);
        
        Assert.False(imTree.IsEmpty);
        Assert.Null(imTree.GetOrDefaultByValue(key1));
        Assert.Null(imTree.GetOrDefaultByValue(key2));
        Assert.Equal(c, imTree.GetOrDefaultByValue(key3));
        
        imTree = imTree.Remove(key3, false);
        
        Assert.True(imTree.IsEmpty);
        Assert.Null(imTree.GetOrDefaultByValue(key1));
        Assert.Null(imTree.GetOrDefaultByValue(key2));
        Assert.Null(imTree.GetOrDefaultByValue(key3));
    }
    
    [Fact]
    public void Test_Collision_Remove()
    {
        var key1 = new H1();
        var key2 = new H2();
        var key3 = new H3();
        
        var a = new A();
        var b = new B();
        var c = new C();
        var imTree = ImmutableTree<object, object>.Empty;

        imTree = imTree.AddOrUpdate(key1, a, false);
        imTree = imTree.AddOrUpdate(key2, b, false);
        imTree = imTree.AddOrUpdate(key3, c, false);

        imTree = imTree.Remove(key2, false);
        
        Assert.False(imTree.IsEmpty);
        Assert.Equal(a, imTree.GetOrDefaultByValue(key1));
        Assert.Equal(c, imTree.GetOrDefaultByValue(key3));
        Assert.Null(imTree.GetOrDefaultByValue(key2));
        
        imTree = imTree.Remove(key1, false);
        
        Assert.False(imTree.IsEmpty);
        Assert.Null(imTree.GetOrDefaultByValue(key1));
        Assert.Equal(c, imTree.GetOrDefaultByValue(key3));
        Assert.Null(imTree.GetOrDefaultByValue(key2));
        
        imTree = imTree.Remove(key3, false);
        
        Assert.True(imTree.IsEmpty);
        Assert.Null(imTree.GetOrDefaultByValue(key1));
        Assert.Null(imTree.GetOrDefaultByValue(key3));
        Assert.Null(imTree.GetOrDefaultByValue(key2));
    }
    
    private class A;
    private class B;
    private class C;
    
    private class H1
    {
        public override int GetHashCode()
        {
            return 1;
        }
    }
    
    private class H2
    {
        public override int GetHashCode()
        {
            return 1;
        }
    }
    
    private class H3
    {
        public override int GetHashCode()
        {
            return 1;
        }
    }
}