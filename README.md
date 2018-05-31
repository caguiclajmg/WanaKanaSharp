<div align="center">
    <table>
        <tr>
            <th>Travis CI</th>
            <th>AppVeyor</th>
        </tr>
        <tr>
            <td>
                <a href="https://travis-ci.org/caguiclajmg/WanaKanaSharp">
                    <img src="https://img.shields.io/travis/caguiclajmg/WanaKanaSharp.svg" />
                </a>
            </td>
            <td>
                <a href="https://ci.appveyor.com/project/caguiclajmg/wanakanasharp">
                    <div align="center">
                        <img src="https://img.shields.io/appveyor/ci/caguiclajmg/WanaKanaSharp.svg" />
                    </div>
                    <div align="center">
                        <img src="https://img.shields.io/appveyor/tests/caguiclajmg/WanaKanaSharp.svg" />
                    </div>
                </a>
            </td>
        </tr>
    </table>
</div>

<div align="center">
    <h1>ワナカナ &lt;--&gt; WanaKanaSharp &lt;--&gt; わなかな</h1>
    <h4>A .NET utility library for checking and converting between Kanji, Hiragana, Katakana and Romaji</h4>
    <h6>This project is a port of :crocodile: <a href="https://github.com/WaniKani/WanaKana">WanaKana</a> :crab:</h6>
</div>

## Usage

NuGet packages for WanaKanaSharp are unavailable at the moment.

## Examples

```cs
    using WanaKanaSharp;

    WanaKana.IsRomaji("hello"); // true
    WanaKana.IsHiragana("こんにちは"); // true
    WanaKana.IsKatakana("テレビ"); // true
    WanaKana.IsKana("これはキュートです") // true
    WanaKana.IsKanji("日本語") // true

    WanaKana.ToRomaji("ひらがな"); // hiragana
    WanaKana.ToRomaji("カタカナ"); // katakana
    WanaKana.ToRomaji("今日 は パーティ", upcaseKatakana = true); // 今日 ha PAATEI
```