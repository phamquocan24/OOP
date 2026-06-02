namespace StudentManagementSystem.Utils;

/// <summary>
/// Provides colored console output helpers using ANSI-style Console colors.
/// </summary>
public static class ColorConsole
{
    public static void Write(string text, ConsoleColor color)
    {
        Console.ForegroundColor = color;
        Console.Write(text);
        Console.ResetColor();
    }

    public static void WriteLine(string text, ConsoleColor color)
    {
        Console.ForegroundColor = color;
        Console.WriteLine(text);
        Console.ResetColor();
    }

    public static void Success(string text)  => WriteLine(text, ConsoleColor.Green);
    public static void Error(string text)    => WriteLine(text, ConsoleColor.Red);
    public static void Warning(string text)  => WriteLine(text, ConsoleColor.Yellow);
    public static void Info(string text)     => WriteLine(text, ConsoleColor.Cyan);
    public static void Muted(string text)    => WriteLine(text, ConsoleColor.DarkGray);
    public static void Highlight(string text)=> WriteLine(text, ConsoleColor.Magenta);

    public static void Header(string text)
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine(text);
        Console.ResetColor();
    }

    /// <summary>Writes a prompt label and reads user input.</summary>
    public static string Prompt(string label)
    {
        Console.ForegroundColor = ConsoleColor.White;
        Console.Write($"  {label}: ");
        Console.ResetColor();
        return Console.ReadLine()?.Trim() ?? string.Empty;
    }

    /// <summary>Writes a prompt and returns true if user presses Y/y.</summary>
    public static bool Confirm(string question)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.Write($"  {question} (y/n): ");
        Console.ResetColor();
        var input = Console.ReadLine()?.Trim().ToLower();
        return input == "y" || input == "yes";
    }

    public static void PressAnyKey(string msg = "Press any key to continue...")
    {
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.Write($"\n  {msg}");
        Console.ResetColor();
        Console.ReadKey(true);
    }
}
