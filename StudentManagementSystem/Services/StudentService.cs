using StudentManagementSystem.Models;
using StudentManagementSystem.Repositories;
using StudentManagementSystem.Services;

namespace StudentManagementSystem.Services;

/// <summary>
/// Business logic layer for student management.
/// Coordinates between StudentRepository and FileService.
/// </summary>
public class StudentService
{
    private readonly StudentRepository _repo;
    private readonly FileService       _fileService;
    private readonly List<Faculty>     _faculties;

    public StudentService(StudentRepository repo, FileService fileService, List<Faculty> faculties)
    {
        _repo        = repo;
        _fileService = fileService;
        _faculties   = faculties;
    }

    // ── CRUD ─────────────────────────────────────────────────────────────────
    public bool AddStudent(Student student)
    {
        if (!_repo.Add(student)) return false;
        _fileService.SaveStudents(_repo.GetAll());
        return true;
    }

    public bool UpdateStudent(Student student)
    {
        if (!_repo.Update(student)) return false;
        _fileService.SaveStudents(_repo.GetAll());
        return true;
    }

    public bool DeleteStudent(string studentId)
    {
        if (!_repo.Delete(studentId)) return false;
        _fileService.SaveStudents(_repo.GetAll());
        return true;
    }

    public Student? GetStudent(string studentId) => _repo.GetById(studentId);

    public IEnumerable<Student> GetAll()              => _repo.GetAll();
    public IEnumerable<Student> Search(string q)      => _repo.Search(q);
    public IEnumerable<Student> GetSortedByName()     => _repo.GetSortedByName();
    public IEnumerable<Student> GetSortedByGpa()      => _repo.GetSortedByGpa();
    public IEnumerable<Student> GetByFaculty(string f)=> _repo.GetByFaculty(f);

    public IEnumerable<Student> GetByStatus(AcademicStatus status)
        => _repo.GetByAcademicStatus(status);

    public int TotalCount => _repo.Count;

    // ── GPA Update ────────────────────────────────────────────────────────────
    /// <summary>Recalculates and stores the student's GPA based on all enrollments.</summary>
    public void UpdateStudentGpa(string studentId, double newGpa)
    {
        var student = _repo.GetById(studentId);
        if (student is null) return;
        student.GPA = newGpa;
        _repo.Update(student);
        _fileService.SaveStudents(_repo.GetAll());
    }

    // ── Seed Data ─────────────────────────────────────────────────────────────
    /// <summary>Seeds the repository with sample data when the store is empty.</summary>
    public void SeedIfEmpty(IEnumerable<Faculty> faculties)
    {
        if (_repo.Count > 0) return;

        var facultyList = faculties.ToList();
        var csId  = facultyList.FirstOrDefault(f => f.Code == "CS")?.FacultyId  ?? "F01";
        var eeId  = facultyList.FirstOrDefault(f => f.Code == "EE")?.FacultyId  ?? "F02";
        var bizId = facultyList.FirstOrDefault(f => f.Code == "BIZ")?.FacultyId ?? "F03";
        var mathId= facultyList.FirstOrDefault(f => f.Code == "MATH")?.FacultyId?? "F04";

        var seeds = new List<Student>
        {
            new("S001ID", "SV001", "Alice Nguyen",    new(2003, 3, 15), Gender.Female, csId,   "Software Engineering",    2021, "alice@uni.edu",   "0901234567", "123 Le Loi, Hanoi"),
            new("S002ID", "SV002", "Bob Tran",         new(2002, 7, 22), Gender.Male,   csId,   "Computer Networks",       2020, "bob@uni.edu",     "0912345678", "45 Tran Hung Dao, HCM"),
            new("S003ID", "SV003", "Carol Le",         new(2003, 11, 8), Gender.Female, eeId,   "Electrical Engineering",  2021, "carol@uni.edu",   "0923456789", "78 Nguyen Hue, Da Nang"),
            new("S004ID", "SV004", "David Pham",       new(2001, 5, 30), Gender.Male,   bizId,  "Business Administration",  2019, "david@uni.edu",   "0934567890", "12 Ly Thuong Kiet, Hue"),
            new("S005ID", "SV005", "Emma Hoang",       new(2004, 1, 17), Gender.Female, csId,   "Artificial Intelligence", 2022, "emma@uni.edu",    "0945678901", "56 Hai Ba Trung, Hanoi"),
            new("S006ID", "SV006", "Frank Vo",         new(2002, 9, 4),  Gender.Male,   mathId, "Applied Mathematics",     2020, "frank@uni.edu",   "0956789012", "90 Dinh Tien Hoang, HCM"),
            new("S007ID", "SV007", "Grace Bui",        new(2003, 6, 25), Gender.Female, eeId,   "Electronics",             2021, "grace@uni.edu",   "0967890123", "34 Vo Thi Sau, Da Nang"),
            new("S008ID", "SV008", "Henry Dang",       new(2001, 12, 3), Gender.Male,   bizId,  "Accounting & Finance",    2019, "henry@uni.edu",   "0978901234", "67 Tran Phu, Nha Trang"),
            new("S009ID", "SV009", "Iris Luu",         new(2004, 4, 14), Gender.Female, csId,   "Data Science",            2022, "iris@uni.edu",    "0989012345", "23 Nguyen Du, Hanoi"),
            new("S010ID", "SV010", "Jack Dinh",        new(2003, 8, 19), Gender.Male,   mathId, "Statistics",              2021, "jack@uni.edu",    "0990123456", "11 Le Duan, HCM"),
        };

        // Assign GPAs
        double[] gpas = { 3.8, 3.1, 2.7, 3.5, 3.9, 2.3, 2.9, 1.8, 3.6, 3.2 };
        for (int i = 0; i < seeds.Count; i++)
            seeds[i].GPA = gpas[i];

        foreach (var s in seeds)
            _repo.Add(s);

        _fileService.SaveStudents(_repo.GetAll());
    }
}
