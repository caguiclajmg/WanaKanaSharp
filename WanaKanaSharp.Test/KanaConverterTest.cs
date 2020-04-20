using System;
using System.Collections.Generic;
using System.Text;

using NUnit.Framework;

using WanaKanaSharp.Utility;

namespace WanaKanaSharp.Test {
    [TestFixture()]
    class KanaConverterTest {
        [TestCase(null, ExpectedResult = "")]
        [TestCase("", ExpectedResult = "")]
        [TestCase("n", ExpectedResult = "ん")]
        [TestCase("onn", ExpectedResult = "おんん")]
        [TestCase("onna", ExpectedResult = "おんな")]
        [TestCase("nnn", ExpectedResult = "んんん")]
        [TestCase("onnna", ExpectedResult = "おんんな")]
        [TestCase("nnnn", ExpectedResult = "んんんん")]
        [TestCase("nyan", ExpectedResult = "にゃん")]
        [TestCase("nnyann", ExpectedResult = "んにゃんん")]
        [TestCase("nnnyannn", ExpectedResult = "んんにゃんんん")]
        [TestCase("n'ya", ExpectedResult = "んや")]
        [TestCase("kin'ya", ExpectedResult = "きんや")]
        [TestCase("shin'ya", ExpectedResult = "しんや")]
        [TestCase("kinyou", ExpectedResult = "きにょう")]
        [TestCase("kin'you", ExpectedResult = "きんよう")]
        [TestCase("kin'yu", ExpectedResult = "きんゆ")]
        [TestCase("ichiban warui", ExpectedResult = "いちばん わるい")]
        public string Convert(string input, bool useObsoleteKana = false, Trie<char, string> customKanaMapping = null) => WanaKana.ToKana(input, useObsoleteKana, customKanaMapping);
    }
}
