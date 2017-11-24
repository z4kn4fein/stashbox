using Microsoft.VisualStudio.TestTools.UnitTesting;
using Stashbox.Attributes;
using Stashbox.Configuration;
using Stashbox.Exceptions;
using Stashbox.Infrastructure;
using Stashbox.Utils;
using System;
using System.Threading.Tasks;

namespace Stashbox.Tests
{
    [TestClass]
    public class StandardResolveTests
    {
        [TestMethod]
        public void StandardResolveTests_Resolve()
        {
            using (IStashboxContainer container = new StashboxContainer())
            {
                container.RegisterType<ITest1, Test1>();
                container.RegisterType<ITest2, Test2>();
                container.RegisterType<ITest3, Test3>();

                var test3 = container.Resolve<ITest3>();
                var test2 = container.Resolve<ITest2>();
                var test1 = container.Resolve<ITest1>();

                Assert.IsNotNull(test3);
                Assert.IsNotNull(test2);
                Assert.IsNotNull(test1);

                Assert.IsInstanceOfType(test1, typeof(Test1));
                Assert.IsInstanceOfType(test2, typeof(Test2));
                Assert.IsInstanceOfType(test3, typeof(Test3));
            }
        }

        [TestMethod]
        public void StandardResolveTests_Ensure_DependencyResolver_CanBeResolved()
        {
            using (IStashboxContainer container = new StashboxContainer())
            {
                container.RegisterScoped<ResolverTest>();
                var resolver = container.Resolve<IDependencyResolver>();

                var test = container.Resolve<ResolverTest>();

                Assert.AreSame(resolver, test.DependencyResolver);

                using (var scope = container.BeginScope())
                {
                    var scopedResolver = scope.Resolve<IDependencyResolver>();
                    var test1 = scope.Resolve<ResolverTest>();

                    Assert.AreSame(scope, scopedResolver);
                    Assert.AreSame(scope, test1.DependencyResolver);
                }
            }
        }

        [TestMethod]
        public void StandardResolveTests_Factory()
        {
            using (IStashboxContainer container = new StashboxContainer())
            {
                container.RegisterType<ITest1, Test1>();
                var test1 = container.ResolveFactory(typeof(ITest1)).DynamicInvoke();

                Assert.IsNotNull(test1);
                Assert.IsInstanceOfType(test1, typeof(Test1));
            }
        }

        [TestMethod]
        public void StandardResolveTests_Factory_Scoped()
        {
            using (IStashboxContainer container = new StashboxContainer())
            {
                container.RegisterType<ITest1, Test1>();
                using (var child = container.BeginScope())
                {
                    var test1 = child.ResolveFactory(typeof(ITest1)).DynamicInvoke();

                    Assert.IsNotNull(test1);
                    Assert.IsInstanceOfType(test1, typeof(Test1));
                }
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ResolutionFailedException))]
        public void StandardResolveTests_Factory_ResolutionFailed()
        {
            using (IStashboxContainer container = new StashboxContainer())
            {
                container.ResolveFactory(typeof(ITest1)).DynamicInvoke();
            }
        }

