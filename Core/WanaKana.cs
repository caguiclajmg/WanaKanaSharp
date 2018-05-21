//
// Hiragana.cs
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
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace WanaKanaSharp
{
	public static class WanaKana
	{
		public enum TokenType
		{
			Unknown,
			Romaji,
			Hiragana,
			Katakana,
			Kanji,
			EnglishPunctuation,
			JapanesePunctuation
		}

		public enum RomanizationMethod
		{
			Hepburn,
			Nihon,
			Kunrei
		}

		public enum IMEMode
		{
			None,
			ToHiragana,
			ToKatakana
		}

		const Char LatinLowercaseStart = '\u0061';
		const Char LatinLowercaseEnd = '\u007A';
		const Char LatinUppercaseStart = '\u0041';
		const Char LatinUppercaseEnd = '\u005A';
		const Char ZenkakuLowercaseStart = '\uFF41';
		const Char ZenkakuLowercaseEnd = '\uFF5A';
		const Char ZenkakuUppercaseStart = '\uFF21';
		const Char ZenkakuUppercaseEnd = '\uFF3A';
		const Char HiraganaStart = '\u3041';
		const Char HiraganaEnd = '\u3096';
		const Char KatakanaStart = '\u30A1';
		const Char KatakanaEnd = '\u30FC';
		const Char KanjiStart = '\u4E00';
		const Char KanjiEnd = '\u9FAF';
		const Char ProlongedSoundMark = '\u30FC';
		const Char KanaSlashDot = '\u30FB';

		static Tuple<Char, Char> ZenkakuNumbers;
		static Tuple<Char, Char> ZenkakuUppercase;
		static Tuple<Char, Char> ZenkakuLowercase;
		static Tuple<Char, Char> ZenkakuPunctuation1;
		static Tuple<Char, Char> ZenkakuPunctuation2;
		static Tuple<Char, Char> ZenkakuPunctuation3;
		static Tuple<Char, Char> ZenkakuPunctuation4;
		static Tuple<Char, Char> ZenkakuSymbolsCurrency;

		static Tuple<Char, Char> HiraganaCharacters;
		static Tuple<Char, Char> KatakanaCharacters;
		static Tuple<Char, Char> HankakuKatakana;
		static Tuple<Char, Char> KatakanaPunctuation;
		static Tuple<Char, Char> KanaPunctuation;
		static Tuple<Char, Char> CJKSymbolsPunctuation;
		static Tuple<Char, Char> CommonCJK;
		static Tuple<Char, Char> RareCJK;

		static Tuple<Char, Char>[] KanaRanges;

		static Tuple<Char, Char>[] JapanesePunctuationRanges;

		static Tuple<Char, Char>[] JapaneseRanges;

		static Tuple<Char, Char> ModernEnglish;

		static Tuple<Char, Char>[] HepburnMacronRanges;

		static Tuple<Char, Char>[] SmartQuoteRanges;

		static Tuple<Char, Char>[] RomajiRanges;

		static Tuple<Char, Char>[] EnglishPunctuationRanges;

		static WanaKana()
		{
			ZenkakuNumbers = Tuple.Create('\uFF10', '\uFF19');
			ZenkakuUppercase = Tuple.Create(ZenkakuUppercaseStart, ZenkakuUppercaseEnd);
			ZenkakuLowercase = Tuple.Create(ZenkakuLowercaseStart, ZenkakuLowercaseEnd);
			ZenkakuPunctuation1 = Tuple.Create('\uFF01', '\uFF0F');
			ZenkakuPunctuation2 = Tuple.Create('\uFF1A', '\uFF1F');
			ZenkakuPunctuation3 = Tuple.Create('\uFF3B', '\uFF3F');
			ZenkakuPunctuation4 = Tuple.Create('\uFF5B', '\uFF60');
			ZenkakuSymbolsCurrency = Tuple.Create('\uFFE0', '\uFFEE');

			HiraganaCharacters = Tuple.Create('\u3040', '\u309F');
			KatakanaCharacters = Tuple.Create('\u30A0', '\u30FF');
			HankakuKatakana = Tuple.Create('\uFF66', '\uFF9F');
			KatakanaPunctuation = Tuple.Create('\u30FB', '\u30FC');
			KanaPunctuation = Tuple.Create('\uFF61', '\uFF65');
			CJKSymbolsPunctuation = Tuple.Create('\u3000', '\u303F');
			CommonCJK = Tuple.Create('\u4E00', '\u9FFF');
			RareCJK = Tuple.Create('\u3400', '\u4DBF');

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

			ModernEnglish = Tuple.Create('\u0000', '\u007F');

			HepburnMacronRanges = new[]
			{
				Tuple.Create('\u0100', '\u0101'),
				Tuple.Create('\u0112', '\u0113'),
				Tuple.Create('\u012A', '\u012B'),
				Tuple.Create('\u014C', '\u014D'),
				Tuple.Create('\u016A', '\u016B'),
			};

			SmartQuoteRanges = new[]
			{
				Tuple.Create('\u2018', '\u2019'),
				Tuple.Create('\u201C', '\u201D')
			};

			RomajiRanges = new[]
			{
				ModernEnglish
			};
			RomajiRanges = RomajiRanges.Concat(HepburnMacronRanges).ToArray();

			EnglishPunctuationRanges = new[]
			{
				Tuple.Create('\u0020', '\u002F'),
				Tuple.Create('\u003A', '\u003F'),
				Tuple.Create('\u005B', '\u0060'),
				Tuple.Create('\u007B', '\u007E')
			};
			EnglishPunctuationRanges = EnglishPunctuationRanges.Concat(SmartQuoteRanges).ToArray();
		}

		public static Boolean IsRomaji(Char input, Regex allowed = null) => IsCharInRange(input, RomajiRanges) || IsMatch(input, allowed);
		public static Boolean IsRomaji(String input, Regex allowed = null) => !String.IsNullOrEmpty(input) && input.All((c) => IsRomaji(c, allowed));

		public static Boolean IsHiragana(Char input) => (input == ProlongedSoundMark) || IsCharInRange(input, HiraganaStart, HiraganaEnd);
		public static Boolean IsHiragana(String input) => !String.IsNullOrEmpty(input) && input.All(IsHiragana);

		public static Boolean IsKatakana(Char input) => IsCharInRange(input, KatakanaStart, KatakanaEnd);

		public static Boolean IsKatakana(String input) => !String.IsNullOrEmpty(input) && input.All(IsKatakana);

		public static Boolean IsKana(Char input) => IsHiragana(input) || IsKatakana(input);
		public static Boolean IsKana(String input) => !String.IsNullOrEmpty(input) && input.All(IsKana);

		public static Boolean IsKanji(Char input) => IsCharInRange(input, KanjiStart, KanjiEnd);
		public static Boolean IsKanji(String input) => !String.IsNullOrEmpty(input) && input.All(IsKanji);

		public static Boolean IsJapanese(Char input, Regex allowed = null) => IsCharInRange(input, JapaneseRanges) || IsMatch(input, allowed);
		public static Boolean IsJapanese(String input, Regex allowed = null) => !String.IsNullOrEmpty(input) && input.All((c) => IsJapanese(c, allowed));

		public static Boolean IsMixed(String input, Boolean passKanji = true) => (HasHiragana(input) || HasKatakana(input)) && HasRomaji(input) && (passKanji || !HasKanji(input));

		public static Boolean IsEnglishPunctuation(Char input) => IsCharInRange(input, EnglishPunctuationRanges);

		public static Boolean IsJapanesePunctuation(Char input) => IsCharInRange(input, JapanesePunctuationRanges);

		public static String ToRomaji(String input, Boolean upcaseKatakana = false, Object customRomajiMapping = null)
		{
			throw new NotImplementedException("WanaKana.ToRomaji(String, Boolean, Object) not yet implemented!");
		}

		public static String ToHiragana(String input, Boolean passRomaji = false, Boolean useObsoleteKana = false)
		{
			throw new NotImplementedException("WanaKana.ToHiragana(String, Boolean, Boolean) not yet implemented!");
		}

		public static String ToKatakana(String input, Boolean passRomaji = false, Boolean useObsoleteKana = false)
		{
			throw new NotImplementedException("WanaKana.ToKatakana(String, Boolean, Boolean) not yet implemented!");
		}

		public static String ToKana(String input, Boolean useObsoleteKana = false, Object customKanaMapping = null)
		{
			throw new NotImplementedException("WanaKana.ToKana(String, Boolean, Object) not yet implemented!");
		}

		public static String[] Tokenize(String input, Boolean compact = false, Boolean detailed = false)
		{
			if (String.IsNullOrEmpty(input)) return null;

			var tokens = new List<String>();

			Int32 position = 0;
			do
			{
				Int32 start = position;
				TokenType type = GetType(input[position]);

				do
				{
					position++;
					if (!(position < input.Length)) break;
				} while (GetType(input[position]) == type);

				tokens.Add(input.Substring(start, position - start));
				Console.WriteLine(input.Substring(start, position - start));
			} while (position < input.Length);

			return tokens.ToArray();
		}

		public static String StripOkurigana(String input, Boolean leading = false, String matchKanji = "")
		{
			if (!IsJapanese(input) ||
				(leading && !IsKana(input[0])) ||
				(!leading && !IsKana(input[input.Length - 1])) ||
				((!String.IsNullOrEmpty(matchKanji) && !HasKanji(matchKanji)) || (String.IsNullOrEmpty(matchKanji) && IsKana(input))))
			{
				return input;
			}

			String[] tokens = Tokenize(String.IsNullOrEmpty(matchKanji) ? input : matchKanji);
			Regex regex = new Regex(leading ? ('^' + tokens[0]) : (tokens[tokens.Length - 1] + '$'));
			input = regex.Replace(input, "");

			return input;
		}

		public static void Bind()
		{
			throw new NotImplementedException("WanaKana.Bind() not yet implemented!");
		}

		public static void Unbind()
		{
			throw new NotImplementedException("WanaKana.Unbind() not yet implemented!");
		}

		static TokenType GetType(Char input)
		{
			if (IsRomaji(input)) return TokenType.Romaji;
			if (IsHiragana(input)) return TokenType.Hiragana;
			if (IsKatakana(input)) return TokenType.Katakana;
			if (IsKanji(input)) return TokenType.Kanji;
			if (IsEnglishPunctuation(input)) return TokenType.EnglishPunctuation;
			if (IsJapanesePunctuation(input)) return TokenType.JapanesePunctuation;

			return TokenType.Unknown;
		}

		static Boolean HasRomaji(String input, Regex allowed = null) => !String.IsNullOrEmpty(input) && input.Any((c) => IsRomaji(c, allowed));
		static Boolean HasHiragana(String input) => !String.IsNullOrEmpty(input) && input.Any(IsHiragana);
		static Boolean HasKatakana(String input) => !String.IsNullOrEmpty(input) && input.Any(IsKatakana);
		static Boolean HasKana(String input) => !String.IsNullOrEmpty(input) && input.Any(IsKana);
		static Boolean HasKanji(String input) => !String.IsNullOrEmpty(input) && input.Any(IsKanji);
		static Boolean HasJapanese(String input, Regex allowed = null) => !String.IsNullOrEmpty(input) && input.Any((c) => IsJapanese(c, allowed));

		static Boolean IsCharInRange(Char input, Char start, Char end) => (input >= start) && (input <= end);
		static Boolean IsCharInRange(Char input, Tuple<Char, Char> range) => IsCharInRange(input, range.Item1, range.Item2);
		static Boolean IsCharInRange(Char input, params Tuple<Char, Char>[] ranges) => ranges.Any((r) => IsCharInRange(input, r));

		static Boolean IsMatch(Char input, Regex allowed) => (allowed != null) && (allowed.IsMatch(input.ToString()));
	}
}
