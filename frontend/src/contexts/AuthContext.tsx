import React, { createContext, useContext, useState, useEffect, ReactNode } from 'react';
import { authAPI } from '../services/api';
import { tokenUtils } from '../utils/tokenUtils';
import { LoginRequest, LoginResponse, User } from '../types/api';

// Auth context interface
interface AuthContextType {
  user: User | null;
  isAuthenticated: boolean;
  isLoading: boolean;
  login: (credentials: LoginRequest) => Promise<void>;
  logout: () => Promise<void>;
  checkAuth: () => boolean;
}

// Tạo context
const AuthContext = createContext<AuthContextType | undefined>(undefined);

// Auth provider props
interface AuthProviderProps {
  children: ReactNode;
}

// Auth provider component
export const AuthProvider: React.FC<AuthProviderProps> = ({ children }) => {
  const [user, setUser] = useState<User | null>(null);
  const [isLoading, setIsLoading] = useState(true);

  // Kiểm tra trạng thái đăng nhập khi component mount
  useEffect(() => {
    checkAuthStatus();
  }, []);

  // Kiểm tra trạng thái đăng nhập
  const checkAuthStatus = () => {
    const hasValidToken = tokenUtils.hasValidToken();
    
    if (hasValidToken) {
      // TODO: Gọi API để lấy thông tin user
      // Hiện tại chỉ kiểm tra token có hợp lệ không
      setUser({
        id: '1',
        username: 'student1',
        email: 'student1@example.com',
        role: 'Student'
      });
    } else {
      setUser(null);
    }
    
    setIsLoading(false);
  };

  // Function đăng nhập
  const login = async (credentials: LoginRequest): Promise<void> => {
    try {
      setIsLoading(true);
      const response: LoginResponse = await authAPI.login(credentials);
      
      // Lưu tokens
      tokenUtils.setTokens(response.accessToken, response.refreshToken, response.expiresIn);
      
      // TODO: Gọi API để lấy thông tin user
      // Hiện tại set user mặc định
      setUser({
        id: '1',
        username: credentials.username,
        email: `${credentials.username}@example.com`,
        role: 'Student'
      });
    } catch (error) {
      console.error('Login error:', error);
      throw error;
    } finally {
      setIsLoading(false);
    }
  };

  // Function đăng xuất
  const logout = async (): Promise<void> => {
    try {
      await authAPI.logout();
    } catch (error) {
      console.error('Logout error:', error);
    } finally {
      setUser(null);
      tokenUtils.clearTokens();
    }
  };

  // Kiểm tra có đăng nhập không
  const checkAuth = (): boolean => {
    return tokenUtils.hasValidToken();
  };

  const value: AuthContextType = {
    user,
    isAuthenticated: !!user,
    isLoading,
    login,
    logout,
    checkAuth
  };

  return (
    <AuthContext.Provider value={value}>
      {children}
    </AuthContext.Provider>
  );
};

// Custom hook để sử dụng AuthContext
export const useAuth = (): AuthContextType => {
  const context = useContext(AuthContext);
  if (context === undefined) {
    throw new Error('useAuth must be used within an AuthProvider');
  }
  return context;
}; 