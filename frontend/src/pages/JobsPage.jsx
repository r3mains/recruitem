import React, { useState, useEffect } from "react";
import { jobsAPI, positionsAPI, skillsAPI, jobTypesAPI, addressesAPI } from "../services/api";
import { useAuth } from "../contexts/AuthContext";
import toast, { Toaster } from "react-hot-toast";

const JobsPage = () => {
  const [jobs, setJobs] = useState([]);
  const [loading, setLoading] = useState(true);
  const [search, setSearch] = useState("");
  const [statusFilter, setStatusFilter] = useState("");
  const [page, setPage] = useState(1);
  const [totalPages, setTotalPages] = useState(1);
  const [totalCount, setTotalCount] = useState(0);
  const [selectedJob, setSelectedJob] = useState(null);
  const [showModal, setShowModal] = useState(false);
  const [modalMode, setModalMode] = useState("view"); // view, create, edit
  const [positions, setPositions] = useState([]);
  const [skills, setSkills] = useState([]);
  const [jobTypes, setJobTypes] = useState([]);
  const [addresses, setAddresses] = useState([]);
  const { hasRole } = useAuth();

  const limit = 10;

  useEffect(() => {
    loadInitialData();
  }, []);

  useEffect(() => {
    loadJobs();
  }, [page, statusFilter]);

  const loadInitialData = async () => {
    try {
      const [positionsData, skillsData, jobTypesData, addressesData] = await Promise.all([
        positionsAPI.getAll(),
        skillsAPI.getAll(),
        jobTypesAPI.getAll(),
        addressesAPI.getAll(),
      ]);
      setPositions(positionsData.positions || positionsData || []);
      setSkills(skillsData.Skills || skillsData.skills || skillsData || []);
      setJobTypes(jobTypesData.jobTypes || jobTypesData || []);
      setAddresses(addressesData || []);
    } catch (error) {
      console.error("Error loading initial data:", error);
    }
  };

  const loadJobs = async () => {
    try {
      setLoading(true);
      const params = new URLSearchParams({
        page,
        pageSize: limit,
      });
      if (search) params.append("search", search);
      if (statusFilter) params.append("statusId", statusFilter);

      const response = await fetch(
        `http://localhost:5153/api/v1/jobs?${params}`,
        {
          headers: {
            Authorization: `Bearer ${localStorage.getItem("token")}`,
          },
        }
      );
      const data = await response.json();

      setJobs(data.jobs || []);
      setTotalPages(data.pagination?.totalPages || 1);
      setTotalCount(data.pagination?.totalCount || 0);
    } catch (error) {
      console.error("Error loading jobs:", error);
      toast.error("Failed to load jobs");
    } finally {
      setLoading(false);
    }
  };

  const handleSearch = () => {
    setPage(1);
    loadJobs();
  };

  const clearFilters = () => {
    setSearch("");
    setStatusFilter("");
    setPage(1);
    setTimeout(loadJobs, 0);
  };

  const openCreateModal = () => {
    setSelectedJob(null);
    setModalMode("create");
    setShowModal(true);
  };

  const openEditModal = (job) => {
    setSelectedJob(job);
    setModalMode("edit");
    setShowModal(true);
  };

  const viewDetails = async (jobId) => {
    try {
      const response = await fetch(
        `http://localhost:5153/api/v1/jobs/${jobId}`,
        {
          headers: {
            Authorization: `Bearer ${localStorage.getItem("token")}`,
          },
        }
      );
      const job = await response.json();
      setSelectedJob(job);
      setModalMode("view");
      setShowModal(true);
    } catch (error) {
      console.error("Error loading job:", error);
      toast.error("Failed to load job details");
    }
  };

  const handleDelete = async (jobId) => {
    if (!window.confirm("Are you sure you want to delete this job?")) return;

    try {
      await fetch(`http://localhost:5153/api/v1/jobs/${jobId}`, {
        method: "DELETE",
        headers: {
          Authorization: `Bearer ${localStorage.getItem("token")}`,
        },
      });
      toast.success("Job deleted successfully");
      loadJobs();
    } catch (error) {
      console.error("Error deleting job:", error);
      toast.error("Failed to delete job");
    }
  };

  const closeModal = () => {
    setShowModal(false);
    setSelectedJob(null);
  };

  if (!hasRole(["Admin", "Recruiter", "HR"])) {
    return (
      <div className="p-6">
        <div className="text-center">
          <h1 className="text-2xl font-bold text-red-600">Access Denied</h1>
          <p className="text-gray-600">
            You don't have permission to manage jobs.
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
        <h1 className="text-2xl font-bold">Job Postings</h1>
        <button
          onClick={openCreateModal}
          className="bg-blue-600 text-white px-4 py-2 rounded hover:bg-blue-700"
        >
          + Create Job
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
              placeholder="Job title..."
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
            Found {totalCount} jobs
          </div>

          {/* Jobs Table */}
          <div className="bg-white shadow rounded-lg overflow-hidden">
            <table className="min-w-full divide-y divide-gray-200">
              <thead className="bg-gray-50">
                <tr>
                  <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">
                    Title
                  </th>
                  <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">
                    Type
                  </th>
                  <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">
                    Location
                  </th>
                  <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">
                    Status
                  </th>
                  <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">
                    Salary
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
                {jobs.map((job) => (
                  <tr key={job.id} className="hover:bg-gray-50">
                    <td className="px-6 py-4">
                      <div className="font-medium text-gray-900">
                        {job.title}
                      </div>
                      <div className="text-xs text-gray-500">
                        {job.recruiterName}
                      </div>
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                      {job.jobType}
                    </td>
                    <td className="px-6 py-4 text-sm text-gray-500">
                      {job.location || "N/A"}
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap">
                      <span
                        className={`px-2 py-1 text-xs rounded-full ${getStatusBadge(
                          job.status
                        )}`}
                      >
                        {job.status}
                      </span>
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                      {job.salaryMin && job.salaryMax
                        ? `$${job.salaryMin.toLocaleString()} - $${job.salaryMax.toLocaleString()}`
                        : "Not specified"}
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                      {job.applicationCount || 0}
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap text-right text-sm font-medium">
                      <button
                        onClick={() => viewDetails(job.id)}
                        className="text-blue-600 hover:text-blue-900 mr-3"
                      >
                        View
                      </button>
                      <button
                        onClick={() => openEditModal(job)}
                        className="text-green-600 hover:text-green-900 mr-3"
                      >
                        Edit
                      </button>
                      <button
                        onClick={() => handleDelete(job.id)}
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
      {showModal && (modalMode === "create" || modalMode === "edit") && (
        <JobModal
          job={selectedJob}
          mode={modalMode}
          positions={positions}
          skills={skills}
          jobTypes={jobTypes}
          addresses={addresses}
          onClose={closeModal}
          onSuccess={() => {
            closeModal();
            loadJobs();
          }}
        />
      )}

      {showModal && modalMode === "view" && (
        <ViewJobModal job={selectedJob} onClose={closeModal} />
      )}
    </div>
  );
};

// Job Modal (Create/Edit)
const JobModal = ({ job, mode, positions, skills, jobTypes, addresses, onClose, onSuccess }) => {
  const [formData, setFormData] = useState({
    title: "",
    description: "",
    positionId: "",
    jobTypeId: "",
    salaryMin: "",
    salaryMax: "",
    addressId: "",
    requiredSkillIds: [],
    preferredSkillIds: [],
    qualifications: [],
  });
  const [skillSearch, setSkillSearch] = useState("");
  const [saving, setSaving] = useState(false);

  useEffect(() => {
    if (job && mode === "edit") {
      setFormData({
        title: job.title || "",
        description: job.description || "",
        positionId: job.position?.id || "",
        jobTypeId: job.jobType?.id || "",
        salaryMin: job.salaryMin || "",
        salaryMax: job.salaryMax || "",
        addressId: job.address?.id || "",
        requiredSkillIds:
          job.skills?.filter((s) => s.required).map((s) => s.id) || [],
        preferredSkillIds:
          job.skills?.filter((s) => !s.required).map((s) => s.id) || [],
        qualifications: job.qualifications || [],
      });
    }
  }, [job, mode]);

  const handleChange = (e) => {
    setFormData({ ...formData, [e.target.name]: e.target.value });
  };

  const toggleSkill = (skillId, type) => {
    setFormData((prev) => {
      const targetArray = type === "required" ? "requiredSkillIds" : "preferredSkillIds";
      const otherArray = type === "required" ? "preferredSkillIds" : "requiredSkillIds";
      
      return {
        ...prev,
        [targetArray]: prev[targetArray].includes(skillId)
          ? prev[targetArray].filter((id) => id !== skillId)
          : [...prev[targetArray], skillId],
        [otherArray]: prev[otherArray].filter((id) => id !== skillId),
      };
    });
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setSaving(true);

    try {
      // Validate required fields
      if (!formData.title || !formData.description) {
        toast.error("Title and description are required");
        setSaving(false);
        return;
      }

      if (!formData.jobTypeId) {
        toast.error("Job type is required");
        setSaving(false);
        return;
      }

      if (!formData.addressId) {
        toast.error("Address is required");
        setSaving(false);
        return;
      }

      if (mode === "create" && !formData.positionId) {
        toast.error("Position is required");
        setSaving(false);
        return;
      }

      const payload = {
        title: formData.title,
        description: formData.description,
        jobTypeId: formData.jobTypeId,
        addressId: formData.addressId,
        requiredSkillIds: formData.requiredSkillIds,
        preferredSkillIds: formData.preferredSkillIds,
        qualifications: formData.qualifications,
      };

      // Add optional salary fields if provided
      if (formData.salaryMin) payload.salaryMin = parseFloat(formData.salaryMin);
      if (formData.salaryMax) payload.salaryMax = parseFloat(formData.salaryMax);

      if (mode === "create") {
        payload.positionId = formData.positionId;
        await jobsAPI.create(payload);
        toast.success("Job created successfully");
      } else {
        const response = await fetch(
          `http://localhost:5153/api/v1/jobs/${job.id}`,
          {
            method: "PUT",
            headers: {
              "Content-Type": "application/json",
              Authorization: `Bearer ${localStorage.getItem("token")}`,
            },
            body: JSON.stringify(payload),
          }
        );
        if (!response.ok) throw new Error("Failed to update job");
        toast.success("Job updated successfully");
      }
      onSuccess();
    } catch (error) {
      console.error("Error saving job:", error);
      console.error("Error response:", error.response?.data);
      const errorMsg = error.response?.data?.errors 
        ? Object.values(error.response.data.errors).flat().join(', ')
        : error.response?.data?.message || "Failed to save job";
      toast.error(errorMsg);
    } finally {
      setSaving(false);
    }
  };

  const filteredSkills = skills.filter((skill) =>
    skill.skillName?.toLowerCase().includes(skillSearch.toLowerCase())
  );

  return (
    <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
      <div className="bg-white p-6 rounded-lg w-11/12 md:w-2/3 max-h-screen overflow-y-auto">
        <div className="flex justify-between items-center mb-4">
          <h2 className="text-xl font-bold">
            {mode === "create" ? "Create Job" : "Edit Job"}
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
              required
              placeholder="e.g., Senior Software Engineer"
              className="w-full border rounded px-3 py-2"
            />
          </div>

          <div>
            <label className="block text-sm font-medium mb-1">
              Description *
            </label>
            <textarea
              name="description"
              value={formData.description}
              onChange={handleChange}
              required
              rows="4"
              placeholder="Job description, responsibilities, requirements..."
              className="w-full border rounded px-3 py-2"
            />
          </div>

          <div className="grid grid-cols-2 gap-4">
            <div>
              <label className="block text-sm font-medium mb-1">
                Position *
              </label>
              <select
                name="positionId"
                value={formData.positionId}
                onChange={handleChange}
                required
                className="w-full border rounded px-3 py-2"
              >
                <option value="">Select Position</option>
                {positions.map((pos) => (
                  <option key={pos.id} value={pos.id}>
                    {pos.title}
                  </option>
                ))}
              </select>
            </div>

            <div>
              <label className="block text-sm font-medium mb-1">
                Job Type *
              </label>
              <select
                name="jobTypeId"
                value={formData.jobTypeId}
                onChange={handleChange}
                required
                className="w-full border rounded px-3 py-2"
              >
                <option value="">Select Type</option>
                {jobTypes.map((type) => (
                  <option key={type.id} value={type.id}>
                    {type.type}
                  </option>
                ))}
              </select>
            </div>
          </div>

          <div>
            <label className="block text-sm font-medium mb-1">
              Location
            </label>
            <select
              name="addressId"
              value={formData.addressId}
              onChange={handleChange}
              className="w-full border rounded px-3 py-2"
            >
              <option value="">Select Location (Optional)</option>
              {addresses.map((addr) => (
                <option key={addr.id} value={addr.id}>
                  {[addr.addressLine1, addr.cityName, addr.stateName]
                    .filter(Boolean)
                    .join(", ")}
                </option>
              ))}
            </select>
          </div>

          <div className="grid grid-cols-2 gap-4">
            <div>
              <label className="block text-sm font-medium mb-1">
                Salary Min
              </label>
              <input
                type="number"
                name="salaryMin"
                value={formData.salaryMin}
                onChange={handleChange}
                placeholder="50000"
                className="w-full border rounded px-3 py-2"
              />
            </div>

            <div>
              <label className="block text-sm font-medium mb-1">
                Salary Max
              </label>
              <input
                type="number"
                name="salaryMax"
                value={formData.salaryMax}
                onChange={handleChange}
                placeholder="80000"
                className="w-full border rounded px-3 py-2"
              />
            </div>
          </div>

          <div>
            <label className="block text-sm font-medium mb-1">Skills</label>
            <input
              type="text"
              value={skillSearch}
              onChange={(e) => setSkillSearch(e.target.value)}
              placeholder="Search skills..."
              className="w-full border rounded px-3 py-2 mb-2"
            />
            <div className="border rounded p-3 max-h-48 overflow-y-auto">
              <div className="mb-2 font-medium text-sm">Required Skills</div>
              {filteredSkills.map((skill) => (
                <label
                  key={`req-${skill.id}`}
                  className="flex items-center gap-2 py-1 cursor-pointer hover:bg-gray-50"
                >
                  <input
                    type="checkbox"
                    checked={formData.requiredSkillIds.includes(skill.id)}
                    onChange={() => toggleSkill(skill.id, "required")}
                    className="rounded"
                  />
                  <span>{skill.skillName}</span>
                </label>
              ))}
              <div className="mt-3 mb-2 font-medium text-sm">Preferred Skills</div>
              {filteredSkills.map((skill) => (
                <label
                  key={`pref-${skill.id}`}
                  className="flex items-center gap-2 py-1 cursor-pointer hover:bg-gray-50"
                >
                  <input
                    type="checkbox"
                    checked={formData.preferredSkillIds.includes(skill.id)}
                    onChange={() => toggleSkill(skill.id, "preferred")}
                    className="rounded"
                  />
                  <span>{skill.skillName}</span>
                </label>
              ))}
            </div>
            <div className="mt-2 text-sm text-gray-600">
              {formData.requiredSkillIds.length} required, {formData.preferredSkillIds.length} preferred
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
              {saving ? "Saving..." : mode === "create" ? "Create" : "Update"}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
};

// View Job Modal
const ViewJobModal = ({ job, onClose }) => {
  return (
    <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
      <div className="bg-white p-6 rounded-lg w-11/12 md:w-2/3 max-h-screen overflow-y-auto">
        <div className="flex justify-between items-center mb-4">
          <h2 className="text-xl font-bold">Job Details</h2>
          <button
            onClick={onClose}
            className="text-gray-500 hover:text-gray-700 text-2xl"
          >
            ×
          </button>
        </div>

        <div className="space-y-6">
          <div>
            <h3 className="text-lg font-semibold mb-3">Basic Information</h3>
            <div className="grid grid-cols-2 gap-4 text-sm">
              <div>
                <span className="font-medium">Title:</span> {job.title}
              </div>
              <div>
                <span className="font-medium">Position:</span>{" "}
                {job.position?.title}
              </div>
              <div>
                <span className="font-medium">Type:</span> {job.jobType?.type}
              </div>
              <div>
                <span className="font-medium">Status:</span>{" "}
                <span
                  className={`px-2 py-1 text-xs rounded ${
                    job.status?.status === "Open"
                      ? "bg-green-100 text-green-800"
                      : "bg-red-100 text-red-800"
                  }`}
                >
                  {job.status?.status}
                </span>
              </div>
              <div>
                <span className="font-medium">Recruiter:</span>{" "}
                {job.recruiter?.fullName}
              </div>
              <div>
                <span className="font-medium">Applications:</span>{" "}
                {job.applicationCount || 0}
              </div>
              {job.salaryMin && job.salaryMax && (
                <div className="col-span-2">
                  <span className="font-medium">Salary Range:</span> $
                  {job.salaryMin.toLocaleString()} - $
                  {job.salaryMax.toLocaleString()}
                </div>
              )}
            </div>
          </div>

          <div>
            <h3 className="text-lg font-semibold mb-3">Description</h3>
            <p className="text-sm text-gray-700 whitespace-pre-wrap">
              {job.description}
            </p>
          </div>

          {job.address && (
            <div>
              <h3 className="text-lg font-semibold mb-3">Location</h3>
              <p className="text-sm text-gray-700">
                {[
                  job.address.addressLine1,
                  job.address.addressLine2,
                  job.address.locality,
                  job.address.cityName,
                  job.address.stateName,
                  job.address.pincode,
                  job.address.countryName,
                ]
                  .filter(Boolean)
                  .join(", ")}
              </p>
            </div>
          )}

          {job.skills && job.skills.length > 0 && (
            <div>
              <h3 className="text-lg font-semibold mb-3">Skills</h3>
              <div>
                <div className="mb-2 font-medium text-sm">Required:</div>
                <div className="flex flex-wrap gap-2 mb-3">
                  {job.skills
                    .filter((s) => s.required)
                    .map((skill) => (
                      <span
                        key={skill.id}
                        className="bg-blue-100 text-blue-800 text-xs px-2 py-1 rounded"
                      >
                        {skill.skillName}
                      </span>
                    ))}
                </div>
                <div className="mb-2 font-medium text-sm">Preferred:</div>
                <div className="flex flex-wrap gap-2">
                  {job.skills
                    .filter((s) => !s.required)
                    .map((skill) => (
                      <span
                        key={skill.id}
                        className="bg-gray-100 text-gray-800 text-xs px-2 py-1 rounded"
                      >
                        {skill.skillName}
                      </span>
                    ))}
                </div>
              </div>
            </div>
          )}

          {job.qualifications && job.qualifications.length > 0 && (
            <div>
              <h3 className="text-lg font-semibold mb-3">Qualifications</h3>
              <ul className="list-disc list-inside text-sm text-gray-700">
                {job.qualifications.map((qual) => (
                  <li key={qual.id}>
                    {qual.qualificationName}
                    {qual.minRequired && ` (Min: ${qual.minRequired})`}
                  </li>
                ))}
              </ul>
            </div>
          )}

          <div className="text-xs text-gray-500">
            <p>Created: {new Date(job.createdAt).toLocaleString()}</p>
            <p>Updated: {new Date(job.updatedAt).toLocaleString()}</p>
          </div>
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

export default JobsPage;
