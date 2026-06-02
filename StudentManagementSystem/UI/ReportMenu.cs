using StudentManagementSystem.Models;
using StudentManagementSystem.Services;
using StudentManagementSystem.Utils;

namespace StudentManagementSystem.UI;

/// <summary>Reports and statistics menu.</summary>
public class ReportMenu
{
    private readonly ReportService  _reportService;
    private readonly StudentService _studentService;
    private readonly FileService    _fileService;
    private readonly List<Faculty>  _faculties;

    public ReportMenu(ReportService reportService, StudentService studentService,
                      FileService fileService, List<Faculty> faculties)
    {
        _reportService  = reportService;
        _studentService = studentService;
        _fileService    = fileService;
        _faculties      = faculties;
    }

    public void Show()
    {
        while (true)
        {
            ConsoleRenderer.PrintMenuTitle("Reports & Statistics", "Main > Reports");
            ConsoleRenderer.PrintMenuOption("1", "Dashboard Overview");
            ConsoleRenderer.PrintMenuOption("2", "Top Students Ranking");
            ConsoleRenderer.PrintMenuOption("3", "GPA Distribution");
            ConsoleRenderer.PrintMenuOption("4", "Students by Faculty");
            ConsoleRenderer.PrintMenuOption("5", "Students by Gender");
            ConsoleRenderer.PrintMenuOption("6", "Enrollment by Year");
            ConsoleRenderer.PrintMenuOption("7", "Subject Pass Rates");
            ConsoleRenderer.PrintMenuOption("8", "Export Students to CSV");
            ConsoleRenderer.PrintMenuOption("0", "Back");
            ConsoleRenderer.PrintMenuPrompt();

            var choice = Console.ReadLine()?.Trim() ?? "";
            Console.ResetColor();

            switch (choice)
            {
                case "1": Dashboard();           break;
                case "2": TopStudents();         break;
                case "3": GpaDistribution();     break;
                case "4": ByFaculty();           break;
                case "5": ByGender();            break;
                case "6": ByEnrollYear();        break;
                case "7": SubjectPassRates();    break;
                case "8": ExportCsv();           break;
                case "0": return;
                default:  ConsoleRenderer.PrintWarning("Invalid."); Thread.Sleep(900); break;
            }
        }
    }

    private void Dashboard()
    {
        ConsoleRenderer.PrintMenuTitle("Dashboard Overview", "Main > Reports > Dashboard");
        var (total, subjects, enrollments, avgGpa, highGpa, lowGpa) = _reportService.GetOverallStats();

        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("  ╔═══════════════════════════════════════════════════╗");
        Console.WriteLine("  ║              SYSTEM DASHBOARD                   ║");
        Console.WriteLine("  ╠═══════════════════════════════════════════════════╣");
        Console.ResetColor();

        PrintStat("Total Students",   total.ToString(),        ConsoleColor.White);
        PrintStat("Total Subjects",   subjects.ToString(),     ConsoleColor.White);
        PrintStat("Total Enrollments",enrollments.ToString(),  ConsoleColor.White);
        PrintStat("Average GPA",      $"{avgGpa:F2} / 4.00",  ConsoleRenderer.GpaConsoleColor(avgGpa));
        PrintStat("Highest GPA",      $"{highGpa:F2}",         ConsoleColor.Green);
        PrintStat("Lowest GPA",       $"{lowGpa:F2}",          ConsoleRenderer.GpaConsoleColor(lowGpa));

        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("  ╚═══════════════════════════════════════════════════╝");
        Console.ResetColor();

        Console.WriteLine();
        // GPA bar
        Console.Write("  Average GPA  ");
        Console.ForegroundColor = ConsoleRenderer.GpaConsoleColor(avgGpa);
        Console.WriteLine($"{ConsoleRenderer.ProgressBar(avgGpa)}  {avgGpa:F2}");
        Console.ResetColor();

        ColorConsole.PressAnyKey();
    }

