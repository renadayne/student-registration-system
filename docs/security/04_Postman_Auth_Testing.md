# 🎨 Postman Auth Testing - Test Authentication bằng Postman

## 📋 Tổng quan

Tài liệu này hướng dẫn chi tiết cách sử dụng Postman để test authentication và authorization trong hệ thống Student Registration System, bao gồm cách login, lưu token và gọi các API được bảo vệ.

---

## 🚀 Bước 1: Cài đặt và Chuẩn bị

### 📥 Cài đặt Postman
1. Tải Postman từ [postman.com](https://www.postman.com/downloads/)
2. Cài đặt và tạo tài khoản (miễn phí)
3. Mở Postman

### 🔧 Chuẩn bị Environment
1. **Tạo Environment mới**:
   - Click "Environments" → "New"
   - Đặt tên: `Student Registration System`
   - Thêm các variables:

| Variable | Initial Value | Current Value |
|----------|---------------|---------------|
| `baseUrl` | `http://localhost:5255` | `http://localhost:5255` |
| `accessToken` | (để trống) | (sẽ được set sau khi login) |
| `userId` | `11111111-1111-1111-1111-111111111111` | `11111111-1111-1111-1111-111111111111` |
| `semesterId` | `20240000-0000-0000-0000-000000000000` | `20240000-0000-0000-0000-000000000000` |

2. **Save Environment** và chọn làm active

---

## 🔐 Bước 2: Tạo Login Request

### 📝 Tạo Request mới
1. **Click "New"** → "Request"
2. **Đặt tên**: `Login - Student`
3. **Chọn Environment**: `Student Registration System`

### ⚙️ Cấu hình Request
- **Method**: `POST`
- **URL**: `{{baseUrl}}/auth/login`
- **Headers**:
  ```
  Content-Type: application/json
  ```

### 📄 Body (raw JSON)
```json
{
  "username": "student1",
  "password": "password123"
}
```

### 🎯 Test Script (Optional)
```javascript
// Tests tab
pm.test("Login successful", function () {
    pm.response.to.have.status(200);
});

pm.test("Response has access token", function () {
    var jsonData = pm.response.json();
    pm.expect(jsonData).to.have.property('accessToken');
    pm.expect(jsonData.accessToken).to.not.be.empty;
});

// Auto-save token to environment
if (pm.response.code === 200) {
    var jsonData = pm.response.json();
    pm.environment.set("accessToken", jsonData.accessToken);
    console.log("Token saved to environment:", jsonData.accessToken);
}
```

### 🚀 Send Request
1. Click **"Send"**
2. **Expected Response (200 OK)**:
   ```json
   {
     "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
     "expiresIn": 7200,
     "tokenType": "Bearer"
   }
   ```

---

## 🔄 Bước 3: Tạo Collection

### 📁 Tạo Collection
1. **Click "New"** → "Collection"
2. **Đặt tên**: `Student Registration API`
3. **Description**: `API testing for Student Registration System`

### 📋 Tổ chức Collection
```
Student Registration API/
├── Authentication/
│   ├── Login - Student
│   ├── Login - Admin
│   └── Login - Invalid Credentials
├── Enrollment/
│   ├── Enroll Course
│   ├── Drop Course
│   └── Get My Enrollments
└── Admin/
    ├── Get All Enrollments
    └── Get Statistics
```

---

## 🛡️ Bước 4: Test Protected Endpoints

### 📚 Test UC05 - Get My Enrollments

#### Tạo Request
1. **New Request** trong collection
2. **Đặt tên**: `Get My Enrollments`
3. **Method**: `GET`
4. **URL**: `{{baseUrl}}/students/{{userId}}/enrollments?semesterId={{semesterId}}`

#### ⚙️ Authorization
1. **Tab "Authorization"**
2. **Type**: `Bearer Token`
3. **Token**: `{{accessToken}}`

#### 🎯 Test Script
```javascript
pm.test("Status code is 200", function () {
    pm.response.to.have.status(200);
});

pm.test("Response is JSON", function () {
    pm.response.to.be.json;
});

pm.test("Response has enrollments array", function () {
    var jsonData = pm.response.json();
    pm.expect(jsonData).to.have.property('enrollments');
    pm.expect(jsonData.enrollments).to.be.an('array');
});

pm.test("User is authenticated", function () {
    pm.expect(pm.request.headers.get("Authorization")).to.include("Bearer");
});
```

### 📝 Test UC03 - Enroll Course

#### Tạo Request
1. **New Request** trong collection
2. **Đặt tên**: `Enroll Course`
3. **Method**: `POST`
4. **URL**: `{{baseUrl}}/api/enrollment`

#### ⚙️ Authorization
- **Type**: `Bearer Token`
- **Token**: `{{accessToken}}`

#### 📄 Body (raw JSON)
```json
{
  "studentId": "{{userId}}",
  "classSectionId": "55555555-5555-5555-5555-555555555555"
}
```

#### 🎯 Test Script
```javascript
pm.test("Enrollment successful", function () {
    pm.response.to.have.status(200);
});

pm.test("Response has enrollment ID", function () {
    var jsonData = pm.response.json();
    pm.expect(jsonData).to.have.property('enrollmentId');
    pm.expect(jsonData.enrollmentId).to.not.be.empty;
});

// Save enrollment ID for later use
if (pm.response.code === 200) {
    var jsonData = pm.response.json();
    pm.environment.set("lastEnrollmentId", jsonData.enrollmentId);
}
```

### 🗑️ Test UC04 - Drop Course

#### Tạo Request
1. **New Request** trong collection
2. **Đặt tên**: `Drop Course`
3. **Method**: `DELETE`
4. **URL**: `{{baseUrl}}/api/enrollment/{{lastEnrollmentId}}`

#### ⚙️ Authorization
- **Type**: `Bearer Token`
- **Token**: `{{accessToken}}`

#### 🎯 Test Script
```javascript
pm.test("Drop course successful", function () {
    pm.response.to.have.status(200);
});

pm.test("Response confirms deletion", function () {
    var jsonData = pm.response.json();
    pm.expect(jsonData).to.have.property('message');
    pm.expect(jsonData.message).to.include('hủy đăng ký');
});
```

---

## 👥 Bước 5: Test Role-based Authorization

### 🎓 Test với Student Role

#### Login as Student
```json
{
  "username": "student1",
  "password": "password123"
}
```

#### Test Student Permissions
- ✅ `GET /students/{id}/enrollments` - Should work
- ✅ `POST /api/enrollment` - Should work
- ✅ `DELETE /api/enrollment/{id}` - Should work
- ❌ `GET /admin/enrollments` - Should return 403

### 👨‍💼 Test với Admin Role

#### Login as Admin
```json
{
  "username": "admin1",
  "password": "adminpass"
}
```

#### Test Admin Permissions
- ✅ `GET /students/{id}/enrollments` - Should work
- ✅ `POST /api/enrollment` - Should work
- ✅ `DELETE /api/enrollment/{id}` - Should work
- ✅ `GET /admin/enrollments` - Should work (if implemented)

---

## 🧪 Bước 6: Test Error Cases

### ❌ Test Cases cho Authentication

#### 1. Login với sai password
```json
{
  "username": "student1",
  "password": "wrongpassword"
}
```
**Expected**: 401 Unauthorized

#### 2. Gọi API không có token
- Remove Authorization header
- **Expected**: 401 Unauthorized

#### 3. Gọi API với token sai
- Set Authorization: `Bearer invalid-token`
- **Expected**: 401 Unauthorized

#### 4. Gọi API với token hết hạn
- Wait for token to expire (2 hours)
- **Expected**: 401 Unauthorized

#### 5. Student gọi Admin API
- Login as student
- Try to access admin endpoint
- **Expected**: 403 Forbidden

---

## 🔧 Bước 7: Automation và Pre-request Scripts

### 🔄 Pre-request Script cho Auto-login
```javascript
// Pre-request Script tab
// Auto-login if no token exists
if (!pm.environment.get("accessToken")) {
    console.log("No token found, performing auto-login...");
    
    // Create login request
    var loginRequest = {
        url: pm.environment.get("baseUrl") + "/auth/login",
        method: "POST",
        header: {
            "Content-Type": "application/json"
        },
        body: {
            mode: "raw",
            raw: JSON.stringify({
                username: "student1",
                password: "password123"
            })
        }
    };
    
    // Send login request
    pm.sendRequest(loginRequest, function (err, response) {
        if (response.code === 200) {
            var jsonData = response.json();
            pm.environment.set("accessToken", jsonData.accessToken);
            console.log("Auto-login successful, token saved");
        } else {
            console.log("Auto-login failed:", response.text());
        }
    });
}
```

### 🎯 Collection Variables
```javascript
// Collection Variables
{
  "defaultStudentId": "11111111-1111-1111-1111-111111111111",
  "defaultSemesterId": "20240000-0000-0000-0000-000000000000",
  "testClassSectionId": "55555555-5555-5555-5555-555555555555"
}
```

---

## 📊 Bước 8: Export/Import Collection

### 📤 Export Collection
1. **Right-click collection** → "Export"
2. **Format**: `Collection v2.1`
3. **Save file**: `StudentRegistrationAPI.postman_collection.json`

### 📥 Import Collection
1. **Import** → "Upload Files"
2. **Select file**: `StudentRegistrationAPI.postman_collection.json`
3. **Import**

### 🌐 Share Collection
1. **Right-click collection** → "Share"
2. **Get public link** hoặc **Invite team members**

---

## 🔍 Bước 9: Monitoring và Debugging

### 📊 Response Time Monitoring
```javascript
// Tests tab
pm.test("Response time is less than 2000ms", function () {
    pm.expect(pm.response.responseTime).to.be.below(2000);
});
```

### 🔍 Debug Token
```javascript
// Tests tab
pm.test("Debug token info", function () {
    var token = pm.environment.get("accessToken");
    if (token) {
        console.log("Current token:", token);
        
        // Decode JWT payload (base64)
        var parts = token.split('.');
        if (parts.length === 3) {
            var payload = parts[1];
            var decoded = JSON.parse(atob(payload));
            console.log("Token payload:", decoded);
        }
    }
});
```

### 📝 Log Requests
```javascript
// Pre-request Script
console.log("Request URL:", pm.request.url);
console.log("Request method:", pm.request.method);
console.log("Request headers:", pm.request.headers);
```

---

## ⚠️ Troubleshooting

### 🔴 Common Issues

#### 1. "Invalid token" error
**Nguyên nhân**: Token expired hoặc sai format
**Giải pháp**: 
- Login lại để lấy token mới
- Kiểm tra token format: `Bearer <token>`

#### 2. "Connection refused" error
**Nguyên nhân**: API server không chạy
**Giải pháp**:
- Kiểm tra API có đang chạy không
- Kiểm tra port đúng (5255)

#### 3. "CORS" error
**Nguyên nhân**: Cross-origin request
**Giải pháp**:
- Sử dụng Postman (không bị CORS)
- Hoặc cấu hình CORS trong API

#### 4. "Environment variable not found"
**Nguyên nhân**: Variable chưa được set
**Giải pháp**:
- Kiểm tra environment đã active chưa
- Kiểm tra variable name đúng không

---

## 📚 Tài liệu liên quan

- [Security Overview](00_Security_Overview.md) - Tổng quan bảo mật
- [Login Flow Guide](02_Login_Flow_Guide.md) - Hướng dẫn đăng nhập
- [Protecting API](03_Protecting_API_with_JWT.md) - Bảo vệ API
- [Troubleshooting](05_Troubleshooting_Auth.md) - Xử lý lỗi

---

## 🎯 Kết luận

Test authentication bằng Postman:
- ✅ **Dễ sử dụng**: GUI friendly, không cần code
- ✅ **Powerful**: Hỗ trợ automation, environment variables
- ✅ **Collaborative**: Có thể share collection với team
- ✅ **Comprehensive**: Test được tất cả scenarios

**Bước tiếp theo**: Đọc [Troubleshooting](05_Troubleshooting_Auth.md) để xử lý các lỗi thường gặp! 