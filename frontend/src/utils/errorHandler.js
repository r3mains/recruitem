export const getErrorMessage = (error, defaultMessage = "Something went wrong. Please try again.") => {
  // Network error (no response)
  if (!error.response) {
    return "Unable to connect to the server. Please check your internet connection.";
  }

  // Get error message from response
  const { data, status } = error.response;

  // Handle specific status codes
  if (status === 401) {
    return data?.message || "Your session has expired. Please log in again.";
  }

  if (status === 403) {
    return data?.message || "You don't have permission to perform this action.";
  }

  if (status === 404) {
    return data?.message || "The requested resource was not found.";
  }

  if (status === 500) {
    return "A server error occurred. Please try again later or contact support.";
  }

  // Extract message from various response formats
  if (data?.message) {
    return data.message;
  }

  if (data?.errors && Array.isArray(data.errors)) {
    return data.errors.join(", ");
  }

  if (typeof data === "string") {
    return data;
  }

  return defaultMessage;
};

export const getValidationErrors = (error) => {
  if (!error.response?.data) {
    return {};
  }

  const { data } = error.response;
  const validationErrors = {};

  // Handle ASP.NET ModelState errors
  if (data.errors && typeof data.errors === "object") {
    Object.keys(data.errors).forEach((field) => {
      const fieldErrors = data.errors[field];
      if (Array.isArray(fieldErrors)) {
        validationErrors[field.toLowerCase()] = fieldErrors.join(", ");
      } else {
        validationErrors[field.toLowerCase()] = fieldErrors;
      }
    });
  }

  return validationErrors;
};

export const getSuccessMessage = (action, resource) => {
  const actions = {
    created: "created successfully",
    updated: "updated successfully",
  };

  const actionText = actions[action.toLowerCase()] || action;
  return `${resource} ${actionText}.`;
};
