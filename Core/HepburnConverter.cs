//
// HepburnConverter.cs
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
using System.Text;

using WanaKanaSharp.Utility;

namespace WanaKanaSharp
{
	public static class HepburnConverter
	{
		static Trie<Char, String> HepburnTree = new Trie<Char, String>();

		static Dictionary<Char, Char> KatakanaMap = new Dictionary<Char, Char>();

		static HepburnConverter()
		{
			BuildHepburnMap();
			BuildKatakanaMap();
		}

		public static String Convert(String input, Boolean upcaseKatakana, Trie<Char, String> customRomajiMapping)
		{
			if (String.IsNullOrEmpty(input)) return "";

			var root = HepburnTree.Root;
			var builder = new StringBuilder();

			Int32 position = 0;
			do
			{
				var pair = Convert(root, input, position);
				var uppercase = upcaseKatakana && IsKatakana(input.Substring(position, pair.Position - position));
				builder.Append(uppercase ? pair.Token.ToUpper() : pair.Token);
				position = pair.Position;
			} while (position < input.Length);

			return builder.ToString();
		}

		static void BuildHepburnMap()
		{
			var root = HepburnTree.Root;

			root.Insert(('あ', "a"), ('い', "i"), ('う', "u"), ('え', "e"), ('お', "o"),
						('か', "ka"), ('き', "ki"), ('く', "ku"), ('け', "ke"), ('こ', "ko"),
						('さ', "sa"), ('し', "shi"), ('す', "su"), ('せ', "se"), ('そ', "so"),
						('た', "ta"), ('ち', "chi"), ('つ', "tsu"), ('て', "te"), ('と', "to"),
						('な', "na"), ('に', "ni"), ('ぬ', "nu"), ('ね', "ne"), ('の', "no"),
						('は', "ha"), ('ひ', "hi"), ('ふ', "hu"), ('へ', "he"), ('ほ', "ho"),
						('ま', "ma"), ('み', "mi"), ('む', "mu"), ('め', "me"), ('も', "mo"),
						('や', "ya"), ('ゆ', "yu"), ('よ', "yo"),
						('ら', "ra"), ('り', "ri"), ('る', "ru"), ('れ', "re"), ('ろ', "ro"),
						('わ', "wa"), ('を', "wo"),
						('ん', "n"),
						('が', "ga"), ('ぎ', "gi"), ('ぐ', "gu"), ('げ', "ge"), ('ご', "go"),
						('ざ', "za"), ('じ', "ji"), ('ず', "zu"), ('ぜ', "ze"), ('ぞ', "zo"),
						('だ', "da"), ('ぢ', "ji"), ('づ', "zu"), ('で', "de"), ('ど', "do"),
						('ば', "ba"), ('び', "bi"), ('ぶ', "bu"), ('べ', "be"), ('ぼ', "bo"),
						('ぱ', "pa"), ('ぴ', "pi"), ('ぷ', "pu"), ('ぺ', "pe"), ('ぽ', "po"),
						('ぁ', "a"), ('ぃ', "i"), ('ぅ', "u"), ('ぇ', "e"), ('ぉ', "o"),
						('ゃ', "ya"), ('ゅ', "yu"), ('ょ', "yo"));

			root.Insert(('。', "."),
						('、', ","),
						('：', ":"),
						('・', "/"),
						('！', "!"),
						('？', "?"),
						('〜', "~"),
						('ー', "-"),
						('「', "‘"),
						('」', "’"),
						('『', "“"),
						('』', "”"),
						('［', "["),
						('］', "]"),
						('（', "("),
						('）', ")"),
						('｛', "{"),
						('｝', "}"),
						('　', " "));

			foreach (var key in new[] { 'き', 'に', 'ひ', 'み', 'り', 'ぎ', 'び', 'ぴ' })
			{
				var node = root[key];
				var prefix = node.Value[0];

				root[key].Insert(('ゃ', prefix + "ya"), ('ゅ', prefix + "yu"), ('ょ', prefix + "yo"));
			}

			foreach (var key in new[] { 'し', 'ち' })
			{
				var node = root[key];
				var prefix = node.Value[0];

				node.Insert(('ゃ', prefix + "ha"), ('ゅ', prefix + "hu"), ('ょ', prefix + "ho"));
			}

			foreach (var key in new[] { 'じ', 'ぢ' })
			{
				var node = root[key];
				var prefix = node.Value[0];

				node.Insert(('ゃ', prefix + "a"), ('ゅ', prefix + "u"), ('ょ', prefix + "o"));
			}

			BuildSokuonTree();
		}

