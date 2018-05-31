<div align="center">
    <a href="https://travis-ci.org/caguiclajmg/WanaKanaSharp">
        <img src="https://img.shields.io/travis/caguiclajmg/WanaKanaSharp.svg" />
    </a>
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
    WanaKana.ToRomaji("今日 は パーティ", upcaseKatakana = true); // 今日 ha PAATI
```