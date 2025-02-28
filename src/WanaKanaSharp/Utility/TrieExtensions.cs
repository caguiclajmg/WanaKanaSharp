using System;

namespace WanaKanaSharp.Utility;

public static class TrieExtensions
{
    public static void Merge<TKey, TValue>(
        this Trie<TKey, TValue> trie,
        Trie<TKey, TValue> other,
        Func<Trie<TKey, TValue>.Node, Trie<TKey, TValue>.Node, TValue> valueMerger
    ) where TKey : notnull
    {
        trie.Root.Merge(other.Root, valueMerger);
    }

    public static Trie<TKey, TValue> Union<TKey, TValue>(
        this Trie<TKey, TValue> left,
        Trie<TKey, TValue> right,
        Func<Trie<TKey, TValue>.Node, Trie<TKey, TValue>.Node, TValue> valueMerger
    ) where TKey : notnull
    {
        var c = new Trie<TKey, TValue>();
        c.Merge(left, valueMerger);
        c.Merge(right, valueMerger);
        return c;
    }
}
