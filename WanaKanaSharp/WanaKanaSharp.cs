//
// WanaKana.cs
//
// Author:
//       John Mark Gabriel Caguicla <jmg.caguicla@yozuru.jp>
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
        public static bool IsRomaji(char input, Regex allowed = null)
        {
            return IsCharInRange(input, CharacterConstants.RomajiRanges) || IsMatch(input, allowed);
        }

        /// <summary>
        /// Indicates whether the supplied input consists exclusively of Romaji characters.
        /// </summary>
        /// <returns><c>true</c>, if the string consists exclusively of Romaji characters, <c>false</c> otherwise.</returns>
        /// <param name="input">The string to test.</param>
        /// <param name="allowed">Additional allowed characters.</param>
        public static bool IsRomaji(string input, Regex allowed = null)
        {
            return !string.IsNullOrEmpty(input) && input.All((c) => IsRomaji(c, allowed));
        }

        /// <summary>
        /// Indicates whether the supplied input is a Hiragana character or not.
        /// </summary>
        /// <returns><c>true</c>, if the input is a Hiragana character, <c>false</c> otherwise.</returns>
        /// <param name="input">The character to test.</param>
        public static bool IsHiragana(char input)
        {
            return (input == CharacterConstants.ProlongedSoundMark) || IsCharInRange(input, CharacterConstants.HiraganaStart, CharacterConstants.HiraganaEnd);
        }

        /// <summary>
        /// Indicates whether the supplied input consists exclusively of Hiragana characters.
        /// </summary>
        /// <returns><c>true</c>, if the string consists exclusively of Hiragana characters, <c>false</c> otherwise.</returns>
        /// <param name="input">Input.</param>
        public static bool IsHiragana(string input)
        {
            return !string.IsNullOrEmpty(input) && input.All(IsHiragana);
        }

        /// <summary>
        /// Indicates whether the supplied input is a Katakana character or not.
        /// </summary>
        /// <returns><c>true</c>, if the input is a Katakana character, <c>false</c> otherwise.</returns>
        /// <param name="input">The character to test.</param>
        public static bool IsKatakana(char input)
        {
            return IsCharInRange(input, CharacterConstants.KatakanaStart, CharacterConstants.KatakanaEnd);
        }

        /// <summary>
        /// Indicates whether the supplied input consists exclusively of Katakana characters.
        /// </summary>
        /// <returns><c>true</c>, if the string consists exclusively of Katakana characters, <c>false</c> otherwise.</returns>
        /// <param name="input">Input.</param>
        public static bool IsKatakana(string input)
        {
            return !string.IsNullOrEmpty(input) && input.All(IsKatakana);
        }

        /// <summary>
        /// Indicates whether the supplied input is a Kana character or not.
        /// </summary>
        /// <returns><c>true</c>, if the input is a Kana character, <c>false</c> otherwise.</returns>
        /// <param name="input">The character to test.</param>
        public static bool IsKana(char input)
        {
            return IsHiragana(input) || IsKatakana(input);
        }

        /// <summary>
        /// Indicates whether the supplied input consists exclusively of Kana characters.
        /// </summary>
        /// <returns><c>true</c>, if the string consists exclusively of Kana characters, <c>false</c> otherwise.</returns>
        /// <param name="input">Input.</param>
        public static bool IsKana(string input)
        {
            return !string.IsNullOrEmpty(input) && input.All(IsKana);
        }

        /// <summary>
        /// Indicates whether the supplied input is a Kanji character or not.
        /// </summary>
        /// <returns><c>true</c>, if the input is a Kanji character, <c>false</c> otherwise.</returns>
        /// <param name="input">The character to test.</param>
        public static bool IsKanji(char input)
        {
            return IsCharInRange(input, CharacterConstants.KanjiStart, CharacterConstants.KanjiEnd);
        }

        /// <summary>
        /// Indicates whether the supplied input consists exclusively of Kanji characters.
        /// </summary>
        /// <returns><c>true</c>, if the string consists exclusively of Kanji characters, <c>false</c> otherwise.</returns>
        /// <param name="input">Input.</param>
        public static bool IsKanji(string input)
        {
            return !string.IsNullOrEmpty(input) && input.All(IsKanji);
        }

        /// <summary>
        /// Indicates whether the supplied input is a Japanese character or not.
        /// </summary>
        /// <returns><c>true</c>, if the input is a Japanese character, <c>false</c> otherwise.</returns>
        /// <param name="input">The character to test.</param>
        /// <param name="allowed">Additional allowed characters.</param>
        public static bool IsJapanese(char input, Regex allowed = null)
        {
            return IsCharInRange(input, CharacterConstants.JapaneseRanges) || IsMatch(input, allowed);
        }

        /// <summary>
        /// Indicates whether the supplied input consists exclusively of Japanese characters.
        /// </summary>
        /// <returns><c>true</c>, if the string consists exclusively of Japanese characters, <c>false</c> otherwise.</returns>
        /// <param name="input">The string to test.</param>
        /// <param name="allowed">Additional allowed characters.</param>
        public static bool IsJapanese(string input, Regex allowed = null)
        {
            return !string.IsNullOrEmpty(input) && input.All((c) => IsJapanese(c, allowed));
        }

        public static bool IsMixed(string input, bool passKanji = true)
        {
            return (HasHiragana(input) || HasKatakana(input)) && HasRomaji(input) && (passKanji || !HasKanji(input));
        }

        /// <summary>
        /// Indicates whether the supplied input is a English punctuation or not.
        /// </summary>
        /// <returns><c>true</c>, if the input is an English punctuation, <c>false</c> otherwise.</returns>
        /// <param name="input">The character to test.</param>
        public static bool IsEnglishPunctuation(char input)
        {
            return IsCharInRange(input, CharacterConstants.EnglishPunctuationRanges);
        }

        /// <summary>
        /// Indicates whether the supplied input is a Japanese punctuation or not.
        /// </summary>
        /// <returns><c>true</c>, if the input is an Japanese punctuation, <c>false</c> otherwise.</returns>
        /// <param name="input">The character to test.</param>
        public static bool IsJapanesePunctuation(char input)
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
        public static string ToRomaji(string input, bool upcaseKatakana = false, Trie<char, string> customRomajiMapping = null)
        {
            return RomajiConverter.ToRomaji(input, upcaseKatakana, customRomajiMapping);
        }

        public static string ToHiragana(string input, bool passRomaji = false, bool useObsoleteKana = false)
        {
            throw new NotImplementedException("WanaKana.ToHiragana(string, bool, bool) not yet implemented!");
        }

        public static string ToKatakana(string input, bool passRomaji = false, bool useObsoleteKana = false)
        {
            throw new NotImplementedException("WanaKana.ToKatakana(string, bool, bool) not yet implemented!");
        }

        public static string ToKana(string input, bool useObsoleteKana = false, Trie<char, string> customKanaMapping = null)
        {
            return KanaConverter.ToKana(input, useObsoleteKana, customKanaMapping);
        }

        public static string[] Tokenize(string input, bool compact = false, bool detailed = false)
        {
            if (string.IsNullOrEmpty(input)) return null;

            var tokens = new List<string>();

            int position = 0;
            do
            {
                int start = position;
                TokenType type = GetTokenType(input[position]);

                do
                {
                    position++;
                    if (!(position < input.Length)) break;
                } while (GetTokenType(input[position]) == type);

                tokens.Add(input[start..position]);
            } while (position < input.Length);

            return [.. tokens];
        }

        /// <summary>
        /// Removes Okurigana from the input string.
        /// </summary>
        /// <returns>The input string stripped of Okurigana.</returns>
        /// <param name="input">The string to strip.</param>
        /// <param name="leading">If set to <c>true</c>, leading Okurigana is removed instead of trailing Okurigana.</param>
        /// <param name="matchKanji">Kanji match pattern.</param>
        public static string StripOkurigana(string input, bool leading = false, string matchKanji = "")
        {
            if (!IsJapanese(input) ||
                (leading && !IsKana(input[0])) ||
                (!leading && !IsKana(input[^1])) ||
                ((!string.IsNullOrEmpty(matchKanji) && !HasKanji(matchKanji)) || (string.IsNullOrEmpty(matchKanji) && IsKana(input))))
            {
                return input;
            }

            string[] tokens = Tokenize(string.IsNullOrEmpty(matchKanji) ? input : matchKanji);
            var regex = new Regex(leading ? ('^' + tokens[0]) : (tokens[^1] + '$'));
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

        static TokenType GetTokenType(char input)
        {
            if (IsRomaji(input)) return TokenType.Romaji;
            if (IsHiragana(input)) return TokenType.Hiragana;
            if (IsKatakana(input)) return TokenType.Katakana;
            if (IsKanji(input)) return TokenType.Kanji;
            if (IsEnglishPunctuation(input)) return TokenType.EnglishPunctuation;
            if (IsJapanesePunctuation(input)) return TokenType.JapanesePunctuation;

            return TokenType.Unknown;
        }

        static bool HasRomaji(string input, Regex allowed = null)
        {
            return !string.IsNullOrEmpty(input) && input.Any((c) => IsRomaji(c, allowed));
        }

        static bool HasHiragana(string input)
        {
            return !string.IsNullOrEmpty(input) && input.Any(IsHiragana);
        }

        static bool HasKatakana(string input)
        {
            return !string.IsNullOrEmpty(input) && input.Any(IsKatakana);
        }

        static bool HasKana(string input)
        {
            return !string.IsNullOrEmpty(input) && input.Any(IsKana);
        }

        static bool HasKanji(string input)
        {
            return !string.IsNullOrEmpty(input) && input.Any(IsKanji);
        }

        static bool HasJapanese(string input, Regex allowed = null)
        {
            return !string.IsNullOrEmpty(input) && input.Any((c) => IsJapanese(c, allowed));
        }

        static bool IsCharInRange(char input, char start, char end)
        {
            return (input >= start) && (input <= end);
        }

        static bool IsCharInRange(char input, (char Start, char End) range)
        {
            return IsCharInRange(input, range.Start, range.End);
        }

        static bool IsCharInRange(char input, params (char Start, char End)[] ranges)
        {
            return ranges.Any((r) => IsCharInRange(input, r));
        }

        static bool IsMatch(char input, Regex allowed)
        {
            return (allowed != null) && (allowed.IsMatch(input.ToString()));
        }
    }
}
