import React, { createContext, useContext } from "react";
import {
  authAPI,
  usersAPI,
  rolesAPI,
  jobsAPI,
  lookupsAPI,
  positionsAPI,
  addressesAPI,
} from "../services/api";

const ApiContext = createContext();

export const useApi = () => {
  const context = useContext(ApiContext);
  if (!context) {
    throw new Error("useApi must be used within an ApiProvider");
  }
  return context;
};

export const ApiProvider = ({ children }) => {
  const api = {
    auth: authAPI,
    users: usersAPI,
    roles: rolesAPI,
    jobs: jobsAPI,
    lookups: lookupsAPI,
    positions: positionsAPI,
    addresses: addressesAPI,
  };

  return <ApiContext.Provider value={api}>{children}</ApiContext.Provider>;
};
