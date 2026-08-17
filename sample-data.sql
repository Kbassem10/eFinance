USE studentregistrationportalDB;
GO

PRINT('Inserting sample data for StudentRegistrationPortal database');
GO

BEGIN TRANSACTION;

-- Lookup / status tables
SET IDENTITY_INSERT StudentStatuses ON;
INSERT INTO StudentStatuses (StudentStatusId, StatusName) VALUES
    (1, 'Active'),
    (2, 'Probation'),
    (3, 'Graduated'),
    (4, 'Suspended');
SET IDENTITY_INSERT StudentStatuses OFF;

SET IDENTITY_INSERT InstructorStatuses ON;
INSERT INTO InstructorStatuses (InstructorStatusId, StatusName) VALUES
    (1, 'Active'),
    (2, 'OnLeave'),
    (3, 'Retired');
SET IDENTITY_INSERT InstructorStatuses OFF;

SET IDENTITY_INSERT CourseStatuses ON;
INSERT INTO CourseStatuses (CourseStatusId, StatusName) VALUES
    (1, 'Active'),
    (2, 'Planned'),
    (3, 'Archived');
SET IDENTITY_INSERT CourseStatuses OFF;

SET IDENTITY_INSERT OfferingStatuses ON;
INSERT INTO OfferingStatuses (OfferingStatusId, StatusName) VALUES
    (1, 'Open'),
    (2, 'Closed'),
    (3, 'Cancelled');
SET IDENTITY_INSERT OfferingStatuses OFF;

SET IDENTITY_INSERT EnrollmentStatuses ON;
INSERT INTO EnrollmentStatuses (EnrollmentStatusId, StatusName) VALUES
    (1, 'Enrolled'),
    (2, 'Waitlisted'),
    (3, 'Dropped'),
    (4, 'Completed');
SET IDENTITY_INSERT EnrollmentStatuses OFF;

SET IDENTITY_INSERT AttendanceStatuses ON;
INSERT INTO AttendanceStatuses (AttendanceStatusId, StatusName) VALUES
    (1, 'Present'),
    (2, 'Absent'),
    (3, 'Late'),
    (4, 'Excused');
SET IDENTITY_INSERT AttendanceStatuses OFF;

-- Departments
SET IDENTITY_INSERT Departments ON;
INSERT INTO Departments (DepartmentId, DepartmentName, DepartmentCode, Description, IsActive, CreatedAt) VALUES
    (1, 'Computer Science', 'CS', 'Computer science and software engineering department.', 1, '2026-01-10'),
    (2, 'Business Administration', 'BUS', 'Business, finance, and management programs.', 1, '2026-01-10'),
    (3, 'Engineering', 'ENG', 'Engineering department for systems and applied sciences.', 1, '2026-01-10'),
    (4, 'Mathematics', 'MATH', 'Mathematics and statistics department.', 1, '2026-01-10');
SET IDENTITY_INSERT Departments OFF;

-- Users and Roles
SET IDENTITY_INSERT Users ON;
INSERT INTO Users (UserId, Email, PasswordHash, IsActive, CreatedAt, UpdatedAt) VALUES
    (1, 'admin@portal.edu', 'hashedadminpassword', 1, '2026-06-01 08:00:00', '2026-06-01 08:00:00'),
    (2, 'julia.nichols@student.edu', 'hashedpass1', 1, '2026-06-02 09:00:00', '2026-06-02 09:00:00'),
    (3, 'michael.chen@student.edu', 'hashedpass2', 1, '2026-06-02 09:10:00', '2026-06-02 09:10:00'),
    (4, 'amanda.rivera@student.edu', 'hashedpass3', 1, '2026-06-02 09:20:00', '2026-06-02 09:20:00'),
    (5, 'prof.elena.rodriguez@portal.edu', 'hashedpass4', 1, '2026-05-15 10:00:00', '2026-05-15 10:00:00'),
    (6, 'dr.samuel.park@portal.edu', 'hashedpass5', 1, '2026-05-15 10:10:00', '2026-05-15 10:10:00'),
    (7, 'registrar@portal.edu', 'hashedregistrar', 1, '2026-06-01 08:30:00', '2026-06-01 08:30:00'),
    (8, 'prof.adrian.kim@portal.edu', 'hashedpass6', 1, '2026-05-26 11:00:00', '2026-05-26 11:00:00'),
    (9, 'noah.miles@student.edu', 'hashedpass7', 1, '2026-06-03 11:15:00', '2026-06-03 11:15:00'),
    (10, 'sophia.jones@student.edu', 'hashedpass8', 1, '2026-06-03 11:25:00', '2026-06-03 11:25:00'),
    (11, 'liam.wood@student.edu', 'hashedpass9', 1, '2026-06-03 11:35:00', '2026-06-03 11:35:00');
