using System;
using System.Threading;

namespace Stashbox.Utils
{
    internal class Swap<TValue> where TValue : class
    {
        private TValue storedValue;

        public TValue Value => this.storedValue;

        public Swap(TValue initialValue)
        {
            this.storedValue = initialValue;
        }

        public bool TrySwapCurrent(TValue currentValue, TValue newValue) =>
            Interlocked.CompareExchange(ref storedValue, newValue, currentValue) == currentValue;

        public void SwapCurrent(Func<TValue, TValue> valueFactory)
        {
            TValue currentValue;
            TValue newValue;
            var counter = 0;

            do
            {
                if (++counter > 20)
                    throw new InvalidOperationException("Swap quota exceeded.");

                currentValue = this.storedValue;
                newValue = valueFactory(currentValue);
            } while (Interlocked.CompareExchange(ref storedValue, newValue, currentValue) != currentValue);
        }
    }
}
