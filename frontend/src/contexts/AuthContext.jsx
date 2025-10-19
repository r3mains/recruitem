import React, { createContext, useContext, useState, useEffect } from "react";
import { useApi } from "./ApiContext";

const AuthContext = createContext();

export const useAuth = () => {
  const context = useContext(AuthContext);
  if (!context) {
    throw new Error("useAuth must be used within an AuthProvider");
  }
  return context;
};

export const AuthProvider = ({ children }) => {
  const [user, setUser] = useState(null);
  const [loading, setLoading] = useState(true);
  const api = useApi();

  useEffect(() => {
    const token = localStorage.getItem("token");
    if (token) {
      getUserFromToken(token);
    } else {
      setLoading(false);
    }
  }, []);

  const getUserFromToken = async (token) => {
    try {
      const payload = JSON.parse(atob(token.split(".")[1]));
      const userDetails = await api.users.getById(payload.sub);
      const roles = await api.roles.getAll();
      const userRole = roles.find((r) => r.id === userDetails.roleId);

      setUser({
        id: userDetails.id,
        email: userDetails.email,
        role: userRole?.name || "Unknown",
      });
    } catch (error) {
      localStorage.removeItem("token");
    } finally {
      setLoading(false);
    }
  };

  const login = async (email, password) => {
    try {
      const response = await api.auth.login({ email, password });
      localStorage.setItem("token", response.token);
      await getUserFromToken(response.token);
      return { success: true };
    } catch (error) {
      return {
        success: false,
        error: error.response?.data?.message || "Login failed",
      };
    }
  };

  const register = async (email, password, role) => {
    try {
      const response = await api.auth.register({ email, password, role });
      localStorage.setItem("token", response.token);
      await getUserFromToken(response.token);
      return { success: true };
    } catch (error) {
      return {
        success: false,
        error: error.response?.data?.message || "Registration failed",
      };
    }
  };

  const logout = () => {
    localStorage.removeItem("token");
    setUser(null);
  };

  const hasRole = (requiredRoles) => {
    if (!user) return false;
    const roles = Array.isArray(requiredRoles)
      ? requiredRoles
      : [requiredRoles];
    return roles.includes(user.role);
  };

  const value = {
    user,
    login,
    register,
    logout,
    hasRole,
    isAuthenticated: !!user,
    loading,
  };

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
};
