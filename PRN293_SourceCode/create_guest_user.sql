-- Create guest user for cart functionality
-- Run this script in your EcommerceDB database

-- First, check if roles exist, if not create them
IF NOT EXISTS (SELECT 1 FROM User_role WHERE Role_name = 'Guest')
BEGIN
    INSERT INTO User_role (Role_name) VALUES ('Guest');
END

-- Create a guest user with ID 999
-- Check if guest user already exists
IF NOT EXISTS (SELECT 1 FROM [User] WHERE User_id = 999)
BEGIN
    INSERT INTO [User] (
        User_id,
        Role_id,
        Email,
        Password,
        Phone,
        User_name,
        Date_of_birth,
        Address,
        Create_date,
        Status,
        IsDelete
    ) VALUES (
        999, -- Guest user ID
        3, -- Guest role (assuming Guest role has ID 3, adjust if needed)
        'guest@example.com',
        'guest123', -- In production, this should be hashed
        '0000000000',
        'Guest User',
        '1990-01-01',
        'Guest Address',
        GETDATE(),
        1, -- Active
        0  -- Not deleted
    );
    
    PRINT 'Guest user created successfully!';
    PRINT 'User ID: 999';
    PRINT 'Email: guest@example.com';
    PRINT 'Role: Guest';
END
ELSE
BEGIN
    PRINT 'Guest user already exists!';
    SELECT User_id, Email, User_name, Role_id FROM [User] WHERE User_id = 999;
END

-- Show all users for verification
PRINT 'All users in database:';
SELECT User_id, Email, User_name, Role_id FROM [User] WHERE IsDelete = 0 ORDER BY User_id;

-- Show all roles for verification
PRINT 'All roles in database:';
SELECT Role_id, Role_name FROM User_role; 