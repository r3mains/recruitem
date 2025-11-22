import axios from "axios";

const API_BASE_URL = "http://localhost:5270/api/v1";

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
    try {
      const response = await api.get("/addresses");
      return response.data;
    } catch (error) {
      console.warn("Addresses endpoint not available, returning empty array");
      return [];
    }
  },

  getFromEmployees: async () => {
    try {
      const response = await api.get("/employees");
      return response.data || [];
    } catch (error) {
      console.warn("Could not get addresses from employees");
      return [];
    }
  },
};

export const lookupsAPI = {
  getStatusTypes: async () => {
    const response = await api.get("/positions/statuses");
    return response.data;
  },

  getJobTypes: async () => {
    const response = await api.get("/jobtype");
    return response.data;
  },

  getPositionStatuses: async () => {
    const response = await api.get("/positions/statuses");
    return response.data;
  },

  getJobStatuses: async () => {
    const response = await api.get("/jobs/statuses");
    return response.data;
  },

  getApplicationStatuses: async () => {
    const response = await api.get("/jobapplications/statuses");
    return response.data;
  },

  getInterviewStatuses: async () => {
    const response = await api.get("/interviews/statuses");
    return response.data;
  },

  getSkills: async (search = null) => {
    const params = search ? `?search=${search}` : "";
    const response = await api.get(`/skills${params}`);
    return response.data;
  },

  getQualifications: async () => {
    const response = await api.get("/qualifications");
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

    const response = await api.get(
      `/screening/applications?${params.toString()}`
    );
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

    const response = await api.get(
      `/screening/shortlisted?${params.toString()}`
    );
    return response.data;
  },

  calculateScore: async (applicationId) => {
    const response = await api.get(
      `/screening/calculate-score/${applicationId}`
    );
    return response.data;
  },

  getApplicationsByScoreRange: async (jobId, minScore, maxScore) => {
    const params = new URLSearchParams();
    params.append("jobId", jobId);
    params.append("minScore", minScore);
    params.append("maxScore", maxScore);

    const response = await api.get(
      `/screening/by-score-range?${params.toString()}`
    );
    return response.data;
  },
};

export const interviewsAPI = {
  getAll: async () => {
    const response = await api.get("/interviews");
    return response.data;
  },

  getById: async (id) => {
    const response = await api.get(`/interviews/${id}`);
    return response.data;
  },

  getByJobApplication: async (jobApplicationId) => {
    const response = await api.get(
      `/interviews/job-application/${jobApplicationId}`
    );
    return response.data;
  },

  create: async (interviewData) => {
    const response = await api.post("/interviews", interviewData);
    return response.data;
  },

  update: async (id, interviewData) => {
    const response = await api.put(`/interviews/${id}`, interviewData);
    return response.data;
  },

  delete: async (id) => {
    await api.delete(`/interviews/${id}`);
  },

  getSchedules: async (interviewId) => {
    const response = await api.get(`/interviews/${interviewId}/schedules`);
    return response.data;
  },

  getScheduleById: async (scheduleId) => {
    const response = await api.get(`/interviews/schedules/${scheduleId}`);
    return response.data;
  },

  createSchedule: async (scheduleData) => {
    const response = await api.post("/interviews/schedules", scheduleData);
    return response.data;
  },

  updateSchedule: async (scheduleId, scheduleData) => {
    const response = await api.put(
      `/interviews/schedules/${scheduleId}`,
      scheduleData
    );
    return response.data;
  },

  deleteSchedule: async (scheduleId) => {
    await api.delete(`/interviews/schedules/${scheduleId}`);
  },

  getFeedback: async (interviewId) => {
    const response = await api.get(`/interviews/${interviewId}/feedback`);
    return response.data;
  },

  getFeedbackById: async (feedbackId) => {
    const response = await api.get(`/interviews/feedback/${feedbackId}`);
    return response.data;
  },

  createFeedback: async (feedbackData) => {
    const response = await api.post("/interviews/feedback", feedbackData);
    return response.data;
  },

  updateFeedback: async (feedbackId, feedbackData) => {
    const response = await api.put(
      `/interviews/feedback/${feedbackId}`,
      feedbackData
    );
    return response.data;
  },

  deleteFeedback: async (feedbackId) => {
    await api.delete(`/interviews/feedback/${feedbackId}`);
  },

  getAllOnlineTests: async () => {
    const response = await api.get("/interviews/online-tests");
    return response.data;
  },

  getOnlineTestById: async (testId) => {
    const response = await api.get(`/interviews/online-tests/${testId}`);
    return response.data;
  },

  getOnlineTestsByJobApplication: async (jobApplicationId) => {
    const response = await api.get(
      `/interviews/online-tests/job-application/${jobApplicationId}`
    );
    return response.data;
  },

  createOnlineTest: async (testData) => {
    const response = await api.post("/interviews/online-tests", testData);
    return response.data;
  },

  updateOnlineTest: async (testId, testData) => {
    const response = await api.put(
      `/interviews/online-tests/${testId}`,
      testData
    );
    return response.data;
  },

  deleteOnlineTest: async (testId) => {
    await api.delete(`/interviews/online-tests/${testId}`);
  },

  getInterviewTypes: async () => {
    const response = await api.get("/interviews/types");
    return response.data;
  },

  schedule: async (interviewData) => {
    const response = await api.post("/interviews/schedule", interviewData);
    return response.data;
  },

  conduct: async (interviewId) => {
    const response = await api.post(`/interviews/${interviewId}/conduct`);
    return response.data;
  },

  scheduleOnlineTest: async (candidateId, testData) => {
    const response = await api.post(
      `/interviews/${candidateId}/online-test`,
      testData
    );
    return response.data;
  },
};

