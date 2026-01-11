import React, { useState, useEffect } from "react";
import { useAuth } from "../contexts/AuthContext";
import { reportsAPI } from "../services/api";
import {
  BarChart,
  Bar,
  LineChart,
  Line,
  PieChart,
  Pie,
  Cell,
  XAxis,
  YAxis,
  CartesianGrid,
  Tooltip,
  Legend,
  ResponsiveContainer,
} from "recharts";

const COLORS = ["#3B82F6", "#10B981", "#F59E0B", "#EF4444", "#8B5CF6", "#EC4899"];

const Dashboard = () => {
  const { user, hasRole } = useAuth();
  const [stats, setStats] = useState(null);
  const [pipeline, setPipeline] = useState([]);
  const [trends, setTrends] = useState([]);
  const [statusDist, setStatusDist] = useState([]);
  const [skillDemand, setSkillDemand] = useState([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    loadDashboardData();
  }, []);

  const loadDashboardData = async () => {
    try {
      setLoading(true);

      if (hasRole(["Admin", "Recruiter", "HR", "Interviewer", "Reviewer"])) {
        const [
          dashStats,
          pipelineData,
          trendsData,
          statusData,
          skillData,
        ] = await Promise.all([
          reportsAPI.getDashboardStats(),
          reportsAPI.getRecruitmentPipeline(),
          reportsAPI.getMonthlyTrends(6),
          reportsAPI.getStatusDistribution(),
          reportsAPI.getSkillDemand(),
        ]);

        setStats(dashStats);
        setPipeline(Array.isArray(pipelineData) ? pipelineData : []);
        setTrends(Array.isArray(trendsData) ? trendsData : []);
        setStatusDist(Array.isArray(statusData) ? statusData : []);
        setSkillDemand(Array.isArray(skillData) ? skillData.slice(0, 10) : []);
      }
    } catch (error) {
      console.error("Error loading dashboard data:", error);
    } finally {
      setLoading(false);
    }
  };

  if (loading) {
    return (
      <div className="min-h-screen flex items-center justify-center">
        <div className="animate-spin rounded-full h-32 w-32 border-b-2 border-blue-600"></div>
      </div>
    );
  }

  if (hasRole("Candidate")) {
    return (
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
        <h1 className="text-3xl font-bold text-gray-900 mb-6">
          Welcome, {user?.email}!
        </h1>
        <div className="bg-white shadow rounded-lg p-6">
          <p className="text-gray-600">
            Use the navigation above to browse jobs and manage your applications.
          </p>
        </div>
      </div>
    );
  }

  return (
    <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
      <div className="mb-8">
        <h1 className="text-3xl font-bold text-gray-900">Recruitment Dashboard</h1>
        <p className="text-gray-600 mt-2">Overview of your recruitment metrics</p>
      </div>

      {stats && (
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6 mb-8">
          <StatCard title="Total Jobs" value={stats.totalJobs} subtitle={`${stats.activeJobs} active`} color="blue" />
          <StatCard title="Total Candidates" value={stats.totalCandidates} subtitle={`${stats.totalApplications} applications`} color="green" />
          <StatCard title="Interviews" value={stats.interviewsScheduled} subtitle={`${stats.interviewsCompleted} completed`} color="purple" />
          <StatCard title="Hired" value={stats.candidatesHired} subtitle={`${stats.offersExtended} offers`} color="yellow" />
        </div>
      )}

      <div className="grid grid-cols-1 lg:grid-cols-2 gap-6 mb-8">
        <div className="bg-white shadow rounded-lg p-6">
          <h2 className="text-lg font-semibold text-gray-900 mb-4">Recruitment Pipeline</h2>
          <ResponsiveContainer width="100%" height={300}>
            <BarChart data={pipeline}>
              <CartesianGrid strokeDasharray="3 3" />
              <XAxis dataKey="stage" angle={-45} textAnchor="end" height={100} />
              <YAxis />
              <Tooltip />
              <Bar dataKey="count" fill="#3B82F6" />
            </BarChart>
          </ResponsiveContainer>
        </div>

        <div className="bg-white shadow rounded-lg p-6">
          <h2 className="text-lg font-semibold text-gray-900 mb-4">Status Distribution</h2>
          <ResponsiveContainer width="100%" height={300}>
            <PieChart>
              <Pie data={statusDist} dataKey="count" nameKey="status" cx="50%" cy="50%" outerRadius={100} label>
                {statusDist.map((entry, index) => (
                  <Cell key={`cell-${index}`} fill={COLORS[index % COLORS.length]} />
                ))}
              </Pie>
              <Tooltip />
              <Legend />
            </PieChart>
          </ResponsiveContainer>
        </div>
      </div>

      <div className="grid grid-cols-1 lg:grid-cols-2 gap-6 mb-8">
        <div className="bg-white shadow rounded-lg p-6">
          <h2 className="text-lg font-semibold text-gray-900 mb-4">Monthly Trends</h2>
          <ResponsiveContainer width="100%" height={300}>
            <LineChart data={trends}>
              <CartesianGrid strokeDasharray="3 3" />
              <XAxis dataKey="month" />
              <YAxis />
              <Tooltip />
              <Legend />
              <Line type="monotone" dataKey="applications" stroke="#3B82F6" name="Applications" />
              <Line type="monotone" dataKey="hired" stroke="#10B981" name="Hired" />
            </LineChart>
          </ResponsiveContainer>
        </div>

        <div className="bg-white shadow rounded-lg p-6">
          <h2 className="text-lg font-semibold text-gray-900 mb-4">Top Skills</h2>
          <ResponsiveContainer width="100%" height={300}>
            <BarChart data={skillDemand} layout="vertical">
              <CartesianGrid strokeDasharray="3 3" />
              <XAxis type="number" />
              <YAxis dataKey="skillName" type="category" width={100} />
              <Tooltip />
              <Bar dataKey="demandCount" fill="#8B5CF6" />
            </BarChart>
          </ResponsiveContainer>
        </div>
      </div>
    </div>
  );
};

const StatCard = ({ title, value, subtitle, color }) => {
  const colorClasses = { blue: "bg-blue-500", green: "bg-green-500", purple: "bg-purple-500", yellow: "bg-yellow-500" };
  return (
    <div className="bg-white overflow-hidden shadow rounded-lg">
      <div className="p-5">
        <div className="flex items-center">
          <div className="flex-shrink-0">
            <div className={`w-12 h-12 ${colorClasses[color]} rounded-md flex items-center justify-center`}>
              <span className="text-white font-bold text-xl">{title.charAt(0)}</span>
            </div>
          </div>
          <div className="ml-5 w-0 flex-1">
            <dl>
              <dt className="text-sm font-medium text-gray-500 truncate">{title}</dt>
              <dd className="text-3xl font-bold text-gray-900">{value}</dd>
              {subtitle && <dd className="text-sm text-gray-600 mt-1">{subtitle}</dd>}
            </dl>
          </div>
        </div>
      </div>
    </div>
  );
};

export default Dashboard;
