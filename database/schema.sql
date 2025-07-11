-- Student Registration System Database Schema
-- SQLite Database Schema for Student Registration System

-- RefreshTokens Table
CREATE TABLE IF NOT EXISTS RefreshTokens (
    Id TEXT PRIMARY KEY,
    Token TEXT NOT NULL UNIQUE,
    UserId TEXT NOT NULL,
    CreatedAt TEXT NOT NULL,
    ExpiresAt TEXT NOT NULL,
    IsRevoked INTEGER NOT NULL DEFAULT 0,
    RevokedAt TEXT,
    RevokedBy TEXT
);

-- Indexes for RefreshTokens
CREATE INDEX IF NOT EXISTS IX_RefreshTokens_UserId ON RefreshTokens(UserId);
CREATE INDEX IF NOT EXISTS IX_RefreshTokens_Token ON RefreshTokens(Token);
CREATE INDEX IF NOT EXISTS IX_RefreshTokens_ExpiresAt ON RefreshTokens(ExpiresAt);
CREATE INDEX IF NOT EXISTS IX_RefreshTokens_IsRevoked ON RefreshTokens(IsRevoked);

-- Users Table (for reference)
CREATE TABLE IF NOT EXISTS Users (
    Id TEXT PRIMARY KEY,
    Username TEXT NOT NULL UNIQUE,
    PasswordHash TEXT NOT NULL,
    Role TEXT NOT NULL,
    CreatedAt TEXT NOT NULL
);

-- Enrollments Table (for reference)
CREATE TABLE IF NOT EXISTS Enrollments (
    Id TEXT PRIMARY KEY,
    StudentId TEXT NOT NULL,
    SectionId TEXT NOT NULL,
    EnrollmentDate TEXT NOT NULL,
    Status TEXT NOT NULL
);

-- Indexes for Enrollments
CREATE INDEX IF NOT EXISTS IX_Enrollments_StudentId ON Enrollments(StudentId);
CREATE INDEX IF NOT EXISTS IX_Enrollments_SectionId ON Enrollments(SectionId);
CREATE INDEX IF NOT EXISTS IX_Enrollments_Status ON Enrollments(Status);

-- Sample data for testing
INSERT OR IGNORE INTO Users (Id, Username, PasswordHash, Role, CreatedAt) VALUES
('11111111-1111-1111-1111-111111111111', 'student1', 'password123', 'Student', datetime('now')),
('22222222-2222-2222-2222-222222222222', 'admin1', 'password123', 'Admin', datetime('now'));

-- Cleanup expired tokens (run periodically)
-- DELETE FROM RefreshTokens WHERE ExpiresAt < datetime('now');
