import { useState, useEffect } from 'react';
import { skillsAPI } from '../services/api';
import { useAuth } from '../contexts/AuthContext';
import toast from 'react-hot-toast';
import LoadingSpinner from '../components/LoadingSpinner';

export default function SkillsPage() {
  const [skills, setSkills] = useState([]);
  const [loading, setLoading] = useState(true);
  const [searchTerm, setSearchTerm] = useState('');
  const [currentPage, setCurrentPage] = useState(1);
  const [totalPages, setTotalPages] = useState(1);
  const [totalCount, setTotalCount] = useState(0);

  // Modal states
  const [showCreateModal, setShowCreateModal] = useState(false);
  const [showViewModal, setShowViewModal] = useState(false);
  const [selectedSkill, setSelectedSkill] = useState(null);
  const [isEditing, setIsEditing] = useState(false);

  // Form data
  const [formData, setFormData] = useState({
    skillName: ''
  });

  const itemsPerPage = 10;
  const { user } = useAuth();
  const canManage = user?.role === 'Admin' || user?.role === 'HR';

  useEffect(() => {
    fetchSkills();
  }, [currentPage, searchTerm]);

  const fetchSkills = async () => {
    try {
      setLoading(true);
      const data = await skillsAPI.getAll(currentPage, itemsPerPage, searchTerm);
      setSkills(data.skills || []);
      setTotalCount(data.pagination.totalCount);
      setTotalPages(data.pagination.totalPages);
    } catch (err) {
      toast.error('Failed to load skills');
      console.error(err);
    } finally {
      setLoading(false);
    }
  };

  const handleCreateNew = () => {
    setFormData({ skillName: '' });
    setSelectedSkill(null);
    setIsEditing(false);
    setShowCreateModal(true);
  };

  const handleEditSkill = (skill) => {
    setFormData({ skillName: skill.skillName });
    setSelectedSkill(skill);
    setIsEditing(true);
    setShowCreateModal(true);
  };

  const handleSubmit = async (e) => {
    e.preventDefault();

    if (!formData.skillName.trim()) {
      toast.error('Please enter a skill name');
      return;
    }

    try {
      if (isEditing && selectedSkill) {
        await skillsAPI.update(selectedSkill.id, { skillName: formData.skillName });
        toast.success('Skill updated successfully');
      } else {
        await skillsAPI.create({ skillName: formData.skillName });
        toast.success('Skill created successfully');
      }

      setShowCreateModal(false);
      setCurrentPage(1);
      await fetchSkills();
    } catch (err) {
      if (err.response?.status === 400 && err.response?.data?.includes('already exists')) {
        toast.error('This skill already exists');
      } else {
        toast.error(err.response?.data?.message || 'Failed to save skill');
      }
    }
  };

  const handleViewSkill = async (skill) => {
    try {
      const details = await skillsAPI.getById(skill.id);
      setSelectedSkill(details);
      setShowViewModal(true);
    } catch (err) {
      toast.error('Failed to load skill details');
    }
  };

  const handleDeleteSkill = async (skill) => {
    if (!window.confirm(`Are you sure you want to delete the skill "${skill.skillName}"?`)) {
      return;
    }

    try {
      await skillsAPI.delete(skill.id);
      toast.success('Skill deleted successfully');
      setCurrentPage(1);
      await fetchSkills();
    } catch (err) {
      toast.error(err.response?.data?.message || 'Failed to delete skill');
    }
  };

  const closeAllModals = () => {
    setShowCreateModal(false);
    setShowViewModal(false);
    setSelectedSkill(null);
    setIsEditing(false);
  };

  const handleSearch = (value) => {
    setSearchTerm(value);
    setCurrentPage(1);
  };

  if (loading && skills.length === 0) return <LoadingSpinner />;

  return (
    <div className="max-w-7xl mx-auto p-6">
      {/* Header */}
      <div className="flex justify-between items-center mb-6">
        <div>
          <h1 className="text-3xl font-bold text-gray-900">Skills Management</h1>
          <p className="text-gray-600 mt-1">Manage technical and soft skills for positions and candidates</p>
        </div>
        {canManage && (
          <button
            onClick={handleCreateNew}
            className="px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 transition-colors"
          >
            + New Skill
          </button>
        )}
      </div>

      {/* Search Bar */}
      <div className="bg-white rounded-lg shadow-md p-4 mb-6">
        <div className="flex gap-4">
          <div className="flex-1">
            <label className="block text-sm font-medium text-gray-700 mb-1">Search Skills</label>
            <input
              type="text"
              placeholder="Search by skill name..."
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

      {/* Skills Table */}
      <div className="bg-white rounded-lg shadow-md overflow-hidden">
        <div className="overflow-x-auto">
          <table className="w-full">
            <thead className="bg-gray-50 border-b border-gray-200">
              <tr>
                <th className="px-6 py-3 text-left text-sm font-semibold text-gray-900">Skill Name</th>
                <th className="px-6 py-3 text-left text-sm font-semibold text-gray-900">Positions</th>
                <th className="px-6 py-3 text-left text-sm font-semibold text-gray-900">Jobs</th>
                <th className="px-6 py-3 text-left text-sm font-semibold text-gray-900">Candidates</th>
                <th className="px-6 py-3 text-left text-sm font-semibold text-gray-900">Total Usage</th>
                <th className="px-6 py-3 text-right text-sm font-semibold text-gray-900">Actions</th>
              </tr>
            </thead>
            <tbody className="divide-y divide-gray-200">
              {skills.length > 0 ? (
                skills.map((skill) => (
                  <tr key={skill.id} className="hover:bg-gray-50 transition-colors">
                    <td className="px-6 py-4 text-sm font-medium text-gray-900">{skill.skillName}</td>
                    <td className="px-6 py-4 text-sm">
                      <span className="px-3 py-1 bg-blue-100 text-blue-800 rounded-full text-sm font-medium">
                        {skill.positionCount || 0}
                      </span>
                    </td>
                    <td className="px-6 py-4 text-sm">
                      <span className="px-3 py-1 bg-green-100 text-green-800 rounded-full text-sm font-medium">
                        {skill.jobCount || 0}
                      </span>
                    </td>
                    <td className="px-6 py-4 text-sm">
                      <span className="px-3 py-1 bg-purple-100 text-purple-800 rounded-full text-sm font-medium">
                        {skill.candidateCount || 0}
                      </span>
                    </td>
                    <td className="px-6 py-4 text-sm font-semibold text-gray-900">
                      {(skill.positionCount || 0) + (skill.jobCount || 0) + (skill.candidateCount || 0)}
                    </td>
                    <td className="px-6 py-4 text-sm text-right">
                      <div className="flex justify-end gap-2">
                        <button
                          onClick={() => handleViewSkill(skill)}
                          className="text-blue-600 hover:text-blue-900 transition-colors"
                          title="View Details"
                        >
                          👁️
                        </button>
                        {canManage && (
                          <>
                            <button
                              onClick={() => handleEditSkill(skill)}
                              className="text-yellow-600 hover:text-yellow-900 transition-colors"
                              title="Edit"
                            >
                              ✏️
                            </button>
                            <button
                              onClick={() => handleDeleteSkill(skill)}
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
                    {loading ? 'Loading skills...' : 'No skills found'}
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
              {Math.min(currentPage * itemsPerPage, totalCount)} of {totalCount} skills
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
                {isEditing ? 'Edit Skill' : 'Create New Skill'}
              </h2>
            </div>

            <form onSubmit={handleSubmit} className="p-6 space-y-4">
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">Skill Name *</label>
                <input
                  type="text"
                  value={formData.skillName}
                  onChange={(e) => setFormData({ skillName: e.target.value })}
                  placeholder="e.g., React, Node.js, Python, Project Management"
                  className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                  required
                  autoFocus
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
                  {isEditing ? 'Update Skill' : 'Create Skill'}
                </button>
              </div>
            </form>
          </div>
        </div>
      )}

      {/* View Modal */}
      {showViewModal && selectedSkill && (
        <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50 p-4">
          <div className="bg-white rounded-lg max-w-2xl w-full">
            <div className="sticky top-0 bg-white border-b border-gray-200 px-6 py-4 flex justify-between items-center">
              <h2 className="text-2xl font-bold text-gray-900">{selectedSkill.skillName}</h2>
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
                <div className="grid grid-cols-3 gap-4">
                  <div className="bg-blue-50 rounded-lg p-4 border border-blue-200">
                    <p className="text-sm text-gray-600">Used in Positions</p>
                    <p className="text-3xl font-bold text-blue-600 mt-1">
                      {selectedSkill.positionCount || 0}
                    </p>
                  </div>

                  <div className="bg-green-50 rounded-lg p-4 border border-green-200">
                    <p className="text-sm text-gray-600">Used in Jobs</p>
                    <p className="text-3xl font-bold text-green-600 mt-1">
                      {selectedSkill.jobCount || 0}
                    </p>
                  </div>

                  <div className="bg-purple-50 rounded-lg p-4 border border-purple-200">
                    <p className="text-sm text-gray-600">Candidate Skills</p>
                    <p className="text-3xl font-bold text-purple-600 mt-1">
                      {selectedSkill.candidateCount || 0}
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
                            ((selectedSkill.positionCount || 0) +
                              (selectedSkill.jobCount || 0) +
                              (selectedSkill.candidateCount || 0)) *
                            5
                          }%`
                        }}
                      />
                    </div>
                    <span className="font-bold text-gray-900">
                      {(selectedSkill.positionCount || 0) +
                        (selectedSkill.jobCount || 0) +
                        (selectedSkill.candidateCount || 0)}
                    </span>
                  </div>
                </div>

                <div className="text-xs text-gray-500 mt-4">
                  <p>ID: {selectedSkill.id}</p>
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
