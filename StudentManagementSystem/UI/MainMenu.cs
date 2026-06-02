using StudentManagementSystem.Models;
using StudentManagementSystem.Services;
using StudentManagementSystem.UI;
using StudentManagementSystem.Utils;

namespace StudentManagementSystem.UI;

/// <summary>Application root menu — wires all sub-menus together.</summary>
public class MainMenu
{
    private readonly StudentMenu    _studentMenu;
    private readonly SubjectMenu    _subjectMenu;
    private readonly EnrollmentMenu _enrollmentMenu;
    private readonly ReportMenu     _reportMenu;
    private readonly StudentService _studentService;

    public MainMenu(StudentMenu    studentMenu,
                    SubjectMenu    subjectMenu,
                    EnrollmentMenu enrollmentMenu,
                    ReportMenu     reportMenu,
                    StudentService studentService)
    {
        _studentMenu    = studentMenu;
        _subjectMenu    = subjectMenu;
        _enrollmentMenu = enrollmentMenu;
        _reportMenu     = reportMenu;
        _studentService = studentService;
    }

    public void Run()
    {
        // Configure console for unicode/emoji support
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Console.Title = "Student Management System v1.0";

        while (true)
        {
            ConsoleRenderer.PrintBanner();
            PrintStats();

            Console.WriteLine();
            ConsoleRenderer.PrintMenuOption("1", "Student Management",   "Add, edit, delete, search students");
            ConsoleRenderer.PrintMenuOption("2", "Subject Management",   "Manage courses and subjects");
            ConsoleRenderer.PrintMenuOption("3", "Enrollment & Scores",  "Enroll students, enter grades");
            ConsoleRenderer.PrintMenuOption("4", "Reports & Statistics", "Rankings, charts, CSV export");
            ConsoleRenderer.PrintMenuOption("0", "Exit", "Save & quit");
            ConsoleRenderer.PrintMenuPrompt();

            var choice = Console.ReadLine()?.Trim() ?? "";
            Console.ResetColor();

            switch (choice)
            {
                case "1": _studentMenu.Show();    break;
                case "2": _subjectMenu.Show();    break;
                case "3": _enrollmentMenu.Show(); break;
                case "4": _reportMenu.Show();     break;
                case "0": Exit(); return;
                default:
                    ConsoleRenderer.PrintWarning("Please enter a number between 0 and 4.");
                    Thread.Sleep(900);
                    break;
            }
        }
    }

    private void PrintStats()
    {
        int count = _studentService.TotalCount;
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine($"  📊 {count} student(s) in database  |  {DateTime.Now:dd MMM yyyy, HH:mm}");
        Console.ResetColor();
    }

    private static void Exit()
    {
        Console.Clear();
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("\n  ╔════════════════════════════════════════╗");
        Console.WriteLine("  ║   Thank you for using SMS v1.0 👋     ║");
        Console.WriteLine("  ║   All data has been saved.             ║");
        Console.WriteLine("  ╚════════════════════════════════════════╝");
        Console.ResetColor();
        Console.WriteLine();
        Thread.Sleep(1200);
    }
}
