using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Stashbox.ContainerExtension;
using Stashbox.Registration;
using Stashbox.Resolution;
using System;

namespace Stashbox.Tests
{
    [TestClass]
    public class InitializerFinalizerTests
    {
        [TestMethod]
        public void InitializerTests_Interface_Method()
        {
            ITest test;
            using (var container = new StashboxContainer())
            {
                container.Register<ITest, Test>(context => context.WithInitializer((t, resolver) => t.Method()));
                test = container.Resolve<ITest>();
            }

            Assert.IsTrue(test.MethodCalled);
        }

        [TestMethod]
        public void InitializerTests_ImplOnly_Method()
        {
            ITest test;
            using (var container = new StashboxContainer())
            {
                container.Register<Test1>();
                container.Register<ITest, Test>(context => context.WithInitializer((t, resolver) => t.ImplMethod(resolver.Resolve<Test1>())));
                test = container.Resolve<ITest>();
            }

            Assert.IsTrue(test.MethodCalled);
        }

        [TestMethod]
        public void InitializerTests_With_Post_Build_Extension()
        {
            ITest test;
            using (var container = new StashboxContainer())
            {
                var post = new Mock<IPostBuildExtension>();

                post.Setup(p => p.PostBuild(It.IsAny<object>(), container.ContainerContext, It.IsAny<ResolutionContext>(),
                    It.IsAny<IServiceRegistration>(), It.IsAny<Type>())).Returns<object, IContainerContext, ResolutionContext, IServiceRegistration, Type>((o, c, r, sr, t) => o);

                container.RegisterExtension(post.Object);
                container.Register<Test1>();
                container.Register<ITest, Test>(context => context.WithInitializer((t, resolver) => t.ImplMethod(resolver.Resolve<Test1>())));
                test = container.Resolve<ITest>();

                post.Verify(p => p.PostBuild(It.IsAny<object>(), container.ContainerContext, It.IsAny<ResolutionContext>(), It.IsAny<IServiceRegistration>(), It.IsAny<Type>()), Times.Exactly(2));
            }

            Assert.IsTrue(test.MethodCalled);
        }

        [TestMethod]
        public void FinalizerTests_Register()
        {
            ITest test;
            using (var container = new StashboxContainer())
            {
                container.Register<ITest, Test>(context => context.WithFinalizer(t => t.Method()));
                test = container.Resolve<ITest>();
            }

            Assert.IsTrue(test.MethodCalled);
        }

        [TestMethod]
        public void FinalizerTests_Register_ByInterface()
        {
            ITest test;
            using (var container = new StashboxContainer())
            {
                container.Register<ITest>(typeof(Test), context => context.WithFinalizer(t => t.Method()));
                test = container.Resolve<ITest>();
            }

            Assert.IsTrue(test.MethodCalled);
        }

        [TestMethod]
        public void FinalizerTests_Register_ByImplementation()
        {
            Test test;
            using (var container = new StashboxContainer())
            {
                container.Register<Test>(context => context.WithFinalizer(t => t.Method()));
                test = container.Resolve<Test>();
            }

            Assert.IsTrue(test.MethodCalled);
        }

        [TestMethod]
        public void FinalizerTests_ReMap()
        {
            ITest test;
            using (var container = new StashboxContainer())
            {
                container.Register<ITest, Test>(context => context.WithFinalizer(t => t.Method()));
                container.ReMap<ITest, Test>(context => context.WithFinalizer(t => t.Method()));
                test = container.Resolve<ITest>();
            }

            Assert.IsTrue(test.MethodCalled);
        }

        [TestMethod]
        public void FinalizerTests_ReMap_ByInterface()
        {
            ITest test;
            using (var container = new StashboxContainer())
            {
                container.Register<ITest>(typeof(Test), context => context.WithFinalizer(t => t.Method()));
                container.ReMap<ITest>(typeof(Test), context => context.WithFinalizer(t => t.Method()));
                test = container.Resolve<ITest>();
            }

            Assert.IsTrue(test.MethodCalled);
        }

