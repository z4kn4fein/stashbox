using Stashbox.Exceptions;
using Stashbox.Lifetime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Xunit;

namespace Stashbox.Tests
{

    public class RegistersTests
    {
        [Fact]
        public void RegistersTests_RegistersAs()
        {
            IStashboxContainer container = new StashboxContainer();
            container.RegisterTypesAs<ITest1>(new[] { typeof(Test1), typeof(Test11), typeof(Test12) });

            var all = container.Resolve<IEnumerable<ITest1>>();

            Assert.Equal(3, all.Count());
        }

        [Fact]
        public void RegistersTests_RegistersAs_Assembly()
        {
            IStashboxContainer container = new StashboxContainer();
            container.RegisterTypesAs<ITest1>(typeof(ITest1).GetTypeInfo().Assembly,
                t => t == typeof(Test1) || t == typeof(Test11) || t == typeof(Test12));

            var all = container.Resolve<IEnumerable<ITest1>>();

            Assert.Equal(3, all.Count());
        }

        [Fact]
        public void RegistersTests_RegistersAs_Selector()
        {
            IStashboxContainer container = new StashboxContainer();
            container.RegisterTypesAs<ITest1>(new[] { typeof(Test1), typeof(Test11), typeof(Test12) }, type => type == typeof(Test12));

            var all = container.Resolve<IEnumerable<ITest1>>();

            Assert.Single(all);
        }

        [Fact]
        public void RegistersTests_RegistersAsSelf_Selector()
        {
            IStashboxContainer container = new StashboxContainer();
            container.RegisterTypes(new[] { typeof(Test1), typeof(Test11), typeof(Test12) }, type => type == typeof(Test12));

            Assert.NotNull(container.Resolve<Test12>());
        }

        [Fact]
        public void RegistersTests_RegistersAs_Scoped()
        {
            IStashboxContainer container = new StashboxContainer();
            container.RegisterTypesAs<ITest1>(new[] { typeof(Test1), typeof(Test11), typeof(Test12), typeof(Test2) }, configurator: context => context.WithScopedLifetime());

            var regs = container.ContainerContext.RegistrationRepository.GetRegistrationMappings().ToArray();

            Assert.Equal(3, regs.Length);
            Assert.True(regs.All(reg => reg.Value.RegistrationContext.Lifetime is ScopedLifetime));
        }

        [Fact]
        public void RegistersTests_RegistersAsSelf_Scoped_Selector()
        {
            IStashboxContainer container = new StashboxContainer();
            container.RegisterTypes(new[] { typeof(Test1), typeof(Test11), typeof(Test12) }, type => type == typeof(Test12), configurator: context => context.WithScopedLifetime());

            var regs = container.ContainerContext.RegistrationRepository.GetRegistrationMappings().ToArray();

            Assert.Equal(4, regs.Length);
            Assert.True(regs.All(reg => reg.Value.RegistrationContext.Lifetime is ScopedLifetime));
        }

        [Fact]
        public void RegistersTests_Registers()
        {
            IStashboxContainer container = new StashboxContainer();
            container.RegisterTypes(new[] { typeof(Test), typeof(Test1), typeof(Test11), typeof(Test12) });

            var test = container.ResolveAll<ITest>();
            var test1 = container.ResolveAll<ITest1>();
            var test2 = container.ResolveAll<ITest2>();

            Assert.Equal(3, test.Count());
            Assert.Equal(3, test1.Count());
            Assert.Equal(2, test2.Count());
        }

        [Fact]
        public void RegistersTests_Registers_Selector()
        {
            IStashboxContainer container = new StashboxContainer();
            container.RegisterTypes(new[] { typeof(Test), typeof(Test1), typeof(Test11), typeof(Test12) }, type => type == typeof(Test12));

            var test = container.ResolveAll<ITest>();
            var test1 = container.ResolveAll<ITest1>();
            var test2 = container.ResolveAll<ITest2>();

            Assert.Single(test);
            Assert.Single(test1);
            Assert.Single(test2);
        }

        [Fact]
        public void RegistersTests_Registers_Configurator()
        {
            IStashboxContainer container = new StashboxContainer();
            container.RegisterTypes(new[] { typeof(Test), typeof(Test1), typeof(Test11), typeof(Test12) }, configurator: context =>
            {
                if (context.ServiceType == typeof(ITest2))
                    context.WithScopedLifetime();
            });

            using var scope = container.BeginScope();

            var test = scope.ResolveAll<ITest>();
            var test1 = scope.ResolveAll<ITest1>();
            var test2 = scope.ResolveAll<ITest2>();

            var scopeds = container.ContainerContext.RegistrationRepository.GetRegistrationMappings().Where(r => r.Value.RegistrationContext.Lifetime is ScopedLifetime).ToArray();

            Assert.Equal(3, test.Count());
            Assert.Equal(3, test1.Count());
            Assert.Equal(2, test2.Count());
            Assert.Equal(2, scopeds.Length);
        }

