import React, { useState, useEffect } from "react";
import { positionsAPI, skillsAPI, employeesAPI, candidatesAPI } from "../services/api";
import { useAuth } from "../contexts/AuthContext";
import toast, { Toaster } from "react-hot-toast";

const PositionsPage = () => {
  const [positions, setPositions] = useState([]);
  const [loading, setLoading] = useState(true);
  const [search, setSearch] = useState("");
  const [statusFilter, setStatusFilter] = useState("");
  const [page, setPage] = useState(1);
  const [totalPages, setTotalPages] = useState(1);
  const [totalCount, setTotalCount] = useState(0);
  const [selectedPosition, setSelectedPosition] = useState(null);
  const [showModal, setShowModal] = useState(false);
  const [modalMode, setModalMode] = useState("view"); // view, create, edit, close
  const { hasRole } = useAuth();

  const limit = 10;

  useEffect(() => {
    loadPositions();
  }, [page, statusFilter]);

  const loadPositions = async () => {
    try {
      setLoading(true);
      const params = new URLSearchParams({
        page,
        pageSize: limit,
      });
      if (search) params.append("search", search);
      if (statusFilter) params.append("statusId", statusFilter);

      const response = await fetch(
        `http://localhost:5153/api/v1/positions?${params}`,
        {
          headers: {
            Authorization: `Bearer ${localStorage.getItem("token")}`,
          },
        }
      );
      const data = await response.json();

      setPositions(data.positions || []);
      setTotalPages(data.pagination?.totalPages || 1);
      setTotalCount(data.pagination?.totalCount || 0);
    } catch (error) {
      console.error("Error loading positions:", error);
      toast.error("Failed to load positions");
    } finally {
      setLoading(false);
    }
  };

  const handleSearch = () => {
    setPage(1);
    loadPositions();
  };

  const clearFilters = () => {
    setSearch("");
    setStatusFilter("");
    setPage(1);
    setTimeout(loadPositions, 0);
  };

  const openCreateModal = () => {
    setSelectedPosition(null);
    setModalMode("create");
    setShowModal(true);
  };

  const openEditModal = (position) => {
    setSelectedPosition(position);
    setModalMode("edit");
    setShowModal(true);
  };

  const openCloseModal = (position) => {
    setSelectedPosition(position);
    setModalMode("close");
    setShowModal(true);
  };

  const viewDetails = async (positionId) => {
    try {
      const position = await positionsAPI.getById(positionId);
      setSelectedPosition(position);
      setModalMode("view");
      setShowModal(true);
    } catch (error) {
      console.error("Error loading position:", error);
      toast.error("Failed to load position details");
    }
  };

  const handleDelete = async (positionId) => {
    if (!window.confirm("Are you sure you want to delete this position?"))
      return;

    try {
      await positionsAPI.delete(positionId);
      toast.success("Position deleted successfully");
      loadPositions();
    } catch (error) {
      console.error("Error deleting position:", error);
      toast.error("Failed to delete position");
    }
  };

  const closeModal = () => {
    setShowModal(false);
    setSelectedPosition(null);
  };

  if (!hasRole(["Admin", "Recruiter", "HR"])) {
    return (
      <div className="p-6">
        <div className="text-center">
          <h1 className="text-2xl font-bold text-red-600">Access Denied</h1>
          <p className="text-gray-600">
            You don't have permission to manage positions.
          </p>
        </div>
      </div>
    );
  }

  const getStatusBadge = (status) => {
    const colors = {
      Open: "bg-green-100 text-green-800",
      Closed: "bg-red-100 text-red-800",
      Hold: "bg-yellow-100 text-yellow-800",
    };
    return colors[status] || "bg-gray-100 text-gray-800";
  };

  return (
    <div className="p-6">
      <Toaster position="top-right" />

      <div className="mb-6 flex justify-between items-center">
        <h1 className="text-2xl font-bold">Positions</h1>
        <button
          onClick={openCreateModal}
          className="bg-blue-600 text-white px-4 py-2 rounded hover:bg-blue-700"
        >
          + Create Position
        </button>
      </div>

      {/* Search/Filter */}
      <div className="bg-white p-4 rounded-lg shadow mb-6">
        <div className="grid grid-cols-1 md:grid-cols-4 gap-4">
          <div className="md:col-span-2">
            <label className="block text-sm font-medium mb-2">Search</label>
            <input
              type="text"
              value={search}
              onChange={(e) => setSearch(e.target.value)}
              placeholder="Position title..."
              className="w-full border rounded px-3 py-2"
              onKeyPress={(e) => e.key === "Enter" && handleSearch()}
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
              <option value="Open">Open</option>
              <option value="Closed">Closed</option>
              <option value="Hold">Hold</option>
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
            Found {totalCount} positions
          </div>

          {/* Positions Table */}
          <div className="bg-white shadow rounded-lg overflow-hidden">
            <table className="min-w-full divide-y divide-gray-200">
              <thead className="bg-gray-50">
                <tr>
                  <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">
                    Title
                  </th>
                  <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">
                    Status
                  </th>
                  <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">
                    Reviewer
                  </th>
                  <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">
                    Interviews
                  </th>
                  <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">
                    Jobs
                  </th>
                  <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">
                    Skills
                  </th>
                  <th className="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase">
                    Actions
                  </th>
                </tr>
              </thead>
              <tbody className="bg-white divide-y divide-gray-200">
                {positions.map((position) => (
                  <tr key={position.id} className="hover:bg-gray-50">
                    <td className="px-6 py-4">
                      <div className="font-medium text-gray-900">
                        {position.title}
                      </div>
                      <div className="text-xs text-gray-500">
                        Created {new Date(position.createdAt).toLocaleDateString()}
                      </div>
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap">
                      <span
                        className={`px-2 py-1 text-xs rounded-full ${getStatusBadge(
                          position.status
                        )}`}
                      >
                        {position.status}
                      </span>
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                      {position.reviewerName || "-"}
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                      {position.numberOfInterviews}
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                      {position.jobCount || 0}
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                      {position.skillCount || 0}
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap text-right text-sm font-medium">
                      <button
                        onClick={() => viewDetails(position.id)}
                        className="text-blue-600 hover:text-blue-900 mr-3"
                      >
                        View
                      </button>
                      <button
                        onClick={() => openEditModal(position)}
                        className="text-green-600 hover:text-green-900 mr-3"
                      >
                        Edit
                      </button>
                      {position.status === "Open" && (
                        <button
                          onClick={() => openCloseModal(position)}
                          className="text-orange-600 hover:text-orange-900 mr-3"
                        >
                          Close
                        </button>
                      )}
                      <button
                        onClick={() => handleDelete(position.id)}
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
      {showModal && modalMode === "close" && (
        <ClosePositionModal
          position={selectedPosition}
          onClose={closeModal}
          onSuccess={() => {
            closeModal();
            loadPositions();
          }}
        />
      )}

      {showModal && (modalMode === "view" || modalMode === "create" || modalMode === "edit") && (
        <PositionModal
          position={selectedPosition}
          mode={modalMode}
          onClose={closeModal}
          onSuccess={() => {
            closeModal();
            loadPositions();
          }}
        />
      )}
    </div>
  );
};

// Position Modal (View/Create/Edit)
const PositionModal = ({ position, mode, onClose, onSuccess }) => {
  const [formData, setFormData] = useState({
    title: "",
    numberOfInterviews: 1,
    reviewerId: "",
    skills: [],
  });
  const [allSkills, setAllSkills] = useState([]);
  const [employees, setEmployees] = useState([]);
  const [skillSearch, setSkillSearch] = useState("");
  const [saving, setSaving] = useState(false);

  useEffect(() => {
    loadData();
  }, []);

  useEffect(() => {
    if (position && (mode === "view" || mode === "edit")) {
      setFormData({
        title: position.title || "",
        numberOfInterviews: position.numberOfInterviews || 1,
        reviewerId: position.reviewer?.id || "",
        skills: position.skills?.map((s) => s.id) || [],
      });
    }
  }, [position, mode]);

  const loadData = async () => {
    try {
      const [skillsData, employeesData] = await Promise.all([
        skillsAPI.getAll(),
        employeesAPI.getAll(),
      ]);
      setAllSkills(skillsData.Skills || skillsData.skills || skillsData || []);
      setEmployees(employeesData || []);
    } catch (error) {
      console.error("Error loading data:", error);
    }
  };

  const handleChange = (e) => {
    setFormData({ ...formData, [e.target.name]: e.target.value });
  };

  const toggleSkill = (skillId) => {
    setFormData((prev) => ({
      ...prev,
      skills: prev.skills.includes(skillId)
        ? prev.skills.filter((id) => id !== skillId)
        : [...prev.skills, skillId],
    }));
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setSaving(true);

    try {
      const payload = {
        title: formData.title,
        numberOfInterviews: parseInt(formData.numberOfInterviews),
        reviewerId: formData.reviewerId || null,
        skills: formData.skills.map((skillId) => ({ skillId })),
      };

      if (mode === "create") {
        await positionsAPI.create(payload);
        toast.success("Position created successfully");
      } else if (mode === "edit") {
        await positionsAPI.update(position.id, payload);
        toast.success("Position updated successfully");
      }
      onSuccess();
    } catch (error) {
      console.error("Error saving position:", error);
      toast.error(error.response?.data?.message || "Failed to save position");
    } finally {
      setSaving(false);
    }
  };

  const isViewMode = mode === "view";

  const filteredSkills = allSkills.filter((skill) =>
    skill.skillName?.toLowerCase().includes(skillSearch.toLowerCase())
  );

  return (
    <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
      <div className="bg-white p-6 rounded-lg w-11/12 md:w-2/3 lg:w-1/2 max-h-screen overflow-y-auto">
        <div className="flex justify-between items-center mb-4">
          <h2 className="text-xl font-bold">
            {mode === "create"
              ? "Create New Position"
              : mode === "edit"
              ? "Edit Position"
              : "Position Details"}
          </h2>
          <button
            onClick={onClose}
            className="text-gray-500 hover:text-gray-700 text-2xl"
          >
            ×
          </button>
        </div>

        <form onSubmit={handleSubmit} className="space-y-4">
          <div>
            <label className="block text-sm font-medium mb-1">Title *</label>
            <input
              type="text"
              name="title"
              value={formData.title}
              onChange={handleChange}
              disabled={isViewMode}
              required
              placeholder="e.g., Senior Software Engineer"
              className="w-full border rounded px-3 py-2 disabled:bg-gray-100"
            />
          </div>

          <div>
            <label className="block text-sm font-medium mb-1">
              Number of Interview Rounds *
            </label>
            <input
              type="number"
              name="numberOfInterviews"
              value={formData.numberOfInterviews}
              onChange={handleChange}
              disabled={isViewMode}
              required
              min="1"
              max="10"
              className="w-full border rounded px-3 py-2 disabled:bg-gray-100"
            />
          </div>

          <div>
            <label className="block text-sm font-medium mb-1">Reviewer</label>
            <select
              name="reviewerId"
              value={formData.reviewerId}
              onChange={handleChange}
              disabled={isViewMode}
              className="w-full border rounded px-3 py-2 disabled:bg-gray-100"
            >
              <option value="">No Reviewer</option>
              {employees.map((emp) => (
                <option key={emp.id} value={emp.id}>
                  {emp.fullName || emp.email}
                </option>
              ))}
            </select>
          </div>

          {!isViewMode && (
            <div>
              <label className="block text-sm font-medium mb-1">
                Required Skills
              </label>
              <input
                type="text"
                value={skillSearch}
                onChange={(e) => setSkillSearch(e.target.value)}
                placeholder="Search skills..."
                className="w-full border rounded px-3 py-2 mb-2"
              />
              <div className="border rounded p-3 max-h-48 overflow-y-auto">
                {filteredSkills.map((skill) => (
                  <label
                    key={skill.id}
                    className="flex items-center gap-2 py-1 cursor-pointer hover:bg-gray-50"
                  >
                    <input
                      type="checkbox"
                      checked={formData.skills.includes(skill.id)}
                      onChange={() => toggleSkill(skill.id)}
                      className="rounded"
                    />
                    <span>{skill.skillName}</span>
                  </label>
                ))}
              </div>
              <div className="mt-2 text-sm text-gray-600">
                {formData.skills.length} skill(s) selected
              </div>
            </div>
          )}

          {isViewMode && position && (
            <div className="mt-4 pt-4 border-t space-y-3">
              <h3 className="font-semibold">Additional Information</h3>
              <div className="text-sm space-y-2">
                <p>
                  <span className="font-medium">Status:</span>{" "}
                  <span
                    className={`px-2 py-1 text-xs rounded ${
                      position.status?.status === "Open"
                        ? "bg-green-100 text-green-800"
                        : position.status?.status === "Closed"
                        ? "bg-red-100 text-red-800"
                        : "bg-yellow-100 text-yellow-800"
                    }`}
                  >
                    {position.status?.status}
                  </span>
                </p>
                <p>
                  <span className="font-medium">Jobs:</span> {position.jobCount || 0}
                </p>
                <p>
                  <span className="font-medium">Skills:</span>{" "}
                  {position.skills?.map((s) => s.skillName).join(", ") || "None"}
                </p>
                {position.closedReason && (
                  <p>
                    <span className="font-medium">Closed Reason:</span>{" "}
                    {position.closedReason}
                  </p>
                )}
                {position.selectedCandidate && (
                  <p>
                    <span className="font-medium">Selected Candidate:</span>{" "}
                    {position.selectedCandidate.fullName} ({position.selectedCandidate.email})
                  </p>
                )}
                <p>
                  <span className="font-medium">Created:</span>{" "}
                  {new Date(position.createdAt).toLocaleString()}
                </p>
                <p>
                  <span className="font-medium">Updated:</span>{" "}
                  {new Date(position.updatedAt).toLocaleString()}
                </p>
              </div>
            </div>
          )}

          {!isViewMode && (
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
                {saving ? "Saving..." : mode === "create" ? "Create" : "Update"}
              </button>
            </div>
          )}
        </form>
      </div>
    </div>
  );
};

// Close Position Modal
const ClosePositionModal = ({ position, onClose, onSuccess }) => {
  const [reason, setReason] = useState("");
  const [selectedCandidateId, setSelectedCandidateId] = useState("");
  const [candidates, setCandidates] = useState([]);
  const [candidateSearch, setCandidateSearch] = useState("");
  const [loadingCandidates, setLoadingCandidates] = useState(false);
  const [closing, setClosing] = useState(false);

  useEffect(() => {
    loadCandidates();
  }, []);

  const loadCandidates = async () => {
    try {
      setLoadingCandidates(true);
      const response = await candidatesAPI.search("", "", 1, 100);
      setCandidates(response.candidates || response);
    } catch (error) {
      console.error("Error loading candidates:", error);
    } finally {
      setLoadingCandidates(false);
    }
  };

  const handleClose = async () => {
    if (!reason && !selectedCandidateId) {
      toast.error("Please provide either a reason or select a candidate");
      return;
    }

    setClosing(true);
    try {
      const response = await fetch(
        `http://localhost:5153/api/v1/positions/${position.id}/close`,
        {
          method: "POST",
          headers: {
            "Content-Type": "application/json",
            Authorization: `Bearer ${localStorage.getItem("token")}`,
          },
          body: JSON.stringify({
            reason: reason || null,
            selectedCandidateId: selectedCandidateId || null,
          }),
        }
      );

      if (!response.ok) {
        const error = await response.json();
        throw new Error(error.message || "Failed to close position");
      }

      toast.success("Position closed successfully");
      onSuccess();
    } catch (error) {
      console.error("Error closing position:", error);
      toast.error(error.message || "Failed to close position");
    } finally {
      setClosing(false);
    }
  };

  const filteredCandidates = candidates.filter((c) =>
    c.fullName?.toLowerCase().includes(candidateSearch.toLowerCase()) ||
    c.email?.toLowerCase().includes(candidateSearch.toLowerCase())
  );

  return (
    <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
      <div className="bg-white p-6 rounded-lg w-11/12 md:w-1/2 max-h-screen overflow-y-auto">
        <div className="flex justify-between items-center mb-4">
          <h2 className="text-xl font-bold">Close Position</h2>
          <button
            onClick={onClose}
            className="text-gray-500 hover:text-gray-700 text-2xl"
          >
            ×
          </button>
        </div>

        <div className="mb-4 p-3 bg-yellow-50 border border-yellow-200 rounded">
          <p className="text-sm text-yellow-800">
            <strong>Note:</strong> You must provide either a closure reason OR select a
            candidate (or both).
          </p>
        </div>

        <div className="space-y-4">
          <div>
            <label className="block text-sm font-medium mb-1">Position</label>
            <input
              type="text"
              value={position?.title}
              disabled
              className="w-full border rounded px-3 py-2 bg-gray-100"
            />
          </div>

          <div>
            <label className="block text-sm font-medium mb-1">
              Select Candidate (Optional)
            </label>
            {loadingCandidates ? (
              <p className="text-sm text-gray-500">Loading candidates...</p>
            ) : (
              <>
                <input
                  type="text"
                  value={candidateSearch}
                  onChange={(e) => setCandidateSearch(e.target.value)}
                  placeholder="Search candidates..."
                  className="w-full border rounded px-3 py-2 mb-2"
                />
                <select
                  value={selectedCandidateId}
                  onChange={(e) => setSelectedCandidateId(e.target.value)}
                  className="w-full border rounded px-3 py-2"
                >
                  <option value="">No candidate selected</option>
                  {filteredCandidates.map((candidate) => (
                    <option key={candidate.id} value={candidate.id}>
                      {candidate.fullName} - {candidate.email}
                    </option>
                  ))}
                </select>
              </>
            )}
          </div>

          <div>
            <label className="block text-sm font-medium mb-1">
              Closure Reason (Optional)
            </label>
            <textarea
              value={reason}
              onChange={(e) => setReason(e.target.value)}
              placeholder="e.g., Position filled, Budget constraints, Requirements changed..."
              rows="4"
              className="w-full border rounded px-3 py-2"
            />
          </div>

          <div className="flex justify-end gap-2 pt-4">
            <button
              onClick={onClose}
              className="px-4 py-2 border rounded hover:bg-gray-50"
            >
              Cancel
            </button>
            <button
              onClick={handleClose}
              disabled={closing || (!reason && !selectedCandidateId)}
              className="bg-red-600 text-white px-4 py-2 rounded hover:bg-red-700 disabled:opacity-50"
            >
              {closing ? "Closing..." : "Close Position"}
            </button>
          </div>
        </div>
      </div>
    </div>
  );
};

export default PositionsPage;
