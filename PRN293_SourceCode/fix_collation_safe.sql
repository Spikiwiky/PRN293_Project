-- Safe Fix for Vietnamese Unicode Support
-- This approach only changes specific columns without altering database collation

-- Step 1: Clean up existing corrupted data first
DELETE FROM Blog WHERE Blog_tittle LIKE '%Cách%' OR Blog_tittle LIKE '%Váy%' OR Blog_tittle LIKE '%Mùa%'
DELETE FROM Blog_category WHERE Blog_category_title IN (N'Thời trang', N'Lifestyle', N'Beauty')

-- Step 2: Change collation for specific columns in Blog table
-- Note: This will work even with SQL_Latin1_General_CP1_CI_AS database collation
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

PRINT 'Column collations updated successfully for Vietnamese support!'
PRINT 'Now you can run sample_blogs.sql' 