using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Stashbox.Utils
{
    internal sealed class HashTree<TValue>
    {
        private class Node<T>
        {
            public readonly int storedKey;
            public T storedValue;
            public Node<T> left;
            public Node<T> right;
            public int height;

            public Node(int key, T value)
            {
                this.storedValue = value;
                this.storedKey = key;
                this.height = 1;
            }
        }

        private Node<TValue> root;

        public bool IsEmpty => this.root == null;

        public void Add(TValue value)
        {
            this.root = Add(this.root, RuntimeHelpers.GetHashCode(value), value);
        }

        public void Add(int key, TValue value)
        {
            this.root = Add(this.root, key, value);
        }

        [MethodImpl(Constants.Inline)]
        public TValue GetOrDefault(int key)
        {
            if (this.root == null)
                return default;

            var node = root;
            while (node != null && node.storedKey != key)
                node = key < node.storedKey ? node.left : node.right;
            return node == null ? default : node.storedValue;
        }

        private static int CalculateHeight(Node<TValue> node)
        {
            if (node.left != null && node.right != null)
                return 1 + (node.left.height > node.right.height ? node.left.height : node.right.height);

            if (node.left == null && node.right == null)
                return 1;

            return 1 + (node.left?.height ?? node.right.height);
        }

        private static int GetBalance(Node<TValue> node)
        {
            if (node.left != null && node.right != null)
                return node.left.height - node.right.height;

            if (node.left == null && node.right == null)
                return 0;

            return node.left?.height ?? node.right.height * -1;
        }

        private static Node<TValue> RotateLeft(Node<TValue> node)
        {
            var root = node.right;
            var left = root.left;

            root.left = node;
            node.right = left;

            root.height = CalculateHeight(root);
            node.height = CalculateHeight(node);

            return root;
        }

        private static Node<TValue> RotateRight(Node<TValue> node)
        {
            var root = node.left;
            var right = root.right;
            root.right = node;
            node.left = right;
            root.height = CalculateHeight(root);
            node.height = CalculateHeight(node);

            return root;
        }

        private static Node<TValue> Add(Node<TValue> node, int key, TValue value)
        {
            if (node == null)
                return new Node<TValue>(key, value);

            if (node.storedKey == key)
            {
                node.storedValue = value;
                return node;
            }

            if (node.storedKey > key)
                node.left = Add(node.left, key, value);
            else
                node.right = Add(node.right, key, value);

            node.height = CalculateHeight(node);
            var balance = GetBalance(node);

            if (balance >= 2)
            {
                if (GetBalance(node.left) == -1)
                {
                    node.left = RotateLeft(node.left);
                    node = RotateRight(node);
                }
                else
                    node = RotateRight(node);
            }

            if (balance <= -2)
            {
                if (GetBalance(node.right) == 1)
                {
                    node.right = RotateRight(node.right);
                    node = RotateLeft(node);
                }
                else
                    node = RotateLeft(node);
            }

            return node;
        }

        public IEnumerable<KeyValue<int, TValue>> Walk()
        {
            if (this.root == null)
                yield break;

            var nodes = new Node<TValue>[this.root.height];
            var currentNode = this.root;
            var index = -1;

            while (currentNode != null || index != -1)
            {
                if (currentNode != null)
                {
                    nodes[++index] = currentNode;
                    currentNode = currentNode.left;
                }
                else
                {
                    currentNode = nodes[index--];
                    yield return new KeyValue<int, TValue>(currentNode.storedKey, currentNode.storedValue);

                    currentNode = currentNode.right;
                }
            }
        }

        public IEnumerable<TValue> WalkOnValues()
        {
            if (this.root == null)
                yield break;

            var nodes = new Node<TValue>[this.root.height];
            var currentNode = this.root;
            var index = -1;

            while (currentNode != null || index != -1)
            {
                if (currentNode != null)
                {
                    nodes[++index] = currentNode;
                    currentNode = currentNode.left;
                }
                else
                {
                    currentNode = nodes[index--];
                    yield return currentNode.storedValue;

                    currentNode = currentNode.right;
                }
            }
        }
    }

    internal sealed class HashTree<TKey, TValue>
    {
        private class Node<TK, T>
        {
            public readonly int storedHash;
            public readonly TK storedKey;
            public T storedValue;
            public Node<TK, T> left;
            public Node<TK, T> right;
            public int height;
            public ExpandableArray<TK, T> collisions;

            public Node(TK key, T value, int hash)
            {
                this.storedValue = value;
                this.storedKey = key;
                this.storedHash = hash;
                this.height = 1;
            }
        }

        private Node<TKey, TValue> root;

        public HashTree() { }

        public HashTree(TKey key, TValue value)
        {
            Add(key, value, false);
        }

        public void Add(TKey key, TValue value, bool byRef = true)
        {
            this.root = Add(this.root, key, byRef ? RuntimeHelpers.GetHashCode(key) : key.GetHashCode(), value, byRef);
        }

        [MethodImpl(Constants.Inline)]
        public TValue GetOrDefault(TKey key, bool byRef = true)
        {
            if (this.root == null)
                return default;

            var node = root;
            var hash = byRef ? RuntimeHelpers.GetHashCode(key) : key.GetHashCode();
            while (node != null && node.storedHash != hash)
                node = hash < node.storedHash ? node.left : node.right;
            return node != null && (byRef && ReferenceEquals(key, node.storedKey) || !byRef && Equals(key, node.storedKey))
                ? node.storedValue
                : node?.collisions == null
                    ? default
                    : node.collisions.GetOrDefault(key, byRef);
        }

        private static int CalculateHeight(Node<TKey, TValue> node)
        {
            if (node.left != null && node.right != null)
                return 1 + (node.left.height > node.right.height ? node.left.height : node.right.height);

            if (node.left == null && node.right == null)
                return 1;

            return 1 + (node.left?.height ?? node.right.height);
        }

        private static int GetBalance(Node<TKey, TValue> node)
        {
            if (node.left != null && node.right != null)
                return node.left.height - node.right.height;

            if (node.left == null && node.right == null)
                return 0;

            return node.left?.height ?? node.right.height * -1;
        }

        private static Node<TKey, TValue> RotateLeft(Node<TKey, TValue> node)
        {
            var root = node.right;
            var left = root.left;

            root.left = node;
            node.right = left;

            root.height = CalculateHeight(root);
            node.height = CalculateHeight(node);

            return root;
        }

        private static Node<TKey, TValue> RotateRight(Node<TKey, TValue> node)
        {
            var root = node.left;
            var right = root.right;
            root.right = node;
            node.left = right;
            root.height = CalculateHeight(root);
            node.height = CalculateHeight(node);

            return root;
        }

        private static Node<TKey, TValue> Add(Node<TKey, TValue> node, TKey key, int hash, TValue value, bool byRef)
        {
            if (node == null)
                return new Node<TKey, TValue>(key, value, hash);

            if (node.storedHash == hash)
            {
                CheckCollisions(node, key, value, byRef);
                return node;
            }

            if (node.storedHash > hash)
                node.left = Add(node.left, key, hash, value, byRef);
            else
                node.right = Add(node.right, key, hash, value, byRef);

            node.height = CalculateHeight(node);
            var balance = GetBalance(node);

            if (balance >= 2)
            {
                if (GetBalance(node.left) == -1)
                {
                    node.left = RotateLeft(node.left);
                    node = RotateRight(node);
                }
                else
                    node = RotateRight(node);
            }

            if (balance <= -2)
            {
                if (GetBalance(node.right) == 1)
                {
                    node.right = RotateRight(node.right);
                    node = RotateLeft(node);
                }
                else
                    node = RotateLeft(node);
            }

            return node;
        }

        private static void CheckCollisions(Node<TKey, TValue> node, TKey key, TValue value, bool byRef)
        {
            if (byRef && ReferenceEquals(key, node.storedKey) || !byRef && Equals(key, node.storedKey))
                node.storedValue = value;

            if (node.collisions == null) node.collisions = new ExpandableArray<TKey, TValue>();
            node.collisions.Add(new KeyValue<TKey, TValue>(key, value));
        }
    }
}
