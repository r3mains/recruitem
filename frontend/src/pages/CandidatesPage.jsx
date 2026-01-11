import React, { useState, useEffect } from "react";
import { candidatesAPI } from "../services/api";
import { useAuth } from "../contexts/AuthContext";
import toast, { Toaster } from "react-hot-toast";

const CandidatesPage = () => {
  const [candidates, setCandidates] = useState([]);
  const [loading, setLoading] = useState(true);
  const [search, setSearch] = useState("");
  const [collegeFilter, setCollegeFilter] = useState("");
  const [page, setPage] = useState(1);
  const [totalPages, setTotalPages] = useState(1);
  const [totalCount, setTotalCount] = useState(0);
  const [selectedCandidate, setSelectedCandidate] = useState(null);
  const [showModal, setShowModal] = useState(false);
  const [modalMode, setModalMode] = useState("view"); // view, create, edit
  const [showBulkImport, setShowBulkImport] = useState(false);
  const { hasRole } = useAuth();

  const limit = 10;

  useEffect(() => {
    loadCandidates();
  }, [page]);

  const loadCandidates = async () => {
    try {
      setLoading(true);
      const response = await candidatesAPI.search(search, "", page, limit);
      setCandidates(response.candidates || response);
      setTotalPages(response.totalPages || 1);
      setTotalCount(response.totalCount || response.length);
    } catch (error) {
      console.error("Error loading candidates:", error);
      toast.error("Failed to load candidates");
    } finally {
      setLoading(false);
    }
  };

  const handleSearch = () => {
    setPage(1);
    loadCandidates();
  };

  const clearFilters = () => {
    setSearch("");
    setCollegeFilter("");
    setPage(1);
    setTimeout(loadCandidates, 0);
  };

  const openCreateModal = () => {
    setSelectedCandidate(null);
    setModalMode("create");
    setShowModal(true);
  };

  const openEditModal = (candidate) => {
    setSelectedCandidate(candidate);
    setModalMode("edit");
    setShowModal(true);
  };

  const viewProfile = async (candidateId) => {
    try {
      const candidate = await candidatesAPI.getById(candidateId);
      setSelectedCandidate(candidate);
      setModalMode("view");
      setShowModal(true);
    } catch (error) {
      console.error("Error loading candidate:", error);
      toast.error("Failed to load candidate details");
    }
  };

  const handleDelete = async (candidateId) => {
    if (!window.confirm("Are you sure you want to delete this candidate?"))
      return;

    try {
      await candidatesAPI.delete(candidateId);
      toast.success("Candidate deleted successfully");
      loadCandidates();
    } catch (error) {
      console.error("Error deleting candidate:", error);
      toast.error("Failed to delete candidate");
    }
  };

  const closeModal = () => {
    setShowModal(false);
    setSelectedCandidate(null);
  };

  if (!hasRole(["Admin", "Recruiter", "HR"])) {
    return (
      <div className="p-6">
        <div className="text-center">
          <h1 className="text-2xl font-bold text-red-600">Access Denied</h1>
          <p className="text-gray-600">
            You don't have permission to view candidates.
          </p>
        </div>
      </div>
    );
  }

  const filteredCandidates = collegeFilter
    ? candidates.filter((c) =>
        c.college?.toLowerCase().includes(collegeFilter.toLowerCase())
      )
    : candidates;

  return (
    <div className="p-6">
      <Toaster position="top-right" />

      <div className="mb-6 flex justify-between items-center">
        <h1 className="text-2xl font-bold">Candidates</h1>
        <div className="flex gap-2">
          <button
            onClick={() => setShowBulkImport(true)}
            className="bg-green-600 text-white px-4 py-2 rounded hover:bg-green-700"
          >
            📥 Bulk Import
          </button>
          <button
            onClick={openCreateModal}
            className="bg-blue-600 text-white px-4 py-2 rounded hover:bg-blue-700"
          >
            + Add Candidate
          </button>
        </div>
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
              placeholder="Name, email, or phone..."
              className="w-full border rounded px-3 py-2"
              onKeyPress={(e) => e.key === "Enter" && handleSearch()}
            />
          </div>
          <div>
            <label className="block text-sm font-medium mb-2">College</label>
            <input
              type="text"
              value={collegeFilter}
              onChange={(e) => setCollegeFilter(e.target.value)}
              placeholder="Filter by college..."
              className="w-full border rounded px-3 py-2"
            />
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
            Found {totalCount} candidates
          </div>

          {/* Candidates Table */}
          <div className="bg-white shadow rounded-lg overflow-hidden">
            <table className="min-w-full divide-y divide-gray-200">
              <thead className="bg-gray-50">
                <tr>
                  <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">
                    Name
                  </th>
                  <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">
                    Email
                  </th>
                  <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">
                    College
                  </th>
                  <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">
                    Skills
                  </th>
                  <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">
                    Applications
                  </th>
                  <th className="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase">
                    Actions
                  </th>
                </tr>
              </thead>
              <tbody className="bg-white divide-y divide-gray-200">
                {filteredCandidates.map((candidate) => (
                  <tr key={candidate.id} className="hover:bg-gray-50">
                    <td className="px-6 py-4 whitespace-nowrap">
                      <div className="font-medium text-gray-900">
                        {candidate.fullName || "N/A"}
                      </div>
                      {candidate.contactNumber && (
                        <div className="text-sm text-gray-500">
                          {candidate.contactNumber}
                        </div>
                      )}
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                      {candidate.email}
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap">
                      <div className="text-sm text-gray-900">
                        {candidate.college || "-"}
                      </div>
                      {candidate.graduationYear && (
                        <div className="text-xs text-gray-500">
                          Class of {candidate.graduationYear}
                        </div>
                      )}
                    </td>
                    <td className="px-6 py-4">
                      <div className="flex flex-wrap gap-1">
                        {candidate.skills && candidate.skills.length > 0 ? (
                          candidate.skills.slice(0, 3).map((skill, idx) => (
                            <span
                              key={idx}
                              className="bg-blue-100 text-blue-800 text-xs px-2 py-1 rounded"
                            >
                              {skill.skillName || skill}
                            </span>
                          ))
                        ) : (
                          <span className="text-gray-400 text-sm">None</span>
                        )}
                        {candidate.skills && candidate.skills.length > 3 && (
                          <span className="text-xs text-gray-500">
                            +{candidate.skills.length - 3}
                          </span>
                        )}
                      </div>
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                      {candidate.applicationCount || candidate.totalApplications || 0}
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap text-right text-sm font-medium">
                      <button
                        onClick={() => viewProfile(candidate.id)}
                        className="text-blue-600 hover:text-blue-900 mr-3"
                      >
                        View
                      </button>
                      <button
                        onClick={() => openEditModal(candidate)}
                        className="text-green-600 hover:text-green-900 mr-3"
                      >
                        Edit
                      </button>
                      <button
                        onClick={() => handleDelete(candidate.id)}
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
      {showModal && (
        <CandidateModal
          candidate={selectedCandidate}
          mode={modalMode}
          onClose={closeModal}
          onSuccess={() => {
            closeModal();
            loadCandidates();
          }}
        />
      )}

      {showBulkImport && (
        <BulkImportModal
          onClose={() => setShowBulkImport(false)}
          onSuccess={() => {
            setShowBulkImport(false);
            loadCandidates();
          }}
        />
      )}
    </div>
  );
};

// Candidate Modal (View/Create/Edit)
const CandidateModal = ({ candidate, mode, onClose, onSuccess }) => {
  const [formData, setFormData] = useState({
    fullName: "",
    email: "",
    contactNumber: "",
    college: "",
    graduationYear: "",
    password: "",
  });
  const [saving, setSaving] = useState(false);

  useEffect(() => {
    if (candidate && (mode === "view" || mode === "edit")) {
      setFormData({
        fullName: candidate.fullName || "",
        email: candidate.email || "",
        contactNumber: candidate.contactNumber || "",
        college: candidate.college || "",
        graduationYear: candidate.graduationYear || "",
        password: "",
      });
    }
  }, [candidate, mode]);

  const handleChange = (e) => {
    setFormData({ ...formData, [e.target.name]: e.target.value });
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setSaving(true);

    try {
      if (mode === "create") {
        await candidatesAPI.create(formData);
        toast.success("Candidate created successfully");
      } else if (mode === "edit") {
        await candidatesAPI.update(candidate.id, formData);
        toast.success("Candidate updated successfully");
      }
      onSuccess();
    } catch (error) {
      console.error("Error saving candidate:", error);
      toast.error(error.response?.data?.message || "Failed to save candidate");
    } finally {
      setSaving(false);
    }
  };

  const isViewMode = mode === "view";

  return (
    <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
      <div className="bg-white p-6 rounded-lg w-11/12 md:w-2/3 lg:w-1/2 max-h-screen overflow-y-auto">
        <div className="flex justify-between items-center mb-4">
          <h2 className="text-xl font-bold">
            {mode === "create"
              ? "Add New Candidate"
              : mode === "edit"
              ? "Edit Candidate"
              : "Candidate Profile"}
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
            <label className="block text-sm font-medium mb-1">Full Name *</label>
            <input
              type="text"
              name="fullName"
              value={formData.fullName}
              onChange={handleChange}
              disabled={isViewMode}
              required
              className="w-full border rounded px-3 py-2 disabled:bg-gray-100"
            />
          </div>

          <div>
            <label className="block text-sm font-medium mb-1">Email *</label>
            <input
              type="email"
              name="email"
              value={formData.email}
              onChange={handleChange}
              disabled={isViewMode || mode === "edit"}
              required
              className="w-full border rounded px-3 py-2 disabled:bg-gray-100"
            />
          </div>

          {mode === "create" && (
            <div>
              <label className="block text-sm font-medium mb-1">
                Password *
              </label>
              <input
                type="password"
                name="password"
                value={formData.password}
                onChange={handleChange}
                required
                className="w-full border rounded px-3 py-2"
              />
            </div>
          )}

          <div>
            <label className="block text-sm font-medium mb-1">
              Contact Number
            </label>
            <input
              type="tel"
              name="contactNumber"
              value={formData.contactNumber}
              onChange={handleChange}
              disabled={isViewMode}
              className="w-full border rounded px-3 py-2 disabled:bg-gray-100"
            />
          </div>

          <div>
            <label className="block text-sm font-medium mb-1">College</label>
            <input
              type="text"
              name="college"
              value={formData.college}
              onChange={handleChange}
              disabled={isViewMode}
              placeholder="e.g., IIT Delhi, Stanford University"
              className="w-full border rounded px-3 py-2 disabled:bg-gray-100"
            />
          </div>

          <div>
            <label className="block text-sm font-medium mb-1">
              Graduation Year
            </label>
            <input
              type="number"
              name="graduationYear"
              value={formData.graduationYear}
              onChange={handleChange}
              disabled={isViewMode}
              placeholder="e.g., 2024"
              min="1950"
              max="2030"
              className="w-full border rounded px-3 py-2 disabled:bg-gray-100"
            />
          </div>

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

          {isViewMode && candidate && (
            <div className="mt-4 pt-4 border-t">
              <h3 className="font-semibold mb-2">Additional Information</h3>
              <div className="text-sm space-y-2">
                <p>
                  <span className="font-medium">Applications:</span>{" "}
                  {candidate.applicationCount || candidate.totalApplications || 0}
                </p>
                <p>
                  <span className="font-medium">Skills:</span>{" "}
                  {candidate.skills?.length || 0}
                </p>
              </div>
            </div>
          )}
        </form>
      </div>
    </div>
  );
};

// Bulk Import Modal
const BulkImportModal = ({ onClose, onSuccess }) => {
  const [file, setFile] = useState(null);
  const [uploading, setUploading] = useState(false);

  const handleFileChange = (e) => {
    setFile(e.target.files[0]);
  };

  const handleUpload = async () => {
    if (!file) {
      toast.error("Please select a file");
      return;
    }

    setUploading(true);
    const formData = new FormData();
    formData.append("file", file);

    try {
      // You'll need to add this endpoint to your API
      const response = await fetch(
        "http://localhost:5153/api/v1/candidates/bulk-import",
        {
          method: "POST",
          headers: {
            Authorization: `Bearer ${localStorage.getItem("token")}`,
          },
          body: formData,
        }
      );

      if (!response.ok) throw new Error("Upload failed");

      const result = await response.json();
      toast.success(
        `Successfully imported ${result.successCount} candidates`
      );
      onSuccess();
    } catch (error) {
      console.error("Error uploading file:", error);
      toast.error("Failed to import candidates");
    } finally {
      setUploading(false);
    }
  };

  return (
    <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
      <div className="bg-white p-6 rounded-lg w-11/12 md:w-1/2">
        <div className="flex justify-between items-center mb-4">
          <h2 className="text-xl font-bold">Bulk Import Candidates</h2>
          <button
            onClick={onClose}
            className="text-gray-500 hover:text-gray-700 text-2xl"
          >
            ×
          </button>
        </div>

        <div className="space-y-4">
          <div>
            <p className="text-sm text-gray-600 mb-2">
              Upload an Excel file (.xlsx) with candidate data. Required columns:
              FullName, Email, ContactNumber (optional), College (optional),
              GraduationYear (optional), Skills (optional).
            </p>
            <a
              href="/template/candidates-template.xlsx"
              className="text-blue-600 hover:underline text-sm"
            >
              Download Template
            </a>
          </div>

          <div>
            <input
              type="file"
              accept=".xlsx,.xls"
              onChange={handleFileChange}
              className="w-full border rounded px-3 py-2"
            />
          </div>

          <div className="flex justify-end gap-2">
            <button
              onClick={onClose}
              className="px-4 py-2 border rounded hover:bg-gray-50"
            >
              Cancel
            </button>
            <button
              onClick={handleUpload}
              disabled={!file || uploading}
              className="bg-blue-600 text-white px-4 py-2 rounded hover:bg-blue-700 disabled:opacity-50"
            >
              {uploading ? "Uploading..." : "Upload"}
            </button>
          </div>
        </div>
      </div>
    </div>
  );
};

export default CandidatesPage;
