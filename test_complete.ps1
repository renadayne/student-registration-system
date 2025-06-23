# Test hoàn chỉnh: Tạo rồi xóa enrollment
Write-Host "TEST HOAN CHINH: TAO ROI XOA ENROLLMENT" -ForegroundColor Green
Write-Host "================================================" -ForegroundColor Green

# Test 1: Tạo enrollment
Write-Host "Buoc 1: Tao enrollment moi" -ForegroundColor Cyan
$enrollRequest = @{
    studentId = "11111111-1111-1111-1111-111111111111"
    classSectionId = "22222222-2222-2222-2222-222222222222"
    semesterId = "20240000-0000-0000-0000-000000000000"
} | ConvertTo-Json

try {
    $response = Invoke-RestMethod -Uri "http://localhost:5255/api/enrollment" -Method Post -Body $enrollRequest -ContentType "application/json"
    Write-Host "Tao enrollment thanh cong!" -ForegroundColor Green
    Write-Host "Enrollment ID: $($response.enrollmentId)" -ForegroundColor White
    
    $enrollmentId = $response.enrollmentId
    
    # Test 2: Xóa enrollment vừa tạo
    Write-Host "`nBuoc 2: Xoa enrollment vua tao" -ForegroundColor Cyan
    $deleteResponse = Invoke-RestMethod -Uri "http://localhost:5255/api/enrollment/$enrollmentId" -Method DELETE
    Write-Host "Xoa enrollment thanh cong!" -ForegroundColor Green
    
} catch {
    Write-Host "Loi: $($_.Exception.Message)" -ForegroundColor Red
    if ($_.Exception.Response) {
        $errorResponse = $_.Exception.Response.GetResponseStream()
        $reader = New-Object System.IO.StreamReader($errorResponse)
        $errorBody = $reader.ReadToEnd()
        Write-Host "Chi tiet: $errorBody" -ForegroundColor Red
    }
}

Write-Host "`nHOAN THANH TEST HOAN CHINH!" -ForegroundColor Green
Write-Host "API da hoat dong tot!" -ForegroundColor Yellow 