SET IDENTITY_INSERT Users OFF;

SET IDENTITY_INSERT Roles ON;
INSERT INTO Roles (RoleId, RoleName) VALUES
    (1, 'Admin'),
    (2, 'Student'),
    (3, 'Instructor'),
    (4, 'Registrar');
SET IDENTITY_INSERT Roles OFF;

INSERT INTO UserRoles (UserId, RoleId, AssignedAt) VALUES
    (1, 1, '2026-06-01 08:05:00'),
    (2, 2, '2026-06-02 09:05:00'),
    (3, 2, '2026-06-02 09:15:00'),
    (4, 2, '2026-06-02 09:25:00'),
    (5, 3, '2026-05-15 10:05:00'),
    (6, 3, '2026-05-15 10:15:00'),
    (7, 4, '2026-06-01 08:35:00'),
    (8, 3, '2026-05-26 11:05:00'),
    (9, 2, '2026-06-03 11:20:00'),
    (10, 2, '2026-06-03 11:30:00'),
    (11, 2, '2026-06-03 11:40:00');

-- Students
SET IDENTITY_INSERT Students ON;
INSERT INTO Students (StudentId, UserId, DepartmentId, StudentStatusId, StudentNumber, FirstName, MiddleName, LastName, NationalId, DateOfBirth, Gender, PhoneNumber, Address, AdmissionDate, AcademicLevel, GPA, CompletedCreditHours, CreatedAt, UpdatedAt) VALUES
    (1, 2, 1, 1, 'S2026001', 'Julia', 'M.', 'Nichols', 'A123456789', '2004-04-12', 'Female', '555-0190', '514 Maple Street, Apt 1', '2026-08-20', 1, 3.80, 15, '2026-06-02 09:10:00', '2026-06-02 09:10:00'),
    (2, 3, 1, 1, 'S2026002', 'Michael', 'T.', 'Chen', 'B987654321', '2003-11-03', 'Male', '555-0191', '822 Oak Avenue', '2025-08-20', 2, 3.45, 30, '2026-06-02 09:20:00', '2026-06-02 09:20:00'),
    (3, 4, 2, 2, 'S2026003', 'Amanda', null, 'Rivera', 'C112233445', '2004-02-28', 'Female', '555-0192', '134 Pine Lane', '2026-08-20', 1, 2.70, 12, '2026-06-02 09:30:00', '2026-06-02 09:30:00'),
    (4, 9, 3, 1, 'S2026004', 'Noah', 'D.', 'Miles', 'D556677889', '2002-09-18', 'Male', '555-0193', '207 Birch Road', '2024-08-20', 3, 3.95, 78, '2026-06-03 11:20:00', '2026-06-03 11:20:00'),
    (5, 10, 4, 1, 'S2026005', 'Sophia', null, 'Jones', 'E998877665', '2003-07-05', 'Female', '555-0194', '411 Cedar Court', '2025-08-20', 2, 3.25, 42, '2026-06-03 11:30:00', '2026-06-03 11:30:00'),
    (6, 11, 2, 4, 'S2026006', 'Liam', 'J.', 'Wood', 'F334455667', '2001-12-11', 'Male', '555-0195', '98 Spruce Drive', '2023-08-20', 4, 2.10, 90, '2026-06-03 11:35:00', '2026-06-03 11:35:00');
