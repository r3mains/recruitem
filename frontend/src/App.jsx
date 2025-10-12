import React from "react";
import { BrowserRouter as Router, Routes, Route } from "react-router-dom";
import { ApiProvider } from "./contexts/ApiContext";
import Login from "./pages/Login";
import Register from "./pages/Register";
import ForgotPassword from "./pages/ForgotPassword";
import ResetPassword from "./pages/ResetPassword";
import JobsDashboard from "./pages/JobsDashboard";
import PublicJobListings from "./pages/PublicJobListings";
import JobApplications from "./pages/JobApplications";

function App() {
  return (
    <ApiProvider>
      <Router>
        <div className="min-h-screen bg-gray-50">
          <Routes>
            <Route path="/login" element={<Login />} />
            <Route path="/register" element={<Register />} />
            <Route path="/forgot-password" element={<ForgotPassword />} />
            <Route path="/reset-password" element={<ResetPassword />} />
            <Route path="/jobs" element={<JobsDashboard />} />
            <Route path="/applications" element={<JobApplications />} />
            <Route path="/browse-jobs" element={<PublicJobListings />} />
            <Route path="/" element={<PublicJobListings />} />
          </Routes>
        </div>
      </Router>
    </ApiProvider>
  );
}

export default App;
