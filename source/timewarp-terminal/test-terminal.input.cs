namespace TimeWarp.Terminal;

#region Purpose
// Scripted key input and capture-writer helpers for TestTerminal.
#endregion

#region Design
// Same type as test-terminal.cs. QueueKey, QueueKeys, and the capture tee stay together.
#endregion

public sealed partial class TestTerminal
{
  // ========== Test Helper Methods ==========

  /// <summary>
  /// Queues a single key press for the next <see cref="ReadKey()"/> call.
  /// </summary>
  /// <param name="key">The console key to queue.</param>
  /// <param name="shift">Whether Shift is pressed.</param>
  /// <param name="alt">Whether Alt is pressed.</param>
  /// <param name="ctrl">Whether Ctrl is pressed.</param>
  public void QueueKey(ConsoleKey key, bool shift = false, bool alt = false, bool ctrl = false)
  {
    char keyChar = key switch
    {
      ConsoleKey.Tab => '\t',
      ConsoleKey.Enter => '\r',
      ConsoleKey.Escape => '\u001b',
      ConsoleKey.Backspace => '\b',
      ConsoleKey.Spacebar => ' ',
      ConsoleKey.A or ConsoleKey.B or ConsoleKey.C or ConsoleKey.D or ConsoleKey.E or ConsoleKey.F
        or ConsoleKey.G or ConsoleKey.H or ConsoleKey.I or ConsoleKey.J or ConsoleKey.K or ConsoleKey.L
        or ConsoleKey.M or ConsoleKey.N or ConsoleKey.O or ConsoleKey.P or ConsoleKey.Q or ConsoleKey.R
        or ConsoleKey.S or ConsoleKey.T or ConsoleKey.U or ConsoleKey.V or ConsoleKey.W or ConsoleKey.X
        or ConsoleKey.Y or ConsoleKey.Z => (char)('a' + (key - ConsoleKey.A)),
      ConsoleKey.D0 or ConsoleKey.D1 or ConsoleKey.D2 or ConsoleKey.D3 or ConsoleKey.D4
        or ConsoleKey.D5 or ConsoleKey.D6 or ConsoleKey.D7 or ConsoleKey.D8
        or ConsoleKey.D9 => (char)('0' + (key - ConsoleKey.D0)),
      ConsoleKey.None or ConsoleKey.Clear or ConsoleKey.Pause
        or ConsoleKey.PageUp or ConsoleKey.PageDown or ConsoleKey.End or ConsoleKey.Home
        or ConsoleKey.LeftArrow or ConsoleKey.UpArrow or ConsoleKey.RightArrow or ConsoleKey.DownArrow
        or ConsoleKey.Select or ConsoleKey.Print or ConsoleKey.Execute or ConsoleKey.PrintScreen
        or ConsoleKey.Insert or ConsoleKey.Delete or ConsoleKey.Help
        or ConsoleKey.LeftWindows or ConsoleKey.RightWindows or ConsoleKey.Applications or ConsoleKey.Sleep
        or ConsoleKey.NumPad0 or ConsoleKey.NumPad1 or ConsoleKey.NumPad2 or ConsoleKey.NumPad3
        or ConsoleKey.NumPad4 or ConsoleKey.NumPad5 or ConsoleKey.NumPad6 or ConsoleKey.NumPad7
        or ConsoleKey.NumPad8 or ConsoleKey.NumPad9
        or ConsoleKey.Multiply or ConsoleKey.Add or ConsoleKey.Separator or ConsoleKey.Subtract
        or ConsoleKey.Decimal or ConsoleKey.Divide
        or ConsoleKey.F1 or ConsoleKey.F2 or ConsoleKey.F3 or ConsoleKey.F4 or ConsoleKey.F5
        or ConsoleKey.F6 or ConsoleKey.F7 or ConsoleKey.F8 or ConsoleKey.F9 or ConsoleKey.F10
        or ConsoleKey.F11 or ConsoleKey.F12 or ConsoleKey.F13 or ConsoleKey.F14 or ConsoleKey.F15
        or ConsoleKey.F16 or ConsoleKey.F17 or ConsoleKey.F18 or ConsoleKey.F19 or ConsoleKey.F20
        or ConsoleKey.F21 or ConsoleKey.F22 or ConsoleKey.F23 or ConsoleKey.F24
        or ConsoleKey.BrowserBack or ConsoleKey.BrowserForward or ConsoleKey.BrowserRefresh
        or ConsoleKey.BrowserStop or ConsoleKey.BrowserSearch or ConsoleKey.BrowserFavorites
        or ConsoleKey.BrowserHome
        or ConsoleKey.VolumeMute or ConsoleKey.VolumeDown or ConsoleKey.VolumeUp
        or ConsoleKey.MediaNext or ConsoleKey.MediaPrevious or ConsoleKey.MediaStop or ConsoleKey.MediaPlay
        or ConsoleKey.LaunchMail or ConsoleKey.LaunchMediaSelect or ConsoleKey.LaunchApp1 or ConsoleKey.LaunchApp2
        or ConsoleKey.Oem1 or ConsoleKey.OemPlus or ConsoleKey.OemComma or ConsoleKey.OemMinus
        or ConsoleKey.OemPeriod or ConsoleKey.Oem2 or ConsoleKey.Oem3 or ConsoleKey.Oem4
        or ConsoleKey.Oem5 or ConsoleKey.Oem6 or ConsoleKey.Oem7 or ConsoleKey.Oem8 or ConsoleKey.Oem102
        or ConsoleKey.Process or ConsoleKey.Packet or ConsoleKey.Attention
        or ConsoleKey.CrSel or ConsoleKey.ExSel or ConsoleKey.EraseEndOfFile
        or ConsoleKey.Play or ConsoleKey.Zoom or ConsoleKey.NoName or ConsoleKey.Pa1
        or ConsoleKey.OemClear => '\0',
      _ => '\0'
    };

    if (key is >= ConsoleKey.A and <= ConsoleKey.Z)
    {
      if (ctrl)
      {
        // Real terminals produce control characters for Ctrl+A through Ctrl+Z
        keyChar = (char)(key - ConsoleKey.A + 1);
      }
      else if (shift)
      {
        keyChar = char.ToUpperInvariant(keyChar);
      }
    }

    KeyQueue.Enqueue(new ConsoleKeyInfo(keyChar, key, shift, alt, ctrl));
  }

