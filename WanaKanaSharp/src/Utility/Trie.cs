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
using System.ComponentModel;
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

        public class Node
        {
            public delegate void NodeVisitor(IEnumerable<TKey> path, TKey key, Node node);

            public Node Parent { get; private set; }
            public TValue Value { get; set; }
            public bool IsLeaf { get => Children.Count == 0; }

            private Dictionary<TKey, Link> Children { get; } = new Dictionary<TKey, Link>();

            public Node() { Value = default; }
            public Node(TValue value) { Value = value; }

            public Node this[TKey key] { get => Children[key].Node; }
            public Node this[IEnumerable<TKey> path]
            {
                get
                {
                    var current = this;
                    foreach (var key in path) current = current.Children[key].Node;
                    return current;
                }
            }

            public Node Duplicate(bool duplicateChildren = true)
            {
                var node = new Node { Value = Value };
                foreach (var pair in Children) node.Add(pair.Key, pair.Value.Node.Duplicate(duplicateChildren));
                return node;
            }

            public Node Add(TKey key, TValue value)
            {
                var node = new Node() { Value = value };
                AddInternal(key, node);
                return node;
            }

            public IEnumerable<Node> Add(params (TKey Key, TValue Value)[] pairs)
            {
                var nodes = new List<Node>();
                foreach (var pair in pairs) nodes.Add(Add(pair.Key, pair.Value));
                return nodes;
            }

            public Node Add(IEnumerable<TKey> path, TValue value)
            {
                var current = this;
                foreach (var key in path) current = current.Children.ContainsKey(key) ? current[key] : current.Add(key, default(TValue));
                current.Value = value;
                return current;
            }

            public IEnumerable<Node> Add(params (IEnumerable<TKey> Key, TValue Value)[] pairs)
            {
                var nodes = new List<Node>();
                foreach (var pair in pairs) nodes.Add(Add(pair.Key, pair.Value));
                return nodes;
            }

            public Link Add(TKey key, Node node)
            {
                return AddInternal(key, node);
            }

            public Link Add(IEnumerable<TKey> path, Node node)
            {
                var current = this;
                foreach (var key in path.Take(path.Count() - 1)) current = current.Children.ContainsKey(key) ? current[key] : current.Add(key, default(TValue));
                return current.Add(path.Last(), node);
            }

            private Link AddInternal(TKey key, Node node)
            {
                var link = new Link(this, node);
                Children.Add(key, link);
                return link;
            }

            public void Merge(Node other, Func<Node, Node, TValue> valueMerger)
            {
                Value = valueMerger(this, other);

                foreach (var child in other.Children)
                {
                    if (Children.ContainsKey(child.Key))
                    {
                        this[child.Key].Merge(child.Value.Node, valueMerger);
                    }
                    else
                    {
                        Add(child.Key, child.Value.Node.Duplicate(true));
                    }
                }
            }

            public void Remove(params TKey[] keys)
            {
                foreach (var key in keys)
                {
                    if (!Children.ContainsKey(key)) throw new KeyNotFoundException();
                    Children.Remove(key);
                }
            }

            public bool TryGetValue(TKey key, out Node node)
            {
                if (!Children.TryGetValue(key, out Link link))
                {
                    node = null;
                    return false;
                }
                node = link.Node;
                return true;
            }

            public bool TryGetValue(IEnumerable<TKey> path, out Node node)
            {
                Link link = null;
                foreach (var key in path)
                {
                    if (!Children.TryGetValue(key, out link))
                    {
                        node = null;
                        return false;
                    }
                }
                node = link?.Node;
                return true;
            }

            public void Traverse(NodeVisitor visitor, int? maxDepth = null)
            {
                Traverse(this, visitor, Array.Empty<TKey>(), new HashSet<Node>(), 0, maxDepth);
            }

            private void Traverse(
                Node node,
                NodeVisitor visitor,
                IEnumerable<TKey> path,
                ISet<Node> visited,
                int currentDepth,
                int? maxDepth
            )
            {
                if (maxDepth.HasValue && currentDepth > maxDepth) return;

                foreach (var child in node.Children)
                {
                    if (visited.Contains(child.Value.Node)) return;

                    var key = child.Key;
                    var link = child.Value;
                    var currentPath = path.Append(key);
                    visitor(currentPath, key, link.Node);
                    var newVisited = new HashSet<Node>(visited) { link.Node };
                    link.Node.Traverse(link.Node, visitor, currentPath, newVisited, currentDepth + 1, maxDepth);
                }
            }
        }

        public class Link
        {
            public Node Parent { get; set; }
            public Node Node { get; set; }

            public Link(Node parent, Node node)
            {
                Parent = parent;
                Node = node;
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