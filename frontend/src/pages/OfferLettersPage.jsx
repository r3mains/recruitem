import React, { useState, useEffect } from "react";
import { offerLetterAPI, applicationsAPI } from "../services/api";
import { useAuth } from "../contexts/AuthContext";
import toast, { Toaster } from "react-hot-toast";

const OfferLettersPage = () => {
  const [offers, setOffers] = useState([]);
  const [loading, setLoading] = useState(true);
  const [search, setSearch] = useState("");
  const [statusFilter, setStatusFilter] = useState("");
  const [page, setPage] = useState(1);
  const [selectedOffer, setSelectedOffer] = useState(null);
  const [showCreateModal, setShowCreateModal] = useState(false);
  const [showViewModal, setShowViewModal] = useState(false);
  const [showGenerateModal, setShowGenerateModal] = useState(false);
  const [applications, setApplications] = useState([]);
  const { hasRole } = useAuth();

  const limit = 10;

  useEffect(() => {
    loadInitialData();
  }, []);

  useEffect(() => {
    loadOffers();
  }, [page, statusFilter]);

  const loadInitialData = async () => {
    try {
      const appsData = await applicationsAPI.getAll();
      setApplications(appsData.applications || appsData || []);
    } catch (error) {
      console.error("Error loading applications:", error);
    }
  };

  const loadOffers = async () => {
    try {
      setLoading(true);
      const response = await offerLetterAPI.getAll();
      let filteredOffers = response.offers || response || [];

      if (statusFilter) {
        filteredOffers = filteredOffers.filter(
          (o) => o.status === statusFilter
        );
      }

      if (search) {
        filteredOffers = filteredOffers.filter((o) =>
          o.candidateName.toLowerCase().includes(search.toLowerCase())
        );
      }

      // Paginate
      const start = (page - 1) * limit;
      setOffers(filteredOffers.slice(start, start + limit));
    } catch (error) {
      console.error("Error loading offers:", error);
      toast.error("Failed to load offer letters");
    } finally {
      setLoading(false);
    }
  };

  const handleSearch = () => {
    setPage(1);
    loadOffers();
  };

  const clearFilters = () => {
    setSearch("");
    setStatusFilter("");
    setPage(1);
    setTimeout(loadOffers, 0);
  };

  const openViewModal = (offer) => {
    setSelectedOffer(offer);
    setShowViewModal(true);
  };

  const openGenerateModal = (offer) => {
    setSelectedOffer(offer);
    setShowGenerateModal(true);
  };

  const closeModals = () => {
    setShowCreateModal(false);
    setShowViewModal(false);
    setShowGenerateModal(false);
    setSelectedOffer(null);
  };

  const handleDelete = async (offerId) => {
    if (!window.confirm("Are you sure you want to delete this offer letter?"))
      return;

    try {
      await offerLetterAPI.delete(offerId);
      toast.success("Offer letter deleted successfully");
      loadOffers();
    } catch (error) {
      console.error("Error deleting offer:", error);
      toast.error("Failed to delete offer letter");
    }
  };

  const handleDownload = async (offerId) => {
    try {
      const offer = offers.find((o) => o.id === offerId);
      if (!offer || !offer.generatedPdfPath) {
        toast.error("PDF not yet generated. Please generate first.");
        return;
      }

      // Create download link
      const link = document.createElement("a");
      link.href = `http://localhost:5153/api/v1/offerletter/${offerId}/download`;
      link.setAttribute("Authorization", `Bearer ${localStorage.getItem("token")}`);
      link.download = `Offer_Letter_${offer.candidateName.replace(/ /g, "_")}.pdf`;
      document.body.appendChild(link);
      link.click();
      document.body.removeChild(link);
      toast.success("Offer letter downloaded");
    } catch (error) {
      console.error("Error downloading:", error);
      toast.error("Failed to download offer letter");
    }
  };

  if (!hasRole(["Admin", "Recruiter", "HR"])) {
    return (
      <div className="p-6">
        <div className="text-center">
          <h1 className="text-2xl font-bold text-red-600">Access Denied</h1>
          <p className="text-gray-600">
            You don't have permission to manage offer letters.
          </p>
        </div>
      </div>
    );
  }

  const getStatusBadge = (status) => {
    const colors = {
      Pending: "bg-yellow-100 text-yellow-800",
      Accepted: "bg-green-100 text-green-800",
      Rejected: "bg-red-100 text-red-800",
      Draft: "bg-gray-100 text-gray-800",
    };
    return colors[status] || "bg-gray-100 text-gray-800";
  };

  return (
    <div className="p-6">
      <Toaster position="top-right" />

      <div className="mb-6 flex justify-between items-center">
        <h1 className="text-2xl font-bold">Offer Letters</h1>
        <button
          onClick={() => setShowCreateModal(true)}
          className="bg-blue-600 text-white px-4 py-2 rounded hover:bg-blue-700"
        >
          + Create Offer Letter
        </button>
      </div>

      {/* Search/Filter */}
      <div className="bg-white p-4 rounded-lg shadow mb-6">
        <div className="grid grid-cols-1 md:grid-cols-4 gap-4">
          <div className="md:col-span-2">
            <label className="block text-sm font-medium mb-2">
              Search Candidate
            </label>
            <input
              type="text"
              value={search}
              onChange={(e) => setSearch(e.target.value)}
              placeholder="Candidate name..."
              className="w-full border rounded px-3 py-2"
              onKeyPress={(e) => e.key === "Enter" && handleSearch()}
            />
          </div>
          <div>
            <label className="block text-sm font-medium mb-2">Status</label>
            <select
              value={statusFilter}
              onChange={(e) => setStatusFilter(e.target.value)}
              className="w-full border rounded px-3 py-2"
            >
              <option value="">All Status</option>
              <option value="Draft">Draft</option>
              <option value="Pending">Pending</option>
              <option value="Accepted">Accepted</option>
              <option value="Rejected">Rejected</option>
            </select>
          </div>
          <div className="flex items-end gap-2">
            <button
              onClick={handleSearch}
              className="bg-blue-600 text-white px-4 py-2 rounded hover:bg-blue-700"
            >
              Search
            </button>
            <button
              onClick={clearFilters}
              className="bg-gray-500 text-white px-4 py-2 rounded hover:bg-gray-600"
            >
              Clear
            </button>
          </div>
        </div>
      </div>

      {loading ? (
        <div className="text-center py-8">
          <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-blue-600 mx-auto"></div>
        </div>
      ) : (
        <>
          <div className="bg-white shadow rounded-lg overflow-hidden">
            <table className="min-w-full divide-y divide-gray-200">
              <thead className="bg-gray-50">
                <tr>
                  <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">
                    Candidate
                  </th>
                  <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">
                    Position
                  </th>
                  <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">
                    Salary
                  </th>
                  <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">
                    Status
                  </th>
                  <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">
                    Joining Date
                  </th>
                  <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">
                    Offer Date
                  </th>
                  <th className="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase">
                    Actions
                  </th>
                </tr>
              </thead>
              <tbody className="bg-white divide-y divide-gray-200">
                {offers.length === 0 ? (
                  <tr>
                    <td colSpan="7" className="px-6 py-8 text-center text-gray-500">
                      No offer letters found
                    </td>
                  </tr>
                ) : (
                  offers.map((offer) => (
                    <tr key={offer.id} className="hover:bg-gray-50">
                      <td className="px-6 py-4">
                        <div className="font-medium text-gray-900">
                          {offer.candidateName}
                        </div>
                        <div className="text-xs text-gray-500">
                          {offer.candidateEmail}
                        </div>
                      </td>
                      <td className="px-6 py-4 text-sm text-gray-500">
                        {offer.jobTitle}
                      </td>
                      <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                        ${offer.salary.toLocaleString()}
                      </td>
                      <td className="px-6 py-4 whitespace-nowrap">
                        <span
                          className={`px-2 py-1 text-xs rounded-full ${getStatusBadge(
                            offer.status
                          )}`}
                        >
                          {offer.status}
                        </span>
                      </td>
                      <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                        {offer.joiningDate
                          ? new Date(offer.joiningDate).toLocaleDateString()
                          : "Not set"}
                      </td>
                      <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                        {new Date(offer.offerDate).toLocaleDateString()}
                      </td>
                      <td className="px-6 py-4 whitespace-nowrap text-right text-sm font-medium">
                        <button
                          onClick={() => openViewModal(offer)}
                          className="text-blue-600 hover:text-blue-900 mr-3"
                        >
                          View
                        </button>
                        {!offer.generatedPdfPath && (
                          <button
                            onClick={() => openGenerateModal(offer)}
                            className="text-green-600 hover:text-green-900 mr-3"
                          >
                            Generate
                          </button>
                        )}
                        {offer.generatedPdfPath && (
                          <button
                            onClick={() => handleDownload(offer.id)}
                            className="text-purple-600 hover:text-purple-900 mr-3"
                          >
                            Download
                          </button>
                        )}
                        <button
                          onClick={() => handleDelete(offer.id)}
                          className="text-red-600 hover:text-red-900"
                        >
                          Delete
                        </button>
                      </td>
                    </tr>
                  ))
                )}
              </tbody>
            </table>
          </div>

          {/* Pagination */}
          <div className="flex justify-center mt-6 gap-2">
            <button
              onClick={() => setPage(Math.max(1, page - 1))}
              disabled={page === 1}
              className="px-4 py-2 border rounded disabled:opacity-50 disabled:cursor-not-allowed hover:bg-gray-50"
            >
              Previous
            </button>
            <span className="px-4 py-2">Page {page}</span>
            <button
              onClick={() => setPage(page + 1)}
              disabled={offers.length < limit}
              className="px-4 py-2 border rounded disabled:opacity-50 disabled:cursor-not-allowed hover:bg-gray-50"
            >
              Next
            </button>
          </div>
        </>
      )}

      {/* Modals */}
      {showCreateModal && (
        <CreateOfferModal
          applications={applications}
          onClose={closeModals}
          onSuccess={() => {
            closeModals();
            loadOffers();
          }}
        />
      )}

      {showViewModal && (
        <ViewOfferModal
          offer={selectedOffer}
          onClose={closeModals}
          onGenerate={() => openGenerateModal(selectedOffer)}
        />
      )}

      {showGenerateModal && (
        <GenerateOfferModal
          offer={selectedOffer}
          onClose={closeModals}
          onSuccess={() => {
            closeModals();
            loadOffers();
          }}
        />
      )}
    </div>
  );
};