        [TestMethod]
        public void FinalizerTests_ReMap_ByImplementation()
        {
            Test test;
            using (var container = new StashboxContainer())
            {
                container.Register<Test>(context => context.WithFinalizer(t => t.Method()));
                container.ReMap<Test>(context => context.WithFinalizer(t => t.Method()));
                test = container.Resolve<Test>();
            }

            Assert.IsTrue(test.MethodCalled);
        }

        [TestMethod]
        public void FinalizerTests_Instance_Interface()
        {
            var test = new Test();
            using (var container = new StashboxContainer())
            {
                container.RegisterInstanceAs<ITest>(test, finalizerDelegate: t => t.Method());
                test = (Test)container.Resolve<ITest>();
            }

            Assert.IsTrue(test.MethodCalled);
        }

        [TestMethod]
        public void FinalizerTests_Instance_Implementation()
        {
            var test = new Test();
            using (var container = new StashboxContainer())
            {
                container.RegisterInstanceAs(test, finalizerDelegate: t => t.Method());
                test = container.Resolve<Test>();
            }

            Assert.IsTrue(test.MethodCalled);
        }

        [TestMethod]
        public void FinalizerTests_WireUp_Interface()
        {
            var test = new Test();
            using (var container = new StashboxContainer())
            {
                container.WireUpAs<ITest>(test, finalizerDelegate: t => t.Method());
                test = (Test)container.Resolve<ITest>();
            }

            Assert.IsTrue(test.MethodCalled);
        }

        [TestMethod]
        public void FinalizerTests_WireUp_Implementation()
        {
            var test = new Test();
            using (var container = new StashboxContainer())
            {
                container.WireUpAs(test, finalizerDelegate: t => t.Method());
                test = container.Resolve<Test>();
            }

            Assert.IsTrue(test.MethodCalled);
        }

        [TestMethod]
        public void FinalizerTests_Register_Multiple_Shouldnt_Throw()
        {
            using (var container = new StashboxContainer())
            {
                container.Register<ITest, Test>(context => context.WithFinalizer(t => t.Method()));
                for (var i = 0; i < 10; i++)
                {
                    var test = container.Resolve<ITest>();
                    Assert.IsFalse(test.MethodCalled);
                }
            }
        }

        [TestMethod]
        public void FinalizerTests_Register_Singleton_Multiple_Shouldnt_Throw()
        {
            using (var container = new StashboxContainer())
            {
                container.Register<ITest, Test>(context => context.WithFinalizer(t => t.Method()).WithSingletonLifetime());
                for (var i = 0; i < 10; i++)
                {
                    var test = container.Resolve<ITest>();
                    Assert.IsFalse(test.MethodCalled);
                }
            }
        }

        [TestMethod]
        public void FinalizerTests_Register_Scoped_Multiple_Shouldnt_Throw()
        {
            using (var container = new StashboxContainer())
            {
                container.Register<ITest, Test>(context => context.WithFinalizer(t => t.Method()).WithScopedLifetime());
                for (var i = 0; i < 10; i++)
                {
                    ITest test;
                    using (var scope = container.BeginScope())
                    {
                        test = scope.Resolve<ITest>();
                        Assert.IsFalse(test.MethodCalled);
                    }

                    Assert.IsTrue(test.MethodCalled);
                }
            }
        }

        [TestMethod]
        public void FinalizerTests_Instance_Implementation_Multiple_Shouldnt_Throw()
        {
            var test = new Test();
            using (var container = new StashboxContainer())
            {
                container.RegisterInstanceAs(test, finalizerDelegate: t => t.Method());
                for (var i = 0; i < 10; i++)
                {
                    var test1 = container.Resolve<Test>();
                    Assert.IsFalse(test1.MethodCalled);
                    Assert.AreSame(test, test1);
                }
            }

            Assert.IsTrue(test.MethodCalled);
        }

        interface ITest
        {
            bool MethodCalled { get; }

            void Method();
        }

        class Test1
        { }

        class Test : ITest
        {
            public void Method()
            {
                if (this.MethodCalled)
                    throw new Exception("Method called multiple times!");

                this.MethodCalled = true;
            }

            public void ImplMethod(Test1 t)
            {
                if (t == null)
                    throw new NullReferenceException();

                if (this.MethodCalled)
                    throw new Exception("Method called multiple times!");

                this.MethodCalled = true;
            }

            public bool MethodCalled { get; private set; }
        }
    }
}
