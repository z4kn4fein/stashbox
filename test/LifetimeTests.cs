using Stashbox.Configuration;
using Stashbox.Exceptions;
using Stashbox.Tests.Utils;
using Stashbox.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Stashbox.Tests
{

    public class LifetimeTests
    {
        [Theory]
        [ClassData(typeof(CompilerTypeTestData))]
        public void LifetimeTests_Resolve(CompilerType compilerType)
        {
            using IStashboxContainer container = new StashboxContainer(c => c.WithCompiler(compilerType));
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

        [Theory]
        [ClassData(typeof(CompilerTypeTestData))]
        public void LifetimeTests_Resolve_Parallel(CompilerType compilerType)
        {
            using IStashboxContainer container = new StashboxContainer(c => c.WithCompiler(compilerType));
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

        [Theory]
        [ClassData(typeof(CompilerTypeTestData))]
        public void LifetimeTests_Resolve_Parallel_Lazy(CompilerType compilerType)
        {
            using IStashboxContainer container = new StashboxContainer(c => c.WithCompiler(compilerType));
            container.Register<ITest1, Test1>(context => context.WithSingletonLifetime());
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

        [Theory]
        [ClassData(typeof(CompilerTypeTestData))]
        public void LifetimeTests_Per_Scoped_Resolution_Request_Dependency(CompilerType compilerType)
        {
            using var container = new StashboxContainer(c => c.WithCompiler(compilerType))
                    .Register<Test5>(context => context.WithPerScopedRequestLifetime())
                    .Register<Test6>()
                    .Register<Test7>();

            var inst1 = container.Resolve<Test7>();
            var inst2 = container.Resolve<Test7>();

            Assert.Same(inst1.Test5, inst1.Test6.Test5);
            Assert.Same(inst2.Test5, inst2.Test6.Test5);
            Assert.NotSame(inst1.Test5, inst2.Test6.Test5);
            Assert.NotSame(inst2.Test5, inst1.Test6.Test5);
        }

        [Theory]
        [ClassData(typeof(CompilerTypeTestData))]
        public void LifetimeTests_Per_Scoped_Resolution_Request_Dependency_Scoped(CompilerType compilerType)
        {
            using var container = new StashboxContainer(c => c.WithCompiler(compilerType))
                .Register<Test5>(context => context.WithPerScopedRequestLifetime())
                .RegisterScoped<Test6>()
                .Register<Test7>();

            using var scope = container.BeginScope();

            var inst1 = scope.Resolve<Test7>();
            var inst2 = scope.Resolve<Test7>();

            Assert.NotSame(inst1.Test5, inst1.Test6.Test5);
            Assert.NotSame(inst2.Test5, inst2.Test6.Test5);
            Assert.NotSame(inst1.Test5, inst2.Test6.Test5);
            Assert.NotSame(inst2.Test5, inst1.Test6.Test5);
        }

        [Theory]
        [ClassData(typeof(CompilerTypeTestData))]
        public void LifetimeTests_Per_Scoped_Resolution_Request_Dependency_WithNull(CompilerType compilerType)
        {
            using IStashboxContainer container = new StashboxContainer(c => c.WithCompiler(compilerType));
            container.Register<Test6>(context => context.WithPerScopedRequestLifetime());

            Assert.Null(container.Resolve<Test6>(nullResultAllowed: true));
        }

        [Theory]
        [ClassData(typeof(CompilerTypeTestData))]
        public void LifetimeTests_Per_Scoped_Resolution_Request(CompilerType compilerType)
        {
            using IStashboxContainer container = new StashboxContainer(c => c.WithCompiler(compilerType));
            container.Register<Test5>(context => context.WithPerScopedRequestLifetime());

            var inst1 = container.Resolve<Test5>();
            var inst2 = container.Resolve<Test5>();

            Assert.NotSame(inst1, inst2);
        }

        [Theory]
        [ClassData(typeof(CompilerTypeTestData))]
        public void LifetimeTests_Scoped_WithNull(CompilerType compilerType)
        {
            using IStashboxContainer container = new StashboxContainer(c => c.WithCompiler(compilerType));
            container.RegisterScoped<Test6>();

            Assert.Null(container.BeginScope().Resolve<Test6>(nullResultAllowed: true));
        }

        [Theory]
        [ClassData(typeof(CompilerTypeTestData))]
        public void LifetimeTests_DefinesScope_Generic(CompilerType compilerType)
        {
            using IStashboxContainer container = new StashboxContainer(c => c.WithCompiler(compilerType));
            container.Register<Test6>();
            container.Register<Test7>(c => c.DefinesScope());
            container.RegisterScoped<Test5>();

            using var scope = container.BeginScope();

            var inst1 = scope.Resolve<Test6>();
            var inst2 = scope.Resolve<Test7>();

            Assert.NotSame(inst1.Test5, inst2.Test5);
            Assert.Same(inst2.Test5, inst2.Test6.Test5);
        }

        [Theory]
        [ClassData(typeof(CompilerTypeTestData))]
        public void LifetimeTests_DefinesScope(CompilerType compilerType)
        {
            using IStashboxContainer container = new StashboxContainer(c => c.WithCompiler(compilerType));
            container.Register<Test6>();
            container.Register(typeof(Test7), c => c.DefinesScope());
            container.RegisterScoped<Test5>();

            using var scope = container.BeginScope();

            var inst1 = scope.Resolve<Test6>();
            var inst2 = scope.Resolve<Test7>();

            Assert.NotSame(inst1.Test5, inst2.Test5);
            Assert.Same(inst2.Test5, inst2.Test6.Test5);
        }

        [Theory]
        [ClassData(typeof(CompilerTypeTestData))]
        public void LifetimeTests_Per_Request(CompilerType compilerType)
        {
            using IStashboxContainer container = new StashboxContainer(c => c.WithCompiler(compilerType));
            container.Register<Test5>(c => c.WithPerRequestLifetime());
            container.Register<Test6>();
            container.Register<Test7>();

            var inst = container.Resolve<Test7>();
            var t5 = container.Resolve<Test5>();

            Assert.NotNull(inst.Test5);
            Assert.NotNull(inst.Test6.Test5);
            Assert.Same(inst.Test5, inst.Test6.Test5);
            Assert.NotSame(t5, inst.Test5);
        }

        [Theory]
        [ClassData(typeof(CompilerTypeTestData))]
        public void LifetimeTests_Per_Request_WithScope(CompilerType compilerType)
        {
            using IStashboxContainer container = new StashboxContainer(c => c.WithCompiler(compilerType));
            container.Register<Test5>(c => c.WithPerRequestLifetime());
            container.Register<Test6>(c => c.WithScopedLifetime());
            container.Register<Test7>();

            using var scope = container.BeginScope();

            var inst = scope.Resolve<Test7>();
            var t5 = scope.Resolve<Test5>();

            Assert.NotNull(inst.Test5);
            Assert.NotNull(inst.Test6.Test5);
            Assert.Same(inst.Test5, inst.Test6.Test5);
            Assert.NotSame(t5, inst.Test5);
        }

        [Theory]
        [ClassData(typeof(CompilerTypeTestData))]
        public void LifetimeTests_Per_Request_Enumerable(CompilerType compilerType)
        {
            using IStashboxContainer container = new StashboxContainer(c => c.WithCompiler(compilerType));
            container.Register<Test5>(c => c.WithPerRequestLifetime());
            container.Register<Test6>();
            container.Register<Test7>();

            var inst = container.ResolveAll<Test7>().First();
            var t5 = container.ResolveAll<Test5>().First();

            Assert.NotNull(inst.Test5);
            Assert.NotNull(inst.Test6.Test5);
            Assert.Same(inst.Test5, inst.Test6.Test5);
            Assert.NotSame(t5, inst.Test5);
        }

        [Theory]
        [ClassData(typeof(CompilerTypeTestData))]
        public void LifetimeTests_Per_Request_Wrapper_Enumerable(CompilerType compilerType)
        {
            using IStashboxContainer container = new StashboxContainer(c => c.WithCompiler(compilerType));
            container.Register<Test5>(c => c.WithPerRequestLifetime());
            container.Register<Test6>();
            container.Register<Test9>();

            var inst = container.Resolve<Test9>();
            var t5 = container.Resolve<Test5>();

            Assert.NotNull(inst.Test5);
            Assert.NotNull(inst.Test6.Test5);
            Assert.Same(inst.Test5.First(), inst.Test6.Test5);
            Assert.NotSame(t5, inst.Test5.First());
        }


        [Theory]
        [ClassData(typeof(CompilerTypeTestData))]
        public void LifetimeTests_Per_Request_Wrapper_Lazy(CompilerType compilerType)
        {
            using IStashboxContainer container = new StashboxContainer(c => c.WithCompiler(compilerType));
            container.Register<Test5>(c => c.WithPerRequestLifetime());
            container.Register<Test6>();
            container.Register<Test10>();

            var inst = container.Resolve<Test10>();
            var t5 = container.Resolve<Test5>();

            Assert.NotNull(inst.Test5);
            Assert.NotNull(inst.Test6.Test5);
            Assert.Same(inst.Test5.Value, inst.Test6.Test5);
            Assert.NotSame(t5, inst.Test5.Value);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void LifetimeTests_Shorter_Lifetime_Not_Resolvable_From_Longer_Direct(bool enabledValidation)
        {
            using IStashboxContainer container = new StashboxContainer(c =>
            {
                if (enabledValidation)
                    c.WithLifetimeValidation();
            });
            container.RegisterSingleton<Test6>();
            container.RegisterScoped<Test5>();

            using var scope = container.BeginScope();

            if (enabledValidation)
            {
                var exception = Assert.Throws<LifetimeValidationFailedException>(() => scope.Resolve<Test6>());
                Assert.Contains("The life-span of", exception.Message);
            }
            else
            {
                Assert.NotNull(scope.Resolve<Test6>());
            }
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void LifetimeTests_Shorter_Lifetime_Not_Resolvable_From_Longer_InDirect(bool enabledValidation)
        {
            using IStashboxContainer container = new StashboxContainer(c =>
            {
                if (enabledValidation)
                    c.WithLifetimeValidation();
            });
            container.RegisterSingleton<Test8>();
            container.Register<Test6>();
            container.RegisterScoped<Test5>();

            using var scope = container.BeginScope();

            if (enabledValidation)
            {
                var exception = Assert.Throws<LifetimeValidationFailedException>(() => scope.Resolve<Test8>());
                Assert.Contains("The life-span of", exception.Message);
            }
            else
            {
                Assert.NotNull(scope.Resolve<Test8>());
            }
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void LifetimeTests_Scoped_Is_Not_Resolvable_From_Root_Direct(bool enabledValidation)
        {
            using IStashboxContainer container = new StashboxContainer(c =>
            {
                if (enabledValidation)
                    c.WithLifetimeValidation();
            });

            container.RegisterScoped<Test5>();

            if (enabledValidation)
            {
                var exception = Assert.Throws<LifetimeValidationFailedException>(() => container.Resolve<Test5>());
                Assert.Contains("from the root scope", exception.Message);
            }
            else
            {
                Assert.NotNull(container.Resolve<Test5>());
            }
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void LifetimeTests_Scoped_Is_Not_Resolvable_From_Root_InDirect(bool enabledValidation)
        {
            using IStashboxContainer container = new StashboxContainer(c =>
            {
                if (enabledValidation)
                    c.WithLifetimeValidation();
            });

            container.Register<Test6>();
            container.RegisterScoped<Test5>();

            if (enabledValidation)
            {
                var exception = Assert.Throws<LifetimeValidationFailedException>(() => container.Resolve<Test6>());
                Assert.Contains("from the root scope", exception.Message);
            }
            else
            {
                Assert.NotNull(container.Resolve<Test6>());
            }
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void LifetimeTests_Scoped_Is_Not_Resolvable_From_Root_AfterResolved(bool enabledValidation)
        {
            using IStashboxContainer container = new StashboxContainer(c =>
            {
                if (enabledValidation)
                    c.WithLifetimeValidation();
            });

            container.Register<Test6>();
            container.RegisterScoped<Test5>();

            if (enabledValidation)
            {
                using var scope = container.BeginScope();
                scope.Resolve<Test6>();
                var exception = Assert.Throws<LifetimeValidationFailedException>(() => container.Resolve<Test6>());
                Assert.Contains("from the root scope", exception.Message);
            }
            else
            {
                using var scope = container.BeginScope();
                scope.Resolve<Test6>();
                Assert.NotNull(container.Resolve<Test6>());
            }
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

        class Test8
        {
            public Test6 Test6 { get; }

            public Test8(Test6 test6)
            {
                Test6 = test6;
            }
        }

        class Test9
        {
            public IEnumerable<Test5> Test5 { get; }
            public Test6 Test6 { get; }

            public Test9(IEnumerable<Test5> test5, Test6 test6)
            {
                Test5 = test5;
                Test6 = test6;
            }
        }

        class Test10
        {
            public Lazy<Test5> Test5 { get; }
            public Test6 Test6 { get; }

            public Test10(Lazy<Test5> test5, Test6 test6)
            {
                Test5 = test5;
                Test6 = test6;
            }
        }
    }
}
