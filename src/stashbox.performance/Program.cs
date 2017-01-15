using Stashbox;
using Stashbox.Attributes;
using System;
using System.Collections.Generic;

namespace stashbox.performance
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var container = new StashboxContainer())
            {
                //container.PrepareType<IFirstService, FirstService>().WithLifetime(new SingletonLifetime()).Register();
                //container.PrepareType<ISecondService, SecondService>().WithLifetime(new SingletonLifetime()).Register();
                //container.PrepareType<IThirdService, ThirdService>().WithLifetime(new SingletonLifetime()).Register();
                //container.RegisterType<ISubObjectOne, SubObjectOne>();
                //container.RegisterType<ISubObjectTwo, SubObjectTwo>();
                //container.RegisterType<ISubObjectThree, SubObjectThree>();
                //container.RegisterType<IComplex1, Complex1>();
                //container.RegisterType<IComplex2, Complex2>();
                //container.RegisterType<IComplex3, Complex3>();

                container.RegisterType<ISimpleAdapter, SimpleAdapterOne>("one");
                container.RegisterType<ISimpleAdapter, SimpleAdapterTwo>("two");
                container.RegisterType<ISimpleAdapter, SimpleAdapterThree>("three");
                container.RegisterType<ISimpleAdapter, SimpleAdapterFour>("four");
                container.RegisterType<ISimpleAdapter, SimpleAdapterFive>("five");

                container.RegisterType<ImportMultiple1>();
                container.RegisterType<ImportMultiple2>();
                container.RegisterType<ImportMultiple3>();

                for (int i = 0; i < 1000000000; i++)
                {
                    var importMultiple1 = (ImportMultiple1)container.Resolve(typeof(ImportMultiple1));
                    var importMultiple2 = (ImportMultiple2)container.Resolve(typeof(ImportMultiple2));
                    var importMultiple3 = (ImportMultiple3)container.Resolve(typeof(ImportMultiple3));
                    //var complex1 = (IComplex1)container.Resolve(typeof(IComplex1));
                    //var complex2 = (IComplex2)container.Resolve(typeof(IComplex2));
                    //var complex3 = (IComplex3)container.Resolve(typeof(IComplex3));
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

        public interface ISimpleAdapter
        {
        }

        public class SimpleAdapterOne : ISimpleAdapter
        {
        }

        public class SimpleAdapterTwo : ISimpleAdapter
        {
        }

        public class SimpleAdapterThree : ISimpleAdapter
        {
        }

        public class SimpleAdapterFour : ISimpleAdapter
        {
        }

        public class SimpleAdapterFive : ISimpleAdapter
        {
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


        public class ImportMultiple1
        {
            private static int counter;

            public ImportMultiple1(IEnumerable<ISimpleAdapter> adapters)
            {
                if (adapters == null)
                {
                    throw new ArgumentNullException(nameof(adapters));
                }

                int adapterCount = 0;
                foreach (var adapter in adapters)
                {
                    if (adapter == null)
                    {
                        throw new ArgumentException("adapters item should be not null");
                    }

                    ++adapterCount;
                }

                if (adapterCount != 5)
                {
                    throw new ArgumentException("there should be 5 adapters and there where: " + adapterCount, nameof(adapters));
                }

                System.Threading.Interlocked.Increment(ref counter);
            }

            public static int Instances
            {
                get { return counter; }
                set { counter = value; }
            }
        }

        public class ImportMultiple2
        {
            private static int counter;

            public ImportMultiple2(IEnumerable<ISimpleAdapter> adapters)
            {
                if (adapters == null)
                {
                    throw new ArgumentNullException(nameof(adapters));
                }

                int adapterCount = 0;
                foreach (var adapter in adapters)
                {
                    if (adapter == null)
                    {
                        throw new ArgumentException("adapters item should be not null");
                    }

                    ++adapterCount;
                }

                if (adapterCount != 5)
                {
                    throw new ArgumentException("there should be 5 adapters and there where: " + adapterCount, nameof(adapters));
                }

                System.Threading.Interlocked.Increment(ref counter);
            }

            public static int Instances
            {
                get { return counter; }
                set { counter = value; }
            }
        }

        public class ImportMultiple3
        {
            private static int counter;

            public ImportMultiple3(IEnumerable<ISimpleAdapter> adapters)
            {
                if (adapters == null)
                {
                    throw new ArgumentNullException(nameof(adapters));
                }

                int adapterCount = 0;
                foreach (var adapter in adapters)
                {
                    if (adapter == null)
                    {
                        throw new ArgumentException("adapters item should be not null");
                    }

                    ++adapterCount;
                }

                if (adapterCount != 5)
                {
                    throw new ArgumentException("there should be 5 adapters and there where: " + adapterCount, nameof(adapters));
                }

                System.Threading.Interlocked.Increment(ref counter);
            }

            public static int Instances
            {
                get { return counter; }
                set { counter = value; }
            }
        }
    }
}
