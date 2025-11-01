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
  getAll: async (statusId) => {
    const params = statusId ? `?statusId=${statusId}` : "";
    const response = await api.get(`/positions${params}`);
    return response.data;
  },

  getById: async (id) => {
    const response = await api.get(`/positions/${id}`);
    return response.data;
  },

  create: async (positionData) => {
    const response = await api.post("/positions", positionData);
    return response.data;
  },

  update: async (id, positionData) => {
    const response = await api.put(`/positions/${id}`, positionData);
    return response.data;
  },

  updateStatus: async (id, statusData) => {
    const response = await api.put(`/positions/${id}/status`, statusData);
    return response.data;
  },

  delete: async (id) => {
    await api.delete(`/positions/${id}`);
  },
};

export const skillsAPI = {
  getAll: async (search) => {
    const params = search ? `?search=${search}` : "";
    const response = await api.get(`/skills${params}`);
    return response.data;
  },

  create: async (skillData) => {
    const response = await api.post("/skills", skillData);
    return response.data;
  },

  update: async (id, skillData) => {
    const response = await api.put(`/skills/${id}`, skillData);
    return response.data;
  },

  delete: async (id) => {
    await api.delete(`/skills/${id}`);
  },
};

export const employeesAPI = {
  getAll: async () => {
    const response = await api.get("/employees");
    return response.data;
  },
};

export const candidatesAPI = {
  search: async (search, skills, page = 1, limit = 20) => {
    const params = new URLSearchParams();
    if (search) params.append("search", search);
    if (skills) params.append("skills", skills);
    params.append("page", page.toString());
    params.append("limit", limit.toString());

    const response = await api.get(`/candidates?${params}`);
    return response.data;
  },

  getById: async (id) => {
    const response = await api.get(`/candidates/${id}`);
    return response.data;
  },

  getProfile: async () => {
    const response = await api.get("/candidates/profile");
    return response.data;
  },

  create: async (candidateData) => {
    const response = await api.post("/candidates", candidateData);
    return response.data;
  },

  update: async (id, candidateData) => {
    const response = await api.put(`/candidates/${id}`, candidateData);
    return response.data;
  },

  delete: async (id) => {
    await api.delete(`/candidates/${id}`);
  },

  getSkills: async (candidateId) => {
    const response = await api.get(`/candidates/${candidateId}/skills`);
    return response.data;
  },

  addSkill: async (candidateId, skillData) => {
    const response = await api.post(
      `/candidates/${candidateId}/skills`,
      skillData
    );
    return response.data;
  },

  updateSkill: async (candidateId, skillId, skillData) => {
    const response = await api.put(
      `/candidates/${candidateId}/skills/${skillId}`,
      skillData
    );
    return response.data;
  },

  removeSkill: async (candidateId, skillId) => {
    await api.delete(`/candidates/${candidateId}/skills/${skillId}`);
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

export const applicationsAPI = {
  getAll: async (filters = {}) => {
    const params = new URLSearchParams();
    if (filters.jobId) params.append("jobId", filters.jobId);
    if (filters.candidateId) params.append("candidateId", filters.candidateId);
    if (filters.statusId) params.append("statusId", filters.statusId);

    const response = await api.get(`/jobapplications?${params.toString()}`);
    return response.data;
  },

  getById: async (id) => {
    const response = await api.get(`/jobapplications/${id}`);
    return response.data;
  },

  create: async (data) => {
    const response = await api.post("/jobapplications", data);
    return response.data;
  },

  updateStatus: async (id, data) => {
    const response = await api.put(`/jobapplications/${id}`, data);
    return response.data;
  },

  delete: async (id) => {
    await api.delete(`/jobapplications/${id}`);
  },

  getMyApplications: async () => {
    const response = await api.get("/jobapplications/my-applications");
    return response.data;
  },

  screen: async (id, data) => {
    const response = await api.post(`/jobapplications/${id}/screen`, data);
    return response.data;
  },

  getStatuses: async () => {
    const response = await api.get("/lookups/status-types");
    return response.data;
  },
};

export const screeningAPI = {
  getApplicationsForScreening: async (filters = {}) => {
    const params = new URLSearchParams();
    if (filters.jobId) params.append("jobId", filters.jobId);
    if (filters.statusId) params.append("statusId", filters.statusId);

    const response = await api.get(`/screening/applications?${params.toString()}`);
    return response.data;
  },

  screenApplication: async (applicationId, data) => {
    const response = await api.post(`/screening/screen/${applicationId}`, data);
    return response.data;
  },

  bulkScreenApplications: async (data) => {
    const response = await api.post("/screening/bulk-screen", data);
    return response.data;
  },

  shortlistApplications: async (data) => {
    const response = await api.post("/screening/shortlist", data);
    return response.data;
  },

  getShortlistedApplications: async (jobId = null) => {
    const params = new URLSearchParams();
    if (jobId) params.append("jobId", jobId);

    const response = await api.get(`/screening/shortlisted?${params.toString()}`);
    return response.data;
  },

  calculateScore: async (applicationId) => {
    const response = await api.get(`/screening/calculate-score/${applicationId}`);
    return response.data;
  },

  getApplicationsByScoreRange: async (jobId, minScore, maxScore) => {
    const params = new URLSearchParams();
    params.append("jobId", jobId);
    params.append("minScore", minScore);
    params.append("maxScore", maxScore);

    const response = await api.get(`/screening/by-score-range?${params.toString()}`);
    return response.data;
  },
};

export default api;