SET IDENTITY_INSERT Students OFF;

-- Instructors
SET IDENTITY_INSERT Instructors ON;
INSERT INTO Instructors (InstructorId, UserId, DepartmentId, InstructorStatusId, EmployeeNumber, FirstName, MiddleName, LastName, NationalId, DateOfBirth, PhoneNumber, AcademicTitle, HireDate, Salary, CreatedAt, UpdatedAt) VALUES
    (1, 5, 1, 1, 'E1001', 'Elena', 'R.', 'Rodriguez', 'I111222333', '1980-03-14', '555-0200', 'Professor', '2015-09-01', 95000.00, '2026-05-15 10:05:00', '2026-05-15 10:05:00'),
    (2, 6, 2, 1, 'E1002', 'Samuel', null, 'Park', 'I444555666', '1978-07-26', '555-0201', 'Associate Professor', '2018-01-10', 88000.00, '2026-05-15 10:15:00', '2026-05-15 10:15:00'),
    (3, 8, 3, 2, 'E1003', 'Adrian', 'K.', 'Kim', 'I777888999', '1975-10-22', '555-0202', 'Senior Lecturer', '2012-03-01', 82000.00, '2026-05-26 11:10:00', '2026-05-26 11:10:00');
SET IDENTITY_INSERT Instructors OFF;

-- Semesters
SET IDENTITY_INSERT Semesters ON;
INSERT INTO Semesters (SemesterId, SemesterName, AcademicYear, StartDate, EndDate, RegistrationStartDate, RegistrationEndDate, IsCurrent) VALUES
    (1, 'Fall 2026', '2026-2027', '2026-08-20', '2026-12-15', '2026-06-01 08:00:00', '2026-08-15 23:59:59', 1),
    (2, 'Spring 2027', '2026-2027', '2027-01-10', '2027-05-05', '2026-11-01 08:00:00', '2027-01-08 23:59:59', 0),
    (3, 'Summer 2027', '2026-2027', '2027-06-01', '2027-08-01', '2027-04-10 08:00:00', '2027-05-30 23:59:59', 0);
SET IDENTITY_INSERT Semesters OFF;

-- Courses
SET IDENTITY_INSERT Courses ON;
INSERT INTO Courses (CourseId, CourseStatusId, CourseCode, CourseName, Description, CreditHours, MaximumStudents, DifficultyLevel, IsActive, CreatedAt, UpdatedAt) VALUES
    (1, 1, 'CS101', 'Introduction to Computer Science', 'Foundational concepts in programming and problem solving.', 3, 40, 'Beginner', 1, '2026-01-10 08:00:00', '2026-01-10 08:00:00'),
    (2, 1, 'CS201', 'Data Structures', 'Study of algorithms, arrays, lists, stacks, queues, and trees.', 4, 30, 'Intermediate', 1, '2026-01-10 08:05:00', '2026-01-10 08:05:00'),
    (3, 1, 'BUS101', 'Principles of Management', 'Business fundamentals with a focus on leadership and planning.', 3, 45, 'Beginner', 1, '2026-01-10 08:10:00', '2026-01-10 08:10:00'),
    (4, 1, 'BUS202', 'Financial Accounting', 'Accounting principles for business decision-making and reporting.', 3, 35, 'Intermediate', 1, '2026-01-10 08:15:00', '2026-01-10 08:15:00'),
    (5, 1, 'ENG150', 'Introduction to Engineering', 'Engineering design principles and systems thinking.', 3, 40, 'Beginner', 1, '2026-01-10 08:20:00', '2026-01-10 08:20:00'),
    (6, 1, 'MATH220', 'Statistics and Probability', 'Probability theory and statistical methods for engineers.', 4, 35, 'Intermediate', 1, '2026-01-10 08:25:00', '2026-01-10 08:25:00'),
    (7, 1, 'CS301', 'Software Engineering', 'Software development lifecycle, testing, and architecture.', 4, 30, 'Advanced', 1, '2026-01-10 08:30:00', '2026-01-10 08:30:00'),
    (8, 1, 'ENG250', 'Engineering Ethics', 'Ethical issues, professional responsibility, and sustainability.', 2, 30, 'Beginner', 1, '2026-01-10 08:35:00', '2026-01-10 08:35:00');
