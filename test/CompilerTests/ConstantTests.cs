using System;
using System.Linq.Expressions;
using Xunit;

namespace Stashbox.Tests.CompilerTests
{
    public class ConstantTests
    {
        enum FakeEnum { Default }

        [Theory]
        [InlineData(2)]
        [InlineData(true)]
        [InlineData((byte)4)]
        [InlineData((char)5)]
        [InlineData((ushort)6)]
        [InlineData((uint)7)]
        [InlineData((ulong)8)]
        [InlineData((sbyte)9)]
        [InlineData((short)10)]
        [InlineData((long)11)]
        [InlineData("test")]
        [InlineData((float)12)]
        [InlineData((double)13)]
        [InlineData(typeof(object))]
        [InlineData(FakeEnum.Default)]
        public void Compile_Constant_Values(object expectedResult)
        {
            var func = expectedResult.AsConstant().ConvertTo(typeof(object)).AsLambda<Func<object>>().CompileFunc();

            Assert.NotNull(func);
            Assert.Equal(expectedResult, func());
        }
    }
}
