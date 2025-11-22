import React from "react";
import { Link, useNavigate } from "react-router-dom";
import { useAuth } from "../contexts/AuthContext";

const Navigation = () => {
  const { user, logout, hasRole } = useAuth();
  const navigate = useNavigate();

  const handleLogout = () => {
    logout();
    navigate("/login");
  };

  if (!user) return null;

  return (
    <nav className="bg-blue-600 text-white shadow-lg">
      <div className="max-w-7xl mx-auto px-4">
        <div className="flex justify-between items-center h-16">
          <div className="flex items-center space-x-8">
            <Link to="/" className="text-xl font-bold">
              RecruiteM
            </Link>

            <div className="flex space-x-4">
              {user && (
                <Link
                  to="/dashboard"
                  className="hover:bg-blue-700 px-3 py-2 rounded"
                >
                  Dashboard
                </Link>
              )}

              {hasRole([
                "Admin",
                "Recruiter",
                "HR",
                "Interviewer",
                "Reviewer",
              ]) && (
                <Link
                  to="/jobs"
                  className="hover:bg-blue-700 px-3 py-2 rounded"
                >
                  Jobs
                </Link>
              )}

              {hasRole([
                "Admin",
                "Recruiter",
                "HR",
                "Interviewer",
                "Reviewer",
              ]) && (
                <Link
                  to="/positions"
                  className="hover:bg-blue-700 px-3 py-2 rounded"
                >
                  Positions
                </Link>
              )}

              {hasRole(["Admin", "Recruiter", "HR"]) && (
                <Link
                  to="/candidates"
                  className="hover:bg-blue-700 px-3 py-2 rounded"
                >
                  Candidates
                </Link>
              )}

              {hasRole([
                "Admin",
                "Recruiter",
                "HR",
                "Interviewer",
                "Reviewer",
              ]) && (
                <Link
                  to="/screening"
                  className="hover:bg-blue-700 px-3 py-2 rounded"
                >
                  Screening
                </Link>
              )}

              {hasRole([
                "Admin",
                "Recruiter",
                "HR",
                "Interviewer",
                "Reviewer",
              ]) && (
                <Link
                  to="/interviews"
                  className="hover:bg-blue-700 px-3 py-2 rounded"
                >
                  Interviews
                </Link>
              )}

              {hasRole(["Admin"]) && (
                <Link
                  to="/users"
                  className="hover:bg-blue-700 px-3 py-2 rounded"
                >
                  Users
                </Link>
              )}

              {hasRole(["Candidate"]) && (
                <Link
                  to="/applications"
                  className="hover:bg-blue-700 px-3 py-2 rounded"
                >
                  My Applications
                </Link>
              )}
            </div>
          </div>

          <div className="flex items-center space-x-4">
            <span className="text-sm">
              {user.email} ({user.role})
            </span>
            <button
              onClick={handleLogout}
              className="bg-blue-700 hover:bg-blue-800 px-3 py-2 rounded text-sm"
            >
              Logout
            </button>
          </div>
        </div>
      </div>
    </nav>
  );
};

export default Navigation;
