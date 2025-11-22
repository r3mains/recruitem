import React from "react";
import { Link } from "react-router-dom";
import { useAuth } from "../contexts/AuthContext";

const LandingPage = () => {
  const { user } = useAuth();

  if (user) {
    return (
      <div className="min-h-screen bg-gradient-to-b from-blue-50 to-white">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
          <div className="pt-20 pb-12 text-center">
            <h1 className="text-4xl font-bold text-gray-900 sm:text-6xl">
              Welcome to RecruiteM
            </h1>
            <p className="mt-6 text-lg text-gray-600 max-w-2xl mx-auto">
              Your comprehensive recruitment management system
            </p>

            <div className="mt-10 grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6 max-w-4xl mx-auto">
              <div className="bg-white rounded-lg shadow-md p-6 hover:shadow-lg transition-shadow">
                <h3 className="text-lg font-semibold text-gray-900 mb-2">
                  Dashboard
                </h3>
                <p className="text-gray-600 mb-4">
                  View your personalized dashboard and statistics
                </p>
                <Link
                  to="/dashboard"
                  className="inline-block bg-blue-600 text-white px-4 py-2 rounded hover:bg-blue-700 transition-colors"
                >
                  Go to Dashboard
                </Link>
              </div>

              <div className="bg-white rounded-lg shadow-md p-6 hover:shadow-lg transition-shadow">
                <h3 className="text-lg font-semibold text-gray-900 mb-2">
                  Browse Jobs
                </h3>
                <p className="text-gray-600 mb-4">
                  Explore available job opportunities
                </p>
                <Link
                  to="/browse-jobs"
                  className="inline-block bg-blue-600 text-white px-4 py-2 rounded hover:bg-blue-700 transition-colors"
                >
                  View Jobs
                </Link>
              </div>

              {user.role === "Candidate" && (
                <div className="bg-white rounded-lg shadow-md p-6 hover:shadow-lg transition-shadow">
                  <h3 className="text-lg font-semibold text-gray-900 mb-2">
                    My Applications
                  </h3>
                  <p className="text-gray-600 mb-4">
                    Track your job applications and their status
                  </p>
                  <Link
                    to="/applications"
                    className="inline-block bg-blue-600 text-white px-4 py-2 rounded hover:bg-blue-700 transition-colors"
                  >
                    View Applications
                  </Link>
                </div>
              )}

              {["Admin", "Recruiter", "HR"].includes(user.role) && (
                <>
                  <div className="bg-white rounded-lg shadow-md p-6 hover:shadow-lg transition-shadow">
                    <h3 className="text-lg font-semibold text-gray-900 mb-2">
                      Manage Jobs
                    </h3>
                    <p className="text-gray-600 mb-4">
                      Create and manage job postings
                    </p>
                    <Link
                      to="/jobs"
                      className="inline-block bg-blue-600 text-white px-4 py-2 rounded hover:bg-blue-700 transition-colors"
                    >
                      Manage Jobs
                    </Link>
                  </div>

                  <div className="bg-white rounded-lg shadow-md p-6 hover:shadow-lg transition-shadow">
                    <h3 className="text-lg font-semibold text-gray-900 mb-2">
                      Candidates
                    </h3>
                    <p className="text-gray-600 mb-4">
                      Review and manage candidates
                    </p>
                    <Link
                      to="/candidates"
                      className="inline-block bg-blue-600 text-white px-4 py-2 rounded hover:bg-blue-700 transition-colors"
                    >
                      View Candidates
                    </Link>
                  </div>

                  <div className="bg-white rounded-lg shadow-md p-6 hover:shadow-lg transition-shadow">
                    <h3 className="text-lg font-semibold text-gray-900 mb-2">
                      Interviews
                    </h3>
                    <p className="text-gray-600 mb-4">
                      Schedule and manage interviews
                    </p>
                    <Link
                      to="/interviews"
                      className="inline-block bg-blue-600 text-white px-4 py-2 rounded hover:bg-blue-700 transition-colors"
                    >
                      Manage Interviews
                    </Link>
                  </div>
                </>
              )}
            </div>
          </div>
        </div>
      </div>
    );
  }

  return (
    <div className="min-h-screen bg-gradient-to-b from-blue-50 to-white">
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div className="pt-20 pb-12 text-center">
          <h1 className="text-4xl font-bold text-gray-900 sm:text-6xl">
            Welcome to RecruiteM
          </h1>
          <p className="mt-6 text-lg text-gray-600 max-w-2xl mx-auto">
            Your comprehensive recruitment management system. Connect talent
            with opportunities.
          </p>

          <div className="mt-10 flex flex-col sm:flex-row gap-4 justify-center">
            <Link
              to="/login"
              className="bg-blue-600 text-white px-8 py-3 rounded-lg text-lg font-medium hover:bg-blue-700 transition-colors"
            >
              Sign In
            </Link>
            <Link
              to="/register"
              className="bg-white text-blue-600 px-8 py-3 rounded-lg text-lg font-medium border-2 border-blue-600 hover:bg-blue-50 transition-colors"
            >
              Create Account
            </Link>
            <Link
              to="/browse-jobs"
              className="bg-gray-100 text-gray-700 px-8 py-3 rounded-lg text-lg font-medium hover:bg-gray-200 transition-colors"
            >
              Browse Jobs
            </Link>
          </div>

          <div className="mt-16 grid grid-cols-1 md:grid-cols-3 gap-8">
            <div className="text-center">
              <div className="bg-blue-100 rounded-full w-16 h-16 flex items-center justify-center mx-auto mb-4">
                <svg
                  className="w-8 h-8 text-blue-600"
                  fill="none"
                  stroke="currentColor"
                  viewBox="0 0 24 24"
                >
                  <path
                    strokeLinecap="round"
                    strokeLinejoin="round"
                    strokeWidth={2}
                    d="M21 13.255A23.931 23.931 0 0112 15c-3.183 0-6.22-.62-9-1.745M16 6V4a2 2 0 00-2-2h-4a2 2 0 00-2-2v2m8 0V6a2 2 0 012 2v6.5"
                  />
                </svg>
              </div>
              <h3 className="text-lg font-semibold text-gray-900 mb-2">
                For Job Seekers
              </h3>
              <p className="text-gray-600">
                Find your dream job and track your applications with ease
              </p>
            </div>

            <div className="text-center">
              <div className="bg-green-100 rounded-full w-16 h-16 flex items-center justify-center mx-auto mb-4">
                <svg
                  className="w-8 h-8 text-green-600"
                  fill="none"
                  stroke="currentColor"
                  viewBox="0 0 24 24"
                >
                  <path
                    strokeLinecap="round"
                    strokeLinejoin="round"
                    strokeWidth={2}
                    d="M17 20h5v-2a3 3 0 00-5.356-1.857M17 20H7m10 0v-2c0-.656-.126-1.283-.356-1.857M7 20H2v-2a3 3 0 015.356-1.857M7 20v-2c0-.656.126-1.283.356-1.857m0 0a5.002 5.002 0 019.288 0M15 7a3 3 0 11-6 0 3 3 0 016 0zm6 3a2 2 0 11-4 0 2 2 0 014 0zM7 10a2 2 0 11-4 0 2 2 0 014 0z"
                  />
                </svg>
              </div>
              <h3 className="text-lg font-semibold text-gray-900 mb-2">
                For Recruiters
              </h3>
              <p className="text-gray-600">
                Streamline your hiring process and find the best candidates
              </p>
            </div>

            <div className="text-center">
              <div className="bg-purple-100 rounded-full w-16 h-16 flex items-center justify-center mx-auto mb-4">
                <svg
                  className="w-8 h-8 text-purple-600"
                  fill="none"
                  stroke="currentColor"
                  viewBox="0 0 24 24"
                >
                  <path
                    strokeLinecap="round"
                    strokeLinejoin="round"
                    strokeWidth={2}
                    d="M9 19v-6a2 2 0 00-2-2H5a2 2 0 00-2 2v6a2 2 0 002 2h2a2 2 0 002-2zm0 0V9a2 2 0 012-2h2a2 2 0 012 2v10m-6 0a2 2 0 002 2h2a2 2 0 002-2m0 0V5a2 2 0 012-2h2a2 2 0 012 2v14a2 2 0 01-2 2h-2a2 2 0 01-2-2z"
                  />
                </svg>
              </div>
              <h3 className="text-lg font-semibold text-gray-900 mb-2">
                Analytics & Reports
              </h3>
              <p className="text-gray-600">
                Get insights into your recruitment process with detailed
                analytics
              </p>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};

export default LandingPage;
