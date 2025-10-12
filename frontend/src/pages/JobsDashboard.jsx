import React, { useState, useEffect, useCallback } from "react";
import { useApi } from "../contexts/ApiContext";

const JobForm = ({
  job,
  jobTypes,
  statusTypes,
  positions,
  addresses,
  onSave,
  onCancel,
}) => {
  const [formData, setFormData] = useState({
    title: job?.title || "",
    description: job?.description || "",
    jobTypeId: job?.jobTypeId || "",
    locationId: job?.locationId || "",
    salaryMin: job?.salaryMin || "",
    salaryMax: job?.salaryMax || "",
    positionId: job?.positionId || "",
    statusId: job?.statusId || "",
  });
  const [saving, setSaving] = useState(false);

  const handleSubmit = async (e) => {
    e.preventDefault();
    setSaving(true);

    if (
      !formData.locationId ||
      !formData.positionId ||
      !formData.jobTypeId ||
      !formData.statusId
    ) {
      alert(
        "Please fill in all required fields (Location, Position, Job Type, Status)"
      );
      setSaving(false);
      return;
    }

    const jobData = {
      title: formData.title,
      description: formData.description,
      jobTypeId: formData.jobTypeId,
      locationId: formData.locationId,
      positionId: formData.positionId,
      statusId: formData.statusId,
      salaryMin: formData.salaryMin ? parseFloat(formData.salaryMin) : null,
      salaryMax: formData.salaryMax ? parseFloat(formData.salaryMax) : null,
      recruiterId: null,
    };

    await onSave(jobData);
    setSaving(false);
  };

  return (
    <div className="fixed inset-0 bg-gray-600 bg-opacity-50 overflow-y-auto h-full w-full z-50">
      <div className="relative top-20 mx-auto p-5 border w-11/12 md:w-3/4 lg:w-1/2 shadow-lg rounded-md bg-white">
        <div className="mt-3">
          <h3 className="text-lg font-medium text-gray-900 mb-4">
            {job ? "Edit Job" : "Add New Job"}
          </h3>

          <form onSubmit={handleSubmit} className="space-y-4">
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">
                Job Title *
              </label>
              <input
                type="text"
                required
                value={formData.title}
                onChange={(e) =>
                  setFormData((prev) => ({ ...prev, title: e.target.value }))
                }
                className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-primary-500 focus:border-primary-500"
                placeholder="e.g. Senior Software Engineer"
              />
            </div>

            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">
                Job Description *
              </label>
              <textarea
                required
                rows={4}
                value={formData.description}
                onChange={(e) =>
                  setFormData((prev) => ({
                    ...prev,
                    description: e.target.value,
                  }))
                }
                className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-primary-500 focus:border-primary-500"
                placeholder="Describe the job responsibilities, requirements, and qualifications..."
              />
            </div>

            <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">
                  Job Type *
                </label>
                <select
                  required
                  value={formData.jobTypeId}
                  onChange={(e) =>
                    setFormData((prev) => ({
                      ...prev,
                      jobTypeId: e.target.value,
                    }))
                  }
                  className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-primary-500 focus:border-primary-500"
                >
                  <option value="">Select Job Type</option>
                  {jobTypes.map((type) => (
                    <option key={type.id} value={type.id}>
                      {type.type}
                    </option>
                  ))}
                </select>
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">
                  Status *
                </label>
                <select
                  required
                  value={formData.statusId}
                  onChange={(e) =>
                    setFormData((prev) => ({
                      ...prev,
                      statusId: e.target.value,
                    }))
                  }
                  className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-primary-500 focus:border-primary-500"
                >
                  <option value="">Select Status</option>
                  {statusTypes
                    .filter((s) => s.context === "Job")
                    .map((status) => (
                      <option key={status.id} value={status.id}>
                        {status.status}
                      </option>
                    ))}
                </select>
              </div>
            </div>

            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">
                Location *
              </label>
              <select
                required
                value={formData.locationId}
                onChange={(e) =>
                  setFormData((prev) => ({
                    ...prev,
                    locationId: e.target.value,
                  }))
                }
                className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-primary-500 focus:border-primary-500"
              >
                <option value="">Select Location</option>
                {addresses.map((address) => (
                  <option key={address.id} value={address.id}>
                    {address.street}, {address.city?.name},{" "}
                    {address.state?.name} {address.zipCode}
                  </option>
                ))}
              </select>
            </div>

            <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">
                  Minimum Salary
                </label>
                <input
                  type="number"
                  min="0"
                  step="1000"
                  value={formData.salaryMin}
                  onChange={(e) =>
                    setFormData((prev) => ({
                      ...prev,
                      salaryMin: e.target.value,
                    }))
                  }
                  className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-primary-500 focus:border-primary-500"
                  placeholder="e.g. 50000"
                />
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">
                  Maximum Salary
                </label>
                <input
                  type="number"
                  min="0"
                  step="1000"
                  value={formData.salaryMax}
                  onChange={(e) =>
                    setFormData((prev) => ({
                      ...prev,
                      salaryMax: e.target.value,
                    }))
                  }
                  className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-primary-500 focus:border-primary-500"
                  placeholder="e.g. 80000"
                />
              </div>
            </div>

            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">
                Position *
              </label>
              <select
                required
                value={formData.positionId}
                onChange={(e) =>
                  setFormData((prev) => ({
                    ...prev,
                    positionId: e.target.value,
                  }))
                }
                className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-primary-500 focus:border-primary-500"
              >
                <option value="">Select Position</option>
                {positions.map((position) => (
                  <option key={position.id} value={position.id}>
                    {position.title} - {position.level}
                  </option>
                ))}
              </select>
            </div>

            <div className="flex justify-end space-x-3 pt-4">
              <button
                type="button"
                onClick={onCancel}
                disabled={saving}
                className="px-4 py-2 border border-gray-300 rounded-md text-gray-700 bg-white hover:bg-gray-50 focus:outline-none focus:ring-2 focus:ring-primary-500 disabled:opacity-50"
              >
                Cancel
              </button>
              <button
                type="submit"
                disabled={saving}
                className="px-4 py-2 bg-primary-600 text-white rounded-md hover:bg-primary-700 focus:outline-none focus:ring-2 focus:ring-primary-500 disabled:opacity-50"
              >
                {saving ? "Saving..." : job ? "Update Job" : "Create Job"}
              </button>
            </div>
          </form>
        </div>
      </div>
    </div>
  );
};

