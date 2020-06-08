namespace Stashbox.Utils
{
    internal class Pair<T1, T2>
    {
        public T1 I1 { get; set; }

        public T2 I2 { get; set; }

        public Pair(T1 item1, T2 item2)
        {
            this.I1 = item1;
            this.I2 = item2;
        }
    }
}
