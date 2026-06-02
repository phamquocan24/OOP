namespace StudentManagementSystem.Models;

/// <summary>
/// Abstract base class representing a person in the system.
/// Demonstrates encapsulation and abstraction.
/// </summary>
public abstract class Person
{
    // ── Private backing fields (Encapsulation) ──────────────────────────────
    private string _fullName = string.Empty;
    private string _email    = string.Empty;
    private string _phone    = string.Empty;

    // ── Properties ──────────────────────────────────────────────────────────
    public string Id          { get; protected set; } = Guid.NewGuid().ToString("N")[..8].ToUpper();
    public Gender Gender      { get; set; }
    public DateTime DateOfBirth { get; set; }

    public string FullName
    {
        get => _fullName;
        set => _fullName = string.IsNullOrWhiteSpace(value)
            ? throw new ArgumentException("Full name cannot be empty.", nameof(value))
            : value.Trim();
    }

    public string Email
    {
        get => _email;
        set
        {
            if (!string.IsNullOrEmpty(value) && !value.Contains('@'))
                throw new ArgumentException("Invalid email address.", nameof(value));
            _email = value.Trim();
        }
    }

    public string Phone
    {
        get => _phone;
        set => _phone = value.Trim();
    }

    /// <summary>Calculated age in years based on DateOfBirth.</summary>
    public int Age
    {
        get
        {
            var today = DateTime.Today;
            var age = today.Year - DateOfBirth.Year;
            if (DateOfBirth.Date > today.AddYears(-age)) age--;
            return age;
        }
    }

    // ── Constructors ─────────────────────────────────────────────────────────
    protected Person() { }

    protected Person(string id, string fullName, DateTime dateOfBirth, Gender gender,
                     string email = "", string phone = "")
    {
        Id          = id;
        FullName    = fullName;
        DateOfBirth = dateOfBirth;
        Gender      = gender;
        Email       = email;
        Phone       = phone;
    }

    // ── Abstract / Virtual methods (Polymorphism) ────────────────────────────
    /// <summary>Returns a short summary string — must be overridden.</summary>
    public abstract string GetSummary();

    /// <summary>Displays person info to console — can be overridden.</summary>
    public virtual void DisplayInfo()
    {
        Console.WriteLine($"  ID       : {Id}");
        Console.WriteLine($"  Name     : {FullName}");
        Console.WriteLine($"  DOB      : {DateOfBirth:dd/MM/yyyy}  (Age: {Age})");
        Console.WriteLine($"  Gender   : {Gender}");
        Console.WriteLine($"  Email    : {(string.IsNullOrEmpty(Email) ? "N/A" : Email)}");
        Console.WriteLine($"  Phone    : {(string.IsNullOrEmpty(Phone) ? "N/A" : Phone)}");
    }

    public override string ToString() => $"[{Id}] {FullName}";
}
