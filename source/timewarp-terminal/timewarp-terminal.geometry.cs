namespace TimeWarp.Terminal;

#region Purpose
// Cursor and window geometry on the production terminal.
#endregion

#region Design
// Same type as timewarp-terminal.cs. IOException from a redirected console falls back to a stable size.
#endregion

public sealed partial class TimeWarpTerminal
{
  /// <inheritdoc />
  public int CursorLeft
  {
    get
    {
      try
      {
        return Console.CursorLeft;
      }
      catch (IOException)
      {
        return 0;
      }
    }
    set
    {
      try
      {
        Console.CursorLeft = value;
      }
      catch (ArgumentOutOfRangeException)
      {
      }
      catch (IOException)
      {
      }
    }
  }

  /// <inheritdoc />
  public int CursorTop
  {
    get
    {
      try
      {
        return Console.CursorTop;
      }
      catch (IOException)
      {
        return 0;
      }
    }
    set
    {
      try
      {
        Console.CursorTop = value;
      }
      catch (ArgumentOutOfRangeException)
      {
      }
      catch (IOException)
      {
      }
    }
  }

  /// <inheritdoc />
  public bool CursorVisible
  {
    get
    {
      if (!OperatingSystem.IsWindows())
      {
        return true;
      }

      try
      {
        return Console.CursorVisible;
      }
      catch (IOException)
      {
        return true;
      }
    }
    set
    {
      // Console.CursorVisible's setter is supported on all platforms
      // (only the getter is Windows-only)
      try
      {
        Console.CursorVisible = value;
      }
      catch (IOException)
      {
      }
    }
  }

  /// <inheritdoc />
  public void SetCursorPosition(int left, int top)
  {
    try
    {
      Console.SetCursorPosition(left, top);
    }
    catch (ArgumentOutOfRangeException)
    {
      // Silently ignore invalid cursor positions
    }
    catch (IOException)
    {
      // Silently ignore I/O errors (e.g., redirected output)
    }
  }

  /// <inheritdoc />
  public (int Left, int Top) GetCursorPosition()
  {
    try
    {
      return Console.GetCursorPosition();
    }
    catch (IOException)
    {
      // Return default if console is redirected
      return (0, 0);
    }
  }

  /// <inheritdoc />
  public int WindowWidth
  {
    get
    {
      try
      {
        return Console.WindowWidth;
      }
      catch (IOException)
      {
        return 80;
      }
    }
  }

  /// <inheritdoc />
  public int WindowHeight
  {
    get
    {
      try
      {
        return Console.WindowHeight;
      }
      catch (IOException)
      {
        return 24;
      }
    }
  }

  /// <inheritdoc />
  public int BufferWidth
  {
    get
    {
      try
      {
        return Console.BufferWidth;
      }
      catch (IOException)
      {
        return 80;
      }
    }
  }

  /// <inheritdoc />
  public int BufferHeight
  {
    get
    {
      try
      {
        return Console.BufferHeight;
      }
      catch (IOException)
      {
        return 300;
      }
    }
  }

}
