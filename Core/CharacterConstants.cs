//
// Constants.cs
//
// Author:
//       John Mark Gabriel Caguicla <caguicla.jmg@hapticbunnystudios.com>
//
// Copyright (c) 2018 John Mark Gabriel Caguicla
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
using System.Linq;

namespace WanaKanaSharp
{
	public static class CharacterConstants
	{
		public const Char LatinLowercaseStart = '\u0061';
		public const Char LatinLowercaseEnd = '\u007A';
		public const Char LatinUppercaseStart = '\u0041';
		public const Char LatinUppercaseEnd = '\u005A';
		public const Char ZenkakuLowercaseStart = '\uFF41';
		public const Char ZenkakuLowercaseEnd = '\uFF5A';
		public const Char ZenkakuUppercaseStart = '\uFF21';
		public const Char ZenkakuUppercaseEnd = '\uFF3A';
		public const Char HiraganaStart = '\u3041';
		public const Char HiraganaEnd = '\u3096';
		public const Char KatakanaStart = '\u30A1';
		public const Char KatakanaEnd = '\u30FC';
		public const Char KanjiStart = '\u4E00';
		public const Char KanjiEnd = '\u9FAF';
		public const Char ProlongedSoundMark = '\u30FC';
		public const Char KanaSlashDot = '\u30FB';

		public static (Char Start, Char End) ZenkakuNumbers;
		public static (Char Start, Char End) ZenkakuUppercase;
		public static (Char Start, Char End) ZenkakuLowercase;
		public static (Char Start, Char End) ZenkakuPunctuation1;
		public static (Char Start, Char End) ZenkakuPunctuation2;
		public static (Char Start, Char End) ZenkakuPunctuation3;
		public static (Char Start, Char End) ZenkakuPunctuation4;
		public static (Char Start, Char End) ZenkakuSymbolsCurrency;

		public static (Char Start, Char End) HiraganaCharacters;
		public static (Char Start, Char End) KatakanaCharacters;
		public static (Char Start, Char End) HankakuKatakana;
		public static (Char Start, Char End) KatakanaPunctuation;
		public static (Char Start, Char End) KanaPunctuation;
		public static (Char Start, Char End) CJKSymbolsPunctuation;
		public static (Char Start, Char End) CommonCJK;
		public static (Char Start, Char End) RareCJK;

		public static (Char Start, Char End)[] KanaRanges;

		public static (Char Start, Char End)[] JapanesePunctuationRanges;

		public static (Char Start, Char End)[] JapaneseRanges;

		public static (Char Start, Char End) ModernEnglish;

		public static (Char Start, Char End)[] HepburnMacronRanges;

		public static (Char Start, Char End)[] SmartQuoteRanges;

		public static (Char Start, Char End)[] RomajiRanges;

		public static (Char Start, Char End)[] EnglishPunctuationRanges;

		static CharacterConstants()
		{
			ZenkakuNumbers = ('\uFF10', '\uFF19');
			ZenkakuUppercase = (ZenkakuUppercaseStart, ZenkakuUppercaseEnd);
			ZenkakuLowercase = (ZenkakuLowercaseStart, ZenkakuLowercaseEnd);
			ZenkakuPunctuation1 = ('\uFF01', '\uFF0F');
			ZenkakuPunctuation2 = ('\uFF1A', '\uFF1F');
			ZenkakuPunctuation3 = ('\uFF3B', '\uFF3F');
			ZenkakuPunctuation4 = ('\uFF5B', '\uFF60');
			ZenkakuSymbolsCurrency = ('\uFFE0', '\uFFEE');

			HiraganaCharacters = ('\u3040', '\u309F');
			KatakanaCharacters = ('\u30A0', '\u30FF');
			HankakuKatakana = ('\uFF66', '\uFF9F');
			KatakanaPunctuation = ('\u30FB', '\u30FC');
			KanaPunctuation = ('\uFF61', '\uFF65');
			CJKSymbolsPunctuation = ('\u3000', '\u303F');
			CommonCJK = ('\u4E00', '\u9FFF');
			RareCJK = ('\u3400', '\u4DBF');

			KanaRanges = new[] {
				HiraganaCharacters,
				KatakanaCharacters,
				KanaPunctuation,
				HankakuKatakana
			};

			JapanesePunctuationRanges = new[]
			{
				CJKSymbolsPunctuation,
				KanaPunctuation,
				KatakanaPunctuation,
				ZenkakuPunctuation1,
				ZenkakuPunctuation2,
				ZenkakuPunctuation3,
				ZenkakuPunctuation4,
				ZenkakuSymbolsCurrency
			};

			JapaneseRanges = new[]
			{
				ZenkakuUppercase,
				ZenkakuLowercase,
				ZenkakuNumbers,
				CommonCJK,
				RareCJK
			};
			JapaneseRanges = JapaneseRanges.Concat(KanaRanges).ToArray();
			JapaneseRanges = JapaneseRanges.Concat(JapanesePunctuationRanges).ToArray();

			ModernEnglish = ('\u0000', '\u007F');

			HepburnMacronRanges = new[]
			{
				('\u0100', '\u0101'),
				('\u0112', '\u0113'),
				('\u012A', '\u012B'),
				('\u014C', '\u014D'),
				('\u016A', '\u016B'),
			};

			SmartQuoteRanges = new[]
			{
				('\u2018', '\u2019'),
				('\u201C', '\u201D')
			};

			RomajiRanges = new[]
			{
				ModernEnglish
			};
			RomajiRanges = RomajiRanges.Concat(HepburnMacronRanges).ToArray();

			EnglishPunctuationRanges = new[]
			{
				('\u0020', '\u002F'),
				('\u003A', '\u003F'),
				('\u005B', '\u0060'),
				('\u007B', '\u007E')
			};
			EnglishPunctuationRanges = EnglishPunctuationRanges.Concat(SmartQuoteRanges).ToArray();
		}
	}
}
