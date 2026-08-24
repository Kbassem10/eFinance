-- ==========================================================
-- Migration 0003: Seed Complete Sample Data for MySQL
-- ==========================================================

USE StudentRegistrationPortal;

SET FOREIGN_KEY_CHECKS = 0;

-- 1. Lookup / Status tables
INSERT IGNORE INTO StudentStatuses (StudentStatusId, StatusName, Description) VALUES
    (1, 'Active', 'Currently active student'),
    (2, 'Probation', 'Academic probation'),
    (3, 'Graduated', 'Graduated student'),
    (4, 'Suspended', 'Suspended student');

INSERT IGNORE INTO InstructorStatuses (InstructorStatusId, StatusName, Description) VALUES
    (1, 'Active', 'Active faculty'),
    (2, 'OnLeave', 'On extended leave'),
    (3, 'Retired', 'Retired faculty');

INSERT IGNORE INTO CourseStatuses (CourseStatusId, StatusName, Description) VALUES
    (1, 'Active', 'Course is offered'),
    (2, 'Planned', 'Course in planning'),
    (3, 'Archived', 'Historical course');

INSERT IGNORE INTO OfferingStatuses (OfferingStatusId, StatusName, Description) VALUES
    (1, 'Open', 'Registration is open'),
    (2, 'Closed', 'Registration is closed'),
    (3, 'Cancelled', 'Course offering cancelled');

INSERT IGNORE INTO EnrollmentStatuses (EnrollmentStatusId, StatusName, Description) VALUES
    (1, 'Enrolled', 'Enrolled and attending'),
    (2, 'Waitlisted', 'Waitlisted for offering'),
    (3, 'Dropped', 'Dropped by student'),
    (4, 'Completed', 'Course completed with grade');

INSERT IGNORE INTO AttendanceStatuses (AttendanceStatusId, StatusName) VALUES
    (1, 'Present'),
    (2, 'Absent'),
    (3, 'Late'),
    (4, 'Excused');

-- 2. Departments
INSERT IGNORE INTO Departments (DepartmentId, DepartmentCode, DepartmentName, CreatedAt) VALUES
    (1, 'CS', 'Computer Science', '2026-01-10'),
    (2, 'BUS', 'Business Administration', '2026-01-10'),
    (3, 'ENG', 'Engineering', '2026-01-10'),
    (4, 'MATH', 'Mathematics', '2026-01-10');

-- 3. Users and Roles
INSERT IGNORE INTO Users (UserId, Email, PasswordHash, IsActive, CreatedAt, UpdatedAt) VALUES
    (1, 'admin@portal.edu', 'AQAAAAIAAYagAAAAEGHashPlaceholderAdmin123!', 1, '2026-06-01 08:00:00', '2026-06-01 08:00:00'),
    (2, 'julia.nichols@student.edu', 'AQAAAAIAAYagAAAAEGHashPlaceholderStudent123!', 1, '2026-06-02 09:00:00', '2026-06-02 09:00:00'),
    (3, 'michael.chen@student.edu', 'AQAAAAIAAYagAAAAEGHashPlaceholderStudent123!', 1, '2026-06-02 09:10:00', '2026-06-02 09:10:00'),
    (4, 'amanda.rivera@student.edu', 'AQAAAAIAAYagAAAAEGHashPlaceholderStudent123!', 1, '2026-06-02 09:20:00', '2026-06-02 09:20:00'),
    (5, 'prof.elena.rodriguez@portal.edu', 'AQAAAAIAAYagAAAAEGHashPlaceholderInst123!', 1, '2026-05-15 10:00:00', '2026-05-15 10:00:00'),
    (6, 'dr.samuel.park@portal.edu', 'AQAAAAIAAYagAAAAEGHashPlaceholderInst123!', 1, '2026-05-15 10:10:00', '2026-05-15 10:10:00'),
    (7, 'registrar@portal.edu', 'AQAAAAIAAYagAAAAEGHashPlaceholderAdmin123!', 1, '2026-06-01 08:30:00', '2026-06-01 08:30:00'),
    (8, 'prof.adrian.kim@portal.edu', 'AQAAAAIAAYagAAAAEGHashPlaceholderInst123!', 1, '2026-05-26 11:00:00', '2026-05-26 11:00:00'),
    (9, 'noah.miles@student.edu', 'AQAAAAIAAYagAAAAEGHashPlaceholderStudent123!', 1, '2026-06-03 11:15:00', '2026-06-03 11:15:00'),
    (10, 'sophia.jones@student.edu', 'AQAAAAIAAYagAAAAEGHashPlaceholderStudent123!', 1, '2026-06-03 11:25:00', '2026-06-03 11:25:00'),
    (11, 'liam.wood@student.edu', 'AQAAAAIAAYagAAAAEGHashPlaceholderStudent123!', 1, '2026-06-03 11:35:00', '2026-06-03 11:35:00');

