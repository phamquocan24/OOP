using StudentManagementSystem.Models;
using StudentManagementSystem.Services;
using StudentManagementSystem.Utils;

namespace StudentManagementSystem.UI;

/// <summary>Student CRUD and search menu.</summary>
public class StudentMenu
{
    private readonly StudentService _studentService;
    private readonly List<Faculty>  _faculties;

    public StudentMenu(StudentService studentService, List<Faculty> faculties)
    {
        _studentService = studentService;
        _faculties      = faculties;
    }

    public void Show()
    {
        while (true)
        {
            ConsoleRenderer.PrintMenuTitle("Student Management", "Main > Students");
            ConsoleRenderer.PrintMenuOption("1", "List All Students");
            ConsoleRenderer.PrintMenuOption("2", "Search / Filter");
            ConsoleRenderer.PrintMenuOption("3", "View Student Details");
            ConsoleRenderer.PrintMenuOption("4", "Add New Student");
            ConsoleRenderer.PrintMenuOption("5", "Edit Student");
            ConsoleRenderer.PrintMenuOption("6", "Delete Student");
            ConsoleRenderer.PrintMenuOption("7", "Sort Students");
            ConsoleRenderer.PrintMenuOption("0", "Back to Main Menu");
            ConsoleRenderer.PrintMenuPrompt();

            var choice = Console.ReadLine()?.Trim() ?? "";
            Console.ResetColor();

            switch (choice)
            {
                case "1": ListAll();            break;
                case "2": SearchStudents();     break;
                case "3": ViewDetails();        break;
                case "4": AddStudent();         break;
                case "5": EditStudent();        break;
                case "6": DeleteStudent();      break;
                case "7": SortMenu();           break;
                case "0": return;
                default:  ConsoleRenderer.PrintWarning("Invalid choice."); Thread.Sleep(900); break;
            }
        }
    }

    // ── List All ─────────────────────────────────────────────────────────────
    private void ListAll(IEnumerable<Student>? overrideList = null)
    {
        ConsoleRenderer.PrintMenuTitle("All Students", "Main > Students > List");
        var students = (overrideList ?? _studentService.GetAll()).ToList();

        if (students.Count == 0)
        {
            ConsoleRenderer.PrintWarning("No students found.");
            ColorConsole.PressAnyKey();
            return;
        }

        ConsoleRenderer.PrintInfo($"Total: {students.Count} student(s)");
        Console.WriteLine();

        var headers = new[] { "#", "Student ID", "Full Name", "Faculty", "Major", "GPA", "Status" };
        var widths  = new[] { 3, 12, 24, 6, 22, 6, 12 };
        var rows    = students.Select((s, i) => new[]
        {
            (i + 1).ToString(),
            s.StudentId,
            s.FullName,
            s.FacultyId,
            s.Major,
            s.GPA.ToString("F2"),
            s.AcademicStatus.ToString()
        }).ToList();

        TablePrinter.Print(headers, rows, widths,
            rightAlign: new[] { true, false, false, false, false, true, false },
            rowColorSelector: i => ConsoleRenderer.GpaConsoleColor(students[i].GPA));

        ColorConsole.PressAnyKey();
    }

    // ── Search ────────────────────────────────────────────────────────────────
    private void SearchStudents()
    {
        ConsoleRenderer.PrintMenuTitle("Search Students", "Main > Students > Search");
        ConsoleRenderer.PrintInfo("Search by: Name, Student ID, Faculty, Major, Email, Phone");

        var keyword = ColorConsole.Prompt("Keyword");
        if (string.IsNullOrWhiteSpace(keyword))
        {
            ConsoleRenderer.PrintWarning("Keyword cannot be empty.");
            ColorConsole.PressAnyKey();
            return;
        }

        var results = _studentService.Search(keyword).ToList();
        ConsoleRenderer.PrintInfo($"Found {results.Count} result(s) for \"{keyword}\"");
        ListAll(results);
    }

