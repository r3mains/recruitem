import React, { useState } from "react";
import { Link, useNavigate } from "react-router-dom";
import { useAuth } from "../contexts/AuthContext";

const Navigation = () => {
  const { user, logout, hasRole } = useAuth();
  const navigate = useNavigate();
  const [openDropdown, setOpenDropdown] = useState(null);

  const handleLogout = () => {
    logout();
    navigate("/login");
  };

  const toggleDropdown = (name) => {
    setOpenDropdown(openDropdown === name ? null : name);
  };

  if (!user) return null;

  return (
    <nav className="bg-blue-600 text-white shadow-lg">
      <div className="max-w-7xl mx-auto px-4">
        <div className="flex justify-between items-center h-16">
          <div className="flex items-center space-x-4">
            <Link to="/" className="text-xl font-bold">
              RecruiteM
            </Link>

            <div className="flex space-x-1">
              {/* Dashboard - Only for non-candidate roles */}
              {hasRole(["Admin", "Recruiter", "HR", "Interviewer", "Reviewer"]) && (
                <Link
                  to="/dashboard"
                  className="hover:bg-blue-700 px-3 py-2 rounded text-sm"
                >
                  Dashboard
                </Link>
              )}

              {/* Recruitment Dropdown */}
              {hasRole(["Admin", "Recruiter", "HR", "Interviewer", "Reviewer"]) && (
                <div className="relative">
                  <button
                    onClick={() => toggleDropdown("recruitment")}
                    className="hover:bg-blue-700 px-3 py-2 rounded text-sm flex items-center"
                  >
                    Recruitment
                    <svg className="w-4 h-4 ml-1" fill="currentColor" viewBox="0 0 20 20">
                      <path fillRule="evenodd" d="M5.293 7.293a1 1 0 011.414 0L10 10.586l3.293-3.293a1 1 0 111.414 1.414l-4 4a1 1 0 01-1.414 0l-4-4a1 1 0 010-1.414z" clipRule="evenodd" />
                    </svg>
                  </button>
                  {openDropdown === "recruitment" && (
                    <div className="absolute left-0 mt-2 w-48 bg-white text-gray-800 rounded-md shadow-lg z-50">
                      <Link to="/jobs" className="block px-4 py-2 hover:bg-gray-100">Jobs</Link>
                      <Link to="/positions" className="block px-4 py-2 hover:bg-gray-100">Positions</Link>
                      {hasRole(["Admin", "Recruiter", "HR"]) && (
                        <Link to="/candidates" className="block px-4 py-2 hover:bg-gray-100">Candidates</Link>
                      )}
                      <Link to="/screening" className="block px-4 py-2 hover:bg-gray-100">Screening</Link>
                      <Link to="/interviews" className="block px-4 py-2 hover:bg-gray-100">Interviews</Link>
                      {hasRole(["Admin", "HR", "Recruiter"]) && (
                        <Link to="/offer-letters" className="block px-4 py-2 hover:bg-gray-100">Offer Letters</Link>
                      )}
                    </div>
                  )}
                </div>
              )}

              {/* Tools Dropdown */}
              {hasRole(["Admin", "Recruiter", "HR"]) && (
                <div className="relative">
                  <button
                    onClick={() => toggleDropdown("tools")}
                    className="hover:bg-blue-700 px-3 py-2 rounded text-sm flex items-center"
                  >
                    Tools
                    <svg className="w-4 h-4 ml-1" fill="currentColor" viewBox="0 0 20 20">
                      <path fillRule="evenodd" d="M5.293 7.293a1 1 0 011.414 0L10 10.586l3.293-3.293a1 1 0 111.414 1.414l-4 4a1 1 0 01-1.414 0l-4-4a1 1 0 010-1.414z" clipRule="evenodd" />
                    </svg>
                  </button>
                  {openDropdown === "tools" && (
                    <div className="absolute left-0 mt-2 w-48 bg-white text-gray-800 rounded-md shadow-lg z-50">
                      <Link to="/resume-parser" className="block px-4 py-2 hover:bg-gray-100">Resume Parser</Link>
                      {hasRole(["Admin", "HR"]) && (
                        <Link to="/email-templates" className="block px-4 py-2 hover:bg-gray-100">Email Templates</Link>
                      )}
                      <Link to="/documents" className="block px-4 py-2 hover:bg-gray-100">Documents</Link>
                      {hasRole(["Admin", "HR"]) && (
                        <Link to="/events" className="block px-4 py-2 hover:bg-gray-100">Events</Link>
                      )}
                      {hasRole(["Admin", "HR"]) && (
                        <Link to="/verifications" className="block px-4 py-2 hover:bg-gray-100">Verifications</Link>
                      )}
                      <Link to="/export" className="block px-4 py-2 hover:bg-gray-100">Export</Link>
                    </div>
                  )}
                </div>
              )}

              {/* Analytics */}
              {hasRole(["Admin", "HR"]) && (
                <Link
                  to="/reports"
                  className="hover:bg-blue-700 px-3 py-2 rounded text-sm"
                >
                  Reports
                </Link>
              )}

              {/* Settings Dropdown */}
              {hasRole(["Admin", "HR"]) && (
                <div className="relative">
                  <button
                    onClick={() => toggleDropdown("settings")}
                    className="hover:bg-blue-700 px-3 py-2 rounded text-sm flex items-center"
                  >
                    Settings
                    <svg className="w-4 h-4 ml-1" fill="currentColor" viewBox="0 0 20 20">
                      <path fillRule="evenodd" d="M5.293 7.293a1 1 0 011.414 0L10 10.586l3.293-3.293a1 1 0 111.414 1.414l-4 4a1 1 0 01-1.414 0l-4-4a1 1 0 010-1.414z" clipRule="evenodd" />
                    </svg>
                  </button>
                  {openDropdown === "settings" && (
                    <div className="absolute left-0 mt-2 w-48 bg-white text-gray-800 rounded-md shadow-lg z-50">
                      <Link to="/skills" className="block px-4 py-2 hover:bg-gray-100">Skills</Link>
                      {hasRole(["Admin"]) && (
                        <>
                          <Link to="/qualifications" className="block px-4 py-2 hover:bg-gray-100">Qualifications</Link>
                          <Link to="/job-types" className="block px-4 py-2 hover:bg-gray-100">Job Types</Link>
                          <Link to="/users" className="block px-4 py-2 hover:bg-gray-100">Users</Link>
                        </>
                      )}
                    </div>
                  )}
                </div>
              )}

              {/* Candidate view */}
              {hasRole(["Candidate"]) && (
                <>
                  <Link
                    to="/browse-jobs"
                    className="hover:bg-blue-700 px-3 py-2 rounded text-sm"
                  >
                    Browse Jobs
                  </Link>
                  <Link
                    to="/applications"
                    className="hover:bg-blue-700 px-3 py-2 rounded text-sm"
                  >
                    My Applications
                  </Link>
                </>
              )}
            </div>
          </div>

          <div className="flex items-center space-x-4">
            <Link to="/profile" className="hover:bg-blue-700 px-3 py-2 rounded text-sm">
              {user.email}
            </Link>
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
