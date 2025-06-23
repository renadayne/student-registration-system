-- Dữ liệu mẫu cho Student Registration System

-- Students
INSERT OR IGNORE INTO Students (Id, FullName) VALUES
('11111111-1111-1111-1111-111111111111', 'Nguyen Van A'),
('22222222-2222-2222-2222-222222222222', 'Tran Thi B');

-- Courses
INSERT OR IGNORE INTO Courses (Id, Name, Credits) VALUES
('c001', 'Lập trình C# cơ bản', 3),
('c002', 'Cơ sở dữ liệu', 3);

-- ClassSections
INSERT OR IGNORE INTO ClassSections (Id, CourseId, Name, Semester, MaxSlot) VALUES
('s001', 'c001', 'LTCB-01', '20241', 30),
('s002', 'c002', 'CSDL-01', '20241', 30);

-- Enrollments
INSERT OR IGNORE INTO Enrollments (Id, StudentId, SectionId, EnrollmentDate, Status) VALUES
('e001', '11111111-1111-1111-1111-111111111111', 's001', datetime('now'), 'Active');

-- RefreshTokens (để trống, sẽ sinh động qua API) 