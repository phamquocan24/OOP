namespace StudentManagementSystem.Models;

/// <summary>
/// Represents a department/faculty within the university.
/// </summary>
public class Faculty
{
    public string FacultyId   { get; set; } = string.Empty;
    public string Name        { get; set; } = string.Empty;
    public string Code        { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string HeadName    { get; set; } = string.Empty;

    public Faculty() { }

    public Faculty(string facultyId, string name, string code,
                   string description = "", string headName = "")
    {
        FacultyId   = facultyId;
        Name        = name;
        Code        = code;
        Description = description;
        HeadName    = headName;
    }

    public override string ToString() => $"[{Code}] {Name}";
}
