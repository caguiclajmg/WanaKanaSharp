//
// RomajiConverter.cs
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

using System.Text;
using WanaKanaSharp.Utility;

namespace WanaKanaSharp;

public abstract class RomajiConverter
{
    readonly Trie<char, string> HepburnTree = new();

    protected abstract Trie<char, string> GetTrie();

    /// <summary>
    /// Converts
    /// </summary>
    /// <returns>The Romaji equivalent of the input string.</returns>
    /// <param name="input">The string to convert.</param>
    /// <param name="upcaseKatakana">If set to <c>true</c>, Katakana characters are converted into uppercase Romaji characters.</param>
    /// <param name="customRomajiMapping">Custom Romaji mapping rules.</param>
    public virtual string ToRomaji(string input, bool upcaseKatakana = false, Trie<char, string>? customRomajiMapping = null)
    {
        if (string.IsNullOrEmpty(input)) return string.Empty;

        var romajiTreeBase = GetTrie();
        var romajiTreeOverlay = customRomajiMapping ?? Trie<char, string>.Empty;
        var romajiTree = Trie<char, string>.Merge(romajiTreeBase, romajiTreeOverlay, (a, b) => b.Value);

        var builder = new StringBuilder();

        int position = 0;
        do
        {
            var (Token, Position) = Convert(romajiTree, input, position);
            var uppercase = upcaseKatakana && WanaKana.IsKatakana(input[position..Position]);
            builder.Append(uppercase ? Token.ToUpper() : Token);
            position = Position;
        } while (position < input.Length);

        return builder.ToString();
    }

    public static (string Token, int Position) Convert(Trie<char, string> romajiTree, string input, int position)
    {
        var current = romajiTree.Root;
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
}