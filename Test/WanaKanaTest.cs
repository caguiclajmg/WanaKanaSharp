//
// WanaKanaTest.cs
//
// Author:
//       John Mark Gabriel Caguicla <caguicla.jmg@hapticbunnystudios.com>
//
// Copyright (c) 2018 Copyright © 2018 John Mark Gabriel Caguicla
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
using System.Text.RegularExpressions;

using NUnit.Framework;

namespace WanaKanaSharp
{
	[TestFixture()]
	public class WanaKanaTest
	{
		[TestCase(null, ExpectedResult = false)]
		[TestCase("", ExpectedResult = false)]
		[TestCase("A", ExpectedResult = true)]
		[TestCase("xYz", ExpectedResult = true)]
		[TestCase("Tōkyō and Ōsaka", ExpectedResult = true)]
		[TestCase("あアA", ExpectedResult = false)]
		[TestCase("お願い", ExpectedResult = false)]
		[TestCase("熟成", ExpectedResult = false)]
		[TestCase("a*b&c-d", ExpectedResult = true)]
		[TestCase("0123456789", ExpectedResult = true)]
		[TestCase("a！b&cーd", ExpectedResult = false)]
		[TestCase("ｈｅｌｌｏ", ExpectedResult = false)]
		//[TestCase("a！b&cーd", new Regex("[！ー]") ExpectedResult = true)]
		public Boolean IsRomaji(String input, Regex allowed = null) => WanaKana.IsRomaji(input, allowed);

		[TestCase(null, ExpectedResult = false)]
		[TestCase("", ExpectedResult = false)]
		[TestCase("あ", ExpectedResult = true)]
		[TestCase("ああ", ExpectedResult = true)]
		[TestCase("ア", ExpectedResult = false)]
		[TestCase("A", ExpectedResult = false)]
		[TestCase("あア", ExpectedResult = false)]
		[TestCase("げーむ", ExpectedResult = true)]
		public Boolean IsHiragana(String input) => WanaKana.IsHiragana(input);

		[TestCase(null, ExpectedResult = false)]
		[TestCase("", ExpectedResult = false)]
		[TestCase("アア", ExpectedResult = true)]
		[TestCase("ア", ExpectedResult = true)]
		[TestCase("あ", ExpectedResult = false)]
		[TestCase("A", ExpectedResult = false)]
		[TestCase("あア", ExpectedResult = false)]
		[TestCase("ゲーム", ExpectedResult = true)]
		public Boolean IsKatkana(String input) => WanaKana.IsKatakana(input);

		[TestCase(null, ExpectedResult = false)]
		[TestCase("", ExpectedResult = false)]
		[TestCase("あ", ExpectedResult = true)]
		[TestCase("ア", ExpectedResult = true)]
		[TestCase("あア", ExpectedResult = true)]
		[TestCase("A", ExpectedResult = false)]
		[TestCase("あAア", ExpectedResult = false)]
		[TestCase("アーあ", ExpectedResult = true)]
		public Boolean IsKana(String input) => WanaKana.IsKana(input);

		[TestCase(null, ExpectedResult = false)]
		[TestCase("", ExpectedResult = false)]
		[TestCase("切腹", ExpectedResult = true)]
		[TestCase("刀", ExpectedResult = true)]
		[TestCase("🐸", ExpectedResult = false)]
		[TestCase("あ", ExpectedResult = false)]
		[TestCase("ア", ExpectedResult = false)]
		[TestCase("あア", ExpectedResult = false)]
		[TestCase("A", ExpectedResult = false)]
		[TestCase("あAア", ExpectedResult = false)]
		[TestCase("１２隻", ExpectedResult = false)]
		[TestCase("12隻", ExpectedResult = false)]
		[TestCase("隻。", ExpectedResult = false)]
		public Boolean IsKanji(String input) => WanaKana.IsKanji(input);

