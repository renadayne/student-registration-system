import axios, { AxiosInstance, AxiosResponse, AxiosError, InternalAxiosRequestConfig } from 'axios';
import { tokenUtils } from '../utils/tokenUtils';
import { 
  LoginRequest, 
  LoginResponse, 
  RefreshTokenRequest, 
  RefreshTokenResponse,
  EnrollRequest,
  EnrollResponse,
  Enrollment,
  ApiError
} from '../types/api';

// Extend AxiosRequestConfig để hỗ trợ _retry property
interface CustomAxiosRequestConfig extends InternalAxiosRequestConfig {
  _retry?: boolean;
}

// Tạo axios instance
const api: AxiosInstance = axios.create({
  baseURL: process.env.REACT_APP_API_URL || 'http://localhost:5255',
  headers: {
    'Content-Type': 'application/json',
  },
});

// Request interceptor - thêm token vào header
api.interceptors.request.use(
  (config) => {
    const token = tokenUtils.getAccessToken();
    if (token && !tokenUtils.isTokenExpired()) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  (error) => {
    return Promise.reject(error);
  }
);

// Response interceptor - xử lý token refresh
api.interceptors.response.use(
  (response: AxiosResponse) => {
    return response;
  },
  async (error: AxiosError<ApiError>) => {
    const originalRequest = error.config as CustomAxiosRequestConfig;
    
    // Nếu lỗi 401 và chưa thử refresh token
    if (error.response?.status === 401 && originalRequest && !originalRequest._retry) {
      originalRequest._retry = true;
      
      try {
        const refreshToken = tokenUtils.getRefreshToken();
        if (!refreshToken) {
          throw new Error('No refresh token available');
        }

        // Gọi API refresh token
        const response = await axios.post<RefreshTokenResponse>(
          `${process.env.REACT_APP_API_URL}/auth/refresh`,
          { refreshToken } as RefreshTokenRequest
        );

        const { accessToken, refreshToken: newRefreshToken, expiresIn } = response.data;
        
        // Cập nhật tokens
        tokenUtils.setTokens(accessToken, newRefreshToken, expiresIn);
        
        // Thử lại request gốc với token mới
        originalRequest.headers.Authorization = `Bearer ${accessToken}`;
        return api(originalRequest);
      } catch (refreshError) {
        // Nếu refresh token cũng hết hạn, xóa tokens và redirect login
        tokenUtils.clearTokens();
        window.location.href = '/login';
        return Promise.reject(refreshError);
      }
    }
    
    return Promise.reject(error);
  }
);

// Auth API
export const authAPI = {
  login: async (credentials: LoginRequest): Promise<LoginResponse> => {
    const response = await api.post<LoginResponse>('/auth/login', credentials);
    return response.data;
  },

  refreshToken: async (refreshToken: string): Promise<RefreshTokenResponse> => {
    const response = await api.post<RefreshTokenResponse>('/auth/refresh', { refreshToken });
    return response.data;
  },

  logout: async (): Promise<void> => {
    try {
      await api.post('/auth/logout');
    } finally {
      tokenUtils.clearTokens();
    }
  }
};

// Enrollment API
export const enrollmentAPI = {
  getEnrollments: async (studentId: string, semesterId?: string): Promise<Enrollment[]> => {
    const params = semesterId ? { semesterId } : {};
    const response = await api.get<Enrollment[]>(`/students/${studentId}/enrollments`, { params });
    return response.data;
  },

  enroll: async (request: EnrollRequest): Promise<EnrollResponse> => {
    const response = await api.post<EnrollResponse>('/enrollments', request);
    return response.data;
  },

  dropEnrollment: async (enrollmentId: string): Promise<void> => {
    await api.delete(`/enrollments/${enrollmentId}`);
  }
};

export default api; 