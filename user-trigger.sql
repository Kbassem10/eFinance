DELIMITER //

DROP TRIGGER IF EXISTS trg_Users_BeforeInsert //
CREATE TRIGGER trg_Users_BeforeInsert
BEFORE INSERT ON Users
FOR EACH ROW
BEGIN
    IF NEW.CreatedAt IS NULL THEN
        SET NEW.CreatedAt = NOW();
    END IF;
    IF NEW.UpdatedAt IS NULL THEN
        SET NEW.UpdatedAt = NOW();
    END IF;
END //

DROP TRIGGER IF EXISTS trg_Users_BeforeUpdate //
CREATE TRIGGER trg_Users_BeforeUpdate
BEFORE UPDATE ON Users
FOR EACH ROW
BEGIN
    SET NEW.UpdatedAt = NOW();
END //

DELIMITER ;