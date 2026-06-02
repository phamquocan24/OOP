using StudentManagementSystem.Models;
using StudentManagementSystem.Repositories;

namespace StudentManagementSystem.Services;

/// <summary>
/// Business logic for enrollments and score management.
/// Recalculates student GPA after any score change.
/// </summary>
public class EnrollmentService
{
    private readonly EnrollmentRepository _repo;
    private readonly StudentRepository    _studentRepo;
    private readonly SubjectRepository    _subjectRepo;
    private readonly FileService          _fileService;

    public EnrollmentService(
        EnrollmentRepository enrollmentRepo,
        StudentRepository    studentRepo,
        SubjectRepository    subjectRepo,
        FileService          fileService)
    {
        _repo        = enrollmentRepo;
        _studentRepo = studentRepo;
        _subjectRepo = subjectRepo;
        _fileService = fileService;
    }

    // ── Enroll ────────────────────────────────────────────────────────────────
    public (bool Success, string Message) EnrollStudent(string studentId, string subjectId,
                                                         int semester, int academicYear)
    {
        if (_studentRepo.GetById(studentId) is null)
            return (false, $"Student '{studentId}' not found.");

        if (_subjectRepo.GetById(subjectId) is null)
            return (false, $"Subject '{subjectId}' not found.");

        var enrollment = new Enrollment(studentId, subjectId, semester, academicYear);
        if (!_repo.Add(enrollment))
            return (false, "Student is already enrolled in this subject for the same semester/year.");

        _fileService.SaveEnrollments(_repo.GetAll());
        return (true, "Enrollment successful.");
    }

    // ── Scores ────────────────────────────────────────────────────────────────
    public (bool Success, string Message) EnterScores(string enrollmentId,
                                                       double midterm, double final)
    {
        var enrollment = _repo.GetById(enrollmentId);
        if (enrollment is null)
            return (false, $"Enrollment '{enrollmentId}' not found.");

        enrollment.MidtermScore = midterm;
        enrollment.FinalScore   = final;
        enrollment.HasScores    = true;

        _repo.Update(enrollment);
        _fileService.SaveEnrollments(_repo.GetAll());

        // Recalculate student GPA
        RecalculateStudentGpa(enrollment.StudentId);
        return (true, $"Scores saved. Average: {enrollment.AverageScore:F2} ({enrollment.LetterGrade})");
    }

    // ── Queries ───────────────────────────────────────────────────────────────
    public Enrollment? GetById(string id) => _repo.GetById(id);

    public IEnumerable<Enrollment> GetAll() => _repo.GetAll();

    public IEnumerable<Enrollment> GetForStudent(string studentId)
        => _repo.GetByStudent(studentId);

    public IEnumerable<Enrollment> GetForSubject(string subjectId)
        => _repo.GetBySubject(subjectId);

    public bool DeleteEnrollment(string id)
    {
        var e = _repo.GetById(id);
        if (e is null) return false;
        string studentId = e.StudentId;
        _repo.Delete(id);
        _fileService.SaveEnrollments(_repo.GetAll());
        RecalculateStudentGpa(studentId);
        return true;
    }

    public int TotalCount => _repo.Count;

    // ── GPA Recalculation ─────────────────────────────────────────────────────
    /// <summary>
    /// Weighted GPA: sum(gradePoint × credits) / sum(credits).
    /// Only graded (HasScores = true) enrollments are counted.
    /// </summary>
    private void RecalculateStudentGpa(string studentId)
    {
        var student = _studentRepo.GetById(studentId);
        if (student is null) return;

        var graded = _repo.GetByStudent(studentId)
                          .Where(e => e.HasScores)
                          .ToList();

        if (graded.Count == 0)
        {
            student.GPA = 0;
        }
        else
        {
            double totalCredits = 0;
            double totalPoints  = 0;

            foreach (var e in graded)
            {
                var subject = _subjectRepo.GetById(e.SubjectId);
                int credits = subject?.Credits ?? 3;
                totalCredits += credits;
                totalPoints  += e.GradePoint * credits;
            }

            student.GPA = totalCredits > 0 ? totalPoints / totalCredits : 0;
        }

        _studentRepo.Update(student);
        _fileService.SaveStudents(_studentRepo.GetAll());
    }

    public void SeedIfEmpty()
    {
        if (_repo.Count > 0) return;

        var enrollments = new List<Enrollment>
        {
            new("SV001", "SUB001", 1, 2021) { HasScores=true, MidtermScore=88, FinalScore=92 },
            new("SV001", "SUB002", 2, 2022) { HasScores=true, MidtermScore=85, FinalScore=90 },
            new("SV002", "SUB001", 1, 2020) { HasScores=true, MidtermScore=72, FinalScore=78 },
            new("SV002", "SUB003", 3, 2021) { HasScores=true, MidtermScore=68, FinalScore=75 },
            new("SV003", "SUB008", 1, 2021) { HasScores=true, MidtermScore=65, FinalScore=70 },
            new("SV004", "SUB009", 1, 2019) { HasScores=true, MidtermScore=82, FinalScore=88 },
            new("SV005", "SUB010", 5, 2022) { HasScores=true, MidtermScore=94, FinalScore=97 },
            new("SV006", "SUB006", 1, 2020) { HasScores=true, MidtermScore=55, FinalScore=60 },
            new("SV007", "SUB008", 1, 2021) { HasScores=true, MidtermScore=70, FinalScore=74 },
            new("SV008", "SUB009", 1, 2019) { HasScores=true, MidtermScore=42, FinalScore=48 },
        };

        foreach (var e in enrollments) _repo.Add(e);
        _fileService.SaveEnrollments(_repo.GetAll());

        // Recalculate GPAs for seeded data
        var studentIds = enrollments.Select(e => e.StudentId).Distinct();
        foreach (var sid in studentIds)
            RecalculateStudentGpa(sid);
    }
}
