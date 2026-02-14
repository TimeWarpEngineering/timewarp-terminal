#!/usr/bin/dotnet --
#:project ../source/timewarp-terminal/timewarp-terminal.csproj

// Demonstrates emoji and wide character alignment in table/panel/rule borders
using TimeWarp.Terminal;

TimeWarpTerminal terminal = new();

// ── Emoji blocks (U+1F000-U+1FAFF) ──
terminal
  .WriteRule("🌤️ Weather Report", style: LineStyle.Doubled)
  .WriteTable(t => t
    .AddColumn("Info")
    .AddColumn("Value")
    .AddRow("📍 Location", "Dallas, United States")
    .AddRow("🌡️ Temperature", "25.2°C (77.4°F)")
    .AddRow("☁️ Condition", "Overcast")
    .Border(BorderStyle.Rounded))
  .WriteLine();

// ── Miscellaneous Symbols U+2600-U+26FF and Dingbats U+2700-U+27BF ──
terminal
  .WriteRule("Misc Symbols + Dingbats")
  .WriteTable(t => t
    .AddColumn("Symbol")
    .AddColumn("Name")
    .AddColumn("Range")
    .AddRow("☀ Sun", "U+2600", "Misc Symbols")
    .AddRow("⚡ Zap", "U+26A1", "Misc Symbols")
    .AddRow("♻ Recycle", "U+267B", "Misc Symbols")
    .AddRow("✅ Check", "U+2705", "Dingbats")
    .AddRow("❌ Cross", "U+274C", "Dingbats")
    .AddRow("✈ Plane", "U+2708", "Dingbats")
    .Border(BorderStyle.Rounded))
  .WriteLine();

// ── Miscellaneous Technical U+2300-U+23FF ──
terminal
  .WriteRule("Misc Technical")
  .WriteTable(t => t
    .AddColumn("Symbol")
    .AddColumn("Name")
    .AddRow("⌚ Watch", "U+231A")
    .AddRow("⌛ Hourglass", "U+231B")
    .AddRow("⏰ Alarm", "U+23F0")
    .AddRow("⏳ Timer", "U+23F3")
    .Border(BorderStyle.Rounded))
  .WriteLine();

// ── Geometric Shapes U+25A0-U+25FF ──
terminal
  .WriteRule("Geometric Shapes")
  .WriteTable(t => t
    .AddColumn("Symbol")
    .AddColumn("Name")
    .AddRow("▶ Play", "U+25B6")
    .AddRow("◀ Reverse", "U+25C0")
    .AddRow("▪ Small Square", "U+25AA")
    .AddRow("▫ White Square", "U+25AB")
    .Border(BorderStyle.Rounded))
  .WriteLine();

// ── Misc Symbols and Arrows U+2B00-U+2BFF ──
terminal
  .WriteRule("Misc Symbols and Arrows")
  .WriteTable(t => t
    .AddColumn("Symbol")
    .AddColumn("Name")
    .AddRow("⬛ Black Square", "U+2B1B")
    .AddRow("⬜ White Square", "U+2B1C")
    .AddRow("⭐ Star", "U+2B50")
    .AddRow("⭕ Circle", "U+2B55")
    .Border(BorderStyle.Rounded))
  .WriteLine();

// ── CJK Characters ──
terminal
  .WriteRule("CJK Characters")
  .WriteTable(t => t
    .AddColumn("Text")
    .AddColumn("Range")
    .AddRow("漢字", "CJK Unified (U+4E00)")
    .AddRow("ひらがな", "Hiragana (U+3041)")
    .AddRow("カタカナ", "Katakana (U+30A0)")
    .AddRow("한글", "Hangul (U+AC00)")
    .Border(BorderStyle.Rounded))
  .WriteLine();

// ── Fullwidth Forms ──
terminal
  .WriteRule("Fullwidth Forms")
  .WriteTable(t => t
    .AddColumn("Fullwidth")
    .AddColumn("Normal")
    .AddRow("ＡＢＣＤ", "ABCD")
    .AddRow("１２３４", "1234")
    .Border(BorderStyle.Rounded))
  .WriteLine();

// ── Multi-codepoint grapheme clusters (ZWJ, flags, skin tones) ──
terminal
  .WriteRule("Multi-Codepoint Clusters")
  .WriteTable(t => t
    .AddColumn("Emoji")
    .AddColumn("Type")
    .AddRow("🇺🇸 Flag", "Regional Indicators")
    .AddRow("👋🏽 Wave", "Skin Tone Modified")
    .AddRow("🌤️ Cloud", "Variation Selector")
    .Border(BorderStyle.Rounded))
  .WriteLine();

// ── Panel with mixed content ──
terminal
  .WritePanel(p => p
    .Header("💠 Status Dashboard")
    .Content("✅ API online  ❌ DB offline  ⏳ Cache warming  ⭐ 99.9% uptime")
    .Border(BorderStyle.Rounded))
  .WriteLine()
  .WriteLine("Demo complete!");
