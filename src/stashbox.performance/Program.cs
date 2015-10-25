using Stashbox;

namespace stashbox.performance
{
    class Program
    {
        static void Main(string[] args)
        {
            var container = new StashboxContainer();
            container.RegisterType<ITest1, Test1>();
            container.RegisterType<ITest2, Test2>();

            while (true)
            {
                var inst = container.Resolve<ITest2>();
            }
        }

        public interface ITest1 { }

        public interface ITest2 { }

        public class Test1 : ITest1 { }

        public class Test2 : ITest2
        {
            public Test2(ITest1 test1) { }
        }
    }
}
