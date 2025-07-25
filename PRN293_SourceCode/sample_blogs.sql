-- Sample Blog Data for Testing
-- Make sure to run this after the migration is applied

-- Clean up existing data first
DELETE FROM Blog WHERE Blog_tittle LIKE '%Cách%' OR Blog_tittle LIKE '%Váy%' OR Blog_tittle LIKE '%Mùa%'
DELETE FROM Blog_category WHERE Blog_category_title IN (N'Thời trang', N'Lifestyle', N'Beauty')

-- Insert sample blog categories if they don't exist
IF NOT EXISTS (SELECT * FROM Blog_category WHERE Blog_category_title = 'Fashion')
BEGIN
    INSERT INTO Blog_category (Blog_category_title, IsDelete) VALUES ('Fashion', 0)
END

IF NOT EXISTS (SELECT * FROM Blog_category WHERE Blog_category_title = 'Lifestyle')
BEGIN
    INSERT INTO Blog_category (Blog_category_title, IsDelete) VALUES ('Lifestyle', 0)
END

IF NOT EXISTS (SELECT * FROM Blog_category WHERE Blog_category_title = 'Beauty')
BEGIN
    INSERT INTO Blog_category (Blog_category_title, IsDelete) VALUES ('Beauty', 0)
END

-- Get category IDs
DECLARE @FashionCategoryId INT = (SELECT Blog_category_id FROM Blog_category WHERE Blog_category_title = 'Fashion')
DECLARE @LifestyleCategoryId INT = (SELECT Blog_category_id FROM Blog_category WHERE Blog_category_title = 'Lifestyle')
DECLARE @BeautyCategoryId INT = (SELECT Blog_category_id FROM Blog_category WHERE Blog_category_title = 'Beauty')

