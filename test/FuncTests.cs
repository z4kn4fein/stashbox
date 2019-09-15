using Microsoft.VisualStudio.TestTools.UnitTesting;
using Stashbox.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace Stashbox.Tests
{
    [TestClass]
    public class FuncTests
    {
        [TestMethod]
        public void FuncTests_Resolve()
        {
            var container = new StashboxContainer();
            container.Register<ITest, Test>();
            var inst = container.Resolve<Func<ITest>>();

            Assert.IsNotNull(inst);
            Assert.IsInstanceOfType(inst, typeof(Func<ITest>));
            Assert.IsInstanceOfType(inst(), typeof(Test));
        }

        [TestMethod]
        public void FuncTests_Resolve_Null()
        {
            var container = new StashboxContainer();
            var inst = container.Resolve<Func<ITest>>(nullResultAllowed: true);

            Assert.IsNull(inst);
        }

        [TestMethod]
        public void FuncTests_Resolve_Lazy()
        {
            var container = new StashboxContainer();
            container.Register<ITest, Test>();
            var inst = container.Resolve<Func<Lazy<ITest>>>();

            Assert.IsNotNull(inst);
            Assert.IsInstanceOfType(inst, typeof(Func<Lazy<ITest>>));
            Assert.IsInstanceOfType(inst().Value, typeof(Test));
        }

        [TestMethod]
        public void FuncTests_Resolve_Lazy_Null()
        {
            var container = new StashboxContainer();
            var inst = container.Resolve<Func<Lazy<ITest>>>(nullResultAllowed: true);

            Assert.IsNull(inst);
        }

        [TestMethod]
        public void FuncTests_Resolve_Enumerable()
        {
            var container = new StashboxContainer();
            container.Register<ITest, Test>();
            var inst = container.Resolve<Func<IEnumerable<ITest>>>();

            Assert.IsNotNull(inst);
            Assert.IsInstanceOfType(inst, typeof(Func<IEnumerable<ITest>>));
            Assert.IsInstanceOfType(inst().First(), typeof(Test));
        }

        [TestMethod]
        public void FuncTests_Resolve_Enumerable_Null()
        {
            var container = new StashboxContainer();
            var inst = container.Resolve<Func<IEnumerable<ITest>>>();

            Assert.AreEqual(0, inst().Count());
        }

        [TestMethod]
        public void FuncTests_Resolve_ConstructorDependency()
        {
            var container = new StashboxContainer();
            container.Register<ITest, Test>();
            container.Register<Test2>();
            var inst = container.Resolve<Test2>();

            Assert.IsNotNull(inst.Test);
            Assert.IsInstanceOfType(inst.Test, typeof(Func<ITest>));
            Assert.IsInstanceOfType(inst.Test(), typeof(Test));
        }

        [TestMethod]
        public void FuncTests_Resolve_ConstructorDependency_Null()
        {
            var container = new StashboxContainer();
            container.Register<Test2>();
            var inst = container.Resolve<Test2>(nullResultAllowed: true);

            Assert.IsNull(inst);
        }

        [TestMethod]
        public void FuncTests_Resolve_ParameterInjection()
        {
            var container = new StashboxContainer();
            container.Register<IFTest1, FTest1>();
            var inst = container.Resolve<Func<ITest, IFTest1>>();

            var t = new Test();
            var r = inst(t);

            Assert.AreSame(t, r.Test);
        }

        [TestMethod]
        public void FuncTests_Resolve_ParameterInjection_2Params()
        {
            var container = new StashboxContainer();
            container.Register<IFTest2, FTest2>();
            var inst = container.Resolve<Func<ITest, IFTest1, IFTest2>>();

            var t = new Test();
            var t1 = new FTest1();
            var r = inst(t, t1);

            Assert.AreSame(t, r.Test);
            Assert.AreSame(t1, r.Test1);
        }

        [TestMethod]
        public void FuncTests_Resolve_ParameterInjection_3Params()
        {
            var container = new StashboxContainer();
            container.Register<IFTest3, FTest3>();
            var inst = container.Resolve<Func<ITest, IFTest1, IFTest2, IFTest3>>();

            var t = new Test();
            var t1 = new FTest1();
            var t2 = new FTest2(null);
            var r = inst(t, t1, t2);

            Assert.AreSame(t, r.Test);
            Assert.AreSame(t1, r.Test1);
            Assert.AreSame(t2, r.Test2);
        }

        [TestMethod]
        public void FuncTests_Resolve_ParameterInjection_SubDependency()
        {
            var container = new StashboxContainer();
            container.Register<FunctTest2>();
            container.Register<FuncTest>();
            var inst = container.Resolve<Func<ITest, FuncTest>>();

            var t = new Test();
            var r = inst(t);

            Assert.AreSame(t, r.Func2.Test);
        }

        [TestMethod]
        public void FuncTests_Resolve_ParameterInjection_Mixed()
        {
            var container = new StashboxContainer();
            container.Register<FuncTest3>();
            container.Register<FuncTest4>();
            container.Register<Dep2>();
            container.Register<Dep4>();
            var inst = container.Resolve<FuncTest3>();

            var d1 = new Dep1();
            var d3 = new Dep3();

            var d = inst.Dep(d1, d3);

            var d12 = new Dep1();

            Assert.AreNotSame(d1, d.Dep(d12).Dep);
            Assert.AreSame(d12, d.Dep(d12).Dep);
        }

        [TestMethod]
        public void FuncTests_Resolve_ParameterInjection_Mixed2()
        {
            var container = new StashboxContainer();
            container.Register<FuncTest5>();
            container.Register<FuncTest6>();
            container.Register<Dep2>();
            container.Register<Dep4>();
            var inst = container.Resolve<FuncTest5>();

            var d3 = new Dep3();

            var d = inst.Dep(d3);

            var d32 = new Dep3();

            Assert.IsNotNull(d.Dep(d32).Dep);
            Assert.AreNotSame(d3, d.Dep(d32).Dep);
            Assert.AreSame(d3, d.Dep1);
        }

        [TestMethod]
        public void FuncTests_Register_FuncDelegate()
        {
            var container = new StashboxContainer();
            container.RegisterFunc<string, RegisteredFuncTest>((name, resolver) =>
            {
                Assert.IsNotNull(name);
                Assert.IsNotNull(resolver);
                return new RegisteredFuncTest(name);
            });

            var test = container.Resolve<Func<string, RegisteredFuncTest>>()("test");

            Assert.AreEqual("test", test.Name);
        }

        [TestMethod]
        public void FuncTests_Register_FuncDelegate_Lazy()
        {
            var container = new StashboxContainer();
            container.RegisterFunc<string, RegisteredFuncTest>((name, resolver) =>
            {
                Assert.IsNotNull(name);
                Assert.IsNotNull(resolver);
                return new RegisteredFuncTest(name);
            });

            var test = container.Resolve<Lazy<Func<string, RegisteredFuncTest>>>().Value("test");

            Assert.AreEqual("test", test.Name);
        }

        [TestMethod]
        public void FuncTests_Register_FuncDelegate_Resolver()
        {
            var container = new StashboxContainer();
            var test1 = new Test();
            container.RegisterInstanceAs<ITest>(test1);
            container.RegisterFunc<string, RegisteredFuncTest2>((name, resolver) => new RegisteredFuncTest2(name, resolver.Resolve<ITest>()));

            var test = container.Resolve<Func<string, RegisteredFuncTest2>>()("test");

            Assert.AreSame(test1, test.Test1);
            Assert.AreEqual("test", test.Name);
        }

        [TestMethod]
        public void FuncTests_Register_FuncDelegate_TwoParams()
        {
            var container = new StashboxContainer();
            var t1 = new Test();
            var t2 = new Test();
            container.RegisterFunc<ITest, ITest, RegisteredFuncTest3>((test1, test2, resolver) => new RegisteredFuncTest3(test1, test2));

            var test = container.Resolve<Func<ITest, ITest, RegisteredFuncTest3>>()(t1, t2);

            Assert.AreSame(t1, test.Test1);
            Assert.AreSame(t2, test.Test2);
        }

        [TestMethod]
        public void FuncTests_Register_FuncDelegate_ThreeParams()
        {
            var container = new StashboxContainer();
            var t1 = new Test();
            var t2 = new Test();
            var t3 = new Test();
            container.RegisterFunc<ITest, ITest, ITest, RegisteredFuncTest4>((test1, test2, test3, resolver) => new RegisteredFuncTest4(test1, test2, test3));

            var test = container.Resolve<Func<ITest, ITest, ITest, RegisteredFuncTest4>>()(t1, t2, t3);

            Assert.AreSame(t1, test.Test1);
            Assert.AreSame(t2, test.Test2);
            Assert.AreSame(t3, test.Test3);
        }

        [TestMethod]
        public void FuncTests_Register_FuncDelegate_FourParams()
        {
            var container = new StashboxContainer();
            var t1 = new Test();
            var t2 = new Test();
            var t3 = new Test();
            var t4 = new Test();
            container.RegisterFunc<ITest, ITest, ITest, ITest, RegisteredFuncTest5>((test1, test2, test3, test4, resolver) => new RegisteredFuncTest5(test1, test2, test3, test4));

            var test = container.Resolve<Func<ITest, ITest, ITest, ITest, RegisteredFuncTest5>>()(t1, t2, t3, t4);

            Assert.AreSame(t1, test.Test1);
            Assert.AreSame(t2, test.Test2);
            Assert.AreSame(t3, test.Test3);
            Assert.AreSame(t4, test.Test4);
        }

        [TestMethod]
        public async Task FuncTests_Register_FuncDelegate_Async()
        {
            var container = new StashboxContainer();
            var test = new Test();
            container.RegisterInstance(test);
            container.RegisterFunc(async resolver => await Task.FromResult(resolver.Resolve<Test>()));

            var inst = await container.Resolve<Func<Task<Test>>>()();
            var inst2 = await container.Resolve<Func<Task<Test>>>()();

            Assert.AreSame(test, inst);
            Assert.AreSame(inst, inst2);
        }

        [TestMethod]
        public async Task FuncTests_Register_FuncDelegate_Async_Longrun()
        {
            var container = new StashboxContainer();
            var test = new Test();
            container.RegisterInstance(test);
            container.RegisterFunc(async resolver =>
            {
                await Task.Delay(1000);
                return resolver.Resolve<Test>();
            });

            var inst = await container.Resolve<Func<Task<Test>>>()();

            Assert.AreSame(test, inst);
        }

        [TestMethod]
        public void FuncTests_Register_Named()
        {
            var container = new StashboxContainer();

            container.RegisterFunc<ITest>(resolver => new Test(), "teszt");

            var test = container.Resolve<Func<ITest>>("teszt")();

            Assert.IsNotNull(test);
        }

        [TestMethod]
        public void FuncTests_Register_Multiple()
        {
            var container = new StashboxContainer();

            container.RegisterFunc<ITest>(resolver => new Test());
            container.RegisterFunc(resolver => new Dep1());

            container.Resolve<Func<ITest>>();
            container.Resolve<Func<Dep1>>();
        }

        [TestMethod]
        public void FuncTests_Register_Multiple_ReMap()
        {
            var container = new StashboxContainer();

            container.Register<ITest, Test>();
            container.RegisterFunc(resolver => resolver.Resolve<ITest>());

            Assert.IsNotNull(container.Resolve<Func<ITest>>()());

            container.ReMap<ITest, Test>();

            Assert.IsNotNull(container.Resolve<Func<ITest>>()());
        }

        [TestMethod]
        public void FuncTests_Register_Parallel()
        {
            var container = new StashboxContainer();

            Parallel.For(0, 5000, i =>
            {
                container.RegisterFunc<ITest>(resolver => new Test(), i.ToString());

                var test = container.Resolve<Func<ITest>>(i.ToString())();

                Assert.IsNotNull(test);
            });
        }

        [TestMethod]
        public void FuncTests_Register_Compiled_Lambda()
        {
            var inst = new StashboxContainer()
                .RegisterFunc(typeof(Test)
                    .GetConstructor(Type.EmptyTypes)
                    .MakeNew()
                    .AsLambda<Func<IDependencyResolver, ITest>>(typeof(IDependencyResolver).AsParameter())
                    .Compile())
                .Resolve<Func<ITest>>()();

            Assert.IsNotNull(inst);
        }

        [TestMethod]
        public void FuncTests_Register_Static_Factory()
        {
            var inst = new StashboxContainer()
                .RegisterFunc(Create)
                .Resolve<Func<ITest>>()();

            Assert.IsNotNull(inst);
        }

        static ITest Create(IDependencyResolver resolver) =>
            new Test();

        class RegisteredFuncTest
        {
            public string Name { get; }

            public RegisteredFuncTest(string name)
            {
                this.Name = name;
            }
        }

        class RegisteredFuncTest2
        {
            public string Name { get; }
            public ITest Test1 { get; }

            public RegisteredFuncTest2(string name, ITest test1)
            {
                this.Name = name;
                this.Test1 = test1;
            }
        }

        class RegisteredFuncTest3
        {
            public ITest Test1 { get; }
            public ITest Test2 { get; }

            public RegisteredFuncTest3(ITest test1, ITest test2)
            {
                this.Test1 = test1;
                this.Test2 = test2;
            }
        }

        class RegisteredFuncTest4
        {
            public ITest Test1 { get; }
            public ITest Test2 { get; }
            public ITest Test3 { get; }

            public RegisteredFuncTest4(ITest test1, ITest test2, ITest test3)
            {
                this.Test1 = test1;
                this.Test2 = test2;
                this.Test3 = test3;
            }
        }

        class RegisteredFuncTest5
        {
            public ITest Test1 { get; }
            public ITest Test2 { get; }
            public ITest Test3 { get; }
            public ITest Test4 { get; }

            public RegisteredFuncTest5(ITest test1, ITest test2, ITest test3, ITest test4)
            {
                this.Test1 = test1;
                this.Test2 = test2;
                this.Test3 = test3;
                this.Test4 = test4;
            }
        }

        class ScopedFuncTest
        {
            public Func<ITest> Factory { get; set; }

            public ScopedFuncTest(Func<ITest> factory)
            {
                this.Factory = factory;
            }
        }

        class FuncTest
        {
            public FunctTest2 Func2 { get; set; }

            public FuncTest(FunctTest2 func2)
            {
                this.Func2 = func2;
            }
        }

        class FunctTest2
        {
            public ITest Test { get; set; }

            public FunctTest2(ITest test)
            {
                this.Test = test;
            }
        }

        class FuncTest3
        {
            public Func<Dep1, Dep3, FuncTest4> Dep { get; set; }

            public FuncTest3(Func<Dep1, Dep3, FuncTest4> dep)
            {
                Dep = dep;
            }
        }

        class FuncTest4
        {
            public Func<Dep1, Dep2> Dep { get; set; }
            public Func<Dep3, Dep4> Dep1 { get; set; }

            public FuncTest4(Func<Dep1, Dep2> dep, Func<Dep3, Dep4> dep1)
            {
                Dep = dep;
                Dep1 = dep1;
            }
        }

        class FuncTest5
        {
            public Func<Dep3, FuncTest6> Dep { get; set; }

            public FuncTest5(Func<Dep3, FuncTest6> dep)
            {
                Dep = dep;
            }
        }

        class FuncTest6
        {
            public Dep3 Dep1 { get; set; }
            public Func<Dep3, Dep4> Dep { get; set; }

            public FuncTest6(Dep3 dep1, Func<Dep3, Dep4> dep)
            {
                Dep1 = dep1;
                Dep = dep;
            }
        }

        class Dep1
        {

        }

        class Dep2
        {
            public Dep1 Dep { get; set; }

            public Dep2(Dep1 dep)
            {
                Dep = dep;
            }
        }

        class Dep3
        {

        }

        class Dep4
        {
            public Dep3 Dep { get; set; }

            public Dep4(Dep3 dep)
            {
                Dep = dep;
            }
        }

        interface ITest
        { }

        interface IFTest1
        {
            ITest Test { get; set; }
        }

        interface IFTest2
        {
            IFTest1 Test1 { get; set; }

            ITest Test { get; set; }
        }

        interface IFTest3
        {
            IFTest2 Test2 { get; set; }

            IFTest1 Test1 { get; set; }

            ITest Test { get; set; }
        }

        class FTest1 : IFTest1
        {
            public ITest Test { get; set; }

            [InjectionMethod]
            public void Init(ITest test)
            {
                this.Test = test;
            }
        }

        class FTest2 : IFTest2
        {
            [Dependency]
            public ITest Test { get; set; }

            public IFTest1 Test1 { get; set; }

            public FTest2(IFTest1 test1)
            {
                this.Test1 = test1;
            }
        }

        class FTest3 : IFTest3
        {
            [Dependency]
            public IFTest2 Test2 { get; set; }

            public IFTest1 Test1 { get; set; }

            public ITest Test { get; set; }

            [InjectionMethod]
            public void Init(ITest test)
            {
                this.Test = test;
            }

            public FTest3(IFTest1 test1)
            {
                this.Test1 = test1;
            }
        }

        class Test : ITest
        { }

        class Test2
        {
            public Func<ITest> Test { get; private set; }

            public Test2(Func<ITest> test)
            {
                this.Test = test;
            }
        }
    }
}
