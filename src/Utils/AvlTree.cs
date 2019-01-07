using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Stashbox.Utils
{
    internal class AvlTree<TValue>
    {
        public static readonly AvlTree<TValue> Empty = new AvlTree<TValue>();

        private static readonly TValue DefaultValue = default;
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
            this.Add(key, value, updateDelegate, false);

        public AvlTree<TValue> AddOrUpdate(int key, TValue value, bool forceUpdate) =>
            this.Add(key, value, null, true);

        [MethodImpl(Constants.Inline)]
        public TValue GetOrDefault(int key)
        {
            var node = this;
            while (!node.IsEmpty && node.storedHash != key)
                node = key < node.storedHash ? node.leftNode : node.rightNode;
            return !node.IsEmpty ? node.storedValue : DefaultValue;
        }

        private AvlTree<TValue> Add(int hash, TValue value, Func<TValue, TValue, TValue> updateDelegate, bool forceUpdate)
        {
            if (this.IsEmpty)
                return new AvlTree<TValue>(hash, value);

            if (hash == this.storedHash)
                return updateDelegate != null 
                    ? new AvlTree<TValue>(hash, updateDelegate(this.storedValue, value), this.leftNode, this.rightNode) 
                    : forceUpdate 
                        ? new AvlTree<TValue>(hash, value, this.leftNode, this.rightNode) 
                        : this;

            var result = hash < this.storedHash
                ? this.SelfCopy(this.leftNode.Add(hash, value, updateDelegate, forceUpdate), this.rightNode)
                : this.SelfCopy(this.leftNode, this.rightNode.Add(hash, value, updateDelegate, forceUpdate));

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

        public IEnumerable<TValue> Walk()
        {
            if (this.height == 0)
                yield break;

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
    }
}
