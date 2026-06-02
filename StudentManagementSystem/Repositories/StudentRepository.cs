using StudentManagementSystem.Interfaces;
using StudentManagementSystem.Models;

namespace StudentManagementSystem.Repositories;

/// <summary>
/// In-memory repository for Student entities with search support.
/// Implements IRepository&lt;Student&gt; and ISearchable&lt;Student&gt;.
/// </summary>
public class StudentRepository : IRepository<Student>, ISearchable<Student>
{
    private readonly List<Student> _students = new();

    // ── IRepository<Student> ────────────────────────────────────────────────
    public int Count => _students.Count;

    public IEnumerable<Student> GetAll() => _students.AsReadOnly();

    public Student? GetById(string id)
        => _students.FirstOrDefault(s =>
               s.StudentId.Equals(id, StringComparison.OrdinalIgnoreCase) ||
               s.Id.Equals(id, StringComparison.OrdinalIgnoreCase));

    public bool Add(Student entity)
    {
        if (entity is null) return false;
        if (GetById(entity.StudentId) is not null) return false; // duplicate
        _students.Add(entity);
        return true;
    }

    public bool Update(Student entity)
    {
        var index = _students.FindIndex(s =>
            s.StudentId.Equals(entity.StudentId, StringComparison.OrdinalIgnoreCase));
        if (index < 0) return false;
        _students[index] = entity;
        return true;
    }

    public bool Delete(string id)
    {
        var student = GetById(id);
        if (student is null) return false;
        _students.Remove(student);
        return true;
    }

    // ── ISearchable<Student> ─────────────────────────────────────────────────
    public IEnumerable<Student> Search(string keyword)
    {
        if (string.IsNullOrWhiteSpace(keyword)) return _students;
        keyword = keyword.Trim().ToLower();
        return _students.Where(s =>
            s.StudentId.ToLower().Contains(keyword)   ||
            s.FullName.ToLower().Contains(keyword)    ||
            s.Major.ToLower().Contains(keyword)       ||
            s.FacultyId.ToLower().Contains(keyword)   ||
            s.Email.ToLower().Contains(keyword)       ||
            s.Phone.Contains(keyword));
    }

    public IEnumerable<Student> Filter(Func<Student, bool> predicate)
        => _students.Where(predicate);

    // ── Bulk operations ──────────────────────────────────────────────────────
    /// <summary>Replaces the entire collection (used by FileService on load).</summary>
    public void LoadAll(IEnumerable<Student> students)
    {
        _students.Clear();
        _students.AddRange(students);
    }

    // ── Sort helpers ─────────────────────────────────────────────────────────
    public IEnumerable<Student> GetSortedByName()
        => _students.OrderBy(s => s.FullName);

    public IEnumerable<Student> GetSortedByGpa(bool descending = true)
        => descending
            ? _students.OrderByDescending(s => s.GPA)
            : _students.OrderBy(s => s.GPA);

    public IEnumerable<Student> GetSortedByStudentId()
        => _students.OrderBy(s => s.StudentId);

    public IEnumerable<Student> GetByFaculty(string facultyId)
        => _students.Where(s => s.FacultyId.Equals(facultyId, StringComparison.OrdinalIgnoreCase));

    public IEnumerable<Student> GetByAcademicStatus(AcademicStatus status)
        => _students.Where(s => s.AcademicStatus == status);
}
