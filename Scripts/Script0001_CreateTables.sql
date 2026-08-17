-- ==========================================================
-- Student Registration Portal - Tables Schema (MySQL 8.4)
-- Creates the 24 Relational Tables with Constraints & Indexes
-- ==========================================================

CREATE DATABASE IF NOT EXISTS StudentRegistrationPortal;
USE StudentRegistrationPortal;

-- Disable Foreign Key Checks during batch table creation
SET FOREIGN_KEY_CHECKS = 0;

-- 1. Users
CREATE TABLE IF NOT EXISTS Users (
    UserId INT AUTO_INCREMENT PRIMARY KEY,
    Email VARCHAR(255) NOT NULL UNIQUE,
    PasswordHash VARCHAR(255) NOT NULL,
    IsActive TINYINT(1) NOT NULL DEFAULT 1,
    CreatedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP
) ENGINE=InnoDB;

-- 2. Roles
CREATE TABLE IF NOT EXISTS Roles (
    RoleId INT AUTO_INCREMENT PRIMARY KEY,
    RoleName VARCHAR(50) NOT NULL UNIQUE,
    Description VARCHAR(255) NULL
) ENGINE=InnoDB;

-- 3. UserRoles (Junction Table)
CREATE TABLE IF NOT EXISTS UserRoles (
    UserId INT NOT NULL,
    RoleId INT NOT NULL,
    AssignedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (UserId, RoleId),
    FOREIGN KEY (UserId) REFERENCES Users(UserId) ON DELETE CASCADE,
    FOREIGN KEY (RoleId) REFERENCES Roles(RoleId) ON DELETE CASCADE
) ENGINE=InnoDB;

-- 4. Departments
CREATE TABLE IF NOT EXISTS Departments (
    DepartmentId INT AUTO_INCREMENT PRIMARY KEY,
    DepartmentCode VARCHAR(20) NOT NULL UNIQUE,
    DepartmentName VARCHAR(100) NOT NULL,
    CreatedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP
) ENGINE=InnoDB;

-- 5. StudentStatuses
CREATE TABLE IF NOT EXISTS StudentStatuses (
    StudentStatusId INT AUTO_INCREMENT PRIMARY KEY,
    StatusName VARCHAR(50) NOT NULL UNIQUE,
    Description VARCHAR(255) NULL
) ENGINE=InnoDB;

-- 6. Students
CREATE TABLE IF NOT EXISTS Students (
    StudentId INT AUTO_INCREMENT PRIMARY KEY,
    UserId INT NOT NULL UNIQUE,
    DepartmentId INT NOT NULL,
    StudentStatusId INT NOT NULL,
    StudentNumber VARCHAR(30) NOT NULL UNIQUE,
    FirstName VARCHAR(100) NOT NULL,
    MiddleName VARCHAR(100) NULL,
    LastName VARCHAR(100) NOT NULL,
    NationalId VARCHAR(30) NULL UNIQUE,
    DateOfBirth DATE NOT NULL,
    Gender VARCHAR(20) NULL,
    PhoneNumber VARCHAR(30) NULL,
    Address VARCHAR(255) NULL,
    AdmissionDate DATE NOT NULL,
    AcademicLevel INT NOT NULL DEFAULT 1,
    GPA DECIMAL(3, 2) NOT NULL DEFAULT 0.00,
    CompletedCreditHours INT NOT NULL DEFAULT 0,
    CreatedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    FOREIGN KEY (UserId) REFERENCES Users(UserId) ON DELETE RESTRICT,
    FOREIGN KEY (DepartmentId) REFERENCES Departments(DepartmentId) ON DELETE RESTRICT,
    FOREIGN KEY (StudentStatusId) REFERENCES StudentStatuses(StudentStatusId) ON DELETE RESTRICT
) ENGINE=InnoDB;

-- 7. InstructorStatuses
CREATE TABLE IF NOT EXISTS InstructorStatuses (
    InstructorStatusId INT AUTO_INCREMENT PRIMARY KEY,
    StatusName VARCHAR(50) NOT NULL UNIQUE,
    Description VARCHAR(255) NULL
) ENGINE=InnoDB;

-- 8. Instructors
CREATE TABLE IF NOT EXISTS Instructors (
    InstructorId INT AUTO_INCREMENT PRIMARY KEY,
    UserId INT NOT NULL UNIQUE,
    DepartmentId INT NOT NULL,
    InstructorStatusId INT NOT NULL,
    EmployeeNumber VARCHAR(30) NOT NULL UNIQUE,
    FirstName VARCHAR(100) NOT NULL,
    MiddleName VARCHAR(100) NULL,
    LastName VARCHAR(100) NOT NULL,
    AcademicTitle VARCHAR(50) NOT NULL,
    Salary DECIMAL(12, 2) NOT NULL DEFAULT 0.00,
    HireDate DATE NOT NULL,
    CreatedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    FOREIGN KEY (UserId) REFERENCES Users(UserId) ON DELETE RESTRICT,
    FOREIGN KEY (DepartmentId) REFERENCES Departments(DepartmentId) ON DELETE RESTRICT,
    FOREIGN KEY (InstructorStatusId) REFERENCES InstructorStatuses(InstructorStatusId) ON DELETE RESTRICT
) ENGINE=InnoDB;

