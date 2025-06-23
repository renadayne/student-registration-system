# Test DELETE endpoint
Write-Host "TEST DELETE ENROLLMENT" -ForegroundColor Green
Write-Host "================================" -ForegroundColor Green

# Enrollment ID từ test trước
$enrollmentId = "1f8aa25b-a522-47e0-8a11-4893feb7ac62"

Write-Host "Test DELETE enrollment: $enrollmentId" -ForegroundColor Cyan

try {
    $response = Invoke-RestMethod -Uri "http://localhost:5255/api/enrollment/$enrollmentId" -Method DELETE
    Write-Host "Huy dang ky thanh cong!" -ForegroundColor Green
} catch {
    Write-Host "Huy dang ky that bai: $($_.Exception.Message)" -ForegroundColor Red
    if ($_.Exception.Response) {
        $errorResponse = $_.Exception.Response.GetResponseStream()
        $reader = New-Object System.IO.StreamReader($errorResponse)
        $errorBody = $reader.ReadToEnd()
        Write-Host "Chi tiet: $errorBody" -ForegroundColor Red
    }
}

Write-Host "HOAN THANH TEST DELETE!" -ForegroundColor Green 