//
// DefaultKanaConverter.cs
//
// Author:
//       John Mark Gabriel Caguicla <jmg.caguicla@guarandoo.me>
//
// Copyright (c) 2023 John Mark Gabriel Caguicla
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
using System.Text;

using WanaKanaSharp.Utility;

namespace WanaKanaSharp.Kana
{
    public partial class DefaultKanaConverter : KanaConverter
    {
        public override string ToKana(string input, Trie<char, string> customKanaMapping)
        {
            if (string.IsNullOrEmpty(input)) return "";

            var builder = new StringBuilder();

            int position = 0;
            do
            {
                var converted = Convert(HiraganaTree, input, position);
                if(input.Substring(converted.Start, converted.End - converted.Start).All(char.IsUpper))
                {
                    converted = Convert(KatakanaTree, input, position);
                }
                builder.Append(converted.Token);
                position = converted.End;
            } while (position < input.Length);

            return builder.ToString();
        }

        private (string Token, int Start, int End) Convert(Trie<char, string> trie, string input, int startPosition)
        {
            var curNode = trie.Root;
            var curIdx = startPosition;
            var curChar = input[curIdx];

            if (!curNode.TryGetValue(char.ToLower(curChar), out var next))
            {
                return (Token: curChar.ToString(), Start: startPosition, End: curIdx + 1);
            }

            while (true)
            {
                curNode = next;
                curIdx++;

                if (curIdx == input.Length) break;

                curChar = input[curIdx];
                if (!curNode.TryGetValue(char.ToLower(curChar), out next)) break;
            }

            return (Token: curNode.Value, Start: startPosition, End: curIdx);
        }
    }

    partial class DefaultKanaConverter
    {
        private readonly static Trie<char, string> HiraganaTree = BuildHiraganaTree();
        private readonly static Trie<char, string> KatakanaTree = BuildKatakanaTree();

        private static Trie<char, string> BuildHiraganaTree()
        {
            var trie = new Trie<char, string>();
            var root = trie.Root;

            root.Add(('a', "あ"), ('i', "い"), ('u', "う"), ('e', "え"), ('o', "お"));
            root.Add(
                ('k', ""), ('s', ""), ('t', ""),
                ('n', "ん"), ('h', ""), ('m', ""),
                ('y', ""), ('r', ""), ('w', ""),
                ('g', ""), ('z', ""), ('d', ""),
                ('b', ""), ('p', ""), ('v', "")
            );
            root['k'].Add(('a', "か"), ('i', "き"), ('u', "く"), ('e', "け"), ('o', "こ"));
            root['s'].Add(('a', "さ"), ('i', "し"), ('u', "す"), ('e', "せ"), ('o', "そ"));
            root['s'].Add(("ha", "しゃ"), ("hi", "し"), ("hu", "しゅ"), ("ho", "しょ"));
            root['t'].Add(('a', "た"), ('i', "ち"), ('u', "つ"), ('e', "て"), ('o', "と"));
            root['n'].Add(('a', "な"), ('i', "に"), ('u', "ぬ"), ('e', "ね"), ('o', "の"));
            root['h'].Add(('a', "は"), ('i', "ひ"), ('u', "ふ"), ('e', "へ"), ('o', "ほ"));
            root['m'].Add(('a', "ま"), ('i', "み"), ('u', "む"), ('e', "め"), ('o', "も"));
            root['y'].Add(('a', "や"), ('u', "ゆ"), ('o', "よ"));
            root['r'].Add(('a', "ら"), ('i', "り"), ('u', "る"), ('e', "れ"), ('o', "ろ"));
            root['w'].Add(('a', "わ"), ('i', "ゐ"), ('e', "ゑ"), ('o', "を"));
            root['g'].Add(('a', "が"), ('i', "ぎ"), ('u', "ぐ"), ('e', "げ"), ('o', "ご"));
            root['z'].Add(('a', "ざ"), ('i', "じ"), ('u', "ず"), ('e', "ぜ"), ('o', "ぞ"));
            root['d'].Add(('a', "だ"), ('i', "ぢ"), ('u', "づ"), ('e', "で"), ('o', "ど"));
            root['b'].Add(('a', "ば"), ('i', "び"), ('u', "ぶ"), ('e', "べ"), ('o', "ぼ"));
            root['p'].Add(('a', "ぱ"), ('i', "ぴ"), ('u', "ぷ"), ('e', "ぺ"), ('o', "ぽ"));
            root['v'].Add(('a', "ゔぁ"), ('i', "ゔぃ"), ('u', "ゔ"), ('e', "ゔぇ"), ('o', "ゔぉ"));
            root.Add("chi", root["ti"]);

            {
                var consonants = new[] {
                    ('k', 'き'), ('s', 'し'), ('t', 'ち'),
                    ('n', 'に'), ('h', 'ひ'), ('m', 'み'),
                    ('r', 'り'), ('g', 'ぎ'), ('z', 'じ'),
                    ('d', 'ぢ'), ('b', 'び'), ('p', 'ぴ'),
                    ('v', 'ゔ'), ('q', 'く'), ('f', 'ふ')
                };
                var smallY = new[] {
                    ("ya", 'ゃ'),
                    ("yi", 'ぃ'),
                    ("yu", 'ゅ'),
                    ("ye", 'ぇ'),
                    ("yo", 'ょ')
                };

                foreach (var a in consonants)
                {
                    foreach (var b in smallY)
                    {
                        root.Add($"{a.Item1}{b.Item1}", $"{a.Item2}{b.Item2}");
                    }
                }
            }

            root.Add(
                ('.', "。"), (',', "、"), (':', "："),
                ('/', "・"), ('!', "！"), ('?', "？"),
                ('~', "〜"), ('-', "ー"), ('‘', "「"),
                ('’', "」"), ('“', "『"), ('”', "』"),
                ('[', "［"), (']', "］"), ('(', "（"),
                (')', "）"), ('{', "｛"), ('}', "｝")
            );

            root.Add("xn", root['n']);
            root.Add("n'", "ん");

            return trie;
        }

