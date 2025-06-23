# Test script cho endpoint GET /students/{studentId}/enrollments (UC05)
# Mục tiêu: Xem danh sách học phần đã đăng ký của sinh viên trong một học kỳ

$baseUrl = "http://localhost:5255"
$studentId = "11111111-1111-1111-1111-111111111111"
$semesterId = "20240000-0000-0000-0000-000000000000"

Write-Host "🧪 Testing UC05 - Get Student Enrollments" -ForegroundColor Cyan
Write-Host "================================================" -ForegroundColor Cyan

# Test 1: Lấy danh sách enrollment của sinh viên
Write-Host "`n📋 Test 1: Lấy danh sách enrollment của sinh viên" -ForegroundColor Yellow
$url = "$baseUrl/students/$studentId/enrollments?semesterId=$semesterId"

try {
    $response = Invoke-RestMethod -Uri $url -Method GET -ContentType "application/json"
    
    Write-Host "✅ Success: Lấy danh sách enrollment thành công" -ForegroundColor Green
    Write-Host "📊 Số lượng enrollment: $($response.Count)" -ForegroundColor Green
    
    if ($response.Count -gt 0) {
        Write-Host "`n📝 Danh sách enrollment:" -ForegroundColor Cyan
        foreach ($enrollment in $response) {
            Write-Host "  - EnrollmentId: $($enrollment.enrollmentId)" -ForegroundColor White
            Write-Host "    CourseId: $($enrollment.courseId)" -ForegroundColor Gray
            Write-Host "    ClassSectionId: $($enrollment.classSectionId)" -ForegroundColor Gray
            Write-Host "    SemesterId: $($enrollment.semesterId)" -ForegroundColor Gray
            Write-Host "    EnrollmentDate: $($enrollment.enrollmentDate)" -ForegroundColor Gray
            Write-Host ""
        }
    } else {
        Write-Host "ℹ️  Sinh viên chưa đăng ký môn học nào trong học kỳ này" -ForegroundColor Yellow
    }
}
catch {
    Write-Host "❌ Error: $($_.Exception.Message)" -ForegroundColor Red
    if ($_.Exception.Response) {
        $statusCode = $_.Exception.Response.StatusCode
        Write-Host "Status Code: $statusCode" -ForegroundColor Red
    }
}

# Test 2: Test với semesterId không hợp lệ
Write-Host "`n📋 Test 2: Test với semesterId không hợp lệ" -ForegroundColor Yellow
$invalidSemesterId = "invalid-semester-id"
$url = "$baseUrl/students/$studentId/enrollments?semesterId=$invalidSemesterId"

try {
    $response = Invoke-RestMethod -Uri $url -Method GET -ContentType "application/json"
    Write-Host "❌ Unexpected success with invalid semesterId" -ForegroundColor Red
}
catch {
    Write-Host "✅ Expected error with invalid semesterId" -ForegroundColor Green
    Write-Host "Error message: $($_.Exception.Message)" -ForegroundColor Gray
}

# Test 3: Test với semesterId rỗng
Write-Host "`n📋 Test 3: Test với semesterId rỗng" -ForegroundColor Yellow
$url = "$baseUrl/students/$studentId/enrollments"

try {
    $response = Invoke-RestMethod -Uri $url -Method GET -ContentType "application/json"
    Write-Host "❌ Unexpected success with empty semesterId" -ForegroundColor Red
}
catch {
    Write-Host "✅ Expected error with empty semesterId" -ForegroundColor Green
    Write-Host "Error message: $($_.Exception.Message)" -ForegroundColor Gray
}

# Test 4: Test với studentId không tồn tại
Write-Host "`n📋 Test 4: Test với studentId không tồn tại" -ForegroundColor Yellow
$nonExistentStudentId = "99999999-9999-9999-9999-999999999999"
$url = "$baseUrl/students/$nonExistentStudentId/enrollments?semesterId=$semesterId"

try {
    $response = Invoke-RestMethod -Uri $url -Method GET -ContentType "application/json"
    
    Write-Host "✅ Success: Lấy danh sách enrollment cho student không tồn tại" -ForegroundColor Green
    Write-Host "📊 Số lượng enrollment: $($response.Count)" -ForegroundColor Green
    
    if ($response.Count -eq 0) {
        Write-Host "ℹ️  Trả về danh sách rỗng cho student không tồn tại (đúng behavior)" -ForegroundColor Yellow
    }
}
catch {
    Write-Host "❌ Error: $($_.Exception.Message)" -ForegroundColor Red
}

Write-Host "`n🎉 Test UC05 hoàn thành!" -ForegroundColor Green
Write-Host "================================================" -ForegroundColor Cyan 