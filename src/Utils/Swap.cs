using System;
using System.Threading;

namespace Stashbox.Utils
{
    internal static class Swap
    {
        public static void SwapValue<TValue>(ref TValue refValue, Func<TValue, TValue> valueFactory)
            where TValue : class
        {
            var currentValue = refValue;
            var newValue = valueFactory(currentValue);

            if (!TrySwapCurrent(ref refValue, currentValue, newValue))
                SwapCurrent(ref refValue, valueFactory);
        }
        
        public static bool TrySwapCurrent<TValue>(ref TValue refValue, TValue currentValue, TValue newValue)
            where TValue : class =>
            ReferenceEquals(Interlocked.CompareExchange(ref refValue, newValue, currentValue), currentValue);
        
        public static void SwapCurrent<TValue>(ref TValue refValue, Func<TValue, TValue> valueFactory)
            where TValue : class
        {
            var wait = new SpinWait();
            TValue currentValue;
            TValue newValue;
            var counter = 0;

            do
            {
                counter++;

                if (counter > 50)
                    throw new InvalidOperationException("Swap quota exceeded.");

                if (counter > 20)
                    wait.SpinOnce();

                currentValue = refValue;
                newValue = valueFactory(currentValue);
            } while (!ReferenceEquals(Interlocked.CompareExchange(ref refValue, newValue, currentValue), currentValue));
        }
    }
}
