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
import JobsPage from "./pages/JobsPage";
import PublicJobListings from "./pages/PublicJobListings";
import JobDetail from "./pages/JobDetail";
import JobApplications from "./pages/JobApplications";
import PositionsPage from "./pages/PositionsPage";
import CandidatesPage from "./pages/CandidatesPage";
import CandidateProfilePage from "./pages/CandidateProfilePage";
import ScreeningPage from "./pages/ScreeningPage";
import InterviewsPage from "./pages/InterviewsPage";
import LandingPage from "./pages/LandingPage";
import ResumeParsingPage from "./pages/ResumeParsingPage";
import EmailTemplatesPage from "./pages/EmailTemplatesPage";
import DocumentsPage from "./pages/DocumentsPage";
import EventsPage from "./pages/EventsPage";
import ReportsPage from "./pages/ReportsPage";
import VerificationPage from "./pages/VerificationPage";
import SkillsPage from "./pages/SkillsPage";
import QualificationsPage from "./pages/QualificationsPage";
import UserManagementPage from "./pages/UserManagementPage";
import JobTypesPage from "./pages/JobTypesPage";
import ProfilePage from "./pages/ProfilePage";
import ExportPage from "./pages/ExportPage";
import OfferLettersPage from "./pages/OfferLettersPage";

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
              <Route path="/" element={<LandingPage />} />

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
                    <JobsPage />
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
                    <ScreeningPage />
                  </ProtectedRoute>
                }
              />

              <Route
                path="/interviews"
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
                    <InterviewsPage />
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

              <Route
                path="/resume-parser"
                element={
                  <ProtectedRoute requiredRoles={["Admin", "Recruiter", "HR"]}>
                    <ResumeParsingPage />
                  </ProtectedRoute>
                }
              />

              <Route
                path="/email-templates"
                element={
                  <ProtectedRoute requiredRoles={["Admin", "HR"]}>
                    <EmailTemplatesPage />
                  </ProtectedRoute>
                }
              />

              <Route
                path="/documents"
                element={
                  <ProtectedRoute requiredRoles={["Admin", "HR", "Recruiter"]}>
                    <DocumentsPage />
                  </ProtectedRoute>
                }
              />

              <Route
                path="/events"
                element={
                  <ProtectedRoute requiredRoles={["Admin", "HR"]}>
                    <EventsPage />
                  </ProtectedRoute>
                }
              />

              <Route
                path="/reports"
                element={
                  <ProtectedRoute requiredRoles={["Admin", "HR"]}>
                    <ReportsPage />
                  </ProtectedRoute>
                }
              />

              <Route
                path="/verifications"
                element={
                  <ProtectedRoute requiredRoles={["Admin", "HR"]}>
                    <VerificationPage />
                  </ProtectedRoute>
                }
              />

              <Route
                path="/skills"
                element={
                  <ProtectedRoute requiredRoles={["Admin", "HR"]}>
                    <SkillsPage />
                  </ProtectedRoute>
                }
              />

              <Route
                path="/qualifications"
                element={
                  <ProtectedRoute requiredRoles={["Admin"]}>
                    <QualificationsPage />
                  </ProtectedRoute>
                }
              />

              <Route
                path="/users"
                element={
                  <ProtectedRoute requiredRoles={["Admin"]}>
                    <UserManagementPage />
                  </ProtectedRoute>
                }
              />

              <Route
                path="/job-types"
                element={
                  <ProtectedRoute requiredRoles={["Admin"]}>
                    <JobTypesPage />
                  </ProtectedRoute>
                }
              />

              <Route
                path="/profile"
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
                    <ProfilePage />
                  </ProtectedRoute>
                }
              />

              <Route
                path="/export"
                element={
                  <ProtectedRoute requiredRoles={["Admin", "HR", "Recruiter"]}>
                    <ExportPage />
                  </ProtectedRoute>
                }
              />

              <Route
                path="/offer-letters"
                element={
                  <ProtectedRoute requiredRoles={["Admin", "HR", "Recruiter"]}>
                    <OfferLettersPage />
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
