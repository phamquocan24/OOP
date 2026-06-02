namespace StudentManagementSystem.Utils;

/// <summary>
/// Renders formatted ASCII tables to the console.
/// </summary>
public static class TablePrinter
{
    private const char TL = '╔', TR = '╗', BL = '╚', BR = '╝';
    private const char H  = '═', V  = '║';
    private const char ML = '╠', MR = '╣', MI = '╬', MT = '╦', MB = '╩';
    private const char SL = '╟', SR = '╢', SI = '╫';

    // ── Box drawing ──────────────────────────────────────────────────────────
    public static void DrawTopBorder(int[] widths)
    {
        Console.ForegroundColor = ConsoleColor.DarkCyan;
        Console.Write(TL);
        for (int i = 0; i < widths.Length; i++)
        {
            Console.Write(new string(H, widths[i] + 2));
            Console.Write(i < widths.Length - 1 ? MT : TR);
        }
        Console.WriteLine();
        Console.ResetColor();
    }

    public static void DrawHeaderSeparator(int[] widths)
    {
        Console.ForegroundColor = ConsoleColor.DarkCyan;
        Console.Write(ML);
        for (int i = 0; i < widths.Length; i++)
        {
            Console.Write(new string(H, widths[i] + 2));
            Console.Write(i < widths.Length - 1 ? MI : MR);
        }
        Console.WriteLine();
        Console.ResetColor();
    }

    public static void DrawRowSeparator(int[] widths)
    {
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.Write(SL);
        for (int i = 0; i < widths.Length; i++)
        {
            Console.Write(new string('─', widths[i] + 2));
            Console.Write(i < widths.Length - 1 ? SI : SR);
        }
        Console.WriteLine();
        Console.ResetColor();
    }

    public static void DrawBottomBorder(int[] widths)
    {
        Console.ForegroundColor = ConsoleColor.DarkCyan;
        Console.Write(BL);
        for (int i = 0; i < widths.Length; i++)
        {
            Console.Write(new string(H, widths[i] + 2));
            Console.Write(i < widths.Length - 1 ? MB : BR);
        }
        Console.WriteLine();
        Console.ResetColor();
    }

    // ── Row drawing ──────────────────────────────────────────────────────────
    public static void DrawHeaderRow(string[] headers, int[] widths)
    {
        Console.ForegroundColor = ConsoleColor.DarkCyan;
        Console.Write(V);
        Console.ResetColor();

        for (int i = 0; i < headers.Length; i++)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write($" {headers[i].PadRight(widths[i])} ");
            Console.ResetColor();
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.Write(V);
            Console.ResetColor();
        }
        Console.WriteLine();
    }

    public static void DrawDataRow(string[] cells, int[] widths,
                                   ConsoleColor rowColor = ConsoleColor.White,
                                   bool[] rightAlign = null!)
    {
        Console.ForegroundColor = ConsoleColor.DarkCyan;
        Console.Write(V);
        Console.ResetColor();

        for (int i = 0; i < cells.Length; i++)
        {
            bool ra = rightAlign != null && i < rightAlign.Length && rightAlign[i];
            string cell = cells[i].Length > widths[i]
                ? cells[i][..(widths[i] - 1)] + "…"
                : cells[i];
            string padded = ra ? cell.PadLeft(widths[i]) : cell.PadRight(widths[i]);

            Console.ForegroundColor = rowColor;
            Console.Write($" {padded} ");
            Console.ResetColor();
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.Write(V);
            Console.ResetColor();
        }
        Console.WriteLine();
    }

    // ── High-level Print ─────────────────────────────────────────────────────
    /// <summary>
    /// Renders a complete table with header and data rows.
    /// </summary>
    public static void Print(string[] headers, List<string[]> rows,
                              int[]? widths = null, bool[]? rightAlign = null,
                              Func<int, ConsoleColor>? rowColorSelector = null)
    {
        widths ??= headers.Select((h, i) =>
            Math.Max(h.Length, rows.Count > 0 ? rows.Max(r => i < r.Length ? r[i].Length : 0) : 0)
        ).ToArray();

        DrawTopBorder(widths);
        DrawHeaderRow(headers, widths);
        DrawHeaderSeparator(widths);

        for (int ri = 0; ri < rows.Count; ri++)
        {
            var color = rowColorSelector?.Invoke(ri) ?? ConsoleColor.White;
            DrawDataRow(rows[ri], widths, color, rightAlign!);
            if (ri < rows.Count - 1)
                DrawRowSeparator(widths);
        }

        DrawBottomBorder(widths);
        Console.WriteLine();
    }
}