		static void BuildSokuonTree()
		{
			var root = HepburnTree.Root;
			var sokuon = root.Insert(('っ', ""));

			var exceptions = new[]
			{
				'あ', 'う', 'え', 'お',
				'や', 'ゆ', 'よ',
				'ん',
				'ぁ', 'ぃ', 'ぅ', 'ぇ', 'ぉ',
				'ゃ', 'ゅ', 'ょ'
			};
			foreach (var child in root.Where((node) => !exceptions.Contains(node.Key)))
			{
				sokuon.Insert(child.Duplicate(true));
			}

			sokuon.TraverseChildren((node) =>
			{
				if (String.IsNullOrEmpty(node.Value)) return;

				var value = node.Value;
				if (node.Value.StartsWith("ch", StringComparison.Ordinal))
				{
					node.Value = 't' + value;
				}
				else
				{
					node.Value = value[0] + value;
				}
			}, -1);
		}

		static void BuildChoonpu()
		{

		}

		static void BuildKatakanaMap()
		{
			foreach (var a in new[]
			{
				('ア', 'あ'), ('イ', 'い'), ('ウ', 'う'), ('エ', 'え'), ('オ', 'お'),
				('カ', 'か'), ('キ', 'き'), ('ク', 'く'), ('ケ', 'け'), ('コ', 'こ'),
				('サ', 'さ'), ('シ', 'し'), ('ス', 'す'), ('セ', 'せ'), ('ソ', 'そ'),
				('タ', 'た'), ('チ', 'ち'), ('ツ', 'つ'), ('テ', 'て'), ('ト', 'と'),
				('ナ', 'な'), ('ニ', 'に'), ('ヌ', 'ぬ'), ('ネ', 'ね'), ('ノ', 'の'),
				('ハ', 'は'), ('ヒ', 'ひ'), ('フ', 'ふ'), ('ヘ', 'へ'), ('ホ', 'ほ'),
				('マ', 'ま'), ('ミ', 'み'), ('ム', 'む'), ('メ', 'め'), ('モ', 'も'),
				('ヤ', 'や'), ('ユ', 'ゆ'), ('ヨ', 'よ'),
				('ラ', 'ら'), ('リ', 'り'), ('ル', 'る'), ('レ', 'れ'), ('ロ', 'ろ'),
				('ワ', 'わ'), ('ヲ', 'を'),
				('ン', 'ん'),
				('ガ', 'が'), ('ギ', 'ぎ'), ('グ', 'ぐ'), ('ゲ', 'げ'), ('ゴ', 'ご'),
				('ザ', 'ざ'), ('ジ', 'じ'), ('ズ', 'ず'), ('ゼ', 'ぜ'), ('ゾ', 'ぞ'),
				('ダ', 'だ'), ('ヂ', 'ぢ'), ('ヅ', 'づ'), ('デ', 'で'), ('ド', 'ど'),
				('バ', 'ば'), ('ビ', 'び'), ('ブ', 'ぶ'), ('ベ', 'べ'), ('ボ', 'ぼ'),
				('パ', 'ぱ'), ('ピ', 'ぴ'), ('プ', 'ぷ'), ('ペ', 'ぺ'), ('ポ', 'ぽ'),
				('ァ', 'ぁ'), ('ィ', 'ぃ'), ('ゥ', 'ぅ'), ('ェ', 'ぇ'), ('ォ', 'ぉ'),
				('ャ', 'ゃ'), ('ュ', 'ゅ'), ('ョ', 'ょ')
			})
			{
				KatakanaMap.Add(a.Item1, a.Item2);
			}
		}

		static (String Token, Int32 Position) Convert(Trie<Char, String>.Node node, String input, Int32 position)
		{
			var c = input[position];
			if (IsKatakana(c))
			{
				if (c == CharacterConstants.ProlongedSoundMark)
				{

				}
				else
				{
					c = KatakanaMap[c];
				}
			}
			if (!node.ContainsKey(c))
			{
				return (Token: c.ToString(), Position: position + 1);
			}

			var current = node;
			var next = current.GetChild(c);

			while (next != null)
			{
				current = next;
				position++;

				if (position == input.Length) break;

				next = current.GetChild(input[position]);
			}

			return (Token: current.Value, Position: position);
		}

		static Boolean IsKatakana(Char input)
		{
			return (input >= CharacterConstants.KatakanaStart) && (input <= CharacterConstants.KatakanaEnd);
		}

		static Boolean IsKatakana(String input)
		{
			return !String.IsNullOrEmpty(input) && input.All(IsKatakana);
		}
	}
}
