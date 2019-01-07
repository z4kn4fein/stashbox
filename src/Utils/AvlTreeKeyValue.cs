using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Stashbox.Utils
{
    internal class AvlTreeKeyValue<TKey, TValue>
    {
        public static readonly AvlTreeKeyValue<TKey, TValue> Empty = new AvlTreeKeyValue<TKey, TValue>();
        private static readonly TValue DefaultValue = default;

        private readonly int height;

        public int StoredHash;
        public TKey StoredKey;
        public TValue StoredValue;
        public AvlTreeKeyValue<TKey, TValue> LeftNode;
        public AvlTreeKeyValue<TKey, TValue> RightNode;
        public ArrayStoreKeyed<TKey, TValue> Collisions;
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
        {
            this.Collisions = ArrayStoreKeyed<TKey, TValue>.Empty;
        }

        public AvlTreeKeyValue<TKey, TValue> AddOrUpdate(TKey key, TValue value, Func<TValue, TValue, TValue> updateDelegate = null) =>
            this.Add(key.GetHashCode(), key, value, updateDelegate, false);

        public AvlTreeKeyValue<TKey, TValue> AddOrUpdate(TKey key, TValue value, bool forceUpdate) =>
            this.Add(key.GetHashCode(), key, value, null, forceUpdate);

        [MethodImpl(Constants.Inline)]
        public TValue GetOrDefault(TKey key)
        {
            if (this.IsEmpty)
                return DefaultValue;

            var hash = key.GetHashCode();
            var node = this;
            while (!node.IsEmpty && node.StoredHash != hash)
                node = hash < node.StoredHash ? node.LeftNode : node.RightNode;
            return !node.IsEmpty && (ReferenceEquals(key, node.StoredKey) || key.Equals(node.StoredKey)) ?
                node.StoredValue : node.Collisions.GetOrDefault(key);
        }

        private AvlTreeKeyValue<TKey, TValue> Add(int hash, TKey key, TValue value, Func<TValue, TValue, TValue> updateDelegate, bool forceUpdate)
        {
            if (this.IsEmpty)
                return new AvlTreeKeyValue<TKey, TValue>(hash, key, value, Empty, Empty, ArrayStoreKeyed<TKey, TValue>.Empty);

            if (hash == this.StoredHash)
                return this.CheckCollision(hash, key, value, updateDelegate, forceUpdate);

            var result = hash < this.StoredHash
                ? this.SelfCopy(this.LeftNode.Add(hash, key, value, updateDelegate, forceUpdate), this.RightNode)
                : this.SelfCopy(this.LeftNode, this.RightNode.Add(hash, key, value, updateDelegate, forceUpdate));

            return result.Balance();
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

        private AvlTreeKeyValue<TKey, TValue> Balance()
        {
            var balance = this.GetBalance();

            if (balance >= 2)
                return this.LeftNode.GetBalance() == -1 ? this.RotateLeftRight() : this.RotateRight();

            if (balance <= -2)
                return this.RightNode.GetBalance() == 1 ? this.RotateRightLeft() : this.RotateLeft();

            return this;
        }

        private AvlTreeKeyValue<TKey, TValue> RotateLeft() => this.RightNode.SelfCopy(this.SelfCopy(this.LeftNode, this.RightNode.LeftNode), this.RightNode.RightNode);
        private AvlTreeKeyValue<TKey, TValue> RotateRight() => this.LeftNode.SelfCopy(this.LeftNode.LeftNode, this.SelfCopy(this.LeftNode.RightNode, this.RightNode));

        private AvlTreeKeyValue<TKey, TValue> RotateRightLeft() => this.SelfCopy(this.LeftNode, this.RightNode.RotateRight()).RotateLeft();
        private AvlTreeKeyValue<TKey, TValue> RotateLeftRight() => this.SelfCopy(this.LeftNode.RotateLeft(), this.RightNode).RotateRight();

        private AvlTreeKeyValue<TKey, TValue> SelfCopy(AvlTreeKeyValue<TKey, TValue> left, AvlTreeKeyValue<TKey, TValue> right) =>
            left == this.LeftNode && right == this.RightNode ? this :
            new AvlTreeKeyValue<TKey, TValue>(this.StoredHash, this.StoredKey, this.StoredValue, left, right, this.Collisions);

        private int GetBalance() => this.LeftNode.height - this.RightNode.height;

        public IEnumerable<TValue> Walk()
        {
            if(this.height == 0)
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

                    if (currentNode.Collisions.Length > 0)
                        for (var i = 0; i < currentNode.Collisions.Length; i++)
                            yield return currentNode.Collisions[i];

                    currentNode = currentNode.RightNode;
                }
            }
        }
    }
}