namespace StudentManagementSystem.Models;

/// <summary>
/// Represents a university student — inherits from Person.
/// Demonstrates inheritance and additional academic-specific properties.
/// </summary>
public class Student : Person
{
    // ── Private backing fields ───────────────────────────────────────────────
    private string _studentId    = string.Empty;
    private double _gpa;
    private int    _enrollmentYear;

    // ── Academic Properties ──────────────────────────────────────────────────
    public string StudentId
    {
        get => _studentId;
        set => _studentId = string.IsNullOrWhiteSpace(value)
            ? throw new ArgumentException("Student ID cannot be empty.", nameof(value))
            : value.Trim().ToUpper();
    }

    public string FacultyId       { get; set; } = string.Empty;
    public string Major           { get; set; } = string.Empty;
    public string Address         { get; set; } = string.Empty;
    public EnrollmentStatus Status { get; set; } = EnrollmentStatus.Active;

    public int EnrollmentYear
    {
        get => _enrollmentYear;
        set => _enrollmentYear = value is >= 1990 and <= 2100
            ? value
            : throw new ArgumentOutOfRangeException(nameof(EnrollmentYear), "Invalid enrollment year.");
    }

    /// <summary>Overall GPA (0.0 – 4.0 scale). Auto-updated by EnrollmentService.</summary>
    public double GPA
    {
        get => _gpa;
        set => _gpa = Math.Round(Math.Clamp(value, 0.0, 4.0), 2);
    }

    /// <summary>Derives academic standing from GPA automatically.</summary>
    public AcademicStatus AcademicStatus => GPA switch
    {
        >= 3.6 => AcademicStatus.Excellent,
        >= 3.2 => AcademicStatus.Good,
        >= 2.5 => AcademicStatus.Average,
        >= 2.0 => AcademicStatus.BelowAverage,
        >= 1.0 => AcademicStatus.Poor,
        _      => AcademicStatus.Expelled
    };

    // ── Constructors ─────────────────────────────────────────────────────────
    public Student() { }

    public Student(string id, string studentId, string fullName,
                   DateTime dateOfBirth, Gender gender, string facultyId,
                   string major, int enrollmentYear,
                   string email = "", string phone = "", string address = "")
        : base(id, fullName, dateOfBirth, gender, email, phone)
    {
        StudentId      = studentId;
        FacultyId      = facultyId;
        Major          = major;
        EnrollmentYear = enrollmentYear;
        Address        = address;
    }

    // ── Overrides (Polymorphism) ─────────────────────────────────────────────
    public override string GetSummary()
        => $"{StudentId} | {FullName} | {Major} | GPA: {GPA:F2} | {AcademicStatus}";

    public override void DisplayInfo()
    {
        base.DisplayInfo();
        Console.WriteLine($"  Student ID     : {StudentId}");
        Console.WriteLine($"  Faculty        : {FacultyId}");
        Console.WriteLine($"  Major          : {Major}");
        Console.WriteLine($"  Enroll Year    : {EnrollmentYear}");
        Console.WriteLine($"  GPA            : {GPA:F2} / 4.00");
        Console.WriteLine($"  Academic Status: {AcademicStatus}");
        Console.WriteLine($"  Enrollment     : {Status}");
        Console.WriteLine($"  Address        : {(string.IsNullOrEmpty(Address) ? "N/A" : Address)}");
    }

    public override string ToString() => $"[{StudentId}] {FullName}";
}
