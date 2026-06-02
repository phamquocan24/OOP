namespace StudentManagementSystem.Models;

/// <summary>
/// Represents an academic subject/course.
/// </summary>
public class Subject
{
    private int _credits;

    public string SubjectId   { get; set; } = string.Empty;
    public string Name        { get; set; } = string.Empty;
    public string Code        { get; set; } = string.Empty;
    public string FacultyId   { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Semester       { get; set; } = 1;

    public int Credits
    {
        get => _credits;
        set => _credits = value is >= 1 and <= 10
            ? value
            : throw new ArgumentOutOfRangeException(nameof(Credits), "Credits must be between 1 and 10.");
    }

    public Subject() { }

    public Subject(string subjectId, string name, string code,
                   int credits, string facultyId = "", int semester = 1,
                   string description = "")
    {
        SubjectId   = subjectId;
        Name        = name;
        Code        = code;
        Credits     = credits;
        FacultyId   = facultyId;
        Semester    = semester;
        Description = description;
    }

    public override string ToString() => $"[{Code}] {Name} ({Credits} cr.)";
}
