using StudentManagementSystem.Models;
using StudentManagementSystem.Services;
using StudentManagementSystem.Utils;

namespace StudentManagementSystem.UI;

/// <summary>Enrollment and score management menu.</summary>
public class EnrollmentMenu
{
    private readonly EnrollmentService _enrollmentService;
    private readonly StudentService    _studentService;
    private readonly SubjectService    _subjectService;

    public EnrollmentMenu(EnrollmentService enrollmentService,
                          StudentService    studentService,
                          SubjectService    subjectService)
    {
        _enrollmentService = enrollmentService;
        _studentService    = studentService;
        _subjectService    = subjectService;
    }

    public void Show()
    {
        while (true)
        {
            ConsoleRenderer.PrintMenuTitle("Enrollment & Scores", "Main > Enrollment");
            ConsoleRenderer.PrintMenuOption("1", "Enroll Student in Subject");
            ConsoleRenderer.PrintMenuOption("2", "Enter / Update Scores");
            ConsoleRenderer.PrintMenuOption("3", "View Student Transcript");
            ConsoleRenderer.PrintMenuOption("4", "View All Enrollments");
            ConsoleRenderer.PrintMenuOption("5", "View Enrollments by Subject");
            ConsoleRenderer.PrintMenuOption("6", "Remove Enrollment");
            ConsoleRenderer.PrintMenuOption("0", "Back");
            ConsoleRenderer.PrintMenuPrompt();

            var choice = Console.ReadLine()?.Trim() ?? "";
            Console.ResetColor();

            switch (choice)
            {
                case "1": EnrollStudent();          break;
                case "2": EnterScores();            break;
                case "3": ViewTranscript();         break;
                case "4": ViewAllEnrollments();     break;
                case "5": ViewBySubject();          break;
                case "6": RemoveEnrollment();       break;
                case "0": return;
                default:  ConsoleRenderer.PrintWarning("Invalid."); Thread.Sleep(900); break;
            }
        }
    }

    private void EnrollStudent()
    {
        ConsoleRenderer.PrintMenuTitle("Enroll Student", "Main > Enrollment > Enroll");

        var studentId = ColorConsole.Prompt("Student ID");
        var student   = _studentService.GetStudent(studentId);
        if (student is null) { ConsoleRenderer.PrintError($"Student '{studentId}' not found."); ColorConsole.PressAnyKey(); return; }
        ConsoleRenderer.PrintInfo($"Student: {student.FullName}");

        var subjectId = ColorConsole.Prompt("Subject ID");
        var subject   = _subjectService.GetSubject(subjectId);
        if (subject is null) { ConsoleRenderer.PrintError($"Subject '{subjectId}' not found."); ColorConsole.PressAnyKey(); return; }
        ConsoleRenderer.PrintInfo($"Subject: {subject.Name} ({subject.Credits} credits)");

        if (!Validator.ReadInt("Semester", 1, 8, out var sem)) { ColorConsole.PressAnyKey(); return; }
        if (!Validator.ReadInt("Academic Year", 2000, 2030, out var year)) { ColorConsole.PressAnyKey(); return; }

        var (success, message) = _enrollmentService.EnrollStudent(studentId, subjectId, sem, year);
        if (success) ConsoleRenderer.PrintSuccess(message);
        else         ConsoleRenderer.PrintError(message);

        ColorConsole.PressAnyKey();
    }

