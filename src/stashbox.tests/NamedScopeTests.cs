﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using Stashbox.Lifetime;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Stashbox.Tests
{
    [TestClass]
    public class NamedScopeTests
    {
        [TestMethod]
        public void NamedScope_Simple_Resolve_Prefer_Named()
        {
            var inst = new StashboxContainer()
                .RegisterType<ITest, Test11>()
                .RegisterType<ITest, Test>(config => config.InNamedScope("A"))
                .RegisterType<ITest, Test1>()
                .BeginScope("A")
                .Resolve<ITest>();

            Assert.IsInstanceOfType(inst, typeof(Test));
        }

        [TestMethod]
        public void NamedScope_Dependency_Resolve_Prefer_Named()
        {
            var inst = new StashboxContainer()
                .RegisterType<Test2>()
                .RegisterType<ITest, Test11>()
                .RegisterType<ITest, Test>(config => config.InNamedScope("A"))
                .RegisterType<ITest, Test1>()
                .BeginScope("A")
                .Resolve<Test2>();

            Assert.IsInstanceOfType(inst.Test, typeof(Test));
        }

        [TestMethod]
        public void NamedScope_Simple_Resolve_Wrapper_Prefer_Named()
        {
            var scope = new StashboxContainer()
                .RegisterType<ITest, Test11>()
                .RegisterType<ITest, Test>(config => config.InNamedScope("A"))
                .RegisterType<ITest, Test1>()
                .BeginScope("A");

            var func = scope.Resolve<Func<ITest>>();
            var lazy = scope.Resolve<Lazy<ITest>>();
            var tuple = scope.Resolve<Tuple<ITest>>();
            var enumerable = scope.Resolve<IEnumerable<ITest>>();
            var all = scope.ResolveAll<ITest>();

            Assert.IsInstanceOfType(func(), typeof(Test));
            Assert.IsInstanceOfType(lazy.Value, typeof(Test));
            Assert.IsInstanceOfType(tuple.Item1, typeof(Test));
            Assert.IsInstanceOfType(enumerable.Last(), typeof(Test));
            Assert.IsInstanceOfType(all.First(), typeof(Test));
        }

        [TestMethod]
        public void NamedScope_Dependency_Resolve_Wrapper_Prefer_Named()
        {
            var inst = new StashboxContainer()
                .RegisterType<Test3>()
                .RegisterType<ITest, Test11>()
                .RegisterType<ITest, Test>(config => config.InNamedScope("A"))
                .RegisterType<ITest, Test1>()
                .BeginScope("A")
                .Resolve<Test3>();

            Assert.IsInstanceOfType(inst.Func(), typeof(Test));
            Assert.IsInstanceOfType(inst.Lazy.Value, typeof(Test));
            Assert.IsInstanceOfType(inst.Enumerable.Last(), typeof(Test));
            Assert.IsInstanceOfType(inst.Tuple.Item1, typeof(Test));
        }

        [TestMethod]
        public void NamedScope_Simple_Resolve_Prefer_Named_Last()
        {
            var inst = new StashboxContainer()
                .RegisterType<ITest, Test11>(config => config.InNamedScope("A"))
                .RegisterType<ITest, Test>()
                .RegisterType<ITest, Test1>(config => config.InNamedScope("A"))
                .BeginScope("A")
                .Resolve<ITest>();

            Assert.IsInstanceOfType(inst, typeof(Test1));
        }

        [TestMethod]
        public void NamedScope_Simple_Resolve_Gets_Same_Within_Scope()
        {
            var scope = new StashboxContainer()
                .RegisterType<ITest, Test>()
                .RegisterType<ITest, Test1>(config => config.InNamedScope("A"))
                .BeginScope("A");

            var a = scope.Resolve<ITest>();
            var b = scope.Resolve<ITest>();

            Assert.AreSame(a, b);
        }

        [TestMethod]
        public void NamedScope_Dependency_Resolve_Wrapper_Gets_Same_Within_Scope()
        {
            var inst = new StashboxContainer()
                .RegisterType<Test3>()
                .RegisterType<ITest, Test>(config => config.InNamedScope("A"))
                .RegisterType<ITest, Test1>()
                .BeginScope("A")
                .Resolve<Test3>();

            Assert.AreSame(inst.Func(), inst.Lazy.Value);
            Assert.AreSame(inst.Lazy.Value, inst.Enumerable.Last());
            Assert.AreSame(inst.Enumerable.Last(), inst.Tuple.Item1);
        }

        [TestMethod]
        public void NamedScope_Simple_Resolve_Get_Last_If_Scoped_Doesnt_Exist()
        {
            var inst = new StashboxContainer()
                .RegisterType<ITest, Test>()
                .RegisterType<ITest, Test1>()
                .BeginScope("A")
                .Resolve<ITest>();

            Assert.IsInstanceOfType(inst, typeof(Test1));
        }

        [TestMethod]
        public void NamedScope_Dependency_Get_Last_If_Scoped_Doesnt_Exist()
        {
            var inst = new StashboxContainer()
                .RegisterType<Test3>()
                .RegisterType<ITest, Test>()
                .RegisterType<ITest, Test1>()
                .BeginScope("A")
                .Resolve<Test3>();

            Assert.IsInstanceOfType(inst.Func(), typeof(Test1));
            Assert.IsInstanceOfType(inst.Lazy.Value, typeof(Test1));
            Assert.IsInstanceOfType(inst.Enumerable.Last(), typeof(Test1));
            Assert.IsInstanceOfType(inst.Tuple.Item1, typeof(Test1));
        }

        [TestMethod]
        public void NamedScope_Defines_Scope_Prefer_Named()
        {
            var inst = new StashboxContainer()
                .RegisterType<Test3>(config => config.DefinesScope("A"))
                .RegisterType<ITest, Test11>()
                .RegisterType<ITest, Test>(config => config.InNamedScope("A"))
                .RegisterType<ITest, Test1>()
                .Resolve<Test3>();

            Assert.IsInstanceOfType(inst.Func(), typeof(Test));
            Assert.IsInstanceOfType(inst.Lazy.Value, typeof(Test));
            Assert.IsInstanceOfType(inst.Enumerable.Last(), typeof(Test));
            Assert.IsInstanceOfType(inst.Tuple.Item1, typeof(Test));

            Assert.AreSame(inst.Func(), inst.Lazy.Value);
            Assert.AreSame(inst.Lazy.Value, inst.Enumerable.Last());
            Assert.AreSame(inst.Enumerable.Last(), inst.Tuple.Item1);
        }

        [TestMethod]
        public void NamedScope_Lifetime_Doesnt_Change()
        {
            var inst = new StashboxContainer()
                .RegisterType<ITest, Test>(config => config.InNamedScope("A").WithSingletonLifetime())
                .ContainerContext.RegistrationRepository.GetRegistrationOrDefault(typeof(ITest), new HashSet<string> { "A" });

            Assert.IsInstanceOfType(inst.RegistrationContext.Lifetime, typeof(ScopedLifetime));
        }

        interface ITest
        { }

        class Test : ITest
        { }

        class Test1 : ITest
        { }

        class Test11 : ITest
        { }

        class Test2
        {
            public Test2(ITest test)
            {
                Test = test;
            }

            public ITest Test { get; }
        }

        class Test3
        {
            public Test3(Func<ITest> func, Lazy<ITest> lazy, IEnumerable<ITest> enumerable, Tuple<ITest> tuple)
            {
                Func = func;
                Lazy = lazy;
                Enumerable = enumerable;
                Tuple = tuple;
            }

            public Func<ITest> Func { get; }
            public Lazy<ITest> Lazy { get; }
            public IEnumerable<ITest> Enumerable { get; }
            public Tuple<ITest> Tuple { get; }
        }
    }
}
