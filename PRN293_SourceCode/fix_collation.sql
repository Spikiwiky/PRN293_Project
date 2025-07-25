-- Fix Database Collation for Vietnamese Unicode Support
-- Run this script BEFORE running sample_blogs.sql

-- Step 1: Change database collation to support Vietnamese
ALTER DATABASE EcommerceDB COLLATE Vietnamese_CI_AS

-- Step 2: Change collation for specific columns in Blog table
ALTER TABLE Blog 
ALTER COLUMN Blog_tittle NVARCHAR(MAX) COLLATE Vietnamese_CI_AS

ALTER TABLE Blog 
ALTER COLUMN Blog_content NVARCHAR(MAX) COLLATE Vietnamese_CI_AS

ALTER TABLE Blog 
ALTER COLUMN BlogSummary NVARCHAR(MAX) COLLATE Vietnamese_CI_AS

ALTER TABLE Blog 
ALTER COLUMN Tags NVARCHAR(MAX) COLLATE Vietnamese_CI_AS

-- Step 3: Change collation for Blog_category table
ALTER TABLE Blog_category 
ALTER COLUMN Blog_category_title NVARCHAR(255) COLLATE Vietnamese_CI_AS

PRINT 'Database collation updated successfully for Vietnamese support!'
PRINT 'Now you can run sample_blogs.sql' 