    // ── View Details ──────────────────────────────────────────────────────────
    private void ViewDetails()
    {
        ConsoleRenderer.PrintMenuTitle("Student Details", "Main > Students > Details");
        var id = ColorConsole.Prompt("Enter Student ID");
        var student = _studentService.GetStudent(id);

        if (student is null)
        {
            ConsoleRenderer.PrintError($"Student '{id}' not found.");
            ColorConsole.PressAnyKey();
            return;
        }

        Console.WriteLine();
        var facultyName = _faculties.FirstOrDefault(f => f.FacultyId == student.FacultyId)?.Name ?? student.FacultyId;

        ConsoleRenderer.PrintSectionHeader("Personal Information");
        Console.WriteLine($"  ID          : {student.Id}");
        Console.WriteLine($"  Student ID  : {student.StudentId}");
        Console.WriteLine($"  Full Name   : {student.FullName}");
        Console.WriteLine($"  Date of Birth: {student.DateOfBirth:dd/MM/yyyy}  (Age: {student.Age})");
        Console.WriteLine($"  Gender      : {student.Gender}");
        Console.WriteLine($"  Email       : {(string.IsNullOrEmpty(student.Email) ? "N/A" : student.Email)}");
        Console.WriteLine($"  Phone       : {(string.IsNullOrEmpty(student.Phone) ? "N/A" : student.Phone)}");
        Console.WriteLine($"  Address     : {(string.IsNullOrEmpty(student.Address) ? "N/A" : student.Address)}");

        ConsoleRenderer.PrintSectionHeader("Academic Information");
        Console.WriteLine($"  Faculty     : {facultyName}");
        Console.WriteLine($"  Major       : {student.Major}");
        Console.WriteLine($"  Enroll Year : {student.EnrollmentYear}");
        Console.WriteLine($"  Status      : {student.Status}");

        Console.Write($"  GPA         : ");
        Console.ForegroundColor = ConsoleRenderer.GpaConsoleColor(student.GPA);
        Console.Write($"{student.GPA:F2} / 4.00  {ConsoleRenderer.ProgressBar(student.GPA)}");
        Console.ResetColor();
        Console.WriteLine();
        Console.Write($"  Standing    : ");
        Console.ForegroundColor = ConsoleRenderer.GpaConsoleColor(student.GPA);
        Console.WriteLine(student.AcademicStatus.ToString());
        Console.ResetColor();

        ColorConsole.PressAnyKey();
    }

    // ── Add ───────────────────────────────────────────────────────────────────
    private void AddStudent()
    {
        ConsoleRenderer.PrintMenuTitle("Add New Student", "Main > Students > Add");

        var studentId = ColorConsole.Prompt("Student ID (e.g. SV011)");
        if (!Validator.IsValidStudentId(studentId))
        {
            ConsoleRenderer.PrintError("Invalid Student ID (3–15 characters required).");
            ColorConsole.PressAnyKey(); return;
        }

        var fullName = ColorConsole.Prompt("Full Name");
        if (!Validator.IsValidName(fullName))
        {
            ConsoleRenderer.PrintError("Invalid name (2–100 characters required).");
            ColorConsole.PressAnyKey(); return;
        }

        var dobStr = ColorConsole.Prompt("Date of Birth (dd/MM/yyyy)");
        if (!Validator.TryParseDate(dobStr, out var dob))
        {
            ConsoleRenderer.PrintError("Invalid date format. Use dd/MM/yyyy.");
            ColorConsole.PressAnyKey(); return;
        }

        Console.WriteLine("  Gender: [1] Male  [2] Female  [3] Other");
        var genderChoice = ColorConsole.Prompt("Select gender");
        var gender = genderChoice switch { "2" => Gender.Female, "3" => Gender.Other, _ => Gender.Male };

        // Faculty selection
        Console.WriteLine();
        ConsoleRenderer.PrintSectionHeader("Available Faculties");
        for (int i = 0; i < _faculties.Count; i++)
            ConsoleRenderer.PrintMenuOption((i + 1).ToString(), _faculties[i].Name, _faculties[i].Code);
        if (!Validator.ReadInt("Select faculty number", 1, _faculties.Count, out var fIdx))
        { ColorConsole.PressAnyKey(); return; }
        var facultyId = _faculties[fIdx - 1].FacultyId;

        var major = ColorConsole.Prompt("Major / Programme");
        if (string.IsNullOrWhiteSpace(major)) major = "Undeclared";

        if (!Validator.ReadInt("Enrollment Year", 2000, 2030, out var enrollYear))
        { ColorConsole.PressAnyKey(); return; }

        var email   = ColorConsole.Prompt("Email (optional, press Enter to skip)");
        var phone   = ColorConsole.Prompt("Phone (optional)");
        var address = ColorConsole.Prompt("Address (optional)");

        var student = new Student(
            Guid.NewGuid().ToString("N")[..8].ToUpper(),
            studentId, fullName, dob, gender,
            facultyId, major, enrollYear, email, phone, address);

        if (_studentService.AddStudent(student))
        {
            ConsoleRenderer.PrintSuccess($"Student '{student.FullName}' added successfully! (ID: {student.StudentId})");
        }
        else
        {
            ConsoleRenderer.PrintError($"Student ID '{studentId}' already exists.");
        }
        ColorConsole.PressAnyKey();
    }

