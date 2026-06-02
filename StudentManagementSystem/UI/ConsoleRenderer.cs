using StudentManagementSystem.Utils;

namespace StudentManagementSystem.UI;

/// <summary>
/// Shared console rendering helpers used by all menu screens.
/// </summary>
public static class ConsoleRenderer
{
    public const string AppName    = "Student Management System";
    public const string AppVersion = "v1.0.0";
    public const string AppAuthor  = "OOP Project — C#";

    // ── Layout ────────────────────────────────────────────────────────────────
    public static void ClearScreen() => Console.Clear();

    public static void PrintBanner()
    {
        Console.Clear();
        var width = 56;
        var topLine    = $"╔{new string('═', width)}╗";
        var botLine    = $"╚{new string('═', width)}╝";
        var midLine    = $"╠{new string('═', width)}╣";
        var emptyLine  = $"║{new string(' ', width)}║";

        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine($"\n  {topLine}");
        Console.WriteLine($"  ║{"  🎓  STUDENT MANAGEMENT SYSTEM  🎓".PadLeft(36).PadRight(width)}║");
        Console.WriteLine($"  ║{"Built with C# & Object-Oriented Programming".PadLeft(43).PadRight(width)}║");
        Console.WriteLine($"  ║{$"Version {AppVersion}".PadLeft(19).PadRight(width)}║");
        Console.WriteLine($"  {midLine}");
        Console.ResetColor();
    }

    public static void PrintMenuTitle(string title, string breadcrumb = "")
    {
        Console.Clear();
        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine($"  ╔{"═".PadRight(54, '═')}╗");
        Console.ForegroundColor = ConsoleColor.White;
        var centered = title.PadLeft((54 + title.Length) / 2).PadRight(54);
        Console.WriteLine($"  ║ {centered}║");
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine($"  ╚{"═".PadRight(54, '═')}╝");
        Console.ResetColor();

        if (!string.IsNullOrEmpty(breadcrumb))
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine($"  📍 {breadcrumb}");
            Console.ResetColor();
        }
        Console.WriteLine();
    }

    public static void PrintSectionHeader(string title)
    {
        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.DarkCyan;
        Console.Write("  ── ");
        Console.ForegroundColor = ConsoleColor.White;
        Console.Write(title);
        Console.ForegroundColor = ConsoleColor.DarkCyan;
        Console.WriteLine($" {"─".PadRight(Math.Max(0, 44 - title.Length), '─')}");
        Console.ResetColor();
    }

    // ── Menu option ───────────────────────────────────────────────────────────
    public static void PrintMenuOption(string key, string label, string? desc = null)
    {
        Console.Write("  ");
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.Write($"[{key}]");
        Console.ForegroundColor = ConsoleColor.White;
        Console.Write($" {label}");
        if (desc != null)
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write($"  — {desc}");
        }
        Console.ResetColor();
        Console.WriteLine();
    }

    public static void PrintMenuPrompt()
    {
        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.White;
        Console.Write("  ▶ Enter your choice: ");
        Console.ForegroundColor = ConsoleColor.Yellow;
    }

    // ── Status badges ─────────────────────────────────────────────────────────
    public static string GpaColor(double gpa) => gpa switch
    {
        >= 3.6 => "Excellent ⭐",
        >= 3.2 => "Good ✅",
        >= 2.5 => "Average 🔵",
        >= 2.0 => "Below Avg ⚠️",
        >= 1.0 => "Poor ❌",
        _      => "Expelled 🚫"
    };

    public static ConsoleColor GpaConsoleColor(double gpa) => gpa switch
    {
        >= 3.6 => ConsoleColor.Green,
        >= 3.2 => ConsoleColor.Cyan,
        >= 2.5 => ConsoleColor.White,
        >= 2.0 => ConsoleColor.Yellow,
        _      => ConsoleColor.Red
    };

    // ── Progress bar ──────────────────────────────────────────────────────────
    public static string ProgressBar(double value, double max = 4.0, int barWidth = 20)
    {
        int filled = (int)Math.Round(value / max * barWidth);
        filled = Math.Clamp(filled, 0, barWidth);
        return $"[{"█".PadRight(filled, '█').PadRight(barWidth, '░')}]";
    }

    // ── Dividers ──────────────────────────────────────────────────────────────
    public static void PrintDivider()
    {
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine($"  {"─".PadRight(55, '─')}");
        Console.ResetColor();
    }

    // ── Loading ───────────────────────────────────────────────────────────────
    public static void PrintSuccess(string msg)
    {
        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"  ✔  {msg}");
        Console.ResetColor();
    }

    public static void PrintError(string msg)
    {
        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"  ✘  {msg}");
        Console.ResetColor();
    }

    public static void PrintWarning(string msg)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"  ⚠  {msg}");
        Console.ResetColor();
    }

    public static void PrintInfo(string msg)
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine($"  ℹ  {msg}");
        Console.ResetColor();
    }
}
