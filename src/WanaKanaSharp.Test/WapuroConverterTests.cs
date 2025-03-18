//
// HepburnConverterTest.cs
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

using NUnit.Framework;
using Shouldly;
using WanaKanaSharp.Converters;
using WanaKanaSharp.Utility;

namespace WanaKanaSharp.Test;

[TestFixture]
public class WapuroConverterTests
{
    private readonly WapuroConverter _converter = new();

    [TestCase(null, ExpectedResult = "")]
    [TestCase("", ExpectedResult = "")]
    [TestCase("ワニカニ　ガ　スゴイ　ダ", ExpectedResult = "wanikani ga sugoi da")]
    [TestCase("わにかに　が　すごい　だ", ExpectedResult = "wanikani ga sugoi da")]
    [TestCase("ワニカニ　が　すごい　だ", ExpectedResult = "wanikani ga sugoi da")]
    // TODO: Add Japanese punctuation -> English punctuation test case
    [TestCase("ワニカニ", true, ExpectedResult = "WANIKANI")]
    [TestCase("ワニカニ　が　すごい　だ", true, ExpectedResult = "WANIKANI ga sugoi da")]
    [TestCase("ばつげーむ", ExpectedResult = "batsuge-mu")]
    [TestCase("一抹げーむ", ExpectedResult = "一抹ge-mu")]
    [TestCase("スーパー", ExpectedResult = "su-pa-")]
    [TestCase("缶コーヒー", ExpectedResult = "缶ko-hi-")]
    // TODO: Add missing test case
    [TestCase("きんにくまん", ExpectedResult = "kinnikuman")]
    [TestCase("んんにんにんにゃんやん", ExpectedResult = "nnninninnyan'yan")]
    [TestCase("かっぱ　たった　しゅっしゅ ちゃっちゃ　やっつ", ExpectedResult = "kappa tatta shusshu chatcha yattsu")]
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
    [TestCase("ふフ", ExpectedResult = "fufu")]
    [TestCase("ふとん", ExpectedResult = "futon")]
    [TestCase("フリー", ExpectedResult = "furi-")]
    [TestCase("し", ExpectedResult = "shi")]
    [TestCase("しゅ", ExpectedResult = "shu")]
    [TestCase("ち", ExpectedResult = "chi")]
    [TestCase("ふ", ExpectedResult = "fu")]
    [TestCase("じゃ", ExpectedResult = "ja")]
    public string ToRomaji(string input, bool upcaseKatakana = false) => _converter.ToRomaji(input, upcaseKatakana);

    [Test]
    public void ToRomajiWithCustomMapping()
    {
        var customMapping = new InMemoryTrie<char, string>();
        var root = customMapping.Root;
        root.Insert(('い', "i"));
        root['い'].Insert(('ぬ', "dog"));

        _converter.ToRomaji("いぬ").ShouldBe("inu");
        _converter.ToRomaji("いぬ", customRomajiMapping: customMapping).ShouldBe("dog");
    }

    [TestCase("kyō", ExpectedResult = "きょう")]
    public string ToKana(string input, bool useObsoleteKana = false) => _converter.ToKana(input, useObsoleteKana: useObsoleteKana);
}