-- 9. CourseStatuses
CREATE TABLE IF NOT EXISTS CourseStatuses (
    CourseStatusId INT AUTO_INCREMENT PRIMARY KEY,
    StatusName VARCHAR(50) NOT NULL UNIQUE,
    Description VARCHAR(255) NULL
) ENGINE=InnoDB;

-- 10. Courses
CREATE TABLE IF NOT EXISTS Courses (
    CourseId INT AUTO_INCREMENT PRIMARY KEY,
    CourseCode VARCHAR(20) NOT NULL UNIQUE,
    CourseName VARCHAR(150) NOT NULL,
    CreditHours INT NOT NULL DEFAULT 3,
    DifficultyLevel VARCHAR(50) NOT NULL DEFAULT 'Undergraduate',
    CourseStatusId INT NOT NULL,
    CreatedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    FOREIGN KEY (CourseStatusId) REFERENCES CourseStatuses(CourseStatusId) ON DELETE RESTRICT
) ENGINE=InnoDB;

-- 11. CourseDepartments (Junction Table)
CREATE TABLE IF NOT EXISTS CourseDepartments (
    CourseId INT NOT NULL,
    DepartmentId INT NOT NULL,
    PRIMARY KEY (CourseId, DepartmentId),
    FOREIGN KEY (CourseId) REFERENCES Courses(CourseId) ON DELETE CASCADE,
    FOREIGN KEY (DepartmentId) REFERENCES Departments(DepartmentId) ON DELETE CASCADE
) ENGINE=InnoDB;

-- 12. CoursePrerequisites
CREATE TABLE IF NOT EXISTS CoursePrerequisites (
    CourseId INT NOT NULL,
    PrerequisiteCourseId INT NOT NULL,
    PRIMARY KEY (CourseId, PrerequisiteCourseId),
    FOREIGN KEY (CourseId) REFERENCES Courses(CourseId) ON DELETE RESTRICT,
    FOREIGN KEY (PrerequisiteCourseId) REFERENCES Courses(CourseId) ON DELETE RESTRICT
) ENGINE=InnoDB;

-- 13. Semesters
CREATE TABLE IF NOT EXISTS Semesters (
    SemesterId INT AUTO_INCREMENT PRIMARY KEY,
    SemesterName VARCHAR(50) NOT NULL,
    AcademicYear INT NOT NULL,
    StartDate DATE NOT NULL,
    EndDate DATE NOT NULL,
    IsCurrent TINYINT(1) NOT NULL DEFAULT 0
) ENGINE=InnoDB;

-- 14. Rooms
CREATE TABLE IF NOT EXISTS Rooms (
    RoomId INT AUTO_INCREMENT PRIMARY KEY,
    RoomNumber VARCHAR(30) NOT NULL UNIQUE,
    BuildingName VARCHAR(100) NOT NULL,
    Capacity INT NOT NULL DEFAULT 30
) ENGINE=InnoDB;

-- 15. OfferingStatuses
CREATE TABLE IF NOT EXISTS OfferingStatuses (
    OfferingStatusId INT AUTO_INCREMENT PRIMARY KEY,
    StatusName VARCHAR(50) NOT NULL UNIQUE,
    Description VARCHAR(255) NULL
) ENGINE=InnoDB;

-- 16. CourseOfferings
CREATE TABLE IF NOT EXISTS CourseOfferings (
    CourseOfferingId INT AUTO_INCREMENT PRIMARY KEY,
    CourseId INT NOT NULL,
    SemesterId INT NOT NULL,
    OfferingStatusId INT NOT NULL,
    SectionNumber VARCHAR(20) NOT NULL,
    Capacity INT NOT NULL DEFAULT 30,
    CreatedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (CourseId) REFERENCES Courses(CourseId) ON DELETE RESTRICT,
    FOREIGN KEY (SemesterId) REFERENCES Semesters(SemesterId) ON DELETE RESTRICT,
    FOREIGN KEY (OfferingStatusId) REFERENCES OfferingStatuses(OfferingStatusId) ON DELETE RESTRICT
) ENGINE=InnoDB;

