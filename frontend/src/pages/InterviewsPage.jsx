import React, { useState, useEffect } from "react";
import { useApi } from "../contexts/ApiContext";

const InterviewsPage = () => {
  const { interviews } = useApi();
  const [interviewsList, setInterviewsList] = useState([]);
  const [interviewTypes, setInterviewTypes] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [showCreateForm, setShowCreateForm] = useState(false);

  const [formData, setFormData] = useState({
    jobApplicationId: "",
    interviewTypeId: "",
    interviewerIds: [],
  });

  const loadInterviews = async () => {
    try {
      setLoading(true);
      setError(null);

      const [interviewsData, typesData] = await Promise.all([
        interviews.getAll(),
        interviews.getInterviewTypes(),
      ]);

      setInterviewsList(interviewsData);
      setInterviewTypes(typesData);
    } catch (err) {
      setError(err.message || "Failed to load interviews");
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    loadInterviews();
  }, []);

  const handleCreateInterview = async (e) => {
    e.preventDefault();

    if (!formData.jobApplicationId || !formData.interviewTypeId) {
      alert("Please fill in all required fields");
      return;
    }

    try {
      await interviews.create(formData);
      setShowCreateForm(false);
      setFormData({
        jobApplicationId: "",
        interviewTypeId: "",
        interviewerIds: [],
      });
      loadInterviews();
    } catch (err) {
      alert("Failed to create interview: " + err.message);
    }
  };

  const handleDeleteInterview = async (id) => {
    if (!window.confirm("Are you sure you want to delete this interview?")) {
      return;
    }

    try {
      await interviews.delete(id);
      loadInterviews();
    } catch (err) {
      alert("Failed to delete interview: " + err.message);
    }
  };

  const handleConductInterview = async (id) => {
    try {
      await interviews.conduct(id);
      loadInterviews();
    } catch (err) {
      alert("Failed to update interview status: " + err.message);
    }
  };

  if (loading) {
    return (
      <div className="flex items-center justify-center min-h-screen">
        <div className="text-xl">Loading interviews...</div>
      </div>
    );
  }

  if (error) {
    return (
      <div className="flex items-center justify-center min-h-screen">
        <div className="text-xl text-red-600">Error: {error}</div>
      </div>
    );
  }

  return (
    <div className="container mx-auto px-4 py-8">
      <div className="flex justify-between items-center mb-6">
        <h1 className="text-3xl font-bold text-gray-800">
          Interview Management
        </h1>
        <button
          onClick={() => setShowCreateForm(true)}
          className="bg-blue-600 text-white px-4 py-2 rounded-lg hover:bg-blue-700 transition-colors"
        >
          Schedule Interview
        </button>
      </div>

      {showCreateForm && (
        <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
          <div className="bg-white rounded-lg p-6 w-full max-w-md">
            <h2 className="text-xl font-bold mb-4">Schedule New Interview</h2>
            <form onSubmit={handleCreateInterview}>
              <div className="mb-4">
                <label className="block text-sm font-medium text-gray-700 mb-2">
                  Job Application ID *
                </label>
                <input
                  type="text"
                  required
                  value={formData.jobApplicationId}
                  onChange={(e) =>
                    setFormData({
                      ...formData,
                      jobApplicationId: e.target.value,
                    })
                  }
                  className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                  placeholder="Enter job application ID"
                />
              </div>

              <div className="mb-4">
                <label className="block text-sm font-medium text-gray-700 mb-2">
                  Interview Type *
                </label>
                <select
                  required
                  value={formData.interviewTypeId}
                  onChange={(e) =>
                    setFormData({
                      ...formData,
                      interviewTypeId: e.target.value,
                    })
                  }
                  className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                >
                  <option value="">Select Interview Type</option>
                  {interviewTypes.map((type) => (
                    <option key={type.id} value={type.id}>
                      {type.type}
                    </option>
                  ))}
                </select>
              </div>

              <div className="flex justify-end space-x-3">
                <button
                  type="button"
                  onClick={() => setShowCreateForm(false)}
                  className="px-4 py-2 text-gray-600 border border-gray-300 rounded-md hover:bg-gray-50"
                >
                  Cancel
                </button>
                <button
                  type="submit"
                  className="px-4 py-2 bg-blue-600 text-white rounded-md hover:bg-blue-700"
                >
                  Schedule Interview
                </button>
              </div>
            </form>
          </div>
        </div>
      )}

      <div className="bg-white shadow rounded-lg">
        <div className="px-6 py-4 border-b border-gray-200">
          <h2 className="text-lg font-semibold text-gray-800">
            All Interviews ({interviewsList.length})
          </h2>
        </div>

        {interviewsList.length === 0 ? (
          <div className="px-6 py-8 text-center text-gray-500">
            No interviews scheduled yet.
          </div>
        ) : (
          <div className="divide-y divide-gray-200">
            {interviewsList.map((interview) => (
              <div key={interview.id} className="px-6 py-4">
                <div className="flex items-center justify-between">
                  <div className="flex-1">
                    <div className="flex items-center space-x-4">
                      <div>
                        <h3 className="text-sm font-medium text-gray-900">
                          {interview.candidateName || "Unknown Candidate"}
                        </h3>
                        <p className="text-sm text-gray-600">
                          {interview.jobTitle || "Unknown Position"}
                        </p>
                      </div>
                      <div className="text-sm text-gray-500">
                        <span className="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium bg-blue-100 text-blue-800">
                          {interview.interviewType || "Unknown Type"}
                        </span>
                      </div>
                      <div className="text-sm text-gray-500">
                        <span
                          className={`inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium ${
                            interview.status === "Completed"
                              ? "bg-green-100 text-green-800"
                              : interview.status === "In Progress"
                              ? "bg-yellow-100 text-yellow-800"
                              : interview.status === "Cancelled"
                              ? "bg-red-100 text-red-800"
                              : "bg-gray-100 text-gray-800"
                          }`}
                        >
                          {interview.status || "Planned"}
                        </span>
                      </div>
                    </div>
                  </div>

                  <div className="flex items-center space-x-2">
                    {interview.status !== "Completed" &&
                      interview.status !== "Cancelled" && (
                        <button
                          onClick={() => handleConductInterview(interview.id)}
                          className="text-sm bg-green-600 text-white px-3 py-1 rounded hover:bg-green-700"
                        >
                          Start Interview
                        </button>
                      )}
                    <button
                      onClick={() => handleDeleteInterview(interview.id)}
                      className="text-sm bg-red-600 text-white px-3 py-1 rounded hover:bg-red-700"
                    >
                      Delete
                    </button>
                  </div>
                </div>

                <div className="mt-2 text-xs text-gray-500">
                  <span>
                    Created:{" "}
                    {new Date(interview.createdAt).toLocaleDateString()}
                  </span>
                  {interview.schedules && interview.schedules.length > 0 && (
                    <span className="ml-4">
                      Schedules: {interview.schedules.length}
                    </span>
                  )}
                  {interview.feedbacks && interview.feedbacks.length > 0 && (
                    <span className="ml-4">
                      Feedback: {interview.feedbacks.length} items
                    </span>
                  )}
                </div>
              </div>
            ))}
          </div>
        )}
      </div>

      <div className="mt-6 grid grid-cols-1 md:grid-cols-4 gap-4">
        <div className="bg-white p-4 rounded-lg shadow">
          <h3 className="text-sm font-medium text-gray-500">
            Total Interviews
          </h3>
          <p className="text-2xl font-bold text-gray-900">
            {interviewsList.length}
          </p>
        </div>
        <div className="bg-white p-4 rounded-lg shadow">
          <h3 className="text-sm font-medium text-gray-500">Planned</h3>
          <p className="text-2xl font-bold text-blue-600">
            {
              interviewsList.filter((i) => !i.status || i.status === "Planned")
                .length
            }
          </p>
        </div>
        <div className="bg-white p-4 rounded-lg shadow">
          <h3 className="text-sm font-medium text-gray-500">In Progress</h3>
          <p className="text-2xl font-bold text-yellow-600">
            {interviewsList.filter((i) => i.status === "In Progress").length}
          </p>
        </div>
        <div className="bg-white p-4 rounded-lg shadow">
          <h3 className="text-sm font-medium text-gray-500">Completed</h3>
          <p className="text-2xl font-bold text-green-600">
            {interviewsList.filter((i) => i.status === "Completed").length}
          </p>
        </div>
      </div>
    </div>
  );
};

export default InterviewsPage;