-- Get a user ID (assuming there's at least one user)
DECLARE @UserId INT = (SELECT TOP 1 User_id FROM [User] WHERE User_id IS NOT NULL)

-- Insert sample blogs
INSERT INTO Blog (
    Blog_category_id, 
    Blog_tittle, 
    Blog_content, 
    BlogSummary, 
    BlogImage, 
    UserId, 
    PublishedDate, 
    CreatedDate, 
    UpdatedDate, 
    IsPublished, 
    ViewCount, 
    Tags
) VALUES 
(
    @FashionCategoryId,
    '8 Inspiring Ways to Wear Dresses in Winter',
    'Winter is not just about bundling up in warm coats, but it''s also an opportunity for girls to express their personality and unique fashion sense. How to achieve outfits that not only keep you warm but also shine creatively during gloomy winter days. GUMAC Fashion will show you winter styling tips for women that are extremely FASHIONABLE while ensuring you stay warm during cold winter days.',
    'Discover 8 inspiring ways to wear dresses in winter, helping you stay both warm and fashionable.',
    '/images/blog-01.jpg',
    @UserId,
    GETDATE(),
    GETDATE(),
    GETDATE(),
    1,
    1250,
    'fashion,winter,dress,style'
),
(
    @LifestyleCategoryId,
    'Perfect Gift Guide for Men on Special Occasions',
    'Finding gifts for men can be a challenge, especially during important holidays. From high-end fashion items to modern tech accessories, there are many choices to express your feelings to the important men in your life.',
    'Discover the perfect gift guide for men on all occasions, from fashion to technology.',
    '/images/blog-02.jpg',
    @UserId,
    DATEADD(day, -5, GETDATE()),
    DATEADD(day, -5, GETDATE()),
    DATEADD(day, -5, GETDATE()),
    1,
    890,
    'gifts,men,fashion,accessories'
),
(
    @BeautyCategoryId,
    '5 Winter-Spring Fashion Trends You Need to Try Now',
    'Winter is gradually passing and spring is approaching. This is the perfect time to update your wardrobe with the latest fashion trends. From soft pastel tones to blooming floral patterns, let''s explore 5 winter-spring fashion trends that are currently popular.',
    'Update your wardrobe with 5 winter-spring fashion trends that are most popular right now.',
    '/images/blog-03.jpg',
    @UserId,
    DATEADD(day, -10, GETDATE()),
    DATEADD(day, -10, GETDATE()),
    DATEADD(day, -10, GETDATE()),
    1,
    1560,
    'trends,winter spring,fashion,colors'
),
(
    @FashionCategoryId,
    'Guide to Elegant Men''s Event Styling',
    'Men''s event styling is not just about looking good but also showing respect and professionalism. From classic suits to modern outfits, let''s explore how to style men for events in an elegant and impressive way.',
    'Detailed guide on how to style men for events elegantly and professionally.',
    '/images/blog-04.jpg',
    @UserId,
    DATEADD(day, -15, GETDATE()),
    DATEADD(day, -15, GETDATE()),
    DATEADD(day, -15, GETDATE()),
    1,
    2100,
    'men,styling,events,elegant'
),
(
    @LifestyleCategoryId,
    'Secrets to Choosing the Perfect Bag for Every Occasion',
    'Bags are not just fashion accessories but also essential items in daily life. Choosing the right bag can elevate your beauty and show sophisticated fashion taste.',
    'Discover secrets to choosing the perfect bag for every occasion and personal style.',
    '/images/blog-05.jpg',
    @UserId,
    DATEADD(day, -20, GETDATE()),
    DATEADD(day, -20, GETDATE()),
    DATEADD(day, -20, GETDATE()),
    1,
    980,
    'bags,accessories,fashion,style'
),
(
    @FashionCategoryId,
    '10 Women''s Fashion Trends 2024 You Can''t Miss',
    '2024 has brought incredibly interesting and diverse women''s fashion trends. From blooming floral patterns to soft pastel tones, from delicate lace details to modern metal accessories. Let''s explore 10 women''s fashion trends that are most popular in 2024.',
    'Discover 10 women''s fashion trends 2024 that are most popular, from patterns to colors.',
    '/images/blog-06.jpg',
    @UserId,
    DATEADD(day, -25, GETDATE()),
    DATEADD(day, -25, GETDATE()),
    DATEADD(day, -25, GETDATE()),
    1,
    1870,
    'trends,women fashion,2024,style'
),
(
    @BeautyCategoryId,
    'Secrets to Choosing Lipstick Colors for Every Skin Tone',
    'Choosing the right lipstick color for your skin tone can completely change your appearance. From natural nude tones to bold red shades, each skin tone will suit different lipstick colors. Let''s learn how to choose the perfect lipstick color for each skin type.',
    'Detailed guide on choosing lipstick colors that suit each skin tone to enhance natural beauty.',
    '/images/blog-01.jpg',
    @UserId,
    DATEADD(day, -30, GETDATE()),
    DATEADD(day, -30, GETDATE()),
    DATEADD(day, -30, GETDATE()),
    1,
    1420,
    'lipstick,makeup,skin tone,beauty'
),
(
    @LifestyleCategoryId,
    'How to Create Unique Personal Fashion Style',
    'Personal fashion style is not just about looking good but also expressing personality and unique aesthetic taste. From color combinations to accessory styling, let''s explore how to create a unique and impressive personal fashion style.',
    'Discover how to create unique personal fashion style that expresses individual personality.',
    '/images/blog-02.jpg',
    @UserId,
    DATEADD(day, -35, GETDATE()),
    DATEADD(day, -35, GETDATE()),
    DATEADD(day, -35, GETDATE()),
    1,
    1680,
    'style,personal,fashion,individual'
),
(
    @FashionCategoryId,
    'Guide to Elegant and Professional Office Styling',
    'Office styling needs to be not only elegant but also show professionalism. From classic suits to modern outfits, let''s explore how to style for the office in an elegant and professional way suitable for the work environment.',
    'Detailed guide on elegant and professional office styling for the work environment.',
    '/images/blog-03.jpg',
    @UserId,
    DATEADD(day, -40, GETDATE()),
    DATEADD(day, -40, GETDATE()),
    DATEADD(day, -40, GETDATE()),
    1,
    1950,
    'office,elegant,professional,outfit'
),
(
    @BeautyCategoryId,
    '5 Basic Skincare Steps for Beginners',
    'Skincare is an important process to maintain healthy and youthful skin. From cleansing to moisturizing, from sun protection to exfoliation, let''s learn 5 basic skincare steps for beginners.',
    'Guide to 5 basic skincare steps for beginners to achieve healthy skin.',
    '/images/blog-04.jpg',
    @UserId,
    DATEADD(day, -45, GETDATE()),
    DATEADD(day, -45, GETDATE()),
    DATEADD(day, -45, GETDATE()),
    1,
    2230,
    'skincare,beauty,skin care,healthy skin'
);

PRINT 'Sample blog data inserted successfully!' 