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