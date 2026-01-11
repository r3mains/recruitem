import { useState, useEffect } from 'react';
import { jobTypesAPI } from '../services/api';
import { useAuth } from '../contexts/AuthContext';
import toast from 'react-hot-toast';
import LoadingSpinner from '../components/LoadingSpinner';

export default function JobTypesPage() {
  const [jobTypes, setJobTypes] = useState([]);
  const [loading, setLoading] = useState(true);
  const [searchTerm, setSearchTerm] = useState('');
  const [currentPage, setCurrentPage] = useState(1);
  const [totalPages, setTotalPages] = useState(1);
  const [totalCount, setTotalCount] = useState(0);

  // Modal states
  const [showCreateModal, setShowCreateModal] = useState(false);
  const [showViewModal, setShowViewModal] = useState(false);
  const [selectedJobType, setSelectedJobType] = useState(null);
  const [isEditing, setIsEditing] = useState(false);

  // Form data
  const [formData, setFormData] = useState({
    type: ''
  });

  const itemsPerPage = 10;
  const { user } = useAuth();
  const canManage = user?.role === 'Admin';

  useEffect(() => {
    fetchJobTypes();
  }, [currentPage, searchTerm]);

  const fetchJobTypes = async () => {
    try {
      setLoading(true);
      const data = await jobTypesAPI.getAll(currentPage, itemsPerPage, searchTerm);
      setJobTypes(data || []);
      // Backend returns array directly, handle pagination from response
      if (Array.isArray(data) && data.length === 0) {
        setTotalPages(1);
        setTotalCount(0);
      } else if (Array.isArray(data)) {
        setTotalPages(Math.ceil(data.length / itemsPerPage));
        setTotalCount(data.length);
      }
    } catch (err) {
      toast.error('Failed to load job types');
      console.error(err);
    } finally {
      setLoading(false);
    }
  };

  const handleCreateNew = () => {
    setFormData({ type: '' });
    setSelectedJobType(null);
    setIsEditing(false);
    setShowCreateModal(true);
  };

  const handleEditJobType = (jobType) => {
    setFormData({ type: jobType.type });
    setSelectedJobType(jobType);
    setIsEditing(true);
    setShowCreateModal(true);
  };

  const handleSubmit = async (e) => {
    e.preventDefault();

    if (!formData.type.trim()) {
      toast.error('Please enter a job type name');
      return;
    }

    try {
      if (isEditing && selectedJobType) {
        await jobTypesAPI.update(selectedJobType.id, { type: formData.type });
        toast.success('Job type updated successfully');
      } else {
        await jobTypesAPI.create({ type: formData.type });
        toast.success('Job type created successfully');
      }

      setShowCreateModal(false);
      setCurrentPage(1);
      await fetchJobTypes();
    } catch (err) {
      if (err.response?.status === 400 && err.response?.data?.includes('already exists')) {
        toast.error('This job type already exists');
      } else {
        toast.error(err.response?.data?.message || 'Failed to save job type');
      }
    }
  };

  const handleViewJobType = async (jobType) => {
    try {
      const details = await jobTypesAPI.getById(jobType.id);
      setSelectedJobType(details);
      setShowViewModal(true);
    } catch (err) {
      toast.error('Failed to load job type details');
    }
  };

  const handleDeleteJobType = async (jobType) => {
    if (!window.confirm(`Are you sure you want to delete the job type "${jobType.type}"?`)) {
      return;
    }

    try {
      await jobTypesAPI.delete(jobType.id);
      toast.success('Job type deleted successfully');
      setCurrentPage(1);
      await fetchJobTypes();
    } catch (err) {
      if (err.response?.data?.includes('in use')) {
        toast.error('This job type is in use and cannot be deleted');
      } else {
        toast.error(err.response?.data?.message || 'Failed to delete job type');
      }
    }
  };

  const closeAllModals = () => {
    setShowCreateModal(false);
    setShowViewModal(false);
    setSelectedJobType(null);
    setIsEditing(false);
  };

  const handleSearch = (value) => {
    setSearchTerm(value);
    setCurrentPage(1);
  };

  if (loading && jobTypes.length === 0) return <LoadingSpinner />;

  return (
    <div className="max-w-7xl mx-auto p-6">
      {/* Header */}
      <div className="flex justify-between items-center mb-6">
        <div>
          <h1 className="text-3xl font-bold text-gray-900">Job Types Management</h1>
          <p className="text-gray-600 mt-1">Manage job classification types (Full-time, Part-time, Contract, etc.)</p>
        </div>
        {canManage && (
          <button
            onClick={handleCreateNew}
            className="px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 transition-colors"
          >
            + New Job Type
          </button>
        )}
      </div>

      {/* Search Bar */}
      <div className="bg-white rounded-lg shadow-md p-4 mb-6">
        <div className="flex gap-4">
          <div className="flex-1">
            <label className="block text-sm font-medium text-gray-700 mb-1">Search Job Types</label>
            <input
              type="text"
              placeholder="Search by job type name..."
              value={searchTerm}
              onChange={(e) => handleSearch(e.target.value)}
              className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
            />
          </div>
          {searchTerm && (
            <div className="flex items-end">
              <button
                onClick={() => handleSearch('')}
                className="px-4 py-2 text-gray-700 bg-gray-200 rounded-lg hover:bg-gray-300 transition-colors"
              >
                Clear
              </button>
            </div>
          )}
        </div>
      </div>

      {/* Job Types Table */}
      <div className="bg-white rounded-lg shadow-md overflow-hidden">
        <div className="overflow-x-auto">
          <table className="w-full">
            <thead className="bg-gray-50 border-b border-gray-200">
              <tr>
                <th className="px-6 py-3 text-left text-sm font-semibold text-gray-900">Job Type</th>
                <th className="px-6 py-3 text-left text-sm font-semibold text-gray-900">Jobs Count</th>
                <th className="px-6 py-3 text-left text-sm font-semibold text-gray-900">Status</th>
                <th className="px-6 py-3 text-right text-sm font-semibold text-gray-900">Actions</th>
              </tr>
            </thead>
            <tbody className="divide-y divide-gray-200">
              {jobTypes.length > 0 ? (
                jobTypes.map((jobType) => (
                  <tr key={jobType.id} className="hover:bg-gray-50 transition-colors">
                    <td className="px-6 py-4 text-sm font-medium text-gray-900">{jobType.type}</td>
                    <td className="px-6 py-4 text-sm">
                      <span className="px-3 py-1 bg-blue-100 text-blue-800 rounded-full text-sm font-medium">
                        {jobType.jobCount || 0}
                      </span>
                    </td>
                    <td className="px-6 py-4 text-sm">
                      {jobType.jobCount > 0 ? (
                        <span className="px-3 py-1 bg-yellow-100 text-yellow-800 rounded-full text-sm font-medium">
                          In Use
                        </span>
                      ) : (
                        <span className="px-3 py-1 bg-gray-100 text-gray-800 rounded-full text-sm font-medium">
                          Unused
                        </span>
                      )}
                    </td>
                    <td className="px-6 py-4 text-sm text-right">
                      <div className="flex justify-end gap-2">
                        <button
                          onClick={() => handleViewJobType(jobType)}
                          className="text-blue-600 hover:text-blue-900 transition-colors"
                          title="View Details"
                        >
                          👁️
                        </button>
                        {canManage && (
                          <>
                            <button
                              onClick={() => handleEditJobType(jobType)}
                              className="text-yellow-600 hover:text-yellow-900 transition-colors"
                              title="Edit"
                            >
                              ✏️
                            </button>
                            <button
                              onClick={() => handleDeleteJobType(jobType)}
                              className="text-red-600 hover:text-red-900 transition-colors"
                              title="Delete"
                            >
                              🗑️
                            </button>
                          </>
                        )}
                      </div>
                    </td>
                  </tr>
                ))
              ) : (
                <tr>
                  <td colSpan="4" className="px-6 py-8 text-center text-gray-500">
                    {loading ? 'Loading job types...' : 'No job types found'}
                  </td>
                </tr>
              )}
            </tbody>
          </table>
        </div>

        {/* Pagination */}
        {totalPages > 1 && (
          <div className="bg-gray-50 px-6 py-4 flex items-center justify-between border-t border-gray-200">
            <div className="text-sm text-gray-600">
              Showing {(currentPage - 1) * itemsPerPage + 1} to{' '}
              {Math.min(currentPage * itemsPerPage, totalCount)} of {totalCount} job types
            </div>
            <div className="flex gap-2">
              <button
                onClick={() => setCurrentPage((p) => Math.max(1, p - 1))}
                disabled={currentPage === 1}
                className="px-3 py-1 text-sm bg-gray-200 text-gray-700 rounded hover:bg-gray-300 disabled:opacity-50"
              >
                Previous
              </button>
              {Array.from({ length: totalPages }, (_, i) => i + 1)
                .slice(Math.max(0, currentPage - 2), Math.min(totalPages, currentPage + 1))
                .map((page) => (
                  <button
                    key={page}
                    onClick={() => setCurrentPage(page)}
                    className={`px-3 py-1 text-sm rounded ${
                      currentPage === page
                        ? 'bg-blue-600 text-white'
                        : 'bg-gray-200 text-gray-700 hover:bg-gray-300'
                    }`}
                  >
                    {page}
                  </button>
                ))}
              <button
                onClick={() => setCurrentPage((p) => Math.min(totalPages, p + 1))}
                disabled={currentPage === totalPages}
                className="px-3 py-1 text-sm bg-gray-200 text-gray-700 rounded hover:bg-gray-300 disabled:opacity-50"
              >
                Next
              </button>
            </div>
          </div>
        )}
      </div>

      {/* Create/Edit Modal */}
      {showCreateModal && (
        <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50 p-4">
          <div className="bg-white rounded-lg max-w-lg w-full">
            <div className="sticky top-0 bg-white border-b border-gray-200 px-6 py-4">
              <h2 className="text-2xl font-bold text-gray-900">
                {isEditing ? 'Edit Job Type' : 'Create New Job Type'}
              </h2>
            </div>

            <form onSubmit={handleSubmit} className="p-6 space-y-4">
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">Job Type Name *</label>
                <input
                  type="text"
                  value={formData.type}
                  onChange={(e) => setFormData({ type: e.target.value })}
                  placeholder="e.g., Full-time, Part-time, Contract, Freelance, Temporary"
                  className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                  required
                  autoFocus
                />
                <p className="mt-1 text-xs text-gray-500">Enter the classification for this job type</p>
              </div>

              <div className="flex justify-end gap-3 pt-4 border-t border-gray-200">
                <button
                  type="button"
                  onClick={closeAllModals}
                  className="px-4 py-2 text-gray-700 bg-gray-200 rounded-lg hover:bg-gray-300 transition-colors"
                >
                  Cancel
                </button>
                <button
                  type="submit"
                  className="px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 transition-colors"
                >
                  {isEditing ? 'Update Job Type' : 'Create Job Type'}
                </button>
              </div>
            </form>
          </div>
        </div>
      )}

      {/* View Modal */}
      {showViewModal && selectedJobType && (
        <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50 p-4">
          <div className="bg-white rounded-lg max-w-2xl w-full">
            <div className="sticky top-0 bg-white border-b border-gray-200 px-6 py-4 flex justify-between items-center">
              <h2 className="text-2xl font-bold text-gray-900">{selectedJobType.type}</h2>
              <button
                onClick={closeAllModals}
                className="text-gray-500 hover:text-gray-700"
              >
                ✕
              </button>
            </div>

            <div className="p-6">
              <div className="space-y-4">
                {/* Job Count Card */}
                <div className="bg-blue-50 rounded-lg p-6 border border-blue-200">
                  <p className="text-sm text-gray-600">Jobs Using This Type</p>
                  <p className="text-4xl font-bold text-blue-600 mt-2">
                    {selectedJobType.jobCount || 0}
                  </p>
                  {selectedJobType.jobCount === 0 && (
                    <p className="mt-2 text-sm text-gray-600">No jobs currently use this type</p>
                  )}
                </div>

                {/* Status Card */}
                <div className="bg-gray-50 rounded-lg p-6 border border-gray-200">
                  <p className="text-sm font-medium text-gray-700">Status</p>
                  <p className="mt-2">
                    {selectedJobType.jobCount > 0 ? (
                      <span className="px-3 py-1 bg-yellow-100 text-yellow-800 rounded-full text-sm font-medium">
                        ⚠️ In Use - Cannot be deleted
                      </span>
                    ) : (
                      <span className="px-3 py-1 bg-green-100 text-green-800 rounded-full text-sm font-medium">
                        ✅ Available - Can be deleted
                      </span>
                    )}
                  </p>
                </div>

                <div className="text-xs text-gray-500 mt-4">
                  <p>ID: {selectedJobType.id}</p>
                </div>
              </div>

              <div className="flex justify-end gap-3 pt-4 border-t border-gray-200 mt-6">
                <button
                  onClick={closeAllModals}
                  className="px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 transition-colors"
                >
                  Close
                </button>
              </div>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}
