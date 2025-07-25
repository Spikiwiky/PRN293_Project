-- Kiểm tra sản phẩm và hình ảnh trong database
SELECT 
    p.Product_id,
    p.Product_name,
    p.Base_price,
    COUNT(pi.Image_url) as ImageCount,
    STRING_AGG(pi.Image_url, ', ') as ImageUrls
FROM Product p
LEFT JOIN Product_image pi ON p.Product_id = pi.Product_id
GROUP BY p.Product_id, p.Product_name, p.Base_price
ORDER BY p.Product_id;

-- Kiểm tra bảng Product_image
SELECT TOP 20 * FROM Product_image;

-- Đếm tổng số sản phẩm có hình ảnh
SELECT 
    COUNT(DISTINCT p.Product_id) as TotalProducts,
    COUNT(DISTINCT CASE WHEN pi.Image_url IS NOT NULL THEN p.Product_id END) as ProductsWithImages,
    COUNT(pi.Image_url) as TotalImages
FROM Product p
LEFT JOIN Product_image pi ON p.Product_id = pi.Product_id; 