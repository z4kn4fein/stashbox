using Stashbox.Entity;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Stashbox.Utils
{
    internal sealed class ImmutableTree<TValue>
    {
        public static readonly ImmutableTree<TValue> Empty = new ImmutableTree<TValue>();

        private readonly int storedHash;
        private readonly TValue storedValue;

        private readonly ImmutableTree<TValue> leftNode;
        private readonly ImmutableTree<TValue> rightNode;

        private readonly int height;
        public bool IsEmpty = true;

        private ImmutableTree(int hash, TValue value, ImmutableTree<TValue> left, ImmutableTree<TValue> right)
        {
            this.storedHash = hash;
            this.leftNode = left;
            this.rightNode = right;
            this.storedValue = value;
            this.IsEmpty = false;
            this.height = 1 + (left.height > right.height ? left.height : right.height);
        }

        private ImmutableTree(int hash, TValue value)
        {
            this.storedHash = hash;
            this.leftNode = Empty;
            this.rightNode = Empty;
            this.storedValue = value;
            this.IsEmpty = false;
            this.height = 1;
        }

        private ImmutableTree()
        { }

        public ImmutableTree<TValue> AddOrUpdate(int key, TValue value, Func<TValue, TValue, TValue> updateDelegate = null) =>
            this.Add(key, value, updateDelegate, false);

        public ImmutableTree<TValue> AddOrUpdate(int key, TValue value, bool forceUpdate) =>
            this.Add(key, value, null, true);

        [MethodImpl(Constants.Inline)]
        public TValue GetOrDefault(int key)
        {
            var node = this;
            while (!node.IsEmpty && node.storedHash != key)
                node = key < node.storedHash ? node.leftNode : node.rightNode;
            return !node.IsEmpty ? node.storedValue : default;
        }

        private ImmutableTree<TValue> Add(int hash, TValue value, Func<TValue, TValue, TValue> updateDelegate, bool forceUpdate)
        {
            if (this.IsEmpty)
                return new ImmutableTree<TValue>(hash, value);

            if (hash == this.storedHash)
                return updateDelegate != null
                    ? new ImmutableTree<TValue>(hash, updateDelegate(this.storedValue, value), this.leftNode, this.rightNode)
                    : forceUpdate
                        ? new ImmutableTree<TValue>(hash, value, this.leftNode, this.rightNode)
                        : this;

            return hash < this.storedHash
                ? this.height == 1
                    ? new ImmutableTree<TValue>(this.storedHash, this.storedValue,
                        new ImmutableTree<TValue>(hash, value), this.rightNode)
                    : Balance(this.storedHash, this.storedValue, this.leftNode.Add(hash, value, updateDelegate, forceUpdate), this.rightNode)
                : this.height == 1
                    ? new ImmutableTree<TValue>(this.storedHash, this.storedValue, this.leftNode,
                        new ImmutableTree<TValue>(hash, value))
                    : Balance(this.storedHash, this.storedValue, this.leftNode, this.rightNode.Add(hash, value, updateDelegate, forceUpdate));
        }

        private static ImmutableTree<TValue> Balance(int hash, TValue value, ImmutableTree<TValue> left, ImmutableTree<TValue> right)
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

            return new ImmutableTree<TValue>(hash, value, left, right);
        }

        private static ImmutableTree<TValue> RotateRight(int hash, TValue value, ImmutableTree<TValue> left, ImmutableTree<TValue> right)
        {
            var r = new ImmutableTree<TValue>(hash, value, left.rightNode, right);
            return new ImmutableTree<TValue>(left.storedHash, left.storedValue, left.leftNode, r);
        }

        private static ImmutableTree<TValue> RotateLeft(int hash, TValue value, ImmutableTree<TValue> left, ImmutableTree<TValue> right)
        {
            var l = new ImmutableTree<TValue>(hash, value, left, right.leftNode);
            return new ImmutableTree<TValue>(right.storedHash, right.storedValue, l, right.rightNode);
        }

        private static ImmutableTree<TValue> RotateRightLeft(int hash, TValue value, ImmutableTree<TValue> left, ImmutableTree<TValue> right)
        {
            var l = new ImmutableTree<TValue>(hash, value, left, right.leftNode.leftNode);
            var r = new ImmutableTree<TValue>(right.storedHash, right.storedValue, right.leftNode.rightNode, right.rightNode);
            return new ImmutableTree<TValue>(right.leftNode.storedHash, right.leftNode.storedValue, l, r);
        }

        private static ImmutableTree<TValue> RotateLeftRight(int hash, TValue value, ImmutableTree<TValue> left, ImmutableTree<TValue> right)
        {
            var l = new ImmutableTree<TValue>(left.storedHash, left.storedValue, left.leftNode, left.rightNode.leftNode);
            var r = new ImmutableTree<TValue>(hash, value, left.rightNode.rightNode, right);
            return new ImmutableTree<TValue>(left.rightNode.storedHash, left.rightNode.storedValue, l, r);
        }

        public override string ToString() => this.IsEmpty ? "empty" : $"{this.storedHash} : {this.storedValue}";


        public IEnumerable<KeyValue<int, TValue>> Walk()
        {
            if (this.IsEmpty)
                yield break;

            var nodes = new ImmutableTree<TValue>[this.height];
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
                    yield return new KeyValue<int, TValue>(currentNode.storedHash, currentNode.storedValue);

                    currentNode = currentNode.rightNode;
                }
            }
        }
    }

    internal sealed class ImmutableTree<TKey, TValue>
    {
        public static readonly ImmutableTree<TKey, TValue> Empty = new ImmutableTree<TKey, TValue>();

        private readonly int height;
        private readonly int storedHash;
        private readonly TKey storedKey;
        private readonly TValue storedValue;
        private readonly ImmutableTree<TKey, TValue> leftNode;
        private readonly ImmutableTree<TKey, TValue> rightNode;
        private readonly ImmutableArray<TKey, TValue> collisions;

        public bool IsEmpty = true;

        private ImmutableTree(int hash, TKey key, TValue value, ImmutableTree<TKey, TValue> left,
            ImmutableTree<TKey, TValue> right, ImmutableArray<TKey, TValue> collisions)
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

        private ImmutableTree()
        { }

        private ImmutableTree(int hash, TKey key, TValue value)
        {
            this.storedKey = key;
            this.storedHash = hash;
            this.leftNode = Empty;
            this.rightNode = Empty;
            this.storedValue = value;
            this.IsEmpty = false;
            this.height = 1;
        }

        public ImmutableTree<TKey, TValue> AddOrUpdate(TKey key, TValue value, Func<TValue, TValue, TValue> updateDelegate = null) =>
            this.Add(key.GetHashCode(), key, value, updateDelegate, false);

        public ImmutableTree<TKey, TValue> AddOrUpdate(TKey key, TValue value, bool forceUpdate) =>
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

        private ImmutableTree<TKey, TValue> Add(int hash, TKey key, TValue value, Func<TValue, TValue, TValue> updateDelegate, bool forceUpdate)
        {
            if (this.IsEmpty)
                return new ImmutableTree<TKey, TValue>(hash, key, value);

            if (hash == this.storedHash)
                return this.CheckCollision(hash, key, value, updateDelegate, forceUpdate);

            return hash < this.storedHash
                ? this.height == 1
                    ? new ImmutableTree<TKey, TValue>(this.storedHash, this.storedKey, this.storedValue,
                        new ImmutableTree<TKey, TValue>(hash, key, value), this.rightNode, this.collisions)
                    : Balance(this.storedHash, this.storedKey, this.storedValue, this.leftNode.Add(hash, key, value, updateDelegate, forceUpdate), this.rightNode, this.collisions)
                : this.height == 1
                    ? new ImmutableTree<TKey, TValue>(this.storedHash, this.storedKey, this.storedValue, this.leftNode,
                        new ImmutableTree<TKey, TValue>(hash, key, value), this.collisions)
                    : Balance(this.storedHash, this.storedKey, this.storedValue, this.leftNode, this.rightNode.Add(hash, key, value, updateDelegate, forceUpdate), this.collisions);
        }

        private ImmutableTree<TKey, TValue> CheckCollision(int hash, TKey key, TValue value, Func<TValue, TValue, TValue> updateDelegate, bool forceUpdate)
        {
            if (ReferenceEquals(key, this.storedKey) || key.Equals(this.storedKey))
                return updateDelegate != null
                    ? new ImmutableTree<TKey, TValue>(hash, key, updateDelegate(this.storedValue, value), this.leftNode, this.rightNode, this.collisions)
                    : forceUpdate
                        ? new ImmutableTree<TKey, TValue>(hash, key, value, this.leftNode, this.rightNode, this.collisions)
                        : this;

            if (this.collisions == null)
                return new ImmutableTree<TKey, TValue>(hash, key, value, this.leftNode, this.rightNode,
                    ImmutableArray<TKey, TValue>.Empty.Add(key, value));

            return new ImmutableTree<TKey, TValue>(hash, key, value, this.leftNode, this.rightNode,
                this.collisions.AddOrUpdate(key, updateDelegate == null || forceUpdate ? value : updateDelegate(this.storedValue, value)));
        }

        private static ImmutableTree<TKey, TValue> Balance(int hash, TKey key, TValue value, ImmutableTree<TKey, TValue> left, ImmutableTree<TKey, TValue> right, ImmutableArray<TKey, TValue> collisions)
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

            return new ImmutableTree<TKey, TValue>(hash, key, value, left, right, collisions);
        }

        private static ImmutableTree<TKey, TValue> RotateRight(int hash, TKey key, TValue value, ImmutableTree<TKey, TValue> left, ImmutableTree<TKey, TValue> right, ImmutableArray<TKey, TValue> collisions)
        {
            var r = new ImmutableTree<TKey, TValue>(hash, key, value, left.rightNode, right, collisions);
            return new ImmutableTree<TKey, TValue>(left.storedHash, left.storedKey, left.storedValue, left.leftNode, r, left.collisions);
        }

        private static ImmutableTree<TKey, TValue> RotateLeft(int hash, TKey key, TValue value, ImmutableTree<TKey, TValue> left, ImmutableTree<TKey, TValue> right, ImmutableArray<TKey, TValue> collisions)
        {
            var l = new ImmutableTree<TKey, TValue>(hash, key, value, left, right.leftNode, collisions);
            return new ImmutableTree<TKey, TValue>(right.storedHash, right.storedKey, right.storedValue, l, right.rightNode, right.collisions);
        }

        private static ImmutableTree<TKey, TValue> RotateRightLeft(int hash, TKey key, TValue value, ImmutableTree<TKey, TValue> left, ImmutableTree<TKey, TValue> right, ImmutableArray<TKey, TValue> collisions)
        {
            var l = new ImmutableTree<TKey, TValue>(hash, key, value, left, right.leftNode.leftNode, collisions);
            var r = new ImmutableTree<TKey, TValue>(right.storedHash, right.storedKey, right.storedValue, right.leftNode.rightNode, right.rightNode, right.collisions);
            return new ImmutableTree<TKey, TValue>(right.leftNode.storedHash, right.leftNode.storedKey, right.leftNode.storedValue, l, r, right.leftNode.collisions);
        }

        private static ImmutableTree<TKey, TValue> RotateLeftRight(int hash, TKey key, TValue value, ImmutableTree<TKey, TValue> left, ImmutableTree<TKey, TValue> right, ImmutableArray<TKey, TValue> collisions)
        {
            var l = new ImmutableTree<TKey, TValue>(left.storedHash, left.storedKey, left.storedValue, left.leftNode, left.rightNode.leftNode, left.collisions);
            var r = new ImmutableTree<TKey, TValue>(hash, key, value, left.rightNode.rightNode, right, collisions);
            return new ImmutableTree<TKey, TValue>(left.rightNode.storedHash, left.rightNode.storedKey, left.rightNode.storedValue, l, r, left.rightNode.collisions);
        }

        public override string ToString() => this.IsEmpty ? "empty" : $"{this.storedKey} : {this.storedValue}";

        public IEnumerable<KeyValue<TKey, TValue>> Walk()
        {
            if (this.IsEmpty)
                yield break;

            var nodes = new ImmutableTree<TKey, TValue>[this.height];
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
                    yield return new KeyValue<TKey, TValue>(currentNode.storedKey, currentNode.storedValue);

                    if (currentNode.collisions != null && currentNode.collisions.Length > 0)
                        for (var i = 0; i < currentNode.collisions.Length; i++)
                            yield return currentNode.collisions.Repository[i];

                    currentNode = currentNode.rightNode;
                }
            }
        }
    }
}
