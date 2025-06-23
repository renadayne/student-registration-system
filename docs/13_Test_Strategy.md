# 13. Test Strategy - Chiến lược Testing Toàn diện

## 📋 Tổng quan

Dự án Student Registration System áp dụng **multi-layer testing strategy** để đảm bảo chất lượng code và functionality:

- **Unit Tests**: Test từng component riêng lẻ
- **Integration Tests**: Test tương tác giữa các layer
- **API Tests**: Test end-to-end qua HTTP endpoints
- **Manual Tests**: Test thủ công qua UI tools

---

## 🏗️ Testing Architecture

### 1. Domain Layer Testing
- **Mục tiêu**: Test logic nghiệp vụ thuần túy
- **Scope**: Entity, Exception, Business Rules
- **Approach**: Unit tests không phụ thuộc DB
- **Tools**: xUnit, không cần mock

### 2. Application Layer Testing
- **Mục tiêu**: Test service logic và rule validation
- **Scope**: Rule Checkers, Services, Use Cases
- **Approach**: Unit tests với mock repositories
- **Tools**: xUnit + Moq

### 3. Infrastructure Layer Testing
- **Mục tiêu**: Test data access và persistence
- **Scope**: Repositories (InMemory, SQLite)
- **Approach**: Integration tests với test database
- **Tools**: xUnit + SQLite in-memory

### 4. API Layer Testing
- **Mục tiêu**: Test HTTP endpoints end-to-end
- **Scope**: Controllers, Middleware, DTOs
- **Approach**: Integration tests qua HTTP
- **Tools**: PowerShell scripts, Postman, curl

---

## 📁 Test Structure

```
tests/
├── StudentRegistration.Application.Tests/
│   └── Services/
│       ├── ClassSectionSlotRuleCheckerTests.cs
│       ├── DropDeadlineRuleCheckerTests.cs
│       ├── EnrollmentRuleCheckerTests.cs
│       ├── MandatoryCourseRuleCheckerTests.cs
│       ├── MaxEnrollmentRuleCheckerTests.cs
│       ├── PrerequisiteRuleCheckerTests.cs
│       └── ScheduleConflictRuleTests.cs
└── StudentRegistration.Infrastructure.Tests/
    └── Repositories/
        ├── InMemoryClassSectionRepositoryTests.cs
        ├── InMemoryCourseRepositoryTests.cs
        ├── InMemoryStudentRecordRepositoryTests.cs
        └── SQLiteEnrollmentRepositoryTests.cs
```

---

## 🧪 Unit Testing Strategy

### Naming Convention
- **File**: `[ClassName]Tests.cs`
- **Class**: `[ClassName]Tests`
- **Method**: `[Scenario]_[ExpectedResult]`

### Test Categories
1. **Happy Path Tests**: Test success scenarios
2. **Edge Case Tests**: Test boundary conditions
3. **Error Case Tests**: Test exception handling
4. **Business Rule Tests**: Test BR01-BR04 validation

### Example: Business Rule Test
```csharp
[Fact]
public void WhenStudentEnrollsMoreThan7Courses_ShouldThrowMaxEnrollmentExceededException()
{
    // Arrange
    var mockRepo = new Mock<IEnrollmentRepository>();
    mockRepo.Setup(r => r.GetEnrollmentsByStudentAndSemester(It.IsAny<Guid>(), It.IsAny<Guid>()))
            .Returns(Enumerable.Range(0, 7).Select(i => new Enrollment()).ToList());
    
    var checker = new MaxEnrollmentRuleChecker(mockRepo.Object);
    
    // Act & Assert
    Assert.ThrowsAsync<MaxEnrollmentExceededException>(() => 
        checker.CheckAsync(Guid.NewGuid(), Guid.NewGuid()));
}
```

---

## 🔗 Integration Testing Strategy

### Repository Integration Tests
- **SQLite In-Memory**: Fast, isolated tests
- **Test Data**: Consistent GUIDs for reproducibility
- **Cleanup**: Reset database state after each test

### API Integration Tests
- **PowerShell Scripts**: Automated HTTP testing
- **Test Scenarios**: Complete workflows
- **Error Handling**: Test all HTTP status codes

---

## 📊 Test Coverage Goals

