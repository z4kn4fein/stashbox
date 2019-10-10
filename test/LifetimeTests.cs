using Stashbox.Lifetime;
using Stashbox.Utils;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Stashbox.Tests
{

    public class LifetimeTests
    {
        [Fact]
        public void LifetimeTests_Resolve()
        {
            using (IStashboxContainer container = new StashboxContainer())
            {
                container.RegisterSingleton<ITest1, Test1>();
                container.Register<ITest2, Test2>();
                container.Register<ITest3, Test3>();

                var test1 = container.Resolve<ITest1>();
                test1.Name = "test1";
                var test2 = container.Resolve<ITest2>();
                var test3 = container.Resolve<ITest3>();

                Assert.NotNull(test1);
                Assert.NotNull(test2);
                Assert.NotNull(test3);
            }
        }

        [Fact]
        public void LifetimeTests_Resolve_Parallel()
        {
            using (IStashboxContainer container = new StashboxContainer())
            {
                container.RegisterSingleton(typeof(ITest1), typeof(Test1));
                container.Register<ITest2, Test2>();
                container.Register<ITest3, Test3>();

                Parallel.For(0, 50000, (i) =>
                {
                    var test1 = container.Resolve<ITest1>();
                    test1.Name = "test1";
                    var test2 = container.Resolve<ITest2>();
                    var test3 = container.Resolve<ITest3>();

                    Assert.NotNull(test1);
                    Assert.NotNull(test2);
                    Assert.NotNull(test3);
                });
            }
        }

        [Fact]
        public void LifetimeTests_Resolve_Parallel_Lazy()
        {
            using (IStashboxContainer container = new StashboxContainer())
            {
                container.Register<ITest1, Test1>(context => context.WithLifetime(new SingletonLifetime()));
                container.Register<ITest2, Test2>();
                container.Register<ITest3, Test3>();

                Parallel.For(0, 50000, (i) =>
                {
                    var test1 = container.Resolve<Lazy<ITest1>>();
                    test1.Value.Name = "test1";
                    var test2 = container.Resolve<Lazy<ITest2>>();
                    var test3 = container.Resolve<Lazy<ITest3>>();

                    Assert.NotNull(test1.Value);
                    Assert.NotNull(test2.Value);
                    Assert.NotNull(test3.Value);
                });
            }
        }

        [Fact]
        public void LifetimeTests_Per_Resolution_Request_Dependency()
        {
            using (IStashboxContainer container = new StashboxContainer())
            {
                container.Register<Test5>(context => context.WithPerResolutionRequestLifetime())
                .Register<Test6>()
                .Register<Test7>();

                var inst1 = container.Resolve<Test7>();
                var inst2 = container.Resolve<Test7>();

                Assert.Same(inst1.Test5, inst1.Test6.Test5);
                Assert.Same(inst2.Test5, inst2.Test6.Test5);
                Assert.NotSame(inst1.Test5, inst2.Test6.Test5);
                Assert.NotSame(inst2.Test5, inst1.Test6.Test5);
            }
        }

        [Fact]
        public void LifetimeTests_Per_Resolution_Request_Dependency_WithNull()
        {
            using (IStashboxContainer container = new StashboxContainer())
            {
                container.Register<Test6>(context => context.WithPerResolutionRequestLifetime());

                Assert.Null(container.Resolve<Test6>(nullResultAllowed: true));
            }
        }

        [Fact]
        public void LifetimeTests_Per_Resolution_Request()
        {
            using (IStashboxContainer container = new StashboxContainer())
            {
                container.Register<Test5>(context => context.WithPerResolutionRequestLifetime());

                var inst1 = container.Resolve<Test5>();
                var inst2 = container.Resolve<Test5>();

                Assert.NotSame(inst1, inst2);
            }
        }

        [Fact]
        public void LifetimeTests_Scoped_WithNull()
        {
            using (IStashboxContainer container = new StashboxContainer())
            {
                container.RegisterScoped<Test6>();

                Assert.Null(container.BeginScope().Resolve<Test6>(nullResultAllowed: true));
            }
        }

        [Fact]
        public void LifetimeTests_StateCheck()
        {
            var scoped = new ScopedLifetime();
            Assert.IsType<ScopedLifetime>(scoped.Create());

            var singleton = new SingletonLifetime();
            Assert.IsType<SingletonLifetime>(singleton.Create());

            var perResolution = new ResolutionRequestLifetime();
            Assert.IsType<ResolutionRequestLifetime>(perResolution.Create());
        }

        interface ITest1 { string Name { get; set; } }

        interface ITest2 { string Name { get; set; } }

        interface ITest3 { string Name { get; set; } }

        class Test1 : ITest1
        {
            public string Name { get; set; }
        }

        class Test2 : ITest2
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

        class Test3 : ITest3
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

        class Test4
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
