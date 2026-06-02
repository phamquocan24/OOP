namespace StudentManagementSystem.Models;

/// <summary>Biological or self-identified gender.</summary>
public enum Gender
{
    Male,
    Female,
    Other
}

/// <summary>Academic standing based on cumulative GPA.</summary>
public enum AcademicStatus
{
    Excellent,   // GPA >= 3.6
    Good,        // GPA >= 3.2
    Average,     // GPA >= 2.5
    BelowAverage,// GPA >= 2.0
    Poor,        // GPA >= 1.0
    Expelled     // GPA < 1.0
}

/// <summary>Enrollment status of a student.</summary>
public enum EnrollmentStatus
{
    Active,
    OnLeave,
    Graduated,
    Expelled,
    Suspended
}

/// <summary>Letter grade mapped from numeric score.</summary>
public enum LetterGrade
{
    A,   // 90-100
    B,   // 80-89
    C,   // 70-79
    D,   // 60-69
    F    // < 60
}
