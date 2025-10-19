import React, { useState, useEffect } from "react";
import { useApi } from "../contexts/ApiContext";

const JobApplications = () => {
  const { applications: jobApplications, lookups } = useApi();
  const [applications, setApplications] = useState([]);
  const [statusTypes, setStatusTypes] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [filters, setFilters] = useState({
    statusId: "",
  });

  useEffect(() => {
    loadData();
  }, [jobApplications, lookups]); // eslint-disable-line react-hooks/exhaustive-deps

  const loadData = async () => {
    try {
      setLoading(true);
      setError(null);

      const [applicationsData, statusTypesData] = await Promise.all([
        jobApplications.getMyApplications(),
        lookups.getStatusTypes(),
      ]);

      setApplications(applicationsData);
      setStatusTypes(
        statusTypesData.filter((st) => st.context === "application")
      );
    } catch (err) {
      console.error("Error loading data:", err);
      setError("Failed to load data");
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

  const applyFilters = () => {
    loadData();
  };

  const updateApplicationStatus = async (applicationId, statusId) => {
    try {
      await jobApplications.update(applicationId, { statusId });
      loadData();
    } catch (err) {
      console.error("Error updating application status:", err);
      setError("Failed to update application status");
    }
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
          <h3 className="font-medium mb-2">Error Loading Applications</h3>
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
        <div className="mb-8">
          <h1 className="text-3xl font-bold text-gray-900">Job Applications</h1>
          <p className="mt-2 text-gray-600">Manage job applications</p>
        </div>

        <div className="bg-white p-6 rounded-lg shadow mb-6">
          <h2 className="text-lg font-medium text-gray-900 mb-4">
            Filter Applications
          </h2>
          <div className="flex items-end space-x-4">
            <div className="flex-1">
              <label className="block text-sm font-medium text-gray-700 mb-2">
                Status
              </label>
              <select
                value={filters.statusId}
                onChange={(e) => handleFilterChange("statusId", e.target.value)}
                className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-primary-500 focus:border-primary-500"
              >
                <option value="">All Statuses</option>
                {statusTypes.map((status) => (
                  <option key={status.id} value={status.id}>
                    {status.status}
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
          </div>
        </div>

        <div className="bg-white shadow rounded-lg">
          <div className="px-6 py-4 border-b border-gray-200">
            <h2 className="text-lg font-medium text-gray-900">
              Applications ({applications.length})
            </h2>
          </div>
          <div className="divide-y divide-gray-200">
            {applications.length === 0 ? (
              <div className="p-6 text-center text-gray-500">
                No applications found.
              </div>
            ) : (
              applications.map((application) => (
                <div key={application.id} className="p-6 hover:bg-gray-50">
                  <div className="flex items-start justify-between">
                    <div className="flex-1">
                      <h3 className="text-lg font-medium text-gray-900">
                        {application.jobTitle}
                      </h3>
                      <p className="mt-1 text-sm text-gray-600">
                        Candidate: {application.candidateName} (
                        {application.candidateEmail})
                      </p>
                      {application.coverLetter && (
                        <p className="mt-2 text-sm text-gray-600">
                          <strong>Cover Letter:</strong>{" "}
                          {application.coverLetter}
                        </p>
                      )}
                      {application.notes && (
                        <p className="mt-2 text-sm text-gray-600">
                          <strong>Notes:</strong> {application.notes}
                        </p>
                      )}
                      <div className="mt-3 flex flex-wrap gap-4 text-sm text-gray-500">
                        <span>
                          <strong>Applied:</strong>{" "}
                          {new Date(application.appliedAt).toLocaleDateString()}
                        </span>
                        {application.reviewedAt && (
                          <span>
                            <strong>Reviewed:</strong>{" "}
                            {new Date(
                              application.reviewedAt
                            ).toLocaleDateString()}
                          </span>
                        )}
                      </div>
                    </div>
                    <div className="ml-4 flex flex-col space-y-2">
                      <span className="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium bg-blue-100 text-blue-800">
                        {getStatusName(application.statusId)}
                      </span>
                      <select
                        value={application.statusId}
                        onChange={(e) =>
                          updateApplicationStatus(
                            application.id,
                            e.target.value
                          )
                        }
                        className="text-xs px-2 py-1 border border-gray-300 rounded focus:outline-none focus:ring-1 focus:ring-primary-500"
                      >
                        {statusTypes.map((status) => (
                          <option key={status.id} value={status.id}>
                            {status.status}
                          </option>
                        ))}
                      </select>
                    </div>
                  </div>
                </div>
              ))
            )}
          </div>
        </div>
      </div>
    </div>
  );
};

export default JobApplications;
