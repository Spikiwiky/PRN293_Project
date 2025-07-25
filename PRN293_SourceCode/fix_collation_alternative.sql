-- Alternative Fix for Vietnamese Unicode Support
-- This approach recreates the columns with proper collation

-- Step 1: Clean up existing corrupted data
DELETE FROM Blog WHERE Blog_tittle LIKE '%Cách%' OR Blog_tittle LIKE '%Váy%' OR Blog_tittle LIKE '%Mùa%'
DELETE FROM Blog_category WHERE Blog_category_title IN (N'Thời trang', N'Lifestyle', N'Beauty')

-- Step 2: Add new columns with proper collation
ALTER TABLE Blog ADD Blog_tittle_new NVARCHAR(MAX) COLLATE Vietnamese_CI_AS
ALTER TABLE Blog ADD Blog_content_new NVARCHAR(MAX) COLLATE Vietnamese_CI_AS
ALTER TABLE Blog ADD BlogSummary_new NVARCHAR(MAX) COLLATE Vietnamese_CI_AS
ALTER TABLE Blog ADD Tags_new NVARCHAR(MAX) COLLATE Vietnamese_CI_AS

-- Step 3: Copy data from old columns to new columns
UPDATE Blog SET 
    Blog_tittle_new = Blog_tittle,
    Blog_content_new = Blog_content,
    BlogSummary_new = BlogSummary,
    Tags_new = Tags

-- Step 4: Drop old columns
ALTER TABLE Blog DROP COLUMN Blog_tittle
ALTER TABLE Blog DROP COLUMN Blog_content
ALTER TABLE Blog DROP COLUMN BlogSummary
ALTER TABLE Blog DROP COLUMN Tags

-- Step 5: Rename new columns to original names
EXEC sp_rename 'Blog.Blog_tittle_new', 'Blog_tittle', 'COLUMN'
EXEC sp_rename 'Blog.Blog_content_new', 'Blog_content', 'COLUMN'
EXEC sp_rename 'Blog.BlogSummary_new', 'BlogSummary', 'COLUMN'
EXEC sp_rename 'Blog.Tags_new', 'Tags', 'COLUMN'

-- Step 6: Fix Blog_category table
ALTER TABLE Blog_category ADD Blog_category_title_new NVARCHAR(255) COLLATE Vietnamese_CI_AS
UPDATE Blog_category SET Blog_category_title_new = Blog_category_title
ALTER TABLE Blog_category DROP COLUMN Blog_category_title
EXEC sp_rename 'Blog_category.Blog_category_title_new', 'Blog_category_title', 'COLUMN'

PRINT 'Columns recreated with proper Vietnamese collation!'
PRINT 'Now you can run sample_blogs.sql' 