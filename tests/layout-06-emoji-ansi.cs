#!/usr/bin/env -S dotnet --
#:project $(SourceDirectory)timewarp-terminal-layout/timewarp-terminal-layout.csproj

// Test emoji + ANSI content inside layout items

#if !JARIBU_MULTI
return await RunAllTests();
#endif

namespace TimeWarp.Terminal.Tests.FlexLayout.EmojiAnsi
{

  [TestTag("Layout")]
  public class LayoutEmojiAnsiTests
  {
    [ModuleInitializer]
    internal static void Register() => RegisterTests<LayoutEmojiAnsiTests>();

    public static async Task Should_preserve_emoji_and_ansi_alignment()
    {
      Layout layout = new LayoutBuilder()
        .Direction(FlexDirection.Row)
        .Gap(2)
        .Item(i => i.Grow(1), panel => panel
          .Header("🚀 Ship")
          .Content("Status: " + "OK".Green())
          .Border(BorderStyle.Rounded))
        .Item(i => i.Grow(1), "Plain ✅")
        .Build();

      string[] lines = layout.Render(60);
      string joined = string.Join('\n', lines);

      joined.ShouldContain("🚀");
      joined.ShouldContain("OK");
      joined.ShouldContain(AnsiColors.Green);
      joined.ShouldContain("✅");
      joined.ShouldContain("╭");
      joined.ShouldContain("╮");

      foreach (string line in lines)
      {
        // Visible width should not exceed container; borders stay intact as full lines.
        AnsiStringUtils.GetVisibleLength(line).ShouldBeLessThanOrEqualTo(60);
      }

      await Task.CompletedTask;
    }
  }

} // namespace TimeWarp.Terminal.Tests.FlexLayout.EmojiAnsi