SET IDENTITY_INSERT Courses OFF;

INSERT INTO CourseDepartments (CourseId, DepartmentId, IsPrimaryDepartment) VALUES
    (1, 1, 1),
    (2, 1, 1),
    (3, 2, 1),
    (4, 2, 1),
    (5, 3, 1),
    (6, 4, 1),
    (7, 1, 1),
    (8, 3, 1);

INSERT INTO CoursePrerequisites (CourseId, PrerequisiteCourseId, MinimumGrade) VALUES
    (2, 1, 'C'),
    (7, 2, 'C'),
    (4, 3, 'C'),
    (8, 5, 'B');

-- Rooms
SET IDENTITY_INSERT Rooms ON;
INSERT INTO Rooms (RoomId, BuildingName, RoomNumber, Capacity, RoomType, IsAvailable) VALUES
    (1, 'Main Hall', '101', 60, 'Lecture Hall', 1),
    (2, 'Main Hall', '102', 40, 'Lecture Room', 1),
    (3, 'Science Building', '201', 35, 'Lab Room', 1),
    (4, 'Business Center', '305', 45, 'Seminar Room', 1);
SET IDENTITY_INSERT Rooms OFF;

-- Course offerings
SET IDENTITY_INSERT CourseOfferings ON;
INSERT INTO CourseOfferings (CourseOfferingId, CourseId, SemesterId, OfferingStatusId, SectionNumber, Capacity, CurrentEnrollmentCount, RegistrationOpen, CreatedAt, UpdatedAt) VALUES
    (1, 1, 1, 1, 'A', 40, 4, 1, '2026-06-10 08:00:00', '2026-06-10 08:00:00'),
    (2, 2, 1, 1, 'A', 30, 3, 1, '2026-06-10 08:05:00', '2026-06-10 08:05:00'),
    (3, 3, 1, 1, 'A', 45, 4, 1, '2026-06-10 08:10:00', '2026-06-10 08:10:00'),
    (4, 4, 1, 1, 'A', 35, 2, 1, '2026-06-10 08:15:00', '2026-06-10 08:15:00'),
    (5, 5, 2, 1, 'A', 40, 2, 1, '2026-06-10 08:20:00', '2026-06-10 08:20:00'),
    (6, 6, 2, 1, 'A', 35, 3, 1, '2026-06-10 08:25:00', '2026-06-10 08:25:00'),
    (7, 7, 2, 2, 'A', 30, 1, 0, '2026-06-10 08:30:00', '2026-06-10 08:30:00'),
    (8, 8, 3, 1, 'A', 30, 2, 1, '2026-06-10 08:35:00', '2026-06-10 08:35:00');
SET IDENTITY_INSERT CourseOfferings OFF;

INSERT INTO CourseOfferingInstructors (CourseOfferingId, InstructorId) VALUES
    (1, 1),
    (2, 1),
    (3, 2),
    (4, 2),
    (5, 3),
    (6, 3),
    (7, 1),
    (8, 2);

-- Course schedule entries
SET IDENTITY_INSERT CourseSchedules ON;
INSERT INTO CourseSchedules (CourseScheduleId, CourseOfferingId, RoomId, DayOfWeek, StartTime, EndTime, ScheduleType) VALUES
    (1, 1, 1, 'Monday', '09:00:00', '10:30:00', 'Lecture'),
    (2, 1, 2, 'Wednesday', '09:00:00', '10:30:00', 'Lecture'),
    (3, 2, 1, 'Tuesday', '11:00:00', '12:30:00', 'Lecture'),
    (4, 2, 3, 'Thursday', '11:00:00', '12:30:00', 'Lab'),
    (5, 3, 4, 'Monday', '13:00:00', '14:30:00', 'Lecture'),
    (6, 4, 4, 'Tuesday', '14:45:00', '16:15:00', 'Lecture'),
    (7, 5, 2, 'Wednesday', '09:00:00', '10:30:00', 'Lecture'),
    (8, 6, 3, 'Thursday', '10:45:00', '12:15:00', 'Lecture'),
    (9, 7, 1, 'Friday', '13:00:00', '14:30:00', 'Lecture'),
    (10, 8, 4, 'Tuesday', '08:30:00', '10:00:00', 'Seminar');
