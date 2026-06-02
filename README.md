<div align="center">

# 🎓 Student Management System

**A console-based Student Management System built with C# and Object-Oriented Programming**

[![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?style=flat-square&logo=dotnet)](https://dotnet.microsoft.com/)
[![C#](https://img.shields.io/badge/C%23-12.0-239120?style=flat-square&logo=c-sharp)](https://docs.microsoft.com/en-us/dotnet/csharp/)
[![License](https://img.shields.io/badge/License-MIT-blue?style=flat-square)](LICENSE)
[![OOP](https://img.shields.io/badge/OOP-Encapsulation%20%7C%20Inheritance%20%7C%20Polymorphism%20%7C%20Abstraction-orange?style=flat-square)]()

</div>

---

## 📋 Table of Contents

- [Overview](#overview)
- [Features](#features)
- [Project Structure](#project-structure)
- [OOP Concepts Demonstrated](#oop-concepts-demonstrated)
- [Getting Started](#getting-started)
- [Usage](#usage)
- [Architecture](#architecture)
- [Data Persistence](#data-persistence)
- [Screenshots](#screenshots)
- [Contributing](#contributing)
- [License](#license)

---

## Overview

The **Student Management System** is a fully functional console application developed in **C# (.NET 8)** as part of an Object-Oriented Programming course project. It provides complete lifecycle management for students, subjects, and academic enrollments — from registration to grade reporting.

The application showcases professional software design patterns including layered architecture (Models → Repositories → Services → UI), the Repository Pattern, and dependency injection principles.

---

## Features

### 👨‍🎓 Student Management
- ➕ Add new students with full personal and academic information
- ✏️ Edit student profiles (contact info, major, enrollment status)
- 🗑️ Delete students with confirmation
- 🔍 Search by name, ID, faculty, major, email, or phone
- 📋 View full student details with GPA progress bar
- 🔃 Sort by name, GPA (highest first), or student ID
- 📄 Paginated table listing with color-coded academic standing

### 📚 Subject Management
- Full CRUD for academic subjects/courses
- Track credits, semester, faculty association, and description
- Search subjects by name, code, or faculty

### 📝 Enrollment & Scores
- Enroll students in specific subjects per semester/academic year
- Prevent duplicate enrollments automatically
- Enter midterm and final exam scores
- Auto-calculated weighted average: **Midterm × 40% + Final × 60%**
- Auto-calculated GPA on the **4.0 scale** (credit-weighted)
- Generate student transcripts with letter grades (A/B/C/D/F)

### 📊 Reports & Statistics
- **Dashboard** — live overview of all system metrics
- **Top Student Rankings** — GPA leaderboard with medals 🥇🥈🥉
- **GPA Distribution** — ASCII bar chart by academic standing
- **Faculty Breakdown** — enrollment by department
- **Gender Distribution** — visual bar chart
- **Enrollment by Year** — cohort analysis
- **Subject Pass Rates** — color-coded by performance
- **CSV Export** — export student data for external analysis

### 💾 Data Persistence
- All data auto-saved to JSON files after every change
- Auto-loaded on startup — no data loss between sessions
- Seeded with 10 sample students, 10 subjects, and 10 enrollments on first run

---

## Project Structure

```
StudentManagementSystem/
│
├── Models/                        # Domain entities
│   ├── Enums.cs                   # Gender, AcademicStatus, EnrollmentStatus, LetterGrade
│   ├── Person.cs                  # Abstract base class (Encapsulation + Abstraction)
│   ├── Student.cs                 # Inherits Person (Inheritance + Polymorphism)
│   ├── Subject.cs                 # Academic course entity
│   ├── Enrollment.cs              # Student–Subject join with score computation
│   └── Faculty.cs                 # Department entity
│
├── Interfaces/                    # Contracts (Abstraction)
│   ├── IRepository.cs             # Generic CRUD interface
│   └── ISearchable.cs             # Generic search interface
│
├── Repositories/                  # Data access layer
│   ├── StudentRepository.cs       # Implements IRepository<Student> + ISearchable<Student>
│   ├── SubjectRepository.cs       # Implements IRepository<Subject>
│   └── EnrollmentRepository.cs    # Implements IRepository<Enrollment>
│
├── Services/                      # Business logic layer
│   ├── FileService.cs             # JSON serialization / CSV export
│   ├── StudentService.cs          # Student business logic + seeder
│   ├── SubjectService.cs          # Subject business logic + seeder
│   ├── EnrollmentService.cs       # Enrollment logic + GPA recalculation
│   └── ReportService.cs           # Statistics and report generation
│
├── UI/                            # Presentation layer
│   ├── ConsoleRenderer.cs         # Shared rendering utilities (banner, tables, colors)
│   ├── MainMenu.cs                # Application root / navigation hub
│   ├── StudentMenu.cs             # Student CRUD screens
│   ├── SubjectMenu.cs             # Subject CRUD screens
│   ├── EnrollmentMenu.cs          # Enrollment & score screens
│   └── ReportMenu.cs              # Report and statistics screens
│
├── Utils/                         # Shared utilities
│   ├── ColorConsole.cs            # Colored console output helpers
│   ├── TablePrinter.cs            # ASCII box-drawing table renderer
│   └── Validator.cs               # Input validation and parsing helpers
│
├── Data/                          # Auto-generated JSON data files
│   ├── students.json
│   ├── subjects.json
│   ├── enrollments.json
│   └── faculties.json
│
├── Program.cs                     # Entry point + manual DI wiring
└── StudentManagementSystem.csproj
```

---

## OOP Concepts Demonstrated

| Concept | Implementation |
|---|---|
| **Encapsulation** | Private backing fields with validated property setters in `Person`, `Student`, `Subject`, `Enrollment` |
| **Inheritance** | `Student` inherits `Person`, gaining all base properties and overriding virtual methods |
| **Polymorphism** | `GetSummary()` and `DisplayInfo()` are abstract/virtual in `Person`, overridden in `Student` |
| **Abstraction** | `Person` is an abstract class; `IRepository<T>` and `ISearchable<T>` are generic interfaces |
| **Generics** | `IRepository<T>` provides a type-safe, reusable CRUD contract |
| **Interfaces** | `StudentRepository` implements both `IRepository<Student>` and `ISearchable<Student>` |
| **Exception Handling** | Property setters throw `ArgumentException`/`ArgumentOutOfRangeException` on invalid data |
| **LINQ** | Extensive use of LINQ for filtering, sorting, grouping, and aggregation |

### Class Diagram (simplified)

```
           ┌─────────────────┐
           │   <<abstract>>  │
           │     Person      │
           │─────────────────│
           │ +Id             │
           │ +FullName       │
           │ +DateOfBirth    │
           │ +Age (computed) │
           │─────────────────│
           │ +GetSummary()◄► │  abstract
           │ +DisplayInfo()▼ │  virtual
           └────────┬────────┘
                    │  inherits
           ┌────────▼────────┐
           │     Student     │
           │─────────────────│
           │ +StudentId      │
           │ +Major          │
           │ +GPA            │
           │ +AcademicStatus │  ← computed from GPA
           │─────────────────│
           │ +GetSummary()▲  │  override
           │ +DisplayInfo()▲ │  override
           └─────────────────┘

  ┌─────────────────────────────────────┐
  │        <<interface>>                │
  │        IRepository<T>              │
  │─────────────────────────────────────│
  │ +GetAll() : IEnumerable<T>          │
  │ +GetById(id) : T?                   │
  │ +Add(entity) : bool                 │
  │ +Update(entity) : bool              │
  │ +Delete(id) : bool                  │
  │ +Count : int                        │
  └─────────────────────────────────────┘
          ▲ implemented by
          │
  StudentRepository, SubjectRepository, EnrollmentRepository
```

---

## Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) or newer

### Installation

1. **Clone the repository**
   ```bash
   git clone https://github.com/phamquocan24/OOP.git
   cd OOP/StudentManagementSystem
   ```

2. **Build the project**
   ```bash
   dotnet build
   ```

3. **Run the application**
   ```bash
   dotnet run
   ```

On first launch, the application automatically seeds 10 students, 10 subjects, and 10 sample enrollments so you can explore all features immediately.

---

## Usage

### Navigation

- Use the **number keys** to select menu options
- Press **Enter** to confirm input
- Press **0** to go back / exit any menu
- Leave fields blank and press Enter to **keep the current value** when editing

### Sample Workflows

**Add a student:**
> Main Menu → `[1]` Student Management → `[4]` Add New Student → Follow prompts

**Enter grades:**
> Main Menu → `[3]` Enrollment & Scores → `[2]` Enter / Update Scores → Enter Student ID → Select subject → Enter scores

**View transcript:**
> Main Menu → `[3]` Enrollment & Scores → `[3]` View Student Transcript → Enter Student ID

**Top 5 students:**
> Main Menu → `[4]` Reports & Statistics → `[2]` Top Students Ranking → Enter `5`

**Export to CSV:**
> Main Menu → `[4]` Reports & Statistics → `[8]` Export Students to CSV

---

## Architecture

The application follows a **layered (N-Tier) architecture**:

```
┌──────────────────────────────────┐
│         UI Layer                 │  ← User interaction (menus, tables)
│  MainMenu / StudentMenu / ...    │
├──────────────────────────────────┤
│        Service Layer             │  ← Business logic, GPA calculation
│  StudentService / ReportService  │
├──────────────────────────────────┤
│      Repository Layer            │  ← Data access (CRUD, search, sort)
│  StudentRepository / ...         │
├──────────────────────────────────┤
│        Model Layer               │  ← Domain entities
│  Person / Student / Enrollment   │
├──────────────────────────────────┤
│      Infrastructure              │  ← File I/O (JSON, CSV)
│  FileService                     │
└──────────────────────────────────┘
```

### GPA Calculation

Student GPA is **credit-weighted** and auto-recalculated after any score change:

```
GPA = Σ(GradePoint_i × Credits_i) / Σ(Credits_i)
```

Individual subject scores use a weighted formula:
```
AverageScore = MidtermScore × 40% + FinalScore × 60%
```

Score → GPA Point mapping:

| Score Range | Letter | GPA Points |
|---|---|---|
| 90 – 100 | A | 4.0 |
| 85 – 89 | A- | 3.7 |
| 80 – 84 | B+ | 3.3 |
| 75 – 79 | B | 3.0 |
| 70 – 74 | B- | 2.7 |
| 65 – 69 | C+ | 2.3 |
| 60 – 64 | C | 2.0 |
| 50 – 59 | D | 1.0 – 1.7 |
| < 50 | F | 0.0 |

---

## Data Persistence

All data is stored as **human-readable JSON** in the `Data/` directory:

```
Data/
├── students.json      ← All student records
├── subjects.json      ← All subject records
├── enrollments.json   ← All enrollment records with scores
└── faculties.json     ← Faculty/department definitions
```

The `Data/` folder is created automatically on first run. JSON files are updated immediately after every add, edit, delete, or score change — ensuring zero data loss.

---

## Academic Status Reference

| GPA Range | Status | Color |
|---|---|---|
| 3.6 – 4.0 | Excellent ⭐ | Green |
| 3.2 – 3.5 | Good ✅ | Cyan |
| 2.5 – 3.1 | Average 🔵 | White |
| 2.0 – 2.4 | Below Average ⚠️ | Yellow |
| 1.0 – 1.9 | Poor ❌ | Red |
| < 1.0 | Expelled 🚫 | Red |

---

## Contributing

Contributions are welcome! Please follow these steps:

1. Fork the repository
2. Create your feature branch: `git checkout -b feature/AmazingFeature`
3. Commit your changes: `git commit -m 'Add AmazingFeature'`
4. Push to the branch: `git push origin feature/AmazingFeature`
5. Open a Pull Request

---

## License

This project is licensed under the **MIT License** — see the [LICENSE](LICENSE) file for details.

---

<div align="center">

Made with ❤️ for the **Object-Oriented Programming** course

**phamquocan24** · [GitHub](https://github.com/phamquocan24)

</div>
