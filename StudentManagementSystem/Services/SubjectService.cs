using StudentManagementSystem.Models;
using StudentManagementSystem.Repositories;

namespace StudentManagementSystem.Services;

/// <summary>Business logic for subject management.</summary>
public class SubjectService
{
    private readonly SubjectRepository _repo;
    private readonly FileService       _fileService;

    public SubjectService(SubjectRepository repo, FileService fileService)
    {
        _repo        = repo;
        _fileService = fileService;
    }

    public bool AddSubject(Subject subject)
    {
        if (!_repo.Add(subject)) return false;
        _fileService.SaveSubjects(_repo.GetAll());
        return true;
    }

    public bool UpdateSubject(Subject subject)
    {
        if (!_repo.Update(subject)) return false;
        _fileService.SaveSubjects(_repo.GetAll());
        return true;
    }

    public bool DeleteSubject(string subjectId)
    {
        if (!_repo.Delete(subjectId)) return false;
        _fileService.SaveSubjects(_repo.GetAll());
        return true;
    }

    public Subject? GetSubject(string id)         => _repo.GetById(id);
    public IEnumerable<Subject> GetAll()          => _repo.GetAll();
    public IEnumerable<Subject> Search(string q)  => _repo.Search(q);
    public int TotalCount                         => _repo.Count;

    public void SeedIfEmpty()
    {
        if (_repo.Count > 0) return;

        var seeds = new List<Subject>
        {
            new("SUB001", "Introduction to Programming",  "CS101", 3, "F01", 1, "Basics of programming with C#"),
            new("SUB002", "Object-Oriented Programming",  "CS201", 3, "F01", 2, "OOP concepts with C# and Java"),
            new("SUB003", "Data Structures & Algorithms", "CS301", 4, "F01", 3, "Fundamental data structures"),
            new("SUB004", "Database Management Systems",  "CS302", 3, "F01", 3, "SQL, NoSQL, and DB design"),
            new("SUB005", "Computer Networks",            "CS401", 3, "F01", 4, "Networking protocols and architecture"),
            new("SUB006", "Calculus I",                   "MA101", 3, "F04", 1, "Limits, derivatives, and integrals"),
            new("SUB007", "Linear Algebra",               "MA201", 3, "F04", 2, "Matrices, vectors, eigenvalues"),
            new("SUB008", "Circuit Theory",               "EE101", 4, "F02", 1, "Basic electrical circuits"),
            new("SUB009", "Principles of Management",     "BIZ101", 3, "F03", 1, "Intro to business management"),
            new("SUB010", "Artificial Intelligence",      "CS501", 3, "F01", 5, "AI algorithms and applications"),
        };

        foreach (var s in seeds) _repo.Add(s);
        _fileService.SaveSubjects(_repo.GetAll());
    }
}
