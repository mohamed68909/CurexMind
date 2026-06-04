import { create } from 'zustand';
import { jwtDecode } from 'jwt-decode';

interface DecodedToken {
  sub?: string;
  email?: string;
  nameid?: string;
  patientId?: string;
  doctorId?: string;
  [key: string]: any;
}

// Helper to resolve ASP.NET long-form or short-form claim names
const getClaimValue = (decoded: any, claimName: string) => {
  const shortName = claimName.split('/').pop() || '';
  return decoded[claimName] || decoded[shortName] || decoded[claimName.toLowerCase()];
};

interface UserState {
  isAuthenticated: boolean;
  token: string | null;
  userId: string | null;
  email: string | null;
  fullName: string | null;
  roles: string[];
  // Patient & Doctor portal identifiers extracted from JWT claims
  patientId: string | null;
  doctorId: string | null;
  login: (token: string, refreshToken: string, fullName?: string) => void;
  logout: () => void;
  initialize: () => void;
}

/** Decode the stored JWT and extract all claims into initial state */
const getInitialState = () => {
  const token = localStorage.getItem('token');
  const refreshToken = localStorage.getItem('refreshToken');
  const storedName = localStorage.getItem('userName');

  if (token && refreshToken) {
    try {
      const decoded: any = jwtDecode<DecodedToken>(token);

      const userId =
        getClaimValue(decoded, 'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier') ||
        decoded.sub;
      const email =
        getClaimValue(decoded, 'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress') ||
        decoded.email;

      const rawRoles =
        getClaimValue(decoded, 'http://schemas.microsoft.com/ws/2008/06/identity/claims/role') ||
        decoded.role ||
        [];
      const roles = Array.isArray(rawRoles) ? rawRoles : rawRoles ? [rawRoles] : [];

      const finalFullName = storedName || decoded.fullName || email?.split('@')[0] || 'User';

      // Check token expiry
      const exp = decoded.exp;
      const now = Date.now() / 1000;
      if (exp && exp < now) {
        return {
          isAuthenticated: false,
          token: null,
          userId: null,
          email: null,
          fullName: null,
          roles: [],
          patientId: null,
          doctorId: null,
        };
      }

      return {
        isAuthenticated: true,
        token,
        userId,
        email,
        fullName: finalFullName,
        roles,
        // Custom claims added by the backend JwtProvider
        patientId: decoded.patientId ?? null,
        doctorId: decoded.doctorId ?? null,
      };
    } catch (error) {
      console.error('Failed to parse initial token:', error);
    }
  }

  return {
    isAuthenticated: false,
    token: null,
    userId: null,
    email: null,
    fullName: null,
    roles: [],
    patientId: null,
    doctorId: null,
  };
};

const initialState = getInitialState();

export const useAuthStore = create<UserState>((set) => ({
  ...initialState,

  login: (token, refreshToken, fullName) => {
    try {
      const decoded: any = jwtDecode<DecodedToken>(token);

      const userId =
        getClaimValue(decoded, 'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier') ||
        decoded.sub;
      const email =
        getClaimValue(decoded, 'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress') ||
        decoded.email;

      const rawRoles =
        getClaimValue(decoded, 'http://schemas.microsoft.com/ws/2008/06/identity/claims/role') ||
        decoded.role ||
        [];
      const roles = Array.isArray(rawRoles) ? rawRoles : rawRoles ? [rawRoles] : [];

      const finalFullName = fullName || decoded.fullName || email?.split('@')[0] || 'User';

      localStorage.setItem('token', token);
      localStorage.setItem('refreshToken', refreshToken);
      localStorage.setItem('userName', finalFullName);

      set({
        isAuthenticated: true,
        token,
        userId,
        email,
        fullName: finalFullName,
        roles,
        patientId: decoded.patientId ?? null,
        doctorId: decoded.doctorId ?? null,
      });
    } catch (error) {
      console.error('Failed to decode token on login:', error);
      set({
        isAuthenticated: true,
        token,
        userId: null,
        email: null,
        fullName: fullName || 'User',
        roles: [],
        patientId: null,
        doctorId: null,
      });
    }
  },

  logout: () => {
    localStorage.removeItem('token');
    localStorage.removeItem('refreshToken');
    localStorage.removeItem('userName');
    set({
      isAuthenticated: false,
      token: null,
      userId: null,
      email: null,
      fullName: null,
      roles: [],
      patientId: null,
      doctorId: null,
    });
  },

  initialize: () => {
    const token = localStorage.getItem('token');
    const refreshToken = localStorage.getItem('refreshToken');
    const storedName = localStorage.getItem('userName');

    if (token && refreshToken) {
      try {
        const decoded: any = jwtDecode<DecodedToken>(token);

        const userId =
          getClaimValue(decoded, 'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier') ||
          decoded.sub;
        const email =
          getClaimValue(decoded, 'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress') ||
          decoded.email;

        const rawRoles =
          getClaimValue(decoded, 'http://schemas.microsoft.com/ws/2008/06/identity/claims/role') ||
          decoded.role ||
          [];
        const roles = Array.isArray(rawRoles) ? rawRoles : rawRoles ? [rawRoles] : [];

        const finalFullName = storedName || decoded.fullName || email?.split('@')[0] || 'User';

        const exp = decoded.exp;
        const now = Date.now() / 1000;
        if (exp && exp < now) {
          console.warn('Stored token is expired. Logging out...');
          localStorage.removeItem('token');
          localStorage.removeItem('refreshToken');
          localStorage.removeItem('userName');
          return;
        }

        set({
          isAuthenticated: true,
          token,
          userId,
          email,
          fullName: finalFullName,
          roles,
          patientId: decoded.patientId ?? null,
          doctorId: decoded.doctorId ?? null,
        });
      } catch (error) {
        console.error('Failed to initialize auth state from token:', error);
        localStorage.removeItem('token');
        localStorage.removeItem('refreshToken');
        localStorage.removeItem('userName');
      }
    }
  },
}));