    private static void PrintStat(string label, string value, ConsoleColor vc)
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.Write("  ║  ");
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.Write(label.PadRight(22));
        Console.ForegroundColor = vc;
        Console.Write(value.PadLeft(24));
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("  ║");
        Console.ResetColor();
    }

    private void TopStudents()
    {
        ConsoleRenderer.PrintMenuTitle("Top Students Ranking", "Main > Reports > Ranking");
        if (!Validator.ReadInt("Show top N students", 1, 50, out var n)) n = 10;

        var top = _reportService.GetTopStudents(n).ToList();
        Console.WriteLine();
        ConsoleRenderer.PrintInfo($"🏆 Top {top.Count} Students by GPA");
        Console.WriteLine();

        var headers = new[] { "Rank", "Student ID", "Full Name", "Major", "GPA", "Standing" };
        var widths  = new[] { 5, 12, 24, 22, 6, 14 };
        var rows    = top.Select((s, i) =>
        {
            string rank = i switch { 0 => "🥇 1", 1 => "🥈 2", 2 => "🥉 3", _ => $"   {i+1}" };
            return new[] { rank, s.StudentId, s.FullName, s.Major, s.GPA.ToString("F2"), s.AcademicStatus.ToString() };
        }).ToList();

        TablePrinter.Print(headers, rows, widths,
            rightAlign: new[] { true, false, false, false, true, false },
            rowColorSelector: i => ConsoleRenderer.GpaConsoleColor(top[i].GPA));

        ColorConsole.PressAnyKey();
    }

    private void GpaDistribution()
    {
        ConsoleRenderer.PrintMenuTitle("GPA Distribution", "Main > Reports > GPA Dist.");
        var dist = _reportService.GetDistributionByAcademicStatus();
        var total = _studentService.TotalCount;

        Console.WriteLine();
        ConsoleRenderer.PrintSectionHeader("Students by Academic Standing");
        Console.WriteLine();

        var statuses = Enum.GetValues<AcademicStatus>();
        foreach (var status in statuses)
        {
            int count = dist.GetValueOrDefault(status, 0);
            double pct = total > 0 ? (double)count / total * 100 : 0;
            int barLen = (int)Math.Round(pct / 100 * 30);

            Console.Write($"  {status,-14} ");
            Console.ForegroundColor = ConsoleRenderer.GpaConsoleColor(status switch
            {
                AcademicStatus.Excellent   => 3.8,
                AcademicStatus.Good        => 3.3,
                AcademicStatus.Average     => 2.7,
                AcademicStatus.BelowAverage=> 2.1,
                AcademicStatus.Poor        => 1.2,
                _                          => 0.5
            });
            Console.Write("█".PadRight(barLen, '█').PadRight(30, '░'));
            Console.ResetColor();
            Console.WriteLine($"  {count,3} ({pct:F1}%)");
        }

        ColorConsole.PressAnyKey();
    }

    private void ByFaculty()
    {
        ConsoleRenderer.PrintMenuTitle("Students by Faculty", "Main > Reports > Faculty");
        var dist  = _reportService.GetDistributionByFaculty();
        var total = _studentService.TotalCount;
        Console.WriteLine();

        var headers = new[] { "Faculty ID", "Faculty Name", "Students", "%" };
        var widths  = new[] { 10, 30, 10, 8 };
        var rows    = dist.Select(kv =>
        {
            var name = _faculties.FirstOrDefault(f => f.FacultyId == kv.Key)?.Name ?? kv.Key;
            double pct = total > 0 ? (double)kv.Value / total * 100 : 0;
            return new[] { kv.Key, name, kv.Value.ToString(), $"{pct:F1}%" };
        }).ToList();

        TablePrinter.Print(headers, rows, widths, rightAlign: new[] { false, false, true, true });
        ColorConsole.PressAnyKey();
    }

    private void ByGender()
    {
        ConsoleRenderer.PrintMenuTitle("Students by Gender", "Main > Reports > Gender");
        var dist  = _reportService.GetDistributionByGender();
        var total = _studentService.TotalCount;
        Console.WriteLine();

        foreach (var (gender, count) in dist)
        {
            double pct = total > 0 ? (double)count / total * 100 : 0;
            int bar = (int)Math.Round(pct / 100 * 30);
            Console.Write($"  {gender,-10} ");
            Console.ForegroundColor = gender == Gender.Female ? ConsoleColor.Magenta : ConsoleColor.Cyan;
            Console.Write("█".PadRight(bar, '█').PadRight(30, '░'));
            Console.ResetColor();
            Console.WriteLine($"  {count,3} ({pct:F1}%)");
        }

        ColorConsole.PressAnyKey();
    }

    private void ByEnrollYear()
    {
        ConsoleRenderer.PrintMenuTitle("Students by Enrollment Year", "Main > Reports > By Year");
        var dist  = _reportService.GetDistributionByYear();
        var total = _studentService.TotalCount;
        Console.WriteLine();

        var headers = new[] { "Year", "Students", "Bar" };
        var widths  = new[] { 6, 10, 30 };
        var rows    = dist.Select(kv =>
        {
            double pct = total > 0 ? (double)kv.Value / total * 100 : 0;
            int bar = (int)Math.Round(pct / 100 * 28);
            return new[] { kv.Key.ToString(), kv.Value.ToString(), "█".PadRight(bar, '█') + $" {pct:F0}%" };
        }).ToList();

        TablePrinter.Print(headers, rows, widths, rightAlign: new[] { false, true, false });
        ColorConsole.PressAnyKey();
    }

    private void SubjectPassRates()
    {
        ConsoleRenderer.PrintMenuTitle("Subject Pass Rates", "Main > Reports > Pass Rates");
        var rates = _reportService.GetPassRateBySubject();
        Console.WriteLine();

        var headers = new[] { "Subject Name", "Pass Rate", "Visual" };
        var widths  = new[] { 32, 10, 28 };
        var rows    = rates.Select(kv =>
        {
            int bar = (int)Math.Round(kv.Value / 100 * 26);
            return new[] { kv.Key, $"{kv.Value:F1}%", "█".PadRight(bar, '█') };
        }).ToList();

        TablePrinter.Print(headers, rows, widths, rightAlign: new[] { false, true, false },
            rowColorSelector: i => rates.Values.ElementAt(i) >= 70
                ? ConsoleColor.Green
                : (rates.Values.ElementAt(i) >= 50 ? ConsoleColor.Yellow : ConsoleColor.Red));

        ColorConsole.PressAnyKey();
    }

    private void ExportCsv()
    {
        ConsoleRenderer.PrintMenuTitle("Export to CSV", "Main > Reports > Export");
        var path = ColorConsole.Prompt("Output file path (e.g. students_export.csv)");
        if (string.IsNullOrWhiteSpace(path)) path = "students_export.csv";

        try
        {
            _fileService.ExportStudentsToCsv(_studentService.GetAll(), path);
            ConsoleRenderer.PrintSuccess($"Exported to: {Path.GetFullPath(path)}");
        }
        catch (Exception ex)
        {
            ConsoleRenderer.PrintError($"Export failed: {ex.Message}");
        }

        ColorConsole.PressAnyKey();
    }
}
