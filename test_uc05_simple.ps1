# Test UC05 - Get Student Enrollments
$baseUrl = "http://localhost:5255"
$studentId = "11111111-1111-1111-1111-111111111111"
$semesterId = "20240000-0000-0000-0000-000000000000"

Write-Host "Testing UC05 - Get Student Enrollments" -ForegroundColor Cyan

# Test 1: Get enrollments
$url = "$baseUrl/students/$studentId/enrollments?semesterId=$semesterId"
Write-Host "URL: $url" -ForegroundColor Yellow

try {
    $response = Invoke-RestMethod -Uri $url -Method GET -ContentType "application/json"
    Write-Host "Success: Count = $($response.Count)" -ForegroundColor Green
    
    if ($response.Count -gt 0) {
        foreach ($enrollment in $response) {
            Write-Host "  EnrollmentId: $($enrollment.enrollmentId)" -ForegroundColor White
        }
    }
}
catch {
    Write-Host "Error: $($_.Exception.Message)" -ForegroundColor Red
}

Write-Host "Test completed!" -ForegroundColor Green 