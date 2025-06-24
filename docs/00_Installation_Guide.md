# Hướng dẫn cài đặt dependencies - Student Registration System

## 📋 Yêu cầu hệ thống

### Backend (.NET 8)
- **.NET 8.0 SDK** (bắt buộc)
- **Visual Studio 2022** hoặc **VS Code** (khuyến nghị)
- **SQLite** (đã có sẵn trong .NET 8)

### Frontend (React)
- **Node.js** >= 16.0.0 (bắt buộc)
- **npm** >= 8.0.0 (thường đi kèm Node.js)

## 🔧 Cài đặt Backend Dependencies

### 1. Cài đặt .NET 8.0 SDK

#### Windows:
```bash
# Tải từ Microsoft
https://dotnet.microsoft.com/download/dotnet/8.0

# Hoặc dùng winget
winget install Microsoft.DotNet.SDK.8
```

#### macOS:
```bash
# Dùng Homebrew
brew install dotnet

# Hoặc tải từ Microsoft
https://dotnet.microsoft.com/download/dotnet/8.0
```

#### Linux (Ubuntu/Debian):
```bash
# Thêm Microsoft package repository
wget https://packages.microsoft.com/config/ubuntu/20.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
sudo dpkg -i packages-microsoft-prod.deb

# Cài đặt .NET 8 SDK
sudo apt-get update
sudo apt-get install -y dotnet-sdk-8.0
```

### 2. Kiểm tra cài đặt
```bash
dotnet --version
# Kết quả: 8.0.x
```

### 3. Cài đặt dependencies cho Backend
```bash
# Từ thư mục gốc project
cd src/StudentRegistration.Api

# Restore packages
dotnet restore

# Build project
dotnet build
```

### 4. Cài đặt Entity Framework Tools (nếu cần)
```bash
# Cài đặt EF CLI tools
dotnet tool install --global dotnet-ef

# Kiểm tra cài đặt
dotnet ef --version
```

## 🎨 Cài đặt Frontend Dependencies

### 1. Cài đặt Node.js

#### Windows:
```bash
# Tải từ nodejs.org
https://nodejs.org/en/download/

# Hoặc dùng winget
winget install OpenJS.NodeJS
```

#### macOS:
```bash
# Dùng Homebrew
brew install node

# Hoặc dùng nvm
curl -o- https://raw.githubusercontent.com/nvm-sh/nvm/v0.39.0/install.sh | bash
nvm install 18
nvm use 18
```

#### Linux:
```bash
# Ubuntu/Debian
curl -fsSL https://deb.nodesource.com/setup_18.x | sudo -E bash -
sudo apt-get install -y nodejs

# Hoặc dùng nvm
curl -o- https://raw.githubusercontent.com/nvm-sh/nvm/v0.39.0/install.sh | bash
nvm install 18
nvm use 18
```

### 2. Kiểm tra cài đặt
```bash
node --version
# Kết quả: v18.x.x hoặc v20.x.x

npm --version
# Kết quả: 8.x.x hoặc 9.x.x
```

### 3. Cài đặt dependencies cho Frontend
```bash
# Từ thư mục gốc project
cd frontend

# Cài đặt dependencies
npm install

# Kiểm tra cài đặt thành công
npm list --depth=0
```

## 📦 Dependencies được sử dụng

### Backend Dependencies (tự động cài qua NuGet)
```xml
<!-- JWT Authentication -->
<PackageReference Include="Microsoft.AspNetCore.Authentication.JwtBearer" Version="8.0.0" />

<!-- Entity Framework -->
<PackageReference Include="Microsoft.EntityFrameworkCore.Sqlite" Version="8.0.0" />
<PackageReference Include="Microsoft.EntityFrameworkCore.Design" Version="8.0.0" />

<!-- JSON Configuration -->
<PackageReference Include="Microsoft.Extensions.Configuration.Json" Version="8.0.0" />

<!-- CORS -->
<PackageReference Include="Microsoft.AspNetCore.Cors" Version="2.2.0" />
```

### Frontend Dependencies (package.json)
```json
{
  "dependencies": {
    "react": "^18.2.0",
    "react-dom": "^18.2.0",
    "react-router-dom": "^6.8.0",
    "axios": "^1.3.0",
    "typescript": "^4.9.0"
  },
  "devDependencies": {
    "tailwindcss": "^3.2.0",
    "postcss": "^8.4.0",
    "autoprefixer": "^10.4.0",
    "@types/react": "^18.0.0",
    "@types/react-dom": "^18.0.0"
  }
}
```

## 🚀 Chạy hệ thống

### 1. Chạy Backend
```bash
# Terminal 1
cd src/StudentRegistration.Api
dotnet run

# API sẽ chạy ở: http://localhost:5255
```

### 2. Chạy Frontend
```bash
# Terminal 2
cd frontend
npm start

# App sẽ chạy ở: http://localhost:3000
```

## 🔍 Kiểm tra cài đặt

### Backend Test:
```bash
# Test build
dotnet build

# Test run
dotnet run --project src/StudentRegistration.Api

# Test API endpoint
curl http://localhost:5255/api/auth/login
```

### Frontend Test:
```bash
# Test build
npm run build

# Test dev server
npm start

# Mở browser: http://localhost:3000
```

## ❗ Lỗi thường gặp

### Backend:
```bash
# Lỗi: .NET SDK not found
# Giải pháp: Cài đặt .NET 8.0 SDK

# Lỗi: Package restore failed
# Giải pháp: 
dotnet restore --force

# Lỗi: Build failed
# Giải pháp:
dotnet clean
dotnet build
```

### Frontend:
```bash
# Lỗi: Node.js not found
# Giải pháp: Cài đặt Node.js >= 16

# Lỗi: npm install failed
# Giải pháp:
rm -rf node_modules package-lock.json
npm install

# Lỗi: Build failed
# Giải pháp:
npm run build
```

## 📝 Checklist cài đặt

### Backend:
- [ ] .NET 8.0 SDK đã cài
- [ ] `dotnet --version` trả về 8.0.x
- [ ] `dotnet restore` thành công
- [ ] `dotnet build` thành công
- [ ] API chạy được ở localhost:5255

### Frontend:
- [ ] Node.js >= 16 đã cài
- [ ] `node --version` trả về v16+ hoặc v18+
- [ ] `npm install` thành công
- [ ] `npm start` chạy được
- [ ] App mở được ở localhost:3000

### Tích hợp:
- [ ] Backend và Frontend chạy đồng thời
- [ ] Login thành công từ Frontend
- [ ] API calls từ Frontend hoạt động
- [ ] CORS không báo lỗi

## 🎯 Bước tiếp theo

Sau khi cài đặt thành công:
1. Đọc [docs/README_How_To_Use.md](README_How_To_Use.md) để biết cách sử dụng
2. Đọc [docs/frontend/README.md](frontend/README.md) để hiểu Frontend
3. Đọc [docs/14_Authentication_Guide.md](14_Authentication_Guide.md) để hiểu Authentication
4. Chạy tests: `dotnet test` và `npm test`

---
**Chúc bạn cài đặt thành công!** 🎉 