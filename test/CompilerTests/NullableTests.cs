using System;
using System.Linq.Expressions;
using Xunit;

namespace Stashbox.Tests.CompilerTests;

public class NullableTests
{
    [Fact]
    public void Compile_Nullable_Primitive()
    {
        Expression<Func<int?>> expr = () => 5;
        var func = expr.CompileFunc();

        Assert.Equal(5, func().Value);
    }

    [Fact]
    public void Compile_Nullable_Primitive_Null()
    {
        Expression<Func<int?>> expr = () => null;
        var func = expr.CompileFunc();

        Assert.False(func().HasValue);
    }

    [Fact]
    public void Compile_Nullable_ValueType()
    {
        Expression<Func<TimeSpan?>> expr = () => TimeSpan.FromSeconds(1);
        var func = expr.CompileFunc();

        Assert.Equal(TimeSpan.FromSeconds(1), func().Value);
    }

    [Fact]
    public void Compile_Nullable_ValueType_Null()
    {
        Expression<Func<TimeSpan?>> expr = () => null;
        var func = expr.CompileFunc();

        Assert.False(func().HasValue);
    }
}