		[TestCase(null, ExpectedResult = false)]
		[TestCase("", ExpectedResult = false)]
		[TestCase("泣き虫", ExpectedResult = true)]
		[TestCase("あア", ExpectedResult = true)]
		[TestCase("A泣き虫", ExpectedResult = false)]
		[TestCase("A", ExpectedResult = false)]
		[TestCase("　", ExpectedResult = true)]
		[TestCase(" ", ExpectedResult = false)]
		[TestCase("泣き虫。＃！〜〈〉《》〔〕［］【】（）｛｝〝〟", ExpectedResult = true)]
		[TestCase("泣き虫.!~", ExpectedResult = false)]
		[TestCase("０１２３４５６７８９", ExpectedResult = true)]
		[TestCase("0123456789", ExpectedResult = false)]
		[TestCase("ＭｅＴｏｏ", ExpectedResult = true)]
		[TestCase("２０１１年", ExpectedResult = true)]
		[TestCase("ﾊﾝｶｸｶﾀｶﾅ", ExpectedResult = true)]
		[TestCase("＃ＭｅＴｏｏ、これを前に「ＫＵＲＯＳＨＩＯ」は、都内で報道陣を前に水中探査ロボットの最終点検の様子を公開しました。イルカのような形をした探査ロボットは、全長３メートル、重さは３５０キロあります。《はじめに》冒頭、安倍総理大臣は、ことしが明治元年から１５０年にあたることに触れ「明治という新しい時代が育てたあまたの人材が、技術優位の欧米諸国が迫る『国難』とも呼ぶべき危機の中で、わが国が急速に近代化を遂げる原動力となった。今また、日本は少子高齢化という『国難』とも呼ぶべき危機に直面している。もう１度、あらゆる日本人にチャンスを創ることで、少子高齢化も克服できる」と呼びかけました。《働き方改革》続いて安倍総理大臣は、具体的な政策課題の最初に「働き方改革」を取り上げ、「戦後の労働基準法制定以来、７０年ぶりの大改革だ。誰もが生きがいを感じて、その能力を思う存分発揮すれば少子高齢化も克服できる」と述べました。そして、同一労働同一賃金の実現や、時間外労働の上限規制の導入、それに労働時間でなく成果で評価するとして労働時間の規制から外す「高度プロフェッショナル制度」の創設などに取り組む考えを強調しました。", ExpectedResult = true)]
		//[TestCase("≪偽括弧≫", new Regex("[≪≫]") ExpectedResult = true)]
		public Boolean IsJapanese(String input, Regex allowed = null) => WanaKana.IsJapanese(input, allowed);

		[TestCase(null, ExpectedResult = null)]
		[TestCase("", ExpectedResult = "")]
		[TestCase("ふふフフ", ExpectedResult = "ふふフフ")]
		[TestCase("abc", ExpectedResult = "abc")]
		[TestCase("ふaふbフcフ", ExpectedResult = "ふaふbフcフ")]

		[TestCase("踏み込む", ExpectedResult = "踏み込")]
		[TestCase("使い方", ExpectedResult = "使い方")]
		[TestCase("申し申し", ExpectedResult = "申し申")]
		[TestCase("お腹", ExpectedResult = "お腹")]
		[TestCase("お祝い", ExpectedResult = "お祝")]

		[TestCase("踏み込む", true, ExpectedResult = "踏み込む")]
		[TestCase("お腹", true, ExpectedResult = "腹")]
		[TestCase("お祝い", true, ExpectedResult = "祝い")]

		[TestCase("おはら", false, "お腹", ExpectedResult = "おはら")]
		[TestCase("ふみこむ", false, "踏み込む", ExpectedResult = "ふみこ")]
		[TestCase("おみまい", true, "お祝い", ExpectedResult = "みまい")]
		[TestCase("おはら", true, "お腹", ExpectedResult = "はら")]
		public String StripOkurigana(String input, Boolean leading = false, String matchKanji = "") => WanaKana.StripOkurigana(input, leading, matchKanji);
	}
}
