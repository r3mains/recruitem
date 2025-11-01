import React, { useState, useEffect } from "react";
import { useApi } from "../contexts/ApiContext";

const ScreeningPage = () => {
  const { lookups, screening } = useApi();
  const [applications, setApplications] = useState([]);
  const [statusTypes, setStatusTypes] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [filters, setFilters] = useState({
    jobId: "",
    statusId: "",
  });
  const [selectedApplications, setSelectedApplications] = useState([]);
  const [bulkAction, setBulkAction] = useState("");

  useEffect(() => {
    loadData();
  }, []);

  const loadData = async () => {
    try {
      setLoading(true);
      setError(null);

      const [applicationsData, statusTypesData] = await Promise.all([
        screening.getApplicationsForScreening(),
        lookups.getStatusTypes(),
      ]);

      setApplications(applicationsData);
      setStatusTypes(
        statusTypesData.filter((st) => st.context === "application")
      );
    } catch (err) {
      setError(err.message || "Failed to load data");
    } finally {
      setLoading(false);
    }
  };

  const handleFilterChange = (field, value) => {
    setFilters({ ...filters, [field]: value });
  };

  const applyFilters = async () => {
    try {
      setLoading(true);
      const data = await screening.getApplicationsForScreening(filters);
      setApplications(data);
    } catch (err) {
      setError(err.message || "Failed to filter applications");
    } finally {
      setLoading(false);
    }
  };

  const handleScreenApplication = async (applicationId, action) => {
    try {
      const statusId = statusTypes.find((st) => st.status === action)?.id;
      if (!statusId) return;

      await screening.screenApplication(applicationId, {
        statusId,
        notes: `Application ${action.toLowerCase()}`,
      });

      await loadData();
    } catch (err) {
      setError(err.message || "Failed to screen application");
    }
  };

  const handleBulkAction = async () => {
    if (!bulkAction || selectedApplications.length === 0) return;

    try {
      const statusId = statusTypes.find((st) => st.status === bulkAction)?.id;
      if (!statusId) return;

      await screening.bulkScreenApplications({
        applicationIds: selectedApplications,
        statusId,
        notes: `Bulk ${bulkAction.toLowerCase()}`,
      });

      await loadData();
      setSelectedApplications([]);
      setBulkAction("");
    } catch (err) {
      setError(err.message || "Failed to perform bulk action");
    }
  };

  const handleCalculateScore = async (applicationId) => {
    try {
      const score = await screening.calculateScore(applicationId);
      alert(`Calculated Score: ${score.toFixed(2)}`);
    } catch (err) {
      setError(err.message || "Failed to calculate score");
    }
  };

  const handleSelectApplication = (applicationId) => {
    setSelectedApplications((prev) =>
      prev.includes(applicationId)
        ? prev.filter((id) => id !== applicationId)
        : [...prev, applicationId]
    );
  };

  const handleSelectAll = () => {
    if (selectedApplications.length === applications.length) {
      setSelectedApplications([]);
    } else {
      setSelectedApplications(applications.map((app) => app.id));
    }
  };

  if (loading) {
    return (
      <div className="min-h-screen flex items-center justify-center">
        <div className="text-lg">Loading applications...</div>
      </div>
    );
  }

  if (error) {
    return (
      <div className="min-h-screen flex items-center justify-center">
        <div className="text-red-600">{error}</div>
      </div>
    );
  }

  return (
    <div className="min-h-screen bg-gray-50 py-8">
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div className="mb-6">
          <h1 className="text-3xl font-bold text-gray-900">
            Application Screening
          </h1>
          <p className="text-gray-600 mt-2">
            Review and manage job applications
          </p>
        </div>

        <div className="bg-white rounded-lg shadow mb-6">
          <div className="p-6 border-b border-gray-200">
            <h2 className="text-lg font-semibold text-gray-900 mb-4">
              Filters
            </h2>
            <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
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
                  {statusTypes.map((status) => (
                    <option key={status.id} value={status.id}>
                      {status.status}
                    </option>
                  ))}
                </select>
              </div>
              <div className="flex items-end">
                <button
                  onClick={applyFilters}
                  className="bg-blue-600 text-white px-6 py-2 rounded-md hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-blue-500"
                >
                  Apply Filters
                </button>
              </div>
            </div>
          </div>
        </div>

        <div className="bg-white rounded-lg shadow">
          <div className="p-6 border-b border-gray-200">
            <div className="flex justify-between items-center">
              <h2 className="text-lg font-semibold text-gray-900">
                Applications ({applications.length})
              </h2>
              {selectedApplications.length > 0 && (
                <div className="flex items-center space-x-4">
                  <select
                    value={bulkAction}
                    onChange={(e) => setBulkAction(e.target.value)}
                    className="px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-primary-500 focus:border-primary-500"
                  >
                    <option value="">Select Action</option>
                    <option value="Accepted">Accept</option>
                    <option value="Rejected">Reject</option>
                    <option value="Shortlisted">Shortlist</option>
                  </select>
                  <button
                    onClick={handleBulkAction}
                    disabled={!bulkAction}
                    className="bg-green-600 text-white px-4 py-2 rounded-md hover:bg-green-700 disabled:bg-gray-400 focus:outline-none focus:ring-2 focus:ring-green-500"
                  >
                    Apply to Selected ({selectedApplications.length})
                  </button>
                </div>
              )}
            </div>
          </div>

          <div className="overflow-x-auto">
            <table className="min-w-full divide-y divide-gray-200">
              <thead className="bg-gray-50">
                <tr>
                  <th className="px-6 py-3 text-left">
                    <input
                      type="checkbox"
                      checked={
                        selectedApplications.length === applications.length &&
                        applications.length > 0
                      }
                      onChange={handleSelectAll}
                      className="rounded border-gray-300 text-blue-600 focus:ring-blue-500"
                    />
                  </th>
                  <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                    Candidate
                  </th>
                  <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                    Job
                  </th>
                  <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                    Status
                  </th>
                  <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                    Applied Date
                  </th>
                  <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                    Score
                  </th>
                  <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                    Actions
                  </th>
                </tr>
              </thead>
              <tbody className="bg-white divide-y divide-gray-200">
                {applications.map((application) => (
                  <tr
                    key={application.id}
                    className={
                      selectedApplications.includes(application.id)
                        ? "bg-blue-50"
                        : "hover:bg-gray-50"
                    }
                  >
                    <td className="px-6 py-4">
                      <input
                        type="checkbox"
                        checked={selectedApplications.includes(application.id)}
                        onChange={() => handleSelectApplication(application.id)}
                        className="rounded border-gray-300 text-blue-600 focus:ring-blue-500"
                      />
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap">
                      <div>
                        <div className="text-sm font-medium text-gray-900">
                          {application.candidateName}
                        </div>
                        <div className="text-sm text-gray-500">
                          {application.candidateEmail}
                        </div>
                      </div>
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                      {application.jobTitle}
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap">
                      <span
                        className={`inline-flex px-2 py-1 text-xs font-semibold rounded-full ${
                          application.statusName === "Applied"
                            ? "bg-yellow-100 text-yellow-800"
                            : application.statusName === "Shortlisted"
                            ? "bg-green-100 text-green-800"
                            : application.statusName === "Rejected"
                            ? "bg-red-100 text-red-800"
                            : "bg-gray-100 text-gray-800"
                        }`}
                      >
                        {application.statusName}
                      </span>
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                      {new Date(application.appliedAt).toLocaleDateString()}
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                      {application.score ? (
                        <span className="font-medium">
                          {application.score.toFixed(1)}
                        </span>
                      ) : (
                        <button
                          onClick={() => handleCalculateScore(application.id)}
                          className="text-blue-600 hover:text-blue-800 text-sm"
                        >
                          Calculate
                        </button>
                      )}
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap text-sm font-medium space-x-2">
                      <button
                        onClick={() =>
                          handleScreenApplication(application.id, "Accepted")
                        }
                        className="text-green-600 hover:text-green-900"
                      >
                        Accept
                      </button>
                      <button
                        onClick={() =>
                          handleScreenApplication(application.id, "Rejected")
                        }
                        className="text-red-600 hover:text-red-900"
                      >
                        Reject
                      </button>
                      <button
                        onClick={() =>
                          handleScreenApplication(application.id, "Shortlisted")
                        }
                        className="text-blue-600 hover:text-blue-900"
                      >
                        Shortlist
                      </button>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>

          {applications.length === 0 && (
            <div className="p-6 text-center text-gray-500">
              No applications found for screening.
            </div>
          )}
        </div>
      </div>
    </div>
  );
};

export default ScreeningPage;
