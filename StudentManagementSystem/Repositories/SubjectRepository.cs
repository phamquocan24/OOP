using StudentManagementSystem.Interfaces;
using StudentManagementSystem.Models;

namespace StudentManagementSystem.Repositories;

/// <summary>In-memory repository for Subject entities.</summary>
public class SubjectRepository : IRepository<Subject>, ISearchable<Subject>
{
    private readonly List<Subject> _subjects = new();

    public int Count => _subjects.Count;

    public IEnumerable<Subject> GetAll() => _subjects.AsReadOnly();

    public Subject? GetById(string id)
        => _subjects.FirstOrDefault(s =>
               s.SubjectId.Equals(id, StringComparison.OrdinalIgnoreCase) ||
               s.Code.Equals(id, StringComparison.OrdinalIgnoreCase));

    public bool Add(Subject entity)
    {
        if (entity is null) return false;
        if (GetById(entity.SubjectId) is not null) return false;
        _subjects.Add(entity);
        return true;
    }

    public bool Update(Subject entity)
    {
        var index = _subjects.FindIndex(s =>
            s.SubjectId.Equals(entity.SubjectId, StringComparison.OrdinalIgnoreCase));
        if (index < 0) return false;
        _subjects[index] = entity;
        return true;
    }

    public bool Delete(string id)
    {
        var subject = GetById(id);
        if (subject is null) return false;
        _subjects.Remove(subject);
        return true;
    }

    public IEnumerable<Subject> Search(string keyword)
    {
        if (string.IsNullOrWhiteSpace(keyword)) return _subjects;
        keyword = keyword.Trim().ToLower();
        return _subjects.Where(s =>
            s.SubjectId.ToLower().Contains(keyword) ||
            s.Name.ToLower().Contains(keyword)      ||
            s.Code.ToLower().Contains(keyword)      ||
            s.FacultyId.ToLower().Contains(keyword));
    }

    public IEnumerable<Subject> Filter(Func<Subject, bool> predicate)
        => _subjects.Where(predicate);

    public void LoadAll(IEnumerable<Subject> subjects)
    {
        _subjects.Clear();
        _subjects.AddRange(subjects);
    }

    public IEnumerable<Subject> GetByFaculty(string facultyId)
        => _subjects.Where(s => s.FacultyId.Equals(facultyId, StringComparison.OrdinalIgnoreCase));
}
