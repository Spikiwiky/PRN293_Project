-- Cleanup Product Images Script
-- Xóa các ảnh sản phẩm trùng lặp và không sử dụng

-- 1. Xem tất cả ảnh sản phẩm hiện tại
SELECT 
    pi.Product_image_id,
    pi.Product_id,
    pi.Image_url,
    p.Name as ProductName
FROM Product_image pi
LEFT JOIN products p ON pi.Product_id = p.product_id
ORDER BY pi.Product_id, pi.Product_image_id;

-- 2. Xóa các ảnh có URL trùng lặp (giữ lại ảnh đầu tiên)
DELETE pi1
FROM Product_image pi1
INNER JOIN Product_image pi2 
WHERE pi1.Product_image_id > pi2.Product_image_id 
AND pi1.Image_url = pi2.Image_url;

-- 3. Xóa các ảnh có URL không hợp lệ hoặc không tồn tại
DELETE FROM Product_image 
WHERE Image_url IS NULL 
OR Image_url = '' 
OR Image_url NOT LIKE '/images/products/%';

-- 4. Xóa các ảnh của sản phẩm đã bị xóa
DELETE pi
FROM Product_image pi
LEFT JOIN products p ON pi.Product_id = p.product_id
WHERE p.product_id IS NULL OR p.is_delete = 1;

-- 5. Kiểm tra kết quả sau khi dọn dẹp
SELECT 
    pi.Product_image_id,
    pi.Product_id,
    pi.Image_url,
    p.Name as ProductName
FROM Product_image pi
LEFT JOIN products p ON pi.Product_id = p.product_id
ORDER BY pi.Product_id, pi.Product_image_id;

PRINT 'Product images cleanup completed!'; 