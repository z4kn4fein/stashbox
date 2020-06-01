using Stashbox.Attributes;
using Stashbox.Utils;
using System;
using Xunit;

namespace Stashbox.Tests
{

    public class BuildUpTests
    {
        [Fact]
        public void BuildUpTests_BuildUp()
        {
            using var container = new StashboxContainer();
            container.Register<ITest, Test>();

            var test1 = new Test1();
            container.WireUp<ITest1>(test1);

            var test2 = new Test2();
            var inst = container.BuildUp(test2);

            Assert.Equal(test2, inst);
            Assert.NotNull(inst);
            Assert.NotNull(inst.Test1);
            Assert.IsType<Test2>(inst);
            Assert.IsType<Test1>(inst.Test1);
            Assert.IsType<Test>(inst.Test1.Test);
        }

        [Fact]
        public void BuildUpTests_BuildUp_Scoped()
        {
            using var container = new StashboxContainer();
            container.Register<ITest3, Test3>().Register<ITest, Test>();
            var test3 = new Test3();
            var inst = (Test3)container.BuildUp<ITest3>(test3);

            Assert.NotNull(inst.Test);
        }

        [Fact]
        public void BuildUpTests_BuildUp_As_InterfaceType()
        {
            using var container = new StashboxContainer();
            container.RegisterScoped<ITest, Test>();

            var test1 = new Test1();
            using (var scope = container.BeginScope())
            {
                scope.BuildUp(test1);
                Assert.False(test1.Test.Disposed);
            }

            Assert.True(test1.Test.Disposed);

            using (var scope = container.BeginScope())
            {
                scope.BuildUp(test1);
                Assert.False(test1.Test.Disposed);
            }

            Assert.True(test1.Test.Disposed);
        }

        interface ITest : IDisposable { bool Disposed { get; } }

        interface ITest1 { ITest Test { get; } }

        interface ITest3 { }

        class Test : ITest
        {
            public void Dispose()
            {
                if (this.Disposed)
                    throw new ObjectDisposedException("disposed");

                this.Disposed = true;
            }

            public bool Disposed { get; private set; }
        }

        class Test1 : ITest1
        {
            [Dependency]
            public ITest Test { get; set; }

            [InjectionMethod]
            public void Init()
            {
                Shield.EnsureNotNull(this.Test, nameof(this.Test));
            }
        }

        class Test2
        {
            [Dependency]
            public ITest1 Test1 { get; set; }
        }

        class Test3 : ITest3
        {
            [Dependency]
            public ITest Test { get; set; }
        }
    }
}