INSERT IGNORE INTO Roles (RoleId, RoleName, Description) VALUES
    (1, 'Admin', 'System Administrator'),
    (2, 'Student', 'Registered Student'),
    (3, 'Instructor', 'Faculty Instructor'),
    (4, 'Registrar', 'Registration Staff');

INSERT IGNORE INTO UserRoles (UserId, RoleId, AssignedAt) VALUES
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

-- 4. Students
INSERT IGNORE INTO Students (StudentId, UserId, DepartmentId, StudentStatusId, StudentNumber, FirstName, MiddleName, LastName, NationalId, DateOfBirth, Gender, PhoneNumber, Address, AdmissionDate, AcademicLevel, GPA, CompletedCreditHours, CreatedAt, UpdatedAt) VALUES
    (1, 2, 1, 1, 'S2026001', 'Julia', 'M.', 'Nichols', 'A123456789', '2004-04-12', 'Female', '555-0190', '514 Maple Street, Apt 1', '2026-08-20', 1, 3.80, 15, '2026-06-02 09:10:00', '2026-06-02 09:10:00'),
    (2, 3, 1, 1, 'S2026002', 'Michael', 'T.', 'Chen', 'B987654321', '2003-11-03', 'Male', '555-0191', '822 Oak Avenue', '2025-08-20', 2, 3.45, 30, '2026-06-02 09:20:00', '2026-06-02 09:20:00'),
    (3, 4, 2, 2, 'S2026003', 'Amanda', NULL, 'Rivera', 'C112233445', '2004-02-28', 'Female', '555-0192', '134 Pine Lane', '2026-08-20', 1, 2.70, 12, '2026-06-02 09:30:00', '2026-06-02 09:30:00'),
    (4, 9, 3, 1, 'S2026004', 'Noah', 'D.', 'Miles', 'D556677889', '2002-09-18', 'Male', '555-0193', '207 Birch Road', '2024-08-20', 3, 3.95, 78, '2026-06-03 11:20:00', '2026-06-03 11:20:00'),
    (5, 10, 4, 1, 'S2026005', 'Sophia', NULL, 'Jones', 'E998877665', '2003-07-05', 'Female', '555-0194', '411 Cedar Court', '2025-08-20', 2, 3.25, 42, '2026-06-03 11:30:00', '2026-06-03 11:30:00'),
    (6, 11, 2, 4, 'S2026006', 'Liam', 'J.', 'Wood', 'F334455667', '2001-12-11', 'Male', '555-0195', '98 Spruce Drive', '2023-08-20', 4, 2.10, 90, '2026-06-03 11:35:00', '2026-06-03 11:35:00');

-- 5. Instructors
INSERT IGNORE INTO Instructors (InstructorId, UserId, DepartmentId, InstructorStatusId, EmployeeNumber, FirstName, MiddleName, LastName, AcademicTitle, Salary, HireDate, CreatedAt, UpdatedAt) VALUES
    (1, 5, 1, 1, 'E1001', 'Elena', 'R.', 'Rodriguez', 'Professor', 95000.00, '2015-09-01', '2026-05-15 10:05:00', '2026-05-15 10:05:00'),
    (2, 6, 2, 1, 'E1002', 'Samuel', NULL, 'Park', 'Associate Professor', 88000.00, '2018-01-10', '2026-05-15 10:15:00', '2026-05-15 10:15:00'),
    (3, 8, 3, 2, 'E1003', 'Adrian', 'K.', 'Kim', 'Senior Lecturer', 82000.00, '2012-03-01', '2026-05-26 11:10:00', '2026-05-26 11:10:00');

-- 6. Semesters
INSERT IGNORE INTO Semesters (SemesterId, SemesterName, AcademicYear, StartDate, EndDate, IsCurrent) VALUES
    (1, 'Fall 2026', 2026, '2026-08-20', '2026-12-15', 1),
    (2, 'Spring 2027', 2027, '2027-01-10', '2027-05-05', 0),
    (3, 'Summer 2027', 2027, '2027-06-01', '2027-08-01', 0);

