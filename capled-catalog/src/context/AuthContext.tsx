import { createContext, useContext, useState, useEffect, ReactNode } from 'react';

interface AuthUser {
  token: string;
  email: string;
  fullName: string;
  clientId: number;
  expiresAt: string;
}

interface AuthContextType {
  user: AuthUser | null;
  isAuthenticated: boolean;
  login: (userData: AuthUser) => void;
  logout: () => void;
}

const AuthContext = createContext<AuthContextType | null>(null);

export const useAuth = () => {
  const ctx = useContext(AuthContext);
  if (!ctx) throw new Error('useAuth must be used within AuthProvider');
  return ctx;
};

export const AuthProvider = ({ children }: { children: ReactNode }) => {
  const [user, setUser] = useState<AuthUser | null>(() => {
    try {
      const saved = localStorage.getItem('capled_auth');
      if (!saved) return null;
      const parsed = JSON.parse(saved);
      // Check if token is expired
      if (new Date(parsed.expiresAt) < new Date()) {
        localStorage.removeItem('capled_auth');
        return null;
      }
      return parsed;
    } catch {
      return null;
    }
  });

  useEffect(() => {
    if (user) {
      localStorage.setItem('capled_auth', JSON.stringify(user));
    } else {
      localStorage.removeItem('capled_auth');
    }
  }, [user]);

  const login = (userData: AuthUser) => {
    setUser(userData);
  };

  const logout = () => {
    setUser(null);
  };

  return (
    <AuthContext.Provider value={{ user, isAuthenticated: !!user, login, logout }}>
      {children}
    </AuthContext.Provider>
  );
};
