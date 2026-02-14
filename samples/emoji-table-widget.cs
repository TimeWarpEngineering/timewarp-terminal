#!/usr/bin/dotnet --
#:project ../source/timewarp-terminal/timewarp-terminal.csproj

// Demonstrates emoji alignment fix in table borders
using TimeWarp.Terminal;

TimeWarpTerminal terminal = new();

terminal
  .WriteRule("🌤️ Weather Report", style: LineStyle.Doubled)
  .WriteTable(t => t
    .AddColumn("Info")
    .AddColumn("Value")
    .AddRow("📍 Location", "Dallas, United States")
    .AddRow("🌡️ Temperature", "25.2°C (77.4°F)")
    .AddRow("☁️ Condition", "Overcast")
    .Border(BorderStyle.Rounded))
  .WriteLine()
  .WritePanel(p => p
    .Header("💠 Status Dashboard")
    .Content("All systems operational ✅")
    .Border(BorderStyle.Rounded))
  .WriteLine()
  .WriteTable(t => t
    .AddColumn("📍 City")
    .AddColumn("🌡️ Temp")
    .AddColumn("☁️ Sky")
    .AddRow("Dallas", "25°C", "Overcast")
    .AddRow("London", "12°C", "Rainy 🌧️")
    .AddRow("Tokyo", "18°C", "Sunny ☀️")
    .Border(BorderStyle.Rounded));
