using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq.Expressions;

namespace Stashbox.Tests.CompilerTests
{
    [TestClass]
    public class NullableTests
    {
        [TestMethod]
        public void Compile_Nullable_Primitive()
        {
            Expression<Func<int?>> expr = () => 5;
            var func = expr.CompileFunc();

            Assert.AreEqual(5, func().Value);
        }

        [TestMethod]
        public void Compile_Nullable_Primitive_Null()
        {
            Expression<Func<int?>> expr = () => null;
            var func = expr.CompileFunc();

            Assert.IsFalse(func().HasValue);
        }

        [TestMethod]
        public void Compile_Nullable_ValueType()
        {
            Expression<Func<TimeSpan?>> expr = () => TimeSpan.FromSeconds(1);
            var func = expr.CompileFunc();

            Assert.AreEqual(TimeSpan.FromSeconds(1), func().Value);
        }

        [TestMethod]
        public void Compile_Nullable_ValueType_Null()
        {
            Expression<Func<TimeSpan?>> expr = () => null;
            var func = expr.CompileFunc();

            Assert.IsFalse(func().HasValue);
        }
    }
}