    private void EnterScores()
    {
        ConsoleRenderer.PrintMenuTitle("Enter Scores", "Main > Enrollment > Scores");

        var studentId = ColorConsole.Prompt("Student ID");
        var student   = _studentService.GetStudent(studentId);
        if (student is null) { ConsoleRenderer.PrintError($"Student '{studentId}' not found."); ColorConsole.PressAnyKey(); return; }

        var enrollments = _enrollmentService.GetForStudent(studentId).ToList();
        if (enrollments.Count == 0)
        {
            ConsoleRenderer.PrintWarning($"No enrollments found for '{studentId}'.");
            ColorConsole.PressAnyKey(); return;
        }

        ConsoleRenderer.PrintInfo($"Enrollments for {student.FullName}:");
        Console.WriteLine();
        for (int i = 0; i < enrollments.Count; i++)
        {
            var e = enrollments[i];
            var sub = _subjectService.GetSubject(e.SubjectId);
            string scoreInfo = e.HasScores ? $"Mid:{e.MidtermScore:F0} | Final:{e.FinalScore:F0} | Avg:{e.AverageScore:F2} ({e.LetterGrade})" : "No scores yet";
            ConsoleRenderer.PrintMenuOption((i+1).ToString(), sub?.Name ?? e.SubjectId, scoreInfo);
        }

        if (!Validator.ReadInt("\nSelect enrollment number", 1, enrollments.Count, out var eIdx))
        { ColorConsole.PressAnyKey(); return; }

        var selected = enrollments[eIdx - 1];
        ConsoleRenderer.PrintInfo($"Entering scores for: {_subjectService.GetSubject(selected.SubjectId)?.Name}");

        if (!Validator.ReadScore("Midterm Score", out var midterm)) { ColorConsole.PressAnyKey(); return; }
        if (!Validator.ReadScore("Final Score",   out var final))   { ColorConsole.PressAnyKey(); return; }

        var (success, message) = _enrollmentService.EnterScores(selected.EnrollmentId, midterm, final);
        if (success) ConsoleRenderer.PrintSuccess(message);
        else         ConsoleRenderer.PrintError(message);

        ColorConsole.PressAnyKey();
    }

    private void ViewTranscript()
    {
        ConsoleRenderer.PrintMenuTitle("Student Transcript", "Main > Enrollment > Transcript");
        var studentId = ColorConsole.Prompt("Student ID");
        var student   = _studentService.GetStudent(studentId);
        if (student is null) { ConsoleRenderer.PrintError($"Student '{studentId}' not found."); ColorConsole.PressAnyKey(); return; }

        var enrollments = _enrollmentService.GetForStudent(studentId).ToList();

        Console.WriteLine();
        ConsoleRenderer.PrintSectionHeader($"Transcript — {student.FullName} ({student.StudentId})");
        Console.WriteLine($"  Faculty : {student.FacultyId}  |  Major: {student.Major}  |  Year: {student.EnrollmentYear}");
        Console.WriteLine();

        if (enrollments.Count == 0)
        {
            ConsoleRenderer.PrintWarning("No enrollments found.");
        }
        else
        {
            var headers = new[] { "Subject ID", "Subject Name", "Credits", "Midterm", "Final", "Average", "Grade", "Result" };
            var widths  = new[] { 10, 30, 7, 8, 7, 8, 6, 8 };
            var rows    = new List<string[]>();

            foreach (var e in enrollments)
            {
                var sub = _subjectService.GetSubject(e.SubjectId);
                rows.Add(new[]
                {
                    e.SubjectId,
                    sub?.Name ?? "Unknown",
                    (sub?.Credits ?? 0).ToString(),
                    e.HasScores ? e.MidtermScore.ToString("F1") : "—",
                    e.HasScores ? e.FinalScore.ToString("F1")   : "—",
                    e.HasScores ? e.AverageScore.ToString("F2") : "—",
                    e.HasScores ? e.LetterGrade.ToString()      : "—",
                    e.HasScores ? (e.IsPassed ? "PASS" : "FAIL") : "Pending"
                });
            }

            TablePrinter.Print(headers, rows, widths,
                rightAlign: new[] { false, false, true, true, true, true, false, false },
                rowColorSelector: i => enrollments[i].HasScores
                    ? (enrollments[i].IsPassed ? ConsoleColor.Green : ConsoleColor.Red)
                    : ConsoleColor.DarkGray);
        }

        Console.Write("  Overall GPA: ");
        Console.ForegroundColor = ConsoleRenderer.GpaConsoleColor(student.GPA);
        Console.WriteLine($"{student.GPA:F2} / 4.00  —  {student.AcademicStatus}");
        Console.ResetColor();

        ColorConsole.PressAnyKey();
    }

