using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Stashbox.Utils
{
    internal class AvlTree<TValue> : IEnumerable<TValue>
    {
        public static readonly AvlTree<TValue> Empty = new AvlTree<TValue>();

        private readonly TValue defaultValue = default;
        private readonly int storedHash;
        private readonly TValue storedValue;

        private readonly AvlTree<TValue> leftNode;
        private readonly AvlTree<TValue> rightNode;

        private readonly int height;
        public bool IsEmpty = true;

        private AvlTree(int hash, TValue value, AvlTree<TValue> left, AvlTree<TValue> right)
        {
            this.storedHash = hash;
            this.leftNode = left;
            this.rightNode = right;
            this.storedValue = value;
            this.IsEmpty = false;
            this.height = 1 + (left.height > right.height ? left.height : right.height);
        }

        private AvlTree(int hash, TValue value)
            : this(hash, value, Empty, Empty)
        { }

        private AvlTree()
        { }

        public AvlTree<TValue> AddOrUpdate(int key, TValue value, Func<TValue, TValue, TValue> updateDelegate = null) =>
            this.Add(key, value, updateDelegate);

        [MethodImpl(Constants.Inline)]
        public TValue GetOrDefault(int key)
        {
            var node = this;
            while (!node.IsEmpty && node.storedHash != key)
                node = key < node.storedHash ? node.leftNode : node.rightNode;
            return !node.IsEmpty ? node.storedValue : this.defaultValue;
        }

        private AvlTree<TValue> Add(int hash, TValue value, Func<TValue, TValue, TValue> updateDelegate)
        {
            if (this.IsEmpty)
                return new AvlTree<TValue>(hash, value);

            if (hash == this.storedHash)
                return updateDelegate == null ? this : new AvlTree<TValue>(hash, updateDelegate(this.storedValue, value), this.leftNode, this.rightNode);

            var result = hash < this.storedHash
                ? this.SelfCopy(this.leftNode.Add(hash, value, updateDelegate), this.rightNode)
                : this.SelfCopy(this.leftNode, this.rightNode.Add(hash, value, updateDelegate));

            return result.Balance();
        }

        private AvlTree<TValue> Balance()
        {
            var balance = this.GetBalance();

            if (balance >= 2)
                return this.leftNode.GetBalance() == -1 ? this.RotateLeftRight() : this.RotateRight();

            if (balance <= -2)
                return this.rightNode.GetBalance() == 1 ? this.RotateRightLeft() : this.RotateLeft();

            return this;
        }

        private AvlTree<TValue> RotateLeft() => this.rightNode.SelfCopy(this.SelfCopy(this.leftNode, this.rightNode.leftNode), this.rightNode.rightNode);
        private AvlTree<TValue> RotateRight() => this.leftNode.SelfCopy(this.leftNode.leftNode, this.SelfCopy(this.leftNode.rightNode, this.rightNode));

        private AvlTree<TValue> RotateRightLeft() => this.SelfCopy(this.leftNode, this.rightNode.RotateRight()).RotateLeft();
        private AvlTree<TValue> RotateLeftRight() => this.SelfCopy(this.leftNode.RotateLeft(), this.rightNode).RotateRight();

        private AvlTree<TValue> SelfCopy(AvlTree<TValue> left, AvlTree<TValue> right) =>
            left == this.leftNode && right == this.rightNode ? this :
            new AvlTree<TValue>(this.storedHash, this.storedValue, left, right);

        private int GetBalance() => this.leftNode.height - this.rightNode.height;

        private IEnumerable<TValue> Walk()
        {
            var nodes = new AvlTree<TValue>[this.height];
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

                    currentNode = currentNode.rightNode;
                }
            }
        }

        public IEnumerator<TValue> GetEnumerator() => this.Walk().GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
    }
}
