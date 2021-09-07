using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Stashbox.Utils.Data
{
    internal sealed class Tree<TValue>
    {
        private class Node
        {
            public readonly int storedKey;
            public TValue storedValue;
            public Node left;
            public Node right;
            public int height;

            public Node(int key, TValue value)
            {
                this.storedValue = value;
                this.storedKey = key;
                this.height = 1;
            }
        }

        private Node root;

        public bool IsEmpty => this.root == null;

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

        private static int CalculateHeight(Node node)
        {
            if (node.left != null && node.right != null)
                return 1 + (node.left.height > node.right.height ? node.left.height : node.right.height);

            if (node.left == null && node.right == null)
                return 1;

            return 1 + (node.left?.height ?? node.right.height);
        }

        private static int GetBalance(Node node)
        {
            if (node.left != null && node.right != null)
                return node.left.height - node.right.height;

            if (node.left == null && node.right == null)
                return 0;

            return node.left?.height ?? node.right.height * -1;
        }

        private static Node RotateLeft(Node node)
        {
            var current = node.right;
            var left = current.left;

            current.left = node;
            node.right = left;

            node.height = CalculateHeight(node);
            current.height = CalculateHeight(current);

            return current;
        }

        private static Node RotateRight(Node node)
        {
            var current = node.left;
            var right = current.right;

            current.right = node;
            node.left = right;

            node.height = CalculateHeight(node);
            current.height = CalculateHeight(current);

            return current;
        }

        private Node Add(Node node, int key, TValue value)
        {
            if (node == null)
                return new Node(key, value);

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

        public IEnumerable<TValue> Walk()
        {
            if (this.root == null)
                yield break;

            switch (this.root.height)
            {
                case 1:
                    yield return this.root.storedValue;
                    break;
                case 2:
                    if (this.root.left != null)
                        yield return this.root.left.storedValue;
                    yield return this.root.storedValue;
                    if (this.root.right != null)
                        yield return this.root.right.storedValue;
                    break;
                default:
                    {
                        var nodes = new Node[this.root.height - 2];
                        var currentNode = this.root;
                        var index = -1;

                        while (true)
                        {
                            if (currentNode != null)
                            {
                                if (currentNode.height == 2)
                                {
                                    if (currentNode.left != null)
                                        yield return currentNode.left.storedValue;
                                    yield return currentNode.storedValue;
                                    if (currentNode.right != null)
                                        yield return currentNode.right.storedValue;

                                    if (index == -1)
                                        break;
                                    currentNode = nodes[index--];
                                    yield return currentNode.storedValue;

                                    currentNode = currentNode.right;
                                }
                                else if (currentNode.height == 1)
                                {
                                    yield return currentNode.storedValue;

                                    if (index == -1)
                                        break;
                                    currentNode = nodes[index--];
                                    yield return currentNode.storedValue;

                                    currentNode = currentNode.right;
                                }
                                else
                                {
                                    nodes[++index] = currentNode;
                                    currentNode = currentNode.left;
                                }
                            }
                            else
                            {
                                if (index == -1)
                                    break;
                                currentNode = nodes[index--];
                                yield return currentNode.storedValue;

                                currentNode = currentNode.right;
                            }

                        }

                        break;
                    }
            }
        }
    }
}