export const qualificationsAPI = {
  getAll: async (search = null, page = 1, pageSize = 10) => {
    const params = new URLSearchParams();
    if (search) params.append("search", search);
    params.append("page", page.toString());
    params.append("pageSize", pageSize.toString());

    const response = await api.get(`/qualification?${params.toString()}`);
    return response.data;
  },

  getById: async (id) => {
    const response = await api.get(`/qualification/${id}`);
    return response.data;
  },

  create: async (qualificationData) => {
    const response = await api.post("/qualification", qualificationData);
    return response.data;
  },

  update: async (id, qualificationData) => {
    const response = await api.put(`/qualification/${id}`, qualificationData);
    return response.data;
  },

  delete: async (id) => {
    await api.delete(`/qualification/${id}`);
  },

  getCount: async (search = null) => {
    const params = search ? `?search=${search}` : "";
    const response = await api.get(`/qualification/count${params}`);
    return response.data;
  },

  checkExists: async (name) => {
    const response = await api.get(
      `/qualification/exists/${encodeURIComponent(name)}`
    );
    return response.data;
  },
};

export const jobTypesAPI = {
  getAll: async (search = null, page = 1, pageSize = 10) => {
    const params = new URLSearchParams();
    if (search) params.append("search", search);
    params.append("page", page.toString());
    params.append("pageSize", pageSize.toString());

    const response = await api.get(`/jobtype?${params.toString()}`);
    return response.data;
  },

  getById: async (id) => {
    const response = await api.get(`/jobtype/${id}`);
    return response.data;
  },

  create: async (jobTypeData) => {
    const response = await api.post("/jobtype", jobTypeData);
    return response.data;
  },

  update: async (id, jobTypeData) => {
    const response = await api.put(`/jobtype/${id}`, jobTypeData);
    return response.data;
  },

  delete: async (id) => {
    await api.delete(`/jobtype/${id}`);
  },

  getCount: async (search = null) => {
    const params = search ? `?search=${search}` : "";
    const response = await api.get(`/jobtype/count${params}`);
    return response.data;
  },

  checkExists: async (type) => {
    const response = await api.get(
      `/jobtype/exists/${encodeURIComponent(type)}`
    );
    return response.data;
  },
};

export default api;
