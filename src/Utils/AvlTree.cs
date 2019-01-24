using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Stashbox.Utils
{
    internal sealed class AvlTree<TValue>
    {
        public static readonly AvlTree<TValue> Empty = new AvlTree<TValue>();

        private readonly int StoredHash;
        private readonly TValue StoredValue;

        private readonly AvlTree<TValue> LeftNode;
        private readonly AvlTree<TValue> RightNode;

        private readonly int height;
        public bool IsEmpty = true;

        private AvlTree(int hash, TValue value, AvlTree<TValue> left, AvlTree<TValue> right)
        {
            this.StoredHash = hash;
            this.LeftNode = left;
            this.RightNode = right;
            this.StoredValue = value;
            this.IsEmpty = false;
            this.height = 1 + (left.height > right.height ? left.height : right.height);
        }

        private AvlTree(int hash, TValue value)
        {
            this.StoredHash = hash;
            this.LeftNode = Empty;
            this.RightNode = Empty;
            this.StoredValue = value;
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
            while (!node.IsEmpty && node.StoredHash != key)
                node = key < node.StoredHash ? node.LeftNode : node.RightNode;
            return !node.IsEmpty ? node.StoredValue : default;
        }

        private AvlTree<TValue> Add(int hash, TValue value, Func<TValue, TValue, TValue> updateDelegate, bool forceUpdate)
        {
            if (this.IsEmpty)
                return new AvlTree<TValue>(hash, value);

            if (hash == this.StoredHash)
                return updateDelegate != null
                    ? new AvlTree<TValue>(hash, updateDelegate(this.StoredValue, value), this.LeftNode, this.RightNode)
                    : forceUpdate
                        ? new AvlTree<TValue>(hash, value, this.LeftNode, this.RightNode)
                        : this;

            return hash < this.StoredHash
                ? this.height == 1
                    ? new AvlTree<TValue>(this.StoredHash, this.StoredValue,
                        new AvlTree<TValue>(hash, value), this.RightNode)
                    : Balance(this.StoredHash, this.StoredValue, this.LeftNode.Add(hash, value, updateDelegate, forceUpdate), this.RightNode)
                : this.height == 1
                    ? new AvlTree<TValue>(this.StoredHash, this.StoredValue, this.LeftNode,
                        new AvlTree<TValue>(hash, value))
                    : Balance(this.StoredHash, this.StoredValue, this.LeftNode, this.RightNode.Add(hash, value, updateDelegate, forceUpdate));
        }

        private static AvlTree<TValue> Balance(int hash, TValue value, AvlTree<TValue> left, AvlTree<TValue> right)
        {
            var balance = left.height - right.height;

            if (balance >= 2)
                return left.LeftNode.height - left.RightNode.height == -1
                    ? RotateLeftRight(hash, value, left, right)
                    : RotateRight(hash, value, left, right);

            if (balance <= -2)
                return right.LeftNode.height - right.RightNode.height == 1
                    ? RotateRightLeft(hash, value, left, right)
                    : RotateLeft(hash, value, left, right);

            return new AvlTree<TValue>(hash, value, left, right);
        }

        private static AvlTree<TValue> RotateRight(int hash, TValue value, AvlTree<TValue> left, AvlTree<TValue> right)
        {
            var r = new AvlTree<TValue>(hash, value, left.RightNode, right);
            return new AvlTree<TValue>(left.StoredHash, left.StoredValue, left.LeftNode, r);
        }

        private static AvlTree<TValue> RotateLeft(int hash, TValue value, AvlTree<TValue> left, AvlTree<TValue> right)
        {
            var l = new AvlTree<TValue>(hash, value, left, right.LeftNode);
            return new AvlTree<TValue>(right.StoredHash, right.StoredValue, l, right.RightNode);
        }

        private static AvlTree<TValue> RotateRightLeft(int hash, TValue value, AvlTree<TValue> left, AvlTree<TValue> right)
        {
            var l = new AvlTree<TValue>(hash, value, left, right.LeftNode.LeftNode);
            var r = new AvlTree<TValue>(right.StoredHash, right.StoredValue, right.LeftNode.RightNode, right.RightNode);
            return new AvlTree<TValue>(right.LeftNode.StoredHash, right.LeftNode.StoredValue, l, r);
        }

        private static AvlTree<TValue> RotateLeftRight(int hash, TValue value, AvlTree<TValue> left, AvlTree<TValue> right)
        {
            var l = new AvlTree<TValue>(left.StoredHash, left.StoredValue, left.LeftNode, left.RightNode.LeftNode);
            var r = new AvlTree<TValue>(hash, value, left.RightNode.RightNode, right);
            return new AvlTree<TValue>(left.RightNode.StoredHash, left.RightNode.StoredValue, l, r);
        }

        public override string ToString() => this.IsEmpty ? "empty" : $"{this.StoredHash} : {this.StoredValue}";


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
                    currentNode = currentNode.LeftNode;
                }
                else
                {
                    currentNode = nodes[index--];
                    yield return currentNode.StoredValue;

                    currentNode = currentNode.RightNode;
                }
            }
        }
    }
}
