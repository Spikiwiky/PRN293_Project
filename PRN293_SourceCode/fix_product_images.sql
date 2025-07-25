-- Kiểm tra sản phẩm và hình ảnh hiện tại
SELECT 
    p.Product_id,
    p.Product_name,
    pc.ProductCategoryTitle as Category,
    COUNT(pi.Image_url) as ImageCount
FROM Product p
LEFT JOIN Product_category pc ON p.Product_category_id = pc.Product_category_id
LEFT JOIN Product_image pi ON p.Product_id = pi.Product_id
WHERE p.IsDelete = 0
GROUP BY p.Product_id, p.Product_name, pc.ProductCategoryTitle
ORDER BY ImageCount ASC, p.Product_id;

-- Thêm hình ảnh mẫu cho các sản phẩm không có hình ảnh
-- Lấy danh sách sản phẩm không có hình ảnh
DECLARE @ProductId INT;
DECLARE @ImageCounter INT = 1;

DECLARE product_cursor CURSOR FOR
SELECT p.Product_id
FROM Product p
LEFT JOIN Product_image pi ON p.Product_id = pi.Product_id
WHERE p.IsDelete = 0 AND pi.Image_url IS NULL
ORDER BY p.Product_id;

OPEN product_cursor;
FETCH NEXT FROM product_cursor INTO @ProductId;

WHILE @@FETCH_STATUS = 0
BEGIN
    -- Thêm hình ảnh mẫu dựa trên category
    DECLARE @CategoryId INT = (SELECT Product_category_id FROM Product WHERE Product_id = @ProductId);
    DECLARE @ImageUrl NVARCHAR(500);
    
    -- Chọn hình ảnh mẫu dựa trên category
    IF @CategoryId = 1 -- Men Clothing
        SET @ImageUrl = '/images/products/aoThunNam.jpg';
    ELSE IF @CategoryId = 2 -- Women Clothing
        SET @ImageUrl = '/images/products/summerDress.jpg';
    ELSE IF @CategoryId = 3 -- Accessories
        SET @ImageUrl = '/images/products/summerHat.jpg';
    ELSE IF @CategoryId = 4 -- Footwear
        SET @ImageUrl = '/images/products/runningShoes.jpg';
    ELSE IF @CategoryId = 5 -- Outerwear
        SET @ImageUrl = '/images/products/winterCoat.jpg';
    ELSE IF @CategoryId = 6 -- Formal Wear
        SET @ImageUrl = '/images/products/businessSuit.jpg';
    ELSE IF @CategoryId = 7 -- Swimwear
        SET @ImageUrl = '/images/products/swimmingTrunks.jpg';
    ELSE IF @CategoryId = 8 -- Sportswear
        SET @ImageUrl = '/images/products/classTShirt.jpg';
    ELSE
        SET @ImageUrl = '/images/products/product-min-01.jpg';
    
    -- Thêm hình ảnh vào database
    INSERT INTO Product_image (Product_id, Image_url, Created_at, Updated_at)
    VALUES (@ProductId, @ImageUrl, GETDATE(), GETDATE());
    
    PRINT 'Added image for Product ID: ' + CAST(@ProductId AS NVARCHAR(10)) + ' - ' + @ImageUrl;
    
    FETCH NEXT FROM product_cursor INTO @ProductId;
END

CLOSE product_cursor;
DEALLOCATE product_cursor;

-- Kiểm tra kết quả sau khi thêm
SELECT 
    p.Product_id,
    p.Product_name,
    pc.ProductCategoryTitle as Category,
    COUNT(pi.Image_url) as ImageCount,
    STRING_AGG(pi.Image_url, ', ') as ImageUrls
FROM Product p
LEFT JOIN Product_category pc ON p.Product_category_id = pc.Product_category_id
LEFT JOIN Product_image pi ON p.Product_id = pi.Product_id
WHERE p.IsDelete = 0
GROUP BY p.Product_id, p.Product_name, pc.ProductCategoryTitle
ORDER BY p.Product_id; 