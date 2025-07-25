-- Add guest user to the database
-- This user will be used for guest cart functionality

INSERT INTO [User] (
    [User_name], 
    [Email], 
    [Password], 
    [Role_id], 
    [Status], 
    [Created_at], 
    [Updated_at]
) 
VALUES (
    'guest_user', 
    'guest@example.com', 
    'guest_password_hash', 
    3, -- Assuming 3 is the guest role ID, adjust as needed
    1, -- Active status
    GETDATE(), 
    GETDATE()
);

-- Get the inserted user ID
DECLARE @GuestUserId INT = SCOPE_IDENTITY();

-- Update the guest user ID to 999 for consistency
UPDATE [User] 
SET [User_id] = 999 
WHERE [User_id] = @GuestUserId;

-- If the above doesn't work due to identity column, use this alternative:
-- First, enable identity insert
-- SET IDENTITY_INSERT [User] ON;
-- 
-- INSERT INTO [User] (
--     [User_id],
--     [User_name], 
--     [Email], 
--     [Password], 
--     [Role_id], 
--     [Status], 
--     [Created_at], 
--     [Updated_at]
-- ) 
-- VALUES (
--     999,
--     'guest_user', 
--     'guest@example.com', 
--     'guest_password_hash', 
--     3, -- Assuming 3 is the guest role ID, adjust as needed
--     1, -- Active status
--     GETDATE(), 
--     GETDATE()
-- );
-- 
-- SET IDENTITY_INSERT [User] OFF;

PRINT 'Guest user created with ID: 999'; 