    // ── Edit ──────────────────────────────────────────────────────────────────
    private void EditStudent()
    {
        ConsoleRenderer.PrintMenuTitle("Edit Student", "Main > Students > Edit");
        var id = ColorConsole.Prompt("Enter Student ID to edit");
        var student = _studentService.GetStudent(id);

        if (student is null)
        {
            ConsoleRenderer.PrintError($"Student '{id}' not found.");
            ColorConsole.PressAnyKey(); return;
        }

        ConsoleRenderer.PrintInfo($"Editing: {student.FullName}  (leave blank to keep current value)");
        Console.WriteLine();

        var newName = ColorConsole.Prompt($"Full Name [{student.FullName}]");
        if (!string.IsNullOrWhiteSpace(newName) && Validator.IsValidName(newName))
            student.FullName = newName;

        var newEmail = ColorConsole.Prompt($"Email [{student.Email}]");
        if (!string.IsNullOrWhiteSpace(newEmail) && Validator.IsValidEmail(newEmail))
            student.Email = newEmail;

        var newPhone = ColorConsole.Prompt($"Phone [{student.Phone}]");
        if (!string.IsNullOrWhiteSpace(newPhone))
            student.Phone = newPhone;

        var newAddress = ColorConsole.Prompt($"Address [{student.Address}]");
        if (!string.IsNullOrWhiteSpace(newAddress))
            student.Address = newAddress;

        var newMajor = ColorConsole.Prompt($"Major [{student.Major}]");
        if (!string.IsNullOrWhiteSpace(newMajor))
            student.Major = newMajor;

        Console.WriteLine("  Status: [1] Active  [2] OnLeave  [3] Graduated  [4] Expelled  [5] Suspended");
        var statusInput = ColorConsole.Prompt($"Enrollment Status [{student.Status}]");
        student.Status = statusInput switch
        {
            "1" => EnrollmentStatus.Active,
            "2" => EnrollmentStatus.OnLeave,
            "3" => EnrollmentStatus.Graduated,
            "4" => EnrollmentStatus.Expelled,
            "5" => EnrollmentStatus.Suspended,
            _   => student.Status
        };

        if (_studentService.UpdateStudent(student))
            ConsoleRenderer.PrintSuccess("Student updated successfully!");
        else
            ConsoleRenderer.PrintError("Update failed.");

        ColorConsole.PressAnyKey();
    }

    // ── Delete ────────────────────────────────────────────────────────────────
    private void DeleteStudent()
    {
        ConsoleRenderer.PrintMenuTitle("Delete Student", "Main > Students > Delete");
        var id = ColorConsole.Prompt("Enter Student ID to delete");
        var student = _studentService.GetStudent(id);

        if (student is null)
        {
            ConsoleRenderer.PrintError($"Student '{id}' not found.");
            ColorConsole.PressAnyKey(); return;
        }

        ConsoleRenderer.PrintWarning($"You are about to delete: {student.FullName} ({student.StudentId})");
        if (!ColorConsole.Confirm("Are you sure?"))
        {
            ConsoleRenderer.PrintInfo("Operation cancelled.");
            ColorConsole.PressAnyKey(); return;
        }

        if (_studentService.DeleteStudent(id))
            ConsoleRenderer.PrintSuccess("Student deleted successfully.");
        else
            ConsoleRenderer.PrintError("Deletion failed.");

        ColorConsole.PressAnyKey();
    }

    // ── Sort ──────────────────────────────────────────────────────────────────
    private void SortMenu()
    {
        ConsoleRenderer.PrintMenuTitle("Sort Students", "Main > Students > Sort");
        ConsoleRenderer.PrintMenuOption("1", "Sort by Name (A–Z)");
        ConsoleRenderer.PrintMenuOption("2", "Sort by GPA (Highest First)");
        ConsoleRenderer.PrintMenuOption("3", "Sort by Student ID");
        ConsoleRenderer.PrintMenuPrompt();

        var ch = Console.ReadLine()?.Trim() ?? "";
        Console.ResetColor();

        IEnumerable<Student> sorted = ch switch
        {
            "1" => _studentService.GetSortedByName(),
            "2" => _studentService.GetSortedByGpa(),
            "3" => _studentService.GetAll().OrderBy(s => s.StudentId),
            _   => _studentService.GetAll()
        };

        ListAll(sorted);
    }
}
