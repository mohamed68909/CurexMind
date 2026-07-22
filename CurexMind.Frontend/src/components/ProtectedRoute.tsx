import React from 'react';
import { Navigate, Outlet } from 'react-router-dom';
import { useAuthStore } from '../store/authStore';

interface ProtectedRouteProps {
  allowedRoles?: string[];
}

export const ProtectedRoute: React.FC<ProtectedRouteProps> = ({ allowedRoles }) => {
  const { isAuthenticated, roles } = useAuthStore();

  // If there's a token but state is not initialized (e.g. on page refresh), fallback
  const isAuth = isAuthenticated || !!localStorage.getItem('token');
  
  if (!isAuth) {
    // Redirect to login if not authenticated
    return <Navigate to="/login" replace />;
  }

  // Check if roles are authorized
  if (allowedRoles && allowedRoles.length > 0) {
    // In case roles are not loaded yet in store but token exists, decode token locally
    let userRoles: string[] = roles;
    if (roles.length === 0 && localStorage.getItem('token')) {
      try {
        const tokenVal = localStorage.getItem('token')!;
        const base64Url = tokenVal.split('.')[1];
        const base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
        const decoded = JSON.parse(window.atob(base64));
        const rawRoles = decoded['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'] || decoded.role || [];
        userRoles = Array.isArray(rawRoles) ? rawRoles : rawRoles ? [rawRoles] : [];
      } catch (e) {
        console.error('Failed to parse token roles in RouteGuard:', e);
      }
    }

    const authorized = allowedRoles.some((role) => userRoles.includes(role));

    if (!authorized) {
      // Redirect to unauthorized page (or back to home)
      return <Navigate to="/unauthorized" replace />;
    }
  }

  return <Outlet />;
};
