using Microsoft.VisualStudio.TestTools.UnitTesting;
using Stashbox.Infrastructure;
using Stashbox.Lifetime;
using Stashbox.Utils;
using System;
using System.Threading.Tasks;

namespace Stashbox.Tests
{
    [TestClass]
    public class LifetimeTests
    {
        [TestMethod]
        public void LifetimeTests_Resolve()
        {
            using (IStashboxContainer container = new StashboxContainer())
            {
                container.RegisterSingleton<ITest1, Test1>();
                container.RegisterType<ITest2, Test2>();
                container.RegisterType<ITest3, Test3>();

                var test1 = container.Resolve<ITest1>();
                test1.Name = "test1";
                var test2 = container.Resolve<ITest2>();
                var test3 = container.Resolve<ITest3>();

                Assert.IsNotNull(test1);
                Assert.IsNotNull(test2);
                Assert.IsNotNull(test3);
            }
        }

        [TestMethod]
        public void LifetimeTests_Resolve_Parallel()
        {
            using (IStashboxContainer container = new StashboxContainer())
            {
                container.RegisterSingleton(typeof(ITest1), typeof(Test1));
                container.RegisterType<ITest2, Test2>();
                container.RegisterType<ITest3, Test3>();

                Parallel.For(0, 50000, (i) =>
                {
                    var test1 = container.Resolve<ITest1>();
                    test1.Name = "test1";
                    var test2 = container.Resolve<ITest2>();
                    var test3 = container.Resolve<ITest3>();

                    Assert.IsNotNull(test1);
                    Assert.IsNotNull(test2);
                    Assert.IsNotNull(test3);
                });
            }
        }

        [TestMethod]
        public void LifetimeTests_Resolve_Parallel_Lazy()
        {
            using (IStashboxContainer container = new StashboxContainer())
            {
                container.RegisterType<ITest1, Test1>(context => context.WithLifetime(new SingletonLifetime()));
                container.RegisterType<ITest2, Test2>();
                container.RegisterType<ITest3, Test3>();

                Parallel.For(0, 50000, (i) =>
                {
                    var test1 = container.Resolve<Lazy<ITest1>>();
                    test1.Value.Name = "test1";
                    var test2 = container.Resolve<Lazy<ITest2>>();
                    var test3 = container.Resolve<Lazy<ITest3>>();

                    Assert.IsNotNull(test1.Value);
                    Assert.IsNotNull(test2.Value);
                    Assert.IsNotNull(test3.Value);
                });
            }
        }

        [TestMethod]
        public void LifetimeTests_Per_Resolution_Request_Dependency()
        {
            using (IStashboxContainer container = new StashboxContainer())
            {
                container.RegisterType<Test5>(context => context.WithPerResolutionRequestLifetime())
                .RegisterType<Test6>()
                .RegisterType<Test7>();

                var inst1 = container.Resolve<Test7>();
                var inst2 = container.Resolve<Test7>();

                Assert.AreSame(inst1.Test5, inst1.Test6.Test5);
                Assert.AreSame(inst2.Test5, inst2.Test6.Test5);
                Assert.AreNotSame(inst1.Test5, inst2.Test6.Test5);
                Assert.AreNotSame(inst2.Test5, inst1.Test6.Test5);
            }
        }

        [TestMethod]
        public void LifetimeTests_Per_Resolution_Request_Dependency_WithNull()
        {
            using (IStashboxContainer container = new StashboxContainer())
            {
                container.RegisterType<Test6>(context => context.WithPerResolutionRequestLifetime());

                Assert.IsNull(container.Resolve<Test6>(nullResultAllowed: true));
            }
        }

        [TestMethod]
        public void LifetimeTests_Per_Resolution_Request()
        {
            using (IStashboxContainer container = new StashboxContainer())
            {
                container.RegisterType<Test5>(context => context.WithPerResolutionRequestLifetime());

                var inst1 = container.Resolve<Test5>();
                var inst2 = container.Resolve<Test5>();

                Assert.AreNotSame(inst1, inst2);
            }
        }

        [TestMethod]
        public void Scoped_Lifetime_Ensure_Thread_Safe()
        {
            for (var i = 0; i < 1000; i++)
            {
                Test4.IsConstructed = false;
                using (IStashboxContainer container = new StashboxContainer())
                {
                    container.RegisterScoped<Test4>();

                    using (var scope = container.BeginScope())
                    {
                        Parallel.For(0, 50, _ => scope.Resolve<Test4>());
                    }
                }
            }
        }

        [TestMethod]
        public void LifetimeTests_Scoped_WithNull()
        {
            using (IStashboxContainer container = new StashboxContainer())
            {
                container.RegisterScoped<Test6>();

                Assert.IsNull(container.BeginScope().Resolve<Test6>(nullResultAllowed: true));
            }
        }

        [TestMethod]
        public void LifetimeTests_StateCheck()
        {
            var scoped = new ScopedLifetime();
            Assert.IsInstanceOfType(scoped.Create(), typeof(ScopedLifetime));

            var singleton = new SingletonLifetime();
            Assert.IsInstanceOfType(singleton.Create(), typeof(SingletonLifetime));

            var perResolution = new ResolutionRequestLifetime();
            Assert.IsInstanceOfType(perResolution.Create(), typeof(ResolutionRequestLifetime));
        }

        public interface ITest1 { string Name { get; set; } }

        public interface ITest2 { string Name { get; set; } }

        public interface ITest3 { string Name { get; set; } }

        public class Test1 : ITest1
        {
            public string Name { get; set; }
        }

        public class Test2 : ITest2
        {
            public ITest1 Test1 { get; }
            public string Name { get; set; }

            public Test2(ITest1 test1)
            {
                Test1 = test1;
                Shield.EnsureNotNull(test1, nameof(test1));
                Shield.EnsureNotNullOrEmpty(test1.Name, nameof(test1.Name));
                Shield.EnsureTypeOf<Test1>(test1);
            }
        }

        public class Test3 : ITest3
        {
            public ITest1 Test1 { get; }
            public ITest2 Test2 { get; }
            public string Name { get; set; }

            public Test3(ITest1 test1, ITest2 test2)
            {
                Test1 = test1;
                Test2 = test2;
                Shield.EnsureNotNull(test1, nameof(test1));
                Shield.EnsureNotNull(test2, nameof(test2));
                Shield.EnsureNotNullOrEmpty(test1.Name, nameof(test1.Name));

                Shield.EnsureTypeOf<Test1>(test1);
                Shield.EnsureTypeOf<Test2>(test2);
            }
        }

        public class Test4
        {
            public static bool IsConstructed;

            public Test4()
            {
                if (IsConstructed)
                    throw new InvalidOperationException();

                IsConstructed = true;
            }
        }

        class Test5
        {
        }

        class Test6
        {
            public Test5 Test5 { get; }

            public Test6(Test5 test5)
            {
                Test5 = test5;
            }
        }

        class Test7
        {
            public Test5 Test5 { get; }
            public Test6 Test6 { get; }

            public Test7(Test5 test5, Test6 test6)
            {
                Test5 = test5;
                Test6 = test6;
            }
        }
    }
}
