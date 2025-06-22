# 📚 Hướng dẫn thứ tự đọc tài liệu - Student Registration System

## 🎯 Mục đích

File này giúp developer hoặc AI assistant hiểu đúng thứ tự tài liệu cần đọc khi mới tham gia dự án hoặc cần maintain. Hệ thống được xây dựng theo Clean Architecture, do đó cần hiểu đúng tư duy từng tầng trước khi đọc code.

**⏱️ Mục tiêu**: Người mới có thể hiểu toàn bộ dự án chỉ với 15 phút đọc tài liệu có hướng dẫn này.

---

## 📋 Thứ tự đọc tài liệu được khuyến nghị

### Bước 1: Hiểu nghiệp vụ cơ bản
| Tài liệu | Mục tiêu khi đọc |
|----------|------------------|
| `01_Business_Requirement.md` | Hiểu nghiệp vụ và phạm vi hệ thống đăng ký khóa học |

### Bước 2: Nắm các Use Case chính
| Tài liệu | Mục tiêu khi đọc |
|----------|------------------|
| `diagrams/use_case_diagram.puml` | Xem sơ đồ Use Case tổng quan |
| `usecases/UC03_RegisterCourse.md` | Hiểu chi tiết flow đăng ký khóa học |

### Bước 3: Hiểu các quy tắc nghiệp vụ
| Tài liệu | Mục tiêu khi đọc |
|----------|------------------|
| `05_Business_Rules.md` | Biết các Rule nghiệp vụ đang được enforce (BR01-BR04) |

### Bước 4: Hiểu kiến trúc kỹ thuật
| Tài liệu | Mục tiêu khi đọc |
|----------|------------------|
| `10_Technical_Architecture.md` | Hiểu cấu trúc hệ thống theo Clean Architecture |
| `README_ARCH.md` | Tổng quan về kiến trúc và các layer |

### Bước 5: Hiểu cách truy xuất dữ liệu
| Tài liệu | Mục tiêu khi đọc |
|----------|------------------|
| `11_Repositories_Guide.md` | Cách truy xuất dữ liệu và mock repositories |

### Bước 6: Hiểu framework validation
| Tài liệu | Mục tiêu khi đọc |
|----------|------------------|
| `12_Rule_Validation_Framework.md` | Cách viết và kiểm tra rule logic |

### Bước 7: Hiểu chiến lược test
| Tài liệu | Mục tiêu khi đọc |
|----------|------------------|
| `13_Test_Strategy.md` | Cách test các tầng và best practices |

---

## 🚀 Lộ trình đọc nhanh (5 phút)

Nếu bạn chỉ có 5 phút, hãy đọc theo thứ tự:

1. **`01_Business_Requirement.md`** - Hiểu hệ thống làm gì
2. **`05_Business_Rules.md`** - Biết các rule quan trọng
3. **`10_Technical_Architecture.md`** - Hiểu cấu trúc code
4. **`13_Test_Strategy.md`** - Biết cách test

---

## 🔄 Lộ trình đọc chi tiết (15 phút)

Nếu bạn có thời gian, hãy đọc đầy đủ theo thứ tự bảng trên để hiểu sâu về:

- **Nghiệp vụ**: Yêu cầu, use case, business rules
- **Kiến trúc**: Clean Architecture, các layer, dependency
- **Implementation**: Repositories, validation framework, testing

---

## 📝 Ghi chú quan trọng

### Version Control
- Mỗi file trong `docs/` nên được version control và review nghiêm túc
- Khi thêm business rule mới → cập nhật `05_Business_Rules.md`
- Khi thêm rule mới → cập nhật tài liệu test tương ứng

### AI Assistant Integration
- Nếu sử dụng AI assistant (như Cursor), hãy feed từ file này trước
- AI sẽ hiểu context và có thể hỗ trợ tốt hơn

### Maintenance
- Khi maintain code, luôn tham khảo business rules trước
- Khi debug, kiểm tra test strategy để hiểu cách test
- Khi thêm feature mới, xem technical architecture để đặt đúng layer

---

## 🎯 Kết quả mong đợi

Sau khi đọc xong theo hướng dẫn này, bạn sẽ:

✅ **Hiểu rõ nghiệp vụ**: Hệ thống đăng ký khóa học với các rule nghiêm ngặt  
✅ **Nắm kiến trúc**: Clean Architecture với 4 layer rõ ràng  
✅ **Biết cách code**: Domain entities, Application services, Infrastructure  
✅ **Hiểu testing**: Unit test cho từng layer, integration test  
✅ **Có thể maintain**: Thêm rule mới, sửa bug, refactor code  

---

## 🔗 Liên kết nhanh

- **Business**: `01_Business_Requirement.md` → `05_Business_Rules.md`
- **Architecture**: `10_Technical_Architecture.md` → `README_ARCH.md`
- **Implementation**: `11_Repositories_Guide.md` → `12_Rule_Validation_Framework.md`
- **Testing**: `13_Test_Strategy.md`

---

*📌 Lưu ý: File này được cập nhật theo từng version của dự án. Hãy đảm bảo bạn đang đọc phiên bản mới nhất.* 