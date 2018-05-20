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
using NUnit.Framework;

namespace WanaKanaSharp
{
	[TestFixture()]
	public class WanaKanaTest
	{
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
