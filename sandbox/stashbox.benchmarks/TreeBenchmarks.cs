extern alias from_project;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using BenchmarkDotNet.Attributes;
using from_project::Stashbox.Utils.Data.Immutable;

namespace Stashbox.Benchmarks;

[MemoryDiagnoser]
public class TreeBenchmarks
{
    private Type[] keys;
        
    private ImmutableDictionary<Type, string> imDict;

    private ImmutableTree<Type, string> imTree;

    [Params(10, 100, 1000)]
    public int Count;
    
    [GlobalSetup]
    public void Setup()
    {
        imDict = ImmutableDictionary<Type, string>.Empty;
        imTree = ImmutableTree<Type, string>.Empty;
        keys = typeof(Dictionary<,>).Assembly.GetTypes().Take(Count).ToArray();

        foreach (var key in keys)
        {
            imDict = imDict.Add(key, "value");
            imTree = imTree.AddOrUpdate(key, "value", true);
        }
    }

    [Benchmark]
    public object ImmutableDictionary_Add()
    {
        var imDictToAdd = ImmutableDictionary<Type, string>.Empty;
        
        foreach (var key in keys)
        {
            imDictToAdd = imDictToAdd.Add(key, "value");
        }
        return imDictToAdd;
    }

    [Benchmark]
    public object ImmutableTree_AddOrUpdate()
    {
        var imTreeToAdd = ImmutableTree<Type, string>.Empty;
        
        foreach (var key in keys)
        {
            imTreeToAdd = imTreeToAdd.AddOrUpdate(key, "value", true);
        }

        return imTreeToAdd;
    }
    
    [Benchmark]
    public void ImmutableDictionary_TryGetValue()
    {
        foreach (var key in keys)
        {
            imDict.TryGetValue(key, out _);
        }
    }

    [Benchmark]
    public void ImmutableTree_GetOrDefault()
    {
        foreach (var key in keys)
        {
            imTree.GetOrDefaultByRef(key);
        }
    }
}