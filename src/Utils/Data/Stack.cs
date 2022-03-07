using System;
using System.Collections.Generic;
using System.Linq;
#nullable disable

namespace Stashbox.Utils.Data
{
    internal class Stack<TValue> : ExpandableArray<TValue>
    {
        public new static Stack<TValue> FromEnumerable(IEnumerable<TValue> enumerable) =>
            new(enumerable);

        public Stack()
        { }

        public Stack(IEnumerable<TValue> initial)
            : base(initial.CastToArray())
        { }

        public TValue Pop()
        {
            if (this.Length == 0)
                return default;

            var result = base.Repository[this.Length - 1];
            base.Repository[--this.Length] = default;
            return result;
        }

        public TValue Front() => this.Length == 0 ? default : base.Repository[this.Length - 1];

        public void PushBack(TValue item)
        {
            if (this.Length == 0)
            {
                this.Length = 1;
                this.Repository = new[] { item };
                return;
            }

            this.EnsureSize();
            var newArray = new TValue[this.Length];
            newArray[0] = item;
            Array.Copy(this.Repository, 0, newArray, 1, this.Length - 1);
            this.Repository = newArray;
        }

        public TValue PeekBack() => this.Length == 0 ? default : this.Repository[0];

        public void ReplaceBack(TValue value) => this.Repository[0] = value;
    }
}
