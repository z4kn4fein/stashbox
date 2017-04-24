using System;
using System.Runtime.CompilerServices;

namespace Stashbox.Utils
{
    internal static class NameGenerator
    {
        [MethodImpl(Constants.Inline)]
        public static object GetRegistrationName(Type typeTo, object name = null) => name ?? typeTo;
    }
}
