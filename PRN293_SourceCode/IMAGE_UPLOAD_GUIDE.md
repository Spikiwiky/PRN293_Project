# Hướng Dẫn Upload Ảnh Thống Nhất

## Tổng Quan
Hệ thống đã được cập nhật để chỉ sử dụng **1 wwwroot duy nhất** cho việc lưu trữ ảnh:
- **Backend API:** `EcommerceBackend.API/wwwroot/images/products/`
- **Frontend:** Chỉ gọi API để upload ảnh, không lưu file trực tiếp

## Cấu Trúc Thư Mục

```
EcommerceBackend.API/wwwroot/images/
├── products/                    # Ảnh sản phẩm (duy nhất)
│   ├── {GUID}_{originalName}.jpg
│   ├── {GUID}_{originalName}.webp
│   └── ...
└── blogs/                       # Ảnh blog (nếu cần)
    └── ...
```

## API Endpoints

### Upload Ảnh Sản Phẩm
```http
POST /api/product/upload-image
Content-Type: multipart/form-data

Form Data:
- imageFile: File ảnh
- productId: ID sản phẩm
```

**Response:**
```json
{
  "imageUrl": "/images/products/{GUID}_{filename}"
}
```

### Thêm URL Ảnh Vào Database
```http
POST /api/product/{productId}/images
Content-Type: application/json

{
  "imageUrl": "/images/products/{filename}"
}
```

## Cách Sử Dụng Trong Frontend

### 1. Upload Ảnh Sản Phẩm (Edit.cshtml.cs)
```csharp
// Xử lý upload ảnh nếu có file
if (imageFile != null && imageFile.Length > 0)
{
    // Gọi API để upload ảnh thay vì lưu file trực tiếp
    var imageUrl = await _productService.UploadProductImageAsync(id, imageFile);
    if (!string.IsNullOrEmpty(imageUrl))
    {
        // Ảnh đã được upload thành công, URL đã được trả về từ API
        // Không cần gọi AddProductImageAsync vì API đã tự động lưu vào DB
    }
    else
    {
        Message = "Upload ảnh thất bại!";
        return Page();
    }
}
```

### 2. Service Method (ProductService.cs)
```csharp
public async Task<string?> UploadProductImageAsync(int productId, IFormFile imageFile)
{
    try
    {
        using var formData = new MultipartFormDataContent();
        using var streamContent = new StreamContent(imageFile.OpenReadStream());
        formData.Add(streamContent, "imageFile", imageFile.FileName);
        formData.Add(new StringContent(productId.ToString()), "productId");

        var response = await _httpClient.PostAsync("/api/product/upload-image", formData);
        response.EnsureSuccessStatusCode();
        
        var responseContent = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<UploadImageResponse>(responseContent, _jsonOptions);
        return result?.ImageUrl;
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error uploading product image");
        return null;
    }
}
```

## Backend Implementation

### ProductController.cs
```csharp
[HttpPost("upload-image")]
public async Task<IActionResult> UploadProductImage([FromForm] IFormFile imageFile, [FromForm] int productId)
{
    if (imageFile == null || imageFile.Length == 0)
        return BadRequest("No file uploaded");

    // Tạo thư mục nếu chưa có
    var uploadPath = Path.Combine(_env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"), "images", "products");
    if (!Directory.Exists(uploadPath))
        Directory.CreateDirectory(uploadPath);

    // Đặt tên file duy nhất
    var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(imageFile.FileName)}";
    var filePath = Path.Combine(uploadPath, fileName);

    // Lưu file
    using (var stream = new FileStream(filePath, FileMode.Create))
    {
        await imageFile.CopyToAsync(stream);
    }

    // Tạo URL ảnh
    var imageUrl = $"/images/products/{fileName}";

    // Lưu vào DB (gọi service)
    await _productService.AddProductImageAsync(productId, imageUrl);

    return Ok(new { imageUrl });
}
```

## Lợi Ích Của Hệ Thống Thống Nhất

### ✅ Ưu Điểm
1. **Không trùng lặp:** Chỉ 1 bản copy của mỗi ảnh
2. **Tiết kiệm dung lượng:** Không lưu trữ file ở nhiều nơi
3. **Dễ quản lý:** Tất cả ảnh tập trung ở 1 nơi
4. **Tính nhất quán:** URL ảnh luôn chính xác
5. **Bảo mật:** Chỉ Backend có quyền lưu file

### 🔧 Bảo Trì
1. **Backup:** Tự động backup trước khi dọn dẹp
2. **Dọn dẹp:** Script tự động xóa file trùng lặp
3. **Monitoring:** Log đầy đủ quá trình upload

## Scripts Dọn Dẹp

### 1. Dọn Dẹp Database
```sql
-- Chạy file: cleanup_product_images.sql
-- Xóa ảnh trùng lặp trong database
```

### 2. Dọn Dẹp Files
```powershell
# Chạy file: cleanup_product_files.ps1
# Xóa file ảnh trùng lặp trong wwwroot
```

## Lưu Ý Quan Trọng

1. **Không lưu file trực tiếp ở Frontend** - Luôn gọi API
2. **Backup trước khi dọn dẹp** - Đảm bảo an toàn dữ liệu
3. **Kiểm tra URL ảnh** - Đảm bảo đường dẫn chính xác
4. **Xử lý lỗi** - Luôn có fallback khi upload thất bại

## Troubleshooting

### Lỗi Upload Thất Bại
1. Kiểm tra quyền ghi thư mục `wwwroot/images/products/`
2. Kiểm tra kết nối API
3. Kiểm tra kích thước file (nên giới hạn < 10MB)

### Ảnh Không Hiển Thị
1. Kiểm tra URL ảnh trong database
2. Kiểm tra file có tồn tại trong thư mục
3. Kiểm tra cấu hình static files trong Program.cs 