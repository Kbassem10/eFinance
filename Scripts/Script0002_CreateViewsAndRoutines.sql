-- ==========================================================
-- Migration 0002: Views, Stored Procedures, Functions
-- ==========================================================

-- 1. VIEWS
CREATE OR REPLACE VIEW vw_Students AS
SELECT 
    s.StudentId,
    s.StudentNumber,
    s.FirstName,
    s.MiddleName,
    s.LastName,
    CONCAT(s.FirstName, ' ', IFNULL(CONCAT(s.MiddleName, ' '), ''), s.LastName) AS FullName,
    u.Email,
    s.NationalId,
    s.DateOfBirth,
    s.Gender,
    s.PhoneNumber,
    s.Address,
    s.AdmissionDate,
    s.AcademicLevel,
    s.GPA,
    s.CompletedCreditHours,
    d.DepartmentId,
    d.DepartmentName,
    d.DepartmentCode,
    st.StudentStatusId,
    st.StatusName AS StatusName,
    s.CreatedAt,
    s.UpdatedAt
FROM Students s
INNER JOIN Users u ON s.UserId = u.UserId
INNER JOIN Departments d ON s.DepartmentId = d.DepartmentId
INNER JOIN StudentStatuses st ON s.StudentStatusId = st.StudentStatusId;

CREATE OR REPLACE VIEW vw_UserDetails AS
SELECT 
    u.UserId,
    u.Email,
    u.IsActive,
    GROUP_CONCAT(r.RoleName SEPARATOR ', ') AS Roles,
    CASE 
        WHEN s.StudentId IS NOT NULL THEN 'Student'
        WHEN i.InstructorId IS NOT NULL THEN 'Instructor'
        ELSE 'Admin/Staff'
    END AS ProfileType,
    COALESCE(
        CONCAT(s.FirstName, ' ', s.LastName),
        CONCAT(i.FirstName, ' ', i.LastName),
        'N/A'
    ) AS ProfileName,
    u.CreatedAt,
    u.UpdatedAt
FROM Users u
LEFT JOIN UserRoles ur ON u.UserId = ur.UserId
LEFT JOIN Roles r ON ur.RoleId = r.RoleId
LEFT JOIN Students s ON u.UserId = s.UserId
LEFT JOIN Instructors i ON u.UserId = i.UserId
GROUP BY 
    u.UserId, 
    u.Email, 
    u.IsActive, 
    s.StudentId, 
    i.InstructorId, 
    s.FirstName, 
    s.LastName, 
    i.FirstName, 
    i.LastName, 
    u.CreatedAt, 
    u.UpdatedAt;

CREATE OR REPLACE VIEW vw_Instructors AS
SELECT 
    i.InstructorId,
    i.EmployeeNumber,
    i.FirstName,
    i.LastName,
    CONCAT(i.FirstName, ' ', IFNULL(CONCAT(i.MiddleName, ' '), ''), i.LastName) AS FullName,
    u.Email,
    i.AcademicTitle,
    i.Salary,
    d.DepartmentId,
    d.DepartmentName,
    insSt.StatusName AS StatusName,
    i.CreatedAt
FROM Instructors i
INNER JOIN Users u ON i.UserId = u.UserId
INNER JOIN Departments d ON i.DepartmentId = d.DepartmentId
INNER JOIN InstructorStatuses insSt ON i.InstructorStatusId = insSt.InstructorStatusId;

CREATE OR REPLACE VIEW vw_Courses AS
SELECT 
    c.CourseId,
    c.CourseCode,
    c.CourseName,
    c.CreditHours,
    c.DifficultyLevel,
    cs.StatusName AS StatusName,
    GROUP_CONCAT(d.DepartmentName SEPARATOR ', ') AS AssignedDepartments
FROM Courses c
INNER JOIN CourseStatuses cs ON c.CourseStatusId = cs.CourseStatusId
LEFT JOIN CourseDepartments cd ON c.CourseId = cd.CourseId
LEFT JOIN Departments d ON cd.DepartmentId = d.DepartmentId
GROUP BY c.CourseId, c.CourseCode, c.CourseName, c.CreditHours, c.DifficultyLevel, cs.StatusName;

CREATE OR REPLACE VIEW vw_Enrollments AS
SELECT 
    e.EnrollmentId,
    s.StudentId,
    s.StudentNumber,
    CONCAT(s.FirstName, ' ', s.LastName) AS StudentName,
    c.CourseCode,
    c.CourseName,
    co.SectionNumber,
    sem.SemesterName,
    sem.AcademicYear,
    es.StatusName AS EnrollmentStatus,
    e.TotalGrade,
    e.LetterGrade,
    e.GradePoints
FROM Enrollments e
INNER JOIN Students s ON e.StudentId = s.StudentId
INNER JOIN CourseOfferings co ON e.CourseOfferingId = co.CourseOfferingId
INNER JOIN Courses c ON co.CourseId = c.CourseId
INNER JOIN Semesters sem ON co.SemesterId = sem.SemesterId
INNER JOIN EnrollmentStatuses es ON e.EnrollmentStatusId = es.EnrollmentStatusId;

