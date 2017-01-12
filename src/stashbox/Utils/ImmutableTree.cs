/*
The MIT License (MIT)

Copyright (c) 2013 Maksim Volkau

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all 
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
*/

/* Source used: https://bitbucket.org/dadhi/dryioc/src/73d3dd5b00756d28e39c690ab7804ce2dd21f6e2/DryIoc/Container.cs?at=dev&fileviewer=file-view-default */

using System;
using System.Collections.Generic;

namespace Stashbox.Utils
{
    /// <summary>
    /// Delegate for updating a node.
    /// </summary>
    /// <typeparam name="V">The node type.</typeparam>
    /// <param name="oldValue">The old value.</param>
    /// <param name="newValue">The new value</param>
    /// <returns></returns>
    public delegate V Update<V>(V oldValue, V newValue);

    /// <summary>
    /// Represents an immutable AVL Tree.
    /// </summary>
    /// <typeparam name="K">The key type.</typeparam>
    /// <typeparam name="V">The value type.</typeparam>
    public sealed class ImmutableTree<K, V>
    {
        /// <summary>Empty tree to start with.</summary>
        public static readonly ImmutableTree<K, V> Empty = new ImmutableTree<K, V>();

        /// <summary>Key of type K that should support <see cref="object.Equals(object)"/> and <see cref="object.GetHashCode"/>.</summary>
        public readonly K Key;

        /// <summary>Value of any type V.</summary>
        public readonly V Value;

        /// <summary>Calculated key hash.</summary>
        public readonly int Hash;

        /// <summary>In case of <see cref="Hash"/> conflicts for different keys contains conflicted keys with their values.</summary>
        public readonly KeyValuePair<K, V>[] Conflicts;

        /// <summary>Left sub-tree/branch, or empty.</summary>
        public readonly ImmutableTree<K, V> Left;

        /// <summary>Right sub-tree/branch, or empty.</summary>
        public readonly ImmutableTree<K, V> Right;

        /// <summary>Height of longest sub-tree/branch plus 1. It is 0 for empty tree, and 1 for single node tree.</summary>
        public readonly int Height;

        /// <summary>Returns true if tree is empty.</summary>
        public bool IsEmpty => Height == 0;

        /// <summary>Returns new tree with added key-value. If value with the same key is exist, then
        /// if <paramref name="update"/> is not specified: then existing value will be replaced by <paramref name="value"/>;
        /// if <paramref name="update"/> is specified: then update delegate will decide what value to keep.</summary>
        /// <param name="key">Key to add.</param><param name="value">Value to add.</param>
        /// <param name="update">(optional) Delegate to decide what value to keep: old or new one.</param>
        /// <returns>New tree with added or updated key-value.</returns>
        public ImmutableTree<K, V> AddOrUpdate(K key, V value, Update<V> update = null)
        {
            return AddOrUpdate(key.GetHashCode(), key, value, update, updateOnly: false);
        }

        /// <summary>Looks for <paramref name="key"/> and replaces its value with new <paramref name="value"/>, or 
        /// runs custom update handler (<paramref name="update"/>) with old and new value to get the updated result.</summary>
        /// <param name="key">Key to look for.</param>
        /// <param name="value">New value to replace key value with.</param>
        /// <param name="update">(optional) Delegate for custom update logic, it gets old and new <paramref name="value"/>
        /// as inputs and should return updated value as output.</param>
        /// <returns>New tree with updated value or the SAME tree if no key found.</returns>
        public ImmutableTree<K, V> Update(K key, V value, Update<V> update = null)
        {
            return AddOrUpdate(key.GetHashCode(), key, value, update, updateOnly: true);
        }

        /// <summary>Looks for key in a tree and returns the key value if found, or <paramref name="defaultValue"/> otherwise.</summary>
        /// <param name="key">Key to look for.</param> <param name="defaultValue">(optional) Value to return if key is not found.</param>
        /// <returns>Found value or <paramref name="defaultValue"/>.</returns>
        public V GetValueOrDefault(K key, V defaultValue = default(V))
        {
            var t = this;
            var hash = key.GetHashCode();
            while (t.Height != 0 && t.Hash != hash)
                t = hash < t.Hash ? t.Left : t.Right;
            return t.Height != 0 && (ReferenceEquals(key, t.Key) || key.Equals(t.Key))
                ? t.Value : t.GetConflictedValueOrDefault(key, defaultValue);
        }

