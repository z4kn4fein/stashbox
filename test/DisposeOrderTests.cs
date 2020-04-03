using Xunit;
using System;
using System.Collections.Generic;

namespace Stashbox.Tests
{
    
    public class DisposeOrderTests
    {
        [Fact]
        public void Ensure_Services_Are_Disposed_In_The_Right_Order()
        {
            var disposables = new List<IDisposable>();
            using (var container = new StashboxContainer(config => config.WithDisposableTransientTracking()))
            {
                var obj = container.Register<DisposableObj1>()
                    .Register<DisposableObj2>()
                    .Register<DisposableObj3>()
                    .Resolve<DisposableObj3>(dependencyOverrides: new object[] { disposables });

                Assert.NotNull(obj);
            }

            Assert.IsType<DisposableObj3>(disposables[0]);
            Assert.IsType<DisposableObj1>(disposables[1]);
            Assert.IsType<DisposableObj2>(disposables[2]);
            Assert.IsType<DisposableObj1>(disposables[3]);
        }

        [Fact]
        public void Ensure_Services_Are_Disposed_In_The_Right_Order_InScope()
        {
            var disposables = new List<IDisposable>();
            using (var container = new StashboxContainer(config => config.WithDisposableTransientTracking()))
            {
                container.Register<DisposableObj1>()
                    .Register<DisposableObj2>()
                    .Register<DisposableObj3>();

                using var scope = container.BeginScope();
                var obj = scope.Resolve<DisposableObj3>(dependencyOverrides: new object[] { disposables });
                Assert.NotNull(obj);
            }

            Assert.IsType<DisposableObj3>(disposables[0]);
            Assert.IsType<DisposableObj1>(disposables[1]);
            Assert.IsType<DisposableObj2>(disposables[2]);
            Assert.IsType<DisposableObj1>(disposables[3]);
        }


        private class DisposableObj1 : IDisposable
        {
            private readonly IList<IDisposable> disposables;

            public DisposableObj1(IList<IDisposable> disposables)
            {
                this.disposables = disposables;
            }

            public void Dispose()
            {
                this.disposables.Add(this);
            }
        }

        private class DisposableObj2 : IDisposable
        {
            private readonly IList<IDisposable> disposables;
            private readonly DisposableObj1 dObj;

            public DisposableObj2(IList<IDisposable> disposables, DisposableObj1 dObj)
            {
                this.disposables = disposables;
                this.dObj = dObj;
            }

            public void Dispose()
            {
                this.disposables.Add(this);
            }
        }

        private class DisposableObj3 : IDisposable
        {
            private readonly IList<IDisposable> disposables;
            private readonly DisposableObj2 dObj;
            private readonly DisposableObj1 dObj1;

            public DisposableObj3(IList<IDisposable> disposables, DisposableObj2 dObj, DisposableObj1 dObj1)
            {
                this.disposables = disposables;
                this.dObj = dObj;
                this.dObj1 = dObj1;
            }

            public void Dispose()
            {
                this.disposables.Add(this);
            }
        }
    }
}
