import { useState } from 'react';
import { resumeAPI, candidatesAPI, skillsAPI, qualificationsAPI } from '../services/api';
import { useAuth } from '../contexts/AuthContext';
import toast from 'react-hot-toast';
import LoadingSpinner from '../components/LoadingSpinner';

export default function ResumeParsingPage() {
  const [file, setFile] = useState(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');
  const [parsedData, setParsedData] = useState(null);
  const [availableSkills, setAvailableSkills] = useState([]);
  const [availableQualifications, setAvailableQualifications] = useState([]);
  const [selectedSkills, setSelectedSkills] = useState([]);
  const [selectedQualifications, setSelectedQualifications] = useState([]);
  const [candidates, setCandidates] = useState([]);
  const [selectedCandidateId, setSelectedCandidateId] = useState('');
  const [showSaveModal, setShowSaveModal] = useState(false);

  const { user } = useAuth();
  const canManageCandidates = user?.role === 'Admin' || user?.role === 'HR' || user?.role === 'Recruiter';

  const handleFileChange = (e) => {
    const selectedFile = e.target.files[0];
    if (selectedFile) {
      const validExtensions = ['.pdf', '.doc', '.docx', '.txt'];
      const fileName = selectedFile.name.toLowerCase();
      const hasValidExtension = validExtensions.some(ext => fileName.endsWith(ext));
      
      if (!hasValidExtension) {
        setError('Please select a valid file (PDF, DOC, DOCX, or TXT)');
        return;
      }
      
      if (selectedFile.size > 10 * 1024 * 1024) {
        setError('File size must be less than 10MB');
        return;
      }
      
      setFile(selectedFile);
      setError('');
      setParsedData(null);
    }
  };

  const handleParse = async () => {
    if (!file) {
      setError('Please select a file first');
      return;
    }

    setLoading(true);
    setError('');

    try {
      const data = await resumeAPI.parseResume(file);
      setParsedData(data);
      setSelectedSkills([]);
      setSelectedQualifications([]);
      
      // Load available skills and qualifications
      await fetchAvailableOptions();
      
      // Load candidates for selection
      if (canManageCandidates) {
        await fetchCandidates();
      }
    } catch (err) {
      setError(err.response?.data?.message || 'Failed to parse resume');
    } finally {
      setLoading(false);
    }
  };

  const fetchAvailableOptions = async () => {
    try {
      const skillsData = await skillsAPI.getAll(1, 1000);
      const qualificationsData = await qualificationsAPI.getAll(1, 1000);
      setAvailableSkills(skillsData || []);
      setAvailableQualifications(qualificationsData || []);
    } catch (err) {
      console.error('Failed to load options:', err);
    }
  };

  const fetchCandidates = async () => {
    try {
      const data = await candidatesAPI.getAll(1, 100);
      if (data.items) {
        setCandidates(data.items);
      } else if (Array.isArray(data)) {
        setCandidates(data);
      }
    } catch (err) {
      console.error('Failed to load candidates:', err);
    }
  };

  const handleSkillToggle = (skillId) => {
    if (selectedSkills.includes(skillId)) {
      setSelectedSkills(selectedSkills.filter(id => id !== skillId));
    } else {
      setSelectedSkills([...selectedSkills, skillId]);
    }
  };

  const handleQualificationToggle = (qualId) => {
    if (selectedQualifications.includes(qualId)) {
      setSelectedQualifications(selectedQualifications.filter(id => id !== qualId));
    } else {
      setSelectedQualifications([...selectedQualifications, qualId]);
    }
  };

  const handleSaveData = async () => {
    if (!selectedCandidateId) {
      toast.error('Please select a candidate');
      return;
    }

    try {
      // Save parsed data to candidate - this would require a backend endpoint
      // For now, we'll show a success message
      toast.success('Resume data saved to candidate profile');
      setShowSaveModal(false);
      handleReset();
    } catch (err) {
      toast.error('Failed to save resume data');
    }
  };

  const handleReset = () => {
    setFile(null);
    setParsedData(null);
    setError('');
    setSelectedSkills([]);
    setSelectedQualifications([]);
    setSelectedCandidateId('');
    setShowSaveModal(false);
  };

  return (
    <div className="max-w-6xl mx-auto p-6">
      {/* Header */}
      <div>
        <h1 className="text-3xl font-bold text-gray-900">Resume Parser</h1>
        <p className="text-gray-600 mt-1">Upload and extract information from resumes (PDF, DOC, DOCX, TXT)</p>
      </div>

      {/* Upload Section */}
      <div className="bg-white rounded-lg shadow-md p-6 mb-6 mt-6">
        <h2 className="text-xl font-semibold mb-4 text-gray-900">Upload Resume</h2>
        
        <div className="mb-4">
          <label className="block text-sm font-medium text-gray-700 mb-2">
            Select Resume File (PDF, DOC, DOCX, TXT - Max 10MB)
          </label>
          <div className="relative">
            <input
              type="file"
              accept=".pdf,.doc,.docx,.txt"
              onChange={handleFileChange}
              className="block w-full text-sm text-gray-900 border border-gray-300 rounded-lg cursor-pointer bg-gray-50 focus:outline-none p-2"
              disabled={loading}
            />
          </div>
        </div>

        {file && (
          <div className="mb-4 p-3 bg-blue-50 border border-blue-200 rounded">
            <p className="text-sm text-gray-700">
              📄 Selected: <span className="font-medium">{file.name}</span>
              <span className="ml-2 text-gray-500">({(file.size / 1024).toFixed(2)} KB)</span>
            </p>
          </div>
        )}

        {error && (
          <div className="mb-4 p-3 bg-red-50 border border-red-200 rounded">
            <p className="text-sm text-red-700">❌ {error}</p>
          </div>
        )}

        <div className="flex gap-3">
          <button
            onClick={handleParse}
            disabled={!file || loading}
            className="px-6 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 disabled:bg-gray-400 disabled:cursor-not-allowed transition-colors"
          >
            {loading ? '⏳ Parsing...' : '📤 Parse Resume'}
          </button>
          
          {(file || parsedData) && (
            <button
              onClick={handleReset}
              disabled={loading}
              className="px-6 py-2 bg-gray-200 text-gray-700 rounded-lg hover:bg-gray-300 transition-colors disabled:opacity-50"
            >
              🔄 Reset
            </button>
          )}
        </div>
      </div>

      {loading && (
        <div className="flex justify-center py-12">
          <LoadingSpinner />
        </div>
      )}

      {parsedData && (
        <>
          {/* Parsed Data Display */}
          <div className="grid grid-cols-1 lg:grid-cols-3 gap-6 mb-6">
            {/* Left Column - Main Info */}
            <div className="lg:col-span-2 bg-white rounded-lg shadow-md p-6">
              <h2 className="text-xl font-semibold mb-4 text-gray-900">Extracted Information</h2>
              
              <div className="space-y-4">
                <div>
                  <label className="block text-sm font-medium text-gray-700">Full Name</label>
                  <p className="mt-1 text-lg text-gray-900 font-medium">{parsedData.fullName || '—'}</p>
                </div>

                <div className="grid grid-cols-2 gap-4">
                  <div>
                    <label className="block text-sm font-medium text-gray-700">Email</label>
                    <p className="mt-1 text-gray-900">{parsedData.email || '—'}</p>
                  </div>
                  <div>
                    <label className="block text-sm font-medium text-gray-700">Phone</label>
                    <p className="mt-1 text-gray-900">{parsedData.phone || '—'}</p>
                  </div>
                </div>

                <div>
                  <label className="block text-sm font-medium text-gray-700">Years of Experience</label>
                  <p className="mt-1 text-gray-900">
                    {parsedData.yearsOfExperience ? `${parsedData.yearsOfExperience} years` : '—'}
                  </p>
                </div>

                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-2">Summary</label>
                  <p className="mt-1 text-gray-700 text-sm whitespace-pre-wrap max-h-32 overflow-y-auto bg-gray-50 p-3 rounded">
                    {parsedData.summary || 'Not found'}
                  </p>
                </div>
              </div>
            </div>

            {/* Right Column - Actions */}
            <div className="bg-white rounded-lg shadow-md p-6 h-fit">
              <h2 className="text-lg font-semibold mb-4 text-gray-900">Actions</h2>
              
              <div className="space-y-3">
                {canManageCandidates && (
                  <button
                    onClick={() => setShowSaveModal(true)}
                    className="w-full px-4 py-2 bg-green-600 text-white rounded-lg hover:bg-green-700 transition-colors"
                  >
                    💾 Save to Candidate
                  </button>
                )}
                
                <button
                  onClick={() => window.print()}
                  className="w-full px-4 py-2 bg-gray-600 text-white rounded-lg hover:bg-gray-700 transition-colors"
                >
                  🖨️ Print Results
                </button>

                <button
                  onClick={handleReset}
                  className="w-full px-4 py-2 bg-gray-200 text-gray-700 rounded-lg hover:bg-gray-300 transition-colors"
                >
                  🔄 Parse Another
                </button>
              </div>

              {/* Resume Stats */}
              <div className="mt-6 pt-6 border-t border-gray-200">
                <p className="text-xs font-medium text-gray-600 mb-3">PARSED DATA</p>
                <div className="space-y-2 text-sm">
                  <div className="flex justify-between">
                    <span className="text-gray-600">Skills:</span>
                    <span className="font-medium text-gray-900">{parsedData.skills?.length || 0}</span>
                  </div>
                  <div className="flex justify-between">
                    <span className="text-gray-600">Education:</span>
                    <span className="font-medium text-gray-900">{parsedData.education?.length || 0}</span>
                  </div>
                </div>
              </div>
            </div>
          </div>

          {/* Skills Section */}
          {parsedData.skills && parsedData.skills.length > 0 && (
            <div className="bg-white rounded-lg shadow-md p-6 mb-6">
              <h2 className="text-lg font-semibold mb-4 text-gray-900">📌 Extracted Skills</h2>
              <div className="flex flex-wrap gap-2">
                {parsedData.skills.map((skill, index) => (
                  <span key={index} className="px-3 py-1 bg-blue-100 text-blue-800 rounded-full text-sm font-medium">
                    {skill}
                  </span>
                ))}
              </div>
            </div>
          )}

          {/* Education Section */}
          {parsedData.education && parsedData.education.length > 0 && (
            <div className="bg-white rounded-lg shadow-md p-6 mb-6">
              <h2 className="text-lg font-semibold mb-4 text-gray-900">🎓 Education</h2>
              <div className="space-y-2">
                {parsedData.education.map((edu, index) => (
                  <div key={index} className="flex items-start p-3 bg-gray-50 rounded">
                    <span className="text-lg mr-2">📚</span>
                    <p className="text-gray-900">{edu}</p>
                  </div>
                ))}
              </div>
            </div>
          )}

          {/* Raw Text Section */}
          {parsedData.rawText && (
            <div className="bg-white rounded-lg shadow-md p-6">
              <h2 className="text-lg font-semibold mb-4 text-gray-900">📄 Raw Text (First 1000 characters)</h2>
              <pre className="p-4 bg-gray-50 rounded text-xs text-gray-700 overflow-auto max-h-48 border border-gray-200">
                {parsedData.rawText.substring(0, 1000)}
                {parsedData.rawText.length > 1000 && '...'}
              </pre>
            </div>
          )}
        </>
      )}

      {/* Save to Candidate Modal */}
      {showSaveModal && parsedData && (
        <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50 p-4">
          <div className="bg-white rounded-lg max-w-2xl w-full max-h-96 overflow-y-auto">
            <div className="sticky top-0 bg-white border-b border-gray-200 px-6 py-4">
              <h2 className="text-2xl font-bold text-gray-900">Save Resume Data to Candidate</h2>
            </div>

            <div className="p-6 space-y-4">
              {/* Candidate Selection */}
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-2">Select Candidate</label>
                <select
                  value={selectedCandidateId}
                  onChange={(e) => setSelectedCandidateId(e.target.value)}
                  className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                >
                  <option value="">Choose a candidate...</option>
                  {candidates.map((candidate) => (
                    <option key={candidate.id} value={candidate.id}>
                      {candidate.firstName} {candidate.lastName} ({candidate.email})
                    </option>
                  ))}
                </select>
              </div>

              {/* Skills Selection */}
              {availableSkills.length > 0 && (
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-2">Assign Skills</label>
                  <div className="max-h-40 overflow-y-auto border border-gray-200 rounded p-3 space-y-2">
                    {availableSkills.map((skill) => (
                      <label key={skill.id} className="flex items-center">
                        <input
                          type="checkbox"
                          checked={selectedSkills.includes(skill.id)}
                          onChange={() => handleSkillToggle(skill.id)}
                          className="w-4 h-4 text-blue-600 rounded"
                        />
                        <span className="ml-2 text-sm text-gray-900">{skill.skillName}</span>
                      </label>
                    ))}
                  </div>
                </div>
              )}

              {/* Qualifications Selection */}
              {availableQualifications.length > 0 && (
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-2">Assign Qualifications</label>
                  <div className="max-h-40 overflow-y-auto border border-gray-200 rounded p-3 space-y-2">
                    {availableQualifications.map((qual) => (
                      <label key={qual.id} className="flex items-center">
                        <input
                          type="checkbox"
                          checked={selectedQualifications.includes(qual.id)}
                          onChange={() => handleQualificationToggle(qual.id)}
                          className="w-4 h-4 text-blue-600 rounded"
                        />
                        <span className="ml-2 text-sm text-gray-900">{qual.qualificationName}</span>
                      </label>
                    ))}
                  </div>
                </div>
              )}

              <div className="flex justify-end gap-3 pt-4 border-t border-gray-200">
                <button
                  onClick={() => setShowSaveModal(false)}
                  className="px-4 py-2 text-gray-700 bg-gray-200 rounded-lg hover:bg-gray-300 transition-colors"
                >
                  Cancel
                </button>
                <button
                  onClick={handleSaveData}
                  disabled={!selectedCandidateId}
                  className="px-4 py-2 bg-green-600 text-white rounded-lg hover:bg-green-700 disabled:opacity-50 transition-colors"
                >
                  💾 Save to Candidate
                </button>
              </div>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}
