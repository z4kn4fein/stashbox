using Stashbox.Configuration;
using Stashbox.Exceptions;
using Stashbox.Lifetime;
using Stashbox.Resolution;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Xunit;

namespace Stashbox.Tests
{
    public class ContainerTests
    {
        [Fact]
        public void ContainerTests_ChildContainer()
        {
            var container = new StashboxContainer();
            container.Register<ITest1, Test1>();
            container.Register<ITest2, Test2>();

            var child = container.CreateChildContainer();
            child.Register<ITest3, Test3>();

            var test3 = child.Resolve<ITest3>();

            Assert.NotNull(test3);
            Assert.IsType<Test3>(test3);
            Assert.Equal(container.ContainerContext, child.ContainerContext.ParentContext);
        }

        [Fact]
        public void ContainerTests_ChildContainer_ResolveFromParent()
        {
            var container = new StashboxContainer();
            container.Register<ITest1, Test1>();

            var child = container.CreateChildContainer();

            var test1 = child.Resolve<ITest1>();

            Assert.NotNull(test1);
            Assert.IsType<Test1>(test1);
        }

        [Fact]
        public void ContainerTests_Validate_MissingDependency()
        {
            using var container = new StashboxContainer();
            container.Register<ITest2, Test2>();
            var agr = Assert.Throws<AggregateException>(() => container.Validate());
            Assert.IsType<ResolutionFailedException>(agr.InnerExceptions[0]);
        }

        [Fact]
        public void ContainerTests_Validate_CircularDependency()
        {
            using var container = new StashboxContainer();
            container.Register<ITest1, Test4>();
            container.Register<ITest3, Test3>();
            var agr = Assert.Throws<AggregateException>(() => container.Validate());
            Assert.IsType<CircularDependencyException>(agr.InnerExceptions[0]);
        }

        [Fact]
        public void ContainerTests_Validate_Ok()
        {
            using var container = new StashboxContainer();
            container.Register<ITest1, Test1>();
            container.Register<ITest2, Test2>();
            container.Validate();
        }

        [Fact]
        public void ContainerTests_Validate_Skips_Open_Generic()
        {
            using var container = new StashboxContainer();
            container.Register(typeof(TestOpenGeneric<>));
            container.Validate();
        }

        [Fact]
        public void ContainerTests_Validate_Throws_When_No_Public_Constructor_Found()
        {
            using var container = new StashboxContainer();
            container.Register<NoPublicConstructor>();
            var agr = Assert.Throws<AggregateException>(() => container.Validate());
            Assert.IsType<ResolutionFailedException>(agr.InnerExceptions[0]);
        }

        [Fact]
        public void ContainerTests_Validate_Throws_Multiple()
        {
            using var container = new StashboxContainer();
            container.Register<NoPublicConstructor>();
            container.Register<ITest1, Test4>();
            container.Register<ITest3, Test3>();
            var agr = Assert.Throws<AggregateException>(() => container.Validate());

            Assert.Equal(3, agr.InnerExceptions.Count);
            Assert.Equal(2, agr.InnerExceptions.Count(e => e is CircularDependencyException));
            Assert.Equal(1, agr.InnerExceptions.Count(e => e is ResolutionFailedException));
        }

        [Fact]
        public void ContainerTests_CheckRegistration()
        {
            using var container = new StashboxContainer();
            container.Register<ITest1, Test1>();

            var reg = container.ContainerContext.RegistrationRepository
                .GetRegistrationMappings().FirstOrDefault(r => r.Key == typeof(ITest1));

            Assert.Equal(typeof(Test1), reg.Value.ImplementationType);

            reg = container.ContainerContext.RegistrationRepository.GetRegistrationMappings().FirstOrDefault(r => r.Value.ImplementationType == typeof(Test1));

            Assert.Equal(typeof(Test1), reg.Value.ImplementationType);
        }

        [Fact]
        public void ContainerTests_CanResolve()
        {
            var container = new StashboxContainer();
            container.Register<ITest1, Test1>();
            container.Register<ITest2, Test2>();

            var child = container.CreateChildContainer();

            Assert.True(container.CanResolve<ITest1>());
            Assert.True(container.CanResolve(typeof(ITest2)));
            Assert.True(container.CanResolve<IEnumerable<ITest2>>());
            Assert.True(container.CanResolve<Lazy<ITest2>>());
            Assert.True(container.CanResolve<Func<ITest2>>());
            Assert.True(container.CanResolve<Tuple<ITest2>>());

            Assert.True(child.CanResolve<ITest1>());
            Assert.True(child.CanResolve(typeof(ITest2)));
            Assert.True(child.CanResolve<IEnumerable<ITest2>>());
            Assert.True(child.CanResolve<Lazy<ITest2>>());
            Assert.True(child.CanResolve<Func<ITest2>>());
            Assert.True(child.CanResolve<Tuple<ITest2>>());

            Assert.False(container.CanResolve<ITest3>());
            Assert.False(container.CanResolve<ITest1>("test"));
            Assert.False(container.CanResolve(typeof(ITest1), "test"));
        }

