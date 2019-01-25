using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Stashbox.Utils
{
    internal sealed class AvlTree<TValue>
    {
        public static readonly AvlTree<TValue> Empty = new AvlTree<TValue>();

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
        {
            this.storedHash = hash;
            this.leftNode = Empty;
            this.rightNode = Empty;
            this.storedValue = value;
            this.IsEmpty = false;
            this.height = 1;
        }

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
            return !node.IsEmpty ? node.storedValue : default;
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

            return hash < this.storedHash
                ? this.height == 1
                    ? new AvlTree<TValue>(this.storedHash, this.storedValue,
                        new AvlTree<TValue>(hash, value), this.rightNode)
                    : Balance(this.storedHash, this.storedValue, this.leftNode.Add(hash, value, updateDelegate, forceUpdate), this.rightNode)
                : this.height == 1
                    ? new AvlTree<TValue>(this.storedHash, this.storedValue, this.leftNode,
                        new AvlTree<TValue>(hash, value))
                    : Balance(this.storedHash, this.storedValue, this.leftNode, this.rightNode.Add(hash, value, updateDelegate, forceUpdate));
        }

        private static AvlTree<TValue> Balance(int hash, TValue value, AvlTree<TValue> left, AvlTree<TValue> right)
        {
            var balance = left.height - right.height;

            if (balance >= 2)
                return left.leftNode.height - left.rightNode.height == -1
                    ? RotateLeftRight(hash, value, left, right)
                    : RotateRight(hash, value, left, right);

            if (balance <= -2)
                return right.leftNode.height - right.rightNode.height == 1
                    ? RotateRightLeft(hash, value, left, right)
                    : RotateLeft(hash, value, left, right);

            return new AvlTree<TValue>(hash, value, left, right);
        }

        private static AvlTree<TValue> RotateRight(int hash, TValue value, AvlTree<TValue> left, AvlTree<TValue> right)
        {
            var r = new AvlTree<TValue>(hash, value, left.rightNode, right);
            return new AvlTree<TValue>(left.storedHash, left.storedValue, left.leftNode, r);
        }

        private static AvlTree<TValue> RotateLeft(int hash, TValue value, AvlTree<TValue> left, AvlTree<TValue> right)
        {
            var l = new AvlTree<TValue>(hash, value, left, right.leftNode);
            return new AvlTree<TValue>(right.storedHash, right.storedValue, l, right.rightNode);
        }

        private static AvlTree<TValue> RotateRightLeft(int hash, TValue value, AvlTree<TValue> left, AvlTree<TValue> right)
        {
            var l = new AvlTree<TValue>(hash, value, left, right.leftNode.leftNode);
            var r = new AvlTree<TValue>(right.storedHash, right.storedValue, right.leftNode.rightNode, right.rightNode);
            return new AvlTree<TValue>(right.leftNode.storedHash, right.leftNode.storedValue, l, r);
        }

        private static AvlTree<TValue> RotateLeftRight(int hash, TValue value, AvlTree<TValue> left, AvlTree<TValue> right)
        {
            var l = new AvlTree<TValue>(left.storedHash, left.storedValue, left.leftNode, left.rightNode.leftNode);
            var r = new AvlTree<TValue>(hash, value, left.rightNode.rightNode, right);
            return new AvlTree<TValue>(left.rightNode.storedHash, left.rightNode.storedValue, l, r);
        }

        public override string ToString() => this.IsEmpty ? "empty" : $"{this.storedHash} : {this.storedValue}";


        public IEnumerable<TValue> Walk()
        {
            if (this.IsEmpty)
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
