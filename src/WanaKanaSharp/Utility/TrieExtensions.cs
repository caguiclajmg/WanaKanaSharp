namespace WanaKanaSharp.Utility;

public static class TrieExtensions
{
    public static void Merge<TKey, TValue>(
        this Trie<TKey, TValue> source,
        Trie<TKey, TValue> other,
        ValueMergerDelegate<TKey, TValue> valueMerger,
        ChildrenMergerDelegate<TKey, TValue> childrenMerger
    ) where TKey : notnull => source.Root.Merge(other.Root, valueMerger, childrenMerger);

    public static void Merge<TKey, TValue>(
        this Trie<TKey, TValue> source, 
        Trie<TKey, TValue> other
    ) where TKey :notnull => source.Root.Merge(other.Root);
}
