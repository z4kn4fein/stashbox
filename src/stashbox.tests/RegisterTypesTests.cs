using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Stashbox.Infrastructure;
using Stashbox.Lifetime;

namespace Stashbox.Tests
{
    [TestClass]
    public class RegisterTypesTests
    {
        [TestMethod]
        public void RegisterTypesTests_RegisterTypesAs()
        {
            IStashboxContainer container = new StashboxContainer();
            container.RegisterTypesAs<ITest1>(new[] { typeof(Test1), typeof(Test11), typeof(Test12) });

            var all = container.Resolve<IEnumerable<ITest1>>();

            Assert.AreEqual(3, all.Count());
        }

        [TestMethod]
        public void RegisterTypesTests_RegisterTypesAs_Selector()
        {
            IStashboxContainer container = new StashboxContainer();
            container.RegisterTypesAs<ITest1>(new[] { typeof(Test1), typeof(Test11), typeof(Test12) }, type => type == typeof(Test12));

            var all = container.Resolve<IEnumerable<ITest1>>();

            Assert.AreEqual(1, all.Count());
        }

        [TestMethod]
        public void RegisterTypesTests_RegisterTypesAsSelf_Selector()
        {
            IStashboxContainer container = new StashboxContainer();
            container.RegisterTypesAsSelf(new[] { typeof(Test1), typeof(Test11), typeof(Test12) }, type => type == typeof(Test12));

            Assert.IsNotNull(container.Resolve<Test12>());
        }

        [TestMethod]
        public void RegisterTypesTests_RegisterTypesAs_Scoped()
        {
            IStashboxContainer container = new StashboxContainer();
            container.RegisterTypesAs<ITest1>(new[] { typeof(Test1), typeof(Test11), typeof(Test12), typeof(Test2) }, configurator: context => context.WithScopedLifetime());

            var regs = container.ContainerContext.RegistrationRepository.GetAllRegistrations().ToArray();

            Assert.AreEqual(3, regs.Length);
            Assert.IsTrue(regs.All(reg => reg.RegistrationContext.Lifetime is ScopedLifetime));
        }

        [TestMethod]
        public void RegisterTypesTests_RegisterTypesAsSelf_Scoped_Selector()
        {
            IStashboxContainer container = new StashboxContainer();
            container.RegisterTypesAsSelf(new[] { typeof(Test1), typeof(Test11), typeof(Test12) }, type => type == typeof(Test12), context => context.WithScopedLifetime());

            var regs = container.ContainerContext.RegistrationRepository.GetAllRegistrations().ToArray();

            Assert.AreEqual(1, regs.Length);
            Assert.IsTrue(regs.All(reg => reg.RegistrationContext.Lifetime is ScopedLifetime));
        }

        [TestMethod]
        public void RegisterTypesTests_RegisterTypes()
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
        public void RegisterTypesTests_RegisterTypes_Selector()
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
        public void RegisterTypesTests_RegisterTypes_Configurator()
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
        public void RegisterTypesTests_RegisterAssembly()
        {
            IStashboxContainer container = new StashboxContainer();
            container.RegisterAssemblyContaining<ITest1>();
            
            var regs = container.ContainerContext.RegistrationRepository.GetAllRegistrations().ToArray();

            Assert.IsTrue(regs.Length > 0);
        }

        [TestMethod]
        public void RegisterTypesTests_RegisterAssemblies()
        {
            IStashboxContainer container = new StashboxContainer();

            var assembly1 = typeof(ITest1).GetTypeInfo().Assembly;
            var assembly2 = typeof(IStashboxContainer).GetTypeInfo().Assembly;

            container.RegisterAssemblies(new[] { assembly1, assembly2 });

            var regs = container.ContainerContext.RegistrationRepository.GetAllRegistrations();

            Assert.IsTrue(regs.Any(r => r.ServiceType == typeof(IStashboxContainer) || r.ServiceType == typeof(ITest1)));
        }

        [TestMethod]
        public void RegisterTypesTests_RegisterAssembly_Selector()
        {
            IStashboxContainer container = new StashboxContainer();
            container.RegisterAssemblyContaining<ITest1>(type => type == typeof(ITest2));

            var regs = container.ContainerContext.RegistrationRepository.GetAllRegistrations();

            Assert.IsTrue(regs.All(reg => reg.ServiceType == typeof(ITest2)));
        }

        [TestMethod]
        public void RegisterTypesTests_RegisterAssembly_Configurator()
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

        public interface ITest { }

        public interface ITest1 { }

        public interface ITest2 { }

        public class Test : ITest { }

        public class Test2 { }

        public class Test1 : ITest, ITest1 { }

        public class Test11 : ITest1, ITest2 { }

        public class Test12 : ITest, ITest1, ITest2 { }
    }
}
