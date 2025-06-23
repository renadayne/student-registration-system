# 🚀 Hướng dẫn sử dụng Postman test Web API - Student Registration System

## 📋 Mục lục
- [1. Giới thiệu](#1-giới-thiệu)
- [2. Yêu cầu trước khi bắt đầu](#2-yêu-cầu-trước-khi-bắt-đầu)
- [3. Cấu hình môi trường trong Postman](#3-cấu-hình-môi-trường-trong-postman)
- [4. Tạo Request: POST /api/enrollment](#4-tạo-request-post-apienrollment)
- [5. Tạo Request: DELETE /api/enrollment/{id}](#5-tạo-request-delete-apienrollmentid)
- [6. Tạo Request: GET /api/enrollment/{id}](#6-tạo-request-get-apienrollmentid)
- [7. Lưu vào Collection](#7-lưu-vào-collection)
- [8. Xuất Collection (cho chia sẻ)](#8-xuất-collection-cho-chia-sẻ)
- [9. Test tự động với Tests tab](#9-test-tự-động-với-tests-tab)
- [10. Gợi ý nâng cao](#10-gợi-ý-nâng-cao)

---

## 1. Giới thiệu

Tài liệu này hướng dẫn sử dụng **Postman** để test 3 endpoint quan trọng của Student Registration System:

| Endpoint | Method | Mô tả | Use Case |
|----------|--------|-------|----------|
| `/api/enrollment` | `POST` | Đăng ký môn học | UC03 |
| `/api/enrollment/{id}` | `DELETE` | Hủy đăng ký môn học | UC04 |
| `/api/enrollment/{id}` | `GET` | Lấy thông tin enrollment | - |

### 🎯 Lợi ích sử dụng Postman:
- ✅ **Gửi request nhanh** - Không cần viết code
- ✅ **Xem response dễ dàng** - Format JSON đẹp
- ✅ **Lưu request** - Tái sử dụng nhiều lần
- ✅ **Test tự động** - Assert kết quả
- ✅ **Chia sẻ team** - Export/Import collection

---

## 2. Yêu cầu trước khi bắt đầu

### 📋 Chuẩn bị
- ✅ **Cài đặt Postman**: [Download Postman](https://www.postman.com/downloads/)
- ✅ **Server đang chạy**: Web API phải hoạt động
- ✅ **Kiến thức cơ bản**: HTTP methods, JSON format

### 🚀 Khởi động Web API
```bash
# Từ thư mục gốc dự án
dotnet run --project src/StudentRegistration.Api

# Hoặc từ thư mục API
cd src/StudentRegistration.Api
dotnet run
```

### 🌐 URL mặc định
- **Base URL**: `http://localhost:5255`
- **Swagger UI**: `http://localhost:5255`
- **Health Check**: `http://localhost:5255/health`

### 📝 Test Data sẵn có
```json
{
  "studentId": "11111111-1111-1111-1111-111111111111",
  "classSectionId": "22222222-2222-2222-2222-222222222222", 
  "semesterId": "20240000-0000-0000-0000-000000000000"
}
```

---

## 3. Cấu hình môi trường trong Postman

### 3.1 Tạo Environment "StudentAPI"

1. **Mở Postman** → Click **"Environments"** → **"New"**
2. **Đặt tên**: `StudentAPI`
3. **Thêm biến**:

| Variable | Initial Value | Current Value |
|----------|---------------|---------------|
| `baseUrl` | `http://localhost:5255` | `http://localhost:5255` |
| `enrollmentId` | (để trống) | (sẽ được set sau) |

4. **Bấm "Save"**
5. **Chọn environment**: Ở góc trên bên phải, chọn `StudentAPI`

### 3.2 Cấu hình Global Headers (tùy chọn)

1. **Settings** → **General** → **Request Headers**
2. **Thêm header mặc định**:
   - `Content-Type`: `application/json`
   - `Accept`: `application/json`

---

## 4. Tạo Request: POST /api/enrollment

### 4.1 Cấu hình Request cơ bản

1. **Tạo request mới**: Click **"New"** → **"Request"**
2. **Đặt tên**: `POST Enroll Course`
3. **Method**: `POST`
4. **URL**: `{{baseUrl}}/api/enrollment`

### 4.2 Cấu hình Headers

| Key | Value |
|-----|-------|
| `Content-Type` | `application/json` |
| `Accept` | `application/json` |

### 4.3 Cấu hình Body

**Chọn**: `Body` → `raw` → `JSON`

```json
{
  "studentId": "11111111-1111-1111-1111-111111111111",
  "classSectionId": "22222222-2222-2222-2222-222222222222",
  "semesterId": "20240000-0000-0000-0000-000000000000"
}
```

### 4.4 Response mong đợi

#### ✅ Success (201 Created)
```json
{
  "enrollmentId": "3c6a525d-fde6-4081-bf25-8ea963d49584",
  "studentId": "11111111-1111-1111-1111-111111111111",
  "classSectionId": "22222222-2222-2222-2222-222222222222",
  "semesterId": "20240000-0000-0000-0000-000000000000",
  "enrollmentDate": "2024-06-23T23:15:30.1234567Z"
}
```

#### ❌ Error Cases

**400 Bad Request** - Validation error:
```json
{
  "message": "Invalid request data",
  "errorCode": "VALIDATION_ERROR"
}
```

**409 Conflict** - Business rule violation:
```json
{
  "message": "Sinh viên đã đăng ký đủ 7 môn học trong học kỳ này",
  "errorCode": "MAX_ENROLLMENT_EXCEEDED"
}
```

### 4.5 Lưu enrollmentId tự động

**Tests tab** - Thêm script để lưu enrollmentId:
```javascript
if (pm.response.code === 201) {
    const response = pm.response.json();
    pm.environment.set("enrollmentId", response.enrollmentId);
    console.log("Enrollment ID saved:", response.enrollmentId);
}
```

---

## 5. Tạo Request: DELETE /api/enrollment/{id}

### 5.1 Cấu hình Request cơ bản

1. **Tạo request mới**: Click **"New"** → **"Request"**
2. **Đặt tên**: `DELETE Drop Course`
3. **Method**: `DELETE`
4. **URL**: `{{baseUrl}}/api/enrollment/{{enrollmentId}}`

### 5.2 Cấu hình Headers

| Key | Value |
|-----|-------|
| `Accept` | `application/json` |

### 5.3 Body
**Không cần body** cho DELETE request

### 5.4 Response mong đợi

#### ✅ Success (204 No Content)
```http
HTTP/1.1 204 No Content
```

#### ❌ Error Cases

**404 Not Found** - Enrollment không tồn tại:
```json
{
  "message": "Không tìm thấy enrollment",
  "errorCode": "ENROLLMENT_NOT_FOUND"
}
```

**403 Forbidden** - Môn bắt buộc hoặc quá hạn:
```json
{
  "message": "Không được hủy môn học bắt buộc",
  "errorCode": "CANNOT_DROP_MANDATORY_COURSE"
}
```

---

## 6. Tạo Request: GET /api/enrollment/{id}

### 6.1 Cấu hình Request cơ bản

1. **Tạo request mới**: Click **"New"** → **"Request"**
2. **Đặt tên**: `GET Enrollment Info`
3. **Method**: `GET`
4. **URL**: `{{baseUrl}}/api/enrollment/{{enrollmentId}}`

### 6.2 Cấu hình Headers

| Key | Value |
|-----|-------|
| `Accept` | `application/json` |

### 6.3 Response mong đợi

#### ✅ Success (200 OK)
```json
{
  "enrollmentId": "3c6a525d-fde6-4081-bf25-8ea963d49584",
  "studentId": "11111111-1111-1111-1111-111111111111",
  "classSectionId": "22222222-2222-2222-2222-222222222222",
  "semesterId": "2024-06-23T23:15:30.1234567Z",
  "enrollmentDate": "2024-06-23T23:15:30.1234567Z"
}
```

#### ❌ Error (404 Not Found)
```json
{
  "message": "Không tìm thấy enrollment",
  "errorCode": "ENROLLMENT_NOT_FOUND"
}
```

---

## 7. Lưu vào Collection

### 7.1 Tạo Collection

1. **Tạo Collection**: Click **"New"** → **"Collection"**
2. **Đặt tên**: `Student Registration API`
3. **Mô tả**: `API testing for Student Registration System`

### 7.2 Thêm requests vào Collection

1. **Kéo thả** hoặc **Copy** 3 requests vào collection
2. **Sắp xếp thứ tự**:
   - `POST Enroll Course`
   - `GET Enrollment Info`
   - `DELETE Drop Course`

### 7.3 Cấu hình Collection

1. **Click vào Collection** → **"Edit"**
2. **Variables**:
   - `baseUrl`: `http://localhost:5255`
3. **Authorization**: None (cho development)
4. **Pre-request Scripts**: (tùy chọn)

---

## 8. Xuất Collection (cho chia sẻ)

### 8.1 Export Collection

1. **Click vào Collection** → **"..."** → **"Export"**
2. **Chọn format**: `Collection v2.1 (recommended)`
3. **Bấm "Export"**
4. **Lưu file**: `StudentRegistrationAPI.postman_collection.json`

### 8.2 Import Collection

1. **Import**: Click **"Import"** → **"Upload Files"**
2. **Chọn file**: `.json` đã export
3. **Import**: Collection sẽ xuất hiện trong workspace

### 8.3 Chia sẻ với team

- **File collection**: Gửi file `.json`
- **Environment**: Export environment riêng
- **Documentation**: Kèm theo tài liệu này

---

## 9. Test tự động với Tests tab

### 9.1 Tests cho POST /api/enrollment

**Tests tab** - Thêm script:
```javascript
// Test status code
pm.test("Status code is 201", function () {
    pm.response.to.have.status(201);
});

// Test response structure
pm.test("Response has enrollmentId", function () {
    const response = pm.response.json();
    pm.expect(response).to.have.property('enrollmentId');
    pm.expect(response.enrollmentId).to.be.a('string');
});

// Save enrollmentId for next request
if (pm.response.code === 201) {
    const response = pm.response.json();
    pm.environment.set("enrollmentId", response.enrollmentId);
    console.log("✅ Enrollment created:", response.enrollmentId);
}
```

### 9.2 Tests cho DELETE /api/enrollment/{id}

**Tests tab** - Thêm script:
```javascript
// Test status code
pm.test("Status code is 204", function () {
    pm.response.to.have.status(204);
});

// Test response is empty
pm.test("Response is empty", function () {
    pm.response.to.not.have.body();
});

console.log("✅ Enrollment deleted successfully");
```

### 9.3 Tests cho GET /api/enrollment/{id}

**Tests tab** - Thêm script:
```javascript
// Test status code
pm.test("Status code is 200", function () {
    pm.response.to.have.status(200);
});

// Test response structure
pm.test("Response has required fields", function () {
    const response = pm.response.json();
    pm.expect(response).to.have.property('enrollmentId');
    pm.expect(response).to.have.property('studentId');
    pm.expect(response).to.have.property('classSectionId');
    pm.expect(response).to.have.property('semesterId');
    pm.expect(response).to.have.property('enrollmentDate');
});

console.log("✅ Enrollment info retrieved");
```

---

## 10. Gợi ý nâng cao

### 10.1 Pre-request Scripts

**Generate GUID tự động** (Pre-request Script):
```javascript
// Generate random GUID for testing
function generateGuid() {
    return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function(c) {
        var r = Math.random() * 16 | 0, v = c == 'x' ? r : (r & 0x3 | 0x8);
        return v.toString(16);
    });
}

// Set random studentId if not exists
if (!pm.environment.get("studentId")) {
    pm.environment.set("studentId", generateGuid());
}
```

### 10.2 Collection Runner

**Chạy hàng loạt request**:
1. **Collection** → **"Run collection"**
2. **Chọn requests** cần chạy
3. **Iterations**: Số lần chạy
4. **Delay**: Thời gian chờ giữa requests
5. **Bấm "Run"**

### 10.3 Newman (Command Line)

**Cài đặt Newman**:
```bash
npm install -g newman
```

**Chạy collection từ command line**:
```bash
newman run StudentRegistrationAPI.postman_collection.json \
  --environment StudentAPI.postman_environment.json \
  --reporters cli,json \
  --reporter-json-export results.json
```

### 10.4 Environment Variables

**Tạo nhiều environment**:
- `Development`: `http://localhost:5255`
- `Staging`: `https://staging-api.example.com`
- `Production`: `https://api.example.com`

### 10.5 Data Files

**Sử dụng CSV/JSON data**:
1. **Tạo file**: `test-data.csv`
2. **Format**:
```csv
studentId,classSectionId,semesterId
11111111-1111-1111-1111-111111111111,22222222-2222-2222-2222-222222222222,20240000-0000-0000-0000-000000000000
33333333-3333-3333-3333-333333333333,44444444-4444-4444-4444-444444444444,20240000-0000-0000-0000-000000000000
```
3. **Collection Runner** → **Data** → **Select file**

---

## 📝 Lưu ý quan trọng

### 🔧 Troubleshooting

| Vấn đề | Nguyên nhân | Cách xử lý |
|--------|-------------|------------|
| **Connection refused** | API chưa chạy | `dotnet run --project src/StudentRegistration.Api` |
| **404 Not Found** | URL sai | Kiểm tra `baseUrl` và endpoint |
| **400 Bad Request** | JSON format sai | Validate JSON syntax |
| **409 Conflict** | Business rule | Xem error message chi tiết |

### 🎯 Best Practices

- ✅ **Lưu enrollmentId** sau khi tạo thành công
- ✅ **Test theo thứ tự**: POST → GET → DELETE
- ✅ **Sử dụng environment variables** thay vì hardcode
- ✅ **Thêm tests** để validate response
- ✅ **Export collection** để chia sẻ team

### 📊 Monitoring

- **Response time**: Kiểm tra performance
- **Status codes**: Đảm bảo đúng business logic
- **Error messages**: Debug dễ dàng
- **Logs**: Kết hợp với server logs

---

## 🎉 Kết luận

Với hướng dẫn này, bạn có thể:
- ✅ **Test API nhanh chóng** trong 5 phút
- ✅ **Tự động hóa testing** với scripts
- ✅ **Chia sẻ với team** qua collection
- ✅ **Debug hiệu quả** với detailed responses

**Happy Testing! 🚀** 