import React, { useState, useEffect } from "react";
import { useApi } from "../contexts/ApiContext";
import { useAuth } from "../contexts/AuthContext";

const CandidatesPage = () => {
  const [candidates, setCandidates] = useState([]);
  const [loading, setLoading] = useState(true);
  const [search, setSearch] = useState("");
  const [skillsFilter, setSkillsFilter] = useState("");
  const [page, setPage] = useState(1);
  const [totalPages, setTotalPages] = useState(1);
  const [totalCount, setTotalCount] = useState(0);
  const [selectedCandidate, setSelectedCandidate] = useState(null);
  const [showProfile, setShowProfile] = useState(false);
  const api = useApi();
  const { hasRole } = useAuth();

  const limit = 10;

  useEffect(() => {
    loadCandidates();
  }, [page]);

  const loadCandidates = async () => {
    try {
      setLoading(true);
      const response = await api.candidates.search(
        search,
        skillsFilter,
        page,
        limit
      );
      setCandidates(response.candidates);
      setTotalPages(response.totalPages);
      setTotalCount(response.totalCount);
    } catch (error) {
      console.error("Error loading candidates:", error);
    } finally {
      setLoading(false);
    }
  };

  const handleSearch = () => {
    setPage(1);
    loadCandidates();
  };

  const clearFilters = () => {
    setSearch("");
    setSkillsFilter("");
    setPage(1);
    setTimeout(loadCandidates, 0);
  };

  const viewProfile = async (candidateId) => {
    try {
      const candidate = await api.candidates.getById(candidateId);
      setSelectedCandidate(candidate);
      setShowProfile(true);
    } catch (error) {
      console.error("Error loading candidate profile:", error);
    }
  };

  const closeProfile = () => {
    setShowProfile(false);
    setSelectedCandidate(null);
  };

  if (!hasRole(["Admin", "Recruiter", "HR"])) {
    return (
      <div className="p-6">
        <div className="text-center">
          <h1 className="text-2xl font-bold text-red-600">Access Denied</h1>
          <p className="text-gray-600">
            You don't have permission to view candidates.
          </p>
        </div>
      </div>
    );
  }

  return (
    <div className="p-6">
      <div className="mb-6">
        <h1 className="text-2xl font-bold mb-4">Candidates</h1>

        <div className="bg-white p-4 rounded-lg shadow mb-6">
          <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
            <div>
              <label className="block text-sm font-medium mb-2">Search</label>
              <input
                type="text"
                value={search}
                onChange={(e) => setSearch(e.target.value)}
                placeholder="Name, email, or phone..."
                className="w-full border rounded px-3 py-2"
              />
            </div>
            <div>
              <label className="block text-sm font-medium mb-2">Skills</label>
              <input
                type="text"
                value={skillsFilter}
                onChange={(e) => setSkillsFilter(e.target.value)}
                placeholder="Java, React, etc..."
                className="w-full border rounded px-3 py-2"
              />
            </div>
            <div className="flex items-end gap-2">
              <button
                onClick={handleSearch}
                className="bg-blue-600 text-white px-4 py-2 rounded hover:bg-blue-700"
              >
                Search
              </button>
              <button
                onClick={clearFilters}
                className="bg-gray-500 text-white px-4 py-2 rounded hover:bg-gray-600"
              >
                Clear
              </button>
            </div>
          </div>
        </div>
      </div>

      {loading ? (
        <div className="text-center py-8">Loading candidates...</div>
      ) : (
        <>
          <div className="mb-4 text-sm text-gray-600">
            Found {totalCount} candidates
          </div>

          <div className="grid gap-4">
            {candidates.map((candidate) => (
              <div
                key={candidate.id}
                className="bg-white p-4 rounded-lg shadow border"
              >
                <div className="flex justify-between items-start">
                  <div className="flex-1">
                    <h3 className="text-lg font-semibold">
                      {candidate.fullName || "No name provided"}
                    </h3>
                    <p className="text-gray-600">{candidate.email}</p>
                    {candidate.contactNumber && (
                      <p className="text-gray-600">{candidate.contactNumber}</p>
                    )}
                    {candidate.addressDetails && (
                      <p className="text-gray-600 text-sm">
                        {candidate.addressDetails}
                      </p>
                    )}
                  </div>

                  <div className="text-right">
                    <div className="text-sm text-gray-500 mb-2">
                      {candidate.skills?.length || 0} skills |{" "}
                      {candidate.totalApplications} applications
                    </div>
                    <button
                      onClick={() => viewProfile(candidate.id)}
                      className="bg-blue-600 text-white px-3 py-1 rounded text-sm hover:bg-blue-700"
                    >
                      View Profile
                    </button>
                  </div>
                </div>

                {candidate.skills && candidate.skills.length > 0 && (
                  <div className="mt-3">
                    <div className="flex flex-wrap gap-2">
                      {candidate.skills.slice(0, 5).map((skill) => (
                        <span
                          key={skill.skillId}
                          className="bg-gray-100 text-gray-700 px-2 py-1 rounded text-xs"
                        >
                          {skill.skillName} ({skill.yearsOfExperience}y)
                        </span>
                      ))}
                      {candidate.skills.length > 5 && (
                        <span className="bg-gray-100 text-gray-700 px-2 py-1 rounded text-xs">
                          +{candidate.skills.length - 5} more
                        </span>
                      )}
                    </div>
                  </div>
                )}
              </div>
            ))}
          </div>

          {totalPages > 1 && (
            <div className="flex justify-center mt-6 gap-2">
              <button
                onClick={() => setPage(Math.max(1, page - 1))}
                disabled={page === 1}
                className="px-3 py-2 border rounded disabled:opacity-50"
              >
                Previous
              </button>

              <span className="px-3 py-2">
                Page {page} of {totalPages}
              </span>

              <button
                onClick={() => setPage(Math.min(totalPages, page + 1))}
                disabled={page === totalPages}
                className="px-3 py-2 border rounded disabled:opacity-50"
              >
                Next
              </button>
            </div>
          )}
        </>
      )}

      {showProfile && selectedCandidate && (
        <CandidateProfileModal
          candidate={selectedCandidate}
          onClose={closeProfile}
          api={api}
        />
      )}
    </div>
  );
};

