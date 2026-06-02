using StudentManagementSystem.Models;
using StudentManagementSystem.Services;
using StudentManagementSystem.Utils;

namespace StudentManagementSystem.UI;

/// <summary>Subject CRUD menu.</summary>
public class SubjectMenu
{
    private readonly SubjectService _subjectService;
    private readonly List<Faculty>  _faculties;

    public SubjectMenu(SubjectService subjectService, List<Faculty> faculties)
    {
        _subjectService = subjectService;
        _faculties      = faculties;
    }

    public void Show()
    {
        while (true)
        {
            ConsoleRenderer.PrintMenuTitle("Subject Management", "Main > Subjects");
            ConsoleRenderer.PrintMenuOption("1", "List All Subjects");
            ConsoleRenderer.PrintMenuOption("2", "Search Subjects");
            ConsoleRenderer.PrintMenuOption("3", "Add New Subject");
            ConsoleRenderer.PrintMenuOption("4", "Edit Subject");
            ConsoleRenderer.PrintMenuOption("5", "Delete Subject");
            ConsoleRenderer.PrintMenuOption("0", "Back");
            ConsoleRenderer.PrintMenuPrompt();

            var choice = Console.ReadLine()?.Trim() ?? "";
            Console.ResetColor();

            switch (choice)
            {
                case "1": ListAll();         break;
                case "2": SearchSubjects();  break;
                case "3": AddSubject();      break;
                case "4": EditSubject();     break;
                case "5": DeleteSubject();   break;
                case "0": return;
                default:  ConsoleRenderer.PrintWarning("Invalid choice."); Thread.Sleep(900); break;
            }
        }
    }

    private void ListAll(IEnumerable<Subject>? overrideList = null)
    {
        ConsoleRenderer.PrintMenuTitle("All Subjects", "Main > Subjects > List");
        var subjects = (overrideList ?? _subjectService.GetAll()).ToList();

        if (subjects.Count == 0)
        {
            ConsoleRenderer.PrintWarning("No subjects found.");
            ColorConsole.PressAnyKey(); return;
        }

        ConsoleRenderer.PrintInfo($"Total: {subjects.Count} subject(s)");
        Console.WriteLine();

        var headers = new[] { "#", "ID", "Code", "Subject Name", "Credits", "Faculty", "Semester" };
        var widths  = new[] { 3, 8, 8, 30, 7, 6, 8 };
        var rows    = subjects.Select((s, i) => new[]
        {
            (i+1).ToString(), s.SubjectId, s.Code, s.Name,
            s.Credits.ToString(), s.FacultyId, s.Semester.ToString()
        }).ToList();

        TablePrinter.Print(headers, rows, widths,
            rightAlign: new[] { true, false, false, false, true, false, true });
        ColorConsole.PressAnyKey();
    }

    private void SearchSubjects()
    {
        ConsoleRenderer.PrintMenuTitle("Search Subjects", "Main > Subjects > Search");
        var keyword = ColorConsole.Prompt("Keyword");
        var results = _subjectService.Search(keyword).ToList();
        ConsoleRenderer.PrintInfo($"Found {results.Count} result(s)");
        ListAll(results);
    }

    private void AddSubject()
    {
        ConsoleRenderer.PrintMenuTitle("Add New Subject", "Main > Subjects > Add");

        var subjectId = ColorConsole.Prompt("Subject ID (e.g. SUB011)");
        var name      = ColorConsole.Prompt("Subject Name");
        var code      = ColorConsole.Prompt("Course Code (e.g. CS601)");

        if (!Validator.ReadInt("Credits", 1, 10, out var credits))
        { ColorConsole.PressAnyKey(); return; }

        if (!Validator.ReadInt("Semester (1-8)", 1, 8, out var semester))
        { ColorConsole.PressAnyKey(); return; }

        Console.WriteLine();
        for (int i = 0; i < _faculties.Count; i++)
            ConsoleRenderer.PrintMenuOption((i+1).ToString(), _faculties[i].Name);
        if (!Validator.ReadInt("Select faculty", 1, _faculties.Count, out var fIdx))
        { ColorConsole.PressAnyKey(); return; }

        var desc = ColorConsole.Prompt("Description (optional)");

        var subject = new Subject(subjectId, name, code, credits,
                                  _faculties[fIdx-1].FacultyId, semester, desc);
        if (_subjectService.AddSubject(subject))
            ConsoleRenderer.PrintSuccess($"Subject '{name}' added!");
        else
            ConsoleRenderer.PrintError($"Subject ID '{subjectId}' already exists.");

        ColorConsole.PressAnyKey();
    }

    private void EditSubject()
    {
        ConsoleRenderer.PrintMenuTitle("Edit Subject", "Main > Subjects > Edit");
        var id = ColorConsole.Prompt("Enter Subject ID to edit");
        var subject = _subjectService.GetSubject(id);

        if (subject is null)
        {
            ConsoleRenderer.PrintError($"Subject '{id}' not found.");
            ColorConsole.PressAnyKey(); return;
        }

        var newName = ColorConsole.Prompt($"Name [{subject.Name}]");
        if (!string.IsNullOrWhiteSpace(newName)) subject.Name = newName;

        var credStr = ColorConsole.Prompt($"Credits [{subject.Credits}]");
        if (Validator.TryParseInt(credStr, out var cred) && cred is >= 1 and <= 10)
            subject.Credits = cred;

        var descStr = ColorConsole.Prompt($"Description [{subject.Description}]");
        if (!string.IsNullOrWhiteSpace(descStr)) subject.Description = descStr;

        if (_subjectService.UpdateSubject(subject))
            ConsoleRenderer.PrintSuccess("Subject updated!");
        else
            ConsoleRenderer.PrintError("Update failed.");

        ColorConsole.PressAnyKey();
    }

    private void DeleteSubject()
    {
        ConsoleRenderer.PrintMenuTitle("Delete Subject", "Main > Subjects > Delete");
        var id = ColorConsole.Prompt("Enter Subject ID to delete");
        var subject = _subjectService.GetSubject(id);

        if (subject is null)
        {
            ConsoleRenderer.PrintError($"Subject '{id}' not found.");
            ColorConsole.PressAnyKey(); return;
        }

        ConsoleRenderer.PrintWarning($"Deleting: {subject.Name} ({subject.Code})");
        if (!ColorConsole.Confirm("Are you sure?"))
        {
            ConsoleRenderer.PrintInfo("Cancelled."); ColorConsole.PressAnyKey(); return;
        }

        if (_subjectService.DeleteSubject(id))
            ConsoleRenderer.PrintSuccess("Subject deleted.");
        else
            ConsoleRenderer.PrintError("Deletion failed.");

        ColorConsole.PressAnyKey();
    }
}
