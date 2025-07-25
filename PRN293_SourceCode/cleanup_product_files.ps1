# Cleanup Product Image Files Script
# Xóa các file ảnh sản phẩm trùng lặp trong wwwroot

$productsPath = "EcommerceBackend.API/wwwroot/images/products"

Write-Host "Starting cleanup of product image files..." -ForegroundColor Green

# Kiểm tra thư mục tồn tại
if (!(Test-Path $productsPath)) {
    Write-Host "Products directory not found: $productsPath" -ForegroundColor Red
    exit 1
}

# Lấy danh sách tất cả file ảnh
$imageFiles = Get-ChildItem -Path $productsPath -Filter "*.jpg" -Recurse
$imageFiles += Get-ChildItem -Path $productsPath -Filter "*.jpeg" -Recurse
$imageFiles += Get-ChildItem -Path $productsPath -Filter "*.png" -Recurse
$imageFiles += Get-ChildItem -Path $productsPath -Filter "*.webp" -Recurse

Write-Host "Found $($imageFiles.Count) image files" -ForegroundColor Yellow

# Tạo backup thư mục
$backupPath = "EcommerceBackend.API/wwwroot/images/products_backup_$(Get-Date -Format 'yyyyMMdd_HHmmss')"
Copy-Item -Path $productsPath -Destination $backupPath -Recurse
Write-Host "Backup created at: $backupPath" -ForegroundColor Green

# Xóa các file có tên trùng lặp (giữ lại file đầu tiên)
$fileGroups = $imageFiles | Group-Object Name
$duplicates = $fileGroups | Where-Object { $_.Count -gt 1 }

if ($duplicates.Count -gt 0) {
    Write-Host "Found $($duplicates.Count) duplicate file names:" -ForegroundColor Yellow
    foreach ($group in $duplicates) {
        Write-Host "  - $($group.Name) ($($group.Count) files)" -ForegroundColor Yellow
        # Giữ lại file đầu tiên, xóa các file còn lại
        $filesToDelete = $group.Group | Select-Object -Skip 1
        foreach ($file in $filesToDelete) {
            Write-Host "    Deleting: $($file.FullName)" -ForegroundColor Red
            Remove-Item $file.FullName -Force
        }
    }
} else {
    Write-Host "No duplicate file names found" -ForegroundColor Green
}

# Xóa các file có kích thước 0 hoặc rất nhỏ (có thể bị lỗi)
$smallFiles = $imageFiles | Where-Object { $_.Length -lt 1024 } # Nhỏ hơn 1KB
if ($smallFiles.Count -gt 0) {
    Write-Host "Found $($smallFiles.Count) files smaller than 1KB:" -ForegroundColor Yellow
    foreach ($file in $smallFiles) {
        Write-Host "  Deleting small file: $($file.Name) ($($file.Length) bytes)" -ForegroundColor Red
        Remove-Item $file.FullName -Force
    }
}

# Đếm file còn lại
$remainingFiles = Get-ChildItem -Path $productsPath -Filter "*.jpg" -Recurse
$remainingFiles += Get-ChildItem -Path $productsPath -Filter "*.jpeg" -Recurse
$remainingFiles += Get-ChildItem -Path $productsPath -Filter "*.png" -Recurse
$remainingFiles += Get-ChildItem -Path $productsPath -Filter "*.webp" -Recurse

Write-Host "Cleanup completed!" -ForegroundColor Green
Write-Host "Remaining files: $($remainingFiles.Count)" -ForegroundColor Green
Write-Host "Files removed: $($imageFiles.Count - $remainingFiles.Count)" -ForegroundColor Green
Write-Host "Backup location: $backupPath" -ForegroundColor Cyan 