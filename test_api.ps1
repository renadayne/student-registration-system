# Test API Script cho Student Registration System
Write-Host "🧪 TESTING STUDENT REGISTRATION API" -ForegroundColor Green
Write-Host "=====================================" -ForegroundColor Green

# Đợi API khởi động
Write-Host "⏳ Đợi API khởi động..." -ForegroundColor Yellow
Start-Sleep -Seconds 3

# Test 1: Kiểm tra Swagger UI
Write-Host "`n📋 Test 1: Kiểm tra Swagger UI" -ForegroundColor Cyan
try {
    $response = Invoke-RestMethod -Uri "http://localhost:5000/swagger/v1/swagger.json" -Method Get
    Write-Host "✅ Swagger UI hoạt động" -ForegroundColor Green
} catch {
    Write-Host "❌ Không thể kết nối API: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host "💡 Hãy đảm bảo API đang chạy: dotnet run --project src/StudentRegistration.Api" -ForegroundColor Yellow
    exit 1
}

# Test 2: Đăng ký môn học (UC03)
Write-Host "`n📋 Test 2: Đăng ký môn học (UC03)" -ForegroundColor Cyan
$enrollRequest = @{
    studentId = "11111111-1111-1111-1111-111111111111"
    classSectionId = "22222222-2222-2222-2222-222222222222"
    semesterId = "20240000-0000-0000-0000-000000000000"
} | ConvertTo-Json

try {
    $response = Invoke-RestMethod -Uri "http://localhost:5000/api/enrollment" -Method Post -Body $enrollRequest -ContentType "application/json"
    Write-Host "✅ Đăng ký thành công!" -ForegroundColor Green
    Write-Host "   Enrollment ID: $($response.enrollmentId)" -ForegroundColor White
    Write-Host "   Student ID: $($response.studentId)" -ForegroundColor White
    Write-Host "   Class Section ID: $($response.classSectionId)" -ForegroundColor White
    
    $enrollmentId = $response.enrollmentId
} catch {
    Write-Host "❌ Đăng ký thất bại: $($_.Exception.Message)" -ForegroundColor Red
    if ($_.Exception.Response) {
        $errorResponse = $_.Exception.Response.GetResponseStream()
        $reader = New-Object System.IO.StreamReader($errorResponse)
        $errorBody = $reader.ReadToEnd()
        Write-Host "   Chi tiết: $errorBody" -ForegroundColor Red
    }
    $enrollmentId = "test-enrollment-id"
}

# Test 3: Lấy thông tin enrollment
Write-Host "`n📋 Test 3: Lấy thông tin enrollment" -ForegroundColor Cyan
try {
    $response = Invoke-RestMethod -Uri "http://localhost:5000/api/enrollment/$enrollmentId" -Method Get
    Write-Host "✅ Lấy thông tin thành công!" -ForegroundColor Green
    Write-Host "   Enrollment ID: $($response.enrollmentId)" -ForegroundColor White
    Write-Host "   Enrollment Date: $($response.enrollmentDate)" -ForegroundColor White
} catch {
    Write-Host "❌ Không thể lấy thông tin enrollment: $($_.Exception.Message)" -ForegroundColor Red
}

# Test 4: Hủy đăng ký môn học (UC04)
Write-Host "`n📋 Test 4: Hủy đăng ký môn học (UC04)" -ForegroundColor Cyan
try {
    $response = Invoke-RestMethod -Uri "http://localhost:5000/api/enrollment/$enrollmentId" -Method Delete
    Write-Host "✅ Hủy đăng ký thành công!" -ForegroundColor Green
} catch {
    Write-Host "❌ Hủy đăng ký thất bại: $($_.Exception.Message)" -ForegroundColor Red
    if ($_.Exception.Response) {
        $errorResponse = $_.Exception.Response.GetResponseStream()
        $reader = New-Object System.IO.StreamReader($errorResponse)
        $errorBody = $reader.ReadToEnd()
        Write-Host "   Chi tiết: $errorBody" -ForegroundColor Red
    }
}

Write-Host "`n🎉 HOÀN THÀNH TEST API!" -ForegroundColor Green
Write-Host "=====================================" -ForegroundColor Green
Write-Host "📖 Swagger UI: http://localhost:5000" -ForegroundColor Yellow
Write-Host "📖 API Docs: http://localhost:5000/swagger" -ForegroundColor Yellow 