using Stashbox;
using Stashbox.Attributes;
using Stashbox.LifeTime;
using System;

namespace stashbox.performance
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var container = new StashboxContainer())
            {
                container.PrepareType<IFirstService, FirstService>().WithLifetime(new SingletonLifetime()).Register();
                container.PrepareType<ISecondService, SecondService>().WithLifetime(new SingletonLifetime()).Register();
                container.PrepareType<IThirdService, ThirdService>().WithLifetime(new SingletonLifetime()).Register();
                container.RegisterType<ISubObjectOne, SubObjectOne>();
                container.RegisterType<ISubObjectTwo, SubObjectTwo>();
                container.RegisterType<ISubObjectThree, SubObjectThree>();
                container.RegisterType<IComplex1, Complex1>();
                container.RegisterType<IComplex2, Complex2>();
                container.RegisterType<IComplex3, Complex3>();

                for (int i = 0; i < 10000000; i++)
                {
                    var complex1 = (IComplex1)container.Resolve(typeof(IComplex1));
                    var complex2 = (IComplex2)container.Resolve(typeof(IComplex2));
                    var complex3 = (IComplex3)container.Resolve(typeof(IComplex3));
                    //container.PrepareType<IFirstService, FirstService>().WithLifetime(new SingletonLifetime()).Register();
                    //container.PrepareType<ISecondService, SecondService>().WithLifetime(new SingletonLifetime()).Register();
                    //container.PrepareType<IThirdService, ThirdService>().WithLifetime(new SingletonLifetime()).Register();
                    //container.RegisterType<ISubObjectOne, SubObjectOne>();
                    //container.RegisterType<ISubObjectTwo, SubObjectTwo>();
                    //container.RegisterType<ISubObjectThree, SubObjectThree>();
                    //container.RegisterType<IComplex1, Complex1>();
                    //container.RegisterType<IComplex2, Complex2>();
                    //container.RegisterType<IComplex3, Complex3>();
                }
            }
            Console.Write("Done");
            Console.ReadKey();
        }

        public interface ITest1 { }

        public interface ITest2 { ITest1 Test1 { get; } }

        public class Test1 : ITest1 { }

        public class Test2 : ITest2
        {
            public Test2() { }

            [Dependency]
            public ITest1 Test1 { get; set; }
        }

        public interface IComplex1
        {
        }

        public interface IComplex2
        {
        }

        public interface IComplex3
        {
        }

        public class Complex1 : IComplex1
        {
            private static int counter;

            public Complex1(
                IFirstService firstService,
                ISecondService secondService,
                IThirdService thirdService,
                ISubObjectOne subObjectOne,
                ISubObjectTwo subObjectTwo,
                ISubObjectThree subObjectThree)
            {
                if (firstService == null)
                {
                    throw new ArgumentNullException(nameof(firstService));
                }

                if (secondService == null)
                {
                    throw new ArgumentNullException(nameof(secondService));
                }

                if (thirdService == null)
                {
                    throw new ArgumentNullException(nameof(thirdService));
                }

                if (subObjectOne == null)
                {
                    throw new ArgumentNullException(nameof(subObjectOne));
                }

                if (subObjectTwo == null)
                {
                    throw new ArgumentNullException(nameof(subObjectTwo));
                }

                if (subObjectThree == null)
                {
                    throw new ArgumentNullException(nameof(subObjectThree));
                }

                System.Threading.Interlocked.Increment(ref counter);
            }

            public static int Instances
            {
                get { return counter; }
                set { counter = value; }
            }
        }

        public class Complex2 : IComplex2
        {
            private static int counter;

            public Complex2(
                IFirstService firstService,
                ISecondService secondService,
                IThirdService thirdService,
                ISubObjectOne subObjectOne,
                ISubObjectTwo subObjectTwo,
                ISubObjectThree subObjectThree)
            {
                if (firstService == null)
                {
                    throw new ArgumentNullException(nameof(firstService));
                }

                if (secondService == null)
                {
                    throw new ArgumentNullException(nameof(secondService));
                }

                if (thirdService == null)
                {
                    throw new ArgumentNullException(nameof(thirdService));
                }

                if (subObjectOne == null)
                {
                    throw new ArgumentNullException(nameof(subObjectOne));
                }

                if (subObjectTwo == null)
                {
                    throw new ArgumentNullException(nameof(subObjectTwo));
                }

                if (subObjectThree == null)
                {
                    throw new ArgumentNullException(nameof(subObjectThree));
                }

                System.Threading.Interlocked.Increment(ref counter);
            }

            public static int Instances
            {
                get { return counter; }
                set { counter = value; }
            }
        }

        public class Complex3 : IComplex3
        {
            private static int counter;

            public Complex3(
                IFirstService firstService,
                ISecondService secondService,
                IThirdService thirdService,
                ISubObjectOne subObjectOne,
                ISubObjectTwo subObjectTwo,
                ISubObjectThree subObjectThree)
            {
                if (firstService == null)
                {
                    throw new ArgumentNullException(nameof(firstService));
                }

                if (secondService == null)
                {
                    throw new ArgumentNullException(nameof(secondService));
                }

                if (thirdService == null)
                {
                    throw new ArgumentNullException(nameof(thirdService));
                }

                if (subObjectOne == null)
                {
                    throw new ArgumentNullException(nameof(subObjectOne));
                }

                if (subObjectTwo == null)
                {
                    throw new ArgumentNullException(nameof(subObjectTwo));
                }

                if (subObjectThree == null)
                {
                    throw new ArgumentNullException(nameof(subObjectThree));
                }

                System.Threading.Interlocked.Increment(ref counter);
            }

            public static int Instances
            {
                get { return counter; }
                set { counter = value; }
            }
        }

        public interface IFirstService
        {
        }

        public class FirstService : IFirstService
        {
            public FirstService()
            {
            }
        }

        public interface ISecondService
        {
        }

        public class SecondService : ISecondService
        {
            public SecondService()
            {
            }
        }

        public interface IThirdService
        {
        }

        public class ThirdService : IThirdService
        {
            public ThirdService()
            {
            }
        }

        public interface ISubObjectOne
        {
        }

        public class SubObjectOne : ISubObjectOne
        {
            public SubObjectOne(IFirstService firstService)
            {
                if (firstService == null)
                {
                    throw new ArgumentNullException(nameof(firstService));
                }
            }
        }

        public interface ISubObjectTwo
        {
        }

        public class SubObjectTwo : ISubObjectTwo
        {
            public SubObjectTwo(ISecondService secondService)
            {
                if (secondService == null)
                {
                    throw new ArgumentNullException(nameof(secondService));
                }
            }
        }

        public interface ISubObjectThree
        {
        }

        public class SubObjectThree : ISubObjectThree
        {
            public SubObjectThree(IThirdService thirdService)
            {
                if (thirdService == null)
                {
                    throw new ArgumentNullException(nameof(thirdService));
                }
            }
        }
    }
}
