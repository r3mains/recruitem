import axios from "axios";

const API_BASE_URL = "http://localhost:5270/api";

const api = axios.create({
  baseURL: API_BASE_URL,
  headers: { "Content-Type": "application/json" },
});

api.interceptors.request.use((config) => {
  const token = localStorage.getItem("token");
  if (token) config.headers.Authorization = `Bearer ${token}`;
  return config;
});

export const authAPI = {
  register: async (userData) => {
    const response = await api.post("/auth/register", {
      email: userData.email,
      password: userData.password,
      roleId: await getRoleIdByName(userData.role),
    });
    return {
      token: response.data.accessToken,
      user: { email: userData.email, role: userData.role },
    };
  },

  login: async (credentials) => {
    const response = await api.post("/auth/login", credentials);
    return {
      token: response.data.accessToken,
      user: await getUserDetails(),
    };
  },
};

export const jobsAPI = {
  getAll: async () => {
    const response = await api.get("/jobs");
    return response.data;
  },

  getPublic: async () => {
    const response = await api.get("/jobs/public");
    return response.data;
  },

  create: async (jobData) => {
    const response = await api.post("/jobs", jobData);
    return response.data;
  },
};

export const positionsAPI = {
  getAll: async () => {
    const response = await api.get("/positions");
    return response.data;
  },
};

export const addressesAPI = {
  getAll: async () => {
    const response = await api.get("/addresses");
    return response.data;
  },
};

export const lookupsAPI = {
  getStatusTypes: async () => {
    const response = await api.get("/lookups/status-types");
    return response.data;
  },

  getJobTypes: async () => {
    const response = await api.get("/lookups/job-types");
    return response.data;
  },
};

export const rolesAPI = {
  getAll: async () => {
    const response = await api.get("/roles");
    return response.data;
  },
};

export const usersAPI = {
  getById: async (id) => {
    const response = await api.get(`/users/${id}`);
    return response.data;
  },
};

const getRoleIdByName = async (roleName) => {
  const roles = await rolesAPI.getAll();
  const role = roles.find(
    (r) => r.name.toLowerCase() === roleName.toLowerCase()
  );
  return role ? role.id : null;
};

const getUserDetails = async () => {
  try {
    const token = localStorage.getItem("token");
    if (!token) return null;

    const payload = JSON.parse(atob(token.split(".")[1]));
    const user = await usersAPI.getById(payload.sub);
    const roles = await rolesAPI.getAll();
    const userRole = roles.find((r) => r.id === user.roleId);

    return {
      id: user.id,
      email: user.email,
      role: userRole?.name || "Unknown",
    };
  } catch (error) {
    return null;
  }
};

export default api;