        private static Trie<char, string> BuildKatakanaTree()
        {
            var trie = new Trie<char, string>();
            var root = trie.Root;

            root.Add(('a', "ア"), ('i', "イ"), ('u', "ウ"), ('e', "エ"), ('o', "オ"));
            root.Add(
                ('k', ""), ('s', ""), ('t', ""),
                ('n', "ん"), ('h', ""), ('m', ""),
                ('y', ""), ('r', ""), ('w', ""),
                ('g', ""), ('z', ""), ('d', ""),
                ('b', ""), ('p', ""), ('v', "")
            );
            root['k'].Add(('a', "カ"), ('i', "キ"), ('u', "ク"), ('e', "ケ"), ('o', "コ"));
            root['s'].Add(('a', "サ"), ('i', "シ"), ('u', "ス"), ('e', "セ"), ('o', "ソ"));
            root['s'].Add(("ha", "シャ"), ("hi", "シ"), ("hu", "シュ"), ("ho", "ショ"));
            root['t'].Add(('a', "タ"), ('i', "チ"), ('u', "ツ"), ('e', "テ"), ('o', "ト"));
            root['n'].Add(('a', "ナ"), ('i', "ニ"), ('u', "ヌ"), ('e', "ネ"), ('o', "ノ"));
            root['h'].Add(('a', "ハ"), ('i', "ヒ"), ('u', "フ"), ('e', "ヘ"), ('o', "ホ"));
            root['m'].Add(('a', "マ"), ('i', "ミ"), ('u', "ム"), ('e', "メ"), ('o', "モ"));
            root['y'].Add(('a', "ヤ"), ('u', "ユ"), ('o', "ヨ"));
            root['r'].Add(('a', "ラ"), ('i', "リ"), ('u', "ル"), ('e', "レ"), ('o', "ロ"));
            root['w'].Add(('a', "ワ"), ('i', "ウィ"), ('e', "ウェ"), ('o', "ヲ"));
            root['g'].Add(('a', "ガ"), ('i', "ギ"), ('u', "グ"), ('e', "ゲ"), ('o', "ゴ"));
            root['z'].Add(('a', "ザ"), ('i', "ジ"), ('u', "ズ"), ('e', "ゼ"), ('o', "ゾ"));
            root['d'].Add(('a', "ダ"), ('i', "ヂ"), ('u', "ヅ"), ('e', "デ"), ('o', "ド"));
            root['b'].Add(('a', "バ"), ('i', "ビ"), ('u', "ブ"), ('e', "ベ"), ('o', "ボ"));
            root['p'].Add(('a', "パ"), ('i', "ピ"), ('u', "プ"), ('e', "ペ"), ('o', "ポ"));
            root['v'].Add(('a', "ヴァ"), ('i', "ヴィ"), ('u', "ヴ"), ('e', "ヴェ"), ('o', "ヴォ"));
            root.Add("chi", root["ti"]);

            {
                var consonants = new[] {
                    ('k', 'キ'), ('s', 'シ'), ('t', 'チ'),
                    ('n', 'ニ'), ('h', 'ヒ'), ('m', 'ミ'),
                    ('r', 'リ'), ('g', 'ギ'), ('z', 'ジ'),
                    ('d', 'ヂ'), ('b', 'ビ'), ('p', 'ぴ'),
                    ('v', 'ゔ'), ('q', 'く'), ('f', 'ふ')
                };
                var smallY = new[] {
                    ("ya", 'ゃ'),
                    ("yi", 'ぃ'),
                    ("yu", 'ゅ'),
                    ("ye", 'ぇ'),
                    ("yo", 'ょ')
                };

                foreach (var a in consonants)
                {
                    foreach (var b in smallY)
                    {
                        root.Add($"{a.Item1}{b.Item1}", $"{a.Item2}{b.Item2}");
                    }
                }
            }

            root.Add(
                ('.', "。"), (',', "、"), (':', "："),
                ('/', "・"), ('!', "！"), ('?', "？"),
                ('~', "〜"), ('-', "ー"), ('‘', "「"),
                ('’', "」"), ('“', "『"), ('”', "』"),
                ('[', "［"), (']', "］"), ('(', "（"),
                (')', "）"), ('{', "｛"), ('}', "｝")
            );

            root.Add("xn", root['n']);
            root.Add("n'", "ん");

            return trie;
        }
    }
}
