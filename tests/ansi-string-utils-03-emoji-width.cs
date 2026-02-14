#!/usr/bin/dotnet --
#:project ../source/timewarp-terminal/timewarp-terminal.csproj

// Test AnsiStringUtils with emoji and wide characters

#if !JARIBU_MULTI
return await RunAllTests();
#endif

namespace TimeWarp.Terminal.Tests.Core.AnsiStringUtilsEmoji
{

[TestTag("Widgets")]
public class AnsiStringUtilsEmojiTests
{
  [ModuleInitializer]
  internal static void Register() => RegisterTests<AnsiStringUtilsEmojiTests>();

  public static async Task Should_get_visible_length_for_emoji()
  {
    // "📍 Location" = emoji(2) + space(1) + "Location"(8) = 11
    int length = AnsiStringUtils.GetVisibleLength("📍 Location");

    length.ShouldBe(11);

    await Task.CompletedTask;
  }

  public static async Task Should_get_visible_length_for_styled_emoji()
  {
    string styled = "\x1b[31m📍 Location\x1b[0m";

    int length = AnsiStringUtils.GetVisibleLength(styled);

    length.ShouldBe(11);

    await Task.CompletedTask;
  }

  public static async Task Should_get_visible_length_for_weather_emoji_string()
  {
    // "🌡️ Temperature" = emoji(2) + space(1) + "Temperature"(11) = 14
    int length = AnsiStringUtils.GetVisibleLength("🌡️ Temperature");

    length.ShouldBe(14);

    await Task.CompletedTask;
  }

  public static async Task Should_pad_right_correctly_with_emoji()
  {
    string text = "📍 Pin";
    // Display width: emoji(2) + space(1) + "Pin"(3) = 6

    string padded = AnsiStringUtils.PadRightVisible(text, 15);

    // Should add 9 spaces to reach width 15
    AnsiStringUtils.GetVisibleLength(padded).ShouldBe(15);
    padded.ShouldEndWith("         "); // 9 spaces

    await Task.CompletedTask;
  }

  public static async Task Should_pad_left_correctly_with_emoji()
  {
    string text = "📍 Pin";

    string padded = AnsiStringUtils.PadLeftVisible(text, 15);

    AnsiStringUtils.GetVisibleLength(padded).ShouldBe(15);
    padded.ShouldStartWith("         "); // 9 spaces

    await Task.CompletedTask;
  }

  public static async Task Should_center_correctly_with_emoji()
  {
    string text = "📍 Pin";
    // Display width 6, target 16 => 5 left, 5 right

    string centered = AnsiStringUtils.CenterVisible(text, 16);

    AnsiStringUtils.GetVisibleLength(centered).ShouldBe(16);

    await Task.CompletedTask;
  }

  public static async Task Should_get_visible_length_for_plain_ascii_unchanged()
  {
    // Regression: ASCII strings must return same values as before
    AnsiStringUtils.GetVisibleLength("Hello World").ShouldBe(11);
    AnsiStringUtils.GetVisibleLength("Error").ShouldBe(5);
    AnsiStringUtils.GetVisibleLength("").ShouldBe(0);
    AnsiStringUtils.GetVisibleLength(null).ShouldBe(0);

    await Task.CompletedTask;
  }
}

} // namespace TimeWarp.Terminal.Tests.Core.AnsiStringUtilsEmoji
