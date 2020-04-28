using Stashbox.Entity;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Stashbox.Utils
{
    internal sealed class HashTree<TValue>
    {
        public static HashTree<TValue> Empty => new HashTree<TValue>();

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
            this.root = Add(this.root, value.GetHashCode(), value);
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
        public static HashTree<TKey, TValue> Empty => new HashTree<TKey, TValue>();

        private class Node<TK, T>
        {
            public readonly int storedHash;
            public readonly TK storedKey;
            public T storedValue;
            public Node<TK, T> left;
            public Node<TK, T> right;
            public int height;

            public Node(TK key, T value)
            {
                this.storedValue = value;
                this.storedKey = key;
                this.storedHash = key.GetHashCode();
                this.height = 1;
            }
        }

        private Node<TKey, TValue> root;

        public bool IsEmpty => this.root == null;

        private HashTree() { }

        public HashTree(TKey key, TValue value)
        {
            Add(key, value);
        }

        public void Add(TKey key, TValue value)
        {
            this.root = Add(this.root, key, key.GetHashCode(), value);
        }

        [MethodImpl(Constants.Inline)]
        public TValue GetOrDefault(TKey key)
        {
            if (this.root == null)
                return default;

            var node = root;
            var hash = key.GetHashCode();
            while (node != null && node.storedHash != hash)
                node = hash < node.storedHash ? node.left : node.right;
            return node != null && (ReferenceEquals(key, node.storedKey) || key.Equals(node.storedKey)) ? node.storedValue : default;
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

        private static Node<TKey, TValue> Add(Node<TKey, TValue> node, TKey key, int hash, TValue value)
        {
            if (node == null)
                return new Node<TKey, TValue>(key, value);

            if (node.storedHash == hash && (ReferenceEquals(key, node.storedKey) || key.Equals(node.storedKey)))
            {
                node.storedValue = value;
                return node;
            }

            if (node.storedHash > hash)
                node.left = Add(node.left, key, hash, value);
            else
                node.right = Add(node.right, key, hash, value);

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

        public IEnumerable<KeyValue<TKey, TValue>> Walk()
        {
            if (this.root == null)
                yield break;

            var nodes = new Node<TKey, TValue>[this.root.height];
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
                    yield return new KeyValue<TKey, TValue>(currentNode.storedKey, currentNode.storedValue);

                    currentNode = currentNode.right;
                }
            }
        }

        public IEnumerable<TValue> WalkOnValues()
        {
            if (this.root == null)
                yield break;

            var nodes = new Node<TKey, TValue>[this.root.height];
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
}