        [Fact]
        public void RegistersTests_ComposeBy()
        {
            IStashboxContainer container = new StashboxContainer();
            container.ComposeBy(typeof(TestCompositionRoot));

            var regs = container.ContainerContext.RegistrationRepository.GetRegistrationMappings().OrderBy(r => r.Value.RegistrationId).ToArray();

            Assert.Equal(2, regs.Length);
            Assert.Same(regs[0].Value.ImplementationType, typeof(Test));
            Assert.Same(regs[1].Value.ImplementationType, typeof(Test1));
        }

        [Fact]
        public void RegistersTests_ComposeBy_Throw_DoesntImplement_ICompositionRoot()
        {
            IStashboxContainer container = new StashboxContainer();
            Assert.Throws<ArgumentException>(() => container.ComposeBy(typeof(Test)));
        }

        [Fact]
        public void RegistersTests_ComposeBy_Generic()
        {
            IStashboxContainer container = new StashboxContainer();
            container.ComposeBy<TestCompositionRoot>();

            var regs = container.ContainerContext.RegistrationRepository.GetRegistrationMappings().OrderBy(r => r.Value.RegistrationId).ToArray();

            Assert.Equal(2, regs.Length);
            Assert.Same(regs[0].Value.ImplementationType, typeof(Test));
            Assert.Same(regs[1].Value.ImplementationType, typeof(Test1));
        }

        [Fact]
        public void RegistersTests_ComposeAssembly()
        {
            IStashboxContainer container = new StashboxContainer();
            container.ComposeAssembly(this.GetType().GetTypeInfo().Assembly, type => !type.FullName.Contains("IssueTests"));

            var regs = container
                .GetRegistrationMappings()
                .OrderBy(r => r.Value.RegistrationId)
                .ToArray();

            Assert.Equal(4, regs.Length);
            Assert.Same(regs[0].Value.ImplementationType, typeof(Test));
            Assert.Same(regs[1].Value.ImplementationType, typeof(Test1));
            Assert.Same(regs[2].Value.ImplementationType, typeof(Test11));
            Assert.Same(regs[3].Value.ImplementationType, typeof(Test12));
        }

        [Fact]
        public void RegistersTests_ComposeAssemblies()
        {
            IStashboxContainer container = new StashboxContainer();
            container.ComposeAssemblies(new[] { this.GetType().GetTypeInfo().Assembly }, type => !type.FullName.Contains("IssueTests"));

            var regs = container.ContainerContext.RegistrationRepository
                .GetRegistrationMappings()
                .OrderBy(r => r.Value.RegistrationId)
                .ToArray();

            Assert.Equal(4, regs.Length);
            Assert.Same(regs[0].Value.ImplementationType, typeof(Test));
            Assert.Same(regs[1].Value.ImplementationType, typeof(Test1));
            Assert.Same(regs[2].Value.ImplementationType, typeof(Test11));
            Assert.Same(regs[3].Value.ImplementationType, typeof(Test12));
        }

        [Fact]
        public void RegistersTests_ComposeAssembly_CompositionRootNotFound()
        {
            IStashboxContainer container = new StashboxContainer();
            Assert.Throws<CompositionRootNotFoundException>(() =>
            container.ComposeAssemblies(new[] { typeof(IStashboxContainer).GetTypeInfo().Assembly }));
        }

        [Fact]
        public void RegistersTests_AsImplementedTypes_Interfaces()
        {
            var container = new StashboxContainer();
            container.Register<Test12>(context => context.AsImplementedTypes());

            var regs = container.ContainerContext.RegistrationRepository.GetRegistrationMappings().OrderBy(r => r.Key.Name).ToArray();

            Assert.Equal(4, regs.Length);
            Assert.Same(regs[0].Key, typeof(ITest));
            Assert.Same(regs[1].Key, typeof(ITest1));
            Assert.Same(regs[2].Key, typeof(ITest2));
            Assert.Same(regs[3].Key, typeof(Test12));
        }

