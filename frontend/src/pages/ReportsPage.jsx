import { useState, useEffect } from 'react';
import { reportsAPI, jobsAPI } from '../services/api';
import toast from 'react-hot-toast';
import LoadingSpinner from '../components/LoadingSpinner';
import { BarChart, Bar, LineChart, Line, PieChart, Pie, Cell, XAxis, YAxis, CartesianGrid, Tooltip, Legend, ResponsiveContainer } from 'recharts';

const COLORS = ['#3b82f6', '#10b981', '#f59e0b', '#ef4444', '#8b5cf6', '#06b6d4', '#ec4899', '#f97316'];

export default function ReportsPage() {
  const [activeTab, setActiveTab] = useState('overview');
  const [loading, setLoading] = useState(true);
  const [jobs, setJobs] = useState([]);
  const [selectedJobId, setSelectedJobId] = useState('');

  // Data states
  const [dashboardStats, setDashboardStats] = useState(null);
  const [pipelineData, setPipelineData] = useState(null);
  const [statusDistribution, setStatusDistribution] = useState([]);
  const [monthlyTrends, setMonthlyTrends] = useState([]);
  const [skillDemand, setSkillDemand] = useState([]);
  const [applicationFunnel, setApplicationFunnel] = useState([]);
  const [experienceWise, setExperienceWise] = useState([]);
  const [collegeWise, setCollegeWise] = useState([]);
  const [recruiterPerformance, setRecruiterPerformance] = useState([]);
  const [jobStats, setJobStats] = useState([]);

  useEffect(() => {
    fetchAllData();
    fetchJobs();
  }, []);

  const fetchJobs = async () => {
    try {
      const data = await jobsAPI.getAll();
      setJobs(data || []);
    } catch (err) {
      console.error('Failed to fetch jobs');
    }
  };

  const fetchAllData = async () => {
    try {
      setLoading(true);
      const [stats, pipeline, status, trends, skills, funnel, exp, college, recruiter, jobSts] = 
        await Promise.all([
          reportsAPI.getDashboardStats(),
          reportsAPI.getRecruitmentPipeline(),
          reportsAPI.getStatusDistribution(),
          reportsAPI.getMonthlyTrends(12),
          reportsAPI.getSkillDemand(),
          reportsAPI.getApplicationFunnel(),
          reportsAPI.getExperienceWiseCandidates(),
          reportsAPI.getCollegeWiseReport(),
          reportsAPI.getRecruiterPerformance(),
          reportsAPI.getJobStats()
        ]);

      setDashboardStats(stats);
      setPipelineData(pipeline);
      setStatusDistribution(status || []);
      setMonthlyTrends(trends || []);
      setSkillDemand(skills || []);
      setApplicationFunnel(funnel || []);
      setExperienceWise(exp || []);
      setCollegeWise(college || []);
      setRecruiterPerformance(recruiter || []);
      setJobStats(jobSts || []);
    } catch (err) {
      toast.error('Failed to load reports data');
      console.error(err);
    } finally {
      setLoading(false);
    }
  };

  if (loading) return <LoadingSpinner />;

  const StatCard = ({ title, value, subtext, color = 'blue' }) => {
    const colorMap = {
      blue: 'bg-blue-50 border-blue-200',
      green: 'bg-green-50 border-green-200',
      purple: 'bg-purple-50 border-purple-200',
      orange: 'bg-orange-50 border-orange-200',
      red: 'bg-red-50 border-red-200'
    };

    return (
      <div className={`${colorMap[color]} border rounded-lg p-4`}>
        <p className="text-sm text-gray-600">{title}</p>
        <p className="text-3xl font-bold text-gray-900 mt-1">{value}</p>
        {subtext && <p className="text-xs text-gray-500 mt-1">{subtext}</p>}
      </div>
    );
  };

  return (
    <div className="max-w-7xl mx-auto p-6">
      {/* Header */}
      <div>
        <h1 className="text-3xl font-bold text-gray-900">Reports & Analytics</h1>
        <p className="text-gray-600 mt-1">Comprehensive recruitment metrics and insights</p>
      </div>

      {/* Tabs */}
      <div className="flex gap-2 mt-6 mb-6 border-b border-gray-200">
        {['overview', 'pipeline', 'performance', 'detailed'].map((tab) => (
          <button
            key={tab}
            onClick={() => setActiveTab(tab)}
            className={`px-4 py-2 font-medium transition-colors ${
              activeTab === tab
                ? 'text-blue-600 border-b-2 border-blue-600'
                : 'text-gray-600 hover:text-gray-900'
            }`}
          >
            {tab.charAt(0).toUpperCase() + tab.slice(1)}
          </button>
        ))}
      </div>

      {/* Overview Tab */}
      {activeTab === 'overview' && dashboardStats && (
        <div className="space-y-6">
          {/* Key Stats */}
          <div className="grid grid-cols-1 md:grid-cols-4 gap-4">
            <StatCard
              title="Total Jobs"
              value={dashboardStats.totalJobs}
              subtext={`${dashboardStats.activeJobs} active`}
              color="blue"
            />
            <StatCard
              title="Total Candidates"
              value={dashboardStats.totalCandidates}
              subtext="All candidates"
              color="green"
            />
            <StatCard
              title="Total Applications"
              value={dashboardStats.totalApplications}
              subtext={`${dashboardStats.pendingApplications} pending`}
              color="purple"
            />
            <StatCard
              title="Candidates Hired"
              value={dashboardStats.candidatesHired}
              subtext="Total hires"
              color="orange"
            />
          </div>

          {/* Recruitment Pipeline */}
          {pipelineData && (
            <div className="bg-white rounded-lg shadow-md p-6">
              <h2 className="text-xl font-bold text-gray-900 mb-4">Recruitment Pipeline</h2>
              <div className="grid grid-cols-2 md:grid-cols-4 gap-4">
                {[
                  { label: 'New Applications', value: pipelineData.newApplications },
                  { label: 'Under Review', value: pipelineData.underReview },
                  { label: 'Shortlisted', value: pipelineData.shortlisted },
                  { label: 'Interview Scheduled', value: pipelineData.interviewScheduled },
                  { label: 'Interview Completed', value: pipelineData.interviewCompleted },
                  { label: 'Offer Extended', value: pipelineData.offerExtended },
                  { label: 'Hired', value: pipelineData.hired },
                  { label: 'Rejected', value: pipelineData.rejected }
                ].map((item) => (
                  <div key={item.label} className="bg-gray-50 rounded p-3">
                    <p className="text-xs text-gray-600">{item.label}</p>
                    <p className="text-2xl font-bold text-gray-900">{item.value}</p>
                  </div>
                ))}
              </div>
            </div>
          )}

          {/* Charts */}
          <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
            {/* Status Distribution */}
            {statusDistribution.length > 0 && (
              <div className="bg-white rounded-lg shadow-md p-6">
                <h2 className="text-xl font-bold text-gray-900 mb-4">Application Status Distribution</h2>
                <ResponsiveContainer width="100%" height={300}>
                  <PieChart>
                    <Pie
                      data={statusDistribution}
                      dataKey="count"
                      nameKey="status"
                      cx="50%"
                      cy="50%"
                      outerRadius={100}
                      label
                    >
                      {statusDistribution.map((entry, index) => (
                        <Cell key={`cell-${index}`} fill={COLORS[index % COLORS.length]} />
                      ))}
                    </Pie>
                    <Tooltip formatter={(value) => `${value} applications`} />
                  </PieChart>
                </ResponsiveContainer>
              </div>
            )}

            {/* Monthly Trends */}
            {monthlyTrends.length > 0 && (
              <div className="bg-white rounded-lg shadow-md p-6">
                <h2 className="text-xl font-bold text-gray-900 mb-4">Monthly Trends (Last 12 Months)</h2>
                <ResponsiveContainer width="100%" height={300}>
                  <LineChart data={monthlyTrends}>
                    <CartesianGrid strokeDasharray="3 3" />
                    <XAxis dataKey="monthName" />
                    <YAxis />
                    <Tooltip />
                    <Legend />
                    <Line type="monotone" dataKey="jobsPosted" stroke="#3b82f6" />
                    <Line type="monotone" dataKey="applicationsReceived" stroke="#10b981" />
                    <Line type="monotone" dataKey="candidatesHired" stroke="#f59e0b" />
                  </LineChart>
                </ResponsiveContainer>
              </div>
            )}
          </div>
        </div>
      )}

      {/* Pipeline Tab */}
      {activeTab === 'pipeline' && (
        <div className="space-y-6">
          {/* Application Funnel */}
          {applicationFunnel.length > 0 && (
            <div className="bg-white rounded-lg shadow-md p-6">
              <h2 className="text-xl font-bold text-gray-900 mb-4">Application Funnel Analysis</h2>
              <div className="space-y-4">
                {applicationFunnel.map((stage, index) => (
                  <div key={index} className="space-y-2">
                    <div className="flex justify-between items-center">
                      <span className="text-sm font-medium text-gray-900">{stage.stage}</span>
                      <div className="text-right">
                        <p className="text-sm font-bold text-gray-900">{stage.count} applications</p>
                        <p className="text-xs text-gray-500">
                          Conversion: {stage.conversionRate?.toFixed(1)}% | Drop-off: {stage.dropOffRate?.toFixed(1)}%
                        </p>
                      </div>
                    </div>
                    <div className="w-full bg-gray-200 rounded-full h-8 overflow-hidden">
                      <div
                        className="bg-blue-600 h-full flex items-center px-2"
                        style={{
                          width: `${Math.min(100, (stage.count / (applicationFunnel[0]?.count || 1)) * 100)}%`
                        }}
                      >
                        <span className="text-xs font-bold text-white">
                          {((stage.count / (applicationFunnel[0]?.count || 1)) * 100).toFixed(0)}%
                        </span>
                      </div>
                    </div>
                  </div>
                ))}
              </div>
            </div>
          )}

          {/* Skill Demand */}
          {skillDemand.length > 0 && (
            <div className="bg-white rounded-lg shadow-md p-6">
              <h2 className="text-xl font-bold text-gray-900 mb-4">Top Skill Demands</h2>
              <ResponsiveContainer width="100%" height={400}>
                <BarChart data={skillDemand.slice(0, 10)}>
                  <CartesianGrid strokeDasharray="3 3" />
                  <XAxis dataKey="skillName" angle={-45} textAnchor="end" height={100} />
                  <YAxis />
                  <Tooltip />
                  <Legend />
                  <Bar dataKey="jobPostings" fill="#3b82f6" name="Job Postings" />
                  <Bar dataKey="candidatesWithSkill" fill="#10b981" name="Candidates" />
                </BarChart>
              </ResponsiveContainer>
            </div>
          )}
        </div>
      )}

      {/* Performance Tab */}
      {activeTab === 'performance' && (
        <div className="space-y-6">
          {/* Recruiter Performance */}
          {recruiterPerformance.length > 0 && (
            <div className="bg-white rounded-lg shadow-md p-6">
              <h2 className="text-xl font-bold text-gray-900 mb-4">Recruiter Performance</h2>
              <div className="overflow-x-auto">
                <table className="w-full text-sm">
                  <thead className="bg-gray-50">
                    <tr>
                      <th className="px-4 py-2 text-left font-medium text-gray-900">Recruiter</th>
                      <th className="px-4 py-2 text-left font-medium text-gray-900">Jobs Posted</th>
                      <th className="px-4 py-2 text-left font-medium text-gray-900">Applications</th>
                      <th className="px-4 py-2 text-left font-medium text-gray-900">Hires</th>
                      <th className="px-4 py-2 text-left font-medium text-gray-900">Avg. Time to Hire</th>
                      <th className="px-4 py-2 text-left font-medium text-gray-900">Conversion Rate</th>
                    </tr>
                  </thead>
                  <tbody className="divide-y divide-gray-200">
                    {recruiterPerformance.map((recruiter) => (
                      <tr key={recruiter.recruiterId} className="hover:bg-gray-50">
                        <td className="px-4 py-2 font-medium text-gray-900">{recruiter.recruiterName}</td>
                        <td className="px-4 py-2 text-gray-600">{recruiter.jobsPosted}</td>
                        <td className="px-4 py-2 text-gray-600">{recruiter.totalApplications}</td>
                        <td className="px-4 py-2 text-gray-600">{recruiter.candidatesHired}</td>
                        <td className="px-4 py-2 text-gray-600">
                          {recruiter.averageTimeToHire?.toFixed(1)} days
                        </td>
                        <td className="px-4 py-2">
                          <div className="flex items-center gap-2">
                            <div className="w-12 bg-gray-200 rounded h-2">
                              <div
                                className="bg-green-600 h-2 rounded"
                                style={{ width: `${recruiter.hireConversionRate * 100}%` }}
                              />
                            </div>
                            <span className="text-xs font-medium">
                              {(recruiter.hireConversionRate * 100).toFixed(1)}%
                            </span>
                          </div>
                        </td>
                      </tr>
                    ))}
                  </tbody>
                </table>
              </div>
            </div>
          )}

          {/* Job Statistics */}
          {jobStats.length > 0 && (
            <div className="bg-white rounded-lg shadow-md p-6">
              <h2 className="text-xl font-bold text-gray-900 mb-4">Job Statistics</h2>
              <div className="overflow-x-auto">
                <table className="w-full text-sm">
                  <thead className="bg-gray-50">
                    <tr>
                      <th className="px-4 py-2 text-left font-medium text-gray-900">Job Title</th>
                      <th className="px-4 py-2 text-left font-medium text-gray-900">Status</th>
                      <th className="px-4 py-2 text-left font-medium text-gray-900">Applications</th>
                      <th className="px-4 py-2 text-left font-medium text-gray-900">Shortlisted</th>
                      <th className="px-4 py-2 text-left font-medium text-gray-900">Interviews</th>
                      <th className="px-4 py-2 text-left font-medium text-gray-900">Hired</th>
                    </tr>
                  </thead>
                  <tbody className="divide-y divide-gray-200">
                    {jobStats.slice(0, 10).map((job) => (
                      <tr key={job.jobId} className="hover:bg-gray-50">
                        <td className="px-4 py-2 font-medium text-gray-900">{job.jobTitle}</td>
                        <td className="px-4 py-2">
                          <span
                            className={`px-2 py-1 rounded-full text-xs font-medium ${
                              job.status === 'Open'
                                ? 'bg-green-100 text-green-800'
                                : 'bg-gray-100 text-gray-800'
                            }`}
                          >
                            {job.status}
                          </span>
                        </td>
                        <td className="px-4 py-2 text-gray-600">{job.totalApplications}</td>
                        <td className="px-4 py-2 text-gray-600">{job.shortlisted}</td>
                        <td className="px-4 py-2 text-gray-600">{job.interviewScheduled}</td>
                        <td className="px-4 py-2 text-gray-600">{job.hired}</td>
                      </tr>
                    ))}
                  </tbody>
                </table>
              </div>
            </div>
          )}
        </div>
      )}

      {/* Detailed Tab */}
      {activeTab === 'detailed' && (
        <div className="space-y-6">
          {/* Experience-wise Candidates */}
          {experienceWise.length > 0 && (
            <div className="bg-white rounded-lg shadow-md p-6">
              <h2 className="text-xl font-bold text-gray-900 mb-4">Experience-wise Candidate Distribution</h2>
              <ResponsiveContainer width="100%" height={300}>
                <BarChart
                  data={experienceWise.map((exp) => ({
                    ...exp,
                    count: exp.candidateCount
                  }))}
                >
                  <CartesianGrid strokeDasharray="3 3" />
                  <XAxis dataKey="experienceRange" />
                  <YAxis />
                  <Tooltip />
                  <Bar dataKey="count" fill="#3b82f6" name="Number of Candidates" />
                </BarChart>
              </ResponsiveContainer>
            </div>
          )}

          {/* College-wise Report */}
          {collegeWise.length > 0 && (
            <div className="bg-white rounded-lg shadow-md p-6">
              <h2 className="text-xl font-bold text-gray-900 mb-4">College-wise Report</h2>
              <div className="overflow-x-auto">
                <table className="w-full text-sm">
                  <thead className="bg-gray-50">
                    <tr>
                      <th className="px-4 py-2 text-left font-medium text-gray-900">College</th>
                      <th className="px-4 py-2 text-left font-medium text-gray-900">Candidates</th>
                      <th className="px-4 py-2 text-left font-medium text-gray-900">Applications</th>
                      <th className="px-4 py-2 text-left font-medium text-gray-900">Shortlisted</th>
                      <th className="px-4 py-2 text-left font-medium text-gray-900">Interviewed</th>
                      <th className="px-4 py-2 text-left font-medium text-gray-900">Hired</th>
                      <th className="px-4 py-2 text-left font-medium text-gray-900">Success Rate</th>
                      <th className="px-4 py-2 text-left font-medium text-gray-900">Avg. Time to Hire</th>
                    </tr>
                  </thead>
                  <tbody className="divide-y divide-gray-200">
                    {collegeWise.map((college) => (
                      <tr key={college.college} className="hover:bg-gray-50">
                        <td className="px-4 py-2 font-medium text-gray-900">{college.college}</td>
                        <td className="px-4 py-2 text-gray-600">{college.totalCandidates}</td>
                        <td className="px-4 py-2 text-gray-600">{college.totalApplications}</td>
                        <td className="px-4 py-2 text-gray-600">{college.shortlistedCount}</td>
                        <td className="px-4 py-2 text-gray-600">{college.interviewedCount}</td>
                        <td className="px-4 py-2 text-gray-600">{college.hiredCount}</td>
                        <td className="px-4 py-2">
                          <div className="flex items-center gap-2">
                            <div className="w-12 bg-gray-200 rounded h-2">
                              <div
                                className="bg-green-600 h-2 rounded"
                                style={{ width: `${college.successRate}%` }}
                              />
                            </div>
                            <span className="text-xs font-medium">{college.successRate?.toFixed(1)}%</span>
                          </div>
                        </td>
                        <td className="px-4 py-2 text-gray-600">
                          {college.averageTimeToHireDays?.toFixed(1)} days
                        </td>
                      </tr>
                    ))}
                  </tbody>
                </table>
              </div>
            </div>
          )}
        </div>
      )}
    </div>
  );
}