SET IDENTITY_INSERT CourseSchedules OFF;

-- Enrollments
SET IDENTITY_INSERT Enrollments ON;
INSERT INTO Enrollments (EnrollmentId, StudentId, CourseOfferingId, EnrollmentStatusId, EnrollmentDate, DropDate, CourseworkGrade, MidtermGrade, FinalExamGrade, TotalGrade, LetterGrade, GradePoints, IsPassed) VALUES
    (1, 1, 1, 1, '2026-08-05 09:00:00', NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0),
    (2, 2, 1, 1, '2026-08-05 09:05:00', NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0),
    (3, 3, 1, 1, '2026-08-05 09:10:00', NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0),
    (4, 4, 2, 1, '2026-08-06 09:15:00', NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0),
    (5, 1, 3, 1, '2026-08-06 09:20:00', NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0),
    (6, 9, 3, 1, '2026-08-06 09:25:00', NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0),
    (7, 10, 4, 2, '2026-08-06 09:30:00', NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0),
    (8, 5, 5, 1, '2027-01-05 10:00:00', NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0),
    (9, 6, 6, 1, '2027-01-05 10:05:00', NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0),
    (10, 2, 2, 1, '2026-08-05 09:35:00', NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0),
    (11, 3, 4, 1, '2026-08-06 09:45:00', NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0),
    (12, 4, 6, 1, '2027-01-05 10:10:00', NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0),
    (13, 1, 7, 3, '2027-01-08 10:30:00', '2027-01-20 15:00:00', NULL, NULL, NULL, NULL, NULL, NULL, 0),
    (14, 5, 8, 1, '2027-06-05 11:00:00', NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0),
    (15, 2, 8, 1, '2027-06-05 11:05:00', NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0),
    (16, 9, 2, 1, '2026-08-05 09:50:00', NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0),
    (17, 10, 5, 1, '2027-01-05 10:15:00', NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0),
    (18, 11, 3, 4, '2026-08-06 09:55:00', NULL, 85.50, 86.00, 90.00, 87.00, 'B+', 3.30, 1);
SET IDENTITY_INSERT Enrollments OFF;

-- Lectures
SET IDENTITY_INSERT Lectures ON;
INSERT INTO Lectures (LectureId, CourseOfferingId, RoomId, LectureTitle, LectureTopic, LectureDate, StartTime, EndTime, IsCancelled, CreatedAt) VALUES
    (1, 1, 1, 'CS101 Opening Lecture', 'Introduction to programming workflows and campus systems.', '2026-08-21', '09:00:00', '10:30:00', 0, '2026-08-10 08:00:00'),
    (2, 2, 1, 'Data Structures Overview', 'Arrays, stacks, and queues; course expectations.', '2026-08-22', '11:00:00', '12:30:00', 0, '2026-08-10 08:05:00'),
    (3, 3, 4, 'Management Principles', 'Roles, planning, organization, and control.', '2026-08-21', '13:00:00', '14:30:00', 0, '2026-08-10 08:10:00'),
    (4, 4, 4, 'Accounting Basics', 'Introduction to financial statements and transactions.', '2026-08-22', '14:45:00', '16:15:00', 0, '2026-08-10 08:15:00'),
    (5, 5, 2, 'Engineering Design Concepts', 'Problem solving, systems approach, and teamwork.', '2027-01-11', '09:00:00', '10:30:00', 0, '2027-01-02 08:00:00'),
    (6, 6, 3, 'Probability Foundations', 'Probability rules, distributions, and inference.', '2027-01-12', '10:45:00', '12:15:00', 0, '2027-01-02 08:05:00'),
    (7, 7, 1, 'Software Engineering Planning', 'Requirements, UML, and iterative delivery.', '2027-01-13', '13:00:00', '14:30:00', 0, '2027-01-02 08:10:00'),
    (8, 8, 4, 'Ethics in Engineering', 'Professional responsibility and sustainability.', '2027-06-02', '08:30:00', '10:00:00', 0, '2027-05-20 08:00:00');
