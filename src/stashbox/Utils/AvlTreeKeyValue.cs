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
        private readonly bool isEmpty = true;

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

        private AvlTreeKeyValue(int hash, TKey key, TValue value)
            : this(hash, key, value, Empty, Empty, ArrayStoreKeyed<TKey, TValue>.Empty)
        { }

        private AvlTreeKeyValue()
        {
            this.collisions = ArrayStoreKeyed<TKey, TValue>.Empty;
        }

        public AvlTreeKeyValue<TKey, TValue> AddOrUpdate(TKey key, TValue value, Func<TValue, TValue, TValue> updateDelegate = null) =>
            this.Add(key.GetHashCode(), key, value, updateDelegate, out bool updated);

        public AvlTreeKeyValue<TKey, TValue> AddOrUpdate(int hash, TKey key, TValue value, Func<TValue, TValue, TValue> updateDelegate = null) =>
            this.Add(hash, key, value, updateDelegate, out bool updated);

        public AvlTreeKeyValue<TKey, TValue> AddOrUpdate(TKey key, TValue value, out bool updated, Func<TValue, TValue, TValue> updateDelegate = null) =>
            this.Add(key.GetHashCode(), key, value, updateDelegate, out updated);

        public AvlTreeKeyValue<TKey, TValue> AddOrUpdate(int hash, TKey key, TValue value, out bool updated, Func<TValue, TValue, TValue> updateDelegate = null) =>
            this.Add(hash, key, value, updateDelegate, out updated);

        public TValue GetOrDefault(TKey key) =>
            this.GetOrDefault(key.GetHashCode(), key);

        public TValue GetOrDefault(int hash, TKey key)
        {
            if (this.isEmpty)
                return default(TValue);

            if (ReferenceEquals(key, this.storedKey))
                return this.storedValue;

            var node = this;
            while (!node.isEmpty && node.storedHash != hash)
                node = hash < node.storedHash ? node.leftNode : node.rightNode;
            return !node.isEmpty && ReferenceEquals(key, node.storedKey) || key.Equals(node.storedKey) ?
                node.storedValue : node.collisions.GetOrDefault(key);
        }

        private AvlTreeKeyValue<TKey, TValue> Add(int hash, TKey key, TValue value, Func<TValue, TValue, TValue> updateDelegate, out bool updated)
        {
            if (this.isEmpty)
            {
                updated = false;
                return new AvlTreeKeyValue<TKey, TValue>(hash, key, value);
            }

            if (hash == this.storedHash)
                return this.CheckCollision(hash, key, value, updateDelegate, out updated);

            var result = hash < this.storedHash
                ? this.SelfCopy(this.leftNode.Add(hash, key, value, updateDelegate, out updated), this.rightNode)
                : this.SelfCopy(this.leftNode, this.rightNode.Add(hash, key, value, updateDelegate, out updated));

            return result.Balance();
        }

        private AvlTreeKeyValue<TKey, TValue> CheckCollision(int hash, TKey key, TValue value, Func<TValue, TValue, TValue> updateDelegate, out bool updated)
        {
            if (ReferenceEquals(key, this.storedKey) || key.Equals(this.storedKey))
            {
                updated = true;
                return updateDelegate == null ? this : new AvlTreeKeyValue<TKey, TValue>(hash, key,
                    updateDelegate(this.storedValue, value), this.leftNode, this.rightNode, this.collisions);
            }

            if (this.collisions == null)
            {
                updated = false;
                return new AvlTreeKeyValue<TKey, TValue>(hash, key, value, this.leftNode, this.rightNode, ArrayStoreKeyed<TKey, TValue>.Empty.Add(key, value));
            }

            var newCollisions = this.collisions.AddOrUpdate(key, updateDelegate == null ? value : updateDelegate(this.storedValue, value), out updated);
            return new AvlTreeKeyValue<TKey, TValue>(hash, key, value, this.leftNode, this.rightNode, newCollisions);
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

        public IEnumerator<TValue> GetEnumerator() => new AvlTreeEnumerator(this);

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        internal class AvlTreeEnumerator : IEnumerator<TValue>
        {
            private TValue current;
            private AvlTreeKeyValue<TKey, TValue> currentNode;
            private int index;
            private readonly AvlTreeKeyValue<TKey, TValue>[] nodes;
            private readonly AvlTreeKeyValue<TKey, TValue> root;

            public object Current => this.current;
            TValue IEnumerator<TValue>.Current => this.current;

            public AvlTreeEnumerator(AvlTreeKeyValue<TKey, TValue> root)
            {
                this.nodes = new AvlTreeKeyValue<TKey, TValue>[root.height];
                this.root = root;

                this.Initialize();
            }

            private void Initialize()
            {
                this.index = -1;
                this.currentNode = this.root;
                while (!this.currentNode.isEmpty)
                {
                    this.nodes[++this.index] = this.currentNode;
                    this.currentNode = this.currentNode.leftNode;
                }
            }

            public bool MoveNext()
            {
                while (!this.currentNode.isEmpty || this.index != -1)
                {
                    if (!this.currentNode.isEmpty)
                    {
                        this.nodes[++this.index] = this.currentNode;
                        this.currentNode = this.currentNode.leftNode;
                    }
                    else
                    {
                        this.currentNode = this.nodes[this.index--];
                        this.current = this.currentNode.storedValue;
                        this.currentNode = this.currentNode.rightNode;
                        return true;
                    }
                }

                return false;
            }

            public void Reset() => this.Initialize();

            public void Dispose()
            { }
        }
    }
}
