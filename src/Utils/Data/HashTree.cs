using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Stashbox.Utils.Data;

[DebuggerTypeProxy(typeof(HashTreeDebugView<,>))]
internal sealed class HashTree<TKey, TValue>
    where TKey : class
{
    private class Node
    {
        public readonly int StoredHash;
        public readonly TKey StoredKey;
        public TValue StoredValue;
        public Node? Left;
        public Node? Right;
        public int Height;
        public ExpandableArray<TKey, TValue>? Collisions;

        public Node(TKey key, TValue value, int hash)
        {
            this.StoredValue = value;
            this.StoredKey = key;
            this.StoredHash = hash;
            this.Height = 1;
        }
    }

    private Node? root;

    public void Add(TKey key, TValue value)
    {
        this.root = Add(this.root, key, key.GetHashCode(), value);
    }

    [MethodImpl(Constants.Inline)]
    public TValue? GetOrDefault(TKey key)
    {
        if (this.root == null)
            return default;

        var node = root;
        var hash = key.GetHashCode();
        while (node != null && node.StoredHash != hash)
            node = hash < node.StoredHash ? node.Left : node.Right;
        return node != null && Equals(key, node.StoredKey)
            ? node.StoredValue
            : node?.Collisions == null
                ? default
                : node.Collisions.GetOrDefaultByValue(key);
    }

    private static int CalculateHeight(Node node)
    {
        if (node is { Left: not null, Right: not null })
            return 1 + (node.Left.Height > node.Right.Height ? node.Left.Height : node.Right.Height);

        if (node.Left == null && node.Right == null)
            return 1;

        return 1 + (node.Left?.Height ?? node.Right!.Height);
    }

    private static int GetBalance(Node node)
    {
        if (node is { Left: not null, Right: not null })
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

        current.Height = CalculateHeight(current);
        node.Height = CalculateHeight(node);

        return current;
    }

    private static Node RotateRight(Node node)
    {
        var current = node.Left;
        var right = current!.Right;
        current.Right = node;
        node.Left = right;
        current.Height = CalculateHeight(current);
        node.Height = CalculateHeight(node);

        return current;
    }

    private static Node Add(Node? node, TKey key, int hash, TValue value)
    {
        if (node == null)
            return new Node(key, value, hash);

        if (node.StoredHash == hash)
        {
            CheckCollisions(node, key, value);
            return node;
        }

        if (node.StoredHash > hash)
            node.Left = Add(node.Left, key, hash, value);
        else
            node.Right = Add(node.Right, key, hash, value);

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

    private static void CheckCollisions(Node node, TKey key, TValue value)
    {
        if (Equals(key, node.StoredKey))
            node.StoredValue = value;
        else
        {
            node.Collisions ??= [];
            node.Collisions.Add(new ReadOnlyKeyValue<TKey, TValue>(key, value));
        }
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

internal class HashTreeDebugView<TKey, TValue> where TKey : class
{
    private readonly HashTree<TKey, TValue> tree;

    public HashTreeDebugView(HashTree<TKey, TValue> tree) { this.tree = tree; }

    [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
    public TValue[] Items => tree.Walk().ToArray();
}