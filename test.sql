--Test SP
CALL sp_RegisterUser('user3@portal.edu', 'pass123', @id, @status, @msg);

SELECT @id AS UserId, @status AS StatusCode, @msg AS Message;

-- Test vw in sql command
SELECT * FROM vw_UserDetails;