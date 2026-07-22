import { useEffect } from 'react';
import { AppRoutes } from './routes';
import { useAuthStore } from './store/authStore';
import { useSettingsStore } from './store/settingsStore';

function App() {
  const initializeAuth = useAuthStore((state) => state.initialize);
  const initializeSettings = useSettingsStore((state) => state.initializeSettings);

  useEffect(() => {
    // Initialize global settings (language, dir, theme)
    initializeSettings();
    // Check localStorage for existing valid user sessions
    initializeAuth();
  }, [initializeAuth, initializeSettings]);

  return (
    <>
      <AppRoutes />
    </>
  );
}

export default App;
