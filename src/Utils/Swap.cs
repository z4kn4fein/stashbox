using System;
using System.Threading;

namespace Stashbox.Utils
{
    internal static class Swap
    {
        public static void SwapValue<T1, T2, T3, T4, TValue>(ref TValue refValue, Func<T1, T2, T3, T4, TValue, TValue> valueFactory, T1 t1, T2 t2, T3 t3, T4 t4)
            where TValue : class
        {
            var currentValue = refValue;
            var newValue = valueFactory(t1, t2, t3, t4, currentValue);

            if (!TrySwapCurrent(ref refValue, currentValue, newValue))
                SwapCurrent(ref refValue, valueFactory, t1, t2, t3, t4);
        }

        public static bool TrySwapCurrent<TValue>(ref TValue refValue, TValue currentValue, TValue newValue)
            where TValue : class =>
            ReferenceEquals(Interlocked.CompareExchange(ref refValue, newValue, currentValue), currentValue);

        public static void SwapCurrent<T1, T2, T3, T4, TValue>(ref TValue refValue, Func<T1, T2, T3, T4, TValue, TValue> valueFactory, T1 t1, T2 t2, T3 t3, T4 t4)
            where TValue : class
        {
            var wait = new SpinWait();
            TValue currentValue;
            TValue newValue;
            var counter = 0;
            var swapThreshold = Environment.ProcessorCount * 6;

            do
            {
                if (counter > 0)
                    wait.SpinOnce();

                if (++counter > swapThreshold)
                    throw new InvalidOperationException("Swap quota exceeded.");
                
                currentValue = refValue;
                newValue = valueFactory(t1, t2, t3, t4, currentValue);
            } while (!ReferenceEquals(Interlocked.CompareExchange(ref refValue, newValue, currentValue), currentValue));
        }
    }
}