const QuickSetup = ({ statusTypes, onRefresh }) => {
  const { positions, addresses } = useApi();
  const [creating, setCreating] = useState(false);

  const createSamplePositions = async () => {
    setCreating(true);
    try {
      const openStatus = statusTypes.find(
        (s) => s.context === "Job" && s.status === "Open"
      );
      const samplePositions = [
        {
          title: "Senior Software Engineer",
          statusId: openStatus?.id,
          numberOfInterviews: 3,
        },
        {
          title: "Frontend Developer",
          statusId: openStatus?.id,
          numberOfInterviews: 2,
        },
        {
          title: "Data Scientist",
          statusId: openStatus?.id,
          numberOfInterviews: 4,
        },
        {
          title: "Product Manager",
          statusId: openStatus?.id,
          numberOfInterviews: 3,
        },
      ];

      for (const position of samplePositions) {
        await positions.create(position);
      }

      alert("Sample positions created successfully!");
      onRefresh();
    } catch {
      alert("Failed to create positions");
    } finally {
      setCreating(false);
    }
  };

  const createSampleAddress = async () => {
    setCreating(true);
    try {
      const sampleAddresses = [
        {
          addressLine1: "123 Tech Street",
          locality: "Downtown",
          pincode: "10001",
        },
        {
          addressLine1: "456 Innovation Ave",
          locality: "Tech District",
          pincode: "10002",
        },
        {
          addressLine1: "789 Startup Blvd",
          locality: "Business Center",
          pincode: "10003",
        },
      ];

      for (const address of sampleAddresses) {
        await addresses.create(address);
      }

      alert("Sample addresses created successfully!");
      onRefresh();
    } catch {
      alert("Failed to create addresses");
    } finally {
      setCreating(false);
    }
  };

  return (
    <div className="bg-yellow-50 border border-yellow-200 rounded-lg p-4 mb-6">
      <h3 className="text-lg font-medium text-yellow-800 mb-2">
        Quick Setup Required
      </h3>
      <p className="text-yellow-700 mb-4">
        You need positions and addresses to create jobs. Click the buttons below
        to create sample data:
      </p>
      <div className="flex space-x-3">
        <button
          onClick={createSamplePositions}
          disabled={creating}
          className="px-4 py-2 bg-blue-600 text-white rounded-md hover:bg-blue-700 disabled:opacity-50"
        >
          {creating ? "Creating..." : "Create Sample Positions"}
        </button>
        <button
          onClick={createSampleAddress}
          disabled={creating}
          className="px-4 py-2 bg-green-600 text-white rounded-md hover:bg-green-700 disabled:opacity-50"
        >
          {creating ? "Creating..." : "Create Sample Addresses"}
        </button>
      </div>
    </div>
  );
};