        [Fact]
        public void ContainerTests_IsRegistered()
        {
            var container = new StashboxContainer();
            container.Register<ITest1, Test1>();
            container.Register<ITest2, Test2>(context => context.WithName("test"));

            var child = container.CreateChildContainer();

            Assert.True(container.IsRegistered<ITest1>());
            Assert.True(container.IsRegistered<ITest2>("test"));
            Assert.True(container.IsRegistered(typeof(ITest1)));
            Assert.False(container.IsRegistered<IEnumerable<ITest1>>());

            Assert.False(child.IsRegistered<ITest1>());
            Assert.False(child.IsRegistered(typeof(ITest1)));
            Assert.False(child.IsRegistered<IEnumerable<ITest1>>());
        }

        [Fact]
        public void ContainerTests_ResolverTest()
        {
            var container = new StashboxContainer();
            container.RegisterResolver(new TestResolver());
            var inst = container.Resolve<ITest1>();

            Assert.IsType<Test1>(inst);
        }

        [Fact]
        public void ContainerTests_ResolverTest_SupportsMany()
        {
            var container = new StashboxContainer();
            container.RegisterResolver(new TestResolver2());
            var inst = container.Resolve<IEnumerable<ITest1>>();

            Assert.IsType<Test1>(inst.First());
        }

        [Fact]
        public void ContainerTests_UnknownType_Config()
        {
            var container = new StashboxContainer(config => config
                .WithUnknownTypeResolution(context => context.WithSingletonLifetime()));

            container.Resolve<Test1>();

            var regs = container.ContainerContext.RegistrationRepository.GetRegistrationMappings().ToArray();

            Assert.Single(regs);
            Assert.Equal(typeof(Test1), regs[0].Key);
            Assert.True(regs[0].Value.RegistrationContext.Lifetime is SingletonLifetime);
        }

        [Fact]
        public void ContainerTests_ChildContainer_Singleton()
        {
            var container = new StashboxContainer();
            container.RegisterSingleton<ITest1, Test1>();

            var child = container.CreateChildContainer();
            child.RegisterSingleton<ITest2, Test2>();

            var test = child.Resolve<ITest2>();

            Assert.NotNull(test);
            Assert.IsType<Test2>(test);
        }

        [Fact]
        public void ContainerTests_ChildContainer_Scoped()
        {
            var container = new StashboxContainer();
            container.RegisterScoped<ITest1, Test1>();

            var child = container.CreateChildContainer();
            child.Register<ITest2, Test2>();

            var test = child.BeginScope().Resolve<ITest2>();

            Assert.NotNull(test);
            Assert.IsType<Test2>(test);
        }

        [Fact]
        public void ContainerTests_ConfigurationChanged()
        {
            ContainerConfiguration newConfig = null;
            var container = new StashboxContainer(config => config.OnContainerConfigurationChanged(nc => newConfig = nc));
            container.Configure(config => config.WithUnknownTypeResolution());

            Assert.True(newConfig.UnknownTypeResolutionEnabled);
        }

        [Fact]
        public void ContainerTests_ConfigurationChange_Null()
        {
            Assert.Throws<ArgumentNullException>(() => new StashboxContainer().Configure(null));
        }

        [Fact]
        public void ContainerTests_Configuration_DuplicatedBehavior_Throws()
        {
            Assert.Throws<ServiceAlreadyRegisteredException>(() => new StashboxContainer(c =>
                c.WithRegistrationBehavior(Rules.RegistrationBehavior.ThrowException))
            .Register<S>().Register<S>());
        }

        [Fact]
        public void ContainerTests_Configuration_DuplicatedBehavior_Does_Not_Throw_On_Replace()
        {
            using var container = new StashboxContainer(c =>
                    c.WithRegistrationBehavior(Rules.RegistrationBehavior.ThrowException));

            container.Register<S>().Register<S>(c => c.ReplaceExisting());
        }

        [Fact]
        public void ContainerTests_Configuration_DuplicatedBehavior_Does_Not_Throw_On_Replace_Only_If_Exists()
        {
            using var container = new StashboxContainer(c =>
                    c.WithRegistrationBehavior(Rules.RegistrationBehavior.ThrowException));

            container.Register<S>().Register<S>(c => c.ReplaceOnlyIfExists());
        }

        [Fact]
        public void ContainerTests_Configuration_DuplicatedBehavior_Does_Not_Throw_On_Different_Implementation()
        {
            using var container = new StashboxContainer(c =>
                c.WithRegistrationBehavior(Rules.RegistrationBehavior.ThrowException));

            container.Register<ITest1, Test1>().Register<ITest1, Test11>();
        }

