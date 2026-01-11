import { useState } from 'react';
import { exportAPI, jobsAPI, candidatesAPI, applicationsAPI } from '../services/api';
import { useAuth } from '../contexts/AuthContext';
import toast from 'react-hot-toast';

export default function ExportPage() {
  const [exporting, setExporting] = useState(null);
  const [filters, setFilters] = useState({
    jobsSearch: '',
    jobsStatus: '',
    jobsJobType: '',
    candidatesSearch: '',
    applicationsSearch: '',
    applicationsJob: '',
    applicationsCandidate: '',
    applicationsStatus: '',
  });

  const { user } = useAuth();
  const canExport = user?.role === 'Admin' || user?.role === 'HR' || user?.role === 'Recruiter';

  const downloadFile = (blob, filename) => {
    const url = window.URL.createObjectURL(blob);
    const link = document.createElement('a');
    link.href = url;
    link.download = filename;
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
    window.URL.revokeObjectURL(url);
  };

  const handleExportJobs = async () => {
    try {
      setExporting('jobs');
      const blob = await exportAPI.exportJobs(
        filters.jobsSearch,
        filters.jobsStatus,
        filters.jobsJobType
      );
      downloadFile(blob, `jobs-export-${new Date().toISOString().split('T')[0]}.csv`);
      toast.success('Jobs exported successfully');
    } catch (err) {
      toast.error('Failed to export jobs');
      console.error(err);
    } finally {
      setExporting(null);
    }
  };

  const handleExportCandidates = async () => {
    try {
      setExporting('candidates');
      const blob = await exportAPI.exportCandidates(filters.candidatesSearch);
      downloadFile(blob, `candidates-export-${new Date().toISOString().split('T')[0]}.csv`);
      toast.success('Candidates exported successfully');
    } catch (err) {
      toast.error('Failed to export candidates');
      console.error(err);
    } finally {
      setExporting(null);
    }
  };

  const handleExportApplications = async () => {
    try {
      setExporting('applications');
      const blob = await exportAPI.exportApplications(
        filters.applicationsSearch,
        filters.applicationsJob,
        filters.applicationsCandidate,
        filters.applicationsStatus
      );
      downloadFile(blob, `applications-export-${new Date().toISOString().split('T')[0]}.csv`);
      toast.success('Applications exported successfully');
    } catch (err) {
      toast.error('Failed to export applications');
      console.error(err);
    } finally {
      setExporting(null);
    }
  };

  const handleExportInterviews = async () => {
    try {
      setExporting('interviews');
      const blob = await exportAPI.exportInterviews();
      downloadFile(blob, `interviews-export-${new Date().toISOString().split('T')[0]}.csv`);
      toast.success('Interviews exported successfully');
    } catch (err) {
      toast.error('Failed to export interviews');
      console.error(err);
    } finally {
      setExporting(null);
    }
  };

  const handleFilterChange = (field, value) => {
    setFilters({
      ...filters,
      [field]: value,
    });
  };

  if (!canExport) {
    return (
      <div className="max-w-4xl mx-auto p-6">
        <div className="bg-red-50 border border-red-200 rounded-lg p-6 text-center">
          <p className="text-red-800">
            ⛔ You don't have permission to export data. Only Admins, HR, and Recruiters can access this feature.
          </p>
        </div>
      </div>
    );
  }

  return (
    <div className="max-w-7xl mx-auto p-6">
      {/* Header */}
      <div className="mb-8">
        <h1 className="text-3xl font-bold text-gray-900">Data Export</h1>
        <p className="text-gray-600 mt-1">Export recruitment data to CSV format for analysis and reporting</p>
      </div>

      {/* Export Cards Grid */}
      <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
        {/* Jobs Export */}
        <div className="bg-white rounded-lg shadow-md p-6">
          <div className="flex items-start justify-between mb-4">
            <div>
              <h2 className="text-xl font-semibold text-gray-900">📋 Jobs</h2>
              <p className="text-sm text-gray-600 mt-1">Export all job postings</p>
            </div>
            <span className="px-3 py-1 bg-blue-100 text-blue-800 rounded-full text-xs font-medium">CSV</span>
          </div>

          <div className="space-y-3 mb-4">
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">Search (optional)</label>
              <input
                type="text"
                placeholder="Search jobs..."
                value={filters.jobsSearch}
                onChange={(e) => handleFilterChange('jobsSearch', e.target.value)}
                className="w-full px-3 py-2 border border-gray-300 rounded-lg text-sm focus:ring-2 focus:ring-blue-500 focus:border-transparent"
              />
            </div>

            <div className="grid grid-cols-2 gap-3">
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">Status (optional)</label>
                <input
                  type="text"
                  placeholder="Status ID..."
                  value={filters.jobsStatus}
                  onChange={(e) => handleFilterChange('jobsStatus', e.target.value)}
                  className="w-full px-3 py-2 border border-gray-300 rounded-lg text-sm focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                />
              </div>
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">Job Type (optional)</label>
                <input
                  type="text"
                  placeholder="Job Type ID..."
                  value={filters.jobsJobType}
                  onChange={(e) => handleFilterChange('jobsJobType', e.target.value)}
                  className="w-full px-3 py-2 border border-gray-300 rounded-lg text-sm focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                />
              </div>
            </div>
          </div>

          <button
            onClick={handleExportJobs}
            disabled={exporting === 'jobs'}
            className="w-full px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 disabled:opacity-50 transition-colors"
          >
            {exporting === 'jobs' ? '⏳ Exporting...' : '📥 Export Jobs'}
          </button>
        </div>

        {/* Candidates Export */}
        <div className="bg-white rounded-lg shadow-md p-6">
          <div className="flex items-start justify-between mb-4">
            <div>
              <h2 className="text-xl font-semibold text-gray-900">👥 Candidates</h2>
              <p className="text-sm text-gray-600 mt-1">Export all candidates with skills & qualifications</p>
            </div>
            <span className="px-3 py-1 bg-green-100 text-green-800 rounded-full text-xs font-medium">CSV</span>
          </div>

          <div className="space-y-3 mb-4">
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">Search (optional)</label>
              <input
                type="text"
                placeholder="Search candidates..."
                value={filters.candidatesSearch}
                onChange={(e) => handleFilterChange('candidatesSearch', e.target.value)}
                className="w-full px-3 py-2 border border-gray-300 rounded-lg text-sm focus:ring-2 focus:ring-green-500 focus:border-transparent"
              />
            </div>
          </div>

          <button
            onClick={handleExportCandidates}
            disabled={exporting === 'candidates'}
            className="w-full px-4 py-2 bg-green-600 text-white rounded-lg hover:bg-green-700 disabled:opacity-50 transition-colors"
          >
            {exporting === 'candidates' ? '⏳ Exporting...' : '📥 Export Candidates'}
          </button>
        </div>

        {/* Applications Export */}
        <div className="bg-white rounded-lg shadow-md p-6">
          <div className="flex items-start justify-between mb-4">
            <div>
              <h2 className="text-xl font-semibold text-gray-900">📝 Applications</h2>
              <p className="text-sm text-gray-600 mt-1">Export job applications with status</p>
            </div>
            <span className="px-3 py-1 bg-purple-100 text-purple-800 rounded-full text-xs font-medium">CSV</span>
          </div>

          <div className="space-y-3 mb-4">
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">Search (optional)</label>
              <input
                type="text"
                placeholder="Search applications..."
                value={filters.applicationsSearch}
                onChange={(e) => handleFilterChange('applicationsSearch', e.target.value)}
                className="w-full px-3 py-2 border border-gray-300 rounded-lg text-sm focus:ring-2 focus:ring-purple-500 focus:border-transparent"
              />
            </div>

            <div className="grid grid-cols-2 gap-3">
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">Job (optional)</label>
                <input
                  type="text"
                  placeholder="Job ID..."
                  value={filters.applicationsJob}
                  onChange={(e) => handleFilterChange('applicationsJob', e.target.value)}
                  className="w-full px-3 py-2 border border-gray-300 rounded-lg text-sm focus:ring-2 focus:ring-purple-500 focus:border-transparent"
                />
              </div>
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">Candidate (optional)</label>
                <input
                  type="text"
                  placeholder="Candidate ID..."
                  value={filters.applicationsCandidate}
                  onChange={(e) => handleFilterChange('applicationsCandidate', e.target.value)}
                  className="w-full px-3 py-2 border border-gray-300 rounded-lg text-sm focus:ring-2 focus:ring-purple-500 focus:border-transparent"
                />
              </div>
            </div>

            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">Status (optional)</label>
              <input
                type="text"
                placeholder="Status ID..."
                value={filters.applicationsStatus}
                onChange={(e) => handleFilterChange('applicationsStatus', e.target.value)}
                className="w-full px-3 py-2 border border-gray-300 rounded-lg text-sm focus:ring-2 focus:ring-purple-500 focus:border-transparent"
              />
            </div>
          </div>

          <button
            onClick={handleExportApplications}
            disabled={exporting === 'applications'}
            className="w-full px-4 py-2 bg-purple-600 text-white rounded-lg hover:bg-purple-700 disabled:opacity-50 transition-colors"
          >
            {exporting === 'applications' ? '⏳ Exporting...' : '📥 Export Applications'}
          </button>
        </div>

        {/* Interviews Export */}
        <div className="bg-white rounded-lg shadow-md p-6">
          <div className="flex items-start justify-between mb-4">
            <div>
              <h2 className="text-xl font-semibold text-gray-900">🎤 Interviews</h2>
              <p className="text-sm text-gray-600 mt-1">Export all scheduled interviews</p>
            </div>
            <span className="px-3 py-1 bg-orange-100 text-orange-800 rounded-full text-xs font-medium">CSV</span>
          </div>

          <p className="text-sm text-gray-600 mb-4">Export all interviews with candidate and feedback details</p>

          <button
            onClick={handleExportInterviews}
            disabled={exporting === 'interviews'}
            className="w-full px-4 py-2 bg-orange-600 text-white rounded-lg hover:bg-orange-700 disabled:opacity-50 transition-colors"
          >
            {exporting === 'interviews' ? '⏳ Exporting...' : '📥 Export Interviews'}
          </button>
        </div>
      </div>

      {/* Information Card */}
      <div className="mt-8 bg-blue-50 border border-blue-200 rounded-lg p-6">
        <h3 className="text-lg font-semibold text-blue-900 mb-2">ℹ️ Export Information</h3>
        <ul className="text-sm text-blue-800 space-y-1">
          <li>✓ All exports are in CSV format for easy opening in Excel or Google Sheets</li>
          <li>✓ Use optional filters to export specific subsets of data</li>
          <li>✓ Files are downloaded immediately to your device</li>
          <li>✓ Large exports may take a moment to process</li>
          <li>✓ All data is exported as of the current moment</li>
          <li>✓ Filter by IDs using the UUID format (e.g., job ID, candidate ID, etc.)</li>
        </ul>
      </div>

      {/* Export Statistics */}
      <div className="mt-8 grid grid-cols-2 md:grid-cols-4 gap-4">
        <div className="bg-white rounded-lg shadow p-4 text-center">
          <p className="text-gray-600 text-sm">📋 Jobs</p>
          <p className="text-2xl font-bold text-blue-600 mt-1">Export</p>
        </div>
        <div className="bg-white rounded-lg shadow p-4 text-center">
          <p className="text-gray-600 text-sm">👥 Candidates</p>
          <p className="text-2xl font-bold text-green-600 mt-1">Export</p>
        </div>
        <div className="bg-white rounded-lg shadow p-4 text-center">
          <p className="text-gray-600 text-sm">📝 Applications</p>
          <p className="text-2xl font-bold text-purple-600 mt-1">Export</p>
        </div>
        <div className="bg-white rounded-lg shadow p-4 text-center">
          <p className="text-gray-600 text-sm">🎤 Interviews</p>
          <p className="text-2xl font-bold text-orange-600 mt-1">Export</p>
        </div>
      </div>
    </div>
  );
}