        /// <summary>Depth-first in-order traversal as described in http://en.wikipedia.org/wiki/Tree_traversal
        /// The only difference is using fixed size array instead of stack for speed-up (~20% faster than stack).</summary>
        /// <returns>Sequence of enumerated key value pairs.</returns>
        public IEnumerable<KeyValuePair<K, V>> Enumerate()
        {
            if (Height == 0)
                yield break;

            var parents = new ImmutableTree<K, V>[Height];

            var tree = this;
            var parentCount = -1;
            while (tree.Height != 0 || parentCount != -1)
            {
                if (tree.Height != 0)
                {
                    parents[++parentCount] = tree;
                    tree = tree.Left;
                }
                else
                {
                    tree = parents[parentCount--];
                    yield return new KeyValuePair<K, V>(tree.Key, tree.Value);

                    if (tree.Conflicts != null)
                        for (var i = 0; i < tree.Conflicts.Length; i++)
                            yield return tree.Conflicts[i];

                    tree = tree.Right;
                }
            }
        }

        #region Implementation

        private ImmutableTree() { }

        private ImmutableTree(int hash, K key, V value, KeyValuePair<K, V>[] conficts, ImmutableTree<K, V> left, ImmutableTree<K, V> right)
        {
            Hash = hash;
            Key = key;
            Value = value;
            Conflicts = conficts;
            Left = left;
            Right = right;
            Height = 1 + (left.Height > right.Height ? left.Height : right.Height);
        }

        private ImmutableTree<K, V> AddOrUpdate(int hash, K key, V value, Update<V> update, bool updateOnly)
        {
            return Height == 0 ? (updateOnly ? this : new ImmutableTree<K, V>(hash, key, value, null, Empty, Empty))
                : (hash == Hash ? UpdateValueAndResolveConflicts(key, value, update, updateOnly)
                : (hash < Hash
                    ? With(Left.AddOrUpdate(hash, key, value, update, updateOnly), Right)
                    : With(Left, Right.AddOrUpdate(hash, key, value, update, updateOnly))).KeepBalanced());
        }

        private ImmutableTree<K, V> UpdateValueAndResolveConflicts(K key, V value, Update<V> update, bool updateOnly)
        {
            if (ReferenceEquals(Key, key) || Key.Equals(key))
                return new ImmutableTree<K, V>(Hash, key, update == null ? value : update(Value, value), Conflicts, Left, Right);

            if (Conflicts == null) // add only if updateOnly is false.
                return updateOnly ? this
                    : new ImmutableTree<K, V>(Hash, Key, Value, new[] { new KeyValuePair<K, V>(key, value) }, Left, Right);

            var found = Conflicts.Length - 1;
            while (found >= 0 && !Equals(Conflicts[found].Key, Key)) --found;
            if (found == -1)
            {
                if (updateOnly) return this;
                var newConflicts = new KeyValuePair<K, V>[Conflicts.Length + 1];
                Array.Copy(Conflicts, 0, newConflicts, 0, Conflicts.Length);
                newConflicts[Conflicts.Length] = new KeyValuePair<K, V>(key, value);
                return new ImmutableTree<K, V>(Hash, Key, Value, newConflicts, Left, Right);
            }

            var conflicts = new KeyValuePair<K, V>[Conflicts.Length];
            Array.Copy(Conflicts, 0, conflicts, 0, Conflicts.Length);
            conflicts[found] = new KeyValuePair<K, V>(key, update == null ? value : update(Conflicts[found].Value, value));
            return new ImmutableTree<K, V>(Hash, Key, Value, conflicts, Left, Right);
        }

        private V GetConflictedValueOrDefault(K key, V defaultValue)
        {
            if (Conflicts != null)
                for (var i = 0; i < Conflicts.Length; i++)
                    if (Equals(Conflicts[i].Key, key))
                        return Conflicts[i].Value;
            return defaultValue;
        }

        private ImmutableTree<K, V> KeepBalanced()
        {
            var delta = Left.Height - Right.Height;
            return delta >= 2 ? With(Left.Right.Height - Left.Left.Height == 1 ? Left.RotateLeft() : Left, Right).RotateRight()
                : (delta <= -2 ? With(Left, Right.Left.Height - Right.Right.Height == 1 ? Right.RotateRight() : Right).RotateLeft()
                : this);
        }

        private ImmutableTree<K, V> RotateRight()
        {
            return Left.With(Left.Left, With(Left.Right, Right));
        }

        private ImmutableTree<K, V> RotateLeft()
        {
            return Right.With(With(Left, Right.Left), Right.Right);
        }

        private ImmutableTree<K, V> With(ImmutableTree<K, V> left, ImmutableTree<K, V> right)
        {
            return left == Left && right == Right ? this : new ImmutableTree<K, V>(Hash, Key, Value, Conflicts, left, right);
        }

        #endregion
    }

    /// <summary>Simple immutable AVL tree with integer keys and object values.</summary>
    public sealed class ImmutableTree<V>
    {
        /// <summary>Empty tree to start with.</summary>
        public static readonly ImmutableTree<V> Empty = new ImmutableTree<V>();

        /// <summary>Key.</summary>
        public readonly int Key;

        /// <summary>Value.</summary>
        public readonly V Value;

