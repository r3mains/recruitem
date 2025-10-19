import React, { useState, useEffect } from "react";
import { useApi } from "../contexts/ApiContext";
import { useAuth } from "../contexts/AuthContext";

const CandidateProfilePage = () => {
  const [profile, setProfile] = useState(null);
  const [skills, setSkills] = useState([]);
  const [allSkills, setAllSkills] = useState([]);
  const [loading, setLoading] = useState(true);
  const [editing, setEditing] = useState(false);
  const [showAddSkill, setShowAddSkill] = useState(false);
  const api = useApi();
  const { user } = useAuth();

  const [formData, setFormData] = useState({
    fullName: "",
    contactNumber: "",
    resumeUrl: "",
  });

  const [newSkill, setNewSkill] = useState({
    skillId: "",
    yearsOfExperience: 0,
  });

  useEffect(() => {
    loadProfile();
    loadAllSkills();
  }, []);

  const loadProfile = async () => {
    try {
      setLoading(true);
      const profileData = await api.candidates.getProfile();
      setProfile(profileData);
      setFormData({
        fullName: profileData.fullName || "",
        contactNumber: profileData.contactNumber || "",
        resumeUrl: profileData.resumeUrl || "",
      });

      if (profileData.id) {
        const skillsData = await api.candidates.getSkills(profileData.id);
        setSkills(skillsData);
      }
    } catch (error) {
      console.error("Error loading profile:", error);
    } finally {
      setLoading(false);
    }
  };

  const loadAllSkills = async () => {
    try {
      const skillsData = await api.skills.getAll();
      setAllSkills(skillsData);
    } catch (error) {
      console.error("Error loading skills:", error);
    }
  };

  const handleSaveProfile = async () => {
    try {
      if (profile) {
        await api.candidates.update(profile.id, formData);
      } else {
        await api.candidates.create(formData);
      }
      setEditing(false);
      loadProfile();
    } catch (error) {
      console.error("Error saving profile:", error);
      alert("Failed to save profile");
    }
  };

  const handleAddSkill = async () => {
    try {
      if (!profile?.id) {
        alert("Please create your profile first");
        return;
      }

      await api.candidates.addSkill(profile.id, newSkill);
      setShowAddSkill(false);
      setNewSkill({ skillId: "", yearsOfExperience: 0 });
      loadProfile();
    } catch (error) {
      console.error("Error adding skill:", error);
      alert("Failed to add skill");
    }
  };

  const handleRemoveSkill = async (skillId) => {
    try {
      if (!profile?.id) return;

      await api.candidates.removeSkill(profile.id, skillId);
      loadProfile();
    } catch (error) {
      console.error("Error removing skill:", error);
      alert("Failed to remove skill");
    }
  };

  const handleUpdateSkill = async (skillId, yearsOfExperience) => {
    try {
      if (!profile?.id) return;

      await api.candidates.updateSkill(profile.id, skillId, {
        yearsOfExperience,
      });
      loadProfile();
    } catch (error) {
      console.error("Error updating skill:", error);
      alert("Failed to update skill");
    }
  };

  if (loading) {
    return <div className="p-6">Loading profile...</div>;
  }

  return (
    <div className="p-6 max-w-4xl mx-auto">
      <div className="mb-6">
        <h1 className="text-2xl font-bold">My Profile</h1>
        <p className="text-gray-600">
          Manage your candidate profile and skills
        </p>
      </div>

      <div className="grid gap-6">
        <div className="bg-white p-6 rounded-lg shadow">
          <div className="flex justify-between items-center mb-4">
            <h2 className="text-lg font-semibold">Basic Information</h2>
            {!editing && (
              <button
                onClick={() => setEditing(true)}
                className="bg-blue-600 text-white px-4 py-2 rounded hover:bg-blue-700"
              >
                Edit Profile
              </button>
            )}
          </div>

          {editing ? (
            <div className="space-y-4">
              <div>
                <label className="block text-sm font-medium mb-2">
                  Full Name
                </label>
                <input
                  type="text"
                  value={formData.fullName}
                  onChange={(e) =>
                    setFormData({ ...formData, fullName: e.target.value })
                  }
                  className="w-full border rounded px-3 py-2"
                />
              </div>

              <div>
                <label className="block text-sm font-medium mb-2">
                  Contact Number
                </label>
                <input
                  type="text"
                  value={formData.contactNumber}
                  onChange={(e) =>
                    setFormData({ ...formData, contactNumber: e.target.value })
                  }
                  className="w-full border rounded px-3 py-2"
                />
              </div>

              <div>
                <label className="block text-sm font-medium mb-2">
                  Resume URL
                </label>
                <input
                  type="url"
                  value={formData.resumeUrl}
                  onChange={(e) =>
                    setFormData({ ...formData, resumeUrl: e.target.value })
                  }
                  className="w-full border rounded px-3 py-2"
                  placeholder="https://..."
                />
              </div>

              <div className="flex gap-2">
                <button
                  onClick={handleSaveProfile}
                  className="bg-green-600 text-white px-4 py-2 rounded hover:bg-green-700"
                >
                  Save
                </button>
                <button
                  onClick={() => setEditing(false)}
                  className="bg-gray-500 text-white px-4 py-2 rounded hover:bg-gray-600"
                >
                  Cancel
                </button>
              </div>
            </div>
          ) : (
            <div className="space-y-3">
              <div>
                <label className="block text-sm font-medium text-gray-700">
                  Email
                </label>
                <p>{user?.email}</p>
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700">
                  Full Name
                </label>
                <p>{profile?.fullName || "Not provided"}</p>
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700">
                  Contact Number
                </label>
                <p>{profile?.contactNumber || "Not provided"}</p>
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700">
                  Resume
                </label>
                {profile?.resumeUrl ? (
                  <a
                    href={profile.resumeUrl}
                    target="_blank"
                    rel="noopener noreferrer"
                    className="text-blue-600 hover:text-blue-800"
                  >
                    View Resume
                  </a>
                ) : (
                  <p>Not provided</p>
                )}
              </div>
            </div>
          )}
        </div>

        <div className="bg-white p-6 rounded-lg shadow">
          <div className="flex justify-between items-center mb-4">
            <h2 className="text-lg font-semibold">Skills</h2>
            {profile?.id && (
              <button
                onClick={() => setShowAddSkill(true)}
                className="bg-green-600 text-white px-4 py-2 rounded hover:bg-green-700"
              >
                Add Skill
              </button>
            )}
          </div>

          {skills.length > 0 ? (
            <div className="grid gap-3">
              {skills.map((skill) => (
                <SkillItem
                  key={skill.skillId}
                  skill={skill}
                  onUpdate={handleUpdateSkill}
                  onRemove={handleRemoveSkill}
                />
              ))}
            </div>
          ) : (
            <p className="text-gray-500">No skills added yet</p>
          )}
        </div>

        <div className="bg-white p-6 rounded-lg shadow">
          <h2 className="text-lg font-semibold mb-4">Application Summary</h2>
          <p>Total Applications: {profile?.totalApplications || 0}</p>
        </div>
      </div>

      {showAddSkill && (
        <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
          <div className="bg-white p-6 rounded-lg w-96">
            <h3 className="text-lg font-bold mb-4">Add Skill</h3>

            <div className="space-y-4">
              <div>
                <label className="block text-sm font-medium mb-2">Skill</label>
                <select
                  value={newSkill.skillId}
                  onChange={(e) =>
                    setNewSkill({ ...newSkill, skillId: e.target.value })
                  }
                  className="w-full border rounded px-3 py-2"
                >
                  <option value="">Select a skill</option>
                  {allSkills
                    .filter(
                      (skill) => !skills.some((s) => s.skillId === skill.id)
                    )
                    .map((skill) => (
                      <option key={skill.id} value={skill.id}>
                        {skill.name}
                      </option>
                    ))}
                </select>
              </div>

              <div>
                <label className="block text-sm font-medium mb-2">
                  Years of Experience
                </label>
                <input
                  type="number"
                  min="0"
                  max="50"
                  value={newSkill.yearsOfExperience}
                  onChange={(e) =>
                    setNewSkill({
                      ...newSkill,
                      yearsOfExperience: parseInt(e.target.value),
                    })
                  }
                  className="w-full border rounded px-3 py-2"
                />
              </div>
            </div>

            <div className="flex justify-end gap-2 mt-6">
              <button
                onClick={() => setShowAddSkill(false)}
                className="bg-gray-500 text-white px-4 py-2 rounded hover:bg-gray-600"
              >
                Cancel
              </button>
              <button
                onClick={handleAddSkill}
                disabled={!newSkill.skillId}
                className="bg-blue-600 text-white px-4 py-2 rounded hover:bg-blue-700 disabled:opacity-50"
              >
                Add
              </button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
};

const SkillItem = ({ skill, onUpdate, onRemove }) => {
  const [editing, setEditing] = useState(false);
  const [years, setYears] = useState(skill.yearsOfExperience);

  const handleSave = () => {
    onUpdate(skill.skillId, years);
    setEditing(false);
  };

  const handleCancel = () => {
    setYears(skill.yearsOfExperience);
    setEditing(false);
  };

  return (
    <div className="flex justify-between items-center p-3 bg-gray-50 rounded">
      <div>
        <span className="font-medium">{skill.skillName}</span>
        {editing ? (
          <div className="flex items-center gap-2 mt-1">
            <input
              type="number"
              min="0"
              max="50"
              value={years}
              onChange={(e) => setYears(parseInt(e.target.value))}
              className="w-20 border rounded px-2 py-1 text-sm"
            />
            <span className="text-sm text-gray-600">years</span>
          </div>
        ) : (
          <div className="text-sm text-gray-600">
            {skill.yearsOfExperience} years of experience
          </div>
        )}
      </div>

      <div className="flex gap-2">
        {editing ? (
          <>
            <button
              onClick={handleSave}
              className="text-green-600 hover:text-green-800 text-sm"
            >
              Save
            </button>
            <button
              onClick={handleCancel}
              className="text-gray-600 hover:text-gray-800 text-sm"
            >
              Cancel
            </button>
          </>
        ) : (
          <>
            <button
              onClick={() => setEditing(true)}
              className="text-blue-600 hover:text-blue-800 text-sm"
            >
              Edit
            </button>
            <button
              onClick={() => onRemove(skill.skillId)}
              className="text-red-600 hover:text-red-800 text-sm"
            >
              Remove
            </button>
          </>
        )}
      </div>
    </div>
  );
};

export default CandidateProfilePage;
