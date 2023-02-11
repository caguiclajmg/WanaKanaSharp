//
// Trie.cs
//
// Author:
//       John Mark Gabriel Caguicla <jmg.caguicla@yozuru.jp>
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
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;

namespace WanaKanaSharp.Utility
{
    public class Trie<TKey, TValue>
    {
        public static Trie<TKey, TValue> Empty { get; } = new Trie<TKey, TValue>();

        public class Node : Dictionary<TKey, Node>
        {
            new class Enumerator : IEnumerator<Node>
            {
                Dictionary<TKey, Node>.Enumerator DictionaryEnumerator;

                public Node Current => DictionaryEnumerator.Current.Value;

                object IEnumerator.Current => Current;

                public Enumerator(Dictionary<TKey, Node>.Enumerator enumerator)
                {
                    DictionaryEnumerator = enumerator;
                }

                public void Dispose()
                {
                    DictionaryEnumerator.Dispose();
                }

                public bool MoveNext() => DictionaryEnumerator.MoveNext();

                public void Reset() => throw new NotImplementedException();
            }

            public TKey Key { get; private set; }
            public Node Parent { get; private set; }
            public TValue Value { get; set; }

            readonly Dictionary<TKey, Node> Children = new Dictionary<TKey, Node>();
            public bool IsLeaf { get => Children.Count == 0; }

            public Node() { Value = default; }
            public Node(TValue value) { Value = value; }

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

            public void TraverseChildren(Action<KeyValuePair<TKey, Node>> action, uint? maxDepth = null)
            {
                TraverseChildren(action, 0, new List<Node>(), maxDepth);
            }

            private void TraverseChildren(Action<KeyValuePair<TKey, Node>> action, uint currentDepth, IList<Node> visited, uint? maxDepth)
            {
                if (maxDepth.HasValue && currentDepth == maxDepth.Value) return;

                foreach (var pair in this)
                {
                    if (visited.Contains(pair.Value)) continue;
                    visited.Add(pair.Value);

                    action(pair);
                    pair.Value.TraverseChildren(action, currentDepth + 1, visited, maxDepth);
                }
            }

            public Node TryGetChild(TKey key)
            {
                return ContainsKey(key) ? this[key] : null;
            }

            public Node Insert(Node child)
            {
                child.Parent = this;
                Children.Add(child.Key, child);
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
                    Insert(child);
                }
            }

            public void Merge(Node other, Func<Node, Node, TValue> valueMerger)
            {
                Value = valueMerger(this, other);

                foreach (var child in other.Children)
                {
                    if (Children.ContainsKey(child.Key))
                    {
                        Children[child.Key].Merge(child.Value, valueMerger);
                    }
                    else
                    {
                        Insert(child.Value.Duplicate(true));
                    }
                }
            }

            public void Remove(params TKey[] keys)
            {
                foreach (var key in keys)
                {
                    if (!Children.ContainsKey(key)) throw new KeyNotFoundException();

                    var node = Children[key];
                    node.Key = default;
                    Children.Remove(key);
                }
            }

            public void Traverse(Action<Node> action, int maxDepth = -1)
            {
                Traverse(action, 0, maxDepth);
            }

            public void TraverseChildren(Action<Node> action, int maxDepth = 0)
            {
                foreach (var child in this)
                {
                    child.Value.Traverse(action, 0, maxDepth);
                }
            }

            void Traverse(Action<Node> action, int currentDepth, int maxDepth)
            {
                action(this);

                if (currentDepth == maxDepth) return;

                foreach (var child in this)
                {
                    child.Value.Traverse(action, currentDepth + 1, maxDepth);
                }
            }

            public Node TryGetChild(IEnumerable<TKey> path)
            {
                var current = this;
                foreach (var key in path) if (ContainsKey(key)) current = current[key]; else return null;
                return current;
            }
        }

        public Node this[TKey key]
        {
            get { return Root[key]; }
        }

        public void Merge(Trie<TKey, TValue> trie, Func<Node, Node, TValue> valueMerger)
        {
            Root.Merge(trie.Root, valueMerger);
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