-- 7. Courses
INSERT IGNORE INTO Courses (CourseId, CourseStatusId, CourseCode, CourseName, CreditHours, DifficultyLevel, CreatedAt, UpdatedAt) VALUES
    (1, 1, 'CS101', 'Introduction to Computer Science', 3, 'Beginner', '2026-01-10 08:00:00', '2026-01-10 08:00:00'),
    (2, 1, 'CS201', 'Data Structures', 4, 'Intermediate', '2026-01-10 08:05:00', '2026-01-10 08:05:00'),
    (3, 1, 'BUS101', 'Principles of Management', 3, 'Beginner', '2026-01-10 08:10:00', '2026-01-10 08:10:00'),
    (4, 1, 'BUS202', 'Financial Accounting', 3, 'Intermediate', '2026-01-10 08:15:00', '2026-01-10 08:15:00'),
    (5, 1, 'ENG150', 'Introduction to Engineering', 3, 'Beginner', '2026-01-10 08:20:00', '2026-01-10 08:20:00'),
    (6, 1, 'MATH220', 'Statistics and Probability', 4, 'Intermediate', '2026-01-10 08:25:00', '2026-01-10 08:25:00'),
    (7, 1, 'CS301', 'Software Engineering', 4, 'Advanced', '2026-01-10 08:30:00', '2026-01-10 08:30:00'),
    (8, 1, 'ENG250', 'Engineering Ethics', 2, 'Beginner', '2026-01-10 08:35:00', '2026-01-10 08:35:00');

INSERT IGNORE INTO CourseDepartments (CourseId, DepartmentId) VALUES
    (1, 1),
    (2, 1),
    (3, 2),
    (4, 2),
    (5, 3),
    (6, 4),
    (7, 1),
    (8, 3);

INSERT IGNORE INTO CoursePrerequisites (CourseId, PrerequisiteCourseId) VALUES
    (2, 1),
    (7, 2),
    (4, 3),
    (8, 5);

-- 8. Rooms
INSERT IGNORE INTO Rooms (RoomId, BuildingName, RoomNumber, Capacity) VALUES
    (1, 'Main Hall', '101', 60),
    (2, 'Main Hall', '102', 40),
    (3, 'Science Building', '201', 35),
    (4, 'Business Center', '305', 45);

-- 9. Course Offerings
INSERT IGNORE INTO CourseOfferings (CourseOfferingId, CourseId, SemesterId, OfferingStatusId, SectionNumber, Capacity, CreatedAt) VALUES
    (1, 1, 1, 1, 'A', 40, '2026-06-10 08:00:00'),
    (2, 2, 1, 1, 'A', 30, '2026-06-10 08:05:00'),
    (3, 3, 1, 1, 'A', 45, '2026-06-10 08:10:00'),
    (4, 4, 1, 1, 'A', 35, '2026-06-10 08:15:00'),
    (5, 5, 2, 1, 'A', 40, '2026-06-10 08:20:00'),
    (6, 6, 2, 1, 'A', 35, '2026-06-10 08:25:00'),
    (7, 7, 2, 2, 'A', 30, '2026-06-10 08:30:00'),
    (8, 8, 3, 1, 'A', 30, '2026-06-10 08:35:00');

INSERT IGNORE INTO CourseOfferingInstructors (CourseOfferingId, InstructorId, IsPrimary) VALUES
    (1, 1, 1),
    (2, 1, 1),
    (3, 2, 1),
    (4, 2, 1),
    (5, 3, 1),
    (6, 3, 1),
    (7, 1, 1),
    (8, 2, 1);

-- 10. Course Schedules
INSERT IGNORE INTO CourseSchedules (CourseScheduleId, CourseOfferingId, RoomId, DayOfWeek, StartTime, EndTime) VALUES
    (1, 1, 1, 1, '09:00:00', '10:30:00'),
    (2, 1, 2, 3, '09:00:00', '10:30:00'),
    (3, 2, 1, 2, '11:00:00', '12:30:00'),
    (4, 2, 3, 4, '11:00:00', '12:30:00'),
    (5, 3, 4, 1, '13:00:00', '14:30:00'),
    (6, 4, 4, 2, '14:45:00', '16:15:00'),
    (7, 5, 2, 3, '09:00:00', '10:30:00'),
    (8, 6, 3, 4, '10:45:00', '12:15:00'),
    (9, 7, 1, 5, '13:00:00', '14:30:00'),
    (10, 8, 4, 2, '08:30:00', '10:00:00');

