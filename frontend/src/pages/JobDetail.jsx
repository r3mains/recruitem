import React, { useState, useEffect, useCallback } from "react";
import { useParams, useNavigate } from "react-router-dom";
import { useAuth } from "../contexts/AuthContext";
import JobApplicationForm from "../components/JobApplicationForm";
import LoadingSpinner from "../components/LoadingSpinner";
import ErrorMessage from "../components/ErrorMessage";

const JobDetail = () => {
  const { id } = useParams();
  const { user } = useAuth();
  const navigate = useNavigate();
  const [job, setJob] = useState(null);
  const [jobTypes, setJobTypes] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [showApplicationForm, setShowApplicationForm] = useState(false);

  const loadJobDetail = useCallback(async () => {
    try {
      setLoading(true);
      setError(null);

      const [jobResponse, typesResponse] = await Promise.all([
        fetch(`http://localhost:5270/api/jobs/${id}`),
        fetch("http://localhost:5270/api/lookups/job-types"),
      ]);

      if (!jobResponse.ok) {
        throw new Error("Job not found");
      }

      const jobData = await jobResponse.json();
      const typesData = await typesResponse.json();

      setJob(jobData);
      setJobTypes(typesData);
    } catch (err) {
      setError(err.message || "Failed to load job details");
    } finally {
      setLoading(false);
    }
  }, [id]);

  useEffect(() => {
    loadJobDetail();
  }, [loadJobDetail]);

  const handleApplyClick = () => {
    if (!user) {
      navigate("/login");
      return;
    }
    setShowApplicationForm(true);
  };

  const handleApplicationSuccess = () => {
    setShowApplicationForm(false);
    alert("Application submitted successfully!");
  };

  const getJobTypeName = (jobTypeId) => {
    const jobType = jobTypes.find((jt) => jt.id === jobTypeId);
    return jobType ? jobType.type : "Unknown";
  };

  if (loading) {
    return (
      <div className="min-h-screen flex items-center justify-center">
        <LoadingSpinner size="large" text="Loading job details..." />
      </div>
    );
  }

  if (error) {
    return (
      <div className="min-h-screen flex items-center justify-center p-4">
        <ErrorMessage
          message={error}
          onRetry={loadJobDetail}
          title="Error Loading Job Details"
        />
      </div>
    );
  }

  if (!job) {
    return (
      <div className="min-h-screen flex items-center justify-center">
        <div className="text-center">
          <h2 className="text-2xl font-bold text-gray-900 mb-4">
            Job Not Found
          </h2>
          <button
            onClick={() => navigate("/jobs")}
            className="bg-primary-600 text-white px-4 py-2 rounded-md hover:bg-primary-700"
          >
            Back to Jobs
          </button>
        </div>
      </div>
    );
  }

  return (
    <div className="min-h-screen bg-gray-50 py-8">
      <div className="max-w-4xl mx-auto px-4 sm:px-6 lg:px-8">
        <div className="mb-6">
          <button
            onClick={() => navigate(-1)}
            className="flex items-center text-primary-600 hover:text-primary-700 mb-4"
          >
            <svg
              className="w-5 h-5 mr-2"
              fill="none"
              stroke="currentColor"
              viewBox="0 0 24 24"
            >
              <path
                strokeLinecap="round"
                strokeLinejoin="round"
                strokeWidth="2"
                d="M15 19l-7-7 7-7"
              />
            </svg>
            Back to Jobs
          </button>
        </div>

        <div className="bg-white shadow rounded-lg overflow-hidden">
          <div className="p-6">
            <div className="flex items-start justify-between mb-6">
              <div className="flex-1">
                <h1 className="text-3xl font-bold text-gray-900 mb-2">
                  {job.title}
                </h1>
                <div className="flex flex-wrap gap-4 text-sm text-gray-600 mb-4">
                  <span className="flex items-center">
                    <svg
                      className="w-4 h-4 mr-1"
                      fill="none"
                      stroke="currentColor"
                      viewBox="0 0 24 24"
                    >
                      <path
                        strokeLinecap="round"
                        strokeLinejoin="round"
                        strokeWidth="2"
                        d="M17.657 16.657L13.414 20.9a1.998 1.998 0 01-2.827 0l-4.244-4.243a8 8 0 1111.314 0z"
                      />
                      <path
                        strokeLinecap="round"
                        strokeLinejoin="round"
                        strokeWidth="2"
                        d="M15 11a3 3 0 11-6 0 3 3 0 016 0z"
                      />
                    </svg>
                    {job.location || "Location not specified"}
                  </span>
                  <span className="flex items-center">
                    <svg
                      className="w-4 h-4 mr-1"
                      fill="none"
                      stroke="currentColor"
                      viewBox="0 0 24 24"
                    >
                      <path
                        strokeLinecap="round"
                        strokeLinejoin="round"
                        strokeWidth="2"
                        d="M21 13.255A23.931 23.931 0 0112 15c-3.183 0-6.22-.62-9-1.745M16 6V4a2 2 0 00-2-2h-4a2 2 0 00-2 2v2m8 6V8a2 2 0 00-2-2H8a2 2 0 00-2 2v8a2 2 0 002 2h8a2 2 0 002-2z"
                      />
                    </svg>
                    {getJobTypeName(job.jobTypeId)}
                  </span>
                  <span className="flex items-center">
                    <svg
                      className="w-4 h-4 mr-1"
                      fill="none"
                      stroke="currentColor"
                      viewBox="0 0 24 24"
                    >
                      <path
                        strokeLinecap="round"
                        strokeLinejoin="round"
                        strokeWidth="2"
                        d="M8 7V3a4 4 0 118 0v4M8 7h8M8 7L6 9m10-2l2 2m-2-2v10a2 2 0 01-2 2H8a2 2 0 01-2-2V9"
                      />
                    </svg>
                    Posted {new Date(job.createdAt).toLocaleDateString()}
                  </span>
                </div>
                {job.salaryMin && job.salaryMax && (
                  <div className="text-lg font-semibold text-green-600 mb-4">
                    ${job.salaryMin?.toLocaleString()} - $
                    {job.salaryMax?.toLocaleString()}
                  </div>
                )}
              </div>
              <span className="inline-flex items-center px-3 py-1 rounded-full text-sm font-medium bg-green-100 text-green-800">
                Open
              </span>
            </div>

            <div className="prose max-w-none mb-8">
              <h2 className="text-xl font-semibold text-gray-900 mb-4">
                Job Description
              </h2>
              <div className="text-gray-700 whitespace-pre-wrap">
                {job.description}
              </div>
            </div>

            {job.position && (
              <div className="mb-8">
                <h2 className="text-xl font-semibold text-gray-900 mb-4">
                  Position Details
                </h2>
                <div className="bg-gray-50 rounded-lg p-4">
                  <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                    <div>
                      <span className="text-sm font-medium text-gray-500">
                        Position Title
                      </span>
                      <div className="text-gray-900">{job.position.title}</div>
                    </div>
                    <div>
                      <span className="text-sm font-medium text-gray-500">
                        Level
                      </span>
                      <div className="text-gray-900">{job.position.level}</div>
                    </div>
                    {job.position.numberOfInterviews && (
                      <div>
                        <span className="text-sm font-medium text-gray-500">
                          Interview Rounds
                        </span>
                        <div className="text-gray-900">
                          {job.position.numberOfInterviews}
                        </div>
                      </div>
                    )}
                  </div>
                </div>
              </div>
            )}

            <div className="border-t border-gray-200 pt-6">
              <div className="flex items-center justify-between">
                <div className="text-sm text-gray-500">
                  Interested in this position?
                </div>
                <button
                  onClick={handleApplyClick}
                  className="bg-green-600 text-white px-6 py-3 rounded-md hover:bg-green-700 focus:outline-none focus:ring-2 focus:ring-green-500 font-medium"
                >
                  {user ? "Apply for This Job" : "Login to Apply"}
                </button>
              </div>
            </div>
          </div>
        </div>

        {showApplicationForm && (
          <JobApplicationForm
            job={job}
            onClose={() => setShowApplicationForm(false)}
            onSuccess={handleApplicationSuccess}
          />
        )}
      </div>
    </div>
  );
};

export default JobDetail;
