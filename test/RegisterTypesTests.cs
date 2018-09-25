using Microsoft.VisualStudio.TestTools.UnitTesting;
using Stashbox.Exceptions;
using Stashbox.Lifetime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Stashbox.Tests
{
    [TestClass]
    public class RegistersTests
    {
        [TestMethod]
        public void RegistersTests_RegistersAs()
        {
            IStashboxContainer container = new StashboxContainer();
            container.RegisterTypesAs<ITest1>(new[] { typeof(Test1), typeof(Test11), typeof(Test12) });

            var all = container.Resolve<IEnumerable<ITest1>>();

            Assert.AreEqual(3, all.Count());
        }

        [TestMethod]
        public void RegistersTests_RegistersAs_Selector()
        {
            IStashboxContainer container = new StashboxContainer();
            container.RegisterTypesAs<ITest1>(new[] { typeof(Test1), typeof(Test11), typeof(Test12) }, type => type == typeof(Test12));

            var all = container.Resolve<IEnumerable<ITest1>>();

            Assert.AreEqual(1, all.Count());
        }

        [TestMethod]
        public void RegistersTests_RegistersAsSelf_Selector()
        {
            IStashboxContainer container = new StashboxContainer();
            container.RegisterTypes(new[] { typeof(Test1), typeof(Test11), typeof(Test12) }, type => type == typeof(Test12));

            Assert.IsNotNull(container.Resolve<Test12>());
        }

        [TestMethod]
        public void RegistersTests_RegistersAs_Scoped()
        {
            IStashboxContainer container = new StashboxContainer();
            container.RegisterTypesAs<ITest1>(new[] { typeof(Test1), typeof(Test11), typeof(Test12), typeof(Test2) }, configurator: context => context.WithScopedLifetime());

            var regs = container.ContainerContext.RegistrationRepository.GetAllRegistrations().ToArray();

            Assert.AreEqual(3, regs.Length);
            Assert.IsTrue(regs.All(reg => reg.RegistrationContext.Lifetime is ScopedLifetime));
        }

        [TestMethod]
        public void RegistersTests_RegistersAsSelf_Scoped_Selector()
        {
            IStashboxContainer container = new StashboxContainer();
            container.RegisterTypes(new[] { typeof(Test1), typeof(Test11), typeof(Test12) }, type => type == typeof(Test12), context => context.WithScopedLifetime());

            var regs = container.ContainerContext.RegistrationRepository.GetAllRegistrations().ToArray();

            Assert.AreEqual(4, regs.Length);
            Assert.IsTrue(regs.All(reg => reg.RegistrationContext.Lifetime is ScopedLifetime));
        }

        [TestMethod]
        public void RegistersTests_Registers()
        {
            IStashboxContainer container = new StashboxContainer();
            container.RegisterTypes(new[] { typeof(Test), typeof(Test1), typeof(Test11), typeof(Test12) });

            var test = container.ResolveAll<ITest>();
            var test1 = container.ResolveAll<ITest1>();
            var test2 = container.ResolveAll<ITest2>();

            Assert.AreEqual(3, test.Count());
            Assert.AreEqual(3, test1.Count());
            Assert.AreEqual(2, test2.Count());
        }

        [TestMethod]
        public void RegistersTests_Registers_Selector()
        {
            IStashboxContainer container = new StashboxContainer();
            container.RegisterTypes(new[] { typeof(Test), typeof(Test1), typeof(Test11), typeof(Test12) }, type => type == typeof(Test12));

            var test = container.ResolveAll<ITest>();
            var test1 = container.ResolveAll<ITest1>();
            var test2 = container.ResolveAll<ITest2>();

            Assert.AreEqual(1, test.Count());
            Assert.AreEqual(1, test1.Count());
            Assert.AreEqual(1, test2.Count());
        }

        [TestMethod]
        public void RegistersTests_Registers_Configurator()
        {
            IStashboxContainer container = new StashboxContainer();
            container.RegisterTypes(new[] { typeof(Test), typeof(Test1), typeof(Test11), typeof(Test12) }, configurator: context =>
            {
                if (context.ServiceType == typeof(ITest2))
                    context.WithScopedLifetime();
            });

            var test = container.ResolveAll<ITest>();
            var test1 = container.ResolveAll<ITest1>();
            var test2 = container.ResolveAll<ITest2>();

            var scopeds = container.ContainerContext.RegistrationRepository.GetAllRegistrations().Where(r => r.RegistrationContext.Lifetime is ScopedLifetime).ToArray();

            Assert.AreEqual(3, test.Count());
            Assert.AreEqual(3, test1.Count());
            Assert.AreEqual(2, test2.Count());
            Assert.AreEqual(2, scopeds.Length);
        }

        [TestMethod]
        public void RegistersTests_RegisterAssembly()
        {
            IStashboxContainer container = new StashboxContainer();
            container.RegisterAssemblyContaining<ITest1>();

            var regs = container.ContainerContext.RegistrationRepository.GetAllRegistrations().ToArray();

            Assert.IsTrue(regs.Length > 0);
        }

        [TestMethod]
        public void RegistersTests_RegisterAssembly_AsSelf()
        {
            IStashboxContainer container = new StashboxContainer();
            container.RegisterAssemblyContaining<ITest1>();

            var regs = container.ContainerContext.RegistrationRepository.GetAllRegistrations().ToArray();

            Assert.IsTrue(regs.Length > 0);
            Assert.IsTrue(regs.Any(reg => reg.ServiceType == typeof(Test)));
        }

        [TestMethod]
        public void RegistersTests_RegisterAssemblies()
        {
            IStashboxContainer container = new StashboxContainer();

            var assembly1 = typeof(ITest1).GetTypeInfo().Assembly;
            var assembly2 = typeof(IStashboxContainer).GetTypeInfo().Assembly;

            container.RegisterAssemblies(new[] { assembly1, assembly2 });

            var regs = container.ContainerContext.RegistrationRepository.GetAllRegistrations();

            Assert.IsTrue(regs.Any(r => r.ServiceType == typeof(IStashboxContainer)));
            Assert.IsTrue(regs.Any(r => r.ServiceType == typeof(ITest1)));
        }

        [TestMethod]
        public void RegistersTests_RegisterAssemblies_AsSelf()
        {
            IStashboxContainer container = new StashboxContainer();

            var assembly1 = typeof(ITest1).GetTypeInfo().Assembly;
            var assembly2 = typeof(IStashboxContainer).GetTypeInfo().Assembly;

            container.RegisterAssemblies(new[] { assembly1, assembly2 });

            var regs = container.ContainerContext.RegistrationRepository.GetAllRegistrations();

            Assert.IsTrue(regs.Any(r => r.ServiceType == typeof(StashboxContainer)));
            Assert.IsTrue(regs.Any(r => r.ServiceType == typeof(Test1)));
        }

        [TestMethod]
        public void RegistersTests_RegisterAssembly_Selector()
        {
            IStashboxContainer container = new StashboxContainer();
            container.RegisterAssemblyContaining<ITest1>(type => type == typeof(Test));

            var regs = container.ContainerContext.RegistrationRepository.GetAllRegistrations();

            Assert.IsTrue(regs.Any(reg => reg.ServiceType == typeof(ITest)));
            Assert.IsTrue(regs.Any(reg => reg.ServiceType == typeof(Test)));
        }

        [TestMethod]
        public void RegistersTests_RegisterAssembly_Configurator()
        {
            IStashboxContainer container = new StashboxContainer();
            container.RegisterAssemblyContaining<ITest1>(configurator: context =>
            {
                if (context.ServiceType == typeof(ITest2))
                    context.WithScopedLifetime();
            });

            var regs = container.ContainerContext.RegistrationRepository.GetAllRegistrations().Where(r => r.RegistrationContext.Lifetime is ScopedLifetime).ToArray();

            Assert.IsTrue(regs.Length > 0);
        }

        [TestMethod]
        public void RegistersTests_RegisterAssembly_Configurator_AsSelf()
        {
            IStashboxContainer container = new StashboxContainer();
            container.RegisterAssemblyContaining<ITest1>(configurator: context =>
            {
                if (context.ServiceType == typeof(Test1))
                    context.WithScopedLifetime();
            });

            var regs = container.ContainerContext.RegistrationRepository.GetAllRegistrations().Where(r => r.RegistrationContext.Lifetime is ScopedLifetime).ToArray();

            Assert.IsTrue(regs.Length > 0);
        }

        [TestMethod]
        public void RegistersTests_ComposeBy()
        {
            IStashboxContainer container = new StashboxContainer();
            container.ComposeBy(typeof(TestCompositionRoot));

            var regs = container.ContainerContext.RegistrationRepository.GetAllRegistrations().OrderBy(r => r.RegistrationNumber).ToArray();

            Assert.AreEqual(2, regs.Length);
            Assert.AreSame(regs[0].ImplementationType, typeof(Test));
            Assert.AreSame(regs[1].ImplementationType, typeof(Test1));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void RegistersTests_ComposeBy_Throw_DoesntImplement_ICompositionRoot()
        {
            IStashboxContainer container = new StashboxContainer();
            container.ComposeBy(typeof(Test));
        }

        [TestMethod]
        public void RegistersTests_ComposeBy_Generic()
        {
            IStashboxContainer container = new StashboxContainer();
            container.ComposeBy<TestCompositionRoot>();

            var regs = container.ContainerContext.RegistrationRepository.GetAllRegistrations().OrderBy(r => r.RegistrationNumber).ToArray();

            Assert.AreEqual(2, regs.Length);
            Assert.AreSame(regs[0].ImplementationType, typeof(Test));
            Assert.AreSame(regs[1].ImplementationType, typeof(Test1));
        }

        [TestMethod]
        public void RegistersTests_ComposeAssembly()
        {
            IStashboxContainer container = new StashboxContainer();
            container.ComposeAssembly(this.GetType().GetTypeInfo().Assembly);

            var regs = container.ContainerContext.RegistrationRepository.GetAllRegistrations().OrderBy(r => r.RegistrationNumber).ToArray();

            Assert.AreEqual(4, regs.Length);
            Assert.AreSame(regs[0].ImplementationType, typeof(Test));
            Assert.AreSame(regs[1].ImplementationType, typeof(Test1));
            Assert.AreSame(regs[2].ImplementationType, typeof(Test11));
            Assert.AreSame(regs[3].ImplementationType, typeof(Test12));
        }

        [TestMethod]
        public void RegistersTests_ComposeAssemblies()
        {
            IStashboxContainer container = new StashboxContainer();
            container.ComposeAssemblies(new[] { this.GetType().GetTypeInfo().Assembly });

            var regs = container.ContainerContext.RegistrationRepository.GetAllRegistrations().OrderBy(r => r.RegistrationNumber).ToArray();

            Assert.AreEqual(4, regs.Length);
            Assert.AreSame(regs[0].ImplementationType, typeof(Test));
            Assert.AreSame(regs[1].ImplementationType, typeof(Test1));
            Assert.AreSame(regs[2].ImplementationType, typeof(Test11));
            Assert.AreSame(regs[3].ImplementationType, typeof(Test12));
        }

        [TestMethod]
        [ExpectedException(typeof(CompositionRootNotFoundException))]
        public void RegistersTests_ComposeAssembly_CompositionRootNotFound()
        {
            IStashboxContainer container = new StashboxContainer();
            container.ComposeAssemblies(new[] { typeof(IStashboxContainer).GetTypeInfo().Assembly });
        }

        [TestMethod]
        public void RegistersTests_AsImplementedTypes_Interfaces()
        {
            var container = new StashboxContainer();
            container.Register<Test12>(context => context.AsImplementedTypes());

            var regs = container.ContainerContext.RegistrationRepository.GetAllRegistrations().OrderBy(r => r.RegistrationNumber).ToArray();

            Assert.AreEqual(4, regs.Length);
            Assert.AreSame(regs[0].ServiceType, typeof(ITest));
            Assert.AreSame(regs[1].ServiceType, typeof(ITest1));
            Assert.AreSame(regs[2].ServiceType, typeof(ITest2));
            Assert.AreSame(regs[3].ServiceType, typeof(Test12));
        }

        [TestMethod]
        public void RegistersTests_AsImplementedTypes_Interfaces_ReMap()
        {
            var container = new StashboxContainer();
            container.Register<Test12>(context => context.AsImplementedTypes());

            var regs = container.ContainerContext.RegistrationRepository.GetAllRegistrations().OrderBy(r => r.RegistrationNumber).ToArray();

            container.ReMap<Test12>(context => context.AsImplementedTypes());

            var regs2 = container.ContainerContext.RegistrationRepository.GetAllRegistrations().OrderBy(r => r.RegistrationNumber).ToArray();

            Assert.AreEqual(regs.Length, regs2.Length);
            Assert.AreNotEqual(regs[0].RegistrationNumber, regs2[0].RegistrationNumber);
            Assert.AreNotEqual(regs[1].RegistrationNumber, regs2[1].RegistrationNumber);
            Assert.AreNotEqual(regs[2].RegistrationNumber, regs2[2].RegistrationNumber);
            Assert.AreNotEqual(regs[3].RegistrationNumber, regs2[3].RegistrationNumber);
        }

        [TestMethod]
        public void RegistersTests_AsImplementedTypes_BaseType()
        {
            var container = new StashboxContainer();
            container.Register<Test14>(context => context.AsImplementedTypes());

            var regs = container.ContainerContext.RegistrationRepository.GetAllRegistrations().OrderBy(r => r.RegistrationNumber).ToArray();

            Assert.AreEqual(6, regs.Length);
            Assert.AreSame(regs[0].ServiceType, typeof(ITest));
            Assert.AreSame(regs[1].ServiceType, typeof(ITest1));
            Assert.AreSame(regs[2].ServiceType, typeof(ITest2));
            Assert.AreSame(regs[3].ServiceType, typeof(Test13));
            Assert.AreSame(regs[4].ServiceType, typeof(Test12));
            Assert.AreSame(regs[5].ServiceType, typeof(Test14));
        }

        [TestMethod]
        public void RegistersTests_AsImplementedTypes_BaseType_ReMap()
        {
            var container = new StashboxContainer();
            container.Register<Test14>(context => context.AsImplementedTypes());

            var regs = container.ContainerContext.RegistrationRepository.GetAllRegistrations().OrderBy(r => r.RegistrationNumber).ToArray();

            container.ReMap<Test14>(context => context.AsImplementedTypes());

            var regs2 = container.ContainerContext.RegistrationRepository.GetAllRegistrations().OrderBy(r => r.RegistrationNumber).ToArray();

            Assert.AreEqual(regs.Length, regs2.Length);
            Assert.AreNotEqual(regs[0].RegistrationNumber, regs2[0].RegistrationNumber);
            Assert.AreNotEqual(regs[1].RegistrationNumber, regs2[1].RegistrationNumber);
            Assert.AreNotEqual(regs[2].RegistrationNumber, regs2[2].RegistrationNumber);
            Assert.AreNotEqual(regs[3].RegistrationNumber, regs2[3].RegistrationNumber);
            Assert.AreNotEqual(regs[4].RegistrationNumber, regs2[4].RegistrationNumber);
            Assert.AreNotEqual(regs[5].RegistrationNumber, regs2[5].RegistrationNumber);
        }

        [TestMethod]
        public void RegistersTests_Generic_ByInterface()
        {
            var container = new StashboxContainer();
            container.RegisterTypesAs(typeof(IGenTest<>), typeof(IGenTest<>).GetTypeInfo().Assembly);

            var regs = container.ContainerContext.RegistrationRepository.GetAllRegistrations().OrderBy(r => r.RegistrationNumber).ToArray();

            Assert.AreEqual(7, regs.Length);
            Assert.AreEqual(typeof(IGenTest<>), regs[0].ServiceType);
            Assert.AreEqual(typeof(IGenTest<int>), regs[1].ServiceType);
            Assert.AreEqual(typeof(IGenTest<double>), regs[2].ServiceType);
            Assert.AreEqual(typeof(IGenTest<object>), regs[3].ServiceType);
            Assert.AreEqual(typeof(IGenTest<int>), regs[4].ServiceType);
            Assert.AreEqual(typeof(IGenTest<double>), regs[5].ServiceType);
            Assert.AreEqual(typeof(IGenTest<object>), regs[6].ServiceType);

            Assert.AreEqual(typeof(GenTest<>), regs[0].ImplementationType);
            Assert.AreEqual(typeof(GenTest1), regs[1].ImplementationType);
            Assert.AreEqual(typeof(GenTest2), regs[2].ImplementationType);
            Assert.AreEqual(typeof(GenTest3), regs[3].ImplementationType);
            Assert.AreEqual(typeof(GenTest4), regs[4].ImplementationType);
            Assert.AreEqual(typeof(GenTest5), regs[5].ImplementationType);
            Assert.AreEqual(typeof(GenTest6), regs[6].ImplementationType);
        }

        [TestMethod]
        public void RegistersTests_Generic_ByBase()
        {
            var container = new StashboxContainer();
            container.RegisterTypesAs(typeof(GenTest<>), typeof(IGenTest<>).GetTypeInfo().Assembly);

            var regs = container.ContainerContext.RegistrationRepository.GetAllRegistrations().OrderBy(r => r.RegistrationNumber).ToArray();

            Assert.AreEqual(4, regs.Length);
            Assert.AreEqual(typeof(GenTest<>), regs[0].ServiceType);
            Assert.AreEqual(typeof(GenTest<int>), regs[1].ServiceType);
            Assert.AreEqual(typeof(GenTest<double>), regs[2].ServiceType);
            Assert.AreEqual(typeof(GenTest<object>), regs[3].ServiceType);

            Assert.AreEqual(typeof(GenTest<>), regs[0].ImplementationType);
            Assert.AreEqual(typeof(GenTest1), regs[1].ImplementationType);
            Assert.AreEqual(typeof(GenTest2), regs[2].ImplementationType);
            Assert.AreEqual(typeof(GenTest3), regs[3].ImplementationType);
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
