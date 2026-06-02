namespace StudentManagementSystem.Models;

/// <summary>
/// Represents a student's enrollment in a specific subject, including scores.
/// Acts as the join entity between Student and Subject.
/// </summary>
public class Enrollment
{
    private double _midtermScore;
    private double _finalScore;

    public string EnrollmentId { get; set; } = Guid.NewGuid().ToString("N")[..8].ToUpper();
    public string StudentId    { get; set; } = string.Empty;
    public string SubjectId    { get; set; } = string.Empty;
    public int    Semester     { get; set; } = 1;
    public int    AcademicYear { get; set; } = DateTime.Now.Year;
    public DateTime EnrolledAt { get; set; } = DateTime.Now;

    /// <summary>Midterm score (0–100).</summary>
    public double MidtermScore
    {
        get => _midtermScore;
        set => _midtermScore = Math.Round(Math.Clamp(value, 0, 100), 2);
    }

    /// <summary>Final exam score (0–100).</summary>
    public double FinalScore
    {
        get => _finalScore;
        set => _finalScore = Math.Round(Math.Clamp(value, 0, 100), 2);
    }

    /// <summary>Whether scores have been entered.</summary>
    public bool HasScores { get; set; } = false;

    /// <summary>Weighted average: Midterm × 40% + Final × 60%.</summary>
    public double AverageScore => HasScores
        ? Math.Round(MidtermScore * 0.4 + FinalScore * 0.6, 2)
        : 0;

    /// <summary>Convert average score to 4.0 GPA scale.</summary>
    public double GradePoint => AverageScore switch
    {
        >= 90 => 4.0,
        >= 85 => 3.7,
        >= 80 => 3.3,
        >= 75 => 3.0,
        >= 70 => 2.7,
        >= 65 => 2.3,
        >= 60 => 2.0,
        >= 55 => 1.7,
        >= 50 => 1.3,
        >= 40 => 1.0,
        _     => 0.0
    };

    /// <summary>Letter grade from average score.</summary>
    public LetterGrade LetterGrade => AverageScore switch
    {
        >= 80 => LetterGrade.A,
        >= 70 => LetterGrade.B,
        >= 60 => LetterGrade.C,
        >= 50 => LetterGrade.D,
        _     => LetterGrade.F
    };

    public bool IsPassed => AverageScore >= 50;

    public Enrollment() { }

    public Enrollment(string studentId, string subjectId, int semester, int academicYear)
    {
        StudentId    = studentId;
        SubjectId    = subjectId;
        Semester     = semester;
        AcademicYear = academicYear;
    }

    public override string ToString()
        => $"[{EnrollmentId}] Student:{StudentId} | Subject:{SubjectId} | Avg:{AverageScore:F2} ({LetterGrade})";
}
