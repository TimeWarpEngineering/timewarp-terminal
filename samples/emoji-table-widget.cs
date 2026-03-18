#!/usr/bin/dotnet --
#:project $(SourceDirectory)timewarp-terminal/timewarp-terminal.csproj

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

// ── Emoji_Presentation=Yes (width 2 without VS16) ──
terminal
  .WriteRule("Default Emoji Presentation")
  .WriteTable(t => t
    .AddColumn("Symbol")
    .AddColumn("Name")
    .AddColumn("Range")
    .AddRow("⚡ Zap", "U+26A1", "Misc Symbols")
    .AddRow("✅ Check", "U+2705", "Dingbats")
    .AddRow("❌ Cross", "U+274C", "Dingbats")
    .AddRow("⭐ Star", "U+2B50", "Misc Sym+Arrows")
    .AddRow("⬛ Black", "U+2B1B", "Misc Sym+Arrows")
    .AddRow("☔ Rain", "U+2614", "Misc Symbols")
    .AddRow("⌚ Watch", "U+231A", "Misc Technical")
    .AddRow("⏰ Alarm", "U+23F0", "Misc Technical")
    .AddRow("➕ Plus", "U+2795", "Dingbats")
    .AddRow("❗ Exclaim", "U+2757", "Dingbats")
    .Border(BorderStyle.Rounded))
  .WriteLine();

// ── Text presentation + VS16 (width 2 only with ️) ──
terminal
  .WriteRule("Text Presentation + VS16")
  .WriteTable(t => t
    .AddColumn("With VS16")
    .AddColumn("Name")
    .AddRow("☀️ Sun", "U+2600 + U+FE0F")
    .AddRow("♻️ Recycle", "U+267B + U+FE0F")
    .AddRow("✈️ Plane", "U+2708 + U+FE0F")
    .AddRow("▶️ Play", "U+25B6 + U+FE0F")
    .AddRow("☁️ Cloud", "U+2601 + U+FE0F")
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
