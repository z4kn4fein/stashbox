using System;
using System.Linq.Expressions;
using Xunit;

namespace Stashbox.Tests.CompilerTests
{

    public class DefaultTests
    {
        [Theory]
        [InlineData(typeof(int), default(int))]
        [InlineData(typeof(bool), default(bool))]
        [InlineData(typeof(byte), default(byte))]
        [InlineData(typeof(char), default(char))]
        [InlineData(typeof(ushort), default(ushort))]
        [InlineData(typeof(uint), default(uint))]
        [InlineData(typeof(ulong), default(ulong))]
        [InlineData(typeof(sbyte), default(sbyte))]
        [InlineData(typeof(short), default(short))]
        [InlineData(typeof(long), default(long))]
        [InlineData(typeof(string), default(string))]
        [InlineData(typeof(float), default(float))]
        [InlineData(typeof(double), default(double))]
        [InlineData(typeof(object), default(object))]
        public void Compile_Default_Values(Type type, object expectedResult)
        {
            var func = type.AsDefault().ConvertTo(typeof(object)).AsLambda<Func<object>>().CompileFunc();

            Assert.NotNull(func);
            Assert.Equal(expectedResult, func());
        }

        [Fact]
        public void Compile_DateTime_Default_Value()
        {
            var func = typeof(DateTime).AsDefault().AsLambda<Func<DateTime>>().CompileFunc();

            Assert.NotNull(func);
            Assert.Equal(default, func());
        }

        [Fact]
        public void Compile_TimeSpan_Default_Value()
        {
            var func = typeof(TimeSpan).AsDefault().AsLambda<Func<TimeSpan>>().CompileFunc();

            Assert.NotNull(func);
            Assert.Equal(default, func());
        }

        [Fact]
        public void Compile_Decimal_Default_Value()
        {
            var func = typeof(decimal).AsDefault().AsLambda<Func<decimal>>().CompileFunc();

            Assert.NotNull(func);
            Assert.Equal(default, func());
        }
    }
}