// Create Offer Modal
const CreateOfferModal = ({ applications, onClose, onSuccess }) => {
  const [formData, setFormData] = useState({
    jobApplicationId: "",
    joiningDate: "",
    salary: "",
    benefits: "",
    additionalTerms: "",
    expiryDate: "",
  });
  const [saving, setSaving] = useState(false);

  const handleSubmit = async (e) => {
    e.preventDefault();
    setSaving(true);

    try {
      const payload = {
        jobApplicationId: formData.jobApplicationId,
        joiningDate: new Date(formData.joiningDate),
        salary: parseFloat(formData.salary),
        benefits: formData.benefits,
        additionalTerms: formData.additionalTerms,
        expiryDate: formData.expiryDate
          ? new Date(formData.expiryDate)
          : null,
      };

      await offerLetterAPI.create(payload);
      toast.success("Offer letter created successfully");
      onSuccess();
    } catch (error) {
      console.error("Error creating offer:", error);
      toast.error("Failed to create offer letter");
    } finally {
      setSaving(false);
    }
  };

  return (
    <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
      <div className="bg-white p-6 rounded-lg w-11/12 md:w-2/3 max-h-screen overflow-y-auto">
        <div className="flex justify-between items-center mb-4">
          <h2 className="text-xl font-bold">Create Offer Letter</h2>
          <button
            onClick={onClose}
            className="text-gray-500 hover:text-gray-700 text-2xl"
          >
            ×
          </button>
        </div>

        <form onSubmit={handleSubmit} className="space-y-4">
          <div>
            <label className="block text-sm font-medium mb-1">
              Job Application *
            </label>
            <select
              value={formData.jobApplicationId}
              onChange={(e) =>
                setFormData({ ...formData, jobApplicationId: e.target.value })
              }
              required
              className="w-full border rounded px-3 py-2"
            >
              <option value="">Select Application</option>
              {applications.map((app) => (
                <option key={app.id} value={app.id}>
                  {app.candidate?.fullName} - {app.job?.title}
                </option>
              ))}
            </select>
          </div>

          <div className="grid grid-cols-2 gap-4">
            <div>
              <label className="block text-sm font-medium mb-1">
                Joining Date *
              </label>
              <input
                type="date"
                value={formData.joiningDate}
                onChange={(e) =>
                  setFormData({ ...formData, joiningDate: e.target.value })
                }
                required
                className="w-full border rounded px-3 py-2"
              />
            </div>

            <div>
              <label className="block text-sm font-medium mb-1">
                Annual Salary *
              </label>
              <input
                type="number"
                value={formData.salary}
                onChange={(e) =>
                  setFormData({ ...formData, salary: e.target.value })
                }
                required
                min="0"
                step="0.01"
                placeholder="0.00"
                className="w-full border rounded px-3 py-2"
              />
            </div>
          </div>

          <div className="grid grid-cols-2 gap-4">
            <div>
              <label className="block text-sm font-medium mb-1">
                Expiry Date
              </label>
              <input
                type="date"
                value={formData.expiryDate}
                onChange={(e) =>
                  setFormData({ ...formData, expiryDate: e.target.value })
                }
                className="w-full border rounded px-3 py-2"
              />
            </div>
          </div>

          <div>
            <label className="block text-sm font-medium mb-1">Benefits</label>
            <textarea
              value={formData.benefits}
              onChange={(e) =>
                setFormData({ ...formData, benefits: e.target.value })
              }
              rows="3"
              placeholder="List benefits (health insurance, 401k, etc.)"
              className="w-full border rounded px-3 py-2"
            />
          </div>

          <div>
            <label className="block text-sm font-medium mb-1">
              Additional Terms
            </label>
            <textarea
              value={formData.additionalTerms}
              onChange={(e) =>
                setFormData({ ...formData, additionalTerms: e.target.value })
              }
              rows="3"
              placeholder="Any additional terms and conditions..."
              className="w-full border rounded px-3 py-2"
            />
          </div>

          <div className="flex justify-end gap-2 pt-4 border-t">
            <button
              type="button"
              onClick={onClose}
              className="px-4 py-2 border rounded hover:bg-gray-50"
            >
              Cancel
            </button>
            <button
              type="submit"
              disabled={saving}
              className="bg-blue-600 text-white px-4 py-2 rounded hover:bg-blue-700 disabled:opacity-50"
            >
              {saving ? "Creating..." : "Create Offer"}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
};

// View Offer Modal
const ViewOfferModal = ({ offer, onClose, onGenerate }) => {
  return (
    <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
      <div className="bg-white p-6 rounded-lg w-11/12 md:w-2/3 max-h-screen overflow-y-auto">
        <div className="flex justify-between items-center mb-4">
          <h2 className="text-xl font-bold">Offer Letter Details</h2>
          <button
            onClick={onClose}
            className="text-gray-500 hover:text-gray-700 text-2xl"
          >
            ×
          </button>
        </div>

        <div className="space-y-6">
          <div>
            <h3 className="text-lg font-semibold mb-3">Candidate Information</h3>
            <div className="grid grid-cols-2 gap-4 text-sm">
              <div>
                <span className="font-medium">Name:</span> {offer.candidateName}
              </div>
              <div>
                <span className="font-medium">Email:</span>{" "}
                {offer.candidateEmail}
              </div>
            </div>
          </div>

          <div>
            <h3 className="text-lg font-semibold mb-3">Position Information</h3>
            <div className="grid grid-cols-2 gap-4 text-sm">
              <div>
                <span className="font-medium">Job Title:</span>{" "}
                {offer.jobTitle}
              </div>
              <div>
                <span className="font-medium">Company:</span>{" "}
                {offer.companyName}
              </div>
            </div>
          </div>

          <div>
            <h3 className="text-lg font-semibold mb-3">Offer Details</h3>
            <div className="grid grid-cols-2 gap-4 text-sm">
              <div>
                <span className="font-medium">Annual Salary:</span> $
                {offer.salary.toLocaleString()}
              </div>
              <div>
                <span className="font-medium">Status:</span>{" "}
                <span
                  className={`px-2 py-1 text-xs rounded ${
                    offer.status === "Accepted"
                      ? "bg-green-100 text-green-800"
                      : offer.status === "Rejected"
                      ? "bg-red-100 text-red-800"
                      : "bg-yellow-100 text-yellow-800"
                  }`}
                >
                  {offer.status}
                </span>
              </div>
              <div>
                <span className="font-medium">Offer Date:</span>{" "}
                {new Date(offer.offerDate).toLocaleDateString()}
              </div>
              <div>
                <span className="font-medium">Joining Date:</span>{" "}
                {offer.joiningDate
                  ? new Date(offer.joiningDate).toLocaleDateString()
                  : "Not set"}
              </div>
              {offer.expiryDate && (
                <div>
                  <span className="font-medium">Expiry Date:</span>{" "}
                  {new Date(offer.expiryDate).toLocaleDateString()}
                </div>
              )}
            </div>
          </div>

          {offer.benefits && (
            <div>
              <h3 className="text-lg font-semibold mb-3">Benefits</h3>
              <p className="text-sm text-gray-700 whitespace-pre-wrap">
                {offer.benefits}
              </p>
            </div>
          )}

          {offer.additionalTerms && (
            <div>
              <h3 className="text-lg font-semibold mb-3">Additional Terms</h3>
              <p className="text-sm text-gray-700 whitespace-pre-wrap">
                {offer.additionalTerms}
              </p>
            </div>
          )}

          {offer.acceptedDate && (
            <div className="bg-green-50 p-4 rounded">
              <p className="text-sm text-green-800">
                <span className="font-medium">Accepted on:</span>{" "}
                {new Date(offer.acceptedDate).toLocaleDateString()}
              </p>
            </div>
          )}

          {offer.rejectedDate && (
            <div className="bg-red-50 p-4 rounded">
              <p className="text-sm text-red-800">
                <span className="font-medium">Rejected on:</span>{" "}
                {new Date(offer.rejectedDate).toLocaleDateString()}
              </p>
              {offer.rejectionReason && (
                <p className="text-sm text-red-800 mt-2">
                  <span className="font-medium">Reason:</span>{" "}
                  {offer.rejectionReason}
                </p>
              )}
            </div>
          )}
        </div>

        <div className="flex justify-end gap-2 pt-4 border-t mt-6">
          {!offer.generatedPdfPath && (
            <button
              onClick={onGenerate}
              className="px-4 py-2 bg-green-600 text-white rounded hover:bg-green-700"
            >
              Generate PDF
            </button>
          )}
          <button
            onClick={onClose}
            className="px-4 py-2 bg-gray-500 text-white rounded hover:bg-gray-600"
          >
            Close
          </button>
        </div>
      </div>
    </div>
  );
};

// Generate Offer Modal
const GenerateOfferModal = ({ offer, onClose, onSuccess }) => {
  const [formData, setFormData] = useState({
    companyName: "",
    companyAddress: "",
    signatoryName: "",
    signatoryDesignation: "",
  });
  const [saving, setSaving] = useState(false);

  const handleSubmit = async (e) => {
    e.preventDefault();
    setSaving(true);

    try {
      const payload = {
        offerLetterId: offer.id,
        ...formData,
      };

      await offerLetterAPI.generate(offer.id, formData);
      toast.success("Offer letter PDF generated successfully");
      onSuccess();
    } catch (error) {
      console.error("Error generating PDF:", error);
      toast.error("Failed to generate offer letter PDF");
    } finally {
      setSaving(false);
    }
  };

  return (
    <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
      <div className="bg-white p-6 rounded-lg w-11/12 md:w-1/2">
        <div className="flex justify-between items-center mb-4">
          <h2 className="text-xl font-bold">Generate PDF</h2>
          <button
            onClick={onClose}
            className="text-gray-500 hover:text-gray-700 text-2xl"
          >
            ×
          </button>
        </div>

        <div className="mb-4 p-4 bg-gray-50 rounded">
          <p className="text-sm">
            <span className="font-medium">Candidate:</span>{" "}
            {offer.candidateName}
          </p>
          <p className="text-sm">
            <span className="font-medium">Position:</span> {offer.jobTitle}
          </p>
        </div>

        <form onSubmit={handleSubmit} className="space-y-4">
          <div>
            <label className="block text-sm font-medium mb-1">
              Company Name
            </label>
            <input
              type="text"
              value={formData.companyName}
              onChange={(e) =>
                setFormData({ ...formData, companyName: e.target.value })
              }
              placeholder="Enter company name..."
              className="w-full border rounded px-3 py-2"
            />
          </div>

          <div>
            <label className="block text-sm font-medium mb-1">
              Company Address
            </label>
            <textarea
              value={formData.companyAddress}
              onChange={(e) =>
                setFormData({ ...formData, companyAddress: e.target.value })
              }
              rows="3"
              placeholder="Enter company address..."
              className="w-full border rounded px-3 py-2"
            />
          </div>

          <div>
            <label className="block text-sm font-medium mb-1">
              Signatory Name
            </label>
            <input
              type="text"
              value={formData.signatoryName}
              onChange={(e) =>
                setFormData({ ...formData, signatoryName: e.target.value })
              }
              placeholder="Name of person signing..."
              className="w-full border rounded px-3 py-2"
            />
          </div>

          <div>
            <label className="block text-sm font-medium mb-1">
              Signatory Designation
            </label>
            <input
              type="text"
              value={formData.signatoryDesignation}
              onChange={(e) =>
                setFormData({
                  ...formData,
                  signatoryDesignation: e.target.value,
                })
              }
              placeholder="e.g., HR Manager, Director"
              className="w-full border rounded px-3 py-2"
            />
          </div>

          <div className="flex justify-end gap-2 pt-4 border-t">
            <button
              type="button"
              onClick={onClose}
              className="px-4 py-2 border rounded hover:bg-gray-50"
            >
              Cancel
            </button>
            <button
              type="submit"
              disabled={saving}
              className="bg-green-600 text-white px-4 py-2 rounded hover:bg-green-700 disabled:opacity-50"
            >
              {saving ? "Generating..." : "Generate PDF"}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
};

export default OfferLettersPage;
