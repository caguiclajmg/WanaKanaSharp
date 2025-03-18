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

namespace WanaKanaSharp.Utility;

public delegate TValue ValueMergerDelegate<TKey, TValue>(Node<TKey, TValue> left, Node<TKey, TValue> right) where TKey : notnull;
public delegate void ChildrenMergerDelegate<TKey, TValue>(Node<TKey, TValue> left, Node<TKey, TValue> right) where TKey : notnull;

public abstract class Node<TKey, TValue>
    : IEnumerable<Node<TKey, TValue>>
    where TKey : notnull
{
    public abstract TKey Key { get; protected set; }
    public abstract TValue Value { get; set; }
    public abstract Node<TKey, TValue>? Parent { get; protected set; }

    protected readonly Dictionary<TKey, Node<TKey, TValue>> Children = [];
    public bool IsLeaf { get => Children.Count == 0; }

    public Node<TKey, TValue> this[TKey key] => Children[key];

    public bool ContainsKey(TKey key) => Children.ContainsKey(key);

    public abstract Node<TKey, TValue> Duplicate(bool copyChildren = false);

    public Node<TKey, TValue>? GetChild(TKey key)
    {
        if (Children.TryGetValue(key, out Node<TKey, TValue>? node)) return node;

        return null;
    }

    public abstract IEnumerator<Node<TKey, TValue>> GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public Node<TKey, TValue> Insert(Node<TKey, TValue> child)
    {
        child.Parent = this;
        Children.Add(child.Key, child);
        return child;
    }

    public void Insert(params Node<TKey, TValue>[] children)
    {
        foreach (var child in children)
        {
            Insert(child);
        }
    }

    public abstract Node<TKey, TValue> Insert((TKey Key, TValue Value) child);

    public void Insert(params (TKey Key, TValue Value)[] children)
    {
        foreach (var child in children)
        {
            Insert(child);
        }
    }

    public void Merge(
        Node<TKey, TValue> other,
        ValueMergerDelegate<TKey, TValue> valueMerger,
        ChildrenMergerDelegate<TKey, TValue> childrenMerger
    )
    {
        Value = valueMerger(this, other);
        childrenMerger(this, other);
    }

    public void Merge(Node<TKey, TValue> other)
    {
        Merge(other, ValueMerger, ChildrenMerger);

        static void ChildrenMerger(Node<TKey, TValue> left, Node<TKey, TValue> right)
        {
            var newChildren = new Dictionary<TKey, Node<TKey, TValue>>();

            foreach (var (key, child) in left.Children)
            {
                if (!right.Children.TryGetValue(key, out var otherChild)) continue;
                // we both have the key, recursively merge
                child.Merge(otherChild, ValueMerger, ChildrenMerger);
            }

            foreach (var (key, otherChild) in right.Children)
            {
                if (left.Children.ContainsKey(key)) continue;
                // we don't have this key, create a duplicate and add to self
                left.Insert(otherChild.Duplicate(true));
            }
        }

        static TValue ValueMerger(Node<TKey, TValue> left, Node<TKey, TValue> right) => right.Value;
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

    public void Traverse(Action<Node<TKey, TValue>> action, int maxDepth = -1)
    {
        Traverse(action, 0, maxDepth);
    }

    public void TraverseChildren(Action<Node<TKey, TValue>> action, int maxDepth = 0)
    {
        foreach (var child in this)
        {
            child.Traverse(action, 0, maxDepth);
        }
    }

    void Traverse(Action<Node<TKey, TValue>> action, int currentDepth, int maxDepth)
    {
        action(this);

        if (currentDepth == maxDepth) return;

        foreach (var child in this)
        {
            child.Traverse(action, currentDepth + 1, maxDepth);
        }
    }
}

public abstract class Trie<TKey, TValue>(Node<TKey, TValue> root)
    where TKey : notnull
{
    public Node<TKey, TValue> this[TKey key]
    {
        get { return Root[key]; }
    }

    public Node<TKey, TValue> Root { get; protected init; } = root;
}

public static class Trie
{
    public static TTrie Merge<TKey, TValue, TTrie>(
        Trie<TKey, TValue> a,
        Trie<TKey, TValue> b,
        ValueMergerDelegate<TKey, TValue> valueMerger,
        ChildrenMergerDelegate<TKey, TValue> childrenMerger
    ) where TKey : notnull where TTrie : Trie<TKey, TValue>, new()
    {
        var trie = new TTrie();
        var root = trie.Root;
        root.Merge(a.Root, valueMerger, childrenMerger);
        root.Merge(b.Root, valueMerger, childrenMerger);
        return trie;
    }

    public static TTrie Merge<TKey, TValue, TTrie>(
        Trie<TKey, TValue> a,
        Trie<TKey, TValue> b
    ) where TKey : notnull where TTrie : Trie<TKey, TValue>, new()
    {
        var trie = new TTrie();
        var root = trie.Root;
        root.Merge(a.Root);
        root.Merge(b.Root);

        return trie;
    }
}