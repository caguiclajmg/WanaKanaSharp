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

namespace WanaKanaSharp.Utility {
    public class Trie<TKey, TValue> {
        public class Node : Dictionary<TKey, Node> {
            public TValue Value { get; set; }

            public Node() { Value = default; }
            public Node(TValue value) { Value = value; }

            public Node Duplicate(bool duplicateChildren = true) {
                var node = new Node { Value = Value };

                foreach(var pair in this) node.Add(pair.Key, duplicateChildren ? pair.Value.Duplicate(duplicateChildren) : pair.Value);

                return node;
            }

            public Node Add(TKey key, TValue value) {
                var node = new Node() { Value = value };
                Add(key, node);
                return node;
            }

            public Node Add((TKey Key, TValue Value) pair) => Add(pair.Key, pair.Value);

            public IEnumerable<Node> Add(params (TKey Key, TValue Value)[] pairs) {
                var nodes = new List<Node>();
                foreach(var pair in pairs) nodes.Add(Add(pair));
                return nodes;
            }

            public Node Add(IEnumerable<TKey> path, TValue value) {
                var current = this;
                foreach(var key in path) current = current.ContainsKey(key) ? current[key] : current.Add(key, default);
                current.Value = value;
                return current;
            }

            public Node Add(IEnumerable<TKey> path, Node node) {
                var current = this;
                foreach(var key in path.Take(path.Count() -1)) current = current.ContainsKey(key) ? current[key] : current.Add(key, default);
                current.Add(path.Last(), node);
                return node;
            }

            public void TraverseChildren(Action<KeyValuePair<TKey, Node>> action, uint? maxDepth = null) {
                TraverseChildren(action, 0, new List<Node>(), maxDepth);
            }

            private void TraverseChildren(Action<KeyValuePair<TKey, Node>> action, uint currentDepth, IList<Node> visited, uint? maxDepth) {
                if(maxDepth.HasValue && currentDepth == maxDepth.Value) return;

                foreach(var pair in this) {
                    if(visited.Contains(pair.Value)) continue;
                    visited.Add(pair.Value);

                    action(pair);
                    pair.Value.TraverseChildren(action, currentDepth + 1, visited, maxDepth);
                }
            }

            public Node TryGetChild(TKey key) {
                return ContainsKey(key) ? this[key] : null;
            }

            public Node TryGetChild(IEnumerable<TKey> path) {
                var current = this;
                foreach(var key in path) if(ContainsKey(key)) current = current[key]; else return null;
                return current;
            }
        }

        public static Trie<TKey, TValue> Merge(Trie<TKey, TValue> a, Trie<TKey, TValue> b) {
            var trie = new Trie<TKey, TValue>();
            var root = trie.Root;

            foreach(var pair in a.Root) root.Add(pair.Key, pair.Value.Duplicate());
            foreach(var pair in b.Root) root.Add(pair.Key, pair.Value.Duplicate());

            return trie;
        }

        public Node Root { get; } = new Node();
    }
}
