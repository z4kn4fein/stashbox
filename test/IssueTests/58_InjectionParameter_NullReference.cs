using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Stashbox.Tests.IssueTests
{
    [TestClass]
    public class InjectionParameterNullReference
    {
        [TestMethod]
        public void InjectionParameter_NullReference()
        {
            var inst = new StashboxContainer()
                .Register<Test>(c => c.WithInjectionParameter("arg", null))
                .Resolve<Test>();

            Assert.IsNotNull(inst);
        }

        [TestMethod]
        public void InjectionParameter_NullReference_Object()
        {
            var inst = new StashboxContainer()
                .Register<Test2>(c => c.WithInjectionParameter("arg", null))
                .Resolve<Test2>();

            Assert.IsNotNull(inst);
        }

        class Test
        {
            public Test(Test2 arg)
            { }
        }

        class Test2
        {
            public Test2(object arg)
            { }
        }
    }
}
