import { useState, useEffect } from 'react';
import { eventAPI, candidatesAPI } from '../services/api';
import { useAuth } from '../contexts/AuthContext';
import toast from 'react-hot-toast';
import LoadingSpinner from '../components/LoadingSpinner';

const EVENT_TYPES = ['Recruitment Drive', 'Campus Drive', 'Webinar', 'Workshop', 'Assessment', 'Interview Round'];

const CANDIDATE_STATUS = [
  { id: '70000000-0000-0000-0000-000000000001', name: 'Registered' },
  { id: '70000000-0000-0000-0000-000000000002', name: 'Checked In' },
  { id: '70000000-0000-0000-0000-000000000003', name: 'Interviewed' },
  { id: '70000000-0000-0000-0000-000000000004', name: 'Selected' },
  { id: '70000000-0000-0000-0000-000000000005', name: 'Rejected' }
];

export default function EventsPage() {
  const [events, setEvents] = useState([]);
  const [filteredEvents, setFilteredEvents] = useState([]);
  const [candidates, setCandidates] = useState([]);
  const [loading, setLoading] = useState(true);
  const [searchTerm, setSearchTerm] = useState('');
  const [selectedType, setSelectedType] = useState('');

  // Pagination
  const [currentPage, setCurrentPage] = useState(1);
  const itemsPerPage = 10;

  // Modal states
  const [showCreateModal, setShowCreateModal] = useState(false);
  const [showViewModal, setShowViewModal] = useState(false);
  const [showCandidatesModal, setShowCandidatesModal] = useState(false);
  const [selectedEvent, setSelectedEvent] = useState(null);
  const [eventCandidates, setEventCandidates] = useState([]);
  const [loadingCandidates, setLoadingCandidates] = useState(false);

  // Form data
  const [formData, setFormData] = useState({
    name: '',
    type: 'Recruitment Drive',
    location: '',
    date: ''
  });

  // Register candidate form
  const [registerForm, setRegisterForm] = useState({
    candidateId: '',
    statusId: '70000000-0000-0000-0000-000000000001'
  });

  const { user } = useAuth();
  const canManage = user?.role === 'Admin' || user?.role === 'HR';

  useEffect(() => {
    fetchData();
  }, []);

  // Filter events
  useEffect(() => {
    let filtered = events;

    if (searchTerm) {
      filtered = filtered.filter(
        (e) =>
          e.name.toLowerCase().includes(searchTerm.toLowerCase()) ||
          e.location?.toLowerCase().includes(searchTerm.toLowerCase())
      );
    }

    if (selectedType) {
      filtered = filtered.filter((e) => e.type === selectedType);
    }

    setFilteredEvents(filtered);
    setCurrentPage(1);
  }, [events, searchTerm, selectedType]);

  const fetchData = async () => {
    try {
      setLoading(true);
      const [eventsData, candData] = await Promise.all([eventAPI.getAll(), candidatesAPI.search()]);
      setEvents(eventsData || []);
      // candidatesAPI.search returns candidates array or object with candidates property
      setCandidates(Array.isArray(candData) ? candData : (candData?.candidates || []));
    } catch (err) {
      toast.error('Failed to load events');
      console.error(err);
    } finally {
      setLoading(false);
    }
  };

  const handleCreateNew = () => {
    setFormData({
      name: '',
      type: 'Recruitment Drive',
      location: '',
      date: ''
    });
    setSelectedEvent(null);
    setShowCreateModal(true);
  };

  const handleEditEvent = (event) => {
    setFormData({
      name: event.name,
      type: event.type || 'Recruitment Drive',
      location: event.location || '',
      date: event.date.split('T')[0]
    });
    setSelectedEvent(event);
    setShowCreateModal(true);
  };

  const handleSubmit = async (e) => {
    e.preventDefault();

    if (!formData.name || !formData.date) {
      toast.error('Please fill in required fields');
      return;
    }

    try {
      const eventData = {
        ...formData,
        date: new Date(formData.date).toISOString(),
        createdBy: user.id
      };

      if (selectedEvent) {
        await eventAPI.update(selectedEvent.id, {
          name: formData.name,
          type: formData.type,
          location: formData.location,
          date: eventData.date
        });
        toast.success('Event updated successfully');
      } else {
        await eventAPI.create(eventData);
        toast.success('Event created successfully');
      }

      await fetchData();
      setShowCreateModal(false);
      setSelectedEvent(null);
    } catch (err) {
      toast.error(err.response?.data?.message || 'Failed to save event');
    }
  };

  const handleViewEvent = (event) => {
    setSelectedEvent(event);
    setShowViewModal(true);
  };

  const handleViewCandidates = async (event) => {
    setSelectedEvent(event);
    setShowCandidatesModal(true);
    try {
      setLoadingCandidates(true);
      const candidates = await eventAPI.getEventCandidates(event.id);
      setEventCandidates(candidates || []);
    } catch (err) {
      toast.error('Failed to load event candidates');
    } finally {
      setLoadingCandidates(false);
    }
  };

  const handleRegisterCandidate = async (e) => {
    e.preventDefault();

    if (!registerForm.candidateId) {
      toast.error('Please select a candidate');
      return;
    }

    try {
      await eventAPI.registerCandidate({
        eventId: selectedEvent.id,
        candidateId: registerForm.candidateId,
        statusId: registerForm.statusId
      });
      toast.success('Candidate registered for event');
      setRegisterForm({ candidateId: '', statusId: '70000000-0000-0000-0000-000000000001' });
      const updated = await eventAPI.getEventCandidates(selectedEvent.id);
      setEventCandidates(updated || []);
    } catch (err) {
      toast.error(err.response?.data?.message || 'Failed to register candidate');
    }
  };

  const handleUpdateCandidateStatus = async (eventCandidateId, statusId) => {
    try {
      await eventAPI.updateCandidateStatus(eventCandidateId, { statusId });
      toast.success('Candidate status updated');
      const updated = await eventAPI.getEventCandidates(selectedEvent.id);
      setEventCandidates(updated || []);
    } catch (err) {
      toast.error('Failed to update candidate status');
    }
  };

  const handleRemoveCandidate = async (eventCandidateId) => {
    if (!window.confirm('Are you sure you want to remove this candidate from the event?')) {
      return;
    }

    try {
      await eventAPI.removeCandidate(eventCandidateId);
      toast.success('Candidate removed from event');
      const updated = await eventAPI.getEventCandidates(selectedEvent.id);
      setEventCandidates(updated || []);
    } catch (err) {
      toast.error('Failed to remove candidate');
    }
  };

  const handleDeleteEvent = async (event) => {
    if (!window.confirm(`Are you sure you want to delete "${event.name}"?`)) {
      return;
    }

    try {
      await eventAPI.delete(event.id);
      toast.success('Event deleted successfully');
      await fetchData();
    } catch (err) {
      toast.error('Failed to delete event');
    }
  };

  const closeAllModals = () => {
    setShowCreateModal(false);
    setShowViewModal(false);
    setShowCandidatesModal(false);
    setSelectedEvent(null);
    setEventCandidates([]);
    setRegisterForm({ candidateId: '', statusId: '70000000-0000-0000-0000-000000000001' });
  };

  if (loading) return <LoadingSpinner />;

  // Pagination
  const paginatedEvents = filteredEvents.slice(
    (currentPage - 1) * itemsPerPage,
    currentPage * itemsPerPage
  );
  const totalPages = Math.ceil(filteredEvents.length / itemsPerPage);

  return (
    <div className="max-w-7xl mx-auto p-6">
      {/* Header */}
      <div className="flex justify-between items-center mb-6">
        <div>
          <h1 className="text-3xl font-bold text-gray-900">Events</h1>
          <p className="text-gray-600 mt-1">Manage recruitment events and candidate registration</p>
        </div>
        {canManage && (
          <button
            onClick={handleCreateNew}
            className="px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 transition-colors"
          >
            + New Event
          </button>
        )}
      </div>

      {/* Search and Filter Bar */}
      <div className="bg-white rounded-lg shadow-md p-4 mb-6">
        <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">Search</label>
            <input
              type="text"
              placeholder="Search by event name or location..."
              value={searchTerm}
              onChange={(e) => setSearchTerm(e.target.value)}
              className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
            />
          </div>

          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">Event Type</label>
            <select
              value={selectedType}
              onChange={(e) => setSelectedType(e.target.value)}
              className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
            >
              <option value="">All Types</option>
              {EVENT_TYPES.map((type) => (
                <option key={type} value={type}>
                  {type}
                </option>
              ))}
            </select>
          </div>

          <div className="flex items-end">
            <button
              onClick={() => {
                setSearchTerm('');
                setSelectedType('');
              }}
              className="w-full px-3 py-2 text-gray-700 bg-gray-200 rounded-lg hover:bg-gray-300 transition-colors"
            >
              Clear Filters
            </button>
          </div>
        </div>
      </div>

      {/* Events Table */}
      <div className="bg-white rounded-lg shadow-md overflow-hidden">
        <div className="overflow-x-auto">
          <table className="w-full">
            <thead className="bg-gray-50 border-b border-gray-200">
              <tr>
                <th className="px-6 py-3 text-left text-sm font-semibold text-gray-900">Event Name</th>
                <th className="px-6 py-3 text-left text-sm font-semibold text-gray-900">Type</th>
                <th className="px-6 py-3 text-left text-sm font-semibold text-gray-900">Location</th>
                <th className="px-6 py-3 text-left text-sm font-semibold text-gray-900">Date</th>
                <th className="px-6 py-3 text-left text-sm font-semibold text-gray-900">Candidates</th>
                <th className="px-6 py-3 text-right text-sm font-semibold text-gray-900">Actions</th>
              </tr>
            </thead>
            <tbody className="divide-y divide-gray-200">
              {paginatedEvents.length > 0 ? (
                paginatedEvents.map((event) => (
                  <tr key={event.id} className="hover:bg-gray-50 transition-colors">
                    <td className="px-6 py-4 text-sm font-medium text-gray-900">{event.name}</td>
                    <td className="px-6 py-4 text-sm">
                      <span className="px-2 py-1 bg-blue-100 text-blue-800 rounded-full text-xs font-medium">
                        {event.type || 'N/A'}
                      </span>
                    </td>
                    <td className="px-6 py-4 text-sm text-gray-600">{event.location || 'N/A'}</td>
                    <td className="px-6 py-4 text-sm text-gray-600">
                      {new Date(event.date).toLocaleDateString()}
                    </td>
                    <td className="px-6 py-4 text-sm text-gray-900 font-medium">
                      {event.candidateCount || 0}
                    </td>
                    <td className="px-6 py-4 text-sm text-right">
                      <div className="flex justify-end gap-2">
                        <button
                          onClick={() => handleViewEvent(event)}
                          className="text-blue-600 hover:text-blue-900 transition-colors"
                          title="View"
                        >
                          👁️
                        </button>
                        <button
                          onClick={() => handleViewCandidates(event)}
                          className="text-purple-600 hover:text-purple-900 transition-colors"
                          title="Candidates"
                        >
                          👥
                        </button>
                        {canManage && (
                          <>
                            <button
                              onClick={() => handleEditEvent(event)}
                              className="text-yellow-600 hover:text-yellow-900 transition-colors"
                              title="Edit"
                            >
                              ✏️
                            </button>
                            <button
                              onClick={() => handleDeleteEvent(event)}
                              className="text-red-600 hover:text-red-900 transition-colors"
                              title="Delete"
                            >
                              🗑️
                            </button>
                          </>
                        )}
                      </div>
                    </td>
                  </tr>
                ))
              ) : (
                <tr>
                  <td colSpan="6" className="px-6 py-8 text-center text-gray-500">
                    No events found
                  </td>
                </tr>
              )}
            </tbody>
          </table>
        </div>

        {/* Pagination */}
        {totalPages > 1 && (
          <div className="bg-gray-50 px-6 py-4 flex items-center justify-between border-t border-gray-200">
            <div className="text-sm text-gray-600">
              Showing {(currentPage - 1) * itemsPerPage + 1} to{' '}
              {Math.min(currentPage * itemsPerPage, filteredEvents.length)} of {filteredEvents.length}{' '}
              events
            </div>
            <div className="flex gap-2">
              <button
                onClick={() => setCurrentPage((p) => Math.max(1, p - 1))}
                disabled={currentPage === 1}
                className="px-3 py-1 text-sm bg-gray-200 text-gray-700 rounded hover:bg-gray-300 disabled:opacity-50"
              >
                Previous
              </button>
              {Array.from({ length: totalPages }, (_, i) => i + 1)
                .slice(Math.max(0, currentPage - 2), Math.min(totalPages, currentPage + 1))
                .map((page) => (
                  <button
                    key={page}
                    onClick={() => setCurrentPage(page)}
                    className={`px-3 py-1 text-sm rounded ${
                      currentPage === page
                        ? 'bg-blue-600 text-white'
                        : 'bg-gray-200 text-gray-700 hover:bg-gray-300'
                    }`}
                  >
                    {page}
                  </button>
                ))}
              <button
                onClick={() => setCurrentPage((p) => Math.min(totalPages, p + 1))}
                disabled={currentPage === totalPages}
                className="px-3 py-1 text-sm bg-gray-200 text-gray-700 rounded hover:bg-gray-300 disabled:opacity-50"
              >
                Next
              </button>
            </div>
          </div>
        )}
      </div>

      {/* Create/Edit Modal */}
      {showCreateModal && (
        <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50 p-4">
          <div className="bg-white rounded-lg max-w-lg w-full">
            <div className="sticky top-0 bg-white border-b border-gray-200 px-6 py-4">
              <h2 className="text-2xl font-bold text-gray-900">
                {selectedEvent ? 'Edit Event' : 'Create New Event'}
              </h2>
            </div>

            <form onSubmit={handleSubmit} className="p-6 space-y-4">
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">Event Name *</label>
                <input
                  type="text"
                  value={formData.name}
                  onChange={(e) => setFormData({ ...formData, name: e.target.value })}
                  placeholder="e.g., Campus Recruitment Drive 2026"
                  className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                  required
                />
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">Event Type</label>
                <select
                  value={formData.type}
                  onChange={(e) => setFormData({ ...formData, type: e.target.value })}
                  className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                >
                  {EVENT_TYPES.map((type) => (
                    <option key={type} value={type}>
                      {type}
                    </option>
                  ))}
                </select>
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">Location</label>
                <input
                  type="text"
                  value={formData.location}
                  onChange={(e) => setFormData({ ...formData, location: e.target.value })}
                  placeholder="e.g., Conference Hall A"
                  className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                />
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">Date *</label>
                <input
                  type="date"
                  value={formData.date}
                  onChange={(e) => setFormData({ ...formData, date: e.target.value })}
                  className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                  required
                />
              </div>

              <div className="flex justify-end gap-3 pt-4 border-t border-gray-200">
                <button
                  type="button"
                  onClick={closeAllModals}
                  className="px-4 py-2 text-gray-700 bg-gray-200 rounded-lg hover:bg-gray-300 transition-colors"
                >
                  Cancel
                </button>
                <button
                  type="submit"
                  className="px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 transition-colors"
                >
                  {selectedEvent ? 'Update Event' : 'Create Event'}
                </button>
              </div>
            </form>
          </div>
        </div>
      )}

      {/* View Modal */}
      {showViewModal && selectedEvent && (
        <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50 p-4">
          <div className="bg-white rounded-lg max-w-2xl w-full max-h-[90vh] overflow-y-auto">
            <div className="sticky top-0 bg-white border-b border-gray-200 px-6 py-4 flex justify-between items-center">
              <h2 className="text-2xl font-bold text-gray-900">{selectedEvent.name}</h2>
              <button
                onClick={closeAllModals}
                className="text-gray-500 hover:text-gray-700"
              >
                ✕
              </button>
            </div>

            <div className="p-6 space-y-4">
              <div className="grid grid-cols-2 gap-4">
                <div>
                  <label className="block text-sm font-semibold text-gray-700 mb-1">Type</label>
                  <div className="px-2 py-1 bg-blue-100 text-blue-800 rounded-full text-sm font-medium inline-block">
                    {selectedEvent.type || 'N/A'}
                  </div>
                </div>

                <div>
                  <label className="block text-sm font-semibold text-gray-700 mb-1">Location</label>
                  <p className="text-gray-900">{selectedEvent.location || 'N/A'}</p>
                </div>
              </div>

              <div>
                <label className="block text-sm font-semibold text-gray-700 mb-1">Date</label>
                <p className="text-gray-900">
                  {new Date(selectedEvent.date).toLocaleDateString('en-US', {
                    weekday: 'long',
                    year: 'numeric',
                    month: 'long',
                    day: 'numeric'
                  })}
                </p>
              </div>

              <div>
                <label className="block text-sm font-semibold text-gray-700 mb-1">Created By</label>
                <p className="text-gray-900">{selectedEvent.createdByName || 'N/A'}</p>
              </div>

              <div>
                <label className="block text-sm font-semibold text-gray-700 mb-1">Created At</label>
                <p className="text-gray-600 text-sm">
                  {new Date(selectedEvent.createdAt).toLocaleString()}
                </p>
              </div>

              <div className="flex justify-end gap-3 pt-4 border-t border-gray-200">
                <button
                  onClick={closeAllModals}
                  className="px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 transition-colors"
                >
                  Close
                </button>
              </div>
            </div>
          </div>
        </div>
      )}

      {/* Candidates Modal */}
      {showCandidatesModal && selectedEvent && (
        <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50 p-4">
          <div className="bg-white rounded-lg max-w-4xl w-full max-h-[90vh] overflow-y-auto">
            <div className="sticky top-0 bg-white border-b border-gray-200 px-6 py-4 flex justify-between items-center">
              <h2 className="text-2xl font-bold text-gray-900">
                Event Candidates - {selectedEvent.name}
              </h2>
              <button
                onClick={closeAllModals}
                className="text-gray-500 hover:text-gray-700"
              >
                ✕
              </button>
            </div>

            <div className="p-6 space-y-6">
              {/* Register Candidate Form */}
              <div className="bg-blue-50 border border-blue-200 rounded-lg p-4">
                <h3 className="font-semibold text-gray-900 mb-3">Register Candidate</h3>
                <form onSubmit={handleRegisterCandidate} className="grid grid-cols-1 md:grid-cols-3 gap-3">
                  <select
                    value={registerForm.candidateId}
                    onChange={(e) => setRegisterForm({ ...registerForm, candidateId: e.target.value })}
                    className="px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500"
                    required
                  >
                    <option value="">Select Candidate</option>
                    {candidates
                      .filter(
                        (c) =>
                          !eventCandidates.some(
                            (ec) => ec.candidateId === c.id
                          )
                      )
                      .map((c) => (
                        <option key={c.id} value={c.id}>
                          {c.fullName || `${c.firstName || ''} ${c.lastName || ''}`.trim()}
                        </option>
                      ))}
                  </select>

                  <select
                    value={registerForm.statusId}
                    onChange={(e) => setRegisterForm({ ...registerForm, statusId: e.target.value })}
                    className="px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500"
                  >
                    {CANDIDATE_STATUS.map((status) => (
                      <option key={status.id} value={status.id}>
                        {status.name}
                      </option>
                    ))}
                  </select>

                  <button
                    type="submit"
                    className="px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 transition-colors"
                  >
                    Register
                  </button>
                </form>
              </div>

              {/* Candidates List */}
              <div>
                <h3 className="font-semibold text-gray-900 mb-3">
                  Registered Candidates ({eventCandidates.length})
                </h3>
                {loadingCandidates ? (
                  <p className="text-gray-500">Loading candidates...</p>
                ) : eventCandidates.length > 0 ? (
                  <div className="overflow-x-auto">
                    <table className="w-full text-sm">
                      <thead className="bg-gray-50">
                        <tr>
                          <th className="px-4 py-2 text-left font-medium text-gray-900">Candidate</th>
                          <th className="px-4 py-2 text-left font-medium text-gray-900">Status</th>
                          <th className="px-4 py-2 text-left font-medium text-gray-900">Registered At</th>
                          <th className="px-4 py-2 text-right font-medium text-gray-900">Actions</th>
                        </tr>
                      </thead>
                      <tbody className="divide-y divide-gray-200">
                        {eventCandidates.map((eventCand) => (
                          <tr key={eventCand.id} className="hover:bg-gray-50">
                            <td className="px-4 py-2 text-gray-900 font-medium">
                              {eventCand.candidateName}
                            </td>
                            <td className="px-4 py-2">
                              <select
                                value={eventCand.statusId}
                                onChange={(e) =>
                                  handleUpdateCandidateStatus(eventCand.id, e.target.value)
                                }
                                className="px-2 py-1 text-xs border border-gray-300 rounded focus:ring-2 focus:ring-blue-500"
                              >
                                {CANDIDATE_STATUS.map((status) => (
                                  <option key={status.id} value={status.id}>
                                    {status.name}
                                  </option>
                                ))}
                              </select>
                            </td>
                            <td className="px-4 py-2 text-gray-600 text-xs">
                              {new Date(eventCand.registeredAt).toLocaleDateString()}
                            </td>
                            <td className="px-4 py-2 text-right">
                              <button
                                onClick={() => handleRemoveCandidate(eventCand.id)}
                                className="text-red-600 hover:text-red-900 transition-colors text-sm"
                              >
                                Remove
                              </button>
                            </td>
                          </tr>
                        ))}
                      </tbody>
                    </table>
                  </div>
                ) : (
                  <p className="text-gray-500">No candidates registered yet</p>
                )}
              </div>

              <div className="flex justify-end gap-3 pt-4 border-t border-gray-200">
                <button
                  onClick={closeAllModals}
                  className="px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 transition-colors"
                >
                  Close
                </button>
              </div>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}