### Business Rules Coverage
- ✅ **BR01**: Max 7 enrollments per semester
- ✅ **BR02**: No schedule conflicts
- ✅ **BR03**: Prerequisites must be met
- ✅ **BR04**: Class section capacity check

### API Endpoints Coverage
- ✅ **UC03**: POST /api/enrollment
- ✅ **UC04**: DELETE /api/enrollment/{id}
- ✅ **UC05**: GET /students/{id}/enrollments

### Repository Coverage
- ✅ **InMemory**: Fast unit testing
- ✅ **SQLite**: Real database testing

---

## 🚀 Test Execution

### Unit Tests
```bash
# Run all unit tests
dotnet test

# Run specific project
dotnet test tests/StudentRegistration.Application.Tests/

# Run with coverage
dotnet test --collect:"XPlat Code Coverage"
```

### Integration Tests
```powershell
# Run API tests
powershell -ExecutionPolicy Bypass -File test_complete.ps1

# Run specific endpoint test
powershell -ExecutionPolicy Bypass -File test_uc05_simple.ps1
```

### Manual Tests
- **Swagger UI**: `http://localhost:5255`
- **Postman**: Import collection từ docs
- **curl**: Command line testing

---

## 📈 Test Metrics

### Coverage Targets
- **Unit Tests**: >90% code coverage
- **Integration Tests**: 100% endpoint coverage
- **Business Rules**: 100% rule validation coverage

### Quality Gates
- ✅ All tests must pass
- ✅ No build warnings
- ✅ API response time < 500ms
- ✅ Error handling tested

---

## 🔧 Test Data Management

### Consistent Test Data
```csharp
// Fixed GUIDs for reproducible tests
public static class TestData
{
    public static readonly Guid StudentId = Guid.Parse("11111111-1111-1111-1111-111111111111");
    public static readonly Guid ClassSectionId = Guid.Parse("22222222-2222-2222-2222-222222222222");
    public static readonly Guid SemesterId = Guid.Parse("20240000-0000-0000-0000-000000000000");
}
```

### Test Isolation
- Each test runs independently
- Database state reset after each test
- No shared state between tests

---

## 🐛 Debugging Tests

### Common Issues
1. **API not running**: Start API before running integration tests
2. **File locked**: Stop API before building
3. **Port conflict**: Check if port 5255 is available
4. **GUID format**: Ensure valid GUID format in test data

### Debug Commands
```bash
# Check API status
curl http://localhost:5255/health

# View test output
dotnet test --logger "console;verbosity=detailed"

# Debug specific test
dotnet test --filter "FullyQualifiedName~TestName"
```

---

## 📚 Tài liệu liên quan

- [TestingGuide.md](api/TestingGuide.md) - Comprehensive testing guide
- [EnrollmentApiGuide.md](api/EnrollmentApiGuide.md) - API testing details
- [PostmanTestingGuide.md](api/PostmanTestingGuide.md) - Manual testing guide
- [12_Rule_Validation_Framework.md](12_Rule_Validation_Framework.md) - Business rules framework

---

## 🎯 Best Practices

### Writing Tests
1. **Arrange-Act-Assert**: Clear test structure
2. **Descriptive names**: Test names explain the scenario
3. **Single responsibility**: One assertion per test
4. **Mock external dependencies**: Isolate unit under test

### Maintaining Tests
1. **Update tests first**: Write tests before implementing features
2. **Refactor tests**: Keep tests clean and maintainable
3. **Document test data**: Explain test scenarios and expected results
4. **Regular review**: Review test coverage and quality

### Continuous Integration
1. **Automated testing**: Run tests on every commit
2. **Test reporting**: Generate and review test reports
3. **Quality gates**: Block deployment if tests fail
4. **Performance monitoring**: Track test execution time

---

## 🔮 Future Enhancements

### Planned Improvements
- **Performance Tests**: Load testing for API endpoints
- **Security Tests**: Authentication and authorization testing
- **Database Tests**: Migration and schema testing
- **UI Tests**: Frontend testing (when implemented)

### Monitoring
- **Test Metrics**: Track test execution time and success rate
- **Coverage Reports**: Monitor code coverage trends
- **Failure Analysis**: Analyze and fix flaky tests
- **Documentation**: Keep testing guides up to date 