const JobsDashboard = () => {
  const { jobs, lookups, positions, addresses } = useApi();
  const [jobsList, setJobsList] = useState([]);
  const [jobTypes, setJobTypes] = useState([]);
  const [statusTypes, setStatusTypes] = useState([]);
  const [positionsList, setPositionsList] = useState([]);
  const [addressesList, setAddressesList] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [showJobForm, setShowJobForm] = useState(false);
  const [editingJob, setEditingJob] = useState(null);
  const [filters, setFilters] = useState({
    recruiterId: "",
    statusId: "",
    positionId: "",
  });

  const loadInitialData = useCallback(async () => {
    try {
      setLoading(true);
      setError(null);

      const [
        jobsData,
        jobTypesData,
        statusTypesData,
        positionsData,
        addressesData,
      ] = await Promise.all([
        jobs.getAll({}),
        lookups.getJobTypes(),
        lookups.getStatusTypes(),
        positions.getAll(),
        addresses.getAll(),
      ]);

      setJobsList(jobsData);
      setJobTypes(jobTypesData);
      setStatusTypes(statusTypesData);
      setPositionsList(positionsData);
      setAddressesList(addressesData);
    } catch (err) {
      setError(err.message || "Failed to load data");
    } finally {
      setLoading(false);
    }
  }, [jobs, lookups, positions, addresses]);

  useEffect(() => {
    loadInitialData();
  }, [jobs, lookups, positions, addresses, loadInitialData]);

  const loadData = async () => {
    try {
      setLoading(true);
      setError(null);

      const [jobsData, jobTypesData, statusTypesData] = await Promise.all([
        jobs.getAll(filters),
        lookups.getJobTypes(),
        lookups.getStatusTypes(),
      ]);

      setJobsList(jobsData);
      setJobTypes(jobTypesData);
      setStatusTypes(statusTypesData);
    } catch (err) {
      setError(err.message || "Failed to load data");
    } finally {
      setLoading(false);
    }
  };

  const handleCreateJob = () => {
    setEditingJob(null);
    setShowJobForm(true);
  };

  const handleEditJob = (job) => {
    setEditingJob(job);
    setShowJobForm(true);
  };

  const handleSaveJob = async (jobData) => {
    try {
      if (editingJob) {
        await jobs.update(editingJob.id, jobData);
      } else {
        await jobs.create(jobData);
      }
      setShowJobForm(false);
      setEditingJob(null);
      await loadData();
    } catch (err) {
      alert("Failed to save job: " + (err.message || "Unknown error"));
    }
  };

  const handleDeleteJob = async (jobId) => {
    if (!confirm("Are you sure you want to delete this job?")) return;

    try {
      await jobs.delete(jobId);
      await loadData();
    } catch (err) {
      alert("Failed to delete job: " + (err.message || "Unknown error"));
    }
  };

  const handleFilterChange = (filterName, value) => {
    setFilters((prev) => ({
      ...prev,
      [filterName]: value,
    }));
  };

  const applyFilters = () => {
    loadData();
  };

  const clearFilters = () => {
    setFilters({
      recruiterId: "",
      statusId: "",
      positionId: "",
    });
    setTimeout(() => loadData(), 0);
  };

  const getJobTypeName = (jobTypeId) => {
    const jobType = jobTypes.find((jt) => jt.id === jobTypeId);
    return jobType ? jobType.type : "Unknown";
  };

  const getStatusName = (statusId) => {
    const status = statusTypes.find((st) => st.id === statusId);
    return status ? status.status : "Unknown";
  };

  if (loading) {
    return (
      <div className="min-h-screen flex items-center justify-center">
        <div className="animate-spin rounded-full h-32 w-32 border-b-2 border-primary-600"></div>
      </div>
    );
  }

  if (error) {
    return (
      <div className="min-h-screen flex items-center justify-center">
        <div className="bg-red-50 border border-red-200 text-red-700 px-6 py-4 rounded-lg max-w-md">
          <h3 className="font-medium mb-2">Error Loading Jobs</h3>
          <p>{error}</p>
          <button
            onClick={loadData}
            className="mt-4 bg-red-600 text-white px-4 py-2 rounded hover:bg-red-700"
          >
            Retry
          </button>
        </div>
      </div>
    );
  }

  return (
    <div className="min-h-screen bg-gray-50 py-8">
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div className="mb-8 flex justify-between items-center">
          <div>
            <h1 className="text-3xl font-bold text-gray-900">Jobs Dashboard</h1>
            <p className="mt-2 text-gray-600">
              Manage and view all job postings
            </p>
          </div>
          <button
            onClick={handleCreateJob}
            className="bg-primary-600 text-white px-6 py-3 rounded-lg hover:bg-primary-700 focus:outline-none focus:ring-2 focus:ring-primary-500 font-medium"
          >
            + Add New Job
          </button>
        </div>

        {(positionsList.length === 0 || addressesList.length === 0) && (
          <QuickSetup statusTypes={statusTypes} onRefresh={loadInitialData} />
        )}

        <div className="bg-white p-6 rounded-lg shadow mb-6">
          <h2 className="text-lg font-medium text-gray-900 mb-4">Filters</h2>
          <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-2">
                Recruiter ID
              </label>
              <input
                type="text"
                value={filters.recruiterId}
                onChange={(e) =>
                  handleFilterChange("recruiterId", e.target.value)
                }
                placeholder="Enter recruiter ID"
                className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-primary-500 focus:border-primary-500"
              />
            </div>
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-2">
                Status
              </label>
              <select
                value={filters.statusId}
                onChange={(e) => handleFilterChange("statusId", e.target.value)}
                className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-primary-500 focus:border-primary-500"
              >
                <option value="">All Statuses</option>
                {statusTypes
                  .filter((s) => s.context === "Job")
                  .map((status) => (
                    <option key={status.id} value={status.id}>
                      {status.status}
                    </option>
                  ))}
              </select>
            </div>
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-2">
                Position ID
              </label>
              <input
                type="text"
                value={filters.positionId}
                onChange={(e) =>
                  handleFilterChange("positionId", e.target.value)
                }
                placeholder="Enter position ID"
                className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-primary-500 focus:border-primary-500"
              />
            </div>
          </div>
          <div className="mt-4 flex space-x-4">
            <button
              onClick={applyFilters}
              className="bg-primary-600 text-white px-4 py-2 rounded-md hover:bg-primary-700 focus:outline-none focus:ring-2 focus:ring-primary-500"
            >
              Apply Filters
            </button>
            <button
              onClick={clearFilters}
              className="bg-gray-600 text-white px-4 py-2 rounded-md hover:bg-gray-700 focus:outline-none focus:ring-2 focus:ring-gray-500"
            >
              Clear Filters
            </button>
          </div>
        </div>

        <div className="bg-white shadow rounded-lg">
          <div className="px-6 py-4 border-b border-gray-200">
            <h2 className="text-lg font-medium text-gray-900">
              Jobs ({jobsList.length})
            </h2>
          </div>
          <div className="divide-y divide-gray-200">
            {jobsList.length === 0 ? (
              <div className="p-6 text-center text-gray-500">
                No jobs found.{" "}
                {filters.recruiterId || filters.statusId || filters.positionId
                  ? "Try adjusting your filters."
                  : "Click 'Add New Job' to create your first job posting."}
              </div>
            ) : (
              jobsList.map((job) => (
                <div key={job.id} className="p-6 hover:bg-gray-50">
                  <div className="flex items-start justify-between">
                    <div className="flex-1">
                      <h3 className="text-lg font-medium text-gray-900">
                        {job.title}{" "}
                        {job.position
                          ? `- ${job.position.title} (${job.position.level})`
                          : ""}
                      </h3>
                      <p className="mt-1 text-sm text-gray-600">
                        {job.description}
                      </p>
                      <div className="mt-3 flex flex-wrap gap-4 text-sm text-gray-500">
                        <span>
                          <strong>Type:</strong> {getJobTypeName(job.jobTypeId)}
                        </span>
                        <span>
                          <strong>Status:</strong> {getStatusName(job.statusId)}
                        </span>
                        <span>
                          <strong>Location:</strong>{" "}
                          {job.location ||
                            (job.address
                              ? `${job.address.street}, ${job.address.city?.name}, ${job.address.state?.name} ${job.address.zipCode}`
                              : "Unknown")}
                        </span>
                        {job.salaryMin && job.salaryMax && (
                          <span>
                            <strong>Salary:</strong> $
                            {job.salaryMin?.toLocaleString()} - $
                            {job.salaryMax?.toLocaleString()}
                          </span>
                        )}
                        <span>
                          <strong>Posted:</strong>{" "}
                          {new Date(job.createdAt).toLocaleDateString()}
                        </span>
                      </div>
                    </div>
                    <div className="ml-4 flex-shrink-0 flex items-center space-x-3">
                      <span
                        className={`inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium ${
                          getStatusName(job.statusId) === "Open"
                            ? "bg-green-100 text-green-800"
                            : getStatusName(job.statusId) === "Closed"
                            ? "bg-red-100 text-red-800"
                            : "bg-yellow-100 text-yellow-800"
                        }`}
                      >
                        {getStatusName(job.statusId)}
                      </span>
                      <button
                        onClick={() => handleEditJob(job)}
                        className="text-primary-600 hover:text-primary-900 text-sm font-medium"
                      >
                        Edit
                      </button>
                      <button
                        onClick={() => handleDeleteJob(job.id)}
                        className="text-red-600 hover:text-red-900 text-sm font-medium"
                      >
                        Delete
                      </button>
                    </div>
                  </div>
                </div>
              ))
            )}
          </div>
        </div>

        {showJobForm && (
          <JobForm
            job={editingJob}
            jobTypes={jobTypes}
            statusTypes={statusTypes}
            positions={positionsList}
            addresses={addressesList}
            onSave={handleSaveJob}
            onCancel={() => {
              setShowJobForm(false);
              setEditingJob(null);
            }}
          />
        )}
      </div>
    </div>
  );
};

export default JobsDashboard;
