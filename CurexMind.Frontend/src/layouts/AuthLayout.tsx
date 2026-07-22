import React from 'react';
import { Outlet } from 'react-router-dom';
import { useSettingsStore } from '../store/settingsStore';
import { Sun, Moon } from 'lucide-react';

export const AuthLayout: React.FC = () => {
  const { theme, language, toggleTheme, setLanguage, t } = useSettingsStore();
  const dir = language === 'ar' ? 'rtl' : 'ltr';

  return (
    <div className="bg-gradient-to-br from-slate-50 via-slate-100 to-brand-50/20 dark:from-dark-bg dark:via-slate-950 dark:to-brand-950/20 min-h-screen flex flex-col justify-center items-center p-4 relative transition-colors duration-300" dir={dir}>
      {/* Glow Effects */}
      <div className="absolute top-10 left-10 w-72 h-72 bg-brand-400/5 rounded-full blur-[80px] pointer-events-none"></div>
      <div className="absolute bottom-10 right-10 w-72 h-72 bg-indigo-500/5 rounded-full blur-[80px] pointer-events-none"></div>

      {/* Settings Panel in the corner */}
      <div className={`absolute top-6 ${language === 'ar' ? 'left-6' : 'right-6'} flex items-center gap-3.5 z-50`}>
        <button
          onClick={() => setLanguage(language === 'ar' ? 'en' : 'ar')}
          className="px-3.5 py-2 text-xs font-bold rounded-xl border border-slate-200 dark:border-dark-border bg-white dark:bg-dark-surface hover:bg-slate-50 dark:hover:bg-slate-800 text-slate-700 dark:text-slate-300 cursor-pointer transition-all duration-200 hover:scale-105 premium-shadow"
        >
          {language === 'ar' ? 'English' : 'العربية'}
        </button>
        <button
          onClick={toggleTheme}
          className="text-slate-500 dark:text-slate-400 p-2.5 rounded-xl border border-slate-200 dark:border-dark-border bg-white dark:bg-dark-surface hover:bg-slate-50 dark:hover:bg-slate-800 cursor-pointer transition-all duration-200 hover:scale-105 premium-shadow"
          title={theme === 'light' ? t('common.dark') : t('common.light')}
        >
          {theme === 'light' ? <Moon className="h-4.5 w-4.5" /> : <Sun className="h-4.5 w-4.5" />}
        </button>
      </div>

      <Outlet />
    </div>
  );
};
