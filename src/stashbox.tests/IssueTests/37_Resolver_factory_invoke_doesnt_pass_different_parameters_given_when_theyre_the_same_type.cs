using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Stashbox.Tests.IssueTests
{
    [TestClass]
    public class ResolverFactoryIssue
    {
        [TestMethod]
        public void Resolver_factory_invoke_doesnt_pass_different_parameters_given_when_theyre_the_same_type()
        {
            var factory = new StashboxContainer()
                .RegisterType<IFoo, Foobar>()
                .Resolve<Func<string, string, IFoo>>();

            //var c = new Container();
            //c.Register<IFoo, Foobar>();

            //var factory = c.Resolve<Func<string, string, IFoo>>();

            Assert.AreEqual("foobar", factory("foo", "bar").Result);
        }

        interface IFoo
        {
            string Result { get; set; }
        }

        class Foobar : IFoo
        {
            public Foobar(string x, string y)
            {
                this.Result = x + y;
            }

            public string Result { get; set; }
        }
    }
}
