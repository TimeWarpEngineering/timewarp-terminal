#!/usr/bin/env -S dotnet --
#:project $(SourceDirectory)timewarp-terminal/timewarp-terminal.csproj

// Test Panel widget basic functionality

#if !JARIBU_MULTI
return await RunAllTests();
#endif

namespace TimeWarp.Terminal.Tests.Core.PanelWidget
{

  [TestTag("Widgets")]
  public class PanelWidgetBasicTests
  {
    [ModuleInitializer]
    internal static void Register() => RegisterTests<PanelWidgetBasicTests>();

    public static async Task Should_render_simple_panel_with_content()
    {
      // Arrange
      Panel panel = new PanelBuilder().Content("Hello World").Build();

      // Act
      string[] lines = panel.Render(40);

      // Assert
      lines.Length.ShouldBe(3); // top border + content + bottom border
      lines[0].ShouldContain("╭"); // rounded top-left
      lines[0].ShouldContain("╮"); // rounded top-right
      lines[1].ShouldContain("Hello World");
      lines[1].ShouldContain("│"); // vertical border
      lines[2].ShouldContain("╰"); // rounded bottom-left
      lines[2].ShouldContain("╯"); // rounded bottom-right

      await Task.CompletedTask;
    }

    public static async Task Should_render_panel_with_header()
    {
      // Arrange
      Panel panel = new PanelBuilder().Header("Notice").Content("Important info").Build();

      // Act
      string[] lines = panel.Render(40);

      // Assert
      lines[0].ShouldContain("Notice");
      lines[1].ShouldContain("Important info");

      await Task.CompletedTask;
    }

    public static async Task Should_render_panel_with_square_border()
    {
      // Arrange
      Panel panel = new PanelBuilder().Content("Test").Border(BorderStyle.Square).Build();

      // Act
      string[] lines = panel.Render(20);

      // Assert
      lines[0].ShouldContain("┌"); // square top-left
      lines[0].ShouldContain("┐"); // square top-right
      lines[2].ShouldContain("└"); // square bottom-left
      lines[2].ShouldContain("┘"); // square bottom-right

      await Task.CompletedTask;
    }

    public static async Task Should_render_panel_with_double_border()
    {
      // Arrange
      Panel panel = new PanelBuilder().Content("Test").Border(BorderStyle.Doubled).Build();

      // Act
      string[] lines = panel.Render(20);

      // Assert
      lines[0].ShouldContain("╔"); // double top-left
      lines[0].ShouldContain("╗"); // double top-right
      lines[0].ShouldContain("═"); // double horizontal
      lines[1].ShouldContain("║"); // double vertical
      lines[2].ShouldContain("╚"); // double bottom-left
      lines[2].ShouldContain("╝"); // double bottom-right

      await Task.CompletedTask;
    }

    public static async Task Should_render_panel_with_heavy_border()
    {
      // Arrange
      Panel panel = new PanelBuilder().Content("Test").Border(BorderStyle.Heavy).Build();

      // Act
      string[] lines = panel.Render(20);

      // Assert
      lines[0].ShouldContain("┏"); // heavy top-left
      lines[0].ShouldContain("┓"); // heavy top-right
      lines[0].ShouldContain("━"); // heavy horizontal
      lines[1].ShouldContain("┃"); // heavy vertical
      lines[2].ShouldContain("┗"); // heavy bottom-left
      lines[2].ShouldContain("┛"); // heavy bottom-right

      await Task.CompletedTask;
    }

    public static async Task Should_render_multiline_content()
    {
      // Arrange
      Panel panel = new PanelBuilder().Content("Line 1\nLine 2\nLine 3").Build();

      // Act
      string[] lines = panel.Render(40);

      // Assert
      lines.Length.ShouldBe(5); // top + 3 content lines + bottom
      lines[1].ShouldContain("Line 1");
      lines[2].ShouldContain("Line 2");
      lines[3].ShouldContain("Line 3");

      await Task.CompletedTask;
    }

    public static async Task Should_render_panel_with_vertical_padding()
    {
      // Arrange
      Panel panel = new PanelBuilder().Content("Content").PaddingVertical(1).Build();

      // Act
      string[] lines = panel.Render(40);

      // Assert
      // top + 1 pad + content + 1 pad + bottom = 5 lines
      lines.Length.ShouldBe(5);
      lines[1].ShouldNotContain("Content"); // padding line
      lines[2].ShouldContain("Content");
      lines[3].ShouldNotContain("Content"); // padding line

      await Task.CompletedTask;
    }

    public static async Task Should_render_panel_with_horizontal_padding()
    {
      // Arrange
      Panel panel = new PanelBuilder().Content("Hi").PaddingHorizontal(3).Build();

      // Act
      string[] lines = panel.Render(40);

      // Assert
      // Content line should have 3 spaces before and after content
      lines[1].ShouldContain("│   Hi"); // 3 spaces of padding

      await Task.CompletedTask;
    }

    public static async Task Should_render_panel_with_fixed_width()
    {
      // Arrange
      Panel panel = new PanelBuilder().Content("Test").Width(20).Build();

      // Act
      string[] lines = panel.Render(80); // terminal width should be ignored

      // Assert
      TimeWarp.Terminal.AnsiStringUtils.GetVisibleLength(lines[0]).ShouldBe(20);
      TimeWarp.Terminal.AnsiStringUtils.GetVisibleLength(lines[2]).ShouldBe(20);

      await Task.CompletedTask;
    }

