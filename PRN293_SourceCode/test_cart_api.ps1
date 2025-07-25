# Test script for Cart API
param(
    [string]$BaseUrl = "https://localhost:7001",
    [string]$UserId = "1"
)

Write-Host "Testing Cart API at: $BaseUrl" -ForegroundColor Green

# Test 1: Debug Cart
Write-Host "`n=== Test 1: Debug Cart ===" -ForegroundColor Yellow
$debugUrl = "$BaseUrl/api/Cart/debug-cart"
$headers = @{
    "Cookie" = "UserId=$UserId"
    "Content-Type" = "application/json"
}

try {
    $response = Invoke-RestMethod -Uri $debugUrl -Method GET -Headers $headers
    Write-Host "Debug Cart Response:" -ForegroundColor Cyan
    $response | ConvertTo-Json -Depth 3
} catch {
    Write-Host "Error in Debug Cart: $($_.Exception.Message)" -ForegroundColor Red
}

# Test 2: Test Update (if cart has items)
Write-Host "`n=== Test 2: Test Update ===" -ForegroundColor Yellow
$testUpdateUrl = "$BaseUrl/api/Cart/test-update"
$testData = @{
    productId = 1
    variantId = ""
    quantity = 2
} | ConvertTo-Json

try {
    $response = Invoke-RestMethod -Uri $testUpdateUrl -Method POST -Headers $headers -Body $testData
    Write-Host "Test Update Response:" -ForegroundColor Cyan
    $response | ConvertTo-Json -Depth 3
} catch {
    Write-Host "Error in Test Update: $($_.Exception.Message)" -ForegroundColor Red
}

# Test 3: Regular Update
Write-Host "`n=== Test 3: Regular Update ===" -ForegroundColor Yellow
$updateUrl = "$BaseUrl/api/Cart/update"
$updateData = @{
    productId = 1
    variantId = ""
    quantity = 3
} | ConvertTo-Json

try {
    $response = Invoke-RestMethod -Uri $updateUrl -Method POST -Headers $headers -Body $updateData
    Write-Host "Regular Update Response:" -ForegroundColor Cyan
    $response | ConvertTo-Json -Depth 3
} catch {
    Write-Host "Error in Regular Update: $($_.Exception.Message)" -ForegroundColor Red
}

Write-Host "`nTesting completed!" -ForegroundColor Green 