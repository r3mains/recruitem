import { useState, useEffect } from 'react';
import { usersAPI } from '../services/api';
import { useAuth } from '../contexts/AuthContext';
import toast from 'react-hot-toast';
import LoadingSpinner from '../components/LoadingSpinner';

export default function UserManagementPage() {
  const [users, setUsers] = useState([]);
  const [loading, setLoading] = useState(true);
  const [searchTerm, setSearchTerm] = useState('');
  const [selectedRole, setSelectedRole] = useState('');
  const [currentPage, setCurrentPage] = useState(1);
  const [totalPages, setTotalPages] = useState(1);
  const [totalCount, setTotalCount] = useState(0);
  const [statistics, setStatistics] = useState(null);
  const [availableRoles, setAvailableRoles] = useState([]);

  // Modal states
  const [showCreateModal, setShowCreateModal] = useState(false);
  const [showEditModal, setShowEditModal] = useState(false);
  const [showViewModal, setShowViewModal] = useState(false);
  const [showRolesModal, setShowRolesModal] = useState(false);
  const [selectedUser, setSelectedUser] = useState(null);
  const [userRoles, setUserRoles] = useState([]);

  // Form data
  const [formData, setFormData] = useState({
    email: '',
    password: '',
    userName: ''
  });

  const itemsPerPage = 10;
  const { user } = useAuth();
  const canManage = user?.role === 'Admin';

  useEffect(() => {
    fetchRoles();
    fetchUsers();
    fetchStatistics();
  }, [currentPage, searchTerm, selectedRole]);

  const fetchRoles = async () => {
    try {
      const roles = await usersAPI.getAllRoles();
      setAvailableRoles(roles || []);
    } catch (err) {
      console.error('Failed to load roles:', err);
    }
  };

  const fetchUsers = async () => {
    try {
      setLoading(true);
      const data = await usersAPI.getAll(searchTerm, selectedRole, currentPage, itemsPerPage);
      
      if (data.items) {
        setUsers(data.items);
        setTotalCount(data.totalCount);
        setTotalPages(Math.ceil(data.totalCount / itemsPerPage));
      } else {
        setUsers([]);
      }
    } catch (err) {
      toast.error('Failed to load users');
      console.error(err);
    } finally {
      setLoading(false);
    }
  };

  const fetchStatistics = async () => {
    try {
      const stats = await usersAPI.getStatistics();
      setStatistics(stats);
    } catch (err) {
      console.error('Failed to load statistics:', err);
    }
  };

  const handleCreateNew = () => {
    setFormData({ email: '', password: '', userName: '' });
    setSelectedUser(null);
    setShowCreateModal(true);
  };

  const handleEditUser = (user) => {
    setFormData({ email: user.email || '', password: '', userName: user.userName || '' });
    setSelectedUser(user);
    setShowEditModal(true);
  };

  const handleSubmitCreate = async (e) => {
    e.preventDefault();

    if (!formData.email.trim()) {
      toast.error('Please enter an email address');
      return;
    }

    if (!formData.password.trim()) {
      toast.error('Please enter a password');
      return;
    }

    try {
      await usersAPI.create({
        email: formData.email,
        password: formData.password,
        userName: formData.userName || undefined
      });
      toast.success('User created successfully');
      setShowCreateModal(false);
      setCurrentPage(1);
      await fetchUsers();
      await fetchStatistics();
    } catch (err) {
      console.error('User creation error:', err.response?.data);
      if (err.response?.data?.errors) {
        const errors = Object.values(err.response.data.errors).flat();
        toast.error(errors.join('. '));
      } else {
        toast.error(err.response?.data?.message || 'Failed to create user');
      }
    }
  };

  const handleSubmitEdit = async (e) => {
    e.preventDefault();

    if (!formData.email.trim()) {
      toast.error('Please enter an email address');
      return;
    }

    try {
      await usersAPI.update(selectedUser.id, {
        email: formData.email,
        userName: formData.userName || undefined
      });
      toast.success('User updated successfully');
      setShowEditModal(false);
      await fetchUsers();
    } catch (err) {
      console.error('User update error:', err.response?.data);
      if (err.response?.data?.errors) {
        const errors = Object.values(err.response.data.errors).flat();
        toast.error(errors.join('. '));
      } else {
        toast.error(err.response?.data?.message || 'Failed to update user');
      }
    }
  };

  const handleViewUser = async (user) => {
    setSelectedUser(user);
    setShowViewModal(true);
  };

  const handleManageRoles = async (user) => {
    try {
      const rolesData = await usersAPI.getUserRoles(user.id);
      setSelectedUser(user);
      setUserRoles(rolesData.roles || []);
      setShowRolesModal(true);
    } catch (err) {
      toast.error('Failed to load user roles');
    }
  };

  const handleSaveRoles = async () => {
    if (!selectedUser) return;

    try {
      await usersAPI.updateRoles(selectedUser.id, userRoles);
      toast.success('User roles updated successfully');
      setShowRolesModal(false);
      await fetchUsers();
      await fetchStatistics();
    } catch (err) {
      console.error('Role update error:', err.response?.data);
      if (err.response?.data?.errors) {
        const errors = Object.values(err.response.data.errors).flat();
        toast.error(errors.join('. '));
      } else {
        toast.error(err.response?.data?.message || 'Failed to update roles');
      }
    }
  };

  const handleToggleRole = (role) => {
    if (userRoles.includes(role)) {
      setUserRoles(userRoles.filter((r) => r !== role));
    } else {
      setUserRoles([...userRoles, role]);
    }
  };

  const handleLockUser = async (user) => {
    if (!window.confirm(`Lock user "${user.email}"?`)) return;

    try {
      await usersAPI.lockUser(user.id);
      toast.success('User locked successfully');
      await fetchUsers();
    } catch (err) {
      console.error('Lock user error:', err.response?.data);
      if (err.response?.data?.errors) {
        const errors = Object.values(err.response.data.errors).flat();
        toast.error(errors.join('. '));
      } else {
        toast.error(err.response?.data?.message || 'Failed to lock user');
      }
    }
  };

  const handleUnlockUser = async (user) => {
    if (!window.confirm(`Unlock user "${user.email}"?`)) return;

    try {
      await usersAPI.unlockUser(user.id);
      toast.success('User unlocked successfully');
      await fetchUsers();
    } catch (err) {
      console.error('Unlock user error:', err.response?.data);
      if (err.response?.data?.errors) {
        const errors = Object.values(err.response.data.errors).flat();
        toast.error(errors.join('. '));
      } else {
        toast.error(err.response?.data?.message || 'Failed to unlock user');
      }
    }
  };

  const handleConfirmEmail = async (user) => {
    if (!window.confirm(`Confirm email for "${user.email}"?`)) return;

    try {
      await usersAPI.confirmEmail(user.id);
      toast.success('Email confirmed successfully');
      await fetchUsers();
    } catch (err) {
      toast.error('Failed to confirm email');
    }
  };

  const handleDeleteUser = async (user) => {
    if (!window.confirm(`Delete user "${user.email}"? This action cannot be undone.`)) return;

    try {
      await usersAPI.delete(user.id);
      toast.success('User deleted successfully');
      setCurrentPage(1);
      await fetchUsers();
      await fetchStatistics();
    } catch (err) {
      console.error('Delete user error:', err.response?.data);
      if (err.response?.data?.errors) {
        const errors = Object.values(err.response.data.errors).flat();
        toast.error(errors.join('. '));
      } else {
        toast.error(err.response?.data?.message || 'Failed to delete user');
      }
    }
  };

  const handleRestoreUser = async (user) => {
    if (!window.confirm(`Restore user "${user.email}"?`)) return;

    try {
      await usersAPI.restore(user.id);
      toast.success('User restored successfully');
      await fetchUsers();
      await fetchStatistics();
    } catch (err) {
      toast.error('Failed to restore user');
    }
  };

  const closeAllModals = () => {
    setShowCreateModal(false);
    setShowEditModal(false);
    setShowViewModal(false);
    setShowRolesModal(false);
    setSelectedUser(null);
    setUserRoles([]);
  };

  const handleSearch = (value) => {
    setSearchTerm(value);
    setCurrentPage(1);
  };

  const isUserLocked = (user) => user.lockoutEnd && new Date(user.lockoutEnd) > new Date();

  if (loading && users.length === 0) return <LoadingSpinner />;

  return (
    <div className="max-w-7xl mx-auto p-6">
      {/* Header */}
      <div className="flex justify-between items-center mb-6">
        <div>
          <h1 className="text-3xl font-bold text-gray-900">User Management</h1>
          <p className="text-gray-600 mt-1">Manage system users, roles, and permissions</p>
        </div>
        {canManage && (
          <button
            onClick={handleCreateNew}
            className="px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 transition-colors"
          >
            + New User
          </button>
        )}
      </div>

      {/* Statistics Cards */}
      {statistics && (
        <div className="grid grid-cols-1 md:grid-cols-4 gap-4 mb-6">
          <div className="bg-white rounded-lg shadow-md p-4 border-l-4 border-blue-600">
            <p className="text-sm text-gray-600">Total Users</p>
            <p className="text-3xl font-bold text-blue-600 mt-1">{statistics.totalUsers}</p>
          </div>

          <div className="bg-white rounded-lg shadow-md p-4 border-l-4 border-green-600">
            <p className="text-sm text-gray-600">Active Users</p>
            <p className="text-3xl font-bold text-green-600 mt-1">{statistics.activeUsers}</p>
          </div>

          <div className="bg-white rounded-lg shadow-md p-4 border-l-4 border-red-600">
            <p className="text-sm text-gray-600">Inactive Users</p>
            <p className="text-3xl font-bold text-red-600 mt-1">{statistics.inactiveUsers}</p>
          </div>

          <div className="bg-white rounded-lg shadow-md p-4 border-l-4 border-purple-600">
            <p className="text-sm text-gray-600">Active Rate</p>
            <p className="text-3xl font-bold text-purple-600 mt-1">
              {statistics.totalUsers > 0
                ? Math.round((statistics.activeUsers / statistics.totalUsers) * 100)
                : 0}
              %
            </p>
          </div>
        </div>
      )}

      {/* Search & Filter Bar */}
      <div className="bg-white rounded-lg shadow-md p-4 mb-6">
        <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">Search Users</label>
            <input
              type="text"
              placeholder="Search by email or username..."
              value={searchTerm}
              onChange={(e) => handleSearch(e.target.value)}
              className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
            />
          </div>

          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">Filter by Role</label>
            <select
              value={selectedRole}
              onChange={(e) => {
                setSelectedRole(e.target.value);
                setCurrentPage(1);
              }}
              className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
            >
              <option value="">All Roles</option>
              {availableRoles.map((role) => (
                <option key={role} value={role}>
                  {role}
                </option>
              ))}
            </select>
          </div>

          <div className="flex items-end">
            {(searchTerm || selectedRole) && (
              <button
                onClick={() => {
                  handleSearch('');
                  setSelectedRole('');
                }}
                className="w-full px-4 py-2 text-gray-700 bg-gray-200 rounded-lg hover:bg-gray-300 transition-colors"
              >
                Clear Filters
              </button>
            )}
          </div>
        </div>
      </div>

      {/* Users Table */}
      <div className="bg-white rounded-lg shadow-md overflow-hidden">
        <div className="overflow-x-auto">
          <table className="w-full">
            <thead className="bg-gray-50 border-b border-gray-200">
              <tr>
                <th className="px-6 py-3 text-left text-sm font-semibold text-gray-900">Email</th>
                <th className="px-6 py-3 text-left text-sm font-semibold text-gray-900">Username</th>
                <th className="px-6 py-3 text-left text-sm font-semibold text-gray-900">Roles</th>
                <th className="px-6 py-3 text-left text-sm font-semibold text-gray-900">Status</th>
                <th className="px-6 py-3 text-left text-sm font-semibold text-gray-900">Created</th>
                <th className="px-6 py-3 text-right text-sm font-semibold text-gray-900">Actions</th>
              </tr>
            </thead>
            <tbody className="divide-y divide-gray-200">
              {users.length > 0 ? (
                users.map((user) => (
                  <tr key={user.id} className="hover:bg-gray-50 transition-colors">
                    <td className="px-6 py-4 text-sm font-medium text-gray-900">{user.email}</td>
                    <td className="px-6 py-4 text-sm text-gray-600">{user.userName || '-'}</td>
                    <td className="px-6 py-4 text-sm">
                      <div className="flex flex-wrap gap-1">
                        {user.roles && user.roles.length > 0 ? (
                          user.roles.map((role) => (
                            <span key={role} className="px-2 py-1 bg-purple-100 text-purple-800 rounded text-xs font-medium">
                              {role}
                            </span>
                          ))
                        ) : (
                          <span className="text-gray-500 text-xs">No roles</span>
                        )}
                      </div>
                    </td>
                    <td className="px-6 py-4 text-sm">
                      <div className="flex flex-col gap-1">
                        <span
                          className={`px-2 py-1 rounded text-xs font-medium inline-block ${
                            user.isDeleted
                              ? 'bg-red-100 text-red-800'
                              : isUserLocked(user)
                              ? 'bg-yellow-100 text-yellow-800'
                              : 'bg-green-100 text-green-800'
                          }`}
                        >
                          {user.isDeleted ? '🗑️ Deleted' : isUserLocked(user) ? '🔒 Locked' : '✅ Active'}
                        </span>
                        {!user.emailConfirmed && (
                          <span className="px-2 py-1 bg-orange-100 text-orange-800 rounded text-xs font-medium inline-block">
                            📧 Unconfirmed
                          </span>
                        )}
                      </div>
                    </td>
                    <td className="px-6 py-4 text-sm text-gray-600">
                      {user.createdAt ? new Date(user.createdAt).toLocaleDateString() : '-'}
                    </td>
                    <td className="px-6 py-4 text-sm text-right">
                      <div className="flex justify-end gap-2">
                        <button
                          onClick={() => handleViewUser(user)}
                          className="text-blue-600 hover:text-blue-900 transition-colors"
                          title="View Details"
                        >
                          👁️
                        </button>
                        {canManage && !user.isDeleted && (
                          <>
                            <button
                              onClick={() => handleEditUser(user)}
                              className="text-yellow-600 hover:text-yellow-900 transition-colors"
                              title="Edit"
                            >
                              ✏️
                            </button>
                            <button
                              onClick={() => handleManageRoles(user)}
                              className="text-purple-600 hover:text-purple-900 transition-colors"
                              title="Manage Roles"
                            >
                              👥
                            </button>
                            {!isUserLocked(user) ? (
                              <button
                                onClick={() => handleLockUser(user)}
                                className="text-orange-600 hover:text-orange-900 transition-colors"
                                title="Lock User"
                              >
                                🔒
                              </button>
                            ) : (
                              <button
                                onClick={() => handleUnlockUser(user)}
                                className="text-green-600 hover:text-green-900 transition-colors"
                                title="Unlock User"
                              >
                                🔓
                              </button>
                            )}
                            {!user.emailConfirmed && (
                              <button
                                onClick={() => handleConfirmEmail(user)}
                                className="text-blue-600 hover:text-blue-900 transition-colors"
                                title="Confirm Email"
                              >
                                📧
                              </button>
                            )}
                            <button
                              onClick={() => handleDeleteUser(user)}
                              className="text-red-600 hover:text-red-900 transition-colors"
                              title="Delete"
                            >
                              🗑️
                            </button>
                          </>
                        )}
                        {canManage && user.isDeleted && (
                          <button
                            onClick={() => handleRestoreUser(user)}
                            className="text-green-600 hover:text-green-900 transition-colors"
                            title="Restore"
                          >
                            ↩️
                          </button>
                        )}
                      </div>
                    </td>
                  </tr>
                ))
              ) : (
                <tr>
                  <td colSpan="6" className="px-6 py-8 text-center text-gray-500">
                    {loading ? 'Loading users...' : 'No users found'}
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
              {Math.min(currentPage * itemsPerPage, totalCount)} of {totalCount} users
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

      {/* Create User Modal */}
      {showCreateModal && (
        <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50 p-4">
          <div className="bg-white rounded-lg max-w-lg w-full">
            <div className="sticky top-0 bg-white border-b border-gray-200 px-6 py-4">
              <h2 className="text-2xl font-bold text-gray-900">Create New User</h2>
            </div>

            <form onSubmit={handleSubmitCreate} className="p-6 space-y-4">
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">Email Address *</label>
                <input
                  type="email"
                  value={formData.email}
                  onChange={(e) => setFormData({ ...formData, email: e.target.value })}
                  placeholder="user@example.com"
                  className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                  required
                  autoFocus
                />
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">Password *</label>
                <input
                  type="password"
                  value={formData.password}
                  onChange={(e) => setFormData({ ...formData, password: e.target.value })}
                  placeholder="Enter secure password"
                  className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                  required
                />
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">Username (Optional)</label>
                <input
                  type="text"
                  value={formData.userName}
                  onChange={(e) => setFormData({ ...formData, userName: e.target.value })}
                  placeholder="User display name"
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
                  Create User
                </button>
              </div>
            </form>
          </div>
        </div>
      )}

      {/* Edit User Modal */}
      {showEditModal && selectedUser && (
        <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50 p-4">
          <div className="bg-white rounded-lg max-w-lg w-full">
            <div className="sticky top-0 bg-white border-b border-gray-200 px-6 py-4">
              <h2 className="text-2xl font-bold text-gray-900">Edit User</h2>
            </div>

            <form onSubmit={handleSubmitEdit} className="p-6 space-y-4">
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">Email Address *</label>
                <input
                  type="email"
                  value={formData.email}
                  onChange={(e) => setFormData({ ...formData, email: e.target.value })}
                  className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                  required
                />
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">Username (Optional)</label>
                <input
                  type="text"
                  value={formData.userName}
                  onChange={(e) => setFormData({ ...formData, userName: e.target.value })}
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
                  Update User
                </button>
              </div>
            </form>
          </div>
        </div>
      )}

      {/* View User Modal */}
      {showViewModal && selectedUser && (
        <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50 p-4">
          <div className="bg-white rounded-lg max-w-2xl w-full">
            <div className="sticky top-0 bg-white border-b border-gray-200 px-6 py-4 flex justify-between items-center">
              <h2 className="text-2xl font-bold text-gray-900">{selectedUser.email}</h2>
              <button onClick={closeAllModals} className="text-gray-500 hover:text-gray-700">
                ✕
              </button>
            </div>

            <div className="p-6 space-y-6">
              <div className="grid grid-cols-2 gap-4">
                <div>
                  <p className="text-sm text-gray-600">Email</p>
                  <p className="text-lg font-medium text-gray-900">{selectedUser.email}</p>
                </div>
                <div>
                  <p className="text-sm text-gray-600">Username</p>
                  <p className="text-lg font-medium text-gray-900">{selectedUser.userName || '-'}</p>
                </div>
                <div>
                  <p className="text-sm text-gray-600">Email Confirmed</p>
                  <p className="text-lg font-medium">{selectedUser.emailConfirmed ? '✅ Yes' : '❌ No'}</p>
                </div>
                <div>
                  <p className="text-sm text-gray-600">Account Status</p>
                  <p className="text-lg font-medium">
                    {selectedUser.isDeleted ? '🗑️ Deleted' : '✅ Active'}
                  </p>
                </div>
                <div>
                  <p className="text-sm text-gray-600">Created</p>
                  <p className="text-lg font-medium">
                    {selectedUser.createdAt ? new Date(selectedUser.createdAt).toLocaleDateString() : '-'}
                  </p>
                </div>
                <div>
                  <p className="text-sm text-gray-600">Updated</p>
                  <p className="text-lg font-medium">
                    {selectedUser.updatedAt ? new Date(selectedUser.updatedAt).toLocaleDateString() : '-'}
                  </p>
                </div>
              </div>

              <div>
                <p className="text-sm font-medium text-gray-700 mb-2">Roles</p>
                <div className="flex flex-wrap gap-2">
                  {selectedUser.roles && selectedUser.roles.length > 0 ? (
                    selectedUser.roles.map((role) => (
                      <span key={role} className="px-3 py-1 bg-purple-100 text-purple-800 rounded-full text-sm font-medium">
                        {role}
                      </span>
                    ))
                  ) : (
                    <span className="text-gray-500">No roles assigned</span>
                  )}
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

      {/* Manage Roles Modal */}
      {showRolesModal && selectedUser && (
        <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50 p-4">
          <div className="bg-white rounded-lg max-w-lg w-full">
            <div className="sticky top-0 bg-white border-b border-gray-200 px-6 py-4">
              <h2 className="text-2xl font-bold text-gray-900">Manage Roles for {selectedUser.email}</h2>
            </div>

            <div className="p-6">
              <div className="space-y-3">
                {availableRoles.map((role) => (
                  <label key={role} className="flex items-center p-3 border border-gray-200 rounded-lg hover:bg-gray-50 cursor-pointer">
                    <input
                      type="checkbox"
                      checked={userRoles.includes(role)}
                      onChange={() => handleToggleRole(role)}
                      className="w-4 h-4 text-blue-600 rounded"
                    />
                    <span className="ml-3 font-medium text-gray-900">{role}</span>
                  </label>
                ))}
              </div>

              <div className="flex justify-end gap-3 pt-6 border-t border-gray-200 mt-6">
                <button
                  type="button"
                  onClick={closeAllModals}
                  className="px-4 py-2 text-gray-700 bg-gray-200 rounded-lg hover:bg-gray-300 transition-colors"
                >
                  Cancel
                </button>
                <button
                  onClick={handleSaveRoles}
                  className="px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 transition-colors"
                >
                  Save Roles
                </button>
              </div>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}
