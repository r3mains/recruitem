import React, { useState, useEffect } from "react";
import { useAuth } from "../contexts/AuthContext";
import { useApi } from "../contexts/ApiContext";
import { Link } from "react-router-dom";

const Dashboard = () => {
  const { user, hasRole } = useAuth();
  const api = useApi();
  const [stats, setStats] = useState({
    totalJobs: 0,
    totalApplications: 0,
    totalCandidates: 0,
    myApplications: 0,
  });
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    loadDashboardData();
  }, []);

  const loadDashboardData = async () => {
    try {
      setLoading(true);

      if (hasRole(["Admin", "Recruiter", "HR"])) {
        const [jobs, applications, candidates] = await Promise.all([
          api.jobs.getAll(),
          api.applications ? api.applications.getAll() : [],
          api.candidates ? api.candidates.getAll() : [],
        ]);

        setStats({
          totalJobs: jobs.length,
          totalApplications: applications.length || 0,
          totalCandidates: candidates.length || 0,
        });
      } else if (hasRole("Candidate")) {
        const myApps = await api.applications.getMyApplications();
        setStats({
          myApplications: myApps.length,
        });
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

  return (
    <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
      <div className="mb-8">
        <h1 className="text-3xl font-bold text-gray-900">
          Welcome back, {user?.email}!
        </h1>
        <p className="text-gray-600 mt-2">Role: {user?.role}</p>
      </div>

      {hasRole(["Admin", "Recruiter", "HR", "Interviewer", "Reviewer"]) && (
        <>
          <div className="grid grid-cols-1 md:grid-cols-3 gap-6 mb-8">
            <div className="bg-white overflow-hidden shadow rounded-lg">
              <div className="p-5">
                <div className="flex items-center">
                  <div className="flex-shrink-0">
                    <div className="w-8 h-8 bg-blue-500 rounded-md flex items-center justify-center">
                      <span className="text-white font-semibold">J</span>
                    </div>
                  </div>
                  <div className="ml-5 w-0 flex-1">
                    <dl>
                      <dt className="text-sm font-medium text-gray-500 truncate">
                        Total Jobs
                      </dt>
                      <dd className="text-lg font-medium text-gray-900">
                        {stats.totalJobs}
                      </dd>
                    </dl>
                  </div>
                </div>
              </div>
            </div>

            <div className="bg-white overflow-hidden shadow rounded-lg">
              <div className="p-5">
                <div className="flex items-center">
                  <div className="flex-shrink-0">
                    <div className="w-8 h-8 bg-green-500 rounded-md flex items-center justify-center">
                      <span className="text-white font-semibold">A</span>
                    </div>
                  </div>
                  <div className="ml-5 w-0 flex-1">
                    <dl>
                      <dt className="text-sm font-medium text-gray-500 truncate">
                        Applications
                      </dt>
                      <dd className="text-lg font-medium text-gray-900">
                        {stats.totalApplications}
                      </dd>
                    </dl>
                  </div>
                </div>
              </div>
            </div>

            <div className="bg-white overflow-hidden shadow rounded-lg">
              <div className="p-5">
                <div className="flex items-center">
                  <div className="flex-shrink-0">
                    <div className="w-8 h-8 bg-purple-500 rounded-md flex items-center justify-center">
                      <span className="text-white font-semibold">C</span>
                    </div>
                  </div>
                  <div className="ml-5 w-0 flex-1">
                    <dl>
                      <dt className="text-sm font-medium text-gray-500 truncate">
                        Candidates
                      </dt>
                      <dd className="text-lg font-medium text-gray-900">
                        {stats.totalCandidates}
                      </dd>
                    </dl>
                  </div>
                </div>
              </div>
            </div>
          </div>

          <div className="bg-white shadow rounded-lg p-6 mb-8">
            <h2 className="text-lg font-medium text-gray-900 mb-4">
              Quick Actions
            </h2>
            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4">
              <Link
                to="/jobs"
                className="bg-blue-50 p-4 rounded-lg hover:bg-blue-100 transition-colors"
              >
                <h3 className="font-medium text-blue-900">Manage Jobs</h3>
                <p className="text-sm text-blue-700 mt-1">
                  Create and edit job postings
                </p>
              </Link>

              <Link
                to="/candidates"
                className="bg-green-50 p-4 rounded-lg hover:bg-green-100 transition-colors"
              >
                <h3 className="font-medium text-green-900">View Candidates</h3>
                <p className="text-sm text-green-700 mt-1">
                  Browse candidate profiles
                </p>
              </Link>

              <Link
                to="/screening"
                className="bg-purple-50 p-4 rounded-lg hover:bg-purple-100 transition-colors"
              >
                <h3 className="font-medium text-purple-900">
                  Screen Applications
                </h3>
                <p className="text-sm text-purple-700 mt-1">
                  Review and process applications
                </p>
              </Link>

              <Link
                to="/positions"
                className="bg-orange-50 p-4 rounded-lg hover:bg-orange-100 transition-colors"
              >
                <h3 className="font-medium text-orange-900">
                  Manage Positions
                </h3>
                <p className="text-sm text-orange-700 mt-1">
                  Create and manage positions
                </p>
              </Link>
            </div>
          </div>
        </>
      )}

      {hasRole("Candidate") && (
        <>
          <div className="grid grid-cols-1 md:grid-cols-2 gap-6 mb-8">
            <div className="bg-white overflow-hidden shadow rounded-lg">
              <div className="p-5">
                <div className="flex items-center">
                  <div className="flex-shrink-0">
                    <div className="w-8 h-8 bg-blue-500 rounded-md flex items-center justify-center">
                      <span className="text-white font-semibold">A</span>
                    </div>
                  </div>
                  <div className="ml-5 w-0 flex-1">
                    <dl>
                      <dt className="text-sm font-medium text-gray-500 truncate">
                        My Applications
                      </dt>
                      <dd className="text-lg font-medium text-gray-900">
                        {stats.myApplications}
                      </dd>
                    </dl>
                  </div>
                </div>
              </div>
            </div>
          </div>

          <div className="bg-white shadow rounded-lg p-6">
            <h2 className="text-lg font-medium text-gray-900 mb-4">
              Quick Actions
            </h2>
            <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
              <Link
                to="/browse-jobs"
                className="bg-blue-50 p-4 rounded-lg hover:bg-blue-100 transition-colors"
              >
                <h3 className="font-medium text-blue-900">Browse Jobs</h3>
                <p className="text-sm text-blue-700 mt-1">
                  Find and apply to jobs
                </p>
              </Link>

              <Link
                to="/profile"
                className="bg-green-50 p-4 rounded-lg hover:bg-green-100 transition-colors"
              >
                <h3 className="font-medium text-green-900">Update Profile</h3>
                <p className="text-sm text-green-700 mt-1">
                  Manage your profile and skills
                </p>
              </Link>

              <Link
                to="/applications"
                className="bg-purple-50 p-4 rounded-lg hover:bg-purple-100 transition-colors"
              >
                <h3 className="font-medium text-purple-900">My Applications</h3>
                <p className="text-sm text-purple-700 mt-1">
                  Track application status
                </p>
              </Link>
            </div>
          </div>
        </>
      )}
    </div>
  );
};

export default Dashboard;
