USE StudentRegistrationPortal;

-- ============================================================================
-- 1. TEST STORED PROCEDURE: sp_RegisterUser
-- ============================================================================

-- Test 1.1: Success Case (Valid user registration)
CALL sp_RegisterUser('sarah.connor@portal.edu', 'P@ssw0rd123!', @id, @status, @msg);
SELECT @id AS NewUserId, @status AS StatusCode, @msg AS StatusMessage;

-- Verify the inserted record
SELECT UserId, Email, IsActive, CreatedAt, UpdatedAt 
FROM Users 
WHERE UserId = @id;

-- Test 1.2: Validation Failure (Empty Password)
CALL sp_RegisterUser('user_no_pass@portal.edu', '', @id_err1, @status_err1, @msg_err1);
SELECT @id_err1 AS NewUserId, @status_err1 AS StatusCode, @msg_err1 AS StatusMessage;

-- Test 1.3: Validation Failure (Invalid Email Format)
CALL sp_RegisterUser('invalid_email_format', 'P@ssw0rd123!', @id_err2, @status_err2, @msg_err2);
SELECT @id_err2 AS NewUserId, @status_err2 AS StatusCode, @msg_err2 AS StatusMessage;

-- Test 1.4: Validation Failure (Duplicate Email)
CALL sp_RegisterUser('sarah.connor@portal.edu', 'AnotherPassword!', @id_err3, @status_err3, @msg_err3);
SELECT @id_err3 AS NewUserId, @status_err3 AS StatusCode, @msg_err3 AS StatusMessage;


-- ============================================================================
-- 2. TEST TRIGGERS: trg_Users_BeforeInsert & trg_Users_BeforeUpdate
-- ============================================================================

-- Test 2.1: Insert Trigger (CreatedAt and UpdatedAt auto-generated)
INSERT INTO Users (Email, PasswordHash, IsActive)
VALUES ('trigger.test@portal.edu', 'hash_test_123', 1);

SELECT UserId, Email, CreatedAt, UpdatedAt 
FROM Users 
WHERE Email = 'trigger.test@portal.edu';

-- Test 2.2: Update Trigger (UpdatedAt refreshed automatically)
DO SLEEP(1); -- Small delay to see timestamp difference
UPDATE Users 
SET IsActive = 0 
WHERE Email = 'trigger.test@portal.edu';

SELECT UserId, Email, IsActive, CreatedAt, UpdatedAt 
FROM Users 
WHERE Email = 'trigger.test@portal.edu';


-- ============================================================================
-- 3. TEST VIEWS
-- ============================================================================

-- Test 3.1: User Details View (User accounts + Roles + Profile Names)
SELECT * FROM vw_UserDetails LIMIT 10;

-- Test 3.2: Filter View by ProfileType
SELECT UserId, Email, Roles, ProfileType, ProfileName 
FROM vw_UserDetails 
WHERE ProfileType = 'Student';

-- Test 3.3: Master Students View
SELECT StudentNumber, FullName, Email, DepartmentName, StatusName, GPA 
FROM vw_Students 
LIMIT 5;

-- Test 3.4: Master Courses View
SELECT CourseCode, CourseName, CreditHours, StatusName, AssignedDepartments 
FROM vw_Courses;

-- Test 3.5: Master Enrollments View
SELECT StudentName, CourseCode, CourseName, SemesterName, EnrollmentStatus, LetterGrade 
FROM vw_Enrollments 
LIMIT 5;


-- ============================================================================
-- 4. TEST FUNCTION: fn_GetStudentTotalCreditHours
-- ============================================================================

-- Calculate total registered credits for Student #1 in Fall 2026 (Semester #1)
SELECT 
    s.StudentId,
    s.FullName,
    fn_GetStudentTotalCreditHours(s.StudentId, 1) AS Fall2026_TotalCredits
FROM vw_Students s
LIMIT 3;