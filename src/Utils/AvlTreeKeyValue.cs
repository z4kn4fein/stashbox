using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Stashbox.Utils
{
    internal sealed class AvlTreeKeyValue<TKey, TValue>
    {
        public static readonly AvlTreeKeyValue<TKey, TValue> Empty = new AvlTreeKeyValue<TKey, TValue>();

        private readonly int height;
        private readonly int StoredHash;
        private readonly TKey StoredKey;
        private readonly TValue StoredValue;
        private readonly AvlTreeKeyValue<TKey, TValue> LeftNode;
        private readonly AvlTreeKeyValue<TKey, TValue> RightNode;
        private readonly ArrayStoreKeyed<TKey, TValue> Collisions;

        public bool IsEmpty = true;

        private AvlTreeKeyValue(int hash, TKey key, TValue value, AvlTreeKeyValue<TKey, TValue> left,
            AvlTreeKeyValue<TKey, TValue> right, ArrayStoreKeyed<TKey, TValue> collisions)
        {
            this.Collisions = collisions;
            this.StoredKey = key;
            this.StoredHash = hash;
            this.LeftNode = left;
            this.RightNode = right;
            this.StoredValue = value;
            this.IsEmpty = false;
            this.height = 1 + (left.height > right.height ? left.height : right.height);
        }

        private AvlTreeKeyValue()
        { }

        private AvlTreeKeyValue(int hash, TKey key, TValue value)
        {
            this.StoredKey = key;
            this.StoredHash = hash;
            this.LeftNode = Empty;
            this.RightNode = Empty;
            this.StoredValue = value;
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
            while (!node.IsEmpty && node.StoredHash != hash)
                node = hash < node.StoredHash ? node.LeftNode : node.RightNode;
            return !node.IsEmpty && (ReferenceEquals(key, node.StoredKey) || key.Equals(node.StoredKey)) ?
                node.StoredValue : node.Collisions == null ? default : node.Collisions.GetOrDefault(key);
        }

        private AvlTreeKeyValue<TKey, TValue> Add(int hash, TKey key, TValue value, Func<TValue, TValue, TValue> updateDelegate, bool forceUpdate)
        {
            if (this.IsEmpty)
                return new AvlTreeKeyValue<TKey, TValue>(hash, key, value);

            if (hash == this.StoredHash)
                return this.CheckCollision(hash, key, value, updateDelegate, forceUpdate);

            return hash < this.StoredHash
                ? this.height == 1
                    ? new AvlTreeKeyValue<TKey, TValue>(this.StoredHash, this.StoredKey, this.StoredValue,
                        new AvlTreeKeyValue<TKey, TValue>(hash, key, value), this.RightNode, this.Collisions)
                    : Balance(this.StoredHash, this.StoredKey, this.StoredValue, this.LeftNode.Add(hash, key, value, updateDelegate, forceUpdate), this.RightNode, this.Collisions)
                : this.height == 1
                    ? new AvlTreeKeyValue<TKey, TValue>(this.StoredHash, this.StoredKey, this.StoredValue, this.LeftNode,
                        new AvlTreeKeyValue<TKey, TValue>(hash, key, value), this.Collisions)
                    : Balance(this.StoredHash, this.StoredKey, this.StoredValue, this.LeftNode, this.RightNode.Add(hash, key, value, updateDelegate, forceUpdate), this.Collisions);
        }

        private AvlTreeKeyValue<TKey, TValue> CheckCollision(int hash, TKey key, TValue value, Func<TValue, TValue, TValue> updateDelegate, bool forceUpdate)
        {
            if (ReferenceEquals(key, this.StoredKey) || key.Equals(this.StoredKey))
                return updateDelegate != null
                    ? new AvlTreeKeyValue<TKey, TValue>(hash, key, updateDelegate(this.StoredValue, value), this.LeftNode, this.RightNode, this.Collisions)
                    : forceUpdate
                        ? new AvlTreeKeyValue<TKey, TValue>(hash, key, value, this.LeftNode, this.RightNode, this.Collisions)
                        : this;

            if (this.Collisions == null)
                return new AvlTreeKeyValue<TKey, TValue>(hash, key, value, this.LeftNode, this.RightNode,
                    ArrayStoreKeyed<TKey, TValue>.Empty.Add(key, value));

            return new AvlTreeKeyValue<TKey, TValue>(hash, key, value, this.LeftNode, this.RightNode,
                this.Collisions.AddOrUpdate(key, updateDelegate == null || forceUpdate ? value : updateDelegate(this.StoredValue, value)));
        }

        private static AvlTreeKeyValue<TKey, TValue> Balance(int hash, TKey key, TValue value, AvlTreeKeyValue<TKey, TValue> left, AvlTreeKeyValue<TKey, TValue> right, ArrayStoreKeyed<TKey, TValue> collisions)
        {
            var balance = left.height - right.height;

            if (balance >= 2)
                return left.LeftNode.height - left.RightNode.height == -1
                    ? RotateLeftRight(hash, key, value, left, right, collisions)
                    : RotateRight(hash, key, value, left, right, collisions);

            if (balance <= -2)
                return right.LeftNode.height - right.RightNode.height == 1
                    ? RotateRightLeft(hash, key, value, left, right, collisions)
                    : RotateLeft(hash, key, value, left, right, collisions);

            return new AvlTreeKeyValue<TKey, TValue>(hash, key, value, left, right, collisions);
        }

        private static AvlTreeKeyValue<TKey, TValue> RotateRight(int hash, TKey key, TValue value, AvlTreeKeyValue<TKey, TValue> left, AvlTreeKeyValue<TKey, TValue> right, ArrayStoreKeyed<TKey, TValue> collisions)
        {
            var r = new AvlTreeKeyValue<TKey, TValue>(hash, key, value, left.RightNode, right, collisions);
            return new AvlTreeKeyValue<TKey, TValue>(left.StoredHash, left.StoredKey, left.StoredValue, left.LeftNode, r, left.Collisions);
        }

        private static AvlTreeKeyValue<TKey, TValue> RotateLeft(int hash, TKey key, TValue value, AvlTreeKeyValue<TKey, TValue> left, AvlTreeKeyValue<TKey, TValue> right, ArrayStoreKeyed<TKey, TValue> collisions)
        {
            var l = new AvlTreeKeyValue<TKey, TValue>(hash, key, value, left, right.LeftNode, collisions);
            return new AvlTreeKeyValue<TKey, TValue>(right.StoredHash, right.StoredKey, right.StoredValue, l, right.RightNode, right.Collisions);
        }

        private static AvlTreeKeyValue<TKey, TValue> RotateRightLeft(int hash, TKey key, TValue value, AvlTreeKeyValue<TKey, TValue> left, AvlTreeKeyValue<TKey, TValue> right, ArrayStoreKeyed<TKey, TValue> collisions)
        {
            var l = new AvlTreeKeyValue<TKey, TValue>(hash, key, value, left, right.LeftNode.LeftNode, collisions);
            var r = new AvlTreeKeyValue<TKey, TValue>(right.StoredHash, right.StoredKey, right.StoredValue, right.LeftNode.RightNode, right.RightNode, right.Collisions);
            return new AvlTreeKeyValue<TKey, TValue>(right.LeftNode.StoredHash, right.LeftNode.StoredKey, right.LeftNode.StoredValue, l, r, right.LeftNode.Collisions);
        }

        private static AvlTreeKeyValue<TKey, TValue> RotateLeftRight(int hash, TKey key, TValue value, AvlTreeKeyValue<TKey, TValue> left, AvlTreeKeyValue<TKey, TValue> right, ArrayStoreKeyed<TKey, TValue> collisions)
        {
            var l = new AvlTreeKeyValue<TKey, TValue>(left.StoredHash, left.StoredKey, left.StoredValue, left.LeftNode, left.RightNode.LeftNode, left.Collisions);
            var r = new AvlTreeKeyValue<TKey, TValue>(hash, key, value, left.RightNode.RightNode, right, collisions);
            return new AvlTreeKeyValue<TKey, TValue>(left.RightNode.StoredHash, left.RightNode.StoredKey, left.RightNode.StoredValue, l, r, left.RightNode.Collisions);
        }

        public override string ToString() => this.IsEmpty ? "empty" : $"{this.StoredKey} : {this.StoredValue}";

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
                    currentNode = currentNode.LeftNode;
                }
                else
                {
                    currentNode = nodes[index--];
                    yield return currentNode.StoredValue;

                    if (currentNode.Collisions != null && currentNode.Collisions.Length > 0)
                        for (var i = 0; i < currentNode.Collisions.Length; i++)
                            yield return currentNode.Collisions[i];

                    currentNode = currentNode.RightNode;
                }
            }
        }
    }
}