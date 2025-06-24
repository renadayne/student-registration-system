# test_auth.ps1
# Script kiểm tra login nhận JWT và gọi API protected

$baseUrl = "http://localhost:5255"
$username = "student1"
$password = "password123"
$studentId = "11111111-1111-1111-1111-111111111111"
$semesterId = "20240000-0000-0000-0000-000000000000"

Write-Host "[1] Đăng nhập lấy JWT token..." -ForegroundColor Cyan
$response = Invoke-RestMethod -Method POST -Uri "$baseUrl/auth/login" -Body (@{ username=$username; password=$password } | ConvertTo-Json) -ContentType "application/json"
$token = $response.accessToken
if (-not $token) {
    Write-Host "❌ Không lấy được token!" -ForegroundColor Red
    exit 1
}
Write-Host "✅ Đăng nhập thành công. Token: $token" -ForegroundColor Green

Write-Host "[2] Gọi API protected (GET /students/{id}/enrollments)..." -ForegroundColor Cyan
$headers = @{ Authorization = "Bearer $token" }
$enrollments = Invoke-RestMethod -Method GET -Uri "$baseUrl/students/$studentId/enrollments?semesterId=$semesterId" -Headers $headers
Write-Host "✅ Kết quả enrollments:" -ForegroundColor Green
$enrollments | ConvertTo-Json -Depth 5 