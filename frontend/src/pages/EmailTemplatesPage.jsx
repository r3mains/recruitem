import { useState, useEffect } from 'react';
import { emailTemplateAPI } from '../services/api';
import { useAuth } from '../contexts/AuthContext';
import toast from 'react-hot-toast';
import LoadingSpinner from '../components/LoadingSpinner';
import ErrorMessage from '../components/ErrorMessage';

const TEMPLATE_CATEGORIES = [
  'General',
  'Shortlist',
  'Interview',
  'Offer',
  'Rejection',
  'Confirmation'
];

export default function EmailTemplatesPage() {
  // State for templates and filters
  const [templates, setTemplates] = useState([]);
  const [filteredTemplates, setFilteredTemplates] = useState([]);
  const [loading, setLoading] = useState(true);
  const [searchTerm, setSearchTerm] = useState('');
  const [selectedCategory, setSelectedCategory] = useState('');
  
  // Pagination
  const [currentPage, setCurrentPage] = useState(1);
  const itemsPerPage = 10;
  
  // Modal states
  const [showCreateModal, setShowCreateModal] = useState(false);
  const [showViewModal, setShowViewModal] = useState(false);
  const [showPreviewModal, setShowPreviewModal] = useState(false);
  const [selectedTemplate, setSelectedTemplate] = useState(null);
  
  // Form data
  const [formData, setFormData] = useState({
    name: '',
    subject: '',
    body: '',
    description: '',
    category: 'General',
    availableVariables: '',
    isActive: true
  });

  // Preview state
  const [previewData, setPreviewData] = useState({
    subject: '',
    body: '',
    variables: {}
  });
  const [previewLoading, setPreviewLoading] = useState(false);

  const { user } = useAuth();
  const canManage = user?.role === 'Admin' || user?.role === 'HR';

  useEffect(() => {
    fetchTemplates();
  }, []);

  // Filter templates based on search and category
  useEffect(() => {
    let filtered = templates;

    if (searchTerm) {
      filtered = filtered.filter((t) =>
        t.name.toLowerCase().includes(searchTerm.toLowerCase()) ||
        t.subject.toLowerCase().includes(searchTerm.toLowerCase())
      );
    }

    if (selectedCategory) {
      filtered = filtered.filter((t) => t.category === selectedCategory);
    }

    setFilteredTemplates(filtered);
    setCurrentPage(1);
  }, [templates, searchTerm, selectedCategory]);

  const fetchTemplates = async () => {
    try {
      setLoading(true);
      const data = await emailTemplateAPI.getAll();
      setTemplates(data);
    } catch (err) {
      toast.error('Failed to load email templates');
      console.error(err);
    } finally {
      setLoading(false);
    }
  };

  const handleCreateNew = () => {
    setFormData({
      name: '',
      subject: '',
      body: '',
      description: '',
      category: 'General',
      availableVariables: '',
      isActive: true
    });
    setShowCreateModal(true);
  };

  const handleEditTemplate = (template) => {
    setFormData({
      name: template.name,
      subject: template.subject,
      body: template.body,
      description: template.description || '',
      category: template.category,
      availableVariables: template.availableVariables || '',
      isActive: template.isActive
    });
    setSelectedTemplate(template);
    setShowCreateModal(true);
  };

  const handleSubmit = async (e) => {
    e.preventDefault();

    try {
      if (selectedTemplate) {
        await emailTemplateAPI.update(selectedTemplate.id, formData);
        toast.success('Template updated successfully');
      } else {
        await emailTemplateAPI.create(formData);
        toast.success('Template created successfully');
      }

      await fetchTemplates();
      setShowCreateModal(false);
      setSelectedTemplate(null);
    } catch (err) {
      toast.error(err.response?.data?.message || 'Failed to save template');
    }
  };

  const handleViewTemplate = (template) => {
    setSelectedTemplate(template);
    setShowViewModal(true);
  };

  const handlePreviewTemplate = async (template) => {
    setSelectedTemplate(template);
    setPreviewData({
      subject: template.subject,
      body: template.body,
      variables: {}
    });
    setShowPreviewModal(true);
  };

  const handlePreviewWithVariables = async () => {
    if (!selectedTemplate) return;

    try {
      setPreviewLoading(true);
      const result = await emailTemplateAPI.preview(selectedTemplate.id, previewData.variables);
      setPreviewData({
        ...previewData,
        subject: result.subject,
        body: result.body
      });
      toast.success('Template preview generated');
    } catch (err) {
      toast.error('Failed to generate preview');
    } finally {
      setPreviewLoading(false);
    }
  };

  const handleDeleteTemplate = async (template) => {
    if (!window.confirm(`Are you sure you want to delete "${template.name}"?`)) {
      return;
    }

    try {
      await emailTemplateAPI.delete(template.id);
      toast.success('Template deleted successfully');
      await fetchTemplates();
    } catch (err) {
      toast.error('Failed to delete template');
    }
  };

  const handleToggleActive = async (template) => {
    try {
      await emailTemplateAPI.update(template.id, {
        ...template,
        isActive: !template.isActive
      });
      toast.success(`Template ${!template.isActive ? 'activated' : 'deactivated'}`);
      await fetchTemplates();
    } catch (err) {
      toast.error('Failed to update template status');
    }
  };

  const closeAllModals = () => {
    setShowCreateModal(false);
    setShowViewModal(false);
    setShowPreviewModal(false);
    setSelectedTemplate(null);
  };

  if (loading) return <LoadingSpinner />;

  // Pagination
  const paginatedTemplates = filteredTemplates.slice(
    (currentPage - 1) * itemsPerPage,
    currentPage * itemsPerPage
  );
  const totalPages = Math.ceil(filteredTemplates.length / itemsPerPage);

  return (
    <div className="max-w-7xl mx-auto p-6">
      {/* Header */}
      <div className="flex justify-between items-center mb-6">
        <div>
          <h1 className="text-3xl font-bold text-gray-900">Email Templates</h1>
          <p className="text-gray-600 mt-1">Manage email templates for recruitment communications</p>
        </div>
        {canManage && (
          <button
            onClick={handleCreateNew}
            className="px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 transition-colors"
          >
            + New Template
          </button>
        )}
      </div>

      {/* Search and Filter Bar */}
      <div className="bg-white rounded-lg shadow-md p-4 mb-6">
        <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">Search</label>
            <input
              type="text"
              placeholder="Search by name or subject..."
              value={searchTerm}
              onChange={(e) => setSearchTerm(e.target.value)}
              className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
            />
          </div>

          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">Category</label>
            <select
              value={selectedCategory}
              onChange={(e) => setSelectedCategory(e.target.value)}
              className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
            >
              <option value="">All Categories</option>
              {TEMPLATE_CATEGORIES.map((cat) => (
                <option key={cat} value={cat}>
                  {cat}
                </option>
              ))}
            </select>
          </div>

          <div className="flex items-end">
            <button
              onClick={() => {
                setSearchTerm('');
                setSelectedCategory('');
              }}
              className="w-full px-3 py-2 text-gray-700 bg-gray-200 rounded-lg hover:bg-gray-300 transition-colors"
            >
              Clear Filters
            </button>
          </div>
        </div>
      </div>

      {/* Templates Table */}
      <div className="bg-white rounded-lg shadow-md overflow-hidden">
        <div className="overflow-x-auto">
          <table className="w-full">
            <thead className="bg-gray-50 border-b border-gray-200">
              <tr>
                <th className="px-6 py-3 text-left text-sm font-semibold text-gray-900">Name</th>
                <th className="px-6 py-3 text-left text-sm font-semibold text-gray-900">Category</th>
                <th className="px-6 py-3 text-left text-sm font-semibold text-gray-900">Subject</th>
                <th className="px-6 py-3 text-left text-sm font-semibold text-gray-900">Status</th>
                <th className="px-6 py-3 text-left text-sm font-semibold text-gray-900">Created</th>
                <th className="px-6 py-3 text-right text-sm font-semibold text-gray-900">Actions</th>
              </tr>
            </thead>
            <tbody className="divide-y divide-gray-200">
              {paginatedTemplates.length > 0 ? (
                paginatedTemplates.map((template) => (
                  <tr key={template.id} className="hover:bg-gray-50 transition-colors">
                    <td className="px-6 py-4 text-sm font-medium text-gray-900">
                      {template.name}
                    </td>
                    <td className="px-6 py-4 text-sm">
                      <span className="px-2 py-1 bg-blue-100 text-blue-800 rounded-full text-xs font-medium">
                        {template.category}
                      </span>
                    </td>
                    <td className="px-6 py-4 text-sm text-gray-600 max-w-xs truncate">
                      {template.subject}
                    </td>
                    <td className="px-6 py-4 text-sm">
                      <span
                        className={`px-2 py-1 rounded-full text-xs font-medium ${
                          template.isActive
                            ? 'bg-green-100 text-green-800'
                            : 'bg-gray-100 text-gray-800'
                        }`}
                      >
                        {template.isActive ? 'Active' : 'Inactive'}
                      </span>
                    </td>
                    <td className="px-6 py-4 text-sm text-gray-600">
                      {new Date(template.createdAt).toLocaleDateString()}
                    </td>
                    <td className="px-6 py-4 text-sm text-right">
                      <div className="flex justify-end gap-2">
                        <button
                          onClick={() => handleViewTemplate(template)}
                          className="text-blue-600 hover:text-blue-900 transition-colors"
                          title="View"
                        >
                          👁️
                        </button>
                        <button
                          onClick={() => handlePreviewTemplate(template)}
                          className="text-purple-600 hover:text-purple-900 transition-colors"
                          title="Preview"
                        >
                          📧
                        </button>
                        {canManage && (
                          <>
                            <button
                              onClick={() => handleEditTemplate(template)}
                              className="text-yellow-600 hover:text-yellow-900 transition-colors"
                              title="Edit"
                            >
                              ✏️
                            </button>
                            <button
                              onClick={() => handleToggleActive(template)}
                              className="text-orange-600 hover:text-orange-900 transition-colors"
                              title={template.isActive ? 'Deactivate' : 'Activate'}
                            >
                              {template.isActive ? '🔒' : '🔓'}
                            </button>
                            <button
                              onClick={() => handleDeleteTemplate(template)}
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
                    No templates found
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
              {Math.min(currentPage * itemsPerPage, filteredTemplates.length)} of{' '}
              {filteredTemplates.length} templates
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
          <div className="bg-white rounded-lg max-w-2xl w-full max-h-[90vh] overflow-y-auto">
            <div className="sticky top-0 bg-white border-b border-gray-200 px-6 py-4">
              <h2 className="text-2xl font-bold text-gray-900">
                {selectedTemplate ? 'Edit Template' : 'Create New Template'}
              </h2>
            </div>

            <form onSubmit={handleSubmit} className="p-6 space-y-4">
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">
                  Template Name *
                </label>
                <input
                  type="text"
                  value={formData.name}
                  onChange={(e) => setFormData({ ...formData, name: e.target.value })}
                  placeholder="e.g., Offer Letter Template"
                  className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                  required
                />
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">Category *</label>
                <select
                  value={formData.category}
                  onChange={(e) => setFormData({ ...formData, category: e.target.value })}
                  className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                  required
                >
                  {TEMPLATE_CATEGORIES.map((cat) => (
                    <option key={cat} value={cat}>
                      {cat}
                    </option>
                  ))}
                </select>
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">Subject *</label>
                <input
                  type="text"
                  value={formData.subject}
                  onChange={(e) => setFormData({ ...formData, subject: e.target.value })}
                  placeholder="e.g., Job Offer - {{JobTitle}}"
                  className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                  required
                />
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">
                  Email Body *
                </label>
                <textarea
                  value={formData.body}
                  onChange={(e) => setFormData({ ...formData, body: e.target.value })}
                  placeholder="Use {{VariableName}} for template variables..."
                  rows="8"
                  className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent font-mono text-sm"
                  required
                />
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">Description</label>
                <textarea
                  value={formData.description}
                  onChange={(e) => setFormData({ ...formData, description: e.target.value })}
                  placeholder="Optional description of template purpose..."
                  rows="2"
                  className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                />
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">
                  Available Variables
                </label>
                <input
                  type="text"
                  value={formData.availableVariables}
                  onChange={(e) =>
                    setFormData({ ...formData, availableVariables: e.target.value })
                  }
                  placeholder="e.g., {{CandidateName}}, {{JobTitle}}, {{CompanyName}}"
                  className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                />
              </div>

              <div className="flex items-center">
                <input
                  type="checkbox"
                  id="isActive"
                  checked={formData.isActive}
                  onChange={(e) => setFormData({ ...formData, isActive: e.target.checked })}
                  className="rounded border-gray-300 text-blue-600"
                />
                <label htmlFor="isActive" className="ml-2 text-sm font-medium text-gray-700">
                  Active (available for use)
                </label>
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
                  {selectedTemplate ? 'Update Template' : 'Create Template'}
                </button>
              </div>
            </form>
          </div>
        </div>
      )}

      {/* View Modal */}
      {showViewModal && selectedTemplate && (
        <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50 p-4">
          <div className="bg-white rounded-lg max-w-2xl w-full max-h-[90vh] overflow-y-auto">
            <div className="sticky top-0 bg-white border-b border-gray-200 px-6 py-4 flex justify-between items-center">
              <h2 className="text-2xl font-bold text-gray-900">{selectedTemplate.name}</h2>
              <button
                onClick={closeAllModals}
                className="text-gray-500 hover:text-gray-700"
              >
                ✕
              </button>
            </div>

            <div className="p-6 space-y-4">
              <div>
                <label className="block text-sm font-semibold text-gray-700 mb-1">
                  Category
                </label>
                <div className="px-2 py-1 bg-blue-100 text-blue-800 rounded-full text-sm font-medium inline-block">
                  {selectedTemplate.category}
                </div>
              </div>

              <div>
                <label className="block text-sm font-semibold text-gray-700 mb-1">Status</label>
                <div
                  className={`px-2 py-1 rounded-full text-sm font-medium inline-block ${
                    selectedTemplate.isActive
                      ? 'bg-green-100 text-green-800'
                      : 'bg-gray-100 text-gray-800'
                  }`}
                >
                  {selectedTemplate.isActive ? 'Active' : 'Inactive'}
                </div>
              </div>

              {selectedTemplate.description && (
                <div>
                  <label className="block text-sm font-semibold text-gray-700 mb-1">
                    Description
                  </label>
                  <p className="text-gray-700">{selectedTemplate.description}</p>
                </div>
              )}

              <div>
                <label className="block text-sm font-semibold text-gray-700 mb-1">Subject</label>
                <p className="bg-gray-50 p-3 rounded border border-gray-200 text-gray-700 font-mono text-sm">
                  {selectedTemplate.subject}
                </p>
              </div>

              <div>
                <label className="block text-sm font-semibold text-gray-700 mb-1">Body</label>
                <div className="bg-gray-50 p-3 rounded border border-gray-200 text-gray-700 font-mono text-sm whitespace-pre-wrap max-h-64 overflow-y-auto">
                  {selectedTemplate.body}
                </div>
              </div>

              {selectedTemplate.availableVariables && (
                <div>
                  <label className="block text-sm font-semibold text-gray-700 mb-1">
                    Available Variables
                  </label>
                  <p className="text-gray-700 text-sm">{selectedTemplate.availableVariables}</p>
                </div>
              )}

              <div>
                <label className="block text-sm font-semibold text-gray-700 mb-1">Created</label>
                <p className="text-gray-600 text-sm">
                  {new Date(selectedTemplate.createdAt).toLocaleString()}
                </p>
              </div>

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

      {/* Preview Modal */}
      {showPreviewModal && selectedTemplate && (
        <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50 p-4">
          <div className="bg-white rounded-lg max-w-3xl w-full max-h-[90vh] overflow-y-auto">
            <div className="sticky top-0 bg-white border-b border-gray-200 px-6 py-4 flex justify-between items-center">
              <h2 className="text-2xl font-bold text-gray-900">Preview: {selectedTemplate.name}</h2>
              <button
                onClick={closeAllModals}
                className="text-gray-500 hover:text-gray-700"
              >
                ✕
              </button>
            </div>

            <div className="p-6 space-y-4">
              {/* Variables Input Section */}
              <div className="bg-blue-50 border border-blue-200 rounded-lg p-4">
                <h3 className="font-semibold text-gray-900 mb-3">Template Variables</h3>
                {selectedTemplate.availableVariables ? (
                  <div className="space-y-2">
                    {selectedTemplate.availableVariables.split(',').map((variable) => {
                      const varName = variable.trim();
                      return (
                        <div key={varName}>
                          <label className="block text-sm font-medium text-gray-700 mb-1">
                            {varName}
                          </label>
                          <input
                            type="text"
                            value={previewData.variables[varName] || ''}
                            onChange={(e) =>
                              setPreviewData({
                                ...previewData,
                                variables: {
                                  ...previewData.variables,
                                  [varName]: e.target.value
                                }
                              })
                            }
                            placeholder={`Enter value for ${varName}`}
                            className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500"
                          />
                        </div>
                      );
                    })}
                    <button
                      onClick={handlePreviewWithVariables}
                      disabled={previewLoading}
                      className="w-full mt-4 px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 disabled:opacity-50 transition-colors"
                    >
                      {previewLoading ? 'Generating Preview...' : 'Generate Preview'}
                    </button>
                  </div>
                ) : (
                  <p className="text-gray-600">No variables available for this template</p>
                )}
              </div>

              {/* Preview Section */}
              <div className="border-t border-gray-200 pt-4">
                <h3 className="font-semibold text-gray-900 mb-3">Email Preview</h3>
                <div className="bg-gray-50 border border-gray-200 rounded-lg p-4 space-y-4">
                  <div>
                    <label className="block text-sm font-medium text-gray-700 mb-1">Subject</label>
                    <div className="bg-white p-3 rounded border border-gray-200 font-mono text-sm">
                      {previewData.subject}
                    </div>
                  </div>

                  <div>
                    <label className="block text-sm font-medium text-gray-700 mb-1">Body</label>
                    <div className="bg-white p-3 rounded border border-gray-200 font-mono text-sm whitespace-pre-wrap max-h-64 overflow-y-auto">
                      {previewData.body}
                    </div>
                  </div>
                </div>
              </div>

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
