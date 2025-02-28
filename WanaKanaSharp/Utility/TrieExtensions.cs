using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace WanaKanaSharp.Utility;

public static class TrieExtensions
{
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
