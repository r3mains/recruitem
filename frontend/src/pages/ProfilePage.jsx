import { useState, useEffect } from 'react';
import { profileAPI } from '../services/api';
import { useAuth } from '../contexts/AuthContext';
import toast from 'react-hot-toast';
import LoadingSpinner from '../components/LoadingSpinner';

export default function ProfilePage() {
  const [profile, setProfile] = useState(null);
  const [loading, setLoading] = useState(true);
  const [isEditing, setIsEditing] = useState(false);
  const [isSaving, setIsSaving] = useState(false);

  const [formData, setFormData] = useState({
    fullName: '',
    contactNumber: '',
    branchAddressId: '',
    joiningDate: '',
    offerLetterUrl: ''
  });

  const { user } = useAuth();
  const isCandidate = user?.role === 'Candidate';
  const isEmployee = user?.role === 'Admin' || user?.role === 'HR' || user?.role === 'Recruiter' || user?.role === 'Interviewer' || user?.role === 'Reviewer';

  useEffect(() => {
    fetchProfile();
  }, []);

  const fetchProfile = async () => {
    try {
      setLoading(true);
      let profileData;

      if (isCandidate) {
        profileData = await profileAPI.getCandidateProfile();
      } else if (isEmployee) {
        profileData = await profileAPI.getEmployeeProfile();
      } else {
        toast.error('Unable to determine your profile type');
        return;
      }

      if (profileData) {
        setProfile(profileData);
        if (isCandidate) {
          setFormData({
            fullName: profileData.fullName || '',
            contactNumber: profileData.contactNumber || '',
            branchAddressId: '',
            joiningDate: '',
            offerLetterUrl: ''
          });
        } else {
          setFormData({
            fullName: profileData.fullName || '',
            contactNumber: '',
            branchAddressId: profileData.branchAddressId || '',
            joiningDate: profileData.joiningDate || '',
            offerLetterUrl: profileData.offerLetterUrl || ''
          });
        }
      }
    } catch (err) {
      if (err.response?.status === 404) {
        toast.info('Profile not found. Please create one first.');
      } else {
        toast.error('Failed to load profile');
      }
      console.error(err);
    } finally {
      setLoading(false);
    }
  };

  const handleInputChange = (e) => {
    const { name, value } = e.target;
    setFormData({
      ...formData,
      [name]: value || ''
    });
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setIsSaving(true);

    try {
      let updateData;

      if (isCandidate) {
        updateData = {
          fullName: formData.fullName || null,
          contactNumber: formData.contactNumber || null,
          addressId: formData.branchAddressId || null
        };
        await profileAPI.updateCandidateProfile(updateData);
      } else {
        updateData = {
          fullName: formData.fullName || null,
          branchAddressId: formData.branchAddressId || null,
          joiningDate: formData.joiningDate || null,
          offerLetterUrl: formData.offerLetterUrl || null
        };
        await profileAPI.updateEmployeeProfile(updateData);
      }

      toast.success('Profile updated successfully');
      setIsEditing(false);
      await fetchProfile();
    } catch (err) {
      toast.error(err.response?.data?.message || 'Failed to update profile');
    } finally {
      setIsSaving(false);
    }
  };

  if (loading) return <LoadingSpinner />;

  return (
    <div className="max-w-4xl mx-auto p-6">
      {/* Header */}
      <div className="flex justify-between items-center mb-8">
        <div>
          <h1 className="text-3xl font-bold text-gray-900">My Profile</h1>
          <p className="text-gray-600 mt-1">View and update your profile information</p>
        </div>
        {!isEditing && (
          <button
            onClick={() => setIsEditing(true)}
            className="px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 transition-colors"
          >
            ✏️ Edit Profile
          </button>
        )}
      </div>

      {/* User Info Card */}
      <div className="bg-white rounded-lg shadow-md p-6 mb-6 border-l-4 border-blue-600">
        <h2 className="text-xl font-semibold text-gray-900 mb-4">Account Information</h2>
        <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
          <div>
            <p className="text-sm text-gray-600">Email Address</p>
            <p className="text-lg font-medium text-gray-900 mt-1">{user?.email || 'N/A'}</p>
          </div>
          <div>
            <p className="text-sm text-gray-600">Username</p>
            <p className="text-lg font-medium text-gray-900 mt-1">{user?.userName || 'N/A'}</p>
          </div>
          <div>
            <p className="text-sm text-gray-600">Role</p>
            <span className="inline-block mt-1 px-3 py-1 bg-purple-100 text-purple-800 rounded-full text-sm font-medium">
              {user?.role || 'N/A'}
            </span>
          </div>
          <div>
            <p className="text-sm text-gray-600">User ID</p>
            <p className="text-sm font-mono text-gray-600 mt-1 break-all">{user?.sub || 'N/A'}</p>
          </div>
        </div>
      </div>

      {/* Profile Details Card */}
      {profile && (
        <div className="bg-white rounded-lg shadow-md p-6 border-l-4 border-green-600">
          <h2 className="text-xl font-semibold text-gray-900 mb-4">Profile Details</h2>

          {!isEditing ? (
            <div className="space-y-6">
              {/* Candidate View */}
              {isCandidate && (
                <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
                  <div>
                    <p className="text-sm text-gray-600">Full Name</p>
                    <p className="text-lg font-medium text-gray-900 mt-1">{profile.fullName || '-'}</p>
                  </div>
                  <div>
                    <p className="text-sm text-gray-600">Contact Number</p>
                    <p className="text-lg font-medium text-gray-900 mt-1">{profile.contactNumber || '-'}</p>
                  </div>
                  {profile.address && (
                    <div className="md:col-span-2">
                      <p className="text-sm text-gray-600">Address</p>
                      <p className="text-lg font-medium text-gray-900 mt-1">
                        {profile.address.city}, {profile.address.state} {profile.address.zipCode}
                      </p>
                    </div>
                  )}
                </div>
              )}

              {/* Employee View */}
              {isEmployee && (
                <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
                  <div>
                    <p className="text-sm text-gray-600">Full Name</p>
                    <p className="text-lg font-medium text-gray-900 mt-1">{profile.fullName || '-'}</p>
                  </div>
                  <div>
                    <p className="text-sm text-gray-600">Joining Date</p>
                    <p className="text-lg font-medium text-gray-900 mt-1">
                      {profile.joiningDate ? new Date(profile.joiningDate).toLocaleDateString() : '-'}
                    </p>
                  </div>
                  {profile.branchAddress && (
                    <div className="md:col-span-2">
                      <p className="text-sm text-gray-600">Branch Address</p>
                      <p className="text-lg font-medium text-gray-900 mt-1">
                        {profile.branchAddress.city}, {profile.branchAddress.state} {profile.branchAddress.zipCode}
                      </p>
                    </div>
                  )}
                  {profile.offerLetterUrl && (
                    <div className="md:col-span-2">
                      <p className="text-sm text-gray-600">Offer Letter</p>
                      <a
                        href={profile.offerLetterUrl}
                        target="_blank"
                        rel="noopener noreferrer"
                        className="text-blue-600 hover:text-blue-900 font-medium mt-1 inline-block"
                      >
                        📄 View Offer Letter
                      </a>
                    </div>
                  )}
                </div>
              )}

              {/* Metadata */}
              <div className="pt-4 border-t border-gray-200 mt-6">
                <div className="grid grid-cols-1 md:grid-cols-2 gap-4 text-sm text-gray-600">
                  <div>
                    <p>Created: {new Date(profile.createdAt).toLocaleDateString()}</p>
                  </div>
                  <div>
                    <p>Last Updated: {new Date(profile.updatedAt).toLocaleDateString()}</p>
                  </div>
                </div>
              </div>
            </div>
          ) : (
            /* Edit Form */
            <form onSubmit={handleSubmit} className="space-y-6">
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-2">Full Name</label>
                <input
                  type="text"
                  name="fullName"
                  value={formData.fullName}
                  onChange={handleInputChange}
                  placeholder="Enter your full name"
                  className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                />
              </div>

              {isCandidate && (
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-2">Contact Number</label>
                  <input
                    type="tel"
                    name="contactNumber"
                    value={formData.contactNumber}
                    onChange={handleInputChange}
                    placeholder="Enter your contact number"
                    className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                  />
                </div>
              )}

              {isEmployee && (
                <>
                  <div>
                    <label className="block text-sm font-medium text-gray-700 mb-2">Joining Date</label>
                    <input
                      type="date"
                      name="joiningDate"
                      value={formData.joiningDate}
                      onChange={handleInputChange}
                      className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                    />
                  </div>

                  <div>
                    <label className="block text-sm font-medium text-gray-700 mb-2">Offer Letter URL</label>
                    <input
                      type="url"
                      name="offerLetterUrl"
                      value={formData.offerLetterUrl}
                      onChange={handleInputChange}
                      placeholder="https://example.com/offer-letter.pdf"
                      className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                    />
                    <p className="mt-1 text-xs text-gray-500">Enter a URL link to your offer letter document</p>
                  </div>
                </>
              )}

              <div className="flex justify-end gap-3 pt-6 border-t border-gray-200">
                <button
                  type="button"
                  onClick={() => setIsEditing(false)}
                  className="px-4 py-2 text-gray-700 bg-gray-200 rounded-lg hover:bg-gray-300 transition-colors"
                  disabled={isSaving}
                >
                  Cancel
                </button>
                <button
                  type="submit"
                  className="px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 transition-colors disabled:opacity-50"
                  disabled={isSaving}
                >
                  {isSaving ? 'Saving...' : 'Save Changes'}
                </button>
              </div>
            </form>
          )}
        </div>
      )}

      {/* No Profile Message */}
      {!profile && (
        <div className="bg-yellow-50 border border-yellow-200 rounded-lg p-6 text-center">
          <p className="text-yellow-800">
            ⚠️ Your profile has not been set up yet. Please contact an administrator to create your profile.
          </p>
        </div>
      )}
    </div>
  );
}
