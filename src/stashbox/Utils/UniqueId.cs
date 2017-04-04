using System;
using System.Text;
using System.Threading;

namespace Stashbox.Utils
{
    internal static class UniqueId
    {
        private static readonly char[] Base62Chars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz".ToCharArray();
        private static int seed = Environment.TickCount;
        private static readonly ThreadLocal<Random> Rand = new ThreadLocal<Random>(() => new Random(Interlocked.Increment(ref seed)));

        public static string New(int length)
        {
            var sb = new StringBuilder(length);

            for (var i = 0; i < length; i++)
                sb.Append(Base62Chars[Rand.Value.Next(62)]);

            return sb.ToString();
        }
    }
}
