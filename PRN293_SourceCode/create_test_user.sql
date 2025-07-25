-- Create test user and role for development/testing
-- Run this script in your EcommerceDB database

-- First, check if roles exist, if not create them
IF NOT EXISTS (SELECT 1 FROM User_role WHERE Role_name = 'Admin')
BEGIN
    INSERT INTO User_role (Role_name) VALUES ('Admin');
END

IF NOT EXISTS (SELECT 1 FROM User_role WHERE Role_name = 'Customer')
BEGIN
    INSERT INTO User_role (Role_name) VALUES ('Customer');
END

-- Create a test user (password: 123456)
-- Check if test user already exists
IF NOT EXISTS (SELECT 1 FROM [User] WHERE Email = 'test@example.com')
BEGIN
    INSERT INTO [User] (
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
        2, -- Customer role (assuming Customer role has ID 2)
        'test@example.com',
        '123456', -- In production, this should be hashed
        '0123456789',
        'Test User',
        '1990-01-01',
        '123 Test Street, Test City',
        GETDATE(),
        1, -- Active
        0  -- Not deleted
    );
    
    PRINT 'Test user created successfully!';
    PRINT 'Email: test@example.com';
    PRINT 'Password: 123456';
    PRINT 'User ID: ' + CAST(SCOPE_IDENTITY() AS VARCHAR(10));
END
ELSE
BEGIN
    PRINT 'Test user already exists!';
    SELECT User_id, Email, User_name FROM [User] WHERE Email = 'test@example.com';
END

-- Show all users for verification
PRINT 'All users in database:';
SELECT User_id, Email, User_name, Role_id FROM [User] WHERE IsDelete = 0;

-- Show all roles for verification
PRINT 'All roles in database:';
SELECT Role_id, Role_name FROM User_role; 