-- Alternative approach: Create guest user with identity insert
-- This script properly handles the identity column constraint

-- First, check if guest user already exists
IF NOT EXISTS (SELECT 1 FROM [User] WHERE [User_id] = 999)
BEGIN
    -- Enable identity insert to allow inserting specific ID
    SET IDENTITY_INSERT [User] ON;
    
    INSERT INTO [User] (
        [User_id],
        [User_name], 
        [Email], 
        [Password], 
        [Role_id], 
        [Status], 
        [Created_at], 
        [Updated_at]
    ) 
    VALUES (
        999,
        'guest_user', 
        'guest@example.com', 
        'guest_password_hash', 
        3, -- Assuming 3 is the guest role ID, adjust as needed
        1, -- Active status
        GETDATE(), 
        GETDATE()
    );
    
    -- Disable identity insert
    SET IDENTITY_INSERT [User] OFF;
    
    PRINT 'Guest user created successfully with ID: 999';
END
ELSE
BEGIN
    PRINT 'Guest user with ID 999 already exists';
END 