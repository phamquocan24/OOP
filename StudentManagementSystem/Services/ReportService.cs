using StudentManagementSystem.Models;
using StudentManagementSystem.Repositories;

namespace StudentManagementSystem.Services;

/// <summary>Generates statistical reports across students, subjects, and enrollments.</summary>
public class ReportService
{
    private readonly StudentRepository    _studentRepo;
    private readonly SubjectRepository    _subjectRepo;
    private readonly EnrollmentRepository _enrollmentRepo;

    public ReportService(StudentRepository s, SubjectRepository sub, EnrollmentRepository e)
    {
        _studentRepo    = s;
        _subjectRepo    = sub;
        _enrollmentRepo = e;
    }

    // ── Student Stats ─────────────────────────────────────────────────────────
    public double GetAverageGpa()
    {
        var students = _studentRepo.GetAll().ToList();
        return students.Count == 0 ? 0 : Math.Round(students.Average(s => s.GPA), 2);
    }

    public double GetHighestGpa() => _studentRepo.GetAll().Max(s => s.GPA);
    public double GetLowestGpa()  => _studentRepo.GetAll().Min(s => s.GPA);

    public IEnumerable<Student> GetTopStudents(int n = 10)
        => _studentRepo.GetSortedByGpa().Take(n);

    public Dictionary<AcademicStatus, int> GetDistributionByAcademicStatus()
        => _studentRepo.GetAll()
            .GroupBy(s => s.AcademicStatus)
            .ToDictionary(g => g.Key, g => g.Count());

    public Dictionary<string, int> GetDistributionByFaculty()
        => _studentRepo.GetAll()
            .GroupBy(s => s.FacultyId)
            .ToDictionary(g => g.Key, g => g.Count());

    public Dictionary<int, int> GetDistributionByYear()
        => _studentRepo.GetAll()
            .GroupBy(s => s.EnrollmentYear)
            .OrderBy(g => g.Key)
            .ToDictionary(g => g.Key, g => g.Count());

    public Dictionary<Gender, int> GetDistributionByGender()
        => _studentRepo.GetAll()
            .GroupBy(s => s.Gender)
            .ToDictionary(g => g.Key, g => g.Count());

    // ── Subject Stats ─────────────────────────────────────────────────────────
    public Dictionary<string, int> GetEnrollmentCountBySubject()
    {
        var counts = _enrollmentRepo.GetAll()
            .GroupBy(e => e.SubjectId)
            .ToDictionary(g => g.Key, g => g.Count());

        return _subjectRepo.GetAll()
            .ToDictionary(s => s.Name, s => counts.GetValueOrDefault(s.SubjectId, 0));
    }

    public Dictionary<string, double> GetPassRateBySubject()
    {
        var result = new Dictionary<string, double>();
        foreach (var subject in _subjectRepo.GetAll())
        {
            var graded = _enrollmentRepo.GetBySubject(subject.SubjectId)
                                        .Where(e => e.HasScores).ToList();
            if (graded.Count == 0)
            {
                result[subject.Name] = 0;
                continue;
            }
            double passRate = (double)graded.Count(e => e.IsPassed) / graded.Count * 100;
            result[subject.Name] = Math.Round(passRate, 1);
        }
        return result;
    }

    // ── Overall Stats ─────────────────────────────────────────────────────────
    public (int TotalStudents, int TotalSubjects, int TotalEnrollments,
            double AvgGpa, double HighGpa, double LowGpa) GetOverallStats()
    {
        var students = _studentRepo.GetAll().ToList();
        return (
            students.Count,
            _subjectRepo.Count,
            _enrollmentRepo.Count,
            students.Count > 0 ? Math.Round(students.Average(s => s.GPA), 2) : 0,
            students.Count > 0 ? students.Max(s => s.GPA) : 0,
            students.Count > 0 ? students.Min(s => s.GPA) : 0
        );
    }
}