SET IDENTITY_INSERT Lectures OFF;

-- Attendance records
SET IDENTITY_INSERT Attendance ON;
INSERT INTO Attendance (AttendanceId, LectureId, StudentId, AttendanceStatusId, CheckInTime, Notes, RecordedAt) VALUES
    (1, 1, 1, 1, '2026-08-21 08:55:00', 'On time', '2026-08-21 09:05:00'),
    (2, 1, 2, 1, '2026-08-21 08:58:00', 'On time', '2026-08-21 09:06:00'),
    (3, 1, 3, 2, NULL, 'Absent due to illness', '2026-08-21 09:10:00'),
    (4, 2, 4, 1, '2026-08-22 10:55:00', 'Arrived early', '2026-08-22 11:05:00'),
    (5, 2, 2, 3, '2026-08-22 11:05:00', 'Late by 5 minutes', '2026-08-22 11:10:00'),
    (6, 3, 1, 1, '2026-08-21 12:55:00', 'Present', '2026-08-21 13:05:00'),
    (7, 4, 10, 1, '2026-08-22 14:40:00', 'Attended', '2026-08-22 14:45:00'),
    (8, 5, 5, 1, '2027-01-11 08:50:00', 'Present', '2027-01-11 09:00:00'),
    (9, 6, 6, 4, NULL, 'Excused absence for appointment', '2027-01-12 10:50:00'),
    (10, 7, 1, 1, '2027-01-13 12:55:00', 'Present', '2027-01-13 13:05:00');
SET IDENTITY_INSERT Attendance OFF;

-- Student holds
SET IDENTITY_INSERT StudentHolds ON;
INSERT INTO StudentHolds (StudentHoldId, StudentId, HoldType, Reason, StartDate, EndDate, IsActive, CreatedAt) VALUES
    (1, 3, 'Financial Hold', 'Outstanding tuition balance for Fall 2026.', '2026-07-01', '2026-09-01', 1, '2026-07-01 12:00:00'),
    (2, 6, 'Academic Probation', 'GPA fell below minimum requirement.', '2026-06-30', NULL, 1, '2026-06-30 12:00:00');
SET IDENTITY_INSERT StudentHolds OFF;

COMMIT;
GO

PRINT('Sample data insertion complete.');
GO

USE studentregistrationportalDB;
GO

SET NOCOUNT ON;
GO

-- Student stored procedures
DROP PROCEDURE IF EXISTS dbo.sp_Student_Insert;
GO
CREATE PROCEDURE dbo.sp_Student_Insert
    @UserId INT,
    @DepartmentId INT,
    @StudentStatusId INT,
    @StudentNumber NVARCHAR(30),
    @FirstName NVARCHAR(100),
    @MiddleName NVARCHAR(100) = NULL,
    @LastName NVARCHAR(100),
    @NationalId NVARCHAR(30) = NULL,
    @DateOfBirth DATE,
    @Gender NVARCHAR(20) = NULL,
    @PhoneNumber NVARCHAR(30) = NULL,
    @Address NVARCHAR(500) = NULL,
    @AdmissionDate DATE,
    @AcademicLevel INT,
    @GPA DECIMAL(4,2),
    @CompletedCreditHours INT,
    @CreatedAt DATETIME = NULL,
    @UpdatedAt DATETIME = NULL
