import React, { useState, useEffect } from "react";
import { useApi } from "../contexts/ApiContext";
import { useAuth } from "../contexts/AuthContext";

const PositionsPage = () => {
  const [positions, setPositions] = useState([]);
  const [statusTypes, setStatusTypes] = useState([]);
  const [employees, setEmployees] = useState([]);
  const [loading, setLoading] = useState(true);
  const [showCreateForm, setShowCreateForm] = useState(false);
  const [statusFilter, setStatusFilter] = useState("");
  const [editingPosition, setEditingPosition] = useState(null);
  const api = useApi();
  const { hasRole } = useAuth();

  const [formData, setFormData] = useState({
    title: "",
    statusId: "",
    closedReason: "",
    numberOfInterviews: 1,
    reviewerId: "",
  });

  useEffect(() => {
    loadData();
  }, [statusFilter]);

  const loadData = async () => {
    try {
      setLoading(true);
      const [positionsData, statusData, employeesData] = await Promise.all([
        api.positions.getAll(statusFilter),
        api.lookups.getStatusTypes(),
        api.employees ? api.employees.getAll() : [],
      ]);

      setPositions(positionsData);
      setStatusTypes(statusData);
      setEmployees(employeesData || []);
    } catch (error) {
      console.error("Error loading data:", error);
    } finally {
      setLoading(false);
    }
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    try {
      if (editingPosition) {
        await api.positions.update(editingPosition.id, formData);
      } else {
        await api.positions.create(formData);
      }

      setShowCreateForm(false);
      setEditingPosition(null);
      resetForm();
      loadData();
    } catch (error) {
      console.error("Error saving position:", error);
    }
  };

  const handleStatusUpdate = async (
    positionId,
    statusId,
    closedReason = ""
  ) => {
    try {
      await api.positions.updateStatus(positionId, { statusId, closedReason });
      loadData();
    } catch (error) {
      console.error("Error updating status:", error);
    }
  };

  const resetForm = () => {
    setFormData({
      title: "",
      statusId: "",
      closedReason: "",
      numberOfInterviews: 1,
      reviewerId: "",
    });
  };

  const startEdit = (position) => {
    setEditingPosition(position);
    setFormData({
      title: position.title,
      statusId: position.statusId,
      closedReason: position.closedReason || "",
      numberOfInterviews: position.numberOfInterviews,
      reviewerId: position.reviewerId || "",
    });
    setShowCreateForm(true);
  };

  const getStatusBadgeColor = (statusName) => {
    switch (statusName?.toLowerCase()) {
      case "open":
        return "bg-green-100 text-green-800";
      case "hold":
        return "bg-yellow-100 text-yellow-800";
      case "closed":
        return "bg-red-100 text-red-800";
      default:
        return "bg-gray-100 text-gray-800";
    }
  };

  if (loading) {
    return <div className="p-6">Loading positions...</div>;
  }

  return (
    <div className="p-6">
      <div className="mb-6">
        <div className="flex justify-between items-center mb-4">
          <h1 className="text-2xl font-bold">Positions Management</h1>
          {hasRole(["Recruiter", "HR"]) && (
            <button
              onClick={() => setShowCreateForm(true)}
              className="bg-blue-600 text-white px-4 py-2 rounded hover:bg-blue-700"
            >
              Create Position
            </button>
          )}
        </div>

        <div className="flex gap-4 mb-4">
          <select
            value={statusFilter}
            onChange={(e) => setStatusFilter(e.target.value)}
            className="border rounded px-3 py-2"
          >
            <option value="">All Status</option>
            {statusTypes.map((status) => (
              <option key={status.id} value={status.id}>
                {status.name}
              </option>
            ))}
          </select>
        </div>
      </div>

      {showCreateForm && (
        <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
          <div className="bg-white p-6 rounded-lg w-96">
            <h2 className="text-xl font-bold mb-4">
              {editingPosition ? "Edit Position" : "Create Position"}
            </h2>
            <form onSubmit={handleSubmit}>
              <div className="mb-4">
                <label className="block text-sm font-medium mb-2">Title</label>
                <input
                  type="text"
                  value={formData.title}
                  onChange={(e) =>
                    setFormData({ ...formData, title: e.target.value })
                  }
                  className="w-full border rounded px-3 py-2"
                  required
                />
              </div>

              <div className="mb-4">
                <label className="block text-sm font-medium mb-2">Status</label>
                <select
                  value={formData.statusId}
                  onChange={(e) =>
                    setFormData({ ...formData, statusId: e.target.value })
                  }
                  className="w-full border rounded px-3 py-2"
                  required
                >
                  <option value="">Select Status</option>
                  {statusTypes.map((status) => (
                    <option key={status.id} value={status.id}>
                      {status.name}
                    </option>
                  ))}
                </select>
              </div>

              <div className="mb-4">
                <label className="block text-sm font-medium mb-2">
                  Number of Interviews
                </label>
                <input
                  type="number"
                  min="1"
                  max="10"
                  value={formData.numberOfInterviews}
                  onChange={(e) =>
                    setFormData({
                      ...formData,
                      numberOfInterviews: parseInt(e.target.value),
                    })
                  }
                  className="w-full border rounded px-3 py-2"
                />
              </div>

              <div className="mb-4">
                <label className="block text-sm font-medium mb-2">
                  Reviewer
                </label>
                <select
                  value={formData.reviewerId}
                  onChange={(e) =>
                    setFormData({ ...formData, reviewerId: e.target.value })
                  }
                  className="w-full border rounded px-3 py-2"
                >
                  <option value="">Select Reviewer</option>
                  {employees.map((emp) => (
                    <option key={emp.id} value={emp.id}>
                      {emp.firstName} {emp.lastName}
                    </option>
                  ))}
                </select>
              </div>

              <div className="mb-4">
                <label className="block text-sm font-medium mb-2">
                  Closed Reason (if applicable)
                </label>
                <textarea
                  value={formData.closedReason}
                  onChange={(e) =>
                    setFormData({ ...formData, closedReason: e.target.value })
                  }
                  className="w-full border rounded px-3 py-2"
                  rows="3"
                />
              </div>

              <div className="flex justify-end gap-2">
                <button
                  type="button"
                  onClick={() => {
                    setShowCreateForm(false);
                    setEditingPosition(null);
                    resetForm();
                  }}
                  className="bg-gray-500 text-white px-4 py-2 rounded hover:bg-gray-600"
                >
                  Cancel
                </button>
                <button
                  type="submit"
                  className="bg-blue-600 text-white px-4 py-2 rounded hover:bg-blue-700"
                >
                  {editingPosition ? "Update" : "Create"}
                </button>
              </div>
            </form>
          </div>
        </div>
      )}

      <div className="grid gap-4">
        {positions.map((position) => (
          <div
            key={position.id}
            className="bg-white p-4 rounded-lg shadow border"
          >
            <div className="flex justify-between items-start mb-2">
              <div>
                <h3 className="text-lg font-semibold">{position.title}</h3>
                <span
                  className={`inline-block px-2 py-1 text-xs rounded-full ${getStatusBadgeColor(
                    position.statusName
                  )}`}
                >
                  {position.statusName}
                </span>
              </div>
              {hasRole(["Recruiter", "HR"]) && (
                <div className="flex gap-2">
                  <button
                    onClick={() => startEdit(position)}
                    className="text-blue-600 hover:text-blue-800 text-sm"
                  >
                    Edit
                  </button>
                  <select
                    onChange={(e) => {
                      const newStatusId = e.target.value;
                      if (newStatusId) {
                        handleStatusUpdate(position.id, newStatusId);
                      }
                    }}
                    className="text-sm border rounded px-2 py-1"
                    defaultValue=""
                  >
                    <option value="">Change Status</option>
                    {statusTypes.map((status) => (
                      <option key={status.id} value={status.id}>
                        {status.name}
                      </option>
                    ))}
                  </select>
                </div>
              )}
            </div>

            <div className="text-sm text-gray-600">
              <p>Interviews: {position.numberOfInterviews}</p>
              {position.reviewerName && (
                <p>Reviewer: {position.reviewerName}</p>
              )}
              {position.closedReason && <p>Reason: {position.closedReason}</p>}
            </div>
          </div>
        ))}
      </div>
    </div>
  );
};

export default PositionsPage;