        [TestMethod]
        public void StandardResolveTests_Factory_ResolutionFailed_Null()
        {
            using (IStashboxContainer container = new StashboxContainer())
            {
                var factory = container.ResolveFactory(typeof(ITest1), nullResultAllowed: true);

                Assert.IsNull(factory);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ResolutionFailedException))]
        public void StandardResolveTests_DependencyResolve_ResolutionFailed()
        {
            using (IStashboxContainer container = new StashboxContainer())
            {
                container.RegisterType<ITest2, Test2>();
                container.Resolve<ITest2>();
            }
        }

        [TestMethod]
        public void StandardResolveTests_DependencyResolve_ResolutionFailed_AllowNull()
        {
            using (IStashboxContainer container = new StashboxContainer())
            {
                container.RegisterType<ITest2, Test2>();
                var result = container.Resolve<ITest2>(nullResultAllowed: true);

                Assert.IsNull(result);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ResolutionFailedException))]
        public void StandardResolveTests_Resolve_ResolutionFailed()
        {
            using (IStashboxContainer container = new StashboxContainer())
            {
                container.Resolve<ITest1>();
            }
        }

        [TestMethod]
        public void StandardResolveTests_Resolve_ResolutionFailed_AllowNull()
        {
            using (IStashboxContainer container = new StashboxContainer())
            {
                var result = container.Resolve<ITest1>(nullResultAllowed: true);

                Assert.IsNull(result);
            }
        }

        [TestMethod]
        public void StandardResolveTests_Resolve_Parallel()
        {
            IStashboxContainer container = new StashboxContainer();
            container.RegisterType<ITest1, Test1>();
            container.RegisterType<ITest2, Test2>();
            container.RegisterType<ITest3, Test3>();

            Parallel.For(0, 50000, (i) =>
            {
                if (i % 100 == 0)
                {
                    container.RegisterType<ITest1, Test1>(context => context.WithName(i.ToString()));
                    container.RegisterType<ITest3, Test3>(context => context.WithName($"ITest3{i.ToString()}"));
                    var test33 = container.Resolve<ITest3>($"ITest3{i.ToString()}");
                    var test11 = container.Resolve(typeof(ITest1), i.ToString());
                    Assert.IsNotNull(test33);
                    Assert.IsNotNull(test11);

                    Assert.IsInstanceOfType(test11, typeof(Test1));
                    Assert.IsInstanceOfType(test33, typeof(Test3));
                }

                var test3 = container.Resolve<ITest3>();
                var test2 = container.Resolve<ITest2>();
                var test1 = container.Resolve<ITest1>();

                Assert.IsNotNull(test3);
                Assert.IsNotNull(test2);
                Assert.IsNotNull(test1);

                Assert.IsInstanceOfType(test1, typeof(Test1));
                Assert.IsInstanceOfType(test2, typeof(Test2));
                Assert.IsInstanceOfType(test3, typeof(Test3));
            });
        }

        [TestMethod]
        public void StandardResolveTests_Resolve_Parallel_Lazy()
        {
            var container = new StashboxContainer();
            container.RegisterType<ITest1, Test1>();
            container.RegisterType<ITest2, Test2>();
            container.RegisterType<ITest3, Test3>();

            Parallel.For(0, 50000, (i) =>
            {
                if (i % 100 == 0)
                {
                    container.RegisterType<ITest1, Test1>();
                    container.RegisterType<ITest3, Test3>();
                }

                var test3 = container.Resolve<Lazy<ITest3>>();
                var test2 = container.Resolve<Lazy<ITest2>>();
                var test1 = container.Resolve<Lazy<ITest1>>();

                Assert.IsNotNull(test3.Value);
                Assert.IsNotNull(test2.Value);
                Assert.IsNotNull(test1.Value);
            });
        }

        [TestMethod]
        public void StandardResolveTests_Resolve_Scoped()
        {
            using (IStashboxContainer container = new StashboxContainer())
            {
                container.RegisterScoped<ITest1, Test1>();

                var inst = container.Resolve<ITest1>();
                var inst2 = container.Resolve<ITest1>();

                Assert.AreSame(inst, inst2);

                using (var child = container.BeginScope())
                {
                    var inst3 = child.Resolve<ITest1>();
                    var inst4 = child.Resolve<ITest1>();

                    Assert.AreNotSame(inst, inst3);
                    Assert.AreSame(inst3, inst4);
                }
            }
        }

        [TestMethod]
        public void StandardResolveTests_Resolve_Scoped_Factory()
        {
            using (IStashboxContainer container = new StashboxContainer())
            {
                container.RegisterScoped<ITest1, Test1>();

                var factory = container.ResolveFactory<ITest1>();

                var inst = factory();
                var inst2 = factory();

                Assert.AreSame(inst, inst2);

                using (var child = container.BeginScope())
                {
                    var scopeFactory = child.ResolveFactory<ITest1>();
                    var inst3 = scopeFactory();
                    var inst4 = scopeFactory();

                    Assert.AreNotSame(inst, inst3);
                    Assert.AreSame(inst3, inst4);
                }
            }
        }

        [TestMethod]
        public void StandardResolveTests_Resolve_Scoped_Injection()
        {
            using (IStashboxContainer container = new StashboxContainer())
            {
                container.RegisterScoped(typeof(ITest1), typeof(Test1));
                container.RegisterScoped<ITest4, Test4>();

                var inst = container.Resolve<ITest4>();
                var inst2 = container.Resolve<ITest4>();

                Assert.AreSame(inst.Test, inst2.Test);
                Assert.AreSame(inst.Test2, inst2.Test2);
                Assert.AreSame(inst.Test, inst2.Test2);

                using (var child = container.BeginScope())
                {
                    var inst3 = child.Resolve<ITest4>();
                    var inst4 = child.Resolve<ITest4>();

                    Assert.AreNotSame(inst.Test, inst4.Test);
                    Assert.AreNotSame(inst.Test2, inst4.Test2);
                    Assert.AreNotSame(inst.Test, inst4.Test2);

                    Assert.AreSame(inst3.Test, inst4.Test);
                    Assert.AreSame(inst3.Test2, inst4.Test2);
                    Assert.AreSame(inst3.Test, inst4.Test2);
                }
            }
        }

        [TestMethod]
        public void StandardResolveTests_Resolve_Scoped_Injection_Factory()
        {
            using (IStashboxContainer container = new StashboxContainer())
            {
                container.RegisterScoped(typeof(ITest1), typeof(Test1));
                container.RegisterScoped<ITest4, Test4>();

                var factory = container.ResolveFactory<ITest4>();

                var inst = factory();
                var inst2 = factory();

                Assert.AreSame(inst.Test, inst2.Test);
                Assert.AreSame(inst.Test2, inst2.Test2);
                Assert.AreSame(inst.Test, inst2.Test2);

                using (var child = container.BeginScope())
                {
                    var scopedFactory = child.ResolveFactory<ITest4>();

                    var inst3 = scopedFactory();
                    var inst4 = scopedFactory();

                    Assert.AreNotSame(inst.Test, inst4.Test);
                    Assert.AreNotSame(inst.Test2, inst4.Test2);
                    Assert.AreNotSame(inst.Test, inst4.Test2);

                    Assert.AreSame(inst3.Test, inst4.Test);
                    Assert.AreSame(inst3.Test2, inst4.Test2);
                    Assert.AreSame(inst3.Test, inst4.Test2);
                }
            }
        }

        [TestMethod]
        public void StandardResolveTests_Resolve_LastService()
        {
            using (IStashboxContainer container = new StashboxContainer())
            {
                container.RegisterType(typeof(ITest1), typeof(Test1));
                container.RegisterType(typeof(ITest1), typeof(Test11));
                container.RegisterType(typeof(ITest1), typeof(Test12));

                var inst = container.Resolve<ITest1>();

                Assert.IsInstanceOfType(inst, typeof(Test12));
            }
        }

        [TestMethod]
        public void StandardResolveTests_Resolve_MostParametersConstructor_WithoutDefault()
        {
            using (IStashboxContainer container = new StashboxContainer(config =>
            config.WithConstructorSelectionRule(Rules.ConstructorSelection.PreferMostParameters)))
            {
                container.RegisterType(typeof(ITest1), typeof(Test1));
                container.RegisterType(typeof(ITest2), typeof(Test22));

                var inst = container.Resolve<ITest2>();
            }
        }

        [TestMethod]
        public void StandardResolveTests_Resolve_MostParametersConstructor()
        {
            using (IStashboxContainer container = new StashboxContainer(config =>
            config.WithConstructorSelectionRule(Rules.ConstructorSelection.PreferMostParameters)))
            {
                container.RegisterType(typeof(ITest1), typeof(Test1), context => context.WithName("test1"));
                container.RegisterType(typeof(ITest1), typeof(Test12), context => context.WithName("test12"));
                container.RegisterType(typeof(ITest2), typeof(Test222));

                var inst = container.Resolve<ITest2>();
            }
        }

        [TestMethod]
        public void StandardResolveTests_Resolve_LeastParametersConstructor()
        {
            using (IStashboxContainer container = new StashboxContainer(config =>
            config.WithConstructorSelectionRule(Rules.ConstructorSelection.PreferLeastParameters)))
            {
                container.RegisterType(typeof(ITest1), typeof(Test1));
                container.RegisterType(typeof(ITest2), typeof(Test2222));

                var inst = container.Resolve<ITest2>();
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ResolutionFailedException))]
        public void StandardResolveTests_Resolve_None_Of_The_Constructors_Selected()
        {
            using (IStashboxContainer container = new StashboxContainer(config =>
            config.WithConstructorSelectionRule(Rules.ConstructorSelection.PreferLeastParameters)))
            {
                container.RegisterType(typeof(ITest2), typeof(Test222));

                container.Resolve<ITest2>();
            }
        }

        [TestMethod]
        public void StandardResolveTests_Resolve_Scoped_NullDependency()
        {
            using (IStashboxContainer container = new StashboxContainer())
            {
                container.RegisterScoped<Test5>();
                var inst = container.Resolve<Test5>(nullResultAllowed: true);

                Assert.IsNull(inst);
            }
        }

        [TestMethod]
        public void StandardResolveTests_Resolve_Singleton_NullDependency()
        {
            using (IStashboxContainer container = new StashboxContainer())
            {
                container.RegisterSingleton<Test5>();
                var inst = container.Resolve<Test5>(nullResultAllowed: true);

                Assert.IsNull(inst);
            }
        }

        [TestMethod]
        public void StandardResolveTests_Resolve_WithFinalizer()
        {
            var finalized = false;
            using (IStashboxContainer container = new StashboxContainer())
            {
                container.RegisterType<Test1>(context => context.WithFinalizer(_ => finalized = true));
                container.Resolve<Test1>();
            }

            Assert.IsTrue(finalized);
        }

        public interface ITest1 { string Name { get; set; } }

        public interface ITest2 { string Name { get; set; } }

        public interface ITest3 { string Name { get; set; } }

        public interface ITest4 { ITest1 Test { get; } ITest1 Test2 { get; } }

        public class Test1 : ITest1
        {
            public string Name { get; set; }
        }

        public class Test11 : ITest1
        {
            public string Name { get; set; }
        }

        public class Test12 : ITest1
        {
            public string Name { get; set; }
        }

        public class Test2 : ITest2
        {
            public string Name { get; set; }

            public Test2(ITest1 test1)
            {
                Shield.EnsureNotNull(test1, nameof(test1));
                Shield.EnsureTypeOf<Test1>(test1);
            }
        }

        public class Test22 : ITest2
        {
            public string Name { get; set; }

            public Test22(ITest1 test1)
            {
                Shield.EnsureNotNull(test1, nameof(test1));
                Shield.EnsureTypeOf<Test1>(test1);
            }

            public Test22(ITest1 test1, int index)
            {
                Assert.Fail("Wrong constructor selected.");
            }
        }

        public class Test222 : ITest2
        {
            public string Name { get; set; }

            public Test222(ITest1 test1)
            {
                Assert.Fail("Wrong constructor selected.");
            }

            public Test222([Dependency("test1")]ITest1 test1, [Dependency("test12")]ITest1 test2)
            {
                Shield.EnsureNotNull(test1, nameof(test1));
                Shield.EnsureNotNull(test2, nameof(test2));
                Shield.EnsureTypeOf<Test1>(test1);
                Shield.EnsureTypeOf<Test12>(test2);
            }
        }

        public class Test2222 : ITest2
        {
            public string Name { get; set; }

            public Test2222(ITest1 test1)
            {
                Shield.EnsureNotNull(test1, nameof(test1));
                Shield.EnsureTypeOf<Test1>(test1);
            }

            public Test2222(ITest1 test1, [Dependency("test12")]ITest1 test2)
            {
                Assert.Fail("Wrong constructor selected.");
            }
        }

        public class Test3 : ITest3
        {
            public string Name { get; set; }

            public Test3(ITest1 test1, ITest2 test2)
            {
                Shield.EnsureNotNull(test1, nameof(test1));
                Shield.EnsureNotNull(test2, nameof(test2));
                Shield.EnsureTypeOf<Test1>(test1);
                Shield.EnsureTypeOf<Test2>(test2);
            }
        }

        public class Test4 : ITest4
        {
            public ITest1 Test { get; }

            [Dependency]
            public ITest1 Test2 { get; set; }

            public Test4(ITest1 test)
            {
                this.Test = test;
            }
        }

        public class Test5
        {
            public Test5(ITest1 test)
            {
            }
        }

        public class ResolverTest
        {
            public IDependencyResolver DependencyResolver { get; }

            public ResolverTest(IDependencyResolver dependencyResolver)
            {
                this.DependencyResolver = dependencyResolver;
            }
        }
    }
}