-- 2. FUNCTIONS
DROP FUNCTION IF EXISTS fn_GetStudentTotalCreditHours;
CREATE FUNCTION fn_GetStudentTotalCreditHours(
    p_StudentId INT,
    p_SemesterId INT
)
RETURNS INT
DETERMINISTIC
READS SQL DATA
BEGIN
    DECLARE v_TotalCreditHours INT DEFAULT 0;

    SELECT IFNULL(SUM(c.CreditHours), 0)
    INTO v_TotalCreditHours
    FROM Enrollments e
    INNER JOIN CourseOfferings co ON e.CourseOfferingId = co.CourseOfferingId
    INNER JOIN Courses c ON co.CourseId = c.CourseId
    WHERE e.StudentId = p_StudentId
      AND co.SemesterId = p_SemesterId
      AND e.EnrollmentStatusId IN (1, 4);

    RETURN v_TotalCreditHours;
END;

-- 3. STORED PROCEDURES
DROP PROCEDURE IF EXISTS sp_ManageStudent;
CREATE PROCEDURE sp_ManageStudent(
    IN p_ActionType VARCHAR(10),
    INOUT p_StudentId INT,
    IN p_UserId INT,
    IN p_DepartmentId INT,
    IN p_StudentStatusId INT,
    IN p_StudentNumber VARCHAR(30),
    IN p_FirstName VARCHAR(100),
    IN p_LastName VARCHAR(100),
    IN p_NationalId VARCHAR(30),
    IN p_AdmissionDate DATE,
    OUT p_ProcessingStatus INT,
    OUT p_ProcessingMessage VARCHAR(255)
)
proc_label: BEGIN
    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        ROLLBACK;
        SET p_ProcessingStatus = 0;
        SET p_ProcessingMessage = 'Database Exception: Transaction rolled back.';
    END;

    SET p_ProcessingStatus = 1;
    SET p_ProcessingMessage = 'Operation completed successfully.';

    IF p_ActionType IN ('INSERT', 'UPDATE') THEN
        IF NOT EXISTS (SELECT 1 FROM Departments WHERE DepartmentId = p_DepartmentId) THEN
            SET p_ProcessingStatus = 0;
            SET p_ProcessingMessage = CONCAT('Validation Error: Department ID ', p_DepartmentId, ' does not exist.');
            LEAVE proc_label;
        END IF;

        IF NOT EXISTS (SELECT 1 FROM StudentStatuses WHERE StudentStatusId = p_StudentStatusId) THEN
            SET p_ProcessingStatus = 0;
            SET p_ProcessingMessage = CONCAT('Validation Error: Status ID ', p_StudentStatusId, ' does not exist.');
            LEAVE proc_label;
        END IF;

        IF YEAR(p_AdmissionDate) > 2026 THEN
            SET p_ProcessingStatus = 0;
            SET p_ProcessingMessage = 'Validation Error: Admission year cannot exceed 2026.';
            LEAVE proc_label;
        END IF;
    END IF;

    START TRANSACTION;

    IF p_ActionType = 'INSERT' THEN
        IF EXISTS (SELECT 1 FROM Students WHERE NationalId = p_NationalId) THEN
            SET p_ProcessingStatus = 0;
            SET p_ProcessingMessage = 'Validation Error: National ID already registered.';
            ROLLBACK;
            LEAVE proc_label;
        END IF;

        INSERT INTO Students (
            UserId, DepartmentId, StudentStatusId, StudentNumber, 
            FirstName, LastName, NationalId, AdmissionDate, GPA, CompletedCreditHours, CreatedAt, UpdatedAt
        ) VALUES (
            p_UserId, p_DepartmentId, p_StudentStatusId, p_StudentNumber,
            p_FirstName, p_LastName, p_NationalId, p_AdmissionDate, 0.00, 0, NOW(), NOW()
        );
        SET p_StudentId = LAST_INSERT_ID();
        SET p_ProcessingMessage = CONCAT('Student created successfully with ID ', p_StudentId);

    ELSEIF p_ActionType = 'UPDATE' THEN
        UPDATE Students
        SET DepartmentId = p_DepartmentId,
            StudentStatusId = p_StudentStatusId,
            FirstName = p_FirstName,
            LastName = p_LastName,
            AdmissionDate = p_AdmissionDate,
            UpdatedAt = NOW()
        WHERE StudentId = p_StudentId;
        SET p_ProcessingMessage = CONCAT('Student ID ', p_StudentId, ' updated successfully.');

    ELSEIF p_ActionType = 'DELETE' THEN
        DELETE FROM Students WHERE StudentId = p_StudentId;
        SET p_ProcessingMessage = CONCAT('Student ID ', p_StudentId, ' deleted successfully.');
    END IF;

    COMMIT;
END;
