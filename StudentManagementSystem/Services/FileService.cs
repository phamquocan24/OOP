using System.Text.Json;
using System.Text.Json.Serialization;
using StudentManagementSystem.Models;
using StudentManagementSystem.Repositories;

namespace StudentManagementSystem.Services;

/// <summary>
/// Handles JSON serialization/deserialization and ensures the Data/ directory exists.
/// </summary>
public class FileService
{
    private readonly string _dataDir;
    private readonly JsonSerializerOptions _options;

    private string StudentsFile    => Path.Combine(_dataDir, "students.json");
    private string SubjectsFile    => Path.Combine(_dataDir, "subjects.json");
    private string EnrollmentsFile => Path.Combine(_dataDir, "enrollments.json");
    private string FacultiesFile   => Path.Combine(_dataDir, "faculties.json");

    public FileService(string dataDirectory = "Data")
    {
        _dataDir = dataDirectory;
        Directory.CreateDirectory(_dataDir);

        _options = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNameCaseInsensitive = true,
            Converters = { new JsonStringEnumConverter() }
        };
    }

    // ── Save ─────────────────────────────────────────────────────────────────
    public void SaveStudents(IEnumerable<Student> students)
        => WriteJson(StudentsFile, students.ToList());

    public void SaveSubjects(IEnumerable<Subject> subjects)
        => WriteJson(SubjectsFile, subjects.ToList());

    public void SaveEnrollments(IEnumerable<Enrollment> enrollments)
        => WriteJson(EnrollmentsFile, enrollments.ToList());

    public void SaveFaculties(IEnumerable<Faculty> faculties)
        => WriteJson(FacultiesFile, faculties.ToList());

    // ── Load ─────────────────────────────────────────────────────────────────
    public List<Student> LoadStudents()
        => ReadJson<List<Student>>(StudentsFile) ?? new();

    public List<Subject> LoadSubjects()
        => ReadJson<List<Subject>>(SubjectsFile) ?? new();

    public List<Enrollment> LoadEnrollments()
        => ReadJson<List<Enrollment>>(EnrollmentsFile) ?? new();

    public List<Faculty> LoadFaculties()
        => ReadJson<List<Faculty>>(FacultiesFile) ?? new();

    // ── Helpers ───────────────────────────────────────────────────────────────
    private void WriteJson<T>(string path, T data)
    {
        try
        {
            var json = JsonSerializer.Serialize(data, _options);
            File.WriteAllText(path, json);
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"[FileService] Failed to save {path}: {ex.Message}");
        }
    }

    private T? ReadJson<T>(string path)
    {
        try
        {
            if (!File.Exists(path)) return default;
            var json = File.ReadAllText(path);
            return JsonSerializer.Deserialize<T>(json, _options);
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"[FileService] Failed to load {path}: {ex.Message}");
            return default;
        }
    }

    // ── CSV Export ────────────────────────────────────────────────────────────
    public void ExportStudentsToCsv(IEnumerable<Student> students, string outputPath)
    {
        var lines = new List<string>
        {
            "StudentID,FullName,DateOfBirth,Gender,Faculty,Major,EnrollYear,GPA,AcademicStatus,Status,Email,Phone"
        };

        foreach (var s in students)
        {
            lines.Add(string.Join(",",
                s.StudentId,
                $"\"{s.FullName}\"",
                s.DateOfBirth.ToString("yyyy-MM-dd"),
                s.Gender,
                s.FacultyId,
                $"\"{s.Major}\"",
                s.EnrollmentYear,
                s.GPA.ToString("F2"),
                s.AcademicStatus,
                s.Status,
                s.Email,
                s.Phone));
        }

        File.WriteAllLines(outputPath, lines);
    }
}