AS
BEGIN
    SET NOCOUNT ON;
    IF @CreatedAt IS NULL SET @CreatedAt = GETDATE();
    IF @UpdatedAt IS NULL SET @UpdatedAt = GETDATE();

    INSERT INTO Students (
        UserId, DepartmentId, StudentStatusId, StudentNumber, FirstName, MiddleName,
        LastName, NationalId, DateOfBirth, Gender, PhoneNumber, Address,
        AdmissionDate, AcademicLevel, GPA, CompletedCreditHours, CreatedAt, UpdatedAt
    ) VALUES (
        @UserId, @DepartmentId, @StudentStatusId, @StudentNumber, @FirstName, @MiddleName,
        @LastName, @NationalId, @DateOfBirth, @Gender, @PhoneNumber, @Address,
        @AdmissionDate, @AcademicLevel, @GPA, @CompletedCreditHours, @CreatedAt, @UpdatedAt
    );

    SELECT SCOPE_IDENTITY() AS StudentId;
END;
GO

DROP PROCEDURE IF EXISTS dbo.sp_Student_GetAll;
GO
CREATE PROCEDURE dbo.sp_Student_GetAll
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        StudentId,
        UserId,
        DepartmentId,
        StudentStatusId,
        StudentNumber,
        FirstName,
        MiddleName,
        LastName,
        NationalId,
        DateOfBirth,
        Gender,
        PhoneNumber,
        Address,
        AdmissionDate,
        AcademicLevel,
        GPA,
        CompletedCreditHours,
        CreatedAt,
        UpdatedAt
    FROM Students;
END;
GO

DROP PROCEDURE IF EXISTS dbo.sp_Student_GetById;
GO
CREATE PROCEDURE dbo.sp_Student_GetById
    @StudentId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        StudentId,
        UserId,
        DepartmentId,
        StudentStatusId,
        StudentNumber,
        FirstName,
        MiddleName,
        LastName,
        NationalId,
        DateOfBirth,
        Gender,
        PhoneNumber,
        Address,
        AdmissionDate,
        AcademicLevel,
        GPA,
        CompletedCreditHours,
        CreatedAt,
        UpdatedAt
    FROM Students
    WHERE StudentId = @StudentId;
END;
GO

DROP PROCEDURE IF EXISTS dbo.sp_Student_Update;
GO
CREATE PROCEDURE dbo.sp_Student_Update
    @StudentId INT,
    @UserId INT,
    @DepartmentId INT,
    @StudentStatusId INT,
    @StudentNumber NVARCHAR(30),
    @FirstName NVARCHAR(100),
    @MiddleName NVARCHAR(100) = NULL,
    @LastName NVARCHAR(100),
    @NationalId NVARCHAR(30) = NULL,
    @DateOfBirth DATE,
    @Gender NVARCHAR(20) = NULL,
    @PhoneNumber NVARCHAR(30) = NULL,
    @Address NVARCHAR(500) = NULL,
    @AdmissionDate DATE,
    @AcademicLevel INT,
    @GPA DECIMAL(4,2),
    @CompletedCreditHours INT,
    @UpdatedAt DATETIME = NULL
AS
BEGIN
    SET NOCOUNT ON;
    IF @UpdatedAt IS NULL SET @UpdatedAt = GETDATE();

    UPDATE Students
    SET
        UserId = @UserId,
        DepartmentId = @DepartmentId,
        StudentStatusId = @StudentStatusId,
        StudentNumber = @StudentNumber,
        FirstName = @FirstName,
        MiddleName = @MiddleName,
        LastName = @LastName,
        NationalId = @NationalId,
        DateOfBirth = @DateOfBirth,
        Gender = @Gender,
        PhoneNumber = @PhoneNumber,
        Address = @Address,
        AdmissionDate = @AdmissionDate,
        AcademicLevel = @AcademicLevel,
        GPA = @GPA,
        CompletedCreditHours = @CompletedCreditHours,
        UpdatedAt = @UpdatedAt
    WHERE StudentId = @StudentId;

    SELECT @@ROWCOUNT AS RowsAffected;
END;
GO

