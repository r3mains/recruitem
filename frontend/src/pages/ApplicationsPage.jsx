import React, { useState, useEffect } from "react";
import { applicationsAPI, jobsAPI, candidatesAPI } from "../services/api";
import { useAuth } from "../contexts/AuthContext";
import toast, { Toaster } from "react-hot-toast";

const ApplicationsPage = () => {
  const [applications, setApplications] = useState([]);
  const [loading, setLoading] = useState(true);
  const [search, setSearch] = useState("");
  const [jobFilter, setJobFilter] = useState("");
  const [candidateFilter, setCandidateFilter] = useState("");
  const [statusFilter, setStatusFilter] = useState("");
  const [page, setPage] = useState(1);
  const [totalPages, setTotalPages] = useState(1);
  const [totalCount, setTotalCount] = useState(0);
  const [selectedApplication, setSelectedApplication] = useState(null);
  const [showModal, setShowModal] = useState(false);
  const [modalMode, setModalMode] = useState("view"); // view, create, updateStatus
  const [jobs, setJobs] = useState([]);
  const [candidates, setCandidates] = useState([]);
  const { hasRole } = useAuth();

  const limit = 10;

  useEffect(() => {
    loadInitialData();
  }, []);

  useEffect(() => {
    loadApplications();
  }, [page, statusFilter, jobFilter, candidateFilter]);

  const loadInitialData = async () => {
    try {
      const [jobsData, candidatesData] = await Promise.all([
        jobsAPI.getAll(),
        candidatesAPI.search("", "", 1, 100),
      ]);
      setJobs(jobsData || []);
      setCandidates(candidatesData.candidates || candidatesData || []);
    } catch (error) {
      console.error("Error loading initial data:", error);
    }
  };

  const loadApplications = async () => {
    try {
      setLoading(true);
      const params = new URLSearchParams({
        page,
        pageSize: limit,
      });
      if (search) params.append("search", search);
      if (jobFilter) params.append("jobId", jobFilter);
      if (candidateFilter) params.append("candidateId", candidateFilter);
      if (statusFilter) params.append("statusId", statusFilter);

      const response = await fetch(
        `http://localhost:5153/api/v1/jobapplication?${params}`,
        {
          headers: {
            Authorization: `Bearer ${localStorage.getItem("token")}`,
          },
        }
      );
      const data = await response.json();

      setApplications(data.applications || []);
      setTotalPages(data.pagination?.totalPages || 1);
      setTotalCount(data.pagination?.totalCount || 0);
    } catch (error) {
      console.error("Error loading applications:", error);
      toast.error("Failed to load applications");
    } finally {
      setLoading(false);
    }
  };

  const handleSearch = () => {
    setPage(1);
    loadApplications();
  };

  const clearFilters = () => {
    setSearch("");
    setJobFilter("");
    setCandidateFilter("");
    setStatusFilter("");
    setPage(1);
    setTimeout(loadApplications, 0);
  };

  const openCreateModal = () => {
    setSelectedApplication(null);
    setModalMode("create");
    setShowModal(true);
  };

  const openUpdateStatusModal = (application) => {
    setSelectedApplication(application);
    setModalMode("updateStatus");
    setShowModal(true);
  };

  const viewDetails = async (applicationId) => {
    try {
      const application = await applicationsAPI.getById(applicationId);
      setSelectedApplication(application);
      setModalMode("view");
      setShowModal(true);
    } catch (error) {
      console.error("Error loading application:", error);
      toast.error("Failed to load application details");
    }
  };

  const handleDelete = async (applicationId) => {
    if (!window.confirm("Are you sure you want to delete this application?"))
      return;

    try {
      await applicationsAPI.delete(applicationId);
      toast.success("Application deleted successfully");
      loadApplications();
    } catch (error) {
      console.error("Error deleting application:", error);
      toast.error("Failed to delete application");
    }
  };

  const closeModal = () => {
    setShowModal(false);
    setSelectedApplication(null);
  };

  if (!hasRole(["Admin", "Recruiter", "HR", "Interviewer"])) {
    return (
      <div className="p-6">
        <div className="text-center">
          <h1 className="text-2xl font-bold text-red-600">Access Denied</h1>
          <p className="text-gray-600">
            You don't have permission to manage applications.
          </p>
        </div>
      </div>
    );
  }

  const getStatusBadge = (status) => {
    const colors = {
      Applied: "bg-blue-100 text-blue-800",
      Screening: "bg-yellow-100 text-yellow-800",
      Shortlisted: "bg-green-100 text-green-800",
      Rejected: "bg-red-100 text-red-800",
      Interviewed: "bg-purple-100 text-purple-800",
      "Offer Extended": "bg-indigo-100 text-indigo-800",
      Hired: "bg-green-200 text-green-900",
    };
    return colors[status] || "bg-gray-100 text-gray-800";
  };

  return (
    <div className="p-6">
      <Toaster position="top-right" />

      <div className="mb-6 flex justify-between items-center">
        <h1 className="text-2xl font-bold">Job Applications</h1>
        <button
          onClick={openCreateModal}
          className="bg-blue-600 text-white px-4 py-2 rounded hover:bg-blue-700"
        >
          + Create Application
        </button>
      </div>

      {/* Search/Filter */}
      <div className="bg-white p-4 rounded-lg shadow mb-6">
        <div className="grid grid-cols-1 md:grid-cols-5 gap-4">
          <div>
            <label className="block text-sm font-medium mb-2">Search</label>
            <input
              type="text"
              value={search}
              onChange={(e) => setSearch(e.target.value)}
              placeholder="Candidate name..."
              className="w-full border rounded px-3 py-2"
              onKeyPress={(e) => e.key === "Enter" && handleSearch()}
            />
          </div>
          <div>
            <label className="block text-sm font-medium mb-2">Job</label>
            <select
              value={jobFilter}
              onChange={(e) => setJobFilter(e.target.value)}
              className="w-full border rounded px-3 py-2"
            >
              <option value="">All Jobs</option>
              {jobs.map((job) => (
                <option key={job.id} value={job.id}>
                  {job.title}
                </option>
              ))}
            </select>
          </div>
          <div>
            <label className="block text-sm font-medium mb-2">Candidate</label>
            <select
              value={candidateFilter}
              onChange={(e) => setCandidateFilter(e.target.value)}
              className="w-full border rounded px-3 py-2"
            >
              <option value="">All Candidates</option>
              {candidates.map((candidate) => (
                <option key={candidate.id} value={candidate.id}>
                  {candidate.fullName}
                </option>
              ))}
            </select>
          </div>
          <div>
            <label className="block text-sm font-medium mb-2">Status</label>
            <select
              value={statusFilter}
              onChange={(e) => setStatusFilter(e.target.value)}
              className="w-full border rounded px-3 py-2"
            >
              <option value="">All Status</option>
              <option value="Applied">Applied</option>
              <option value="Screening">Screening</option>
              <option value="Shortlisted">Shortlisted</option>
              <option value="Interviewed">Interviewed</option>
              <option value="Rejected">Rejected</option>
              <option value="Offer Extended">Offer Extended</option>
              <option value="Hired">Hired</option>
            </select>
          </div>
          <div className="flex items-end gap-2">
            <button
              onClick={handleSearch}
              className="bg-blue-600 text-white px-4 py-2 rounded hover:bg-blue-700"
            >
              Search
            </button>
            <button
              onClick={clearFilters}
              className="bg-gray-500 text-white px-4 py-2 rounded hover:bg-gray-600"
            >
              Clear
            </button>
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
            Found {totalCount} applications
          </div>

          {/* Applications Table */}
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
                    Status
                  </th>
                  <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">
                    Score
                  </th>
                  <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">
                    Rounds
                  </th>
                  <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">
                    Applied
                  </th>
                  <th className="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase">
                    Actions
                  </th>
                </tr>
              </thead>
              <tbody className="bg-white divide-y divide-gray-200">
                {applications.map((application) => (
                  <tr key={application.id} className="hover:bg-gray-50">
                    <td className="px-6 py-4">
                      <div className="font-medium text-gray-900">
                        {application.candidateName}
                      </div>
                      <div className="text-xs text-gray-500">
                        {application.candidateEmail}
                      </div>
                    </td>
                    <td className="px-6 py-4">
                      <div className="text-sm text-gray-900">
                        {application.jobTitle}
                      </div>
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap">
                      <span
                        className={`px-2 py-1 text-xs rounded-full ${getStatusBadge(
                          application.statusName
                        )}`}
                      >
                        {application.statusName}
                      </span>
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                      {application.score ? application.score.toFixed(1) : "-"}
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                      {application.numberOfInterviewRounds || "-"}
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                      {application.appliedAt
                        ? new Date(application.appliedAt).toLocaleDateString()
                        : "-"}
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap text-right text-sm font-medium">
                      <button
                        onClick={() => viewDetails(application.id)}
                        className="text-blue-600 hover:text-blue-900 mr-3"
                      >
                        View
                      </button>
                      <button
                        onClick={() => openUpdateStatusModal(application)}
                        className="text-green-600 hover:text-green-900 mr-3"
                      >
                        Update
                      </button>
                      <button
                        onClick={() => handleDelete(application.id)}
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

          {/* Pagination */}
          {totalPages > 1 && (
            <div className="flex justify-center mt-6 gap-2">
              <button
                onClick={() => setPage(Math.max(1, page - 1))}
                disabled={page === 1}
                className="px-4 py-2 border rounded disabled:opacity-50 disabled:cursor-not-allowed hover:bg-gray-50"
              >
                Previous
              </button>
              <span className="px-4 py-2">
                Page {page} of {totalPages}
              </span>
              <button
                onClick={() => setPage(Math.min(totalPages, page + 1))}
                disabled={page === totalPages}
                className="px-4 py-2 border rounded disabled:opacity-50 disabled:cursor-not-allowed hover:bg-gray-50"
              >
                Next
              </button>
            </div>
          )}
        </>
      )}

      {/* Modals */}
      {showModal && modalMode === "create" && (
        <CreateApplicationModal
          jobs={jobs}
          candidates={candidates}
          onClose={closeModal}
          onSuccess={() => {
            closeModal();
            loadApplications();
          }}
        />
      )}

      {showModal && modalMode === "updateStatus" && (
        <UpdateStatusModal
          application={selectedApplication}
          onClose={closeModal}
          onSuccess={() => {
            closeModal();
            loadApplications();
          }}
        />
      )}

      {showModal && modalMode === "view" && (
        <ViewApplicationModal
          application={selectedApplication}
          onClose={closeModal}
        />
      )}
    </div>
  );
};

// Create Application Modal
const CreateApplicationModal = ({ jobs, candidates, onClose, onSuccess }) => {
  const [formData, setFormData] = useState({
    jobId: "",
    candidateId: "",
  });
  const [saving, setSaving] = useState(false);

  const handleSubmit = async (e) => {
    e.preventDefault();
    setSaving(true);

    try {
      await applicationsAPI.create(formData);
      toast.success("Application created successfully");
      onSuccess();
    } catch (error) {
      console.error("Error creating application:", error);
      toast.error(
        error.response?.data?.message || "Failed to create application"
      );
    } finally {
      setSaving(false);
    }
  };

  return (
    <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
      <div className="bg-white p-6 rounded-lg w-11/12 md:w-1/2">
        <div className="flex justify-between items-center mb-4">
          <h2 className="text-xl font-bold">Create Application</h2>
          <button
            onClick={onClose}
            className="text-gray-500 hover:text-gray-700 text-2xl"
          >
            ×
          </button>
        </div>

        <form onSubmit={handleSubmit} className="space-y-4">
          <div>
            <label className="block text-sm font-medium mb-1">Job *</label>
            <select
              value={formData.jobId}
              onChange={(e) =>
                setFormData({ ...formData, jobId: e.target.value })
              }
              required
              className="w-full border rounded px-3 py-2"
            >
              <option value="">Select Job</option>
              {jobs.map((job) => (
                <option key={job.id} value={job.id}>
                  {job.title} - {job.company}
                </option>
              ))}
            </select>
          </div>

          <div>
            <label className="block text-sm font-medium mb-1">
              Candidate *
            </label>
            <select
              value={formData.candidateId}
              onChange={(e) =>
                setFormData({ ...formData, candidateId: e.target.value })
              }
              required
              className="w-full border rounded px-3 py-2"
            >
              <option value="">Select Candidate</option>
              {candidates.map((candidate) => (
                <option key={candidate.id} value={candidate.id}>
                  {candidate.fullName} - {candidate.email}
                </option>
              ))}
            </select>
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
              {saving ? "Creating..." : "Create"}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
};

// Update Status Modal
const UpdateStatusModal = ({ application, onClose, onSuccess }) => {
  const [formData, setFormData] = useState({
    statusId: "",
    score: "",
    numberOfInterviewRounds: "",
    comment: "",
  });
  const [saving, setSaving] = useState(false);

  const statusOptions = [
    { id: "Applied", name: "Applied" },
    { id: "Screening", name: "Screening" },
    { id: "Shortlisted", name: "Shortlisted" },
    { id: "Interviewed", name: "Interviewed" },
    { id: "Rejected", name: "Rejected" },
    { id: "Offer Extended", name: "Offer Extended" },
    { id: "Hired", name: "Hired" },
  ];

  const handleSubmit = async (e) => {
    e.preventDefault();
    setSaving(true);

    try {
      const payload = {
        statusId: formData.statusId,
        score: formData.score ? parseFloat(formData.score) : null,
        numberOfInterviewRounds: formData.numberOfInterviewRounds
          ? parseInt(formData.numberOfInterviewRounds)
          : null,
        comment: formData.comment || null,
      };

      await applicationsAPI.updateStatus(application.id, payload);
      toast.success("Application updated successfully");
      onSuccess();
    } catch (error) {
      console.error("Error updating application:", error);
      toast.error(
        error.response?.data?.message || "Failed to update application"
      );
    } finally {
      setSaving(false);
    }
  };

  return (
    <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
      <div className="bg-white p-6 rounded-lg w-11/12 md:w-1/2 max-h-screen overflow-y-auto">
        <div className="flex justify-between items-center mb-4">
          <h2 className="text-xl font-bold">Update Application</h2>
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
            {application.candidateName}
          </p>
          <p className="text-sm">
            <span className="font-medium">Job:</span> {application.jobTitle}
          </p>
          <p className="text-sm">
            <span className="font-medium">Current Status:</span>{" "}
            {application.statusName}
          </p>
        </div>

        <form onSubmit={handleSubmit} className="space-y-4">
          <div>
            <label className="block text-sm font-medium mb-1">
              New Status *
            </label>
            <select
              value={formData.statusId}
              onChange={(e) =>
                setFormData({ ...formData, statusId: e.target.value })
              }
              required
              className="w-full border rounded px-3 py-2"
            >
              <option value="">Select Status</option>
              {statusOptions.map((status) => (
                <option key={status.id} value={status.id}>
                  {status.name}
                </option>
              ))}
            </select>
          </div>

          <div>
            <label className="block text-sm font-medium mb-1">
              Score (0-100)
            </label>
            <input
              type="number"
              value={formData.score}
              onChange={(e) =>
                setFormData({ ...formData, score: e.target.value })
              }
              min="0"
              max="100"
              step="0.1"
              placeholder="e.g., 85.5"
              className="w-full border rounded px-3 py-2"
            />
          </div>

          <div>
            <label className="block text-sm font-medium mb-1">
              Number of Interview Rounds
            </label>
            <input
              type="number"
              value={formData.numberOfInterviewRounds}
              onChange={(e) =>
                setFormData({
                  ...formData,
                  numberOfInterviewRounds: e.target.value,
                })
              }
              min="1"
              max="10"
              placeholder="e.g., 3"
              className="w-full border rounded px-3 py-2"
            />
          </div>

          <div>
            <label className="block text-sm font-medium mb-1">Comment</label>
            <textarea
              value={formData.comment}
              onChange={(e) =>
                setFormData({ ...formData, comment: e.target.value })
              }
              rows="4"
              placeholder="Add notes about this status change..."
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
              {saving ? "Updating..." : "Update"}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
};

// View Application Modal
const ViewApplicationModal = ({ application, onClose }) => {
  return (
    <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
      <div className="bg-white p-6 rounded-lg w-11/12 md:w-2/3 max-h-screen overflow-y-auto">
        <div className="flex justify-between items-center mb-4">
          <h2 className="text-xl font-bold">Application Details</h2>
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
                {application.candidateName}
              </div>
              <div>
                <span className="font-medium">Email:</span>{" "}
                {application.candidateEmail}
              </div>
              <div>
                <span className="font-medium">Job:</span>{" "}
                {application.jobTitle}
              </div>
              <div>
                <span className="font-medium">Company:</span>{" "}
                {application.companyName}
              </div>
              <div>
                <span className="font-medium">Status:</span>{" "}
                <span
                  className={`px-2 py-1 text-xs rounded ${
                    application.statusName === "Hired"
                      ? "bg-green-100 text-green-800"
                      : application.statusName === "Rejected"
                      ? "bg-red-100 text-red-800"
                      : "bg-blue-100 text-blue-800"
                  }`}
                >
                  {application.statusName}
                </span>
              </div>
              <div>
                <span className="font-medium">Score:</span>{" "}
                {application.score ? application.score.toFixed(1) : "N/A"}
              </div>
              <div>
                <span className="font-medium">Interview Rounds:</span>{" "}
                {application.numberOfInterviewRounds || "Not set"}
              </div>
              <div>
                <span className="font-medium">Applied:</span>{" "}
                {application.appliedAt
                  ? new Date(application.appliedAt).toLocaleString()
                  : "N/A"}
              </div>
              <div>
                <span className="font-medium">Last Updated:</span>{" "}
                {application.lastUpdated
                  ? new Date(application.lastUpdated).toLocaleString()
                  : "N/A"}
              </div>
            </div>
          </div>

          {/* Comments */}
          {application.comments && application.comments.length > 0 && (
            <div>
              <h3 className="text-lg font-semibold mb-3">Comments</h3>
              <div className="space-y-2">
                {application.comments.map((comment) => (
                  <div
                    key={comment.id}
                    className="p-3 bg-gray-50 rounded border border-gray-200"
                  >
                    <p className="text-sm">{comment.comment}</p>
                    <p className="text-xs text-gray-500 mt-1">
                      By {comment.commentedBy} on{" "}
                      {new Date(comment.commentedAt).toLocaleString()}
                    </p>
                  </div>
                ))}
              </div>
            </div>
          )}

          {/* Status History */}
          {application.statusHistory && application.statusHistory.length > 0 && (
            <div>
              <h3 className="text-lg font-semibold mb-3">Status History</h3>
              <div className="space-y-2">
                {application.statusHistory.map((history) => (
                  <div
                    key={history.id}
                    className="p-3 bg-blue-50 rounded border border-blue-200"
                  >
                    <p className="text-sm">
                      <span className="font-medium">{history.fromStatus}</span>{" "}
                      → <span className="font-medium">{history.toStatus}</span>
                    </p>
                    {history.note && (
                      <p className="text-sm mt-1">{history.note}</p>
                    )}
                    <p className="text-xs text-gray-500 mt-1">
                      By {history.changedBy} on{" "}
                      {new Date(history.changedAt).toLocaleString()}
                    </p>
                  </div>
                ))}
              </div>
            </div>
          )}

          {/* Documents */}
          {application.documents && application.documents.length > 0 && (
            <div>
              <h3 className="text-lg font-semibold mb-3">Documents</h3>
              <div className="space-y-2">
                {application.documents.map((doc) => (
                  <div
                    key={doc.id}
                    className="p-3 bg-gray-50 rounded border border-gray-200 flex justify-between items-center"
                  >
                    <div>
                      <p className="text-sm font-medium">{doc.documentType}</p>
                      <p className="text-xs text-gray-500">
                        {doc.originalFileName}
                      </p>
                    </div>
                    <a
                      href={doc.url}
                      target="_blank"
                      rel="noopener noreferrer"
                      className="text-blue-600 hover:text-blue-900 text-sm"
                    >
                      View
                    </a>
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

export default ApplicationsPage;
