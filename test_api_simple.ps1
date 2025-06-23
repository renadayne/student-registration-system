# Test API Script cho Student Registration System
Write-Host "TESTING STUDENT REGISTRATION API" -ForegroundColor Green
Write-Host "=====================================" -ForegroundColor Green

# Doi API khoi dong
Write-Host "Doi API khoi dong..." -ForegroundColor Yellow
Start-Sleep -Seconds 3

# Test 1: Kiem tra Swagger UI
Write-Host "Test 1: Kiem tra Swagger UI" -ForegroundColor Cyan
try {
    $response = Invoke-RestMethod -Uri "http://localhost:5255/swagger/v1/swagger.json" -Method Get
    Write-Host "Swagger UI hoat dong" -ForegroundColor Green
} catch {
    Write-Host "Khong the ket noi API: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host "Hay dam bao API dang chay: dotnet run --project src/StudentRegistration.Api" -ForegroundColor Yellow
    exit 1
}

# Test 2: Dang ky mon hoc (UC03)
Write-Host "Test 2: Dang ky mon hoc (UC03)" -ForegroundColor Cyan
$enrollRequest = @{
    studentId = "11111111-1111-1111-1111-111111111111"
    classSectionId = "22222222-2222-2222-2222-222222222222"
    semesterId = "20240000-0000-0000-0000-000000000000"
} | ConvertTo-Json

try {
    $response = Invoke-RestMethod -Uri "http://localhost:5255/api/enrollment" -Method Post -Body $enrollRequest -ContentType "application/json"
    Write-Host "Dang ky thanh cong!" -ForegroundColor Green
    Write-Host "Enrollment ID: $($response.enrollmentId)" -ForegroundColor White
    $enrollmentId = $response.enrollmentId
} catch {
    Write-Host "Dang ky that bai: $($_.Exception.Message)" -ForegroundColor Red
    $enrollmentId = "test-enrollment-id"
}

Write-Host "HOAN THANH TEST API!" -ForegroundColor Green
Write-Host "Swagger UI: http://localhost:5255" -ForegroundColor Yellow 