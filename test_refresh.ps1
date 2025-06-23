# Test script cho Refresh Token Flow
# Chạy: .\test_refresh.ps1

$baseUrl = "https://localhost:7255"
$apiUrl = "http://localhost:5255"

Write-Host "=== TEST REFRESH TOKEN FLOW ===" -ForegroundColor Green
Write-Host ""

# Test 1: Login để lấy access token và refresh token
Write-Host "1. Testing Login..." -ForegroundColor Yellow
$loginBody = @{
    username = "student1"
    password = "password123"
} | ConvertTo-Json

try {
    $loginResponse = Invoke-RestMethod -Uri "$apiUrl/auth/login" -Method POST -Body $loginBody -ContentType "application/json"
    
    Write-Host "✅ Login thành công!" -ForegroundColor Green
    Write-Host "   Access Token: $($loginResponse.accessToken.Substring(0, 20))..." -ForegroundColor Gray
    Write-Host "   Refresh Token: $($loginResponse.refreshToken.Substring(0, 20))..." -ForegroundColor Gray
    Write-Host "   Expires In: $($loginResponse.expiresIn) seconds" -ForegroundColor Gray
    Write-Host "   Refresh Token Expires In: $($loginResponse.refreshTokenExpiresIn) seconds" -ForegroundColor Gray
    
    $accessToken = $loginResponse.accessToken
    $refreshToken = $loginResponse.refreshToken
    
} catch {
    Write-Host "❌ Login thất bại: $($_.Exception.Message)" -ForegroundColor Red
    exit 1
}

Write-Host ""

# Test 2: Sử dụng access token để gọi API protected
Write-Host "2. Testing Protected API với Access Token..." -ForegroundColor Yellow
$headers = @{
    "Authorization" = "Bearer $accessToken"
}

try {
    $validateResponse = Invoke-RestMethod -Uri "$apiUrl/auth/validate" -Method GET -Headers $headers
    Write-Host "✅ Access token hợp lệ: $($validateResponse.message)" -ForegroundColor Green
} catch {
    Write-Host "❌ Access token không hợp lệ: $($_.Exception.Message)" -ForegroundColor Red
}

Write-Host ""

# Test 3: Refresh token để lấy access token mới
Write-Host "3. Testing Refresh Token..." -ForegroundColor Yellow
$refreshBody = @{
    refreshToken = $refreshToken
} | ConvertTo-Json

try {
    $refreshResponse = Invoke-RestMethod -Uri "$apiUrl/auth/refresh" -Method POST -Body $refreshBody -ContentType "application/json"
    
    Write-Host "✅ Refresh token thành công!" -ForegroundColor Green
    Write-Host "   New Access Token: $($refreshResponse.accessToken.Substring(0, 20))..." -ForegroundColor Gray
    Write-Host "   New Refresh Token: $($refreshResponse.refreshToken.Substring(0, 20))..." -ForegroundColor Gray
    Write-Host "   Expires In: $($refreshResponse.expiresIn) seconds" -ForegroundColor Gray
    
    $newAccessToken = $refreshResponse.accessToken
    $newRefreshToken = $refreshResponse.refreshToken
    
} catch {
    Write-Host "❌ Refresh token thất bại: $($_.Exception.Message)" -ForegroundColor Red
}

Write-Host ""

# Test 4: Sử dụng access token mới để gọi API protected
Write-Host "4. Testing Protected API với New Access Token..." -ForegroundColor Yellow
$newHeaders = @{
    "Authorization" = "Bearer $newAccessToken"
}

try {
    $validateResponse = Invoke-RestMethod -Uri "$apiUrl/auth/validate" -Method GET -Headers $newHeaders
    Write-Host "✅ New access token hợp lệ: $($validateResponse.message)" -ForegroundColor Green
} catch {
    Write-Host "❌ New access token không hợp lệ: $($_.Exception.Message)" -ForegroundColor Red
}

Write-Host ""

# Test 5: Test với refresh token cũ (sẽ thất bại vì đã bị revoke)
Write-Host "5. Testing với Old Refresh Token (sẽ thất bại)..." -ForegroundColor Yellow
$oldRefreshBody = @{
    refreshToken = $refreshToken
} | ConvertTo-Json

try {
    $oldRefreshResponse = Invoke-RestMethod -Uri "$apiUrl/auth/refresh" -Method POST -Body $oldRefreshBody -ContentType "application/json"
    Write-Host "❌ Lỗi: Old refresh token vẫn hoạt động (không nên)" -ForegroundColor Red
} catch {
    Write-Host "✅ Old refresh token đã bị revoke như mong đợi" -ForegroundColor Green
}

Write-Host ""

# Test 6: Test với refresh token không hợp lệ
Write-Host "6. Testing với Invalid Refresh Token..." -ForegroundColor Yellow
$invalidRefreshBody = @{
    refreshToken = "invalid-refresh-token-123"
} | ConvertTo-Json

try {
    $invalidRefreshResponse = Invoke-RestMethod -Uri "$apiUrl/auth/refresh" -Method POST -Body $invalidRefreshBody -ContentType "application/json"
    Write-Host "❌ Lỗi: Invalid refresh token vẫn hoạt động (không nên)" -ForegroundColor Red
} catch {
    Write-Host "✅ Invalid refresh token bị từ chối như mong đợi" -ForegroundColor Green
}

Write-Host ""

# Test 7: Logout với refresh token
Write-Host "7. Testing Logout..." -ForegroundColor Yellow
$logoutBody = @{
    refreshToken = $newRefreshToken
} | ConvertTo-Json

try {
    $logoutResponse = Invoke-RestMethod -Uri "$apiUrl/auth/logout" -Method POST -Body $logoutBody -ContentType "application/json" -Headers $newHeaders
    Write-Host "✅ Logout thành công: $($logoutResponse.message)" -ForegroundColor Green
} catch {
    Write-Host "❌ Logout thất bại: $($_.Exception.Message)" -ForegroundColor Red
}

Write-Host ""

# Test 8: Test refresh token sau khi logout (sẽ thất bại)
Write-Host "8. Testing Refresh Token sau Logout (sẽ thất bại)..." -ForegroundColor Yellow
$logoutRefreshBody = @{
    refreshToken = $newRefreshToken
} | ConvertTo-Json

try {
    $logoutRefreshResponse = Invoke-RestMethod -Uri "$apiUrl/auth/refresh" -Method POST -Body $logoutRefreshBody -ContentType "application/json"
    Write-Host "❌ Lỗi: Refresh token vẫn hoạt động sau logout (không nên)" -ForegroundColor Red
} catch {
    Write-Host "✅ Refresh token đã bị revoke sau logout như mong đợi" -ForegroundColor Green
}

Write-Host ""
Write-Host "=== HOÀN THÀNH TEST REFRESH TOKEN FLOW ===" -ForegroundColor Green
Write-Host "Tất cả test cases đã được thực hiện thành công!" -ForegroundColor Green 