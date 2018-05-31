//
// Hiragana.cs
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
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

using WanaKanaSharp.Utility;

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

		/// <summary>
		/// Specifies the Romanization rules to use during conversion
		/// </summary>
		public enum RomanizationMethod
		{
			/// <summary>
			/// Hepburn Romanization
			/// </summary>
			Hepburn,

			/// <summary>
			/// Kunrei-shiki Romanization
			/// </summary>
			Kunrei,

			/// <summary>
			/// Nihon-shiki Romanization
			/// </summary>
			Nihon
		}

		public enum IMEMode
		{
			None,
			ToHiragana,
			ToKatakana
		}

		/// <summary>
		/// Indicates whether the supplied input is a Romaji character or not.
		/// </summary>
		/// <returns><c>true</c>, if the input is a Romaji character, <c>false</c> otherwise.</returns>
		/// <param name="input">The character to test.</param>
		/// <param name="allowed">Additional allowed characters.</param>
		public static Boolean IsRomaji(Char input, Regex allowed = null)
		{
			return IsCharInRange(input, CharacterConstants.RomajiRanges) || IsMatch(input, allowed);
		}

		/// <summary>
		/// Indicates whether the supplied input consists exclusively of Romaji characters.
		/// </summary>
		/// <returns><c>true</c>, if the string consists exclusively of Romaji characters, <c>false</c> otherwise.</returns>
		/// <param name="input">The string to test.</param>
		/// <param name="allowed">Additional allowed characters.</param>
		public static Boolean IsRomaji(String input, Regex allowed = null)
		{
			return !String.IsNullOrEmpty(input) && input.All((c) => IsRomaji(c, allowed));
		}

		/// <summary>
		/// Indicates whether the supplied input is a Hiragana character or not.
		/// </summary>
		/// <returns><c>true</c>, if the input is a Hiragana character, <c>false</c> otherwise.</returns>
		/// <param name="input">The character to test.</param>
		public static Boolean IsHiragana(Char input)
		{
			return (input == CharacterConstants.ProlongedSoundMark) || IsCharInRange(input, CharacterConstants.HiraganaStart, CharacterConstants.HiraganaEnd);
		}

		/// <summary>
		/// Indicates whether the supplied input consists exclusively of Hiragana characters.
		/// </summary>
		/// <returns><c>true</c>, if the string consists exclusively of Hiragana characters, <c>false</c> otherwise.</returns>
		/// <param name="input">Input.</param>
		public static Boolean IsHiragana(String input)
		{
			return !String.IsNullOrEmpty(input) && input.All(IsHiragana);
		}

		/// <summary>
		/// Indicates whether the supplied input is a Katakana character or not.
		/// </summary>
		/// <returns><c>true</c>, if the input is a Katakana character, <c>false</c> otherwise.</returns>
		/// <param name="input">The character to test.</param>
		public static Boolean IsKatakana(Char input)
		{
			return IsCharInRange(input, CharacterConstants.KatakanaStart, CharacterConstants.KatakanaEnd);
		}

		/// <summary>
		/// Indicates whether the supplied input consists exclusively of Katakana characters.
		/// </summary>
		/// <returns><c>true</c>, if the string consists exclusively of Katakana characters, <c>false</c> otherwise.</returns>
		/// <param name="input">Input.</param>
		public static Boolean IsKatakana(String input)
		{
			return !String.IsNullOrEmpty(input) && input.All(IsKatakana);
		}

		/// <summary>
		/// Indicates whether the supplied input is a Kana character or not.
		/// </summary>
		/// <returns><c>true</c>, if the input is a Kana character, <c>false</c> otherwise.</returns>
		/// <param name="input">The character to test.</param>
		public static Boolean IsKana(Char input)
		{
			return IsHiragana(input) || IsKatakana(input);
		}

		/// <summary>
		/// Indicates whether the supplied input consists exclusively of Kana characters.
		/// </summary>
		/// <returns><c>true</c>, if the string consists exclusively of Kana characters, <c>false</c> otherwise.</returns>
		/// <param name="input">Input.</param>
		public static Boolean IsKana(String input)
		{
			return !String.IsNullOrEmpty(input) && input.All(IsKana);
		}

		/// <summary>
		/// Indicates whether the supplied input is a Kanji character or not.
		/// </summary>
		/// <returns><c>true</c>, if the input is a Kanji character, <c>false</c> otherwise.</returns>
		/// <param name="input">The character to test.</param>
		public static Boolean IsKanji(Char input)
		{
			return IsCharInRange(input, CharacterConstants.KanjiStart, CharacterConstants.KanjiEnd);
		}

		/// <summary>
		/// Indicates whether the supplied input consists exclusively of Kanji characters.
		/// </summary>
		/// <returns><c>true</c>, if the string consists exclusively of Kanji characters, <c>false</c> otherwise.</returns>
		/// <param name="input">Input.</param>
		public static Boolean IsKanji(String input)
		{
			return !String.IsNullOrEmpty(input) && input.All(IsKanji);
		}

		/// <summary>
		/// Indicates whether the supplied input is a Japanese character or not.
		/// </summary>
		/// <returns><c>true</c>, if the input is a Japanese character, <c>false</c> otherwise.</returns>
		/// <param name="input">The character to test.</param>
		/// <param name="allowed">Additional allowed characters.</param>
		public static Boolean IsJapanese(Char input, Regex allowed = null)
		{
			return IsCharInRange(input, CharacterConstants.JapaneseRanges) || IsMatch(input, allowed);
		}

		/// <summary>
		/// Indicates whether the supplied input consists exclusively of Japanese characters.
		/// </summary>
		/// <returns><c>true</c>, if the string consists exclusively of Japanese characters, <c>false</c> otherwise.</returns>
		/// <param name="input">The string to test.</param>
		/// <param name="allowed">Additional allowed characters.</param>
		public static Boolean IsJapanese(String input, Regex allowed = null)
		{
			return !String.IsNullOrEmpty(input) && input.All((c) => IsJapanese(c, allowed));
		}

		public static Boolean IsMixed(String input, Boolean passKanji = true)
		{
			return (HasHiragana(input) || HasKatakana(input)) && HasRomaji(input) && (passKanji || !HasKanji(input));
		}

		/// <summary>
		/// Indicates whether the supplied input is a English punctuation or not.
		/// </summary>
		/// <returns><c>true</c>, if the input is an English punctuation, <c>false</c> otherwise.</returns>
		/// <param name="input">The character to test.</param>
		public static Boolean IsEnglishPunctuation(Char input)
		{
			return IsCharInRange(input, CharacterConstants.EnglishPunctuationRanges);
		}

		/// <summary>
		/// Indicates whether the supplied input is a Japanese punctuation or not.
		/// </summary>
		/// <returns><c>true</c>, if the input is an Japanese punctuation, <c>false</c> otherwise.</returns>
		/// <param name="input">The character to test.</param>
		public static Boolean IsJapanesePunctuation(Char input)
		{
			return IsCharInRange(input, CharacterConstants.JapanesePunctuationRanges);
		}

		/// <summary>
		/// Converts
		/// </summary>
		/// <returns>The Romaji equivalent of the input string.</returns>
		/// <param name="input">The string to convert.</param>
		/// <param name="upcaseKatakana">If set to <c>true</c>, Katakana characters are converted into uppercase Romaji characters.</param>
		/// <param name="customRomajiMapping">Custom Romaji mapping rules.</param>
		/// <param name="romanizationMethod">Romanization method to use.</param>
		public static String ToRomaji(String input, Boolean upcaseKatakana = false, Trie<Char, String> customRomajiMapping = null, RomanizationMethod romanizationMethod = RomanizationMethod.Hepburn)
		{
			switch (romanizationMethod)
			{
				case RomanizationMethod.Hepburn:
					return HepburnConverter.Convert(input, upcaseKatakana, customRomajiMapping);

				case RomanizationMethod.Kunrei:
					throw new NotImplementedException("Kunrei-shiki not yet implemented!");

				case RomanizationMethod.Nihon:
					throw new NotImplementedException("Nihon-shiki converter not yet implemented!");

				default:
					throw new ArgumentException("Unknown romanization method!");
			}
		}

		public static String ToHiragana(String input, Boolean passRomaji = false, Boolean useObsoleteKana = false)
		{
			throw new NotImplementedException("WanaKana.ToHiragana(String, Boolean, Boolean) not yet implemented!");
		}

		public static String ToKatakana(String input, Boolean passRomaji = false, Boolean useObsoleteKana = false)
		{
			throw new NotImplementedException("WanaKana.ToKatakana(String, Boolean, Boolean) not yet implemented!");
		}

		public static String ToKana(String input, Boolean useObsoleteKana = false, Trie<Char, String> customKanaMapping = null)
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

		/// <summary>
		/// Removes Okurigana from the input string.
		/// </summary>
		/// <returns>The input string stripped of Okurigana.</returns>
		/// <param name="input">The string to strip.</param>
		/// <param name="leading">If set to <c>true</c>, leading Okurigana is removed instead of trailing Okurigana.</param>
		/// <param name="matchKanji">Kanji match pattern.</param>
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

		static Boolean HasRomaji(String input, Regex allowed = null)
		{
			return !String.IsNullOrEmpty(input) && input.Any((c) => IsRomaji(c, allowed));
		}

		static Boolean HasHiragana(String input)
		{
			return !String.IsNullOrEmpty(input) && input.Any(IsHiragana);
		}

		static Boolean HasKatakana(String input)
		{
			return !String.IsNullOrEmpty(input) && input.Any(IsKatakana);
		}

		static Boolean HasKana(String input)
		{
			return !String.IsNullOrEmpty(input) && input.Any(IsKana);
		}

		static Boolean HasKanji(String input)
		{
			return !String.IsNullOrEmpty(input) && input.Any(IsKanji);
		}

		static Boolean HasJapanese(String input, Regex allowed = null)
		{
			return !String.IsNullOrEmpty(input) && input.Any((c) => IsJapanese(c, allowed));
		}

		static Boolean IsCharInRange(Char input, Char start, Char end)
		{
			return (input >= start) && (input <= end);
		}

		static Boolean IsCharInRange(Char input, (Char Start, Char End) range)
		{
			return IsCharInRange(input, range.Start, range.End);
		}

		static Boolean IsCharInRange(Char input, params (Char Start, Char End)[] ranges)
		{
			return ranges.Any((r) => IsCharInRange(input, r));
		}

		static Boolean IsMatch(Char input, Regex allowed)
		{
			return (allowed != null) && (allowed.IsMatch(input.ToString()));
		}
	}
}
