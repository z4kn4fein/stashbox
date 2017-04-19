using System;
using System.Threading;

namespace Stashbox.Utils
{
    internal static class Swap
    {
        public static bool TrySwapCurrent<TValue>(ref TValue refValue, TValue currentValue, TValue newValue)
            where TValue : class =>
            ReferenceEquals(Interlocked.CompareExchange(ref refValue, newValue, currentValue), currentValue);

        public static void SwapCurrent<TValue>(ref TValue refValue, Func<TValue, TValue> valueFactory)
            where TValue : class
        {
            TValue currentValue;
            TValue newValue;
            var counter = 0;

            do
            {
                if (++counter > 20)
                    throw new InvalidOperationException("Swap quota exceeded.");

                currentValue = refValue;
                newValue = valueFactory(currentValue);
            } while (!ReferenceEquals(Interlocked.CompareExchange(ref refValue, newValue, currentValue), currentValue));
        }
    }
}