    public static async Task Should_render_panel_with_border_color()
    {
      // Arrange
      Panel panel = new PanelBuilder().Content("Test").BorderColor(AnsiColors.Cyan).Build();

      // Act
      string[] lines = panel.Render(30);

      // Assert
      lines[0].ShouldContain(AnsiColors.Cyan);
      lines[0].ShouldContain(AnsiColors.Reset);
      lines[1].ShouldContain(AnsiColors.Cyan);
      lines[2].ShouldContain(AnsiColors.Cyan);

      await Task.CompletedTask;
    }

    public static async Task Should_render_panel_without_border_when_style_is_none()
    {
      // Arrange
      Panel panel = new PanelBuilder().Content("Line 1\nLine 2").Border(BorderStyle.None).Build();

      // Act
      string[] lines = panel.Render(40);

      // Assert
      lines.Length.ShouldBe(2);
      lines[0].ShouldBe("Line 1");
      lines[1].ShouldBe("Line 2");

      await Task.CompletedTask;
    }

    public static async Task Should_handle_empty_content()
    {
      // Arrange
      Panel panel = new PanelBuilder().Content("").Build();

      // Act
      string[] lines = panel.Render(20);

      // Assert
      lines.Length.ShouldBe(3); // top + empty content row + bottom

      await Task.CompletedTask;
    }

    public static async Task Should_handle_null_content()
    {
      // Arrange
      Panel panel = new PanelBuilder().Content(null).Build();

      // Act
      string[] lines = panel.Render(20);

      // Assert
      lines.Length.ShouldBe(3); // top + empty content row + bottom

      await Task.CompletedTask;
    }

    public static async Task Should_widen_panel_so_default_padding_keeps_uniform_line_width()
    {
      // Regression: Width(4) with default pad 1 used to force contentAreaWidth=1 without
      // widening, so content rows were wider than the top/bottom borders.
      // Arrange
      Panel panel = new PanelBuilder().Content("X").Width(4).Build();

      // Act
      string[] lines = panel.Render(80);

      // Assert
      int expectedWidth = AnsiStringUtils.GetVisibleLength(lines[0]);
      expectedWidth.ShouldBeGreaterThanOrEqualTo(5);
      foreach (string line in lines)
      {
        AnsiStringUtils.GetVisibleLength(line).ShouldBe(expectedWidth);
      }

      lines[1].ShouldEndWith("│");

      await Task.CompletedTask;
    }

    public static async Task Should_widen_panel_for_larger_horizontal_padding()
    {
      // Arrange - pad 2 → min width 2 + 2*2 + 1 = 7
      Panel panel = new PanelBuilder().Content("X").Width(4).PaddingHorizontal(2).Build();

      // Act
      string[] lines = panel.Render(80);

      // Assert
      int expectedWidth = AnsiStringUtils.GetVisibleLength(lines[0]);
      expectedWidth.ShouldBeGreaterThanOrEqualTo(7);
      foreach (string line in lines)
      {
        AnsiStringUtils.GetVisibleLength(line).ShouldBe(expectedWidth);
      }

      lines[1].ShouldEndWith("│");

      await Task.CompletedTask;
    }

    public static async Task Should_normalize_crlf_content_with_border()
    {
      // Arrange
      Panel panel = new PanelBuilder().Content("Line1\r\nLine2").Build();

      // Act
      string[] lines = panel.Render(40);

      // Assert
      lines.Length.ShouldBe(4); // top + 2 content + bottom
      lines[1].ShouldContain("Line1");
      lines[2].ShouldContain("Line2");
      foreach (string line in lines)
      {
        line.ShouldNotContain("\r");
      }

      lines[1].ShouldStartWith("│");
      lines[2].ShouldStartWith("│");

      await Task.CompletedTask;
    }

    public static async Task Should_normalize_crlf_content_without_border()
    {
      // Arrange
      Panel panel = new PanelBuilder()
        .Content("Line1\r\nLine2")
        .Border(BorderStyle.None)
        .Build();

      // Act
      string[] lines = panel.Render(40);

      // Assert
      lines.Length.ShouldBe(2);
      lines[0].ShouldBe("Line1");
      lines[1].ShouldBe("Line2");

      await Task.CompletedTask;
    }

    public static async Task Should_build_independent_snapshots()
    {
      // Regression: Build() returned the live panel instance, so building twice
      // returned the same object and post-Build builder calls mutated it.
      // Arrange
      PanelBuilder builder = new PanelBuilder()
        .Header("First")
        .Content("A")
        .Width(20);

      // Act
      Panel first = builder.Build();

      _ = builder
        .Header("Second")
        .Content("B")
        .Width(30);

      Panel second = builder.Build();

      // Assert
      ReferenceEquals(first, second).ShouldBeFalse();
      first.Header.ShouldBe("First");
      first.Content.ShouldBe("A");
      first.Width.ShouldBe(20);
      second.Header.ShouldBe("Second");
      second.Content.ShouldBe("B");
      second.Width.ShouldBe(30);

      await Task.CompletedTask;
    }

    public static async Task Should_clamp_negative_padding_without_throwing()
    {
      // Arrange - builder setters clamp; public setters still need Render-time clamp
      Panel panel = new PanelBuilder()
        .Content("Hi")
        .Padding(-3, -2)
        .Width(20)
        .Build();
      panel.PaddingHorizontal = -5;
      panel.PaddingVertical = -4;

      // Act
      string[] lines = panel.Render(80);

      // Assert - negative padding clamps to 0; fixed width still honored
      foreach (string line in lines)
      {
        AnsiStringUtils.GetVisibleLength(line).ShouldBe(20);
      }

      await Task.CompletedTask;
    }
  }

} // namespace TimeWarp.Terminal.Tests.Core.PanelWidget
