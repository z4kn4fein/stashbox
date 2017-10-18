using System;
using System.Reflection;
using System.Reflection.Emit;
using Stashbox.BuildUp.Expressions.Compile;
using static System.Linq.Expressions.Expression;

namespace playground
{
    class Program
    {
        static void Main(string[] args)
        {
            var variable = Variable(typeof(int));
            var param = Parameter(typeof(int));
            var block = Block(new[] { variable },
                Assign(variable, Constant(5)),
                Lambda(
                    Block(
                        Assign(variable, param),
                        variable
                        ),
                    param)
            );

            Delegate d;
            var lambda = Lambda<Func<int, Func<int, int>>>(block, param).TryEmit(out d);

            var a = (Func<int, Func<int, int>>)d;
            var k = a(5);
            var g = k(2);

            DynamicModule.DynamicAssemblyBuilder.Value.Save("Stashbox.Dynamic.dll");
        }
    }
}