-- 11. Enrollments
INSERT IGNORE INTO Enrollments (EnrollmentId, StudentId, CourseOfferingId, EnrollmentStatusId, RegistrationDate, TotalGrade, LetterGrade, GradePoints) VALUES
    (1, 1, 1, 1, '2026-08-05 09:00:00', NULL, NULL, NULL),
    (2, 2, 1, 1, '2026-08-05 09:05:00', NULL, NULL, NULL),
    (3, 3, 1, 1, '2026-08-05 09:10:00', NULL, NULL, NULL),
    (4, 4, 2, 1, '2026-08-06 09:15:00', NULL, NULL, NULL),
    (5, 1, 3, 1, '2026-08-06 09:20:00', NULL, NULL, NULL),
    (6, 4, 3, 1, '2026-08-06 09:25:00', NULL, NULL, NULL),
    (7, 5, 4, 2, '2026-08-06 09:30:00', NULL, NULL, NULL),
    (8, 5, 5, 1, '2027-01-05 10:00:00', NULL, NULL, NULL),
    (9, 6, 6, 1, '2027-01-05 10:05:00', NULL, NULL, NULL),
    (10, 2, 2, 1, '2026-08-05 09:35:00', NULL, NULL, NULL),
    (11, 3, 4, 1, '2026-08-06 09:45:00', NULL, NULL, NULL),
    (12, 4, 6, 1, '2027-01-05 10:10:00', NULL, NULL, NULL),
    (13, 1, 7, 3, '2027-01-08 10:30:00', NULL, NULL, NULL),
    (14, 5, 8, 1, '2027-06-05 11:00:00', NULL, NULL, NULL),
    (15, 2, 8, 1, '2027-06-05 11:05:00', NULL, NULL, NULL),
    (16, 4, 2, 1, '2026-08-05 09:50:00', NULL, NULL, NULL),
    (17, 5, 5, 1, '2027-01-05 10:15:00', NULL, NULL, NULL),
    (18, 6, 3, 4, '2026-08-06 09:55:00', 87.00, 'B+', 3.30);

-- 12. Lectures
INSERT IGNORE INTO Lectures (LectureId, CourseScheduleId, LectureDate, Topic) VALUES
    (1, 1, '2026-08-21', 'Introduction to programming workflows'),
    (2, 2, '2026-08-22', 'Data structures overview'),
    (3, 3, '2026-08-21', 'Management principles and planning'),
    (4, 4, '2026-08-22', 'Accounting basics and ledger entries'),
    (5, 5, '2027-01-11', 'Engineering design concepts'),
    (6, 6, '2027-01-12', 'Probability foundations and Bayes rule'),
    (7, 7, '2027-01-13', 'Software architecture and UML'),
    (8, 8, '2027-06-02', 'Professional responsibility');

-- 13. Attendance
INSERT IGNORE INTO Attendance (AttendanceId, LectureId, StudentId, AttendanceStatusId, Remarks) VALUES
    (1, 1, 1, 1, 'On time'),
    (2, 1, 2, 1, 'On time'),
    (3, 1, 3, 2, 'Absent due to illness'),
    (4, 2, 4, 1, 'Arrived early'),
    (5, 2, 2, 3, 'Late by 5 minutes'),
    (6, 3, 1, 1, 'Present'),
    (7, 4, 5, 1, 'Attended'),
    (8, 5, 5, 1, 'Present'),
    (9, 6, 6, 4, 'Excused absence for appointment'),
    (10, 7, 1, 1, 'Present');

-- 14. Student Holds
INSERT IGNORE INTO StudentHolds (StudentHoldId, StudentId, HoldType, Reason, PlacedDate, ReleasedDate, IsActive) VALUES
    (1, 3, 'Financial Hold', 'Outstanding tuition balance for Fall 2026.', '2026-07-01 12:00:00', '2026-09-01 12:00:00', 1),
    (2, 6, 'Academic Probation', 'GPA fell below minimum requirement.', '2026-06-30 12:00:00', NULL, 1);

SET FOREIGN_KEY_CHECKS = 1;
