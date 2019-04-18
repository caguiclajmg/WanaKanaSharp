//
// Constants.cs
//
// Author:
//       John Mark Gabriel Caguicla <jmg@caguicla.me>
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

    static class CharacterConstants
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

        public static (Char Start, Char End) ZenkakuNumbers = ('\uFF10', '\uFF19');
        public static (Char Start, Char End) ZenkakuUppercase = (ZenkakuUppercaseStart, ZenkakuUppercaseEnd);
        public static (Char Start, Char End) ZenkakuLowercase = (ZenkakuLowercaseStart, ZenkakuLowercaseEnd);
        public static (Char Start, Char End) ZenkakuPunctuation1 = ('\uFF01', '\uFF0F');
        public static (Char Start, Char End) ZenkakuPunctuation2 = ('\uFF1A', '\uFF1F');
        public static (Char Start, Char End) ZenkakuPunctuation3 = ('\uFF3B', '\uFF3F');
        public static (Char Start, Char End) ZenkakuPunctuation4 = ('\uFF5B', '\uFF60');
        public static (Char Start, Char End) ZenkakuSymbolsCurrency = ('\uFFE0', '\uFFEE');

        public static readonly (Char Start, Char End) HiraganaCharacters = ('\u3040', '\u309F');
        public static readonly (Char Start, Char End) KatakanaCharacters = ('\u30A0', '\u30FF');
        public static readonly (Char Start, Char End) HankakuKatakana = ('\uFF66', '\uFF9F');
        public static readonly (Char Start, Char End) KatakanaPunctuation = ('\u30FB', '\u30FC');
        public static readonly (Char Start, Char End) KanaPunctuation = ('\uFF61', '\uFF65');
        public static readonly (Char Start, Char End) CJKSymbolsPunctuation = ('\u3000', '\u303F');
        public static readonly (Char Start, Char End) CommonCJK = ('\u4E00', '\u9FFF');
        public static readonly (Char Start, Char End) RareCJK = ('\u3400', '\u4DBF');

        public static readonly (Char Start, Char End)[] KanaRanges = {
            HiraganaCharacters,
            KatakanaCharacters,
            KanaPunctuation,
            HankakuKatakana
        };

        public static readonly (Char Start, Char End)[] JapanesePunctuationRanges = {
            CJKSymbolsPunctuation,
            KanaPunctuation,
            KatakanaPunctuation,
            ZenkakuPunctuation1,
            ZenkakuPunctuation2,
            ZenkakuPunctuation3,
            ZenkakuPunctuation4,
            ZenkakuSymbolsCurrency
        };

        public static readonly (Char Start, Char End)[] JapaneseRanges;

        public static readonly (Char Start, Char End) ModernEnglish = ('\u0000', '\u007F');

        public static readonly (Char Start, Char End)[] HepburnMacronRanges = {
            ('\u0100', '\u0101'),
            ('\u0112', '\u0113'),
            ('\u012A', '\u012B'),
            ('\u014C', '\u014D'),
            ('\u016A', '\u016B')
        };

        public static readonly (Char Start, Char End)[] SmartQuoteRanges = {
            ('\u2018', '\u2019'),
            ('\u201C', '\u201D')
        };

        public static readonly (Char Start, Char End)[] RomajiRanges;

        public static readonly (Char Start, Char End)[] EnglishPunctuationRanges;

        static CharacterConstants()
        {
            JapaneseRanges = new[] {
                ZenkakuUppercase,
                ZenkakuLowercase,
                ZenkakuNumbers,
                CommonCJK,
                RareCJK
            };
            JapaneseRanges = JapaneseRanges.Concat(KanaRanges).ToArray();
            JapaneseRanges = JapaneseRanges.Concat(JapanesePunctuationRanges).ToArray();

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