    private void ViewAllEnrollments()
    {
        ConsoleRenderer.PrintMenuTitle("All Enrollments", "Main > Enrollment > All");
        var all = _enrollmentService.GetAll().ToList();
        ConsoleRenderer.PrintInfo($"Total enrollments: {all.Count}");
        Console.WriteLine();

        var headers = new[] { "Enroll ID", "Student ID", "Subject ID", "Semester", "Year", "Average", "Grade" };
        var widths  = new[] { 10, 12, 10, 8, 6, 8, 6 };
        var rows    = all.Select(e => new[]
        {
            e.EnrollmentId, e.StudentId, e.SubjectId,
            e.Semester.ToString(), e.AcademicYear.ToString(),
            e.HasScores ? e.AverageScore.ToString("F2") : "N/A",
            e.HasScores ? e.LetterGrade.ToString()      : "—"
        }).ToList();

        TablePrinter.Print(headers, rows, widths);
        ColorConsole.PressAnyKey();
    }

    private void ViewBySubject()
    {
        ConsoleRenderer.PrintMenuTitle("Enrollments by Subject", "Main > Enrollment > By Subject");
        var subjectId = ColorConsole.Prompt("Subject ID");
        var subject   = _subjectService.GetSubject(subjectId);
        if (subject is null) { ConsoleRenderer.PrintError($"Subject '{subjectId}' not found."); ColorConsole.PressAnyKey(); return; }

        var enrollments = _enrollmentService.GetForSubject(subjectId).ToList();
        ConsoleRenderer.PrintInfo($"Subject: {subject.Name} | Enrolled: {enrollments.Count} student(s)");
        Console.WriteLine();

        var headers = new[] { "#", "Student ID", "Student Name", "Midterm", "Final", "Average", "Grade" };
        var widths  = new[] { 3, 12, 24, 8, 7, 8, 6 };
        var rows    = enrollments.Select((e, i) =>
        {
            var stu = _studentService.GetStudent(e.StudentId);
            return new[]
            {
                (i+1).ToString(), e.StudentId,
                stu?.FullName ?? "Unknown",
                e.HasScores ? e.MidtermScore.ToString("F1") : "—",
                e.HasScores ? e.FinalScore.ToString("F1")   : "—",
                e.HasScores ? e.AverageScore.ToString("F2") : "—",
                e.HasScores ? e.LetterGrade.ToString()      : "—"
            };
        }).ToList();

        TablePrinter.Print(headers, rows, widths,
            rowColorSelector: i => enrollments[i].HasScores
                ? (enrollments[i].IsPassed ? ConsoleColor.Green : ConsoleColor.Red)
                : ConsoleColor.DarkGray);
        ColorConsole.PressAnyKey();
    }

    private void RemoveEnrollment()
    {
        ConsoleRenderer.PrintMenuTitle("Remove Enrollment", "Main > Enrollment > Remove");
        var id = ColorConsole.Prompt("Enrollment ID");
        var e  = _enrollmentService.GetById(id);

        if (e is null) { ConsoleRenderer.PrintError($"Enrollment '{id}' not found."); ColorConsole.PressAnyKey(); return; }

        ConsoleRenderer.PrintWarning($"Removing: Student {e.StudentId} from Subject {e.SubjectId}");
        if (!ColorConsole.Confirm("Confirm?")) { ConsoleRenderer.PrintInfo("Cancelled."); ColorConsole.PressAnyKey(); return; }

        if (_enrollmentService.DeleteEnrollment(id))
            ConsoleRenderer.PrintSuccess("Enrollment removed. GPA recalculated.");
        else
            ConsoleRenderer.PrintError("Removal failed.");

        ColorConsole.PressAnyKey();
    }
}
