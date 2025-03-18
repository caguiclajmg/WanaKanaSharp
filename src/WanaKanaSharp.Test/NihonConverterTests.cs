using NUnit.Framework;
using WanaKanaSharp.Converters;
using WanaKanaSharp.Utility;

namespace WanaKanaSharp.Test;

[TestFixture]
public class NihonConverterTests
{
    [TestCase(null, ExpectedResult = "")]
    [TestCase("", ExpectedResult = "")]
    [TestCase("ワニカニ　ガ　スゴイ　ダ", ExpectedResult = "wanikani ga sugoi da")]
    [TestCase("わにかに　が　すごい　だ", ExpectedResult = "wanikani ga sugoi da")]
    [TestCase("ワニカニ　が　すごい　だ", ExpectedResult = "wanikani ga sugoi da")]
    // TODO: Add Japanese punctuation -> English punctuation test case
    [TestCase("ワニカニ", true, ExpectedResult = "WANIKANI")]
    [TestCase("ワニカニ　が　すごい　だ", true, ExpectedResult = "WANIKANI ga sugoi da")]
    [TestCase("ばつげーむ", ExpectedResult = "batuge-mu")]
    [TestCase("一抹げーむ", ExpectedResult = "一抹ge-mu")]
    [TestCase("スーパー", ExpectedResult = "suupaa")]
    [TestCase("缶コーヒー", ExpectedResult = "缶koohii")]
    // TODO: Add missing test case
    [TestCase("きんにくまん", ExpectedResult = "kinnikuman")]
    [TestCase("んんにんにんにゃんやん", ExpectedResult = "nnninninnyan'yan")]
    [TestCase("かっぱ　たった　しゅっしゅ ちゃっちゃ　やっつ", ExpectedResult = "kappa tatta syussyu tyattya yattu")]
    [TestCase("っ", ExpectedResult = "")]
    [TestCase("ヶ", ExpectedResult = "ヶ")]
    [TestCase("ヵ", ExpectedResult = "ヵ")]
    [TestCase("ゃ", ExpectedResult = "ya")]
    [TestCase("ゅ", ExpectedResult = "yu")]
    [TestCase("ょ", ExpectedResult = "yo")]
    [TestCase("ぁ", ExpectedResult = "a")]
    [TestCase("ぃ", ExpectedResult = "i")]
    [TestCase("ぅ", ExpectedResult = "u")]
    [TestCase("ぇ", ExpectedResult = "e")]
    [TestCase("ぉ", ExpectedResult = "o")]
    [TestCase("おんよみ", ExpectedResult = "on'yomi")]
    [TestCase("んよ んあ んゆ", ExpectedResult = "n'yo n'a n'yu")]
    [TestCase("シンヨ", ExpectedResult = "shin'yo")]
    [TestCase("ふフ", ExpectedResult = "huhu")]
    [TestCase("ふとん", ExpectedResult = "huton")]
    [TestCase("フリー", ExpectedResult = "hurii")]
    [TestCase("し", ExpectedResult = "si")]
    [TestCase("しゅ", ExpectedResult = "syu")]
    [TestCase("ち", ExpectedResult = "ti")]
    [TestCase("ふ", ExpectedResult = "hu")]
    [TestCase("じゃ", ExpectedResult = "zya")]
    public string Convert(string input, bool upcaseKatakana = false, Trie<char, string> customRomajiMapping = null)
    {
        var converter = new NihonConverter();
        return converter.ToRomaji(input, upcaseKatakana, customRomajiMapping);
    }
}