-- 17. CourseOfferingInstructors (Junction Table)
CREATE TABLE IF NOT EXISTS CourseOfferingInstructors (
    CourseOfferingId INT NOT NULL,
    InstructorId INT NOT NULL,
    IsPrimary TINYINT(1) NOT NULL DEFAULT 1,
    PRIMARY KEY (CourseOfferingId, InstructorId),
    FOREIGN KEY (CourseOfferingId) REFERENCES CourseOfferings(CourseOfferingId) ON DELETE CASCADE,
    FOREIGN KEY (InstructorId) REFERENCES Instructors(InstructorId) ON DELETE RESTRICT
) ENGINE=InnoDB;

-- 18. EnrollmentStatuses
CREATE TABLE IF NOT EXISTS EnrollmentStatuses (
    EnrollmentStatusId INT AUTO_INCREMENT PRIMARY KEY,
    StatusName VARCHAR(50) NOT NULL UNIQUE,
    Description VARCHAR(255) NULL
) ENGINE=InnoDB;

-- 19. Enrollments
CREATE TABLE IF NOT EXISTS Enrollments (
    EnrollmentId INT AUTO_INCREMENT PRIMARY KEY,
    StudentId INT NOT NULL,
    CourseOfferingId INT NOT NULL,
    EnrollmentStatusId INT NOT NULL,
    RegistrationDate DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    TotalGrade DECIMAL(5, 2) NULL,
    LetterGrade VARCHAR(5) NULL,
    GradePoints DECIMAL(3, 2) NULL,
    FOREIGN KEY (StudentId) REFERENCES Students(StudentId) ON DELETE CASCADE,
    FOREIGN KEY (CourseOfferingId) REFERENCES CourseOfferings(CourseOfferingId) ON DELETE RESTRICT,
    FOREIGN KEY (EnrollmentStatusId) REFERENCES EnrollmentStatuses(EnrollmentStatusId) ON DELETE RESTRICT
) ENGINE=InnoDB;

-- 20. CourseSchedules
CREATE TABLE IF NOT EXISTS CourseSchedules (
    CourseScheduleId INT AUTO_INCREMENT PRIMARY KEY,
    CourseOfferingId INT NOT NULL,
    RoomId INT NOT NULL,
    DayOfWeek INT NOT NULL,
    StartTime TIME NOT NULL,
    EndTime TIME NOT NULL,
    FOREIGN KEY (CourseOfferingId) REFERENCES CourseOfferings(CourseOfferingId) ON DELETE CASCADE,
    FOREIGN KEY (RoomId) REFERENCES Rooms(RoomId) ON DELETE RESTRICT
) ENGINE=InnoDB;

-- 21. Lectures
CREATE TABLE IF NOT EXISTS Lectures (
    LectureId INT AUTO_INCREMENT PRIMARY KEY,
    CourseScheduleId INT NOT NULL,
    LectureDate DATE NOT NULL,
    Topic VARCHAR(255) NULL,
    FOREIGN KEY (CourseScheduleId) REFERENCES CourseSchedules(CourseScheduleId) ON DELETE CASCADE
) ENGINE=InnoDB;

-- 22. AttendanceStatuses
CREATE TABLE IF NOT EXISTS AttendanceStatuses (
    AttendanceStatusId INT AUTO_INCREMENT PRIMARY KEY,
    StatusName VARCHAR(50) NOT NULL UNIQUE
) ENGINE=InnoDB;

-- 23. Attendance
CREATE TABLE IF NOT EXISTS Attendance (
    AttendanceId INT AUTO_INCREMENT PRIMARY KEY,
    LectureId INT NOT NULL,
    StudentId INT NOT NULL,
    AttendanceStatusId INT NOT NULL,
    Remarks VARCHAR(255) NULL,
    FOREIGN KEY (LectureId) REFERENCES Lectures(LectureId) ON DELETE CASCADE,
    FOREIGN KEY (StudentId) REFERENCES Students(StudentId) ON DELETE RESTRICT,
    FOREIGN KEY (AttendanceStatusId) REFERENCES AttendanceStatuses(AttendanceStatusId) ON DELETE RESTRICT
) ENGINE=InnoDB;

-- 24. StudentHolds
CREATE TABLE IF NOT EXISTS StudentHolds (
    StudentHoldId INT AUTO_INCREMENT PRIMARY KEY,
    StudentId INT NOT NULL,
    HoldType VARCHAR(50) NOT NULL,
    Reason VARCHAR(255) NOT NULL,
    PlacedDate DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    ReleasedDate DATETIME NULL,
    IsActive TINYINT(1) NOT NULL DEFAULT 1,
    FOREIGN KEY (StudentId) REFERENCES Students(StudentId) ON DELETE CASCADE
) ENGINE=InnoDB;

SET FOREIGN_KEY_CHECKS = 1;
