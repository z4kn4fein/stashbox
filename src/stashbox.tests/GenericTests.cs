using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ronin.Common;

namespace Stashbox.Tests
{
    [TestClass]
    public class GenericTests
    {
        [TestMethod]
        public void GenericTests_ResolveTest()
        {
            var container = new StashboxContainer();

            container.RegisterType(typeof(Test1<,>), typeof(ITest1<,>));
            var inst = container.Resolve<ITest1<int, string>>();

            Shield.EnsureNotNull(inst);
            Shield.EnsureTypeOf<Test1<int, string>>(inst);
        }
    }

    public interface ITest1<I, K>
    {
        I IProp { get; }
        K KProp { get; }
    }

    public class Test1<I, K> : ITest1<I, K>
    {
        public I IProp { get; }
        public K KProp { get; }
    }
}
