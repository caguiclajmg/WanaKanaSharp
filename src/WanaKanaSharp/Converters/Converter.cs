//
// Converter.cs
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

using System;
using System.Security.Principal;
using System.Text;
using WanaKanaSharp.Utility;

namespace WanaKanaSharp.Converters;

public abstract class Converter : IRomajiConverter, IKanaConverter
{
    private readonly MyInMemoryTrie _romajiTree = new();
    private readonly MyInMemoryTrie _kanaTree = new();

    protected abstract void BuildRomajiTree(MyTrie trie);
    protected abstract void BuildKanaTree(MyTrie trie);

    protected Converter()
    {
        BuildRomajiTree(_romajiTree);
        BuildKanaTree(_kanaTree);

        (char Key, string Value)[] punctuation = [
            ('。', "."),
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
            ('　', " ")
        ];
        _romajiTree.Root.Insert(punctuation);
        _kanaTree.Root.Insert(punctuation);
    }

    private static (string Token, int Position) Convert(MyTrie trie, string input, int position)
    {
        var current = trie.Root;
        var next = current.GetChild(input[position]);

        if (next == null) return (Token: input[position].ToString(), Position: position + 1);

        while (next != null)
        {
            current = next;
            position++;

            if (position == input.Length) break;

            next = current.GetChild(input[position]);
        }

        return (Token: current.Value, Position: position);
    }

    private static string Convert(
        MyTrie trie,
        string input,
        Func<string, Range, string, string>? processToken = null, 
        MyTrie? customMapping = null
    )
    {
        if (string.IsNullOrEmpty(input)) return string.Empty;

        customMapping ??= MyInMemoryTrie.Empty;
        var mapping = Trie.Merge<char, string, MyInMemoryTrie>(trie, customMapping);

        var builder = new StringBuilder();

        int position = 0;
        do
        {
            var (Token, Position) = Convert(mapping, input, position);
            var value = processToken != null ? processToken.Invoke(input, new Range(position, Position), Token) : Token;
            builder.Append(value);
            position = Position;
        } while (position < input.Length);

        return builder.ToString();
    }

    /// <summary>
    /// Converts
    /// </summary>
    /// <param name="input">The string to convert.</param>
    /// <param name="upcaseKatakana">If set to <c>true</c>, Katakana characters are converted into uppercase Romaji characters.</param>
    /// <param name="customRomajiMapping">Custom Romaji mapping rules, this is recursively merged with the base Romaji tree and takes precedence in case of conflicts.</param>
    /// <returns>The Romaji equivalent of the input string.</returns>
    public virtual string ToRomaji(string input, bool upcaseKatakana = false, MyTrie? customRomajiMapping = null)
    {
        return Convert(_romajiTree, input, ProcessToken, customMapping: customRomajiMapping);

        string ProcessToken(string input, Range range, string token)
        {
            var uppercase = upcaseKatakana && WanaKana.IsKatakana(input[range.Start..range.End]);
            token = uppercase ? token.ToUpper() : token;
            return token;
        }
    }

    /// <summary>
    /// Converts
    /// </summary>
    /// <param name="input"></param>
    /// <param name="useObsoleteKana"></param>
    /// <param name="customKanaMapping"></param>
    /// <returns>The Kana equivalent of the input string.</returns>
    public virtual string ToKana(string input, bool useObsoleteKana, MyTrie? customKanaMapping = null)
    {
        return Convert(_romajiTree, input, ProcessToken, customMapping: customKanaMapping);

        static string ProcessToken(string input, Range range, string token) => token;
    }
}