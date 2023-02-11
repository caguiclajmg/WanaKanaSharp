//
// Trie.cs
//
// Author:
//       John Mark Gabriel Caguicla <jmg.caguicla@guarandoo.me>
//
// Copyright (c) 2020 John Mark Gabriel Caguicla
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace WanaKanaSharp.Utility
{
    public static class TrieExtensions
    {
        public static TValue GetValueOrDefault<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> dictionary, TKey key)
        {
            if (!dictionary.TryGetValue(key, out var value)) return default;
            return value;
        }

        public static Trie<TKey, TValue>.Node GetValueOrDefault<TKey, TValue>(this Trie<TKey, TValue>.Node node, IEnumerable<TKey> path)
        {
            var current = node;
            foreach (var key in path) if (!current.TryGetValue(key, out current)) return null;
            return current;
        }
    }

    public class Trie<TKey, TValue>
    {
        public static Trie<TKey, TValue> Empty { get; } = new Trie<TKey, TValue>();

        public class Node : Dictionary<TKey, Node>
        {
            public delegate void NodeVisitor(IEnumerable<TKey> path, TKey key, Node node);

            public TKey Key { get; private set; }
            public Node Parent { get; private set; }
            public TValue Value { get; set; }

            public bool IsLeaf { get => Count == 0; }

            public Node() { Value = default; }
            public Node(TValue value) { Value = value; }

            public Node this[IEnumerable<TKey> path]
            {
                get
                {
                    var current = this;
                    foreach (var key in path) current = current[key];
                    return current;
                }
            }

            public Node Duplicate(bool duplicateChildren = true)
            {
                var node = new Node { Value = Value };

                foreach (var pair in this) node.Add(pair.Key, duplicateChildren ? pair.Value.Duplicate(duplicateChildren) : pair.Value);

                return node;
            }

            public Node Add(TKey key, TValue value)
            {
                var node = new Node() { Value = value };
                Add(key, node);
                return node;
            }

            public Node Add((TKey Key, TValue Value) pair) => Add(pair.Key, pair.Value);

            public IEnumerable<Node> Add(params (TKey Key, TValue Value)[] pairs)
            {
                var nodes = new List<Node>();
                foreach (var pair in pairs) nodes.Add(Add(pair));
                return nodes;
            }

            public IEnumerable<Node> Add(params (IEnumerable<TKey> Key, TValue Value)[] pairs)
            {
                var nodes = new List<Node>();
                foreach (var pair in pairs) nodes.Add(Add(pair.Key, pair.Value));
                return nodes;
            }

            public Node Add(IEnumerable<TKey> path, TValue value)
            {
                var current = this;
                foreach (var key in path) current = current.ContainsKey(key) ? current[key] : current.Add(key, default);
                current.Value = value;
                return current;
            }

            public Node Add(IEnumerable<TKey> path, Node node)
            {
                var current = this;
                foreach (var key in path.Take(path.Count() - 1)) current = current.ContainsKey(key) ? current[key] : current.Add(key, default);
                current.Add(path.Last(), node);
                return node;
            }

            public void TraverseChildren(NodeVisitor visitor, uint? maxDepth = null)
            {
                TraverseChildren(visitor, Array.Empty<TKey>(), 0, new List<Node>(), maxDepth);
            }

            private void TraverseChildren(
                NodeVisitor visitor,
                IEnumerable<TKey> path,
                uint currentDepth,
                IList<Node> visited,
                uint? maxDepth
            )
            {
                if (maxDepth.HasValue && currentDepth == maxDepth.Value) return;

                foreach (var pair in this)
                {
                    if (visited.Contains(pair.Value)) continue;
                    visited.Add(pair.Value);

                    var newPath = path.Append(pair.Key);
                    visitor(newPath, pair.Key, pair.Value);
                    pair.Value.TraverseChildren(visitor, newPath, currentDepth + 1, visited, maxDepth);
                }
            }

            public Node Insert(Node child)
            {
                child.Parent = this;
                Add(child.Key, child);
                return child;
            }

            public void Insert(params Node[] children)
            {
                foreach (var child in children)
                {
                    Insert(child);
                }
            }

            public void Insert(params (TKey Key, TValue Value)[] children)
            {
                foreach (var child in children)
                {
                    var node = new Node(child.Value) { Key = child.Key };
                    Insert(node);
                }
            }

            public void Merge(Node other, Func<Node, Node, TValue> valueMerger)
            {
                Value = valueMerger(this, other);

                foreach (var child in other)
                {
                    if (ContainsKey(child.Key))
                    {
                        this[child.Key].Merge(child.Value, valueMerger);
                    }
                    else
                    {
                        var copy = child.Value.Duplicate(true);
                        copy.Key = child.Key;
                        Insert(copy);
                    }
                }
            }

            public void Remove(params TKey[] keys)
            {
                foreach (var key in keys)
                {
                    if (!ContainsKey(key)) throw new KeyNotFoundException();

                    var node = this[key];
                    node.Key = default;
                    Remove(key);
                }
            }

            public void Traverse(NodeVisitor visitor, int? maxDepth = null)
            {
                foreach (var child in this)
                {
                    var key = child.Key;
                    var node = child.Value;
                    var path = Array.Empty<TKey>();
                    var visited = new HashSet<Node>();
                    visitor(path.Append(key), key, node);
                    visited.Add(node);
                    node.Traverse(visitor, path, visited, 0, maxDepth);
                }
            }

            private void Traverse(NodeVisitor visitor, IEnumerable<TKey> path, ICollection<Node> visited, int currentDepth, int? maxDepth)
            {
                if (visited.Contains(this)) return;
                if (maxDepth.HasValue && currentDepth > maxDepth) return;

                foreach (var child in this)
                {
                    var key = child.Key;
                    var node = child.Value;
                    var currentPath = path.Append(key);
                    visitor(currentPath, key, node);
                    visited.Add(node);
                    node.Traverse(visitor, currentPath, visited, currentDepth + 1, maxDepth);
                }
            }
        }

        public Node this[TKey key]
        {
            get { return Root[key]; }
        }

        public void Merge(Trie<TKey, TValue> trie)
        {
            Merge(trie, (t, u) => u.Value);
        }

        public void Merge(Trie<TKey, TValue> trie, Func<Node, Node, TValue> valueMerger)
        {
            Root.Merge(trie.Root, valueMerger);
        }

        public static Trie<TKey, TValue> Merge(Trie<TKey, TValue> a, Trie<TKey, TValue> b)
        {
            return Merge(a, b, (t, u) => u.Value);
        }

        public static Trie<TKey, TValue> Merge(Trie<TKey, TValue> a, Trie<TKey, TValue> b, Func<Node, Node, TValue> valueMerger)
        {
            var trie = new Trie<TKey, TValue>();
            var root = trie.Root;
            root.Merge(a.Root, valueMerger);
            root.Merge(b.Root, valueMerger);

            return trie;
        }

        public Node Root { get; } = new Node();
    }
}