        [Fact]
        public void RegistersTests_AsImplementedTypes_Interfaces_NoDuplicate()
        {
            var container = new StashboxContainer();
            container.Register<Test12>(context => context.AsImplementedTypes().AsImplementedTypes().AsServiceAlso<ITest1>().AsImplementedTypes());

            var regs = container.ContainerContext.RegistrationRepository.GetRegistrationMappings().OrderBy(r => r.Key.Name).ToArray();

            Assert.Equal(4, regs.Length);
            Assert.Same(regs[0].Key, typeof(ITest));
            Assert.Same(regs[1].Key, typeof(ITest1));
            Assert.Same(regs[2].Key, typeof(ITest2));
            Assert.Same(regs[3].Key, typeof(Test12));
        }

        [Fact]
        public void RegistersTests_AsImplementedTypes_Interfaces_ReMap()
        {
            var container = new StashboxContainer();
            container.Register<Test12>(context => context.AsImplementedTypes());

            var regs = container.ContainerContext.RegistrationRepository.GetRegistrationMappings().OrderBy(r => r.Key.Name).ToArray();

            container.ReMap<Test12>(context => context.AsImplementedTypes());

            var regs2 = container.ContainerContext.RegistrationRepository.GetRegistrationMappings().OrderBy(r => r.Key.Name).ToArray();

            Assert.Equal(regs.Length, regs2.Length);
            Assert.NotEqual(regs[0].Value.RegistrationId, regs2[0].Value.RegistrationId);
            Assert.NotEqual(regs[1].Value.RegistrationId, regs2[1].Value.RegistrationId);
            Assert.NotEqual(regs[2].Value.RegistrationId, regs2[2].Value.RegistrationId);
            Assert.NotEqual(regs[3].Value.RegistrationId, regs2[3].Value.RegistrationId);
        }

        [Fact]
        public void RegistersTests_AsImplementedTypes_BaseType()
        {
            var container = new StashboxContainer();
            container.Register(typeof(Test14), context => context.AsImplementedTypes());

            var regs = container.ContainerContext.RegistrationRepository.GetRegistrationMappings().OrderBy(r => r.Key.Name).ToArray();

            Assert.Equal(6, regs.Length);
            Assert.Same(regs[0].Key, typeof(ITest));
            Assert.Same(regs[1].Key, typeof(ITest1));
            Assert.Same(regs[2].Key, typeof(ITest2));
            Assert.Same(regs[3].Key, typeof(Test12));
            Assert.Same(regs[4].Key, typeof(Test13));
            Assert.Same(regs[5].Key, typeof(Test14));
        }

        [Fact]
        public void RegistersTests_AsImplementedTypes_BaseType_ReMap()
        {
            var container = new StashboxContainer();
            container.Register<Test14>(context => context.AsImplementedTypes());

            var regs = container.ContainerContext.RegistrationRepository.GetRegistrationMappings().OrderBy(r => r.Key.Name).ToArray();

            container.ReMap<Test14>(context => context.AsImplementedTypes());

            var regs2 = container.ContainerContext.RegistrationRepository.GetRegistrationMappings().OrderBy(r => r.Key.Name).ToArray();

            Assert.Equal(regs.Length, regs2.Length);
            Assert.NotEqual(regs[0].Value.RegistrationId, regs2[0].Value.RegistrationId);
            Assert.NotEqual(regs[1].Value.RegistrationId, regs2[1].Value.RegistrationId);
            Assert.NotEqual(regs[2].Value.RegistrationId, regs2[2].Value.RegistrationId);
            Assert.NotEqual(regs[3].Value.RegistrationId, regs2[3].Value.RegistrationId);
            Assert.NotEqual(regs[4].Value.RegistrationId, regs2[4].Value.RegistrationId);
            Assert.NotEqual(regs[5].Value.RegistrationId, regs2[5].Value.RegistrationId);
        }

