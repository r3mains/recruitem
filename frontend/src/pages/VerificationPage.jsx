import { useState, useEffect } from 'react';
import { verificationAPI, candidatesAPI, documentAPI } from '../services/api';
import { useAuth } from '../contexts/AuthContext';
import toast from 'react-hot-toast';
import LoadingSpinner from '../components/LoadingSpinner';

const VERIFICATION_STATUS = [
  { id: '80000000-0000-0000-0000-000000000001', name: 'Pending' },
  { id: '80000000-0000-0000-0000-000000000002', name: 'In Progress' },
  { id: '80000000-0000-0000-0000-000000000003', name: 'Verified' },
  { id: '80000000-0000-0000-0000-000000000004', name: 'Rejected' }
];

export default function VerificationPage() {
  const [verifications, setVerifications] = useState([]);
  const [filteredVerifications, setFilteredVerifications] = useState([]);
  const [candidates, setCandidates] = useState([]);
  const [documents, setDocuments] = useState([]);
  const [loading, setLoading] = useState(true);
  const [searchTerm, setSearchTerm] = useState('');
  const [selectedStatus, setSelectedStatus] = useState('');
  const [selectedCandidate, setSelectedCandidate] = useState('');

  // Pagination
  const [currentPage, setCurrentPage] = useState(1);
  const itemsPerPage = 10;

  // Modal states
  const [showCreateModal, setShowCreateModal] = useState(false);
  const [showViewModal, setShowViewModal] = useState(false);
  const [selectedVerification, setSelectedVerification] = useState(null);
  const [isEditing, setIsEditing] = useState(false);

  // Form data
  const [formData, setFormData] = useState({
    candidateId: '',
    documentId: '',
    statusId: '80000000-0000-0000-0000-000000000001',
    comments: ''
  });

  const { user } = useAuth();
  const canVerify = user?.role === 'Admin' || user?.role === 'HR';

  useEffect(() => {
    fetchData();
  }, []);

  // Filter verifications
  useEffect(() => {
    let filtered = verifications;

    if (searchTerm) {
      filtered = filtered.filter(
        (v) =>
          v.candidateName.toLowerCase().includes(searchTerm.toLowerCase()) ||
          v.documentType.toLowerCase().includes(searchTerm.toLowerCase())
      );
    }

    if (selectedStatus) {
      filtered = filtered.filter((v) => v.status === selectedStatus);
    }

    if (selectedCandidate) {
      filtered = filtered.filter((v) => v.candidateId === selectedCandidate);
    }

    setFilteredVerifications(filtered);
    setCurrentPage(1);
  }, [verifications, searchTerm, selectedStatus, selectedCandidate]);

  const fetchData = async () => {
    try {
      setLoading(true);
      const [verifData, candData, docData] = await Promise.all([
        verificationAPI.getAll(),
        candidatesAPI.search(),
        documentAPI.getAll()
      ]);
      setVerifications(verifData || []);
      // candidatesAPI.search returns candidates array or object with candidates property
      setCandidates(Array.isArray(candData) ? candData : (candData?.candidates || []));
      setDocuments(docData || []);
    } catch (err) {
      toast.error('Failed to load verifications');
      console.error(err);
    } finally {
      setLoading(false);
    }
  };

  const handleCreateNew = () => {
    setFormData({
      candidateId: '',
      documentId: '',
      statusId: '80000000-0000-0000-0000-000000000001',
      comments: ''
    });
    setSelectedVerification(null);
    setIsEditing(false);
    setShowCreateModal(true);
  };

  const handleEditVerification = (verification) => {
    setFormData({
      candidateId: verification.candidateId,
      documentId: verification.documentId,
      statusId: VERIFICATION_STATUS.find((s) => s.name === verification.status)?.id || '80000000-0000-0000-0000-000000000001',
      comments: verification.comments || ''
    });
    setSelectedVerification(verification);
    setIsEditing(true);
    setShowCreateModal(true);
  };

  const handleSubmit = async (e) => {
    e.preventDefault();

    if (!formData.candidateId || !formData.documentId) {
      toast.error('Please fill in all required fields');
      return;
    }

    try {
      if (isEditing && selectedVerification) {
        await verificationAPI.update(selectedVerification.id, {
          statusId: formData.statusId,
          comments: formData.comments
        });
        toast.success('Verification updated successfully');
      } else {
        await verificationAPI.create({
          candidateId: formData.candidateId,
          documentId: formData.documentId,
          statusId: formData.statusId,
          comments: formData.comments,
          verifiedBy: user.id
        });
        toast.success('Verification created successfully');
      }

      await fetchData();
      setShowCreateModal(false);
      setSelectedVerification(null);
    } catch (err) {
      toast.error(err.response?.data?.message || 'Failed to save verification');
    }
  };

  const handleViewVerification = (verification) => {
    setSelectedVerification(verification);
    setShowViewModal(true);
  };

  const handleDeleteVerification = async (verification) => {
    if (
      !window.confirm(
        `Are you sure you want to delete the verification for "${verification.candidateName}"?`
      )
    ) {
      return;
    }

    try {
      await verificationAPI.delete(verification.id);
      toast.success('Verification deleted successfully');
      await fetchData();
    } catch (err) {
      toast.error('Failed to delete verification');
    }
  };

  const closeAllModals = () => {
    setShowCreateModal(false);
    setShowViewModal(false);
    setSelectedVerification(null);
    setIsEditing(false);
  };

  const getStatusColor = (status) => {
    switch (status) {
      case 'Pending':
        return 'bg-yellow-100 text-yellow-800';
      case 'In Progress':
        return 'bg-blue-100 text-blue-800';
      case 'Verified':
        return 'bg-green-100 text-green-800';
      case 'Rejected':
        return 'bg-red-100 text-red-800';
      default:
        return 'bg-gray-100 text-gray-800';
    }
  };

  if (loading) return <LoadingSpinner />;

  // Pagination
  const paginatedVerifications = filteredVerifications.slice(
    (currentPage - 1) * itemsPerPage,
    currentPage * itemsPerPage
  );
  const totalPages = Math.ceil(filteredVerifications.length / itemsPerPage);

  return (
    <div className="max-w-7xl mx-auto p-6">
      {/* Header */}
      <div className="flex justify-between items-center mb-6">
        <div>
          <h1 className="text-3xl font-bold text-gray-900">Verification Management</h1>
          <p className="text-gray-600 mt-1">Track and manage candidate document verifications</p>
        </div>
        {canVerify && (
          <button
            onClick={handleCreateNew}
            className="px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 transition-colors"
          >
            + New Verification
          </button>
        )}
      </div>

      {/* Search and Filter Bar */}
      <div className="bg-white rounded-lg shadow-md p-4 mb-6">
        <div className="grid grid-cols-1 md:grid-cols-4 gap-4">
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">Search</label>
            <input
              type="text"
              placeholder="Search by candidate or document..."
              value={searchTerm}
              onChange={(e) => setSearchTerm(e.target.value)}
              className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
            />
          </div>

          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">Status</label>
            <select
              value={selectedStatus}
              onChange={(e) => setSelectedStatus(e.target.value)}
              className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
            >
              <option value="">All Status</option>
              {VERIFICATION_STATUS.map((status) => (
                <option key={status.id} value={status.name}>
                  {status.name}
                </option>
              ))}
            </select>
          </div>

          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">Candidate</label>
            <select
              value={selectedCandidate}
              onChange={(e) => setSelectedCandidate(e.target.value)}
              className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
            >
              <option value="">All Candidates</option>
              {candidates.map((c) => (
                <option key={c.id} value={c.id}>
                  {c.fullName || `${c.firstName || ''} ${c.lastName || ''}`.trim()}
                </option>
              ))}
            </select>
          </div>

          <div className="flex items-end">
            <button
              onClick={() => {
                setSearchTerm('');
                setSelectedStatus('');
                setSelectedCandidate('');
              }}
              className="w-full px-3 py-2 text-gray-700 bg-gray-200 rounded-lg hover:bg-gray-300 transition-colors"
            >
              Clear Filters
            </button>
          </div>
        </div>
      </div>

      {/* Verifications Table */}
      <div className="bg-white rounded-lg shadow-md overflow-hidden">
        <div className="overflow-x-auto">
          <table className="w-full">
            <thead className="bg-gray-50 border-b border-gray-200">
              <tr>
                <th className="px-6 py-3 text-left text-sm font-semibold text-gray-900">Candidate</th>
                <th className="px-6 py-3 text-left text-sm font-semibold text-gray-900">Document</th>
                <th className="px-6 py-3 text-left text-sm font-semibold text-gray-900">Status</th>
                <th className="px-6 py-3 text-left text-sm font-semibold text-gray-900">Verified By</th>
                <th className="px-6 py-3 text-left text-sm font-semibold text-gray-900">Verified At</th>
                <th className="px-6 py-3 text-right text-sm font-semibold text-gray-900">Actions</th>
              </tr>
            </thead>
            <tbody className="divide-y divide-gray-200">
              {paginatedVerifications.length > 0 ? (
                paginatedVerifications.map((verification) => (
                  <tr key={verification.id} className="hover:bg-gray-50 transition-colors">
                    <td className="px-6 py-4 text-sm font-medium text-gray-900">
                      {verification.candidateName}
                    </td>
                    <td className="px-6 py-4 text-sm text-gray-600">{verification.documentType}</td>
                    <td className="px-6 py-4 text-sm">
                      <span
                        className={`px-2 py-1 rounded-full text-xs font-medium ${getStatusColor(
                          verification.status
                        )}`}
                      >
                        {verification.status}
                      </span>
                    </td>
                    <td className="px-6 py-4 text-sm text-gray-600">{verification.verifiedByName || 'N/A'}</td>
                    <td className="px-6 py-4 text-sm text-gray-600">
                      {verification.verifiedAt
                        ? new Date(verification.verifiedAt).toLocaleDateString()
                        : 'Pending'}
                    </td>
                    <td className="px-6 py-4 text-sm text-right">
                      <div className="flex justify-end gap-2">
                        <button
                          onClick={() => handleViewVerification(verification)}
                          className="text-blue-600 hover:text-blue-900 transition-colors"
                          title="View"
                        >
                          👁️
                        </button>
                        {canVerify && (
                          <>
                            <button
                              onClick={() => handleEditVerification(verification)}
                              className="text-yellow-600 hover:text-yellow-900 transition-colors"
                              title="Edit"
                            >
                              ✏️
                            </button>
                            <button
                              onClick={() => handleDeleteVerification(verification)}
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
                  <td colSpan="6" className="px-6 py-8 text-center text-gray-500">
                    No verifications found
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
              {Math.min(currentPage * itemsPerPage, filteredVerifications.length)} of{' '}
              {filteredVerifications.length} verifications
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
                {isEditing ? 'Update Verification' : 'Create New Verification'}
              </h2>
            </div>

            <form onSubmit={handleSubmit} className="p-6 space-y-4">
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">Candidate *</label>
                <select
                  value={formData.candidateId}
                  onChange={(e) => setFormData({ ...formData, candidateId: e.target.value })}
                  disabled={isEditing}
                  className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent disabled:bg-gray-100"
                  required
                >
                  <option value="">Select Candidate</option>
                  {candidates.map((c) => (
                    <option key={c.id} value={c.id}>
                      {c.fullName || `${c.firstName || ''} ${c.lastName || ''}`.trim()}
                    </option>
                  ))}
                </select>
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">Document *</label>
                <select
                  value={formData.documentId}
                  onChange={(e) => setFormData({ ...formData, documentId: e.target.value })}
                  disabled={isEditing}
                  className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent disabled:bg-gray-100"
                  required
                >
                  <option value="">Select Document</option>
                  {documents
                    .filter(
                      (d) =>
                        !isEditing ||
                        d.id === selectedVerification?.documentId ||
                        !verifications.some((v) => v.documentId === d.id)
                    )
                    .map((d) => (
                      <option key={d.id} value={d.id}>
                        {d.candidateName} - {d.documentType} ({d.originalFileName})
                      </option>
                    ))}
                </select>
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">Status *</label>
                <select
                  value={formData.statusId}
                  onChange={(e) => setFormData({ ...formData, statusId: e.target.value })}
                  className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                  required
                >
                  {VERIFICATION_STATUS.map((status) => (
                    <option key={status.id} value={status.id}>
                      {status.name}
                    </option>
                  ))}
                </select>
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">Comments</label>
                <textarea
                  value={formData.comments}
                  onChange={(e) => setFormData({ ...formData, comments: e.target.value })}
                  placeholder="Add verification comments, issues, or notes..."
                  rows="4"
                  className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                />
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
                  {isEditing ? 'Update' : 'Create'}
                </button>
              </div>
            </form>
          </div>
        </div>
      )}

      {/* View Modal */}
      {showViewModal && selectedVerification && (
        <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50 p-4">
          <div className="bg-white rounded-lg max-w-2xl w-full max-h-[90vh] overflow-y-auto">
            <div className="sticky top-0 bg-white border-b border-gray-200 px-6 py-4 flex justify-between items-center">
              <h2 className="text-2xl font-bold text-gray-900">Verification Details</h2>
              <button
                onClick={closeAllModals}
                className="text-gray-500 hover:text-gray-700"
              >
                ✕
              </button>
            </div>

            <div className="p-6 space-y-4">
              <div className="grid grid-cols-2 gap-4">
                <div>
                  <label className="block text-sm font-semibold text-gray-700 mb-1">Candidate</label>
                  <p className="text-gray-900">{selectedVerification.candidateName}</p>
                </div>

                <div>
                  <label className="block text-sm font-semibold text-gray-700 mb-1">Document Type</label>
                  <p className="text-gray-900">{selectedVerification.documentType}</p>
                </div>
              </div>

              <div className="grid grid-cols-2 gap-4">
                <div>
                  <label className="block text-sm font-semibold text-gray-700 mb-1">Status</label>
                  <span
                    className={`px-2 py-1 rounded-full text-sm font-medium ${getStatusColor(
                      selectedVerification.status
                    )}`}
                  >
                    {selectedVerification.status}
                  </span>
                </div>

                <div>
                  <label className="block text-sm font-semibold text-gray-700 mb-1">Verified By</label>
                  <p className="text-gray-900">{selectedVerification.verifiedByName || 'N/A'}</p>
                </div>
              </div>

              <div>
                <label className="block text-sm font-semibold text-gray-700 mb-1">Verified At</label>
                <p className="text-gray-600">
                  {selectedVerification.verifiedAt
                    ? new Date(selectedVerification.verifiedAt).toLocaleString()
                    : 'Not yet verified'}
                </p>
              </div>

              {selectedVerification.comments && (
                <div>
                  <label className="block text-sm font-semibold text-gray-700 mb-1">Comments</label>
                  <p className="text-gray-900 bg-gray-50 p-3 rounded whitespace-pre-wrap">
                    {selectedVerification.comments}
                  </p>
                </div>
              )}

              <div className="flex justify-end gap-3 pt-4 border-t border-gray-200">
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