        [Fact]
        public void ContainerTests_Configuration_DuplicatedBehavior_Skip()
        {
            using var container = new StashboxContainer(c =>
                 c.WithRegistrationBehavior(Rules.RegistrationBehavior.SkipDuplications))
            .Register<S>(c => c.WithInitializer((s, r) => s.Id = 0)).Register<S>(c => c.WithInitializer((s, r) => s.Id = 1));
            var regs = container.GetRegistrationMappings();

            Assert.Single(regs);
            Assert.Equal(0, container.Resolve<S>().Id);
        }

        [Fact]
        public void ContainerTests_Configuration_DuplicatedBehavior_Preserve()
        {
            var regs = new StashboxContainer(c =>
                c.WithRegistrationBehavior(Rules.RegistrationBehavior.PreserveDuplications))
            .Register<Test1>().Register<Test1>().GetRegistrationMappings();

            Assert.Equal(2, regs.Count());
        }

        [Fact]
        public void ContainerTests_Configuration_DuplicatedBehavior_Preserve_Cache_Invalidates()
        {
            using var container = new StashboxContainer(c =>
                c.WithRegistrationBehavior(Rules.RegistrationBehavior.PreserveDuplications))
            .Register<ITest1, Test1>();

            var a = container.Resolve<ITest1>();

            container.Register<ITest1, Test11>();

            a = container.Resolve<ITest1>();

            Assert.IsType<Test11>(a);
        }

        [Fact]
        public void ContainerTests_Configuration_DuplicatedBehavior_Replace()
        {
            using var container = new StashboxContainer(c =>
                c.WithRegistrationBehavior(Rules.RegistrationBehavior.ReplaceExisting))
           .Register<S>(c => c.WithInitializer((s, r) => s.Id = 0)).Register<S>(c => c.WithInitializer((s, r) => s.Id = 1));
            var regs = container.GetRegistrationMappings();

            Assert.Single(regs);
            Assert.Equal(1, container.Resolve<S>().Id);
        }

        [Fact]
        public void ContainerTests_Throws_When_TypeMap_Invalid()
        {
            using var container = new StashboxContainer();
            Assert.Throws<InvalidRegistrationException>(() => container.Register(typeof(ITest1), typeof(Test2)));
            Assert.Throws<InvalidRegistrationException>(() => container.RegisterDecorator(typeof(ITest1), typeof(Test2)));
            Assert.Throws<InvalidRegistrationException>(() => container.RegisterDecorator<ITest1>(typeof(Test2)));
            Assert.Throws<InvalidRegistrationException>(() => container.ReMapDecorator<ITest1>(typeof(Test2)));
        }

        interface ITest1 { }

        interface ITest2 { }

        interface ITest3 { }

        interface ITest5
        {
            Func<ITest2> Func { get; }
            Lazy<ITest2> Lazy { get; }
            IEnumerable<ITest3> Enumerable { get; }
            Tuple<ITest2, ITest3> Tuple { get; }
        }

        class Test1 : ITest1
        {
        }

        class Test11 : ITest1
        {
        }

        class Test2 : ITest2
        {
            public Test2(ITest1 test1)
            {
            }
        }

        class Test3 : ITest3
        {
            public Test3(ITest1 test1, ITest2 test2)
            {
            }
        }

        class Test4 : ITest1
        {
            public Test4(ITest3 test3)
            {

            }
        }

        class Test5 : ITest5
        {
            public Test5(Func<ITest2> func, Lazy<ITest2> lazy, IEnumerable<ITest3> enumerable, Tuple<ITest2, ITest3> tuple)
            {
                Func = func;
                Lazy = lazy;
                Enumerable = enumerable;
                Tuple = tuple;
            }

            public Func<ITest2> Func { get; }
            public Lazy<ITest2> Lazy { get; }
            public IEnumerable<ITest3> Enumerable { get; }
            public Tuple<ITest2, ITest3> Tuple { get; }
        }

        class TestResolver : IResolver
        {
            public bool CanUseForResolution(TypeInformation typeInfo, ResolutionContext resolutionInfo)
            {
                return typeInfo.Type == typeof(ITest1);
            }

            public Expression GetExpression(IResolutionStrategy resolutionStrategy,
                TypeInformation typeInfo,
                ResolutionContext resolutionInfo)
            {
                return Expression.Constant(new Test1());
            }
        }

        class TestResolver2 : IEnumerableSupportedResolver
        {
            public bool CanUseForResolution(TypeInformation typeInfo, ResolutionContext resolutionInfo)
            {
                return typeInfo.Type == typeof(ITest1);
            }

            public Expression GetExpression(IResolutionStrategy resolutionStrategy,
                TypeInformation typeInfo,
                ResolutionContext resolutionInfo)
            {
                return Expression.Constant(new Test1());
            }

            public IEnumerable<Expression> GetExpressionsForEnumerableRequest(
                IResolutionStrategy resolutionStrategy,
                TypeInformation typeInfo,
                ResolutionContext resolutionInfo)
            {
                return new Expression[] { Expression.Constant(new Test1()) };
            }
        }

        class S { public int Id { get; set; } }

        class TestOpenGeneric<T> { }

        class NoPublicConstructor
        {
            protected NoPublicConstructor() { }
        }
    }
}
