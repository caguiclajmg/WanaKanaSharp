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
		enum TokenType
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

		public static Boolean IsRomaji(Char input, Regex allowed = null) => IsCharInRange(input, Constants.RomajiRanges) || IsMatch(input, allowed);
		public static Boolean IsRomaji(String input, Regex allowed = null) => !String.IsNullOrEmpty(input) && input.All((c) => IsRomaji(c, allowed));

		public static Boolean IsHiragana(Char input) => (input == Constants.ProlongedSoundMark) || IsCharInRange(input, Constants.HiraganaStart, Constants.HiraganaEnd);
		public static Boolean IsHiragana(String input) => !String.IsNullOrEmpty(input) && input.All(IsHiragana);

		public static Boolean IsKatakana(Char input) => IsCharInRange(input, Constants.KatakanaStart, Constants.KatakanaEnd);

		public static Boolean IsKatakana(String input) => !String.IsNullOrEmpty(input) && input.All(IsKatakana);

		public static Boolean IsKana(Char input) => IsHiragana(input) || IsKatakana(input);
		public static Boolean IsKana(String input) => !String.IsNullOrEmpty(input) && input.All(IsKana);

		public static Boolean IsKanji(Char input) => IsCharInRange(input, Constants.KanjiStart, Constants.KanjiEnd);
		public static Boolean IsKanji(String input) => !String.IsNullOrEmpty(input) && input.All(IsKanji);

		public static Boolean IsJapanese(Char input, Regex allowed = null) => IsCharInRange(input, Constants.JapaneseRanges) || IsMatch(input, allowed);
		public static Boolean IsJapanese(String input, Regex allowed = null) => !String.IsNullOrEmpty(input) && input.All((c) => IsJapanese(c, allowed));

		public static Boolean IsMixed(String input, Boolean passKanji = true) => (HasHiragana(input) || HasKatakana(input)) && HasRomaji(input) && (passKanji || !HasKanji(input));

		public static Boolean IsEnglishPunctuation(Char input) => IsCharInRange(input, Constants.EnglishPunctuationRanges);

		public static Boolean IsJapanesePunctuation(Char input) => IsCharInRange(input, Constants.JapanesePunctuationRanges);

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
				TokenType type = GetTokenType(input[position]);

				do
				{
					position++;
					if (!(position < input.Length)) break;
				} while (GetTokenType(input[position]) == type);

				tokens.Add(input.Substring(start, position - start));
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

		static TokenType GetTokenType(Char input)
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
		static Boolean IsCharInRange(Char input, (Char Start, Char End) range) => IsCharInRange(input, range.Start, range.End);
		static Boolean IsCharInRange(Char input, params (Char Start, Char End)[] ranges) => ranges.Any((r) => IsCharInRange(input, r));

		static Boolean IsMatch(Char input, Regex allowed) => (allowed != null) && (allowed.IsMatch(input.ToString()));
	}
}
