//
// KanaConverter.cs
//
// Author:
//       John Mark Gabriel Caguicla <jmg.caguicla@yozuru.jp>
//
// Copyright (c) 2019 John Mark Gabriel Caguicla
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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using WanaKanaSharp.Utility;

namespace WanaKanaSharp {
    public class KanaConverter {
        private readonly static Trie<char, string> kanaTree = BuildKanaTree();

        static KanaConverter() {
            kanaTree = BuildKanaTree();
        }

        static Trie<char, string> BuildKanaTree() {
            var trie = new Trie<char, string>();
            var root = trie.Root;

            root.Add(('a', "あ"), ('i', "い"), ('u', "う"), ('e', "え"), ('o', "お"));
            root.Add(
                ('k', ""),
                ('s', ""),
                ('t', ""),
                ('n', "ん"),
                ('h', ""),
                ('m', ""),
                ('y', ""),
                ('r', ""),
                ('w', ""),
                ('g', ""),
                ('z', ""),
                ('d', ""),
                ('b', ""),
                ('p', ""),
                ('v', "")
            );
            root['k'].Add(('a', "か"), ('i', "き"), ('u', "く"), ('e', "け"), ('o', "こ"));
            root['s'].Add(('a', "さ"), ('i', "し"), ('u', "す"), ('e', "せ"), ('o', "そ"));
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
            root.Add("chi", root.TryGetChild("ti"));

            {
                var consonants = new[] {
                    ('k', 'き'),
                    ('s', 'し'),
                    ('t', 'ち'),
                    ('n', 'に'),
                    ('h', 'ひ'),
                    ('m', 'み'),
                    ('r', 'り'),
                    ('g', 'ぎ'),
                    ('z', 'じ'),
                    ('d', 'ぢ'),
                    ('b', 'び'),
                    ('p', 'ぴ'),
                    ('v', 'ゔ'),
                    ('q', 'く'),
                    ('f', 'ふ')
                };
                var smallY = new[] {
                    ("ya", 'ゃ'),
                    ("yi", 'ぃ'),
                    ("yu", 'ゅ'),
                    ("ye", 'ぇ'),
                    ("yo", 'ょ')
                };

                foreach(var a in consonants) {
                    foreach(var b in smallY) {
                        root.Add($"{a.Item1}{b.Item1}", $"{a.Item2}{b.Item2}");
                    }
                }
            }

            root.Add(
                ('.', "。"),
                (',', "、"),
                (':', "："),
                ('/', "・"),
                ('!', "！"),
                ('?', "？"),
                ('~', "〜"),
                ('-', "ー"),
                ('‘', "「"),
                ('’', "」"),
                ('“', "『"),
                ('”', "』"),
                ('[', "［"),
                (']', "］"),
                ('(', "（"),
                (')', "）"),
                ('{', "｛"),
                ('}', "｝")
            );

            {
                var aliases = new[] { "n'", "xn" };
                foreach(var alias in aliases) root.Add(alias, root.TryGetChild("n"));
            }


            return trie;
        }

        public KanaConverter() {

        }

        public static string ToKana(string input, bool useObsoleteKana, Trie<char, string> customKanaMapping) {
            if(string.IsNullOrEmpty(input)) return "";

            var builder = new StringBuilder();

            int position = 0;
            do {
                var pair = Convert(kanaTree, input, position);
                builder.Append(pair.Token);
                position = pair.Position;
            } while(position < input.Length);

            return builder.ToString();
        }

        static (string Token, int Position) Convert(Trie<char, string> romajiTree, string input, int position) {
            var current = romajiTree.Root;
            var next = current.TryGetChild(input[position]);

            if(next == null) return (Token: input[position].ToString(), Position: position + 1);

            while(next != null) {
                current = next;
                position++;

                if(position == input.Length) break;

                next = current.TryGetChild(input[position]);
            }

            return (Token: current.Value, Position: position);
        }
    }
}
