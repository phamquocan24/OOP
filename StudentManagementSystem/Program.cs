using StudentManagementSystem.Models;
using StudentManagementSystem.Repositories;
using StudentManagementSystem.Services;
using StudentManagementSystem.UI;

// ── Configure Console ──────────────────────────────────────────────────────────
Console.OutputEncoding = System.Text.Encoding.UTF8;
Console.InputEncoding  = System.Text.Encoding.UTF8;

// ── Bootstrap services (manual DI) ────────────────────────────────────────────
var fileService = new FileService("Data");

// Repositories
var studentRepo    = new StudentRepository();
var subjectRepo    = new SubjectRepository();
var enrollmentRepo = new EnrollmentRepository();

// Load persisted data
studentRepo.LoadAll(fileService.LoadStudents());
subjectRepo.LoadAll(fileService.LoadSubjects());
enrollmentRepo.LoadAll(fileService.LoadEnrollments());

// Faculties (static master data — seeded once)
var faculties = fileService.LoadFaculties();
if (faculties.Count == 0)
{
    faculties = new List<Faculty>
    {
        new("F01", "Faculty of Computer Science",       "CS",   "Computing, software, and AI"),
        new("F02", "Faculty of Electrical Engineering", "EE",   "Electronics and electrical systems"),
        new("F03", "Faculty of Business",               "BIZ",  "Management, accounting, economics"),
        new("F04", "Faculty of Mathematics",            "MATH", "Pure and applied mathematics"),
    };
    fileService.SaveFaculties(faculties);
}

// Services
var studentService    = new StudentService(studentRepo, fileService, faculties);
var subjectService    = new SubjectService(subjectRepo, fileService);
var enrollmentService = new EnrollmentService(enrollmentRepo, studentRepo, subjectRepo, fileService);
var reportService     = new ReportService(studentRepo, subjectRepo, enrollmentRepo);

// ── Seed demo data if first run ───────────────────────────────────────────────
subjectService.SeedIfEmpty();
studentService.SeedIfEmpty(faculties);
enrollmentService.SeedIfEmpty();

// ── Build UI ──────────────────────────────────────────────────────────────────
var studentMenu    = new StudentMenu(studentService, faculties);
var subjectMenu    = new SubjectMenu(subjectService, faculties);
var enrollmentMenu = new EnrollmentMenu(enrollmentService, studentService, subjectService);
var reportMenu     = new ReportMenu(reportService, studentService, fileService, faculties);

var mainMenu = new MainMenu(studentMenu, subjectMenu, enrollmentMenu, reportMenu, studentService);

// ── Launch ────────────────────────────────────────────────────────────────────
try
{
    mainMenu.Run();
}
catch (Exception ex)
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine($"\n  Fatal error: {ex.Message}");
    Console.WriteLine(ex.StackTrace);
    Console.ResetColor();
    Console.ReadKey();
}
