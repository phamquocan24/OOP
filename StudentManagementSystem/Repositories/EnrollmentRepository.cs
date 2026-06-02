using StudentManagementSystem.Interfaces;
using StudentManagementSystem.Models;

namespace StudentManagementSystem.Repositories;

/// <summary>In-memory repository for Enrollment entities.</summary>
public class EnrollmentRepository : IRepository<Enrollment>, ISearchable<Enrollment>
{
    private readonly List<Enrollment> _enrollments = new();

    public int Count => _enrollments.Count;

    public IEnumerable<Enrollment> GetAll() => _enrollments.AsReadOnly();

    public Enrollment? GetById(string id)
        => _enrollments.FirstOrDefault(e =>
               e.EnrollmentId.Equals(id, StringComparison.OrdinalIgnoreCase));

    public bool Add(Enrollment entity)
    {
        if (entity is null) return false;
        // Prevent duplicate enrollment (same student + subject + semester + year)
        bool exists = _enrollments.Any(e =>
            e.StudentId == entity.StudentId &&
            e.SubjectId == entity.SubjectId &&
            e.Semester  == entity.Semester  &&
            e.AcademicYear == entity.AcademicYear);
        if (exists) return false;
        _enrollments.Add(entity);
        return true;
    }

    public bool Update(Enrollment entity)
    {
        var index = _enrollments.FindIndex(e =>
            e.EnrollmentId.Equals(entity.EnrollmentId, StringComparison.OrdinalIgnoreCase));
        if (index < 0) return false;
        _enrollments[index] = entity;
        return true;
    }

    public bool Delete(string id)
    {
        var enrollment = GetById(id);
        if (enrollment is null) return false;
        _enrollments.Remove(enrollment);
        return true;
    }

    public IEnumerable<Enrollment> Search(string keyword)
    {
        if (string.IsNullOrWhiteSpace(keyword)) return _enrollments;
        keyword = keyword.Trim().ToLower();
        return _enrollments.Where(e =>
            e.StudentId.ToLower().Contains(keyword) ||
            e.SubjectId.ToLower().Contains(keyword) ||
            e.EnrollmentId.ToLower().Contains(keyword));
    }

    public IEnumerable<Enrollment> Filter(Func<Enrollment, bool> predicate)
        => _enrollments.Where(predicate);

    public void LoadAll(IEnumerable<Enrollment> enrollments)
    {
        _enrollments.Clear();
        _enrollments.AddRange(enrollments);
    }

    public IEnumerable<Enrollment> GetByStudent(string studentId)
        => _enrollments.Where(e => e.StudentId.Equals(studentId, StringComparison.OrdinalIgnoreCase));

    public IEnumerable<Enrollment> GetBySubject(string subjectId)
        => _enrollments.Where(e => e.SubjectId.Equals(subjectId, StringComparison.OrdinalIgnoreCase));

    public Enrollment? GetByStudentAndSubject(string studentId, string subjectId)
        => _enrollments.FirstOrDefault(e =>
               e.StudentId.Equals(studentId, StringComparison.OrdinalIgnoreCase) &&
               e.SubjectId.Equals(subjectId, StringComparison.OrdinalIgnoreCase));
}
