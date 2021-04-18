using System;
using System.Threading;

namespace Stashbox.Utils
{
    internal static class Swap
    {
        private const int MinimumSwapThreshold = 50;

        public static bool SwapValue<T1, T2, T3, T4, TValue>(ref TValue refValue, Func<T1, T2, T3, T4, TValue, TValue> valueFactory, T1 t1, T2 t2, T3 t3, T4 t4)
            where TValue : class
        {
            var currentValue = refValue;
            var newValue = valueFactory(t1, t2, t3, t4, currentValue);

            var original = Interlocked.CompareExchange(ref refValue, newValue, currentValue);

            if (ReferenceEquals(original, currentValue))
                return !ReferenceEquals(newValue, currentValue);

            return RepeatSwap(ref refValue, valueFactory, t1, t2, t3, t4);
        }

        private static bool RepeatSwap<T1, T2, T3, T4, TValue>(ref TValue refValue, Func<T1, T2, T3, T4, TValue, TValue> valueFactory, T1 t1, T2 t2, T3 t3, T4 t4)
            where TValue : class
        {
            var wait = new SpinWait();
            var desiredThreshold = Environment.ProcessorCount * 6;
            var swapThreshold = desiredThreshold <= MinimumSwapThreshold ? MinimumSwapThreshold : desiredThreshold;

            while (true)
            {
                var currentValue = refValue;
                var newValue = valueFactory(t1, t2, t3, t4, currentValue);

                var original = Interlocked.CompareExchange(ref refValue, newValue, currentValue);

                if (ReferenceEquals(original, currentValue))
                    return !ReferenceEquals(newValue, currentValue);

                if (wait.Count > swapThreshold)
                    throw new InvalidOperationException("Swap quota exceeded.");

                wait.SpinOnce();
            }
        }
    }
}
