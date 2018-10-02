using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace Stashbox.Tests
{
    [TestClass]
    public class DisposeOrderTests
    {
        [TestMethod]
        public void Ensure_Services_Are_Disposed_In_The_Right_Order()
        {
            var disposables = new List<IDisposable>();
            using (var container = new StashboxContainer(config => config.WithDisposableTransientTracking()))
            {
                var obj = container.Register<DisposableObj1>()
                    .Register<DisposableObj2>()
                    .Register<DisposableObj3>()
                    .Resolve<DisposableObj3>(dependencyOverrides: new object[] { disposables });

                Assert.IsNotNull(obj);
            }

            Assert.IsInstanceOfType(disposables[0], typeof(DisposableObj3));
            Assert.IsInstanceOfType(disposables[1], typeof(DisposableObj1));
            Assert.IsInstanceOfType(disposables[2], typeof(DisposableObj2));
            Assert.IsInstanceOfType(disposables[3], typeof(DisposableObj1));
        }

        [TestMethod]
        public void Ensure_Services_Are_Disposed_In_The_Right_Order_InScope()
        {
            var disposables = new List<IDisposable>();
            using (var container = new StashboxContainer(config => config.WithDisposableTransientTracking()))
            {
                container.Register<DisposableObj1>()
                    .Register<DisposableObj2>()
                    .Register<DisposableObj3>();

                using (var scope = container.BeginScope())
                {
                    var obj = scope.Resolve<DisposableObj3>(dependencyOverrides: new object[] { disposables });
                    Assert.IsNotNull(obj);
                }
            }

            Assert.IsInstanceOfType(disposables[0], typeof(DisposableObj3));
            Assert.IsInstanceOfType(disposables[1], typeof(DisposableObj1));
            Assert.IsInstanceOfType(disposables[2], typeof(DisposableObj2));
            Assert.IsInstanceOfType(disposables[3], typeof(DisposableObj1));
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
