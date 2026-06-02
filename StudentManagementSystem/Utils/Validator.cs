namespace StudentManagementSystem.Utils;

/// <summary>
/// Input validation helpers used across menu screens.
/// </summary>
public static class Validator
{
    public static bool IsValidStudentId(string id)
        => !string.IsNullOrWhiteSpace(id) && id.Length is >= 3 and <= 15;

    public static bool IsValidName(string name)
        => !string.IsNullOrWhiteSpace(name) && name.Trim().Length is >= 2 and <= 100;

    public static bool IsValidEmail(string email)
        => string.IsNullOrEmpty(email) || (email.Contains('@') && email.Contains('.'));

    public static bool IsValidPhone(string phone)
        => string.IsNullOrEmpty(phone) || (phone.All(c => char.IsDigit(c) || c is '+' or '-' or ' ') && phone.Length is >= 7 and <= 15);

    public static bool IsValidScore(double score) => score is >= 0 and <= 100;

    public static bool IsValidYear(int year) => year is >= 1990 and <= 2100;

    public static bool IsValidGpa(double gpa) => gpa is >= 0 and <= 4.0;

    // ── Parsing helpers ───────────────────────────────────────────────────────
    public static bool TryParseDouble(string input, out double value)
        => double.TryParse(input.Replace(',', '.'),
               System.Globalization.NumberStyles.Number,
               System.Globalization.CultureInfo.InvariantCulture,
               out value);

    public static bool TryParseInt(string input, out int value)
        => int.TryParse(input.Trim(), out value);

    public static bool TryParseDate(string input, out DateTime date)
        => DateTime.TryParseExact(input.Trim(), new[] { "dd/MM/yyyy", "yyyy-MM-dd", "dd-MM-yyyy" },
               System.Globalization.CultureInfo.InvariantCulture,
               System.Globalization.DateTimeStyles.None, out date);

    /// <summary>
    /// Reads an integer from the console within an inclusive range.
    /// Returns false if the input is not a valid integer in range.
    /// </summary>
    public static bool ReadInt(string prompt, int min, int max, out int result)
    {
        ColorConsole.Write($"  {prompt} ({min}-{max}): ", ConsoleColor.White);
        var input = Console.ReadLine() ?? string.Empty;
        if (TryParseInt(input, out result) && result >= min && result <= max) return true;
        ColorConsole.Error($"  Invalid input. Please enter a number between {min} and {max}.");
        return false;
    }

    /// <summary>Reads a score (0-100), returns false if invalid.</summary>
    public static bool ReadScore(string prompt, out double score)
    {
        ColorConsole.Write($"  {prompt} (0-100): ", ConsoleColor.White);
        var input = Console.ReadLine() ?? string.Empty;
        if (TryParseDouble(input, out score) && IsValidScore(score)) return true;
        ColorConsole.Error("  Invalid score. Must be between 0.00 and 100.00.");
        return false;
    }
}