  /// <summary>
  /// Queues a <see cref="ConsoleKeyInfo"/> directly.
  /// </summary>
  /// <param name="keyInfo">The key info to queue.</param>
  public void QueueKeyInfo(ConsoleKeyInfo keyInfo)
    => KeyQueue.Enqueue(keyInfo);

  /// <summary>
  /// Queues a string of characters as individual key presses.
  /// </summary>
  /// <param name="text">The text to queue as key presses.</param>
  public void QueueKeys(string text)
  {
    ArgumentNullException.ThrowIfNull(text);
    foreach (char c in text)
    {
      ConsoleKey key = CharToConsoleKey(c);
      bool shift = char.IsAsciiLetterUpper(c);
      KeyQueue.Enqueue(new ConsoleKeyInfo(c, key, shift, false, false));
    }
  }

  /// <summary>
  /// Queues a string followed by Enter.
  /// </summary>
  /// <param name="text">The text to queue as a complete line.</param>
  public void QueueLine(string text)
  {
    QueueKeys(text);
    QueueKey(ConsoleKey.Enter);
  }

  /// <summary>
  /// Queues an arrow key press.
  /// </summary>
  /// <param name="direction">The arrow direction (UpArrow, DownArrow, LeftArrow, RightArrow).</param>
  public void QueueArrow(ConsoleKey direction)
  {
    if (direction is ConsoleKey.UpArrow or ConsoleKey.DownArrow
        or ConsoleKey.LeftArrow or ConsoleKey.RightArrow)
    {
      QueueKey(direction);
    }
  }

