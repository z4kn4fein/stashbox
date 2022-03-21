using System;
using Xunit;

namespace Stashbox.Tests.IssueTests
{

    public class ResolverFactoryIssue
    {
        [Fact]
        public void Resolver_factory_invoke_doesnt_pass_different_parameters_given_when_theyre_the_same_type()
        {
            var factory = new StashboxContainer()
                .Register<IFoo, Foobar>()
                .ResolveFactory(typeof(IFoo), parameterTypes: new[] { typeof(string), typeof(string) });

            Assert.Equal("foobar", ((IFoo)factory.DynamicInvoke("foo", "bar")).Result);
        }

        [Fact]
        public void Resolver_factory_invoke_doesnt_pass_different_parameters_given_when_theyre_the_same_type_func()
        {
            var factory = new StashboxContainer()
                .Register<IFoo, Foobar>()
                .Resolve<Func<string, string, IFoo>>();

            Assert.Equal("foobar", factory("foo", "bar").Result);
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
