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
            var lambda = Lambda<Func<int, Func<int, int>>>(block, param).Body.TryEmit(out d, typeof(Func<int, int>), typeof(Func<int, int>), param);

            var a = (Func<int, Func<int, int>>)d.DynamicInvoke(4);
            var k = a(5);

            DynamicModule.DynamicAssemblyBuilder.Value.Save("Stashbox.Dynamic.dll");
        }
    }
}
