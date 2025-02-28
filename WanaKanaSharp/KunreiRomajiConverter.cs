using System;
using System.Linq;
using WanaKanaSharp.Utility;

namespace WanaKanaSharp;

public class KunreiRomajiConverter : RomajiConverter
{
    static readonly Trie<char, string> RomajiTree = new();

    static Trie<char, string> BuildHiraganaTree()
    {
        var trie = new Trie<char, string>();
        var root = trie.Root;

        root.Insert(('あ', "a"), ('い', "i"), ('う', "u"), ('え', "e"), ('お', "o"),
                    ('か', "ka"), ('き', "ki"), ('く', "ku"), ('け', "ke"), ('こ', "ko"),
                    ('さ', "sa"), ('し', "si"), ('す', "su"), ('せ', "se"), ('そ', "so"),
                    ('た', "ta"), ('ち', "ti"), ('つ', "tu"), ('て', "te"), ('と', "to"),
                    ('な', "na"), ('に', "ni"), ('ぬ', "nu"), ('ね', "ne"), ('の', "no"),
                    ('は', "ha"), ('ひ', "hi"), ('ふ', "hu"), ('へ', "he"), ('ほ', "ho"),
                    ('ま', "ma"), ('み', "mi"), ('む', "mu"), ('め', "me"), ('も', "mo"),
                    ('や', "ya"), ('ゆ', "yu"), ('よ', "yo"),
                    ('ら', "ra"), ('り', "ri"), ('る', "ru"), ('れ', "re"), ('ろ', "ro"),
                    ('わ', "wa"), ('を', "wo"),
                    ('ん', "n"),
                    ('が', "ga"), ('ぎ', "gi"), ('ぐ', "gu"), ('げ', "ge"), ('ご', "go"),
                    ('ざ', "za"), ('じ', "zy"), ('ず', "zu"), ('ぜ', "ze"), ('ぞ', "zo"),
                    ('だ', "da"), ('ぢ', "ji"), ('づ', "zu"), ('で', "de"), ('ど', "do"),
                    ('ば', "ba"), ('び', "bi"), ('ぶ', "bu"), ('べ', "be"), ('ぼ', "bo"),
                    ('ぱ', "pa"), ('ぴ', "pi"), ('ぷ', "pu"), ('ぺ', "pe"), ('ぽ', "po"),
                    ('ぁ', "a"), ('ぃ', "i"), ('ぅ', "u"), ('ぇ', "e"), ('ぉ', "o"),
                    ('ゃ', "ya"), ('ゅ', "yu"), ('ょ', "yo"));

        {
            var whitelist = new[] { 'き', 'に', 'ひ', 'み', 'り', 'ぎ', 'び', 'ぴ' };

            foreach (var key in whitelist)
            {
                var node = root[key];
                var prefix = node.Value[0];

                root[key].Insert(('ゃ', prefix + "ya"), ('ゅ', prefix + "yu"), ('ょ', prefix + "yo"));
            }
        }

        {
            var whitelist = new[] { 'し', 'ち' };

            foreach (var key in whitelist)
            {
                var node = root[key];
                var prefix = node.Value[0];

                node.Insert(('ゃ', prefix + "ya"), ('ゅ', prefix + "yu"), ('ょ', prefix + "yo"));
            }
        }

        {
            var whitelist = new[] { 'じ', 'ぢ' };

            foreach (var key in whitelist)
            {
                var node = root[key];
                var prefix = node.Value[0];

                node.Insert(('ゃ', prefix + "ya"), ('ゅ', prefix + "yu"), ('ょ', prefix + "yo"));
            }
        }

        {
            var node = root['ん'];
            var prefix = node.Value[0];

            node.Insert(('や', prefix + "'ya"), ('ゆ', prefix + "'yu"), ('よ', prefix + "'yo"));
        }

        {
            var node = root['ん'];
            var prefix = node.Value[0];

            node.Insert(('あ', prefix + "'a"), ('い', prefix + "'i"), ('う', prefix + "'u"), ('え', prefix + "'e"), ('お', prefix + "'o"));
        }

        {
            var sokuon = root.Insert(('っ', ""));
            var exceptions = new[]
            {
                    'あ', 'い', 'う', 'え', 'お',
                    'や', 'ゆ', 'よ',
                    'ん',
                    'ぁ', 'ぃ', 'ぅ', 'ぇ', 'ぉ',
                    'ゃ', 'ゅ', 'ょ',
                    'っ'
                };

            foreach (var child in root.Where((node) => !exceptions.Contains(node.Key)))
            {
                sokuon.Insert(child.Duplicate(true));
            }

            sokuon.TraverseChildren((node) =>
            {
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

        return trie;
    }

    static Trie<char, string> BuildKatakanaTree()
    {
        var trie = new Trie<char, string>();
        var root = trie.Root;

        root.Insert(('ア', "a"), ('イ', "i"), ('ウ', "u"), ('エ', "e"), ('オ', "o"),
                    ('カ', "ka"), ('キ', "ki"), ('ク', "ku"), ('ケ', "ke"), ('コ', "ko"),
                    ('サ', "sa"), ('シ', "shi"), ('ス', "su"), ('セ', "se"), ('ソ', "so"),
                    ('タ', "ta"), ('チ', "chi"), ('ツ', "tsu"), ('テ', "te"), ('ト', "to"),
                    ('ナ', "na"), ('ニ', "ni"), ('ヌ', "nu"), ('ネ', "ne"), ('ノ', "no"),
                    ('ハ', "ha"), ('ヒ', "hi"), ('フ', "hu"), ('ヘ', "he"), ('ホ', "ho"),
                    ('マ', "ma"), ('ミ', "mi"), ('ム', "mu"), ('メ', "me"), ('モ', "mo"),
                    ('ヤ', "ya"), ('ユ', "yu"), ('ヨ', "yo"),
                    ('ラ', "ra"), ('リ', "ri"), ('ル', "ru"), ('レ', "re"), ('ロ', "ro"),
                    ('ワ', "wa"), ('ヲ', "wo"),
                    ('ン', "n"),
                    ('ガ', "ga"), ('ギ', "gi"), ('グ', "gu"), ('ゲ', "ge"), ('ゴ', "go"),
                    ('ザ', "za"), ('ジ', "ji"), ('ズ', "zu"), ('ゼ', "ze"), ('ゾ', "zo"),
                    ('ダ', "da"), ('ヂ', "ji"), ('ヅ', "zu"), ('デ', "de"), ('ド', "do"),
                    ('バ', "ba"), ('ビ', "bi"), ('ブ', "bu"), ('ベ', "be"), ('ボ', "bo"),
                    ('パ', "pa"), ('ピ', "pi"), ('プ', "pu"), ('ペ', "pe"), ('ポ', "po"),
                    ('ァ', "a"), ('ィ', "i"), ('ゥ', "u"), ('ェ', "e"), ('ォ', "o"),
                    ('ャ', "ya"), ('ュ', "yu"), ('ョ', "yo"));

        {
            var whitelist = new[] { 'キ', 'ニ', 'ヒ', 'ミ', 'リ', 'ギ', 'ビ', 'ピ' };

            foreach (var key in whitelist)
            {
                var node = root[key];
                var prefix = node.Value[0];

                root[key].Insert(('ャ', prefix + "ya"), ('ュ', prefix + "yu"), ('ョ', prefix + "yo"));
            }
        }

        {
            var whitelist = new[] { 'シ', 'チ' };

            foreach (var key in whitelist)
            {
                var node = root[key];
                var prefix = node.Value[0];

                node.Insert(('ャ', prefix + "ya"), ('ュ', prefix + "yu"), ('ョ', prefix + "yo"));
            }
        }

        {
            var whitelist = new[] { 'ジ', 'ヂ' };

            foreach (var key in whitelist)
            {
                var node = root[key];
                var prefix = node.Value[0];

                node.Insert(('ャ', prefix + "a"), ('ュ', prefix + "u"), ('ョ', prefix + "o"));
            }
        }

        {
            var node = root['ン'];
            var prefix = node.Value[0];

            node.Insert(('ヤ', prefix + "'ya"), ('ユ', prefix + "'yu"), ('ヨ', prefix + "'yo"));
        }

        {
            var node = root['ン'];
            var prefix = node.Value[0];

            node.Insert(('ア', prefix + "'a"), ('イ', prefix + "'i"), ('ウ', prefix + "'u"), ('エ', prefix + "'e"), ('オ', prefix + "'o"));
        }

        {
            var sokuon = root.Insert(('ッ', ""));
            var exceptions = new[]
            {
                    'ア', 'イ', 'ウ', 'エ', 'オ',
                    'ヤ', 'ユ', 'ヨ',
                    'ン',
                    'ァ', 'ィ', 'ゥ', 'ェ', 'ォ',
                    'ャ', 'ュ', 'ョ',
                    'ッ'
                };

            foreach (var child in root.Where((node) => !exceptions.Contains(node.Key)))
            {
                sokuon.Insert(child.Duplicate(true));
            }

            sokuon.TraverseChildren((node) =>
            {
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

            {
                var blacklist = new[] { 'ン', 'ッ' };

                root.TraverseChildren((node) =>
                {
                    if (blacklist.Contains(node.Key)) return;

                    var value = node.Value;

                    node.Insert((Key: 'ー', Value: value + value[^1]));
                });
            }
        }

        return trie;
    }

    static KunreiRomajiConverter()
    {
        var hiraganaTree = BuildHiraganaTree();
        var katakanaTree = BuildKatakanaTree();
        var kanaTree = Trie<char, string>.Merge(hiraganaTree, katakanaTree, (a, b) => b.Value);

        var root = RomajiTree.Root;

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

        RomajiTree.Merge(kanaTree, (a, b) => b.Value);
    }

    protected override Trie<char, string> GetTrie() => RomajiTree;
}
