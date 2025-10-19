import React, { useState, useEffect, useCallback } from "react";
import { useApi } from "../contexts/ApiContext";

const ApplicationsPage = () => {
  const { applications } = useApi();
  const [applicationsList, setApplicationsList] = useState([]);
  const [statusTypes, setStatusTypes] = useState([]);
  const [loading, setLoading] = useState(false);
  const [filter, setFilter] = useState({
    jobId: "",
    statusId: "",
  });

  const loadApplications = useCallback(async () => {
    setLoading(true);
    try {
      const data = await applications.getAll(filter);
      setApplicationsList(data);
    } catch (error) {
      console.error("Error loading applications:", error);
    } finally {
      setLoading(false);
    }
  }, [applications, filter]);

  const loadStatusTypes = useCallback(async () => {
    try {
      const data = await applications.getStatuses();
      setStatusTypes(data);
    } catch (error) {
      console.error("Error loading status types:", error);
    }
  }, [applications]);

  useEffect(() => {
    loadApplications();
    loadStatusTypes();
  }, [loadApplications, loadStatusTypes]);

  const updateApplicationStatus = async (id, statusId, notes) => {
    try {
      await applications.updateStatus(id, { statusId, notes });
      loadApplications();
    } catch (error) {
      console.error("Error updating application:", error);
    }
  };

  const getStatusColor = (statusName) => {
    switch (statusName?.toLowerCase()) {
      case "applied":
        return "bg-blue-100 text-blue-800";
      case "reviewing":
        return "bg-yellow-100 text-yellow-800";
      case "shortlisted":
        return "bg-green-100 text-green-800";
      case "rejected":
        return "bg-red-100 text-red-800";
      case "interviewed":
        return "bg-purple-100 text-purple-800";
      default:
        return "bg-gray-100 text-gray-800";
    }
  };

  return (
    <div className="min-h-screen bg-gray-50 p-6">
      <div className="max-w-7xl mx-auto">
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
                  value={filter.statusId}
                  onChange={(e) =>
                    setFilter({ ...filter, statusId: e.target.value })
                  }
                  className="w-full border border-gray-300 rounded-md px-3 py-2 focus:outline-none focus:ring-2 focus:ring-blue-500"
                >
                  <option value="">All Status</option>
                  {statusTypes.map((status) => (
                    <option key={status.id} value={status.id}>
                      {status.name}
                    </option>
                  ))}
                </select>
              </div>
              <div className="flex items-end">
                <button
                  onClick={loadApplications}
                  className="bg-blue-600 text-white px-4 py-2 rounded-md hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-blue-500"
                >
                  Apply Filters
                </button>
              </div>
            </div>
          </div>
        </div>

        <div className="bg-white rounded-lg shadow">
          <div className="overflow-x-auto">
            <table className="min-w-full divide-y divide-gray-200">
              <thead className="bg-gray-50">
                <tr>
                  <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                    Candidate
                  </th>
                  <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                    Job Title
                  </th>
                  <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                    Applied Date
                  </th>
                  <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                    Status
                  </th>
                  <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                    Actions
                  </th>
                </tr>
              </thead>
              <tbody className="bg-white divide-y divide-gray-200">
                {loading ? (
                  <tr>
                    <td colSpan="5" className="text-center py-8">
                      Loading...
                    </td>
                  </tr>
                ) : applicationsList.length === 0 ? (
                  <tr>
                    <td colSpan="5" className="text-center py-8 text-gray-500">
                      No applications found
                    </td>
                  </tr>
                ) : (
                  applicationsList.map((application) => (
                    <ApplicationRow
                      key={application.id}
                      application={application}
                      statusTypes={statusTypes}
                      onStatusUpdate={updateApplicationStatus}
                      getStatusColor={getStatusColor}
                    />
                  ))
                )}
              </tbody>
            </table>
          </div>
        </div>
      </div>
    </div>
  );
};

const ApplicationRow = ({
  application,
  statusTypes,
  onStatusUpdate,
  getStatusColor,
}) => {
  const [isEditing, setIsEditing] = useState(false);
  const [selectedStatus, setSelectedStatus] = useState(
    application.statusId || ""
  );
  const [notes, setNotes] = useState(application.notes || "");

  const handleSave = async () => {
    await onStatusUpdate(application.id, selectedStatus, notes);
    setIsEditing(false);
  };

  const handleCancel = () => {
    setSelectedStatus(application.statusId || "");
    setNotes(application.notes || "");
    setIsEditing(false);
  };

  const currentStatus = statusTypes.find((s) => s.id === application.statusId);

  return (
    <tr>
      <td className="px-6 py-4 whitespace-nowrap">
        <div className="flex items-center">
          <div className="flex-shrink-0 h-10 w-10">
            <div className="h-10 w-10 rounded-full bg-gray-300 flex items-center justify-center">
              <span className="text-sm font-medium text-gray-700">
                {application.candidateName?.charAt(0) || "?"}
              </span>
            </div>
          </div>
          <div className="ml-4">
            <div className="text-sm font-medium text-gray-900">
              {application.candidateName || "Unknown"}
            </div>
            <div className="text-sm text-gray-500">
              {application.candidateEmail}
            </div>
          </div>
        </div>
      </td>
      <td className="px-6 py-4 whitespace-nowrap">
        <div className="text-sm text-gray-900">{application.jobTitle}</div>
      </td>
      <td className="px-6 py-4 whitespace-nowrap">
        <div className="text-sm text-gray-900">
          {new Date(application.appliedAt).toLocaleDateString()}
        </div>
      </td>
      <td className="px-6 py-4 whitespace-nowrap">
        {isEditing ? (
          <select
            value={selectedStatus}
            onChange={(e) => setSelectedStatus(e.target.value)}
            className="border border-gray-300 rounded-md px-2 py-1 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500"
          >
            <option value="">Select Status</option>
            {statusTypes.map((status) => (
              <option key={status.id} value={status.id}>
                {status.name}
              </option>
            ))}
          </select>
        ) : (
          <span
            className={`inline-flex px-2 py-1 text-xs font-semibold rounded-full ${getStatusColor(
              currentStatus?.name
            )}`}
          >
            {currentStatus?.name || "Unknown"}
          </span>
        )}
      </td>
      <td className="px-6 py-4 whitespace-nowrap text-sm font-medium">
        {isEditing ? (
          <div className="flex space-x-2">
            <button
              onClick={handleSave}
              className="text-green-600 hover:text-green-900"
            >
              Save
            </button>
            <button
              onClick={handleCancel}
              className="text-gray-600 hover:text-gray-900"
            >
              Cancel
            </button>
          </div>
        ) : (
          <button
            onClick={() => setIsEditing(true)}
            className="text-blue-600 hover:text-blue-900"
          >
            Review
          </button>
        )}
      </td>
    </tr>
  );
};

export default ApplicationsPage;