const CandidateProfileModal = ({ candidate, onClose, api }) => {
  const [skills, setSkills] = useState([]);
  const [loadingSkills, setLoadingSkills] = useState(true);

  useEffect(() => {
    loadSkills();
  }, [candidate.id]);

  const loadSkills = async () => {
    try {
      setLoadingSkills(true);
      const skillsData = await api.candidates.getSkills(candidate.id);
      setSkills(skillsData);
    } catch (error) {
      console.error("Error loading skills:", error);
      setSkills([]);
    } finally {
      setLoadingSkills(false);
    }
  };

  return (
    <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
      <div className="bg-white p-6 rounded-lg w-11/12 md:w-2/3 lg:w-1/2 max-h-screen overflow-y-auto">
        <div className="flex justify-between items-center mb-4">
          <h2 className="text-xl font-bold">Candidate Profile</h2>
          <button
            onClick={onClose}
            className="text-gray-500 hover:text-gray-700 text-xl"
          >
            ×
          </button>
        </div>

        <div className="space-y-4">
          <div>
            <h3 className="font-semibold text-lg">
              {candidate.fullName || "No name provided"}
            </h3>
            <p className="text-gray-600">{candidate.email}</p>
          </div>

          {candidate.contactNumber && (
            <div>
              <label className="block text-sm font-medium text-gray-700">
                Phone
              </label>
              <p>{candidate.contactNumber}</p>
            </div>
          )}

          {candidate.addressDetails && (
            <div>
              <label className="block text-sm font-medium text-gray-700">
                Address
              </label>
              <p>{candidate.addressDetails}</p>
            </div>
          )}

          {candidate.resumeUrl && (
            <div>
              <label className="block text-sm font-medium text-gray-700">
                Resume
              </label>
              <a
                href={candidate.resumeUrl}
                target="_blank"
                rel="noopener noreferrer"
                className="text-blue-600 hover:text-blue-800"
              >
                View Resume
              </a>
            </div>
          )}

          <div>
            <label className="block text-sm font-medium text-gray-700 mb-2">
              Skills
            </label>
            {loadingSkills ? (
              <p>Loading skills...</p>
            ) : skills.length > 0 ? (
              <div className="grid gap-2">
                {skills.map((skill) => (
                  <div
                    key={skill.skillId}
                    className="flex justify-between items-center p-2 bg-gray-50 rounded"
                  >
                    <span>{skill.skillName}</span>
                    <span className="text-sm text-gray-600">
                      {skill.yearsOfExperience} years
                    </span>
                  </div>
                ))}
              </div>
            ) : (
              <p className="text-gray-500">No skills listed</p>
            )}
          </div>

          <div>
            <label className="block text-sm font-medium text-gray-700">
              Statistics
            </label>
            <p>Total Applications: {candidate.totalApplications}</p>
          </div>
        </div>
      </div>
    </div>
  );
};

export default CandidatesPage;
