#!/usr/bin/env -S dotnet --
#:project $(SourceDirectory)timewarp-terminal/timewarp-terminal.csproj

using System.Text;

// Test UnicodeWidth functionality

#if !JARIBU_MULTI
return await RunAllTests();
#endif

namespace TimeWarp.Terminal.Tests.Core.UnicodeWidthTests
{

  [TestTag("Widgets")]
  public class UnicodeWidthBasicTests
  {
    [ModuleInitializer]
    internal static void Register() => RegisterTests<UnicodeWidthBasicTests>();

    public static async Task Should_return_width_1_for_ascii_characters()
    {
      UnicodeWidth.GetTextWidth("Hello").ShouldBe(5);
      UnicodeWidth.GetTextWidth("ABC").ShouldBe(3);

      await Task.CompletedTask;
    }

    public static async Task Should_return_width_0_for_empty_and_null()
    {
      UnicodeWidth.GetTextWidth("").ShouldBe(0);
      UnicodeWidth.GetTextWidth(null).ShouldBe(0);

      await Task.CompletedTask;
    }

    public static async Task Should_return_width_2_for_basic_emoji()
    {
      UnicodeWidth.GetTextWidth("😀").ShouldBe(2);
      UnicodeWidth.GetTextWidth("🎉").ShouldBe(2);

      await Task.CompletedTask;
    }

    public static async Task Should_return_width_2_for_weather_emoji()
    {
      // 🌤️ is U+1F324 + U+FE0F (variation selector)
      UnicodeWidth.GetTextWidth("🌤️").ShouldBe(2);

      await Task.CompletedTask;
    }

    public static async Task Should_return_width_2_for_pin_emoji()
    {
      UnicodeWidth.GetTextWidth("📍").ShouldBe(2);

      await Task.CompletedTask;
    }

    public static async Task Should_return_width_2_for_thermometer_emoji()
    {
      // 🌡️ is U+1F321 + U+FE0F
      UnicodeWidth.GetTextWidth("🌡️").ShouldBe(2);

      await Task.CompletedTask;
    }

    public static async Task Should_return_width_2_for_cloud_emoji()
    {
      // ☁️ is U+2601 + U+FE0F
      UnicodeWidth.GetTextWidth("☁️").ShouldBe(2);

      await Task.CompletedTask;
    }

    public static async Task Should_return_width_2_for_cjk_character()
    {
      UnicodeWidth.GetTextWidth("漢").ShouldBe(2);
      UnicodeWidth.GetTextWidth("字").ShouldBe(2);

      await Task.CompletedTask;
    }

    public static async Task Should_return_width_2_for_fullwidth_form()
    {
      // U+FF21 = fullwidth 'A'
      UnicodeWidth.GetTextWidth("\uFF21").ShouldBe(2);

      await Task.CompletedTask;
    }

    public static async Task Should_return_width_2_for_hangul()
    {
      UnicodeWidth.GetTextWidth("한").ShouldBe(2);

      await Task.CompletedTask;
    }

    public static async Task Should_return_width_2_for_flag_emoji()
    {
      // 🇺🇸 = U+1F1FA + U+1F1F8 (two regional indicators forming one grapheme cluster)
      UnicodeWidth.GetTextWidth("🇺🇸").ShouldBe(2);

      await Task.CompletedTask;
    }

    public static async Task Should_return_width_2_for_skin_tone_emoji()
    {
      // 👋🏽 = wave + Fitzpatrick modifier
      UnicodeWidth.GetTextWidth("👋🏽").ShouldBe(2);

      await Task.CompletedTask;
    }

    public static async Task Should_return_correct_width_for_mixed_string()
    {
      // "📍 Location" = emoji(2) + space(1) + "Location"(8) = 11
      UnicodeWidth.GetTextWidth("📍 Location").ShouldBe(11);

      await Task.CompletedTask;
    }

    public static async Task Should_return_correct_width_for_multiple_emoji()
    {
      // "📍🌡️☁️" = 2 + 2 + 2 = 6
      UnicodeWidth.GetTextWidth("📍🌡️☁️").ShouldBe(6);

      await Task.CompletedTask;
    }

    public static async Task Should_return_correct_width_for_cjk_string()
    {
      // 5 CJK characters = 10
      UnicodeWidth.GetTextWidth("漢字テスト").ShouldBe(10);

      await Task.CompletedTask;
    }

    public static async Task Should_return_width_1_for_box_drawing_characters()
    {
      UnicodeWidth.GetTextWidth("─").ShouldBe(1);
      UnicodeWidth.GetTextWidth("│").ShouldBe(1);
      UnicodeWidth.GetTextWidth("╭").ShouldBe(1);
      UnicodeWidth.GetTextWidth("╮").ShouldBe(1);

      await Task.CompletedTask;
    }

    public static async Task Should_return_rune_width_for_single_runes()
    {
      UnicodeWidth.GetRuneWidth(new Rune('A')).ShouldBe(1);
      UnicodeWidth.GetRuneWidth(new Rune('漢')).ShouldBe(2);
      UnicodeWidth.GetRuneWidth(new Rune(0x200D)).ShouldBe(0); // ZWJ
      UnicodeWidth.GetRuneWidth(new Rune(0xFE0F)).ShouldBe(0); // VS16

      await Task.CompletedTask;
    }

    public static async Task Should_return_width_1_for_nfd_combining_sequence()
    {
      // NFD "é" = "e" + U+0301 (combining acute accent) renders as one column
      UnicodeWidth.GetTextWidth("e\u0301").ShouldBe(1);

      await Task.CompletedTask;
    }

    public static async Task Should_return_width_1_for_narrow_smp_symbol_blocks()
    {
      // U+1F0A1 Playing Card Ace of Spades (EAW=N)
      UnicodeWidth.GetRuneWidth(new Rune(0x1F0A1)).ShouldBe(1);

      // U+1F70A Alchemical Symbol for Vinegar (EAW=N)
      UnicodeWidth.GetRuneWidth(new Rune(0x1F70A)).ShouldBe(1);

      await Task.CompletedTask;
    }

    public static async Task Should_return_width_2_for_mahjong_red_dragon()
    {
      // U+1F004 has Emoji_Presentation=Yes despite its narrow block
      UnicodeWidth.GetRuneWidth(new Rune(0x1F004)).ShouldBe(2);

      await Task.CompletedTask;
    }

    public static async Task Should_return_width_2_for_tangut_character()
    {
      // U+17000 Tangut Ideograph (EAW=W)
      UnicodeWidth.GetRuneWidth(new Rune(0x17000)).ShouldBe(2);

      await Task.CompletedTask;
    }
  }

} // namespace TimeWarp.Terminal.Tests.Core.UnicodeWidthTests
