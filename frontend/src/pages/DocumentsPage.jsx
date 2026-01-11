import { useState, useEffect } from 'react';
import { documentAPI, candidatesAPI } from '../services/api';
import { useAuth } from '../contexts/AuthContext';
import toast from 'react-hot-toast';
import LoadingSpinner from '../components/LoadingSpinner';

const formatFileSize = (bytes) => {
  if (!bytes) return 'N/A';
  if (bytes === 0) return '0 Bytes';
  const k = 1024;
  const sizes = ['Bytes', 'KB', 'MB', 'GB'];
  const i = Math.floor(Math.log(bytes, k));
  return Math.round((bytes / Math.pow(k, i)) * 100) / 100 + ' ' + sizes[i];
};

export default function DocumentsPage() {
  const [documents, setDocuments] = useState([]);
  const [filteredDocuments, setFilteredDocuments] = useState([]);
  const [candidates, setCandidates] = useState([]);
  const [documentTypes, setDocumentTypes] = useState([]);
  const [loading, setLoading] = useState(true);
  const [searchTerm, setSearchTerm] = useState('');
  const [selectedCandidate, setSelectedCandidate] = useState('');
  const [selectedDocType, setSelectedDocType] = useState('');

  // Pagination
  const [currentPage, setCurrentPage] = useState(1);
  const itemsPerPage = 10;

  // Modal states
  const [showUploadModal, setShowUploadModal] = useState(false);
  const [showViewModal, setShowViewModal] = useState(false);
  const [selectedDocument, setSelectedDocument] = useState(null);
  const [uploading, setUploading] = useState(false);

  // Form data
  const [uploadForm, setUploadForm] = useState({
    candidateId: '',
    documentTypeId: '',
    file: null
  });

  const { user } = useAuth();
  const canUpload = user?.role === 'Admin' || user?.role === 'HR' || user?.role === 'Recruiter';
  const canManage = user?.role === 'Admin' || user?.role === 'HR';

  useEffect(() => {
    fetchData();
  }, []);

  // Filter documents
  useEffect(() => {
    let filtered = documents;

    if (searchTerm) {
      filtered = filtered.filter(
        (d) =>
          d.candidateName.toLowerCase().includes(searchTerm.toLowerCase()) ||
          d.originalFileName?.toLowerCase().includes(searchTerm.toLowerCase()) ||
          d.documentType.toLowerCase().includes(searchTerm.toLowerCase())
      );
    }

    if (selectedCandidate) {
      filtered = filtered.filter((d) => d.candidateId === selectedCandidate);
    }

    if (selectedDocType) {
      filtered = filtered.filter((d) => d.documentType === selectedDocType);
    }

    setFilteredDocuments(filtered);
    setCurrentPage(1);
  }, [documents, searchTerm, selectedCandidate, selectedDocType]);

  const fetchData = async () => {
    try {
      setLoading(true);
      const [docsData, candData, typesData] = await Promise.all([
        documentAPI.getAll(),
        candidatesAPI.search(),
        documentAPI.getDocumentTypes()
      ]);
      setDocuments(docsData || []);
      // candidatesAPI.search returns candidates array or object with candidates property
      setCandidates(Array.isArray(candData) ? candData : (candData?.candidates || []));
      setDocumentTypes(typesData || []);
    } catch (err) {
      toast.error('Failed to load documents');
      console.error(err);
    } finally {
      setLoading(false);
    }
  };

  const handleFileChange = (e) => {
    setUploadForm({
      ...uploadForm,
      file: e.target.files[0]
    });
  };

  const handleUpload = async (e) => {
    e.preventDefault();

    if (!uploadForm.candidateId || !uploadForm.documentTypeId || !uploadForm.file) {
      toast.error('Please fill in all required fields');
      return;
    }

    try {
      setUploading(true);
      const formData = new FormData();
      formData.append('CandidateId', uploadForm.candidateId);
      formData.append('DocumentTypeId', uploadForm.documentTypeId);
      formData.append('File', uploadForm.file);

      await documentAPI.upload(formData);
      toast.success('Document uploaded successfully');
      setShowUploadModal(false);
      setUploadForm({ candidateId: '', documentTypeId: '', file: null });
      await fetchData();
    } catch (err) {
      toast.error(err.response?.data?.message || 'Failed to upload document');
    } finally {
      setUploading(false);
    }
  };

  const handleViewDocument = (document) => {
    setSelectedDocument(document);
    setShowViewModal(true);
  };

  const handleDownload = async (document) => {
    try {
      const blob = await documentAPI.download(document.id);
      const url = window.URL.createObjectURL(blob);
      const link = document.createElement('a');
      link.href = url;
      link.download = document.originalFileName || 'document';
      document.body.appendChild(link);
      link.click();
      link.remove();
      window.URL.revokeObjectURL(url);
      toast.success('Document downloaded');
    } catch (err) {
      toast.error('Failed to download document');
    }
  };

  const handleDeleteDocument = async (document) => {
    if (!window.confirm(`Are you sure you want to delete "${document.originalFileName}"?`)) {
      return;
    }

    try {
      await documentAPI.delete(document.id);
      toast.success('Document deleted successfully');
      await fetchData();
    } catch (err) {
      toast.error('Failed to delete document');
    }
  };

  const closeAllModals = () => {
    setShowUploadModal(false);
    setShowViewModal(false);
    setSelectedDocument(null);
    setUploadForm({ candidateId: '', documentTypeId: '', file: null });
  };

  if (loading) return <LoadingSpinner />;

  // Pagination
  const paginatedDocuments = filteredDocuments.slice(
    (currentPage - 1) * itemsPerPage,
    currentPage * itemsPerPage
  );
  const totalPages = Math.ceil(filteredDocuments.length / itemsPerPage);

  return (
    <div className="max-w-7xl mx-auto p-6">
      {/* Header */}
      <div className="flex justify-between items-center mb-6">
        <div>
          <h1 className="text-3xl font-bold text-gray-900">Documents</h1>
          <p className="text-gray-600 mt-1">Manage candidate documents and files</p>
        </div>
        {canUpload && (
          <button
            onClick={() => setShowUploadModal(true)}
            className="px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 transition-colors"
          >
            + Upload Document
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
              placeholder="Search by candidate, file, or type..."
              value={searchTerm}
              onChange={(e) => setSearchTerm(e.target.value)}
              className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
            />
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

          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">Document Type</label>
            <select
              value={selectedDocType}
              onChange={(e) => setSelectedDocType(e.target.value)}
              className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
            >
              <option value="">All Types</option>
              {documentTypes.map((type) => (
                <option key={type.id} value={type.type}>
                  {type.type}
                </option>
              ))}
            </select>
          </div>

          <div className="flex items-end">
            <button
              onClick={() => {
                setSearchTerm('');
                setSelectedCandidate('');
                setSelectedDocType('');
              }}
              className="w-full px-3 py-2 text-gray-700 bg-gray-200 rounded-lg hover:bg-gray-300 transition-colors"
            >
              Clear Filters
            </button>
          </div>
        </div>
      </div>

      {/* Documents Table */}
      <div className="bg-white rounded-lg shadow-md overflow-hidden">
        <div className="overflow-x-auto">
          <table className="w-full">
            <thead className="bg-gray-50 border-b border-gray-200">
              <tr>
                <th className="px-6 py-3 text-left text-sm font-semibold text-gray-900">
                  Candidate
                </th>
                <th className="px-6 py-3 text-left text-sm font-semibold text-gray-900">
                  Document Type
                </th>
                <th className="px-6 py-3 text-left text-sm font-semibold text-gray-900">File Name</th>
                <th className="px-6 py-3 text-left text-sm font-semibold text-gray-900">Size</th>
                <th className="px-6 py-3 text-left text-sm font-semibold text-gray-900">
                  Uploaded At
                </th>
                <th className="px-6 py-3 text-right text-sm font-semibold text-gray-900">
                  Actions
                </th>
              </tr>
            </thead>
            <tbody className="divide-y divide-gray-200">
              {paginatedDocuments.length > 0 ? (
                paginatedDocuments.map((doc) => (
                  <tr key={doc.id} className="hover:bg-gray-50 transition-colors">
                    <td className="px-6 py-4 text-sm font-medium text-gray-900">
                      {doc.candidateName}
                    </td>
                    <td className="px-6 py-4 text-sm">
                      <span className="px-2 py-1 bg-blue-100 text-blue-800 rounded-full text-xs font-medium">
                        {doc.documentType}
                      </span>
                    </td>
                    <td className="px-6 py-4 text-sm text-gray-600 max-w-xs truncate">
                      {doc.originalFileName || 'Unknown'}
                    </td>
                    <td className="px-6 py-4 text-sm text-gray-600">
                      {formatFileSize(doc.sizeBytes)}
                    </td>
                    <td className="px-6 py-4 text-sm text-gray-600">
                      {new Date(doc.uploadedAt).toLocaleDateString()}
                    </td>
                    <td className="px-6 py-4 text-sm text-right">
                      <div className="flex justify-end gap-2">
                        <button
                          onClick={() => handleViewDocument(doc)}
                          className="text-blue-600 hover:text-blue-900 transition-colors"
                          title="View"
                        >
                          👁️
                        </button>
                        <button
                          onClick={() => handleDownload(doc)}
                          className="text-green-600 hover:text-green-900 transition-colors"
                          title="Download"
                        >
                          ⬇️
                        </button>
                        {canManage && (
                          <button
                            onClick={() => handleDeleteDocument(doc)}
                            className="text-red-600 hover:text-red-900 transition-colors"
                            title="Delete"
                          >
                            🗑️
                          </button>
                        )}
                      </div>
                    </td>
                  </tr>
                ))
              ) : (
                <tr>
                  <td colSpan="6" className="px-6 py-8 text-center text-gray-500">
                    No documents found
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
              {Math.min(currentPage * itemsPerPage, filteredDocuments.length)} of{' '}
              {filteredDocuments.length} documents
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

      {/* Upload Modal */}
      {showUploadModal && (
        <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50 p-4">
          <div className="bg-white rounded-lg max-w-lg w-full">
            <div className="sticky top-0 bg-white border-b border-gray-200 px-6 py-4">
              <h2 className="text-2xl font-bold text-gray-900">Upload Document</h2>
            </div>

            <form onSubmit={handleUpload} className="p-6 space-y-4">
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">
                  Candidate *
                </label>
                <select
                  value={uploadForm.candidateId}
                  onChange={(e) =>
                    setUploadForm({ ...uploadForm, candidateId: e.target.value })
                  }
                  className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
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
                <label className="block text-sm font-medium text-gray-700 mb-1">
                  Document Type *
                </label>
                <select
                  value={uploadForm.documentTypeId}
                  onChange={(e) =>
                    setUploadForm({ ...uploadForm, documentTypeId: e.target.value })
                  }
                  className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                  required
                >
                  <option value="">Select Document Type</option>
                  {documentTypes.map((type) => (
                    <option key={type.id} value={type.id}>
                      {type.type}
                    </option>
                  ))}
                </select>
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">File *</label>
                <div className="relative border-2 border-dashed border-gray-300 rounded-lg p-6 text-center hover:border-blue-500 transition-colors cursor-pointer">
                  <input
                    type="file"
                    onChange={handleFileChange}
                    className="absolute inset-0 w-full h-full opacity-0 cursor-pointer"
                    required
                  />
                  <div>
                    <p className="text-gray-600">
                      {uploadForm.file ? uploadForm.file.name : 'Click to select file'}
                    </p>
                    <p className="text-xs text-gray-500 mt-1">Max 10 MB</p>
                  </div>
                </div>
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
                  disabled={uploading}
                  className="px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 disabled:opacity-50 transition-colors"
                >
                  {uploading ? 'Uploading...' : 'Upload'}
                </button>
              </div>
            </form>
          </div>
        </div>
      )}

      {/* View Modal */}
      {showViewModal && selectedDocument && (
        <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50 p-4">
          <div className="bg-white rounded-lg max-w-2xl w-full max-h-[90vh] overflow-y-auto">
            <div className="sticky top-0 bg-white border-b border-gray-200 px-6 py-4 flex justify-between items-center">
              <h2 className="text-2xl font-bold text-gray-900">Document Details</h2>
              <button
                onClick={closeAllModals}
                className="text-gray-500 hover:text-gray-700"
              >
                ✕
              </button>
            </div>

            <div className="p-6 space-y-4">
              <div>
                <label className="block text-sm font-semibold text-gray-700 mb-1">Candidate</label>
                <p className="text-gray-900">{selectedDocument.candidateName}</p>
              </div>

              <div>
                <label className="block text-sm font-semibold text-gray-700 mb-1">
                  Document Type
                </label>
                <div className="px-2 py-1 bg-blue-100 text-blue-800 rounded-full text-sm font-medium inline-block">
                  {selectedDocument.documentType}
                </div>
              </div>

              <div>
                <label className="block text-sm font-semibold text-gray-700 mb-1">File Name</label>
                <p className="text-gray-900">{selectedDocument.originalFileName || 'Unknown'}</p>
              </div>

              <div>
                <label className="block text-sm font-semibold text-gray-700 mb-1">File Size</label>
                <p className="text-gray-900">{formatFileSize(selectedDocument.sizeBytes)}</p>
              </div>

              <div>
                <label className="block text-sm font-semibold text-gray-700 mb-1">MIME Type</label>
                <p className="text-gray-900 text-sm font-mono">{selectedDocument.mimeType || 'N/A'}</p>
              </div>

              <div>
                <label className="block text-sm font-semibold text-gray-700 mb-1">Uploaded At</label>
                <p className="text-gray-600">
                  {new Date(selectedDocument.uploadedAt).toLocaleString()}
                </p>
              </div>

              {selectedDocument.uploadedByName && (
                <div>
                  <label className="block text-sm font-semibold text-gray-700 mb-1">
                    Uploaded By
                  </label>
                  <p className="text-gray-900">{selectedDocument.uploadedByName}</p>
                </div>
              )}

              <div className="flex justify-end gap-3 pt-4 border-t border-gray-200">
                <button
                  onClick={() => handleDownload(selectedDocument)}
                  className="px-4 py-2 bg-green-600 text-white rounded-lg hover:bg-green-700 transition-colors"
                >
                  Download
                </button>
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
