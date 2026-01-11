import { useState, useEffect } from 'react';
import { qualificationsAPI } from '../services/api';
import { useAuth } from '../contexts/AuthContext';
import toast from 'react-hot-toast';
import LoadingSpinner from '../components/LoadingSpinner';

export default function QualificationsPage() {
  const [qualifications, setQualifications] = useState([]);
  const [loading, setLoading] = useState(true);
  const [searchTerm, setSearchTerm] = useState('');
  const [currentPage, setCurrentPage] = useState(1);
  const [totalPages, setTotalPages] = useState(1);
  const [totalCount, setTotalCount] = useState(0);
  const [statistics, setStatistics] = useState(null);

  // Modal states
  const [showCreateModal, setShowCreateModal] = useState(false);
  const [showViewModal, setShowViewModal] = useState(false);
  const [selectedQualification, setSelectedQualification] = useState(null);
  const [isEditing, setIsEditing] = useState(false);

  // Form data
  const [formData, setFormData] = useState({
    qualificationName: ''
  });

  const itemsPerPage = 10;
  const { user } = useAuth();
  const canManage = user?.role === 'Admin';

  useEffect(() => {
    fetchQualifications();
    fetchStatistics();
  }, [currentPage, searchTerm]);

  const fetchQualifications = async () => {
    try {
      setLoading(true);
      const data = await qualificationsAPI.getAll(currentPage, itemsPerPage, searchTerm);
      setQualifications(data || []);
      // Backend returns array directly, handle pagination from response
      if (Array.isArray(data) && data.length === 0) {
        setTotalPages(1);
        setTotalCount(0);
      } else if (Array.isArray(data)) {
        setTotalPages(Math.ceil(data.length / itemsPerPage));
        setTotalCount(data.length);
      }
    } catch (err) {
      toast.error('Failed to load qualifications');
      console.error(err);
    } finally {
      setLoading(false);
    }
  };

  const fetchStatistics = async () => {
    try {
      const stats = await qualificationsAPI.getStatistics();
      setStatistics(stats);
    } catch (err) {
      console.error('Failed to load statistics:', err);
    }
  };

  const handleCreateNew = () => {
    setFormData({ qualificationName: '' });
    setSelectedQualification(null);
    setIsEditing(false);
    setShowCreateModal(true);
  };

  const handleEditQualification = (qualification) => {
    setFormData({ qualificationName: qualification.qualificationName });
    setSelectedQualification(qualification);
    setIsEditing(true);
    setShowCreateModal(true);
  };

  const handleSubmit = async (e) => {
    e.preventDefault();

    if (!formData.qualificationName.trim()) {
      toast.error('Please enter a qualification name');
      return;
    }

    try {
      if (isEditing && selectedQualification) {
        await qualificationsAPI.update(selectedQualification.id, {
          qualificationName: formData.qualificationName
        });
        toast.success('Qualification updated successfully');
      } else {
        await qualificationsAPI.create({
          qualificationName: formData.qualificationName
        });
        toast.success('Qualification created successfully');
      }

      setShowCreateModal(false);
      setCurrentPage(1);
      await fetchQualifications();
      await fetchStatistics();
    } catch (err) {
      console.error('Qualification save error:', err.response?.data);
      
      // Handle validation errors
      if (err.response?.data?.errors) {
        const errors = Object.values(err.response.data.errors).flat();
        toast.error(errors.join('. '));
      } else if (err.response?.status === 400 && err.response?.data?.detail) {
        toast.error(err.response.data.detail);
      } else if (err.response?.status === 400 && typeof err.response?.data === 'string' && err.response.data.toLowerCase().includes('already exists')) {
        toast.error('This qualification already exists');
      } else {
        const errorMessage = err.response?.data?.title || err.response?.data?.detail || err.response?.data || 'Failed to save qualification';
        toast.error(typeof errorMessage === 'string' ? errorMessage : 'Failed to save qualification');
      }
    }
  };

  const handleViewQualification = async (qualification) => {
    try {
      const details = await qualificationsAPI.getById(qualification.id);
      setSelectedQualification(details);
      setShowViewModal(true);
    } catch (err) {
      toast.error('Failed to load qualification details');
    }
  };

  const handleDeleteQualification = async (qualification) => {
    if (!window.confirm(`Are you sure you want to delete the qualification "${qualification.qualificationName}"?`)) {
      return;
    }

    try {
      await qualificationsAPI.delete(qualification.id);
      toast.success('Qualification deleted successfully');
      setCurrentPage(1);
      await fetchQualifications();
      await fetchStatistics();
    } catch (err) {
      if (err.response?.data?.includes('in use')) {
        toast.error('This qualification is in use and cannot be deleted');
      } else {
        toast.error(err.response?.data?.message || 'Failed to delete qualification');
      }
    }
  };

  const closeAllModals = () => {
    setShowCreateModal(false);
    setShowViewModal(false);
    setSelectedQualification(null);
    setIsEditing(false);
  };

  const handleSearch = (value) => {
    setSearchTerm(value);
    setCurrentPage(1);
  };

  if (loading && qualifications.length === 0) return <LoadingSpinner />;

  return (
    <div className="max-w-7xl mx-auto p-6">
      {/* Header */}
      <div className="flex justify-between items-center mb-6">
        <div>
          <h1 className="text-3xl font-bold text-gray-900">Qualifications Management</h1>
          <p className="text-gray-600 mt-1">Manage educational and professional qualifications</p>
        </div>
        {canManage && (
          <button
            onClick={handleCreateNew}
            className="px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 transition-colors"
          >
            + New Qualification
          </button>
        )}
      </div>

      {/* Statistics Cards */}
      {statistics && (
        <div className="grid grid-cols-1 md:grid-cols-4 gap-4 mb-6">
          <div className="bg-white rounded-lg shadow-md p-4 border-l-4 border-blue-600">
            <p className="text-sm text-gray-600">Total Qualifications</p>
            <p className="text-3xl font-bold text-blue-600 mt-1">{statistics.totalQualifications}</p>
          </div>

          <div className="bg-white rounded-lg shadow-md p-4 border-l-4 border-green-600">
            <p className="text-sm text-gray-600">In Use</p>
            <p className="text-3xl font-bold text-green-600 mt-1">{statistics.inUseQualifications}</p>
          </div>

          <div className="bg-white rounded-lg shadow-md p-4 border-l-4 border-yellow-600">
            <p className="text-sm text-gray-600">Available</p>
            <p className="text-3xl font-bold text-yellow-600 mt-1">{statistics.availableQualifications}</p>
          </div>

          <div className="bg-white rounded-lg shadow-md p-4 border-l-4 border-purple-600">
            <p className="text-sm text-gray-600">Usage Rate</p>
            <p className="text-3xl font-bold text-purple-600 mt-1">{statistics.usagePercentage}%</p>
          </div>
        </div>
      )}

      {/* Search Bar */}
      <div className="bg-white rounded-lg shadow-md p-4 mb-6">
        <div className="flex gap-4">
          <div className="flex-1">
            <label className="block text-sm font-medium text-gray-700 mb-1">Search Qualifications</label>
            <input
              type="text"
              placeholder="Search by qualification name..."
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

      {/* Qualifications Table */}
      <div className="bg-white rounded-lg shadow-md overflow-hidden">
        <div className="overflow-x-auto">
          <table className="w-full">
            <thead className="bg-gray-50 border-b border-gray-200">
              <tr>
                <th className="px-6 py-3 text-left text-sm font-semibold text-gray-900">Qualification Name</th>
                <th className="px-6 py-3 text-left text-sm font-semibold text-gray-900">Jobs</th>
                <th className="px-6 py-3 text-left text-sm font-semibold text-gray-900">Candidates</th>
                <th className="px-6 py-3 text-left text-sm font-semibold text-gray-900">Total Usage</th>
                <th className="px-6 py-3 text-right text-sm font-semibold text-gray-900">Actions</th>
              </tr>
            </thead>
            <tbody className="divide-y divide-gray-200">
              {qualifications.length > 0 ? (
                qualifications.map((qualification) => (
                  <tr key={qualification.id} className="hover:bg-gray-50 transition-colors">
                    <td className="px-6 py-4 text-sm font-medium text-gray-900">{qualification.qualificationName}</td>
                    <td className="px-6 py-4 text-sm">
                      <span className="px-3 py-1 bg-blue-100 text-blue-800 rounded-full text-sm font-medium">
                        {qualification.jobCount || 0}
                      </span>
                    </td>
                    <td className="px-6 py-4 text-sm">
                      <span className="px-3 py-1 bg-green-100 text-green-800 rounded-full text-sm font-medium">
                        {qualification.candidateCount || 0}
                      </span>
                    </td>
                    <td className="px-6 py-4 text-sm font-semibold text-gray-900">
                      {(qualification.jobCount || 0) + (qualification.candidateCount || 0)}
                    </td>
                    <td className="px-6 py-4 text-sm text-right">
                      <div className="flex justify-end gap-2">
                        <button
                          onClick={() => handleViewQualification(qualification)}
                          className="text-blue-600 hover:text-blue-900 transition-colors"
                          title="View Details"
                        >
                          👁️
                        </button>
                        {canManage && (
                          <>
                            <button
                              onClick={() => handleEditQualification(qualification)}
                              className="text-yellow-600 hover:text-yellow-900 transition-colors"
                              title="Edit"
                            >
                              ✏️
                            </button>
                            <button
                              onClick={() => handleDeleteQualification(qualification)}
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
                  <td colSpan="5" className="px-6 py-8 text-center text-gray-500">
                    {loading ? 'Loading qualifications...' : 'No qualifications found'}
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
              {Math.min(currentPage * itemsPerPage, totalCount)} of {totalCount} qualifications
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
                {isEditing ? 'Edit Qualification' : 'Create New Qualification'}
              </h2>
            </div>

            <form onSubmit={handleSubmit} className="p-6 space-y-4">
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">Qualification Name *</label>
                <input
                  type="text"
                  value={formData.qualificationName}
                  onChange={(e) => setFormData({ qualificationName: e.target.value })}
                  placeholder="e.g., Bachelor of Computer Science, MBA, PMP Certification"
                  className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                  required
                  autoFocus
                />
                <p className="mt-1 text-xs text-gray-500">Enter the name of the educational or professional qualification</p>
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
                  {isEditing ? 'Update Qualification' : 'Create Qualification'}
                </button>
              </div>
            </form>
          </div>
        </div>
      )}

      {/* View Modal */}
      {showViewModal && selectedQualification && (
        <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50 p-4">
          <div className="bg-white rounded-lg max-w-2xl w-full">
            <div className="sticky top-0 bg-white border-b border-gray-200 px-6 py-4 flex justify-between items-center">
              <h2 className="text-2xl font-bold text-gray-900">{selectedQualification.qualificationName}</h2>
              <button
                onClick={closeAllModals}
                className="text-gray-500 hover:text-gray-700"
              >
                ✕
              </button>
            </div>

            <div className="p-6">
              <div className="space-y-4">
                {/* Usage Stats */}
                <div className="grid grid-cols-2 gap-4">
                  <div className="bg-blue-50 rounded-lg p-4 border border-blue-200">
                    <p className="text-sm text-gray-600">Used in Jobs</p>
                    <p className="text-3xl font-bold text-blue-600 mt-1">
                      {selectedQualification.jobCount || 0}
                    </p>
                  </div>

                  <div className="bg-green-50 rounded-lg p-4 border border-green-200">
                    <p className="text-sm text-gray-600">Used by Candidates</p>
                    <p className="text-3xl font-bold text-green-600 mt-1">
                      {selectedQualification.candidateCount || 0}
                    </p>
                  </div>
                </div>

                {/* Total Usage */}
                <div className="bg-gray-50 rounded-lg p-4 border border-gray-200">
                  <p className="text-sm font-medium text-gray-700">Total Usage</p>
                  <div className="mt-2 flex items-center gap-2">
                    <div className="flex-1 bg-gray-200 rounded-full h-3">
                      <div
                        className="bg-blue-600 h-3 rounded-full"
                        style={{
                          width: `${
                            Math.min(
                              100,
                              ((selectedQualification.jobCount || 0) +
                                (selectedQualification.candidateCount || 0)) *
                                5
                            )
                          }%`
                        }}
                      />
                    </div>
                    <span className="font-bold text-gray-900">
                      {(selectedQualification.jobCount || 0) +
                        (selectedQualification.candidateCount || 0)}
                    </span>
                  </div>
                </div>

                <div className="text-xs text-gray-500 mt-4">
                  <p>ID: {selectedQualification.id}</p>
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
