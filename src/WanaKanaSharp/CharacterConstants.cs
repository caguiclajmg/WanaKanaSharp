//
// Constants.cs
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

namespace WanaKanaSharp;

static class CharacterConstants
{
    public const char LatinLowercaseStart = '\u0061';
    public const char LatinLowercaseEnd = '\u007A';
    public const char LatinUppercaseStart = '\u0041';
    public const char LatinUppercaseEnd = '\u005A';
    public const char ZenkakuLowercaseStart = '\uFF41';
    public const char ZenkakuLowercaseEnd = '\uFF5A';
    public const char ZenkakuUppercaseStart = '\uFF21';
    public const char ZenkakuUppercaseEnd = '\uFF3A';
    public const char HiraganaStart = '\u3041';
    public const char HiraganaEnd = '\u3096';
    public const char KatakanaStart = '\u30A1';
    public const char KatakanaEnd = '\u30FC';
    public const char KanjiStart = '\u4E00';
    public const char KanjiEnd = '\u9FAF';
    public const char ProlongedSoundMark = '\u30FC';
    public const char KanaSlashDot = '\u30FB';
    public const char Circumflex = '\u0302';
    public const char Macron = '\u0304';

    public static readonly (char Start, char End) ZenkakuNumbers = ('\uFF10', '\uFF19');
    public static readonly (char Start, char End) ZenkakuUppercase = (ZenkakuUppercaseStart, ZenkakuUppercaseEnd);
    public static readonly (char Start, char End) ZenkakuLowercase = (ZenkakuLowercaseStart, ZenkakuLowercaseEnd);
    public static readonly (char Start, char End) ZenkakuPunctuation1 = ('\uFF01', '\uFF0F');
    public static readonly (char Start, char End) ZenkakuPunctuation2 = ('\uFF1A', '\uFF1F');
    public static readonly (char Start, char End) ZenkakuPunctuation3 = ('\uFF3B', '\uFF3F');
    public static readonly (char Start, char End) ZenkakuPunctuation4 = ('\uFF5B', '\uFF60');
    public static readonly (char Start, char End) ZenkakuSymbolsCurrency = ('\uFFE0', '\uFFEE');

    public static readonly (char Start, char End) HiraganaCharacters = ('\u3040', '\u309F');
    public static readonly (char Start, char End) KatakanaCharacters = ('\u30A0', '\u30FF');
    public static readonly (char Start, char End) HankakuKatakana = ('\uFF66', '\uFF9F');
    public static readonly (char Start, char End) KatakanaPunctuation = ('\u30FB', '\u30FC');
    public static readonly (char Start, char End) KanaPunctuation = ('\uFF61', '\uFF65');
    public static readonly (char Start, char End) CJKSymbolsPunctuation = ('\u3000', '\u303F');
    public static readonly (char Start, char End) CommonCJK = ('\u4E00', '\u9FFF');
    public static readonly (char Start, char End) RareCJK = ('\u3400', '\u4DBF');

    public static readonly (char Start, char End)[] KanaRanges = [
        HiraganaCharacters,
        KatakanaCharacters,
        KanaPunctuation,
        HankakuKatakana
    ];

    public static readonly (char Start, char End)[] JapanesePunctuationRanges = [
        CJKSymbolsPunctuation,
        KanaPunctuation,
        KatakanaPunctuation,
        ZenkakuPunctuation1,
        ZenkakuPunctuation2,
        ZenkakuPunctuation3,
        ZenkakuPunctuation4,
        ZenkakuSymbolsCurrency
    ];

    public static readonly (char Start, char End)[] JapaneseRanges;

    public static readonly (char Start, char End) ModernEnglish = ('\u0000', '\u007F');

    public static readonly (char Start, char End)[] HepburnMacronRanges = [
        ('\u0100', '\u0101'),
        ('\u0112', '\u0113'),
        ('\u012A', '\u012B'),
        ('\u014C', '\u014D'),
        ('\u016A', '\u016B')
    ];

    public static readonly (char Start, char End)[] SmartQuoteRanges = [
        ('\u2018', '\u2019'),
        ('\u201C', '\u201D')
    ];

    public static readonly (char Start, char End)[] RomajiRanges;

    public static readonly (char Start, char End)[] EnglishPunctuationRanges;

    static CharacterConstants()
    {
        JapaneseRanges = [
            ZenkakuUppercase,
            ZenkakuLowercase,
            ZenkakuNumbers,
            CommonCJK,
            RareCJK
        ];
        JapaneseRanges = [.. JapaneseRanges, .. KanaRanges];
        JapaneseRanges = [.. JapaneseRanges, .. JapanesePunctuationRanges];

        RomajiRanges =
        [
            ModernEnglish
        ];
        RomajiRanges = [.. RomajiRanges, .. HepburnMacronRanges];

        EnglishPunctuationRanges =
        [
            ('\u0020', '\u002F'),
            ('\u003A', '\u003F'),
            ('\u005B', '\u0060'),
            ('\u007B', '\u007E')
        ];
        EnglishPunctuationRanges = [.. EnglishPunctuationRanges, .. SmartQuoteRanges];
    }
}
