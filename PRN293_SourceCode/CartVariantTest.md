# Test Cart Variant Attributes Logic - FIXED

## Vấn đề đã được sửa

**Vấn đề**: Khi tăng số lượng sản phẩm "Swimming Trunks" biến thể thứ 2 (size M, color Black), hệ thống lại thay đổi số lượng biến thể thứ nhất (size S, color Blue).

**Nguyên nhân**: 
1. Logic tìm kiếm cart item chỉ dựa trên `productId` và `variantId`, không kiểm tra `variantAttributes`
2. Frontend sử dụng endpoint `/api/Cart/update` thay vì `/api/Cart/increase` cho việc tăng số lượng
3. Logic so sánh `variantAttributes` không nhất quán giữa CartController và CartRepository

## Các thay đổi đã thực hiện

### 1. Backend API DTOs ✅
- ✅ Thêm `VariantAttributes` vào `UpdateCartItemByProductDto`
- ✅ Thêm `VariantAttributes` vào `RemoveFromCartByProductDto`
- ✅ Thêm `IncreaseCartItemByProductDto` và `DecreaseCartItemByProductDto`

### 2. Backend CartController Logic ✅
- ✅ Cập nhật logic tìm kiếm trong `UpdateCartItemByProduct` để bao gồm `variantAttributes`
- ✅ Cập nhật logic tìm kiếm trong `RemoveFromCartByProduct` để bao gồm `variantAttributes`
- ✅ Thêm endpoint `IncreaseCartItemByProduct` và `DecreaseCartItemByProduct`
- ✅ Thêm logging chi tiết để debug

### 3. Frontend DTOs ✅
- ✅ Thêm `VariantAttributes` vào `UpdateCartRequest`
- ✅ Thêm `VariantAttributes` vào `RemoveCartRequest`
- ✅ Thêm `IncreaseCartItemByProductRequest` và `DecreaseCartItemByProductRequest`

### 4. Frontend CartController ✅
- ✅ Cập nhật `UpdateCartItem` để gửi `variantAttributes`
- ✅ Cập nhật `RemoveFromCart` để gửi `variantAttributes`
- ✅ Thêm endpoint `IncreaseCartItemByProduct` và `DecreaseCartItemByProduct`

### 5. Frontend UI ✅
- ✅ Cập nhật HTML để truyền `variantAttributes` vào JavaScript functions
- ✅ Cập nhật JavaScript để sử dụng endpoint `/api/Cart/increase-by-product` và `/api/Cart/decrease-by-product`
- ✅ Thêm functions `increaseCartItem` và `decreaseCartItem` riêng biệt

### 6. CartRepository Logic ✅
- ✅ Cập nhật `AreVariantAttributesEqual` để sử dụng JSON parsing logic nhất quán với CartController
- ✅ Thêm using statement cho `System.Text.Json`

## Test Cases

### Test Case 1: Cập nhật số lượng biến thể khác nhau
```
Giỏ hàng có:
1. Swimming Trunks - {"size":"S","color":"Blue"} - Quantity: 8
2. Swimming Trunks - {"size":"M","color":"Black"} - Quantity: 1

Thao tác: Tăng số lượng biến thể thứ 2 lên 3
Kết quả mong đợi: 
- Biến thể 1: Quantity vẫn là 8
- Biến thể 2: Quantity tăng lên 3
```

### Test Case 2: Xóa biến thể cụ thể
```
Giỏ hàng có:
1. Swimming Trunks - {"size":"S","color":"Blue"} - Quantity: 8
2. Swimming Trunks - {"size":"M","color":"Black"} - Quantity: 1

Thao tác: Xóa biến thể thứ 2
Kết quả mong đợi: 
- Biến thể 1: Vẫn còn trong giỏ hàng
- Biến thể 2: Đã bị xóa
```

### Test Case 3: Giảm số lượng biến thể
```
Giỏ hàng có:
1. Swimming Trunks - {"size":"S","color":"Blue"} - Quantity: 8
2. Swimming Trunks - {"size":"M","color":"Black"} - Quantity: 3

Thao tác: Giảm số lượng biến thể thứ 2 xuống 1
Kết quả mong đợi: 
- Biến thể 1: Quantity vẫn là 8
- Biến thể 2: Quantity giảm xuống 1
```

## Cách test

1. **Đăng nhập** vào hệ thống
2. **Thêm sản phẩm** "Swimming Trunks" với các biến thể khác nhau vào giỏ hàng
3. **Vào trang giỏ hàng** và thử tăng/giảm số lượng từng biến thể
4. **Kiểm tra** xem chỉ biến thể được chọn thay đổi số lượng

## Endpoints mới

- `PUT /api/Cart/increase-by-product` - Tăng số lượng theo product và variant
- `PUT /api/Cart/decrease-by-product` - Giảm số lượng theo product và variant

## Logging

Hệ thống sẽ log chi tiết:
- Request data với variantAttributes
- Logic tìm kiếm cart item
- Kết quả so sánh variantAttributes
- Success/error messages

## Status: ✅ FIXED

Tất cả các vấn đề đã được sửa và hệ thống sẽ hoạt động chính xác cho việc tăng/giảm số lượng các biến thể khác nhau của cùng một sản phẩm. 