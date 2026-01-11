import React, { useState, useEffect } from "react";
import { screeningAPI, positionsAPI, applicationsAPI } from "../services/api";
import { useAuth } from "../contexts/AuthContext";
import toast, { Toaster } from "react-hot-toast";

const ScreeningPage = () => {
  const [applications, setApplications] = useState([]);
  const [positions, setPositions] = useState([]);
  const [loading, setLoading] = useState(true);
  const [search, setSearch] = useState("");
  const [positionFilter, setPositionFilter] = useState("");
  const [page, setPage] = useState(1);
  const [selectedApplication, setSelectedApplication] = useState(null);
  const [showScreenModal, setShowScreenModal] = useState(false);
  const [showViewModal, setShowViewModal] = useState(false);
  const [showShortlistModal, setShowShortlistModal] = useState(false);
  const { hasRole } = useAuth();

  const limit = 10;

  useEffect(() => {
    loadInitialData();
  }, []);

  useEffect(() => {
    loadApplications();
  }, [page, positionFilter]);

  const loadInitialData = async () => {
    try {
      const positionsData = await positionsAPI.getAll();
      setPositions(positionsData.positions || positionsData || []);
    } catch (error) {
      console.error("Error loading positions:", error);
    }
  };

  const loadApplications = async () => {
    try {
      setLoading(true);
      const params = {
        page,
        pageSize: limit,
      };
      if (search) params.search = search;
      if (positionFilter) params.positionId = positionFilter;

      const response = await screeningAPI.getApplicationsForScreening(params);
      setApplications(response.applications || []);
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
    setPositionFilter("");
    setPage(1);
    setTimeout(loadApplications, 0);
  };

  const openScreenModal = (application) => {
    console.log("Opening screen modal for application:", application);
    setSelectedApplication(application);
    setShowScreenModal(true);
  };

  const openViewModal = async (application) => {
    try {
      console.log("Application data:", application);
      // Use 'id' instead of 'jobApplicationId'
      const appId = application.id || application.jobApplicationId;
      if (!appId || appId === '00000000-0000-0000-0000-000000000000') {
        toast.error("Invalid application ID");
        return;
      }
      const fullApp = await applicationsAPI.getById(appId);
      console.log("Full application data:", fullApp);
      setSelectedApplication(fullApp);
      setShowViewModal(true);
    } catch (error) {
      console.error("Error loading application:", error);
      toast.error("Failed to load application details");
    }
  };

  const openShortlistModal = (application) => {
    console.log("Opening shortlist modal for application:", application);
    setSelectedApplication(application);
    setShowShortlistModal(true);
  };

  const closeModals = () => {
    setShowScreenModal(false);
    setShowViewModal(false);
    setShowShortlistModal(false);
    setSelectedApplication(null);
  };

  if (!hasRole(["Admin", "Recruiter", "HR", "Reviewer"])) {
    return (
      <div className="p-6">
        <div className="text-center">
          <h1 className="text-2xl font-bold text-red-600">Access Denied</h1>
          <p className="text-gray-600">
            You don't have permission to screen applications.
          </p>
        </div>
      </div>
    );
  }

  const getStatusBadge = (status) => {
    const colors = {
      Pending: "bg-yellow-100 text-yellow-800",
      Approved: "bg-green-100 text-green-800",
      Rejected: "bg-red-100 text-red-800",
      Shortlisted: "bg-blue-100 text-blue-800",
      Screening: "bg-purple-100 text-purple-800",
    };
    return colors[status] || "bg-gray-100 text-gray-800";
  };

  return (
    <div className="p-6">
      <Toaster position="top-right" />

      <div className="mb-6 flex justify-between items-center">
        <h1 className="text-2xl font-bold">Resume Screening</h1>
      </div>

      {/* Search/Filter */}
      <div className="bg-white p-4 rounded-lg shadow mb-6">
        <div className="grid grid-cols-1 md:grid-cols-4 gap-4">
          <div className="md:col-span-2">
            <label className="block text-sm font-medium mb-2">
              Search Candidate
            </label>
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
            <label className="block text-sm font-medium mb-2">Position</label>
            <select
              value={positionFilter}
              onChange={(e) => setPositionFilter(e.target.value)}
              className="w-full border rounded px-3 py-2"
            >
              <option value="">All Positions</option>
              {positions.map((pos) => (
                <option key={pos.id} value={pos.id}>
                  {pos.title}
                </option>
              ))}
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
          {/* Applications Table */}
          <div className="bg-white shadow rounded-lg overflow-hidden">
            <table className="min-w-full divide-y divide-gray-200">
              <thead className="bg-gray-50">
                <tr>
                  <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">
                    Candidate
                  </th>
                  <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">
                    Position / Job
                  </th>
                  <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">
                    Status
                  </th>
                  <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">
                    Score
                  </th>
                  <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">
                    Applied Date
                  </th>
                  <th className="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase">
                    Actions
                  </th>
                </tr>
              </thead>
              <tbody className="bg-white divide-y divide-gray-200">
                {applications.length === 0 ? (
                  <tr>
                    <td colSpan="6" className="px-6 py-8 text-center text-gray-500">
                      No applications found for screening
                    </td>
                  </tr>
                ) : (
                  applications.map((app) => (
                    <tr key={app.id} className="hover:bg-gray-50">
                      <td className="px-6 py-4">
                        <div className="font-medium text-gray-900">
                          {app.candidateName}
                        </div>
                        <div className="text-xs text-gray-500">
                          {app.candidateEmail}
                        </div>
                      </td>
                      <td className="px-6 py-4">
                        <div className="text-sm text-gray-900">
                          {app.jobTitle}
                        </div>
                        <div className="text-xs text-gray-500">
                          {app.positionTitle}
                        </div>
                      </td>
                      <td className="px-6 py-4 whitespace-nowrap">
                        <span
                          className={`px-2 py-1 text-xs rounded-full ${getStatusBadge(
                            app.status
                          )}`}
                        >
                          {app.status}
                        </span>
                      </td>
                      <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                        {app.score ? `${app.score}/100` : "Not scored"}
                      </td>
                      <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                        {new Date(app.appliedAt).toLocaleDateString()}
                      </td>
                      <td className="px-6 py-4 whitespace-nowrap text-right text-sm font-medium">
                        <button
                          onClick={() => openViewModal(app)}
                          className="text-blue-600 hover:text-blue-900 mr-3"
                        >
                          View
                        </button>
                        {app.status !== "Shortlisted" && (
                          <>
                            <button
                              onClick={() => openScreenModal(app)}
                              className="text-green-600 hover:text-green-900 mr-3"
                            >
                              Screen
                            </button>
                            <button
                              onClick={() => openShortlistModal(app)}
                              className="text-purple-600 hover:text-purple-900"
                            >
                              Shortlist
                            </button>
                          </>
                        )}
                      </td>
                    </tr>
                  ))
                )}
              </tbody>
            </table>
          </div>

          {/* Pagination */}
          <div className="flex justify-center mt-6 gap-2">
            <button
              onClick={() => setPage(Math.max(1, page - 1))}
              disabled={page === 1}
              className="px-4 py-2 border rounded disabled:opacity-50 disabled:cursor-not-allowed hover:bg-gray-50"
            >
              Previous
            </button>
            <span className="px-4 py-2">Page {page}</span>
            <button
              onClick={() => setPage(page + 1)}
              disabled={applications.length < limit}
              className="px-4 py-2 border rounded disabled:opacity-50 disabled:cursor-not-allowed hover:bg-gray-50"
            >
              Next
            </button>
          </div>
        </>
      )}

      {/* Modals */}
      {showScreenModal && (
        <ScreenModal
          application={selectedApplication}
          onClose={closeModals}
          onSuccess={() => {
            closeModals();
            loadApplications();
          }}
        />
      )}

      {showViewModal && (
        <ViewApplicationModal
          application={selectedApplication}
          onClose={closeModals}
        />
      )}

      {showShortlistModal && (
        <ShortlistModal
          application={selectedApplication}
          onClose={closeModals}
          onSuccess={() => {
            closeModals();
            loadApplications();
          }}
        />
      )}
    </div>
  );
};

// Screen Modal
const ScreenModal = ({ application, onClose, onSuccess }) => {
  const [formData, setFormData] = useState({
    score: "",
    comments: "",
    approved: false,
  });
  const [saving, setSaving] = useState(false);

  const handleSubmit = async (e) => {
    e.preventDefault();
    setSaving(true);

    try {
      const payload = {
        jobApplicationId: application.jobApplicationId,
        score: formData.score ? parseFloat(formData.score) : null,
        comments: formData.comments,
        approved: formData.approved,
      };

      await fetch("http://localhost:5153/api/v1/screening/screen", {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${localStorage.getItem("token")}`,
        },
        body: JSON.stringify(payload),
      });

      toast.success("Application screened successfully");
      onSuccess();
    } catch (error) {
      console.error("Error screening application:", error);
      toast.error("Failed to screen application");
    } finally {
      setSaving(false);
    }
  };

  return (
    <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
      <div className="bg-white p-6 rounded-lg w-11/12 md:w-1/2 max-h-screen overflow-y-auto">
        <div className="flex justify-between items-center mb-4">
          <h2 className="text-xl font-bold">Screen Application</h2>
          <button
            onClick={onClose}
            className="text-gray-500 hover:text-gray-700 text-2xl"
          >
            ×
          </button>
        </div>

        <div className="mb-4 p-4 bg-gray-50 rounded">
          <h3 className="font-medium mb-2">Application Details</h3>
          <div className="text-sm space-y-1">
            <p>
              <span className="font-medium">Candidate:</span>{" "}
              {application.candidateName}
            </p>
            <p>
              <span className="font-medium">Job:</span> {application.jobTitle}
            </p>
            <p>
              <span className="font-medium">Position:</span>{" "}
              {application.positionTitle}
            </p>
            <p>
              <span className="font-medium">Current Status:</span>{" "}
              <span className={`px-2 py-1 text-xs rounded ${
                application.status === "Approved" ? "bg-green-100 text-green-800" :
                application.status === "Rejected" ? "bg-red-100 text-red-800" :
                "bg-yellow-100 text-yellow-800"
              }`}>
                {application.status}
              </span>
            </p>
          </div>
        </div>

        <form onSubmit={handleSubmit} className="space-y-4">
          <div>
            <label className="block text-sm font-medium mb-1">
              Score (0-100)
            </label>
            <input
              type="number"
              min="0"
              max="100"
              step="0.1"
              value={formData.score}
              onChange={(e) =>
                setFormData({ ...formData, score: e.target.value })
              }
              placeholder="Enter score..."
              className="w-full border rounded px-3 py-2"
            />
          </div>

          <div>
            <label className="block text-sm font-medium mb-1">Comments</label>
            <textarea
              value={formData.comments}
              onChange={(e) =>
                setFormData({ ...formData, comments: e.target.value })
              }
              rows="4"
              placeholder="Add screening comments..."
              className="w-full border rounded px-3 py-2"
            />
          </div>

          <div className="flex items-center gap-2">
            <input
              type="checkbox"
              id="approved"
              checked={formData.approved}
              onChange={(e) =>
                setFormData({ ...formData, approved: e.target.checked })
              }
              className="rounded"
            />
            <label htmlFor="approved" className="text-sm font-medium">
              Approve this application
            </label>
          </div>

          <div className="flex justify-end gap-2 pt-4 border-t">
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
              className="bg-green-600 text-white px-4 py-2 rounded hover:bg-green-700 disabled:opacity-50"
            >
              {saving ? "Screening..." : "Submit Screening"}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
};

// View Application Modal
const ViewApplicationModal = ({ application, onClose }) => {
  if (!application) return null;

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
          <div>
            <h3 className="text-lg font-semibold mb-3">Candidate Information</h3>
            <div className="grid grid-cols-2 gap-4 text-sm">
              <div>
                <span className="font-medium">Name:</span>{" "}
                {application.candidateName || "N/A"}
              </div>
              <div>
                <span className="font-medium">Email:</span>{" "}
                {application.candidateEmail || "N/A"}
              </div>
              <div>
                <span className="font-medium">Phone:</span>{" "}
                {application.candidatePhone || "N/A"}
              </div>
              <div>
                <span className="font-medium">Applied:</span>{" "}
                {application.appliedAt ? new Date(application.appliedAt).toLocaleDateString() : "N/A"}
              </div>
            </div>
          </div>

          <div>
            <h3 className="text-lg font-semibold mb-3">Job Information</h3>
            <div className="grid grid-cols-2 gap-4 text-sm">
              <div>
                <span className="font-medium">Job Title:</span>{" "}
                {application.jobTitle || "N/A"}
              </div>
              <div>
                <span className="font-medium">Position:</span>{" "}
                {application.positionTitle || "N/A"}
              </div>
              <div>
                <span className="font-medium">Status:</span>{" "}
                <span
                  className={`px-2 py-1 text-xs rounded ${
                    application.statusName === "Approved" || application.status === "Approved"
                      ? "bg-green-100 text-green-800"
                      : application.statusName === "Rejected" || application.status === "Rejected"
                      ? "bg-red-100 text-red-800"
                      : "bg-yellow-100 text-yellow-800"
                  }`}
                >
                  {application.statusName || application.status || "N/A"}
                </span>
              </div>
              <div>
                <span className="font-medium">Company:</span>{" "}
                {application.companyName || "N/A"}
              </div>
            </div>
          </div>

          {application.score && (
            <div>
              <h3 className="text-lg font-semibold mb-3">Screening Results</h3>
              <div className="grid grid-cols-2 gap-4 text-sm">
                <div>
                  <span className="font-medium">Score:</span> {application.score}
                  /100
                </div>
                <div>
                  <span className="font-medium">Interview Rounds:</span>{" "}
                  {application.numberOfInterviewRounds || 0}
                </div>
              </div>
            </div>
          )}

          {application.comments && Array.isArray(application.comments) && application.comments.length > 0 && (
            <div>
              <h3 className="text-lg font-semibold mb-3">Comments</h3>
              <div className="space-y-2">
                {application.comments.map((comment, index) => (
                  <div
                    key={comment.id || index}
                    className="bg-gray-50 p-3 rounded text-sm"
                  >
                    <p className="text-gray-700">{comment.comment || comment.text || "No comment"}</p>
                    <div className="mt-2 text-xs text-gray-500">
                      {comment.commenterName && `By ${comment.commenterName}`}
                      {comment.createdAt && ` on ${new Date(comment.createdAt).toLocaleString()}`}
                    </div>
                  </div>
                ))}
              </div>
            </div>
          )}
        </div>

        <div className="flex justify-end pt-4 border-t mt-6">
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

// Shortlist Modal
const ShortlistModal = ({ application, onClose, onSuccess }) => {
  const [comments, setComments] = useState("");
  const [saving, setSaving] = useState(false);

  const handleSubmit = async (e) => {
    e.preventDefault();
    setSaving(true);

    try {
      const payload = {
        jobApplicationId: application.jobApplicationId,
        comments,
      };

      await fetch("http://localhost:5153/api/v1/screening/shortlist", {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${localStorage.getItem("token")}`,
        },
        body: JSON.stringify(payload),
      });

      toast.success("Application shortlisted successfully");
      onSuccess();
    } catch (error) {
      console.error("Error shortlisting application:", error);
      toast.error("Failed to shortlist application");
    } finally {
      setSaving(false);
    }
  };

  return (
    <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
      <div className="bg-white p-6 rounded-lg w-11/12 md:w-1/2">
        <div className="flex justify-between items-center mb-4">
          <h2 className="text-xl font-bold">Shortlist Candidate</h2>
          <button
            onClick={onClose}
            className="text-gray-500 hover:text-gray-700 text-2xl"
          >
            ×
          </button>
        </div>

        <div className="mb-4 p-4 bg-gray-50 rounded">
          <p className="text-sm">
            <span className="font-medium">Candidate:</span>{" "}
            {application.candidateName}
          </p>
          <p className="text-sm">
            <span className="font-medium">Job:</span> {application.jobTitle}
          </p>
        </div>

        <form onSubmit={handleSubmit} className="space-y-4">
          <div>
            <label className="block text-sm font-medium mb-1">
              Comments (Optional)
            </label>
            <textarea
              value={comments}
              onChange={(e) => setComments(e.target.value)}
              rows="4"
              placeholder="Add comments about why this candidate is being shortlisted..."
              className="w-full border rounded px-3 py-2"
            />
          </div>

          <div className="flex justify-end gap-2 pt-4 border-t">
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
              className="bg-purple-600 text-white px-4 py-2 rounded hover:bg-purple-700 disabled:opacity-50"
            >
              {saving ? "Shortlisting..." : "Shortlist Candidate"}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
};

export default ScreeningPage;
