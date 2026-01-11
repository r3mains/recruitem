import axios from "axios";
import { getErrorMessage } from "../utils/errorHandler";

const API_BASE_URL = "http://localhost:5153/api/v1";

const api = axios.create({
  baseURL: API_BASE_URL,
  headers: { "Content-Type": "application/json" },
});

api.interceptors.request.use((config) => {
  const token = localStorage.getItem("token");
  if (token) config.headers.Authorization = `Bearer ${token}`;
  return config;
});

// Response interceptor for error handling
api.interceptors.response.use(
  (response) => response,
  (error) => {
    // Handle 401 - redirect to login
    if (error.response?.status === 401) {
      const currentPath = window.location.pathname;
      if (currentPath !== "/login" && currentPath !== "/register") {
        localStorage.removeItem("token");
        window.location.href = "/login";
      }
    }
    return Promise.reject(error);
  }
);

export const authAPI = {
  register: async (userData) => {
    const response = await api.post("/auth/register", {
      email: userData.email,
      password: userData.password,
      role: userData.role || "Candidate",
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
    return response.data.jobs || response.data;
  },

  getAllPaginated: async (page = 1, pageSize = 10, search = null, statusId = null) => {
    const params = new URLSearchParams({ page, pageSize });
    if (search) params.append("search", search);
    if (statusId) params.append("statusId", statusId);
    const response = await api.get(`/jobs?${params}`);
    return response.data;
  },

  getPublic: async () => {
    const response = await api.get("/jobs/public");
    return response.data.Jobs || response.data.jobs || response.data;
  },

  getPublicPaginated: async (page = 1, pageSize = 10) => {
    const params = new URLSearchParams({ page, pageSize });
    const response = await api.get(`/jobs/public?${params}`);
    return response.data;
  },

  getById: async (id) => {
    const response = await api.get(`/jobs/${id}`);
    return response.data;
  },

  getPublicById: async (id) => {
    const response = await api.get(`/jobs/public/${id}`);
    return response.data;
  },

  create: async (jobData) => {
    const response = await api.post("/jobs", jobData);
    return response.data;
  },

  update: async (id, jobData) => {
    const response = await api.put(`/jobs/${id}`, jobData);
    return response.data;
  },

  delete: async (id) => {
    const response = await api.delete(`/jobs/${id}`);
    return response.data;
  },
};

export const positionsAPI = {
  getAll: async (statusId) => {
    const params = statusId ? `?statusId=${statusId}` : "";
    const response = await api.get(`/positions${params}`);
    return response.data.positions || response.data;
  },

  getAllPaginated: async (page = 1, pageSize = 10, search = null, statusId = null) => {
    const params = new URLSearchParams({ page, pageSize });
    if (search) params.append("search", search);
    if (statusId) params.append("statusId", statusId);
    const response = await api.get(`/positions?${params}`);
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

export const employeesAPI = {
  getAll: async () => {
    try {
      const response = await api.get("/employee");
      return response.data || [];
    } catch (error) {
      console.warn("Employees endpoint not available, returning empty array");
      return [];
    }
  },
};

export const candidatesAPI = {
  search: async (search, skills, page = 1, limit = 20) => {
    const params = new URLSearchParams();
    if (search) params.append("search", search);
    if (skills) params.append("skills", skills);
    params.append("page", page.toString());
    params.append("pageSize", limit.toString());

    const response = await api.get(`/candidate?${params}`);
    return response.data.candidates || response.data;
  },

  getById: async (id) => {
    const response = await api.get(`/candidate/${id}`);
    return response.data;
  },

  getProfile: async () => {
    const response = await api.get("/candidate/profile");
    return response.data;
  },

  create: async (candidateData) => {
    const response = await api.post("/candidate", candidateData);
    return response.data;
  },

  update: async (id, candidateData) => {
    const response = await api.put(`/candidate/${id}`, candidateData);
    return response.data;
  },

  delete: async (id) => {
    await api.delete(`/candidate/${id}`);
  },

  getSkills: async (candidateId) => {
    const response = await api.get(`/candidate/${candidateId}/skills`);
    return response.data;
  },

  addSkill: async (candidateId, skillData) => {
    const response = await api.post(
      `/candidate/${candidateId}/skills`,
      skillData
    );
    return response.data;
  },

  updateSkill: async (candidateId, skillId, skillData) => {
    const response = await api.put(
      `/candidate/${candidateId}/skills/${skillId}`,
      skillData
    );
    return response.data;
  },

  removeSkill: async (candidateId, skillId) => {
    await api.delete(`/candidate/${candidateId}/skills/${skillId}`);
  },
};

export const addressesAPI = {
  getAll: async () => {
    try {
      const response = await api.get("/address");
      return response.data;
    } catch (error) {
      console.warn("Addresses endpoint not available, returning empty array");
      return [];
    }
  },

  getFromEmployees: async () => {
    try {
      const response = await api.get("/employee");
      return response.data || [];
    } catch (error) {
      console.warn("Could not get addresses from employees");
      return [];
    }
  },
};

export const lookupsAPI = {
  getStatusTypes: async () => {
    const response = await api.get("/lookups/statuses");
    return response.data;
  },

  getJobTypes: async () => {
    const response = await api.get("/JobType");
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
    const response = await api.get("/users/roles");
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

    const response = await api.get(`/JobApplication?${params.toString()}`);
    return response.data.applications || response.data;
  },

  getById: async (id) => {
    const response = await api.get(`/JobApplication/${id}`);
    return response.data;
  },

  create: async (data) => {
    const response = await api.post("/JobApplication", data);
    return response.data;
  },

  apply: async (jobId) => {
    const response = await api.post("/JobApplication/apply", { jobId });
    return response.data;
  },

  updateStatus: async (id, data) => {
    const response = await api.put(`/JobApplication/${id}`, data);
    return response.data;
  },

  delete: async (id) => {
    await api.delete(`/JobApplication/${id}`);
  },

  getMyApplications: async () => {
    const response = await api.get("/JobApplication/my-applications");
    return response.data.applications || response.data;
  },

  screen: async (id, data) => {
    const response = await api.post(`/JobApplication/${id}/screen`, data);
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
    if (filters.positionId) params.append("positionId", filters.positionId);
    if (filters.search) params.append("search", filters.search);
    if (filters.page) params.append("page", filters.page);
    if (filters.pageSize) params.append("pageSize", filters.pageSize);

    const response = await api.get(
      `/screening/applications?${params.toString()}`
    );
    return response.data;
  },

  screenResume: async (data) => {
    const response = await api.post("/screening/screen", data);
    return response.data;
  },

  addComment: async (data) => {
    const response = await api.post("/screening/comments", data);
    return response.data;
  },

  shortlistCandidate: async (data) => {
    const response = await api.post("/screening/shortlist", data);
    return response.data;
  },

  getStatistics: async (filters = {}) => {
    const params = new URLSearchParams();
    if (filters.positionId) params.append("positionId", filters.positionId);
    if (filters.fromDate) params.append("fromDate", filters.fromDate);
    if (filters.toDate) params.append("toDate", filters.toDate);

    const response = await api.get(
      `/screening/statistics?${params.toString()}`
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

  getTypes: async () => {
    const response = await api.get("/interviews/types");
    return response.data;
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

export const emailTemplateAPI = {
  getAll: async () => {
    const response = await api.get('/emailtemplate');
    return response.data;
  },

  getById: async (id) => {
    const response = await api.get(`/emailtemplate/${id}`);
    return response.data;
  },

  getByCategory: async (category) => {
    const response = await api.get(`/emailtemplate/category/${category}`);
    return response.data;
  },

  getActive: async () => {
    const response = await api.get('/emailtemplate/active');
    return response.data;
  },

  create: async (templateData) => {
    const response = await api.post('/emailtemplate', templateData);
    return response.data;
  },

  update: async (id, templateData) => {
    const response = await api.put(`/emailtemplate/${id}`, templateData);
    return response.data;
  },

  delete: async (id) => {
    await api.delete(`/emailtemplate/${id}`);
  },

  preview: async (templateId, variables) => {
    const response = await api.post('/emailtemplate/preview', {
      templateId,
      variables
    });
    return response.data;
  },

  send: async (templateId, toEmail, toName, variables) => {
    const response = await api.post('/emailtemplate/send', {
      templateId,
      toEmail,
      toName,
      variables
    });
    return response.data;
  },
};

export const offerLetterAPI = {
  getAll: async () => {
    const response = await api.get('/offerletter');
    return response.data;
  },

  getById: async (id) => {
    const response = await api.get(`/offerletter/${id}`);
    return response.data;
  },

  getByApplication: async (applicationId) => {
    const response = await api.get(`/offerletter/application/${applicationId}`);
    return response.data;
  },

  getByStatus: async (status) => {
    const response = await api.get(`/offerletter/status/${status}`);
    return response.data;
  },

  create: async (offerData) => {
    const response = await api.post('/offerletter', offerData);
    return response.data;
  },

  update: async (id, offerData) => {
    const response = await api.put(`/offerletter/${id}`, offerData);
    return response.data;
  },

  delete: async (id) => {
    await api.delete(`/offerletter/${id}`);
  },

  generate: async (offerLetterId, companyDetails) => {
    const response = await api.post('/offerletter/generate', {
      offerLetterId,
      ...companyDetails
    });
    return response.data;
  },

  send: async (offerLetterId, companyDetails) => {
    const response = await api.post('/offerletter/send', {
      offerLetterId,
      ...companyDetails
    });
    return response.data;
  },

  download: async (id) => {
    const response = await api.get(`/offerletter/${id}/download`, {
      responseType: 'blob'
    });
    return response.data;
  },

  accept: async (offerLetterId) => {
    const response = await api.post('/offerletter/accept', { offerLetterId });
    return response.data;
  },

  reject: async (offerLetterId, reason) => {
    const response = await api.post('/offerletter/reject', {
      offerLetterId,
      reason
    });
    return response.data;
  },
};

export const reportsAPI = {
  getDashboardStats: async () => {
    const response = await api.get('/reports/dashboard');
    return response.data;
  },

  getRecruitmentPipeline: async () => {
    const response = await api.get('/reports/pipeline');
    return response.data;
  },

  getJobStats: async (startDate, endDate) => {
    const params = {};
    if (startDate) params.startDate = startDate;
    if (endDate) params.endDate = endDate;
    const response = await api.get('/reports/job-stats', { params });
    return response.data;
  },

  getRecruiterPerformance: async (startDate, endDate) => {
    const params = {};
    if (startDate) params.startDate = startDate;
    if (endDate) params.endDate = endDate;
    const response = await api.get('/reports/recruiter-performance', { params });
    return response.data;
  },

  getTimeToHire: async (startDate, endDate) => {
    const params = {};
    if (startDate) params.startDate = startDate;
    if (endDate) params.endDate = endDate;
    const response = await api.get('/reports/time-to-hire', { params });
    return response.data;
  },

  getStatusDistribution: async () => {
    const response = await api.get('/reports/status-distribution');
    return response.data;
  },

  getInterviewStats: async (startDate, endDate) => {
    const params = {};
    if (startDate) params.startDate = startDate;
    if (endDate) params.endDate = endDate;
    const response = await api.get('/reports/interview-stats', { params });
    return response.data;
  },

  getSourceAnalysis: async () => {
    const response = await api.get('/reports/source-analysis');
    return response.data;
  },

  getMonthlyTrends: async (months = 12) => {
    const response = await api.get('/reports/monthly-trends', {
      params: { months }
    });
    return response.data;
  },

  getSkillDemand: async () => {
    const response = await api.get('/reports/skill-demand');
    return response.data;
  },

  getApplicationFunnel: async (jobId) => {
    const params = jobId ? { jobId } : {};
    const response = await api.get('/reports/application-funnel', { params });
    return response.data;
  },

  getExperienceWiseCandidates: async () => {
    const response = await api.get('/reports/experience-wise-candidates');
    return response.data;
  },

  getCollegeWiseReport: async () => {
    const response = await api.get('/reports/college-wise');
    return response.data;
  },
};

export const documentAPI = {
  getAll: async () => {
    const response = await api.get('/documents');
    return response.data;
  },

  getById: async (id) => {
    const response = await api.get(`/documents/${id}`);
    return response.data;
  },

  getByCandidate: async (candidateId) => {
    const response = await api.get(`/documents/candidate/${candidateId}`);
    return response.data;
  },

  upload: async (formData) => {
    const response = await api.post('/documents/upload', formData, {
      headers: {
        'Content-Type': 'multipart/form-data'
      }
    });
    return response.data;
  },

  update: async (id, documentData) => {
    const response = await api.put(`/documents/${id}`, documentData);
    return response.data;
  },

  delete: async (id) => {
    await api.delete(`/documents/${id}`);
  },

  download: async (id) => {
    const response = await api.get(`/documents/${id}/download`, {
      responseType: 'blob'
    });
    return response.data;
  },

  getDocumentTypes: async () => {
    const response = await api.get('/documents/types');
    return response.data;
  },

  createDocumentType: async (typeData) => {
    const response = await api.post('/documents/types', typeData);
    return response.data;
  },

  updateDocumentType: async (id, typeData) => {
    const response = await api.put(`/documents/types/${id}`, typeData);
    return response.data;
  },

  deleteDocumentType: async (id) => {
    await api.delete(`/documents/types/${id}`);
  },
};

export const eventAPI = {
  getAll: async () => {
    const response = await api.get('/events');
    return response.data;
  },

  getById: async (id) => {
    const response = await api.get(`/events/${id}`);
    return response.data;
  },

  create: async (eventData) => {
    const response = await api.post('/events', eventData);
    return response.data;
  },

  update: async (id, eventData) => {
    const response = await api.put(`/events/${id}`, eventData);
    return response.data;
  },

  delete: async (id) => {
    await api.delete(`/events/${id}`);
  },

  getEventCandidates: async (eventId) => {
    const response = await api.get(`/events/${eventId}/candidates`);
    return response.data;
  },

  registerCandidate: async (registrationData) => {
    const response = await api.post('/events/register-candidate', registrationData);
    return response.data;
  },

  updateCandidateStatus: async (eventCandidateId, statusData) => {
    const response = await api.put(`/events/event-candidates/${eventCandidateId}/status`, statusData);
    return response.data;
  },

  removeCandidate: async (eventCandidateId) => {
    await api.delete(`/events/event-candidates/${eventCandidateId}`);
  },
};

export const verificationAPI = {
  getAll: async () => {
    const response = await api.get('/verifications');
    return response.data;
  },

  getById: async (id) => {
    const response = await api.get(`/verifications/${id}`);
    return response.data;
  },

  getByCandidate: async (candidateId) => {
    const response = await api.get(`/verifications/candidate/${candidateId}`);
    return response.data;
  },

  create: async (verificationData) => {
    const response = await api.post('/verifications', verificationData);
    return response.data;
  },

  update: async (id, verificationData) => {
    const response = await api.put(`/verifications/${id}`, verificationData);
    return response.data;
  },

  delete: async (id) => {
    await api.delete(`/verifications/${id}`);
  },
};

export const skillsAPI = {
  getAll: async (page = 1, pageSize = 10, search = '') => {
    const params = { page, pageSize };
    if (search) params.search = search;
    const response = await api.get('/skills', { params });
    return response.data;
  },

  getById: async (id) => {
    const response = await api.get(`/skills/${id}`);
    return response.data;
  },

  create: async (skillData) => {
    const response = await api.post('/skills', skillData);
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

export const qualificationsAPI = {
  getAll: async (page = 1, pageSize = 10, search = '') => {
    const params = { page, pageSize };
    if (search) params.search = search;
    const response = await api.get('/Qualification', { params });
    return response.data;
  },

  getById: async (id) => {
    const response = await api.get(`/Qualification/${id}`);
    return response.data;
  },

  create: async (qualificationData) => {
    const response = await api.post('/Qualification', qualificationData);
    return response.data;
  },

  update: async (id, qualificationData) => {
    const response = await api.put(`/Qualification/${id}`, qualificationData);
    return response.data;
  },

  delete: async (id) => {
    await api.delete(`/Qualification/${id}`);
  },

  getStatistics: async () => {
    const response = await api.get('/Qualification/statistics');
    return response.data;
  },
};

export const usersAPI = {
  getAll: async (searchTerm = '', role = '', page = 1, pageSize = 10) => {
    const params = { page, pageSize };
    if (searchTerm) params.searchTerm = searchTerm;
    if (role) params.role = role;
    const response = await api.get('/users', { params });
    return response.data;
  },

  getById: async (id) => {
    const response = await api.get(`/users/${id}`);
    return response.data;
  },

  create: async (userData) => {
    const response = await api.post('/users', userData);
    return response.data;
  },

  update: async (id, userData) => {
    const response = await api.put(`/users/${id}`, userData);
    return response.data;
  },

  delete: async (id) => {
    const response = await api.delete(`/users/${id}`);
    return response.data;
  },

  restore: async (id) => {
    const response = await api.post(`/users/${id}/restore`);
    return response.data;
  },

  getUserRoles: async (id) => {
    const response = await api.get(`/users/${id}/roles`);
    return response.data;
  },

  updateRoles: async (id, roles) => {
    const response = await api.put(`/users/${id}/roles`, roles);
    return response.data;
  },

  lockUser: async (id, lockoutEnd = null) => {
    const params = {};
    if (lockoutEnd) params.lockoutEnd = lockoutEnd;
    const response = await api.post(`/users/${id}/lock`, {}, { params });
    return response.data;
  },

  unlockUser: async (id) => {
    const response = await api.post(`/users/${id}/unlock`);
    return response.data;
  },

  confirmEmail: async (id) => {
    const response = await api.post(`/users/${id}/confirm-email`);
    return response.data;
  },

  getStatistics: async () => {
    const response = await api.get('/users/statistics');
    return response.data;
  },

  getAllRoles: async () => {
    const response = await api.get('/users/roles');
    return response.data;
  },
};

export const jobTypesAPI = {
  getAll: async (page = 1, pageSize = 10, search = '') => {
    const params = { page, pageSize };
    if (search) params.search = search;
    const response = await api.get('/JobType', { params });
    return response.data;
  },

  getById: async (id) => {
    const response = await api.get(`/JobType/${id}`);
    return response.data;
  },

  create: async (jobTypeData) => {
    const response = await api.post('/JobType', jobTypeData);
    return response.data;
  },

  update: async (id, jobTypeData) => {
    const response = await api.put(`/JobType/${id}`, jobTypeData);
    return response.data;
  },

  delete: async (id) => {
    await api.delete(`/JobType/${id}`);
  },
};

export const profileAPI = {
  // Employee Profile
  getEmployeeProfile: async () => {
    const response = await api.get('/profiles/employee');
    return response.data;
  },

  updateEmployeeProfile: async (profileData) => {
    const response = await api.put('/profiles/employee', profileData);
    return response.data;
  },

  // Candidate Profile
  getCandidateProfile: async () => {
    const response = await api.get('/profiles/candidate');
    return response.data;
  },

  updateCandidateProfile: async (profileData) => {
    const response = await api.put('/profiles/candidate', profileData);
    return response.data;
  },
};

export const resumeAPI = {
  parseResume: async (file) => {
    const formData = new FormData();
    formData.append('ResumeFile', file);
    
    const response = await api.post('/resume/parse', formData, {
      headers: {
        'Content-Type': 'multipart/form-data',
      },
    });
    return response.data;
  },
};

export const exportAPI = {
  exportJobs: async (search = '', statusId = '', jobTypeId = '') => {
    const params = {};
    if (search) params.search = search;
    if (statusId) params.statusId = statusId;
    if (jobTypeId) params.jobTypeId = jobTypeId;
    
    const response = await api.get('/export/jobs', { 
      params,
      responseType: 'blob' 
    });
    return response.data;
  },

  exportCandidates: async (search = '') => {
    const params = {};
    if (search) params.search = search;
    
    const response = await api.get('/export/candidates', { 
      params,
      responseType: 'blob' 
    });
    return response.data;
  },

  exportApplications: async (search = '', jobId = '', candidateId = '', statusId = '') => {
    const params = {};
    if (search) params.search = search;
    if (jobId) params.jobId = jobId;
    if (candidateId) params.candidateId = candidateId;
    if (statusId) params.statusId = statusId;
    
    const response = await api.get('/export/applications', { 
      params,
      responseType: 'blob' 
    });
    return response.data;
  },

  exportInterviews: async () => {
    const response = await api.get('/export/interviews', { 
      responseType: 'blob' 
    });
    return response.data;
  },
};

export default api;
