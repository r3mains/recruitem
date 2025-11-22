import React, { useState, useEffect } from "react";
import { useNavigate } from "react-router-dom";
import { useAuth } from "../contexts/AuthContext";
import { useApi } from "../contexts/ApiContext";
import JobApplicationForm from "../components/JobApplicationForm";
import LoadingSpinner from "../components/LoadingSpinner";
import ErrorMessage from "../components/ErrorMessage";

const PublicJobListings = () => {
  const { user } = useAuth();
  const api = useApi();
  const [jobs, setJobs] = useState([]);
  const [jobTypes, setJobTypes] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [filters, setFilters] = useState({
    jobTypeId: "",
  });
  const [showApplicationForm, setShowApplicationForm] = useState(false);
  const [selectedJob, setSelectedJob] = useState(null);
  const navigate = useNavigate();

  useEffect(() => {
    loadData();
  }, []);

  const loadData = async () => {
    try {
      setLoading(true);
      setError(null);

      const response = await fetch("http://localhost:5270/api/jobs/public");
      const jobsData = await response.json();

      const typesResponse = await fetch(
        "http://localhost:5270/api/lookups/job-types"
      );
      const typesData = await typesResponse.json();

      setJobs(jobsData);
      setJobTypes(typesData);
    } catch (err) {
      setError(err.message || "Failed to load data");
    } finally {
      setLoading(false);
    }
  };

  const handleFilterChange = (filterName, value) => {
    setFilters((prev) => ({
      ...prev,
      [filterName]: value,
    }));
  };

  const applyFilters = async () => {
    try {
      setLoading(true);
      const params = new URLSearchParams();
      if (filters.jobTypeId) params.append("jobTypeId", filters.jobTypeId);

      const response = await fetch(
        `http://localhost:5270/api/jobs/public?${params.toString()}`
      );
      const data = await response.json();
      setJobs(data);
    } catch (err) {
      setError(err.message || "Failed to filter jobs");
    } finally {
      setLoading(false);
    }
  };

  const getJobTypeName = (jobTypeId) => {
    const jobType = jobTypes.find((jt) => jt.id === jobTypeId);
    return jobType ? jobType.type : "Unknown";
  };

  const handleApplyClick = (job) => {
    if (!user) {
      navigate("/login");
      return;
    }
    setSelectedJob(job);
    setShowApplicationForm(true);
  };

  const handleApplicationSuccess = () => {
    setShowApplicationForm(false);
    setSelectedJob(null);
    alert("Application submitted successfully!");
  };

  if (loading) {
    return (
      <div className="min-h-screen flex items-center justify-center">
        <LoadingSpinner size="large" text="Loading jobs..." />
      </div>
    );
  }

  if (error) {
    return (
      <div className="min-h-screen flex items-center justify-center p-4">
        <ErrorMessage
          message={error}
          onRetry={loadData}
          title="Error Loading Jobs"
        />
      </div>
    );
  }

  return (
    <div className="min-h-screen bg-gray-50 py-8">
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div className="mb-8">
          <h1 className="text-3xl font-bold text-gray-900">Available Jobs</h1>
          <p className="mt-2 text-gray-600">Find your dream job</p>
        </div>

        <div className="bg-white p-6 rounded-lg shadow mb-6">
          <h2 className="text-lg font-medium text-gray-900 mb-4">
            Filter Jobs
          </h2>
          <div className="flex items-end space-x-4">
            <div className="flex-1">
              <label className="block text-sm font-medium text-gray-700 mb-2">
                Job Type
              </label>
              <select
                value={filters.jobTypeId}
                onChange={(e) =>
                  handleFilterChange("jobTypeId", e.target.value)
                }
                className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-primary-500 focus:border-primary-500"
              >
                <option value="">All Types</option>
                {jobTypes.map((type) => (
                  <option key={type.id} value={type.id}>
                    {type.type}
                  </option>
                ))}
              </select>
            </div>
            <button
              onClick={applyFilters}
              className="bg-primary-600 text-white px-6 py-2 rounded-md hover:bg-primary-700 focus:outline-none focus:ring-2 focus:ring-primary-500"
            >
              Filter
            </button>
            <button
              onClick={() => navigate("/login")}
              className="bg-green-600 text-white px-6 py-2 rounded-md hover:bg-green-700 focus:outline-none focus:ring-2 focus:ring-green-500"
            >
              {user ? "Logged in" : "Login to Apply"}
            </button>
          </div>
        </div>

        <div className="grid gap-6">
          {jobs.length === 0 ? (
            <div className="bg-white p-12 rounded-lg shadow text-center">
              <div className="max-w-md mx-auto">
                <div className="bg-gray-100 rounded-full w-20 h-20 flex items-center justify-center mx-auto mb-6">
                  <svg
                    className="w-10 h-10 text-gray-400"
                    fill="none"
                    stroke="currentColor"
                    viewBox="0 0 24 24"
                  >
                    <path
                      strokeLinecap="round"
                      strokeLinejoin="round"
                      strokeWidth={2}
                      d="M21 13.255A23.931 23.931 0 0112 15c-3.183 0-6.22-.62-9-1.745M16 6V4a2 2 0 00-2-2h-4a2 2 0 00-2-2v2m8 0V6a2 2 0 012 2v6.5"
                    />
                  </svg>
                </div>
                <h3 className="text-xl font-semibold text-gray-900 mb-2">
                  No Jobs Available
                </h3>
                <p className="text-gray-600 mb-6">
                  There are currently no job openings available. Please check
                  back later or create an account to be notified when new
                  positions are posted.
                </p>
                <div className="flex flex-col sm:flex-row gap-3 justify-center">
                  <button
                    onClick={loadData}
                    className="bg-blue-600 text-white px-6 py-2 rounded-md hover:bg-blue-700 transition-colors"
                  >
                    Refresh
                  </button>
                  {!user && (
                    <button
                      onClick={() => navigate("/register")}
                      className="bg-gray-100 text-gray-700 px-6 py-2 rounded-md hover:bg-gray-200 transition-colors"
                    >
                      Create Account
                    </button>
                  )}
                </div>
              </div>
            </div>
          ) : (
            jobs.map((job) => (
              <div
                key={job.id}
                className="bg-white p-6 rounded-lg shadow hover:shadow-md transition-shadow"
              >
                <div className="flex items-start justify-between">
                  <div className="flex-1">
                    <h3 className="text-xl font-semibold text-gray-900 mb-2">
                      {job.title}
                    </h3>
                    <p className="text-gray-600 mb-4">{job.description}</p>
                    <div className="flex flex-wrap gap-4 text-sm text-gray-500">
                      <span>
                        <strong>Type:</strong> {getJobTypeName(job.jobTypeId)}
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
                  <div className="ml-4">
                    <span className="inline-flex items-center px-3 py-1 rounded-full text-sm font-medium bg-green-100 text-green-800">
                      Open
                    </span>
                  </div>
                </div>
                <div className="mt-4 pt-4 border-t border-gray-200">
                  <button
                    onClick={() => navigate(`/job/${job.id}`)}
                    className="bg-primary-600 text-white px-4 py-2 rounded-md hover:bg-primary-700 focus:outline-none focus:ring-2 focus:ring-primary-500 mr-3"
                  >
                    View Details
                  </button>
                  <button
                    onClick={() => handleApplyClick(job)}
                    className="bg-green-600 text-white px-4 py-2 rounded-md hover:bg-green-700 focus:outline-none focus:ring-2 focus:ring-green-500"
                  >
                    {user ? "Apply Now" : "Login to Apply"}
                  </button>
                </div>
              </div>
            ))
          )}
        </div>

        {showApplicationForm && selectedJob && (
          <JobApplicationForm
            job={selectedJob}
            onClose={() => {
              setShowApplicationForm(false);
              setSelectedJob(null);
            }}
            onSuccess={handleApplicationSuccess}
          />
        )}
      </div>
    </div>
  );
};

export default PublicJobListings;