        [Fact]
        public void RegistersTests_Generic_ByInterface()
        {
            var container = new StashboxContainer();
            container.RegisterTypesAs(typeof(IGenTest<>), typeof(IGenTest<>).GetTypeInfo().Assembly);

            var regs = container.ContainerContext.RegistrationRepository.GetRegistrationMappings().OrderBy(r => r.Value.RegistrationId).ToArray();

            Assert.Equal(7, regs.Length);
            Assert.Equal(typeof(IGenTest<>), regs[0].Key);
            Assert.Equal(typeof(IGenTest<int>), regs[1].Key);
            Assert.Equal(typeof(IGenTest<double>), regs[2].Key);
            Assert.Equal(typeof(IGenTest<object>), regs[3].Key);
            Assert.Equal(typeof(IGenTest<int>), regs[4].Key);
            Assert.Equal(typeof(IGenTest<double>), regs[5].Key);
            Assert.Equal(typeof(IGenTest<object>), regs[6].Key);

            Assert.Equal(typeof(GenTest<>), regs[0].Value.ImplementationType);
            Assert.Equal(typeof(GenTest1), regs[1].Value.ImplementationType);
            Assert.Equal(typeof(GenTest2), regs[2].Value.ImplementationType);
            Assert.Equal(typeof(GenTest3), regs[3].Value.ImplementationType);
            Assert.Equal(typeof(GenTest4), regs[4].Value.ImplementationType);
            Assert.Equal(typeof(GenTest5), regs[5].Value.ImplementationType);
            Assert.Equal(typeof(GenTest6), regs[6].Value.ImplementationType);
        }

        [Fact]
        public void RegistersTests_Generic_ByBase()
        {
            var container = new StashboxContainer();
            container.RegisterTypesAs(typeof(GenTest<>), typeof(IGenTest<>).GetTypeInfo().Assembly);

            var regs = container.ContainerContext.RegistrationRepository.GetRegistrationMappings().OrderBy(r => r.Value.RegistrationId).ToArray();

            Assert.Equal(4, regs.Length);
            Assert.Equal(typeof(GenTest<>), regs[0].Key);
            Assert.Equal(typeof(GenTest<int>), regs[1].Key);
            Assert.Equal(typeof(GenTest<double>), regs[2].Key);
            Assert.Equal(typeof(GenTest<object>), regs[3].Key);

            Assert.Equal(typeof(GenTest<>), regs[0].Value.ImplementationType);
            Assert.Equal(typeof(GenTest1), regs[1].Value.ImplementationType);
            Assert.Equal(typeof(GenTest2), regs[2].Value.ImplementationType);
            Assert.Equal(typeof(GenTest3), regs[3].Value.ImplementationType);
        }

        [Fact]
        public void RegistersTests_AsServiceAlso_Generic_Fail()
        {
            var container = new StashboxContainer();
            Assert.Throws<ArgumentException>(() => container.Register<Test1>(context => context.AsServiceAlso<Test2>()));
        }

        [Fact]
        public void RegistersTests_AsServiceAlso_Fail()
        {
            var container = new StashboxContainer();
            Assert.Throws<ArgumentException>(() => container.Register(typeof(Test1), context => context.AsServiceAlso<Test2>()));
        }

        [Fact]
        public void RegistersTests_AsServiceAlso_Transient()
        {
            var container = new StashboxContainer();
            container.Register<ITest, Test1>(context => context.AsServiceAlso<ITest1>());

            var inst = container.Resolve<ITest>();
            var inst1 = container.Resolve<ITest1>();
            Assert.NotNull(inst);
            Assert.NotNull(inst1);
            Assert.IsType<Test1>(inst);
            Assert.IsType<Test1>(inst1);
        }

        [Fact]
        public void RegistersTests_AsServiceAlso_Singleton()
        {
            var container = new StashboxContainer();
            container.Register(typeof(ITest), typeof(Test1), context => context.AsServiceAlso<ITest1>().WithSingletonLifetime());

            var inst = container.Resolve<ITest>();
            var inst1 = container.Resolve<ITest1>();
            Assert.Same(inst, inst1);
        }

        interface ITest { }

        interface ITest1 { }

        interface ITest2 { }

        class Test : ITest { }

        interface IGenTest<T> { }

        class GenTest<T> : IGenTest<T> { }

        class GenTest1 : GenTest<int> { }

        class GenTest2 : GenTest<double> { }

        class GenTest3 : GenTest<object> { }

        class GenTest4 : IGenTest<int> { }

        class GenTest5 : IGenTest<double> { }

        class GenTest6 : IGenTest<object> { }

        class Test2 { }

        class Test1 : ITest, ITest1 { }

        class Test11 : ITest1, ITest2 { }

        class Test12 : ITest, ITest1, ITest2 { }

        class Test13 : Test12 { }

        class Test14 : Test13 { }

        class TestCompositionRoot : ICompositionRoot
        {
            public void Compose(IStashboxContainer container)
            {
                container.Register<ITest, Test>();
                container.Register<ITest1, Test1>();
            }
        }

        class TestCompositionRoot2 : ICompositionRoot
        {
            public void Compose(IStashboxContainer container)
            {
                container.Register<ITest1, Test11>();
                container.Register<ITest2, Test12>();
            }
        }
    }
}