  /// <summary>
  /// Clears all captured output.
  /// </summary>
  public void ClearOutput()
  {
    _ = OutputWriter.GetStringBuilder().Clear();
    _ = ErrorWriter.GetStringBuilder().Clear();
  }

  /// <summary>
  /// Clears all queued keys.
  /// </summary>
  public void ClearKeys()
    => KeyQueue.Clear();

  /// <summary>
  /// Checks if the standard output contains the specified text.
  /// </summary>
  /// <param name="text">The text to search for.</param>
  /// <returns><c>true</c> if the output contains the text; otherwise, <c>false</c>.</returns>
  public bool OutputContains(string text)
    => Output.Contains(text, StringComparison.Ordinal);

  /// <summary>
  /// Checks if the error output contains the specified text.
  /// </summary>
  /// <param name="text">The text to search for.</param>
  /// <returns><c>true</c> if the error output contains the text; otherwise, <c>false</c>.</returns>
  public bool ErrorContains(string text)
    => ErrorOutput.Contains(text, StringComparison.Ordinal);

  /// <summary>
  /// Gets the output lines as an array.
  /// </summary>
  /// <returns>An array of output lines.</returns>
  public string[] GetOutputLines()
    => Output.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);

  /// <summary>
  /// Gets the number of keys currently in the queue.
  /// </summary>
  public int KeysInQueue => KeyQueue.Count;

  /// <summary>
  /// Disposes the resources used by this instance and clears the test context if this is the current terminal.
  /// </summary>
  public void Dispose()
  {
    if (Disposed)
    {
      return;
    }

    // Clear context if this is the current terminal (restores previous Terminal.Instance)
    if (TestTerminalContext.Current == this)
    {
      TestTerminalContext.ClearCurrent();
    }

    InputReader.Dispose();
    OutputWriter.Dispose();
    ErrorWriter.Dispose();

    // Dispose only the streams this instance created in its constructor.
    // Consumer-assigned Standard*Stream replacements are owned by the consumer and must not be disposed here.
    OwnedStandardInputStream.Dispose();
    OwnedStandardOutputStream.Dispose();
    OwnedStandardErrorStream.Dispose();
    Disposed = true;
  }

  private static void WriteTo(TextWriter destination, StringWriter capture, string value, bool newLine)
  {
    if (newLine)
    {
      destination.WriteLine(value);
    }
    else
    {
      destination.Write(value);
    }

    if (ReferenceEquals(destination, capture))
    {
      return;
    }

    if (newLine)
    {
      capture.WriteLine(value);
    }
    else
    {
      capture.Write(value);
    }
  }

  private static async Task WriteLineToAsync(TextWriter destination, StringWriter capture, string value)
  {
    await destination.WriteLineAsync(value).ConfigureAwait(false);
    if (!ReferenceEquals(destination, capture))
    {
      await capture.WriteLineAsync(value).ConfigureAwait(false);
    }
  }

  private static ConsoleKey CharToConsoleKey(char c) => c switch
  {
    >= 'a' and <= 'z' => ConsoleKey.A + (c - 'a'),
    >= 'A' and <= 'Z' => ConsoleKey.A + (c - 'A'),
    >= '0' and <= '9' => ConsoleKey.D0 + (c - '0'),
    ' ' => ConsoleKey.Spacebar,
    '\t' => ConsoleKey.Tab,
    '\r' or '\n' => ConsoleKey.Enter,
    '\b' => ConsoleKey.Backspace,
    '-' => ConsoleKey.OemMinus,
    '=' => ConsoleKey.OemPlus,
    '/' => ConsoleKey.Oem2,
    '.' => ConsoleKey.OemPeriod,
    ',' => ConsoleKey.OemComma,
    _ => ConsoleKey.NoName
  };

}