        /// <summary>Left sub-tree/branch, or empty.</summary>
        public readonly ImmutableTree<V> Left;

        /// <summary>Right sub-tree/branch, or empty.</summary>
        public readonly ImmutableTree<V> Right;

        /// <summary>Height of longest sub-tree/branch plus 1. It is 0 for empty tree, and 1 for single node tree.</summary>
        public readonly int Height;

        /// <summary>Returns true is tree is empty.</summary>
        public bool IsEmpty => Height == 0;

        /// <summary>Returns new tree with added or updated value for specified key.</summary>
        /// <param name="key"></param> <param name="value"></param>
        /// <returns>New tree.</returns>
        public ImmutableTree<V> AddOrUpdate(int key, V value)
        {
            return AddOrUpdate(key, value, false, null);
        }

        /// <summary>Delegate to calculate new value from and old and a new value.</summary>
        /// <param name="oldValue">Old</param> <param name="newValue">New</param> <returns>Calculated result.</returns>
        public delegate V UpdateValue(V oldValue, V newValue);

        /// <summary>Returns new tree with added or updated value for specified key.</summary>
        /// <param name="key">Key</param> <param name="value">Value</param>
        /// <param name="updateValue">(optional) Delegate to calculate new value from and old and a new value.</param>
        /// <returns>New tree.</returns>
        public ImmutableTree<V> AddOrUpdate(int key, V value, UpdateValue updateValue)
        {
            return AddOrUpdate(key, value, false, updateValue);
        }

        /// <summary>Returns new tree with updated value for the key, Or the same tree if key was not found.</summary>
        /// <param name="key"></param> <param name="value"></param>
        /// <returns>New tree if key is found, or the same tree otherwise.</returns>
        public ImmutableTree<V> Update(int key, V value)
        {
            return AddOrUpdate(key, value, true, null);
        }

        /// <summary>Get value for found key or null otherwise.</summary>
        /// <param name="key"></param> <returns>Found value or null.</returns>
        public V GetValueOrDefault(int key)
        {
            var tree = this;
            while (tree.Height != 0 && tree.Key != key)
                tree = key < tree.Key ? tree.Left : tree.Right;
            return tree.Height != 0 ? tree.Value : default(V);
        }

        /// <summary>Returns all sub-trees enumerated from left to right.</summary> 
        /// <returns>Enumerated sub-trees or empty if tree is empty.</returns>
        public IEnumerable<ImmutableTree<V>> Enumerate()
        {
            if (Height == 0)
                yield break;

            var parents = new ImmutableTree<V>[Height];

            var tree = this;
            var parentCount = -1;
            while (tree.Height != 0 || parentCount != -1)
            {
                if (tree.Height != 0)
                {
                    parents[++parentCount] = tree;
                    tree = tree.Left;
                }
                else
                {
                    tree = parents[parentCount--];
                    yield return tree;
                    tree = tree.Right;
                }
            }
        }

        #region Implementation

        private ImmutableTree() { }

        private ImmutableTree(int key, V value, ImmutableTree<V> left, ImmutableTree<V> right)
        {
            Key = key;
            Value = value;
            Left = left;
            Right = right;
            Height = 1 + (left.Height > right.Height ? left.Height : right.Height);
        }

        private ImmutableTree<V> AddOrUpdate(int key, V value, bool updateOnly, UpdateValue update)
        {
            return Height == 0 ? // tree is empty
                    (updateOnly ? this : new ImmutableTree<V>(key, value, Empty, Empty))
                : (key == Key ? // actual update
                    new ImmutableTree<V>(key, update == null ? value : update(Value, value), Left, Right)
                : (key < Key    // try update on left or right sub-tree
                    ? With(Left.AddOrUpdate(key, value, updateOnly, update), Right)
                    : With(Left, Right.AddOrUpdate(key, value, updateOnly, update))).KeepBalanced());
        }

        private ImmutableTree<V> KeepBalanced()
        {
            var delta = Left.Height - Right.Height;
            return delta >= 2 ? With(Left.Right.Height - Left.Left.Height == 1 ? Left.RotateLeft() : Left, Right).RotateRight()
                : (delta <= -2 ? With(Left, Right.Left.Height - Right.Right.Height == 1 ? Right.RotateRight() : Right).RotateLeft()
                : this);
        }

        private ImmutableTree<V> RotateRight()
        {
            return Left.With(Left.Left, With(Left.Right, Right));
        }

        private ImmutableTree<V> RotateLeft()
        {
            return Right.With(With(Left, Right.Left), Right.Right);
        }

        private ImmutableTree<V> With(ImmutableTree<V> left, ImmutableTree<V> right)
        {
            return left == Left && right == Right ? this : new ImmutableTree<V>(Key, Value, left, right);
        }

        #endregion
    }
}
