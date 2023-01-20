using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Stashbox.Utils.Data;

[DebuggerTypeProxy(typeof(TreeDebugView<>))]
internal sealed class Tree<TValue>
{
    private class Node
    {
        public readonly int StoredKey;
        public TValue StoredValue;
        public Node? Left;
        public Node? Right;
        public int Height;

        public Node(int key, TValue value)
        {
            this.StoredValue = value;
            this.StoredKey = key;
            this.Height = 1;
        }
    }

    private Node? root;

    public bool IsEmpty => this.root == null;

    public void Add(int key, TValue value)
    {
        this.root = Add(this.root, key, value);
    }

    [MethodImpl(Constants.Inline)]
    public TValue? GetOrDefault(int key)
    {
        if (this.root == null)
            return default;

        var node = root;
        while (node != null && node.StoredKey != key)
            node = key < node.StoredKey ? node.Left : node.Right;
        return node == null ? default : node.StoredValue;
    }

    private static int CalculateHeight(Node node)
    {
        if (node.Left != null && node.Right != null)
            return 1 + (node.Left.Height > node.Right.Height ? node.Left.Height : node.Right.Height);

        if (node.Left == null && node.Right == null)
            return 1;

        return 1 + (node.Left?.Height ?? node.Right!.Height);
    }

    private static int GetBalance(Node node)
    {
        if (node.Left != null && node.Right != null)
            return node.Left.Height - node.Right.Height;

        if (node.Left == null && node.Right == null)
            return 0;

        return node.Left?.Height ?? node.Right!.Height * -1;
    }

    private static Node RotateLeft(Node node)
    {
        var current = node.Right;
        var left = current!.Left;

        current.Left = node;
        node.Right = left;

        node.Height = CalculateHeight(node);
        current.Height = CalculateHeight(current);

        return current;
    }

    private static Node RotateRight(Node node)
    {
        var current = node.Left;
        var right = current!.Right;

        current.Right = node;
        node.Left = right;

        node.Height = CalculateHeight(node);
        current.Height = CalculateHeight(current);

        return current;
    }

    private static Node Add(Node? node, int key, TValue value)
    {
        if (node == null)
            return new Node(key, value);

        if (node.StoredKey == key)
        {
            node.StoredValue = value;
            return node;
        }

        if (node.StoredKey > key)
            node.Left = Add(node.Left, key, value);
        else
            node.Right = Add(node.Right, key, value);

        node.Height = CalculateHeight(node);
        var balance = GetBalance(node);

        switch (balance)
        {
            case >= 2 when GetBalance(node.Left!) == -1:
                node.Left = RotateLeft(node.Left!);
                node = RotateRight(node);
                break;
            case >= 2:
                node = RotateRight(node);
                break;
            case <= -2 when GetBalance(node.Right!) == 1:
                node.Right = RotateRight(node.Right!);
                node = RotateLeft(node);
                break;
            case <= -2:
                node = RotateLeft(node);
                break;
        }

        return node;
    }

    public IEnumerable<TValue> Walk()
    {
        if (this.root == null)
            yield break;

        switch (this.root.Height)
        {
            case 1:
                yield return this.root.StoredValue;
                break;
            case 2:
                if (this.root.Left != null)
                    yield return this.root.Left.StoredValue;
                yield return this.root.StoredValue;
                if (this.root.Right != null)
                    yield return this.root.Right.StoredValue;
                break;
            default:
            {
                var nodes = new Node[this.root.Height - 2];
                var currentNode = this.root;
                var index = -1;

                while (true)
                {
                    if (currentNode != null)
                    {
                        if (currentNode.Height == 2)
                        {
                            if (currentNode.Left != null)
                                yield return currentNode.Left.StoredValue;
                            yield return currentNode.StoredValue;
                            if (currentNode.Right != null)
                                yield return currentNode.Right.StoredValue;

                            if (index == -1)
                                break;
                            currentNode = nodes[index--];
                            yield return currentNode.StoredValue;

                            currentNode = currentNode.Right;
                        }
                        else if (currentNode.Height == 1)
                        {
                            yield return currentNode.StoredValue;

                            if (index == -1)
                                break;
                            currentNode = nodes[index--];
                            yield return currentNode.StoredValue;

                            currentNode = currentNode.Right;
                        }
                        else
                        {
                            nodes[++index] = currentNode;
                            currentNode = currentNode.Left;
                        }
                    }
                    else
                    {
                        if (index == -1)
                            break;
                        currentNode = nodes[index--];
                        yield return currentNode.StoredValue;

                        currentNode = currentNode.Right;
                    }

                }

                break;
            }
        }
    }
}

internal class TreeDebugView<TValue>
{
    private readonly Tree<TValue> tree;

    public TreeDebugView(Tree<TValue> tree) { this.tree = tree; }

    [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
    public TValue[] Items { get { return tree.Walk().ToArray(); } }
}