using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Stashbox.Utils
{
    internal sealed class AvlTreeKeyValue<TKey, TValue>
    {
        public static readonly AvlTreeKeyValue<TKey, TValue> Empty = new AvlTreeKeyValue<TKey, TValue>();

        private readonly int height;
        private readonly int storedHash;
        private readonly TKey storedKey;
        private readonly TValue storedValue;
        private readonly AvlTreeKeyValue<TKey, TValue> leftNode;
        private readonly AvlTreeKeyValue<TKey, TValue> rightNode;
        private readonly ArrayStoreKeyed<TKey, TValue> collisions;

        public bool IsEmpty = true;

        private AvlTreeKeyValue(int hash, TKey key, TValue value, AvlTreeKeyValue<TKey, TValue> left,
            AvlTreeKeyValue<TKey, TValue> right, ArrayStoreKeyed<TKey, TValue> collisions)
        {
            this.collisions = collisions;
            this.storedKey = key;
            this.storedHash = hash;
            this.leftNode = left;
            this.rightNode = right;
            this.storedValue = value;
            this.IsEmpty = false;
            this.height = 1 + (left.height > right.height ? left.height : right.height);
        }

        private AvlTreeKeyValue()
        { }

        private AvlTreeKeyValue(int hash, TKey key, TValue value)
        {
            this.storedKey = key;
            this.storedHash = hash;
            this.leftNode = Empty;
            this.rightNode = Empty;
            this.storedValue = value;
            this.IsEmpty = false;
            this.height = 1;
        }

        public AvlTreeKeyValue<TKey, TValue> AddOrUpdate(TKey key, TValue value, Func<TValue, TValue, TValue> updateDelegate = null) =>
            this.Add(key.GetHashCode(), key, value, updateDelegate, false);

        public AvlTreeKeyValue<TKey, TValue> AddOrUpdate(TKey key, TValue value, bool forceUpdate) =>
            this.Add(key.GetHashCode(), key, value, null, forceUpdate);

        [MethodImpl(Constants.Inline)]
        public TValue GetOrDefault(TKey key)
        {
            if (this.IsEmpty)
                return default;

            var hash = key.GetHashCode();
            var node = this;
            while (!node.IsEmpty && node.storedHash != hash)
                node = hash < node.storedHash ? node.leftNode : node.rightNode;
            return !node.IsEmpty && (ReferenceEquals(key, node.storedKey) || key.Equals(node.storedKey)) 
                ? node.storedValue 
                : node.collisions == null 
                    ? default
                    : node.collisions.GetOrDefault(key);
        }

        private AvlTreeKeyValue<TKey, TValue> Add(int hash, TKey key, TValue value, Func<TValue, TValue, TValue> updateDelegate, bool forceUpdate)
        {
            if (this.IsEmpty)
                return new AvlTreeKeyValue<TKey, TValue>(hash, key, value);

            if (hash == this.storedHash)
                return this.CheckCollision(hash, key, value, updateDelegate, forceUpdate);

            return hash < this.storedHash
                ? this.height == 1
                    ? new AvlTreeKeyValue<TKey, TValue>(this.storedHash, this.storedKey, this.storedValue,
                        new AvlTreeKeyValue<TKey, TValue>(hash, key, value), this.rightNode, this.collisions)
                    : Balance(this.storedHash, this.storedKey, this.storedValue, this.leftNode.Add(hash, key, value, updateDelegate, forceUpdate), this.rightNode, this.collisions)
                : this.height == 1
                    ? new AvlTreeKeyValue<TKey, TValue>(this.storedHash, this.storedKey, this.storedValue, this.leftNode,
                        new AvlTreeKeyValue<TKey, TValue>(hash, key, value), this.collisions)
                    : Balance(this.storedHash, this.storedKey, this.storedValue, this.leftNode, this.rightNode.Add(hash, key, value, updateDelegate, forceUpdate), this.collisions);
        }

        private AvlTreeKeyValue<TKey, TValue> CheckCollision(int hash, TKey key, TValue value, Func<TValue, TValue, TValue> updateDelegate, bool forceUpdate)
        {
            if (ReferenceEquals(key, this.storedKey) || key.Equals(this.storedKey))
                return updateDelegate != null
                    ? new AvlTreeKeyValue<TKey, TValue>(hash, key, updateDelegate(this.storedValue, value), this.leftNode, this.rightNode, this.collisions)
                    : forceUpdate
                        ? new AvlTreeKeyValue<TKey, TValue>(hash, key, value, this.leftNode, this.rightNode, this.collisions)
                        : this;

            if (this.collisions == null)
                return new AvlTreeKeyValue<TKey, TValue>(hash, key, value, this.leftNode, this.rightNode,
                    ArrayStoreKeyed<TKey, TValue>.Empty.Add(key, value));

            return new AvlTreeKeyValue<TKey, TValue>(hash, key, value, this.leftNode, this.rightNode,
                this.collisions.AddOrUpdate(key, updateDelegate == null || forceUpdate ? value : updateDelegate(this.storedValue, value)));
        }

        private static AvlTreeKeyValue<TKey, TValue> Balance(int hash, TKey key, TValue value, AvlTreeKeyValue<TKey, TValue> left, AvlTreeKeyValue<TKey, TValue> right, ArrayStoreKeyed<TKey, TValue> collisions)
        {
            var balance = left.height - right.height;

            if (balance >= 2)
                return left.leftNode.height - left.rightNode.height == -1
                    ? RotateLeftRight(hash, key, value, left, right, collisions)
                    : RotateRight(hash, key, value, left, right, collisions);

            if (balance <= -2)
                return right.leftNode.height - right.rightNode.height == 1
                    ? RotateRightLeft(hash, key, value, left, right, collisions)
                    : RotateLeft(hash, key, value, left, right, collisions);

            return new AvlTreeKeyValue<TKey, TValue>(hash, key, value, left, right, collisions);
        }

        private static AvlTreeKeyValue<TKey, TValue> RotateRight(int hash, TKey key, TValue value, AvlTreeKeyValue<TKey, TValue> left, AvlTreeKeyValue<TKey, TValue> right, ArrayStoreKeyed<TKey, TValue> collisions)
        {
            var r = new AvlTreeKeyValue<TKey, TValue>(hash, key, value, left.rightNode, right, collisions);
            return new AvlTreeKeyValue<TKey, TValue>(left.storedHash, left.storedKey, left.storedValue, left.leftNode, r, left.collisions);
        }

        private static AvlTreeKeyValue<TKey, TValue> RotateLeft(int hash, TKey key, TValue value, AvlTreeKeyValue<TKey, TValue> left, AvlTreeKeyValue<TKey, TValue> right, ArrayStoreKeyed<TKey, TValue> collisions)
        {
            var l = new AvlTreeKeyValue<TKey, TValue>(hash, key, value, left, right.leftNode, collisions);
            return new AvlTreeKeyValue<TKey, TValue>(right.storedHash, right.storedKey, right.storedValue, l, right.rightNode, right.collisions);
        }

        private static AvlTreeKeyValue<TKey, TValue> RotateRightLeft(int hash, TKey key, TValue value, AvlTreeKeyValue<TKey, TValue> left, AvlTreeKeyValue<TKey, TValue> right, ArrayStoreKeyed<TKey, TValue> collisions)
        {
            var l = new AvlTreeKeyValue<TKey, TValue>(hash, key, value, left, right.leftNode.leftNode, collisions);
            var r = new AvlTreeKeyValue<TKey, TValue>(right.storedHash, right.storedKey, right.storedValue, right.leftNode.rightNode, right.rightNode, right.collisions);
            return new AvlTreeKeyValue<TKey, TValue>(right.leftNode.storedHash, right.leftNode.storedKey, right.leftNode.storedValue, l, r, right.leftNode.collisions);
        }

        private static AvlTreeKeyValue<TKey, TValue> RotateLeftRight(int hash, TKey key, TValue value, AvlTreeKeyValue<TKey, TValue> left, AvlTreeKeyValue<TKey, TValue> right, ArrayStoreKeyed<TKey, TValue> collisions)
        {
            var l = new AvlTreeKeyValue<TKey, TValue>(left.storedHash, left.storedKey, left.storedValue, left.leftNode, left.rightNode.leftNode, left.collisions);
            var r = new AvlTreeKeyValue<TKey, TValue>(hash, key, value, left.rightNode.rightNode, right, collisions);
            return new AvlTreeKeyValue<TKey, TValue>(left.rightNode.storedHash, left.rightNode.storedKey, left.rightNode.storedValue, l, r, left.rightNode.collisions);
        }

        public override string ToString() => this.IsEmpty ? "empty" : $"{this.storedKey} : {this.storedValue}";

        public IEnumerable<TValue> Walk()
        {
            if (this.IsEmpty)
                yield break;

            var nodes = new AvlTreeKeyValue<TKey, TValue>[this.height];
            var currentNode = this;
            var index = -1;

            while (!currentNode.IsEmpty || index != -1)
            {
                if (!currentNode.IsEmpty)
                {
                    nodes[++index] = currentNode;
                    currentNode = currentNode.leftNode;
                }
                else
                {
                    currentNode = nodes[index--];
                    yield return currentNode.storedValue;

                    if (currentNode.collisions != null && currentNode.collisions.Length > 0)
                        for (var i = 0; i < currentNode.collisions.Length; i++)
                            yield return currentNode.collisions[i];

                    currentNode = currentNode.rightNode;
                }
            }
        }
    }
}