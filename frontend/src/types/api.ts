// Auth interfaces
export interface LoginRequest {
  username: string;
  password: string;
}

export interface LoginResponse {
  accessToken: string;
  refreshToken: string;
  expiresIn: number;
}

export interface RefreshTokenRequest {
  refreshToken: string;
}

export interface RefreshTokenResponse {
  accessToken: string;
  refreshToken: string;
  expiresIn: number;
}

// Enrollment interfaces
export interface Enrollment {
  id: string;
  studentId: string;
  classSectionId: string;
  enrollmentDate: string;
  status: 'Active' | 'Dropped';
}

export interface EnrollRequest {
  studentId: string;
  classSectionId: string;
}

export interface EnrollResponse {
  enrollmentId: string;
  message: string;
}

// User interface
export interface User {
  id: string;
  username: string;
  email: string;
  role: string;
}

// Error interface
export interface ApiError {
  message: string;
  statusCode: number;
} 