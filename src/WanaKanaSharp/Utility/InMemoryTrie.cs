using System;
using System.Collections;
using System.Collections.Generic;

namespace WanaKanaSharp.Utility;

public class InMemoryTrie<TKey, TValue> : Trie<TKey, TValue>
    where TKey : notnull
{
    class NodeImpl(TKey key, TValue value)
        : Node<TKey, TValue>
    {
        class Enumerator(Dictionary<TKey, Node<TKey, TValue>>.Enumerator enumerator) : IEnumerator<Node<TKey, TValue>>
        {
            private bool _disposed;

            Dictionary<TKey, Node<TKey, TValue>>.Enumerator DictionaryEnumerator = enumerator;

            public Node<TKey, TValue> Current { get => DictionaryEnumerator.Current.Value; }

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

            public void Reset() => ((IEnumerator<KeyValuePair<TKey, Node<TKey, TValue>>>)DictionaryEnumerator).Reset();
        }

        public override TKey Key { get; protected set; } = key;
        public override Node<TKey, TValue>? Parent { get; protected set; } = null;
        public override TValue Value { get; set; } = value;

        public NodeImpl() : this(default!, default!) { }

        public override IEnumerator<Node<TKey, TValue>> GetEnumerator()
        {
            return new Enumerator(Children.GetEnumerator());
        }

        public override Node<TKey, TValue> Duplicate(bool copyChildren = false)
        {
            var node = new NodeImpl(Key, Value);

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

        public override Node<TKey, TValue> Insert((TKey Key, TValue Value) child)
        {
            var node = new NodeImpl(child.Key, child.Value);
            return Insert(node);
        }
    }

    public readonly static InMemoryTrie<TKey, TValue> Empty = new();

    public InMemoryTrie() : base(new NodeImpl()) { }
}

public static class InMemoryTrie
{
    public static InMemoryTrie<TKey, TValue> Merge<TKey, TValue>(
        Trie<TKey, TValue> a,
        Trie<TKey, TValue> b,
        ValueMergerDelegate<TKey, TValue> valueMerger,
        ChildrenMergerDelegate<TKey, TValue> childrenMerger
    ) where TKey : notnull => Trie.Merge<TKey, TValue, InMemoryTrie<TKey, TValue>>(a, b, valueMerger, childrenMerger);

    public static InMemoryTrie<TKey, TValue> Merge<TKey, TValue>(
        Trie<TKey, TValue> a,
        Trie<TKey, TValue> b
    ) where TKey : notnull => Trie.Merge<TKey, TValue, InMemoryTrie<TKey, TValue>>(a, b);
}