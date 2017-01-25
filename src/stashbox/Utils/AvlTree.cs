using System;
using System.Collections;
using System.Collections.Generic;

namespace Stashbox.Utils
{
    /// <summary>
    /// Represents an immutable AVL Tree implementation
    /// </summary>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <typeparam name="TValue">The type of the collection.</typeparam>
    public class AvlTree<TKey, TValue> : IEnumerable<TValue>
    {
        private static readonly AvlTree<TKey, TValue> Empty = new AvlTree<TKey, TValue>();

        private readonly int storedHash;
        private readonly TValue storedValue;

        private readonly AvlTree<TKey, TValue> leftNode;
        private readonly AvlTree<TKey, TValue> rightNode;

        private readonly int height;
        private readonly bool isEmpty = true;

        public TValue Value => this.storedValue;
        public bool HasMultipleItems => this.height > 1;

        private AvlTree(int hash, TValue value, AvlTree<TKey, TValue> left, AvlTree<TKey, TValue> right)
        {
            this.storedHash = hash;
            this.leftNode = left;
            this.rightNode = right;
            this.storedValue = value;
            this.isEmpty = false;
            this.height = 1 + (left.height > right.height ? left.height : right.height);
        }

        private AvlTree(int hash, TValue value)
            : this(hash, value, Empty, Empty)
        { }

        /// <summary>
        /// Constructs an <see cref="AvlTree{TKey, TValue}"/>
        /// </summary>
        public AvlTree()
        { }

        /// <summary>
        /// Adds a value to the tree.
        /// </summary>
        /// <param name="key">The key to add.</param>
        /// <param name="value">The value to add.</param>
        /// <param name="updateDelegate">The update delegate.</param>
        /// <returns>The modified tree.</returns>
        public AvlTree<TKey, TValue> AddOrUpdate(TKey key, TValue value, Func<TValue, TValue, TValue> updateDelegate = null)
        {
            var hash = key.GetHashCode();
            return this.Add(hash, value, updateDelegate);
        }

        /// <summary>
        /// Retrieves an item from the tree or returns with the item's default value.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>The value.</returns>
        public TValue GetOrDefault(TKey key)
        {
            var hash = key.GetHashCode();

            var node = this;
            while (!node.isEmpty && node.storedHash != hash)
                node = hash < node.storedHash ? node.leftNode : node.rightNode;
            return !node.isEmpty ? node.storedValue : default(TValue);
        }

        private AvlTree<TKey, TValue> Add(int hash, TValue value, Func<TValue, TValue, TValue> updateDelegate)
        {
            if (this.isEmpty)
                return new AvlTree<TKey, TValue>(hash, value);

            if (hash == this.storedHash)
                return updateDelegate == null ? this : new AvlTree<TKey, TValue>(hash, updateDelegate(this.storedValue, value), this.leftNode, this.rightNode);

            var result = hash < this.storedHash
                ? this.SelfCopy(this.leftNode.Add(hash, value, updateDelegate), this.rightNode)
                : this.SelfCopy(this.leftNode, this.rightNode.Add(hash, value, updateDelegate));

            return result.Balance();
        }

        private AvlTree<TKey, TValue> Balance()
        {
            var balance = this.GetBalance();

            if (balance >= 2)
                return this.leftNode.GetBalance() == -1 ? this.RotateLeftRight() : this.RotateRight();

            if (balance <= -2)
                return this.rightNode.GetBalance() == 1 ? this.RotateRightLeft() : this.RotateLeft();

            return this;
        }

        private AvlTree<TKey, TValue> RotateLeft() => this.rightNode.SelfCopy(this.SelfCopy(this.leftNode, this.rightNode.leftNode), this.rightNode.rightNode);
        private AvlTree<TKey, TValue> RotateRight() => this.leftNode.SelfCopy(this.leftNode.leftNode, this.SelfCopy(this.leftNode.rightNode, this.rightNode));

        private AvlTree<TKey, TValue> RotateRightLeft() => this.SelfCopy(this.leftNode, this.rightNode.RotateRight()).RotateLeft();
        private AvlTree<TKey, TValue> RotateLeftRight() => this.SelfCopy(this.leftNode.RotateLeft(), this.rightNode).RotateRight();

        private AvlTree<TKey, TValue> SelfCopy(AvlTree<TKey, TValue> left, AvlTree<TKey, TValue> right) =>
            new AvlTree<TKey, TValue>(this.storedHash, this.storedValue, left, right);

        private int GetBalance() => this.leftNode.height - this.rightNode.height;

        public IEnumerator<TValue> GetEnumerator()
        {
            return new AvlTreeEnumerator<TKey, TValue>(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public class AvlTreeEnumerator<TKey, TValue> : IEnumerator<TValue>
        {
            private TValue current;
            private AvlTree<TKey, TValue> currentNode;
            private int index;
            private readonly AvlTree<TKey, TValue>[] nodes;
            private readonly AvlTree<TKey, TValue> root;

            public object Current => this.current;
            TValue IEnumerator<TValue>.Current => this.current;

            public AvlTreeEnumerator(AvlTree<TKey, TValue> root)
            {
                this.nodes = new AvlTree<TKey, TValue>[root.height];
                this.root = root;

                this.Initialize();
            }

            private void Initialize()
            {
                this.index = -1;
                this.currentNode = root;
                while (!this.currentNode.isEmpty)
                {
                    nodes[++index] = this.currentNode;
                    this.currentNode = this.currentNode.leftNode;
                }
            }

            public bool MoveNext()
            {
                while (!this.currentNode.isEmpty || index != -1)
                {
                    if (!this.currentNode.isEmpty)
                    {
                        nodes[++index] = this.currentNode;
                        this.currentNode = this.currentNode.leftNode;
                    }
                    else
                    {
                        this.currentNode = nodes[index--];
                        this.current = this.currentNode.storedValue;
                        this.currentNode = this.currentNode.rightNode;
                        return true;
                    }
                }

                return false;
            }

            public void Reset()
            {
                this.Initialize();
            }

            public void Dispose()
            {
            }
        }
    }
}