DROP PROCEDURE IF EXISTS dbo.sp_Student_Delete;
GO
CREATE PROCEDURE dbo.sp_Student_Delete
    @StudentId INT
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM Students
    WHERE StudentId = @StudentId;

    SELECT @@ROWCOUNT AS RowsAffected;
END;
GO

-- Course stored procedures
DROP PROCEDURE IF EXISTS dbo.sp_Course_Insert;
GO
CREATE PROCEDURE dbo.sp_Course_Insert
    @CourseStatusId INT,
    @CourseCode NVARCHAR(30),
    @CourseName NVARCHAR(200),
    @Description NVARCHAR(1000) = NULL,
    @CreditHours INT,
    @MaximumStudents INT,
    @DifficultyLevel NVARCHAR(30) = NULL,
    @IsActive BIT,
    @CreatedAt DATETIME = NULL,
    @UpdatedAt DATETIME = NULL
AS
BEGIN
    SET NOCOUNT ON;
    IF @CreatedAt IS NULL SET @CreatedAt = GETDATE();
    IF @UpdatedAt IS NULL SET @UpdatedAt = GETDATE();

    INSERT INTO Courses (
        CourseStatusId,
        CourseCode,
        CourseName,
        Description,
        CreditHours,
        MaximumStudents,
        DifficultyLevel,
        IsActive,
        CreatedAt,
        UpdatedAt
    ) VALUES (
        @CourseStatusId,
        @CourseCode,
        @CourseName,
        @Description,
        @CreditHours,
        @MaximumStudents,
        @DifficultyLevel,
        @IsActive,
        @CreatedAt,
        @UpdatedAt
    );

    SELECT SCOPE_IDENTITY() AS CourseId;
END;
GO

DROP PROCEDURE IF EXISTS dbo.sp_Course_GetAll;
GO
CREATE PROCEDURE dbo.sp_Course_GetAll
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        CourseId,
        CourseStatusId,
        CourseCode,
        CourseName,
        Description,
        CreditHours,
        MaximumStudents,
        DifficultyLevel,
        IsActive,
        CreatedAt,
        UpdatedAt
    FROM Courses;
END;
GO

DROP PROCEDURE IF EXISTS dbo.sp_Course_GetById;
GO
CREATE PROCEDURE dbo.sp_Course_GetById
    @CourseId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        CourseId,
        CourseStatusId,
        CourseCode,
        CourseName,
        Description,
        CreditHours,
        MaximumStudents,
        DifficultyLevel,
        IsActive,
        CreatedAt,
        UpdatedAt
    FROM Courses
    WHERE CourseId = @CourseId;
END;
GO

DROP PROCEDURE IF EXISTS dbo.sp_Course_Update;
GO
CREATE PROCEDURE dbo.sp_Course_Update
    @CourseId INT,
    @CourseStatusId INT,
    @CourseCode NVARCHAR(30),
    @CourseName NVARCHAR(200),
    @Description NVARCHAR(1000) = NULL,
    @CreditHours INT,
    @MaximumStudents INT,
    @DifficultyLevel NVARCHAR(30) = NULL,
    @IsActive BIT,
    @UpdatedAt DATETIME = NULL
AS
BEGIN
    SET NOCOUNT ON;
    IF @UpdatedAt IS NULL SET @UpdatedAt = GETDATE();

    UPDATE Courses
    SET
        CourseStatusId = @CourseStatusId,
        CourseCode = @CourseCode,
        CourseName = @CourseName,
        Description = @Description,
        CreditHours = @CreditHours,
        MaximumStudents = @MaximumStudents,
        DifficultyLevel = @DifficultyLevel,
        IsActive = @IsActive,
        UpdatedAt = @UpdatedAt
    WHERE CourseId = @CourseId;

    SELECT @@ROWCOUNT AS RowsAffected;
END;
GO

DROP PROCEDURE IF EXISTS dbo.sp_Course_Delete;
GO
CREATE PROCEDURE dbo.sp_Course_Delete
    @CourseId INT
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM Courses
    WHERE CourseId = @CourseId;

    SELECT @@ROWCOUNT AS RowsAffected;
END;
GO
