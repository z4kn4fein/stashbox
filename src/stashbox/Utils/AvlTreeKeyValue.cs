using System;
using System.Collections;
using System.Collections.Generic;

namespace Stashbox.Utils
{
    internal class AvlTreeKeyValue<TKey, TValue> : IEnumerable<TValue>
    {
        public static readonly AvlTreeKeyValue<TKey, TValue> Empty = new AvlTreeKeyValue<TKey, TValue>();

        private readonly int storedHash;
        private readonly TKey storedKey;
        private readonly TValue storedValue;
        private readonly AvlTreeKeyValue<TKey, TValue> leftNode;
        private readonly AvlTreeKeyValue<TKey, TValue> rightNode;
        private readonly ArrayStoreKeyed<TKey, TValue> collisions;

        private readonly int height;
        private readonly bool isEmpty;

        public int StoredHash => this.storedHash;
        public TKey StoredKey => this.storedKey;
        public TValue StoredValue => this.storedValue;
        public AvlTreeKeyValue<TKey, TValue> LeftNode => this.leftNode;
        public AvlTreeKeyValue<TKey, TValue> RightNode => this.rightNode;
        public ArrayStoreKeyed<TKey, TValue> Collisions => this.collisions;
        public TValue Value => this.storedValue;
        public bool HasMultipleItems => this.height > 1;
        public bool IsEmpty => this.isEmpty;

        private AvlTreeKeyValue(int hash, TKey key, TValue value, AvlTreeKeyValue<TKey, TValue> left,
            AvlTreeKeyValue<TKey, TValue> right, ArrayStoreKeyed<TKey, TValue> collisions)
        {
            this.collisions = collisions;
            this.storedKey = key;
            this.storedHash = hash;
            this.leftNode = left;
            this.rightNode = right;
            this.storedValue = value;
            this.isEmpty = false;
            this.height = 1 + (left.height > right.height ? left.height : right.height);
        }

        private AvlTreeKeyValue()
        {
            this.isEmpty = true;
            this.collisions = ArrayStoreKeyed<TKey, TValue>.Empty;
        }

        public AvlTreeKeyValue<TKey, TValue> AddOrUpdate(TKey key, TValue value, Func<TValue, TValue, TValue> updateDelegate = null) =>
            this.Add(key.GetHashCode(), key, value, updateDelegate);

        public AvlTreeKeyValue<TKey, TValue> AddOrUpdate(int hash, TKey key, TValue value, Func<TValue, TValue, TValue> updateDelegate = null) =>
            this.Add(hash, key, value, updateDelegate);

        public TValue GetOrDefault(TKey key) =>
            this.GetOrDefault(key.GetHashCode(), key);

        public TValue GetOrDefault(int hash, TKey key)
        {
            var node = this;
            while (!node.isEmpty && node.storedHash != hash)
                node = hash < node.storedHash ? node.leftNode : node.rightNode;
            return !node.isEmpty && (ReferenceEquals(key, node.storedKey) || key.Equals(node.storedKey)) ?
                node.storedValue : node.collisions.GetOrDefault(key);
        }

        private AvlTreeKeyValue<TKey, TValue> Add(int hash, TKey key, TValue value, Func<TValue, TValue, TValue> updateDelegate)
        {
            if (this.isEmpty)
                return new AvlTreeKeyValue<TKey, TValue>(hash, key, value, Empty, Empty, ArrayStoreKeyed<TKey, TValue>.Empty);

            if (hash == this.storedHash)
                return this.CheckCollision(hash, key, value, updateDelegate);

            var result = hash < this.storedHash
                ? this.SelfCopy(this.leftNode.Add(hash, key, value, updateDelegate), this.rightNode)
                : this.SelfCopy(this.leftNode, this.rightNode.Add(hash, key, value, updateDelegate));

            return result.Balance();
        }

        private AvlTreeKeyValue<TKey, TValue> CheckCollision(int hash, TKey key, TValue value, Func<TValue, TValue, TValue> updateDelegate)
        {
            if (ReferenceEquals(key, this.storedKey) || key.Equals(this.storedKey))
                return updateDelegate == null ? this : new AvlTreeKeyValue<TKey, TValue>(hash, key,
                    updateDelegate(this.storedValue, value), this.leftNode, this.rightNode, this.collisions);

            if (this.collisions == null)
                return new AvlTreeKeyValue<TKey, TValue>(hash, key, value, this.leftNode, this.rightNode,
                    ArrayStoreKeyed<TKey, TValue>.Empty.Add(key, value));

            return new AvlTreeKeyValue<TKey, TValue>(hash, key, value, this.leftNode, this.rightNode,
                this.collisions.AddOrUpdate(key, updateDelegate == null ? value : updateDelegate(this.storedValue, value)));
        }

        private AvlTreeKeyValue<TKey, TValue> Balance()
        {
            var balance = this.GetBalance();

            if (balance >= 2)
                return this.leftNode.GetBalance() == -1 ? this.RotateLeftRight() : this.RotateRight();

            if (balance <= -2)
                return this.rightNode.GetBalance() == 1 ? this.RotateRightLeft() : this.RotateLeft();

            return this;
        }

        private AvlTreeKeyValue<TKey, TValue> RotateLeft() => this.rightNode.SelfCopy(this.SelfCopy(this.leftNode, this.rightNode.leftNode), this.rightNode.rightNode);
        private AvlTreeKeyValue<TKey, TValue> RotateRight() => this.leftNode.SelfCopy(this.leftNode.leftNode, this.SelfCopy(this.leftNode.rightNode, this.rightNode));

        private AvlTreeKeyValue<TKey, TValue> RotateRightLeft() => this.SelfCopy(this.leftNode, this.rightNode.RotateRight()).RotateLeft();
        private AvlTreeKeyValue<TKey, TValue> RotateLeftRight() => this.SelfCopy(this.leftNode.RotateLeft(), this.rightNode).RotateRight();

        private AvlTreeKeyValue<TKey, TValue> SelfCopy(AvlTreeKeyValue<TKey, TValue> left, AvlTreeKeyValue<TKey, TValue> right) =>
            left == this.leftNode && right == this.rightNode ? this :
            new AvlTreeKeyValue<TKey, TValue>(this.storedHash, this.storedKey, this.storedValue, left, right, this.collisions);

        private int GetBalance() => this.leftNode.height - this.rightNode.height;

        private IEnumerable<TValue> Walk()
        {
            var nodes = new AvlTreeKeyValue<TKey, TValue>[this.height];
            var currentNode = this;
            var index = -1;

            while (!currentNode.isEmpty || index != -1)
            {
                if (!currentNode.isEmpty)
                {
                    nodes[++index] = currentNode;
                    currentNode = currentNode.leftNode;
                }
                else
                {
                    currentNode = nodes[index--];
                    yield return currentNode.storedValue;

                    if (currentNode.collisions.Length > 0)
                        for (int i = 0; i < currentNode.collisions.Length; i++)
                            yield return currentNode.collisions[i];

                    currentNode = currentNode.rightNode;
                }
            }
        }

        public IEnumerator<TValue> GetEnumerator() => this.Walk().GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
    }
}
