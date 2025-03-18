using System;

namespace WanaKanaSharp.Converters;

public class WapuroConverter : HepburnConverter, IRomajiConverter, IKanaConverter
{
    protected override string TranslateLongVowel(MyNode node)
    {
        var value = node.Value;
        var translated = value + "-";
        return translated;
    }

    public override string ToKana(string input, bool useObsoleteKana, MyTrie? customKanaMapping = null)
    {
        throw new NotImplementedException($"{nameof(ToKana)} is currently not implemented for {nameof(WapuroConverter)}.");
    }
}