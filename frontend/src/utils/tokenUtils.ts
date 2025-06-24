// Token storage keys
const ACCESS_TOKEN_KEY = 'accessToken';
const REFRESH_TOKEN_KEY = 'refreshToken';
const TOKEN_EXPIRY_KEY = 'tokenExpiry';

// Token utilities
export const tokenUtils = {
  // Lưu tokens vào localStorage
  setTokens: (accessToken: string, refreshToken: string, expiresIn: number) => {
    const expiryTime = Date.now() + expiresIn * 1000;
    localStorage.setItem(ACCESS_TOKEN_KEY, accessToken);
    localStorage.setItem(REFRESH_TOKEN_KEY, refreshToken);
    localStorage.setItem(TOKEN_EXPIRY_KEY, expiryTime.toString());
  },

  // Lấy access token
  getAccessToken: (): string | null => {
    return localStorage.getItem(ACCESS_TOKEN_KEY);
  },

  // Lấy refresh token
  getRefreshToken: (): string | null => {
    return localStorage.getItem(REFRESH_TOKEN_KEY);
  },

  // Kiểm tra token có hết hạn chưa
  isTokenExpired: (): boolean => {
    const expiryTime = localStorage.getItem(TOKEN_EXPIRY_KEY);
    if (!expiryTime) return true;
    
    return Date.now() > parseInt(expiryTime);
  },

  // Kiểm tra có token không
  hasValidToken: (): boolean => {
    const token = tokenUtils.getAccessToken();
    return token !== null && !tokenUtils.isTokenExpired();
  },

  // Xóa tất cả tokens
  clearTokens: () => {
    localStorage.removeItem(ACCESS_TOKEN_KEY);
    localStorage.removeItem(REFRESH_TOKEN_KEY);
    localStorage.removeItem(TOKEN_EXPIRY_KEY);
  },

  // Cập nhật access token
  updateAccessToken: (accessToken: string, expiresIn: number) => {
    const expiryTime = Date.now() + expiresIn * 1000;
    localStorage.setItem(ACCESS_TOKEN_KEY, accessToken);
    localStorage.setItem(TOKEN_EXPIRY_KEY, expiryTime.toString());
  }
}; 