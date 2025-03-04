﻿//
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

namespace WanaKanaSharp.Utility;

public class Trie<TKey, TValue> where TKey : notnull
{
    public static Trie<TKey, TValue> Empty { get; } = new Trie<TKey, TValue>();

    public class Node(TKey key, TValue value) : IEnumerable<Node>
    {
        class Enumerator(Dictionary<TKey, Node>.Enumerator enumerator) : IEnumerator<Node>
        {
            private bool _disposed;

            Dictionary<TKey, Node>.Enumerator DictionaryEnumerator = enumerator;

            public Node Current { get => DictionaryEnumerator.Current.Value; }

            object IEnumerator.Current => Current;

            protected void Dispose(bool disposing)
            {
                if (_disposed) return;
                if (disposing)
                {
                    DictionaryEnumerator.Dispose();
                }
                _disposed = true;
            }

            public void Dispose()
            {
                Dispose(disposing: true);
                GC.SuppressFinalize(this);
            }

            public bool MoveNext() => DictionaryEnumerator.MoveNext();

            public void Reset() => ((IEnumerator<KeyValuePair<TKey, Node>>)DictionaryEnumerator).Reset();
        }

        public TKey Key { get; private set; } = key;
        public Node? Parent { get; private set; } = null;
        public TValue Value { get; set; } = value;

        readonly Dictionary<TKey, Node> Children = [];
        public bool IsLeaf { get => Children.Count == 0; }

        public Node this[TKey key] => Children[key];

        public bool ContainsKey(TKey key) => Children.ContainsKey(key);

        public Node Duplicate(bool copyChildren = false)
        {
            var node = new Node(Key, Value);

            if (copyChildren)
            {
                foreach (var child in this)
                {
                    var c = child.Duplicate(copyChildren);
                    node.Insert(c);
                }
            }

            return node;
        }

        public Node? GetChild(TKey key)
        {
            if (Children.TryGetValue(key, out Node? node)) return node;

            return null;
        }

        public IEnumerator<Node> GetEnumerator()
        {
            return new Enumerator(Children.GetEnumerator());
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

        public Node Insert((TKey Key, TValue Value) child)
        {
            var node = new Node(child.Key, child.Value);
            return Insert(node);
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

            foreach (var otherChild in other.Children)
            {
                if (Children.TryGetValue(otherChild.Key, out var child))
                {
                    child.Merge(otherChild.Value, valueMerger);
                }
                else
                {
                    Insert(otherChild.Value.Duplicate(true));
                }
            }
        }

        public void Remove(params TKey[] keys)
        {
            foreach (var key in keys)
            {
                if (!Children.TryGetValue(key, out var child)) throw new KeyNotFoundException();

                var node = Children[key];
                node.Key = default!;
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
                child.Traverse(action, 0, maxDepth);
            }
        }

        void Traverse(Action<Node> action, int currentDepth, int maxDepth)
        {
            action(this);

            if (currentDepth == maxDepth) return;

            foreach (var child in this)
            {
                child.Traverse(action, currentDepth + 1, maxDepth);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public Node this[TKey key]
    {
        get { return Root[key]; }
    }

    public Node Root { get; } = new Node(default!, default!);

    public static Trie<TKey, TValue> Merge(
        Trie<TKey, TValue> a,
        Trie<TKey, TValue> b,
        Func<Node, Node, TValue> valueMerger
    )
    {
        var trie = new Trie<TKey, TValue>();
        var root = trie.Root;
        root.Merge(a.Root, valueMerger);
        root.Merge(b.Root, valueMerger);

        return trie;
    }
}
