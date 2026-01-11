import React, { useState, useEffect } from "react";
import { interviewsAPI, applicationsAPI, employeesAPI, skillsAPI } from "../services/api";
import { useAuth } from "../contexts/AuthContext";
import toast, { Toaster } from "react-hot-toast";

const InterviewsPage = () => {
  const [interviews, setInterviews] = useState([]);
  const [loading, setLoading] = useState(true);
  const [search, setSearch] = useState("");
  const [statusFilter, setStatusFilter] = useState("");
  const [selectedInterview, setSelectedInterview] = useState(null);
  const [showModal, setShowModal] = useState(false);
  const [modalMode, setModalMode] = useState("view"); // view, create, addFeedback, schedule
  const [applications, setApplications] = useState([]);
  const [employees, setEmployees] = useState([]);
  const [skills, setSkills] = useState([]);
  const [interviewTypes, setInterviewTypes] = useState([]);
  const { hasRole } = useAuth();

  useEffect(() => {
    loadInitialData();
  }, []);

  const loadInitialData = async () => {
    try {
      const [interviewsData, appsData, empsData, skillsData, typesData] = await Promise.all([
        interviewsAPI.getAll(),
        applicationsAPI.getAll({}),
        employeesAPI.getAll(),
        skillsAPI.getAll(),
        interviewsAPI.getTypes(),
      ]);
      setInterviews(interviewsData || []);
      setApplications(appsData.applications || appsData || []);
      setEmployees(empsData || []);
      setSkills(skillsData.Skills || skillsData.skills || skillsData || []);
      setInterviewTypes(typesData || []);
    } catch (error) {
      console.error("Error loading initial data:", error);
      toast.error("Failed to load data");
    } finally {
      setLoading(false);
    }
  };

  const loadInterviews = async () => {
    try {
      setLoading(true);
      const data = await interviewsAPI.getAll();
      setInterviews(data || []);
    } catch (error) {
      console.error("Error loading interviews:", error);
      toast.error("Failed to load interviews");
    } finally {
      setLoading(false);
    }
  };

  const openCreateModal = () => {
    setSelectedInterview(null);
    setModalMode("create");
    setShowModal(true);
  };

  const openScheduleModal = (interview) => {
    setSelectedInterview(interview);
    setModalMode("schedule");
    setShowModal(true);
  };

  const openFeedbackModal = (interview) => {
    setSelectedInterview(interview);
    setModalMode("addFeedback");
    setShowModal(true);
  };

  const viewDetails = async (interviewId) => {
    try {
      const interview = await interviewsAPI.getById(interviewId);
      setSelectedInterview(interview);
      setModalMode("view");
      setShowModal(true);
    } catch (error) {
      console.error("Error loading interview:", error);
      toast.error("Failed to load interview details");
    }
  };

  const handleDelete = async (interviewId) => {
    if (!window.confirm("Are you sure you want to delete this interview?"))
      return;

    try {
      await interviewsAPI.delete(interviewId);
      toast.success("Interview deleted successfully");
      loadInterviews();
    } catch (error) {
      console.error("Error deleting interview:", error);
      toast.error("Failed to delete interview");
    }
  };

  const closeModal = () => {
    setShowModal(false);
    setSelectedInterview(null);
  };

  if (!hasRole(["Admin", "Recruiter", "HR", "Interviewer"])) {
    return (
      <div className="p-6">
        <div className="text-center">
          <h1 className="text-2xl font-bold text-red-600">Access Denied</h1>
          <p className="text-gray-600">
            You don't have permission to manage interviews.
          </p>
        </div>
      </div>
    );
  }

  const filteredInterviews = interviews.filter((interview) => {
    const matchesSearch =
      !search ||
      interview.candidateName?.toLowerCase().includes(search.toLowerCase()) ||
      interview.jobTitle?.toLowerCase().includes(search.toLowerCase());
    const matchesStatus = !statusFilter || interview.status === statusFilter;
    return matchesSearch && matchesStatus;
  });

  const getStatusBadge = (status) => {
    const colors = {
      Scheduled: "bg-blue-100 text-blue-800",
      Completed: "bg-green-100 text-green-800",
      Cancelled: "bg-red-100 text-red-800",
      Pending: "bg-yellow-100 text-yellow-800",
    };
    return colors[status] || "bg-gray-100 text-gray-800";
  };

  return (
    <div className="p-6">
      <Toaster position="top-right" />

      <div className="mb-6 flex justify-between items-center">
        <h1 className="text-2xl font-bold">Interviews</h1>
        <button
          onClick={openCreateModal}
          className="bg-blue-600 text-white px-4 py-2 rounded hover:bg-blue-700"
        >
          + Schedule Interview
        </button>
      </div>

      {/* Search/Filter */}
      <div className="bg-white p-4 rounded-lg shadow mb-6">
        <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
          <div className="md:col-span-2">
            <label className="block text-sm font-medium mb-2">Search</label>
            <input
              type="text"
              value={search}
              onChange={(e) => setSearch(e.target.value)}
              placeholder="Candidate or job title..."
              className="w-full border rounded px-3 py-2"
            />
          </div>
          <div>
            <label className="block text-sm font-medium mb-2">Status</label>
            <select
              value={statusFilter}
              onChange={(e) => setStatusFilter(e.target.value)}
              className="w-full border rounded px-3 py-2"
            >
              <option value="">All Status</option>
              <option value="Scheduled">Scheduled</option>
              <option value="Completed">Completed</option>
              <option value="Cancelled">Cancelled</option>
              <option value="Pending">Pending</option>
            </select>
          </div>
        </div>
      </div>

      {loading ? (
        <div className="text-center py-8">
          <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-blue-600 mx-auto"></div>
        </div>
      ) : (
        <>
          <div className="mb-4 text-sm text-gray-600">
            Found {filteredInterviews.length} interviews
          </div>

          {/* Interviews Table */}
          <div className="bg-white shadow rounded-lg overflow-hidden">
            <table className="min-w-full divide-y divide-gray-200">
              <thead className="bg-gray-50">
                <tr>
                  <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">
                    Candidate
                  </th>
                  <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">
                    Job
                  </th>
                  <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">
                    Type
                  </th>
                  <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">
                    Round
                  </th>
                  <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">
                    Status
                  </th>
                  <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">
                    Interviewers
                  </th>
                  <th className="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase">
                    Actions
                  </th>
                </tr>
              </thead>
              <tbody className="bg-white divide-y divide-gray-200">
                {filteredInterviews.map((interview) => (
                  <tr key={interview.id} className="hover:bg-gray-50">
                    <td className="px-6 py-4">
                      <div className="font-medium text-gray-900">
                        {interview.candidateName || "N/A"}
                      </div>
                    </td>
                    <td className="px-6 py-4">
                      <div className="text-sm text-gray-900">
                        {interview.jobTitle || "N/A"}
                      </div>
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                      {interview.interviewType || "N/A"}
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                      Round {interview.roundNumber}
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap">
                      <span
                        className={`px-2 py-1 text-xs rounded-full ${getStatusBadge(
                          interview.status
                        )}`}
                      >
                        {interview.status || "Pending"}
                      </span>
                    </td>
                    <td className="px-6 py-4 text-sm text-gray-500">
                      {interview.interviewers?.length || 0}
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap text-right text-sm font-medium">
                      <button
                        onClick={() => viewDetails(interview.id)}
                        className="text-blue-600 hover:text-blue-900 mr-3"
                      >
                        View
                      </button>
                      <button
                        onClick={() => openScheduleModal(interview)}
                        className="text-purple-600 hover:text-purple-900 mr-3"
                      >
                        Schedule
                      </button>
                      <button
                        onClick={() => openFeedbackModal(interview)}
                        className="text-green-600 hover:text-green-900 mr-3"
                      >
                        Feedback
                      </button>
                      <button
                        onClick={() => handleDelete(interview.id)}
                        className="text-red-600 hover:text-red-900"
                      >
                        Delete
                      </button>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        </>
      )}

      {/* Modals */}
      {showModal && modalMode === "create" && (
        <CreateInterviewModal
          applications={applications}
          employees={employees}
          interviewTypes={interviewTypes}
          onClose={closeModal}
          onSuccess={() => {
            closeModal();
            loadInterviews();
          }}
        />
      )}

      {showModal && modalMode === "schedule" && (
        <ScheduleInterviewModal
          interview={selectedInterview}
          onClose={closeModal}
          onSuccess={() => {
            closeModal();
            loadInterviews();
          }}
        />
      )}

      {showModal && modalMode === "addFeedback" && (
        <AddFeedbackModal
          interview={selectedInterview}
          skills={skills}
          onClose={closeModal}
          onSuccess={() => {
            closeModal();
            loadInterviews();
          }}
        />
      )}

      {showModal && modalMode === "view" && (
        <ViewInterviewModal interview={selectedInterview} onClose={closeModal} />
      )}
    </div>
  );
};

// Create Interview Modal
const CreateInterviewModal = ({ applications, employees, interviewTypes, onClose, onSuccess }) => {
  const [formData, setFormData] = useState({
    jobApplicationId: "",
    interviewTypeId: "",
    roundNumber: 1,
    interviewerIds: [],
  });
  const [saving, setSaving] = useState(false);

  const handleSubmit = async (e) => {
    e.preventDefault();
    setSaving(true);

    try {
      await interviewsAPI.create(formData);
      toast.success("Interview scheduled successfully");
      onSuccess();
    } catch (error) {
      console.error("Error creating interview:", error);
      toast.error(
        error.response?.data?.message || "Failed to schedule interview"
      );
    } finally {
      setSaving(false);
    }
  };

  const toggleInterviewer = (interviewerId) => {
    setFormData((prev) => ({
      ...prev,
      interviewerIds: prev.interviewerIds.includes(interviewerId)
        ? prev.interviewerIds.filter((id) => id !== interviewerId)
        : [...prev.interviewerIds, interviewerId],
    }));
  };

  return (
    <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
      <div className="bg-white p-6 rounded-lg w-11/12 md:w-2/3 max-h-screen overflow-y-auto">
        <div className="flex justify-between items-center mb-4">
          <h2 className="text-xl font-bold">Schedule Interview</h2>
          <button
            onClick={onClose}
            className="text-gray-500 hover:text-gray-700 text-2xl"
          >
            ×
          </button>
        </div>

        <form onSubmit={handleSubmit} className="space-y-4">
          <div>
            <label className="block text-sm font-medium mb-1">
              Application *
            </label>
            <select
              value={formData.jobApplicationId}
              onChange={(e) =>
                setFormData({ ...formData, jobApplicationId: e.target.value })
              }
              required
              className="w-full border rounded px-3 py-2"
            >
              <option value="">Select Application</option>
              {applications.map((app) => (
                <option key={app.id} value={app.id}>
                  {app.candidateName} - {app.jobTitle}
                </option>
              ))}
            </select>
          </div>

          <div>
            <label className="block text-sm font-medium mb-1">
              Interview Type *
            </label>
            <select
              value={formData.interviewTypeId}
              onChange={(e) =>
                setFormData({ ...formData, interviewTypeId: e.target.value })
              }
              required
              className="w-full border rounded px-3 py-2"
            >
              <option value="">Select Type</option>
              {interviewTypes.map((type) => (
                <option key={type.id} value={type.id}>
                  {type.type}
                </option>
              ))}
            </select>
          </div>

          <div>
            <label className="block text-sm font-medium mb-1">
              Round Number *
            </label>
            <input
              type="number"
              value={formData.roundNumber}
              onChange={(e) =>
                setFormData({ ...formData, roundNumber: parseInt(e.target.value) })
              }
              required
              min="1"
              max="10"
              className="w-full border rounded px-3 py-2"
            />
          </div>

          <div>
            <label className="block text-sm font-medium mb-1">
              Interviewers
            </label>
            <div className="border rounded p-3 max-h-48 overflow-y-auto">
              {employees.map((emp) => (
                <label
                  key={emp.id}
                  className="flex items-center gap-2 py-1 cursor-pointer hover:bg-gray-50"
                >
                  <input
                    type="checkbox"
                    checked={formData.interviewerIds.includes(emp.id)}
                    onChange={() => toggleInterviewer(emp.id)}
                    className="rounded"
                  />
                  <span>{emp.fullName || emp.email}</span>
                </label>
              ))}
            </div>
            <div className="mt-2 text-sm text-gray-600">
              {formData.interviewerIds.length} interviewer(s) selected
            </div>
          </div>

          <div className="flex justify-end gap-2 pt-4">
            <button
              type="button"
              onClick={onClose}
              className="px-4 py-2 border rounded hover:bg-gray-50"
            >
              Cancel
            </button>
            <button
              type="submit"
              disabled={saving}
              className="bg-blue-600 text-white px-4 py-2 rounded hover:bg-blue-700 disabled:opacity-50"
            >
              {saving ? "Scheduling..." : "Schedule"}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
};

// Schedule Interview Modal
const ScheduleInterviewModal = ({ interview, onClose, onSuccess }) => {
  const [formData, setFormData] = useState({
    interviewId: interview.id,
    scheduledAt: "",
    location: "",
    meetingLink: "",
  });
  const [saving, setSaving] = useState(false);

  const handleSubmit = async (e) => {
    e.preventDefault();
    setSaving(true);

    try {
      // Convert datetime-local to ISO string for backend
      const payload = {
        ...formData,
        scheduledAt: new Date(formData.scheduledAt).toISOString(),
      };
      console.log("Scheduling with payload:", payload);
      await interviewsAPI.createSchedule(payload);
      toast.success("Interview scheduled successfully");
      onSuccess();
    } catch (error) {
      console.error("Error scheduling interview:", error);
      toast.error(
        error.response?.data?.message || "Failed to schedule interview"
      );
    } finally {
      setSaving(false);
    }
  };

  return (
    <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
      <div className="bg-white p-6 rounded-lg w-11/12 md:w-1/2">
        <div className="flex justify-between items-center mb-4">
          <h2 className="text-xl font-bold">Add Schedule</h2>
          <button
            onClick={onClose}
            className="text-gray-500 hover:text-gray-700 text-2xl"
          >
            ×
          </button>
        </div>

        <div className="mb-4 p-3 bg-gray-50 border border-gray-200 rounded">
          <p className="text-sm">
            <span className="font-medium">Candidate:</span>{" "}
            {interview.candidateName}
          </p>
          <p className="text-sm">
            <span className="font-medium">Job:</span> {interview.jobTitle}
          </p>
          <p className="text-sm">
            <span className="font-medium">Type:</span> {interview.interviewType}
          </p>
        </div>

        <form onSubmit={handleSubmit} className="space-y-4">
          <div>
            <label className="block text-sm font-medium mb-1">
              Schedule Date & Time *
            </label>
            <input
              type="datetime-local"
              value={formData.scheduledAt}
              onChange={(e) =>
                setFormData({ ...formData, scheduledAt: e.target.value })
              }
              required
              className="w-full border rounded px-3 py-2"
            />
          </div>

          <div>
            <label className="block text-sm font-medium mb-1">Location</label>
            <input
              type="text"
              value={formData.location}
              onChange={(e) =>
                setFormData({ ...formData, location: e.target.value })
              }
              placeholder="e.g., Office Room 301, Virtual"
              className="w-full border rounded px-3 py-2"
            />
          </div>

          <div>
            <label className="block text-sm font-medium mb-1">
              Meeting Link
            </label>
            <input
              type="url"
              value={formData.meetingLink}
              onChange={(e) =>
                setFormData({ ...formData, meetingLink: e.target.value })
              }
              placeholder="e.g., https://meet.google.com/abc-defg-hij"
              className="w-full border rounded px-3 py-2"
            />
          </div>

          <div className="flex justify-end gap-2 pt-4">
            <button
              type="button"
              onClick={onClose}
              className="px-4 py-2 border rounded hover:bg-gray-50"
            >
              Cancel
            </button>
            <button
              type="submit"
              disabled={saving}
              className="bg-blue-600 text-white px-4 py-2 rounded hover:bg-blue-700 disabled:opacity-50"
            >
              {saving ? "Saving..." : "Add Schedule"}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
};

// Add Feedback Modal
const AddFeedbackModal = ({ interview, skills, onClose, onSuccess }) => {
  const [formData, setFormData] = useState({
    interviewId: interview.id,
    forSkill: "",
    rating: 5,
    feedback: "",
  });
  const [saving, setSaving] = useState(false);

  const handleSubmit = async (e) => {
    e.preventDefault();
    setSaving(true);

    try {
      await interviewsAPI.createFeedback(formData);
      toast.success("Feedback added successfully");
      onSuccess();
    } catch (error) {
      console.error("Error adding feedback:", error);
      toast.error(error.response?.data?.message || "Failed to add feedback");
    } finally {
      setSaving(false);
    }
  };

  return (
    <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
      <div className="bg-white p-6 rounded-lg w-11/12 md:w-1/2">
        <div className="flex justify-between items-center mb-4">
          <h2 className="text-xl font-bold">Add Feedback</h2>
          <button
            onClick={onClose}
            className="text-gray-500 hover:text-gray-700 text-2xl"
          >
            ×
          </button>
        </div>

        <div className="mb-4 p-3 bg-gray-50 border border-gray-200 rounded">
          <p className="text-sm">
            <span className="font-medium">Candidate:</span>{" "}
            {interview.candidateName}
          </p>
          <p className="text-sm">
            <span className="font-medium">Job:</span> {interview.jobTitle}
          </p>
          <p className="text-sm">
            <span className="font-medium">Round:</span> {interview.roundNumber}
          </p>
        </div>

        <form onSubmit={handleSubmit} className="space-y-4">
          <div>
            <label className="block text-sm font-medium mb-1">Skill *</label>
            <select
              value={formData.forSkill}
              onChange={(e) =>
                setFormData({ ...formData, forSkill: e.target.value })
              }
              required
              className="w-full border rounded px-3 py-2"
            >
              <option value="">Select Skill</option>
              {skills && skills.length > 0 ? (
                skills.map((skill) => (
                  <option key={skill.id} value={skill.id}>
                    {skill.skillName}
                  </option>
                ))
              ) : (
                <option disabled>No skills available</option>
              )}
            </select>
          </div>

          <div>
            <label className="block text-sm font-medium mb-1">
              Rating (1-5) *
            </label>
            <input
              type="number"
              value={formData.rating}
              onChange={(e) =>
                setFormData({ ...formData, rating: parseInt(e.target.value) })
              }
              required
              min="1"
              max="5"
              className="w-full border rounded px-3 py-2"
            />
            <p className="text-xs text-gray-500 mt-1">Rate from 1 (poor) to 5 (excellent)</p>
          </div>

          <div>
            <label className="block text-sm font-medium mb-1">Feedback</label>
            <textarea
              value={formData.feedback}
              onChange={(e) =>
                setFormData({ ...formData, feedback: e.target.value })
              }
              rows="4"
              placeholder="Provide detailed feedback about the candidate's performance..."
              className="w-full border rounded px-3 py-2"
            />
          </div>

          <div className="flex justify-end gap-2 pt-4">
            <button
              type="button"
              onClick={onClose}
              className="px-4 py-2 border rounded hover:bg-gray-50"
            >
              Cancel
            </button>
            <button
              type="submit"
              disabled={saving}
              className="bg-blue-600 text-white px-4 py-2 rounded hover:bg-blue-700 disabled:opacity-50"
            >
              {saving ? "Saving..." : "Add Feedback"}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
};

// View Interview Modal
const ViewInterviewModal = ({ interview, onClose }) => {
  return (
    <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
      <div className="bg-white p-6 rounded-lg w-11/12 md:w-2/3 max-h-screen overflow-y-auto">
        <div className="flex justify-between items-center mb-4">
          <h2 className="text-xl font-bold">Interview Details</h2>
          <button
            onClick={onClose}
            className="text-gray-500 hover:text-gray-700 text-2xl"
          >
            ×
          </button>
        </div>

        <div className="space-y-6">
          {/* Basic Info */}
          <div>
            <h3 className="text-lg font-semibold mb-3">Basic Information</h3>
            <div className="grid grid-cols-2 gap-4 text-sm">
              <div>
                <span className="font-medium">Candidate:</span>{" "}
                {interview.candidateName}
              </div>
              <div>
                <span className="font-medium">Job:</span> {interview.jobTitle}
              </div>
              <div>
                <span className="font-medium">Type:</span>{" "}
                {interview.interviewType}
              </div>
              <div>
                <span className="font-medium">Round:</span>{" "}
                {interview.roundNumber}
              </div>
              <div>
                <span className="font-medium">Status:</span>{" "}
                <span
                  className={`px-2 py-1 text-xs rounded ${
                    interview.status === "Completed"
                      ? "bg-green-100 text-green-800"
                      : interview.status === "Cancelled"
                      ? "bg-red-100 text-red-800"
                      : "bg-blue-100 text-blue-800"
                  }`}
                >
                  {interview.status}
                </span>
              </div>
              <div>
                <span className="font-medium">Created:</span>{" "}
                {new Date(interview.createdAt).toLocaleString()}
              </div>
            </div>
          </div>

          {/* Interviewers */}
          {interview.interviewers && interview.interviewers.length > 0 && (
            <div>
              <h3 className="text-lg font-semibold mb-3">Interviewers</h3>
              <div className="space-y-2">
                {interview.interviewers.map((interviewer) => (
                  <div
                    key={interviewer.id}
                    className="p-3 bg-gray-50 rounded border border-gray-200"
                  >
                    <p className="text-sm font-medium">
                      {interviewer.interviewerName}
                    </p>
                    <p className="text-xs text-gray-500">
                      {interviewer.interviewerEmail}
                    </p>
                  </div>
                ))}
              </div>
            </div>
          )}

          {/* Schedules */}
          {interview.schedules && interview.schedules.length > 0 && (
            <div>
              <h3 className="text-lg font-semibold mb-3">Schedules</h3>
              <div className="space-y-2">
                {interview.schedules.map((schedule) => (
                  <div
                    key={schedule.id}
                    className="p-3 bg-blue-50 rounded border border-blue-200"
                  >
                    <p className="text-sm">
                      <span className="font-medium">Time:</span>{" "}
                      {new Date(schedule.scheduledAt).toLocaleString()}
                    </p>
                    {schedule.location && (
                      <p className="text-sm">
                        <span className="font-medium">Location:</span>{" "}
                        {schedule.location}
                      </p>
                    )}
                    {schedule.meetingLink && (
                      <p className="text-sm">
                        <span className="font-medium">Link:</span>{" "}
                        <a
                          href={schedule.meetingLink}
                          target="_blank"
                          rel="noopener noreferrer"
                          className="text-blue-600 hover:underline"
                        >
                          {schedule.meetingLink}
                        </a>
                      </p>
                    )}
                    <p className="text-xs text-gray-500 mt-1">
                      Created by {schedule.createdByName}
                    </p>
                  </div>
                ))}
              </div>
            </div>
          )}

          {/* Feedbacks */}
          {interview.feedbacks && interview.feedbacks.length > 0 && (
            <div>
              <h3 className="text-lg font-semibold mb-3">Feedback</h3>
              <div className="space-y-2">
                {interview.feedbacks.map((feedback) => (
                  <div
                    key={feedback.id}
                    className="p-3 bg-green-50 rounded border border-green-200"
                  >
                    <div className="flex justify-between items-start">
                      <div>
                        <p className="text-sm font-medium">
                          {feedback.skillName}
                        </p>
                        <p className="text-sm">Rating: {feedback.rating}/10</p>
                      </div>
                      <span className="text-xs text-gray-500">
                        By {feedback.feedbackByName}
                      </span>
                    </div>
                    {feedback.feedback && (
                      <p className="text-sm mt-2">{feedback.feedback}</p>
                    )}
                    <p className="text-xs text-gray-500 mt-1">
                      {new Date(feedback.createdAt).toLocaleString()}
                    </p>
                  </div>
                ))}
              </div>
            </div>
          )}
        </div>

        <div className="flex justify-end pt-4">
          <button
            onClick={onClose}
            className="px-4 py-2 bg-gray-500 text-white rounded hover:bg-gray-600"
          >
            Close
          </button>
        </div>
      </div>
    </div>
  );
};

export default InterviewsPage;
