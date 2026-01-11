import React, { createContext, useContext, useState, useEffect } from "react";
import { useApi } from "./ApiContext";
import { getErrorMessage } from "../utils/errorHandler";

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
      if (!token) {
        console.error("No token provided to getUserFromToken");
        setLoading(false);
        return;
      }
      
      const payload = JSON.parse(atob(token.split(".")[1]));
      
      // Extract roles from JWT token (role claim can be a string or array)
      let userRoles = [];
      const roleClaimKey = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role";
      if (payload[roleClaimKey]) {
        const roleClaim = payload[roleClaimKey];
        userRoles = Array.isArray(roleClaim) ? roleClaim : [roleClaim];
      }

      const userObject = {
        id: payload.sub || payload["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"],
        email: payload.email || payload["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress"],
        roles: userRoles,
        role: userRoles[0] || "Unknown", // Primary role for backward compatibility
      };
      setUser(userObject);
    } catch (error) {
      console.error("Error parsing token:", error);
      localStorage.removeItem("token");
    } finally {
      setLoading(false);
    }
  };

  const login = async (email, password) => {
    try {
      const response = await api.auth.login({ email, password });
      const token = response.token || response.accessToken;
      if (!token) {
        throw new Error("No token received from server");
      }
      localStorage.setItem("token", token);
      await getUserFromToken(token);
      return { success: true };
    } catch (error) {
      return {
        success: false,
        error: getErrorMessage(error, "Login failed. Please check your credentials and try again."),
      };
    }
  };

  const register = async (email, password, role) => {
    try {
      const response = await api.auth.register({ email, password, role });
      const token = response.token || response.accessToken;
      if (!token) {
        throw new Error("No token received from server");
      }
      localStorage.setItem("token", token);
      await getUserFromToken(token);
      return { success: true };
    } catch (error) {
      return {
        success: false,
        error: getErrorMessage(error, "Registration failed. Please try again."),
      };
    }
  };

  const logout = () => {
    localStorage.removeItem("token");
    setUser(null);
  };

  const hasRole = (requiredRoles) => {
    if (!user || !user.roles) return false;
    const roles = Array.isArray(requiredRoles)
      ? requiredRoles
      : [requiredRoles];
    return user.roles.some(userRole => roles.includes(userRole));
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
