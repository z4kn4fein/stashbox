using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq.Expressions;

namespace Stashbox.Tests.CompilerTests
{
    [TestClass]
    public class DefaultTests
    {
        [DataTestMethod]
        [DataRow(typeof(int), default(int))]
        [DataRow(typeof(bool), default(bool))]
        [DataRow(typeof(byte), default(byte))]
        [DataRow(typeof(char), default(char))]
        [DataRow(typeof(ushort), default(ushort))]
        [DataRow(typeof(uint), default(uint))]
        [DataRow(typeof(ulong), default(ulong))]
        [DataRow(typeof(sbyte), default(sbyte))]
        [DataRow(typeof(short), default(short))]
        [DataRow(typeof(long), default(long))]
        [DataRow(typeof(string), default(string))]
        [DataRow(typeof(float), default(float))]
        [DataRow(typeof(double), default(double))]
        [DataRow(typeof(object), default(object))]
        public void Compile_Default_Values(Type type, object expectedResult)
        {
            var func = type.AsDefault().ConvertTo(typeof(object)).AsLambda<Func<object>>().CompileFunc();

            Assert.IsNotNull(func);
            Assert.AreEqual(expectedResult, func());
        }

        [TestMethod]
        public void Compile_DateTime_Default_Value()
        {
            var func = typeof(DateTime).AsDefault().AsLambda<Func<DateTime>>().CompileFunc();

            Assert.IsNotNull(func);
            Assert.AreEqual(default, func());
        }

        [TestMethod]
        public void Compile_TimeSpan_Default_Value()
        {
            var func = typeof(TimeSpan).AsDefault().AsLambda<Func<TimeSpan>>().CompileFunc();

            Assert.IsNotNull(func);
            Assert.AreEqual(default, func());
        }

        [TestMethod]
        public void Compile_Decimal_Default_Value()
        {
            var func = typeof(decimal).AsDefault().AsLambda<Func<decimal>>().CompileFunc();

            Assert.IsNotNull(func);
            Assert.AreEqual(default, func());
        }
    }
}
