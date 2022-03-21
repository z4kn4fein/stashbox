using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Stashbox.Resolution.Wrappers
{
    internal class EnumerableWrapper : IEnumerableWrapper
    {
        public Expression WrapExpression(TypeInformation originalTypeInformation,
            TypeInformation wrappedTypeInformation,
            IEnumerable<ServiceContext> serviceContexts) =>
            wrappedTypeInformation.Type.InitNewArray(serviceContexts.Select(e => e.ServiceExpression));

        public bool TryUnWrap(TypeInformation typeInformation, out TypeInformation unWrappedType)
        {
            var enumerableType = typeInformation.Type.GetEnumerableType();
            if (enumerableType == null)
            {
                unWrappedType = default;
                return false;
            }

            unWrappedType = typeInformation.Clone(enumerableType);
            return true;
        }
    }
}
