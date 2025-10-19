import React from "react";
import { BrowserRouter as Router, Routes, Route } from "react-router-dom";
import { ApiProvider } from "./contexts/ApiContext";
import { AuthProvider } from "./contexts/AuthContext";
import ProtectedRoute from "./components/ProtectedRoute";
import Navigation from "./components/Navigation";
import Login from "./pages/Login";
import Register from "./pages/Register";
import ForgotPassword from "./pages/ForgotPassword";
import ResetPassword from "./pages/ResetPassword";
import Dashboard from "./pages/Dashboard";
import JobsDashboard from "./pages/JobsDashboard";
import PublicJobListings from "./pages/PublicJobListings";
import JobDetail from "./pages/JobDetail";
import JobApplications from "./pages/JobApplications";
import PositionsPage from "./pages/PositionsPage";
import CandidatesPage from "./pages/CandidatesPage";
import CandidateProfilePage from "./pages/CandidateProfilePage";
import ApplicationsPage from "./pages/ApplicationsPage";

function App() {
  return (
    <ApiProvider>
      <AuthProvider>
        <Router>
          <div className="min-h-screen bg-gray-50">
            <Navigation />
            <Routes>
              <Route path="/login" element={<Login />} />
              <Route path="/register" element={<Register />} />
              <Route path="/forgot-password" element={<ForgotPassword />} />
              <Route path="/reset-password" element={<ResetPassword />} />
              <Route path="/browse-jobs" element={<PublicJobListings />} />
              <Route path="/job/:id" element={<JobDetail />} />
              <Route path="/" element={<PublicJobListings />} />

              <Route
                path="/dashboard"
                element={
                  <ProtectedRoute
                    requiredRoles={[
                      "Admin",
                      "Recruiter",
                      "HR",
                      "Interviewer",
                      "Reviewer",
                      "Candidate",
                    ]}
                  >
                    <Dashboard />
                  </ProtectedRoute>
                }
              />

              <Route
                path="/jobs"
                element={
                  <ProtectedRoute
                    requiredRoles={[
                      "Admin",
                      "Recruiter",
                      "HR",
                      "Interviewer",
                      "Reviewer",
                    ]}
                  >
                    <JobsDashboard />
                  </ProtectedRoute>
                }
              />

              <Route
                path="/positions"
                element={
                  <ProtectedRoute
                    requiredRoles={[
                      "Admin",
                      "Recruiter",
                      "HR",
                      "Interviewer",
                      "Reviewer",
                    ]}
                  >
                    <PositionsPage />
                  </ProtectedRoute>
                }
              />

              <Route
                path="/candidates"
                element={
                  <ProtectedRoute requiredRoles={["Admin", "Recruiter", "HR"]}>
                    <CandidatesPage />
                  </ProtectedRoute>
                }
              />

              <Route
                path="/screening"
                element={
                  <ProtectedRoute
                    requiredRoles={[
                      "Admin",
                      "Recruiter",
                      "HR",
                      "Interviewer",
                      "Reviewer",
                    ]}
                  >
                    <ApplicationsPage />
                  </ProtectedRoute>
                }
              />

              <Route
                path="/profile"
                element={
                  <ProtectedRoute requiredRoles={["Candidate"]}>
                    <CandidateProfilePage />
                  </ProtectedRoute>
                }
              />

              <Route
                path="/applications"
                element={
                  <ProtectedRoute requiredRoles={["Candidate"]}>
                    <JobApplications />
                  </ProtectedRoute>
                }
              />
            </Routes>
          </div>
        </Router>
      </AuthProvider>
    </ApiProvider>
  );
}

export default App;
