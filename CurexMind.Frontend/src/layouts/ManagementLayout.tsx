import React, { useState } from 'react';
import { NavLink, Outlet, useNavigate } from 'react-router-dom';
import { useAuthStore } from '../store/authStore';
import { useSettingsStore } from '../store/settingsStore';
import { 
  LayoutDashboard, 
  Users, 
  Calendar, 
  FileText, 
  Bed, 
  LogOut, 
  Menu, 
  X, 
  Bell,
  Sun,
  Moon
} from 'lucide-react';
import curexmindLogo from '../assets/curexmind_logo.png';

export const ManagementLayout: React.FC = () => {
  const { logout, fullName } = useAuthStore();
  const { theme, language, toggleTheme, setLanguage, t } = useSettingsStore();
  const navigate = useNavigate();
  const [mobileMenuOpen, setMobileMenuOpen] = useState(false);

  const handleLogout = () => {
    logout();
    navigate('/login');
  };

  const navItems = [
    { to: '/management', label: t('nav.dashboard'), icon: LayoutDashboard, end: true },
    { to: '/management/patients', label: t('nav.patients'), icon: Users },
    { to: '/management/appointments', label: t('nav.appointments'), icon: Calendar },
    { to: '/management/invoices', label: t('nav.invoices'), icon: FileText },
    { to: '/management/stays', label: t('nav.stays'), icon: Bed },
  ];

  const isRtl = language === 'ar';
  const dir = isRtl ? 'rtl' : 'ltr';

  return (
    <div className="flex h-screen overflow-hidden bg-slate-50 dark:bg-dark-bg font-sans transition-colors duration-300" dir={dir}>
      
      {/* Desktop Sidebar Redesigned */}
      <aside className="hidden md:flex md:flex-shrink-0 flex-col w-66 bg-white dark:bg-dark-surface border-r border-slate-200/60 dark:border-dark-border premium-shadow transition-all duration-300">
        <div className="flex flex-col h-full justify-between">
          <div>
            {/* Header / Logo */}
            <div className="h-20 flex items-center px-6 border-b border-slate-100 dark:border-dark-border gap-3">
              <div className="p-1 bg-brand-50 dark:bg-brand-950/40 rounded-lg">
                <img src={curexmindLogo} alt="CurexMind Logo" className="h-9 w-9 rounded-md object-contain" />
              </div>
              <div>
                <h1 className="text-base font-extrabold text-slate-900 dark:text-white leading-tight font-cairo bg-gradient-to-r from-brand-600 to-indigo-500 dark:from-brand-400 dark:to-indigo-300 bg-clip-text text-transparent">
                  CurexMind
                </h1>
                <p className="text-[9px] text-slate-400 dark:text-slate-500 font-bold tracking-wider uppercase">{t('nav.staffPanel')}</p>
              </div>
            </div>

            {/* Nav Menu with Active Border Indicators */}
            <nav className="mt-8 px-3 space-y-1.5">
              {navItems.map((item) => {
                const Icon = item.icon;
                return (
                  <NavLink
                    key={item.to}
                    to={item.to}
                    end={item.end}
                    className={({ isActive }) => {
                      const baseClass = "flex items-center px-4 py-3 text-sm font-bold transition-all group gap-3 rounded-xl hover:scale-[1.02]";
                      const activeClass = isActive
                        ? `bg-brand-50/50 dark:bg-brand-950/20 text-brand-600 dark:text-brand-400 border-l-4 border-brand-500 ${isRtl ? 'border-l-0 border-r-4' : ''}`
                        : "text-slate-500 dark:text-slate-400 hover:bg-slate-50 dark:hover:bg-slate-900 hover:text-slate-900 dark:hover:text-white";
                      return `${baseClass} ${activeClass}`;
                    }}
                  >
                    <Icon className="h-5 w-5 flex-shrink-0" />
                    <span className="font-cairo">{item.label}</span>
                  </NavLink>
                );
              })}
            </nav>
          </div>

          {/* User Profile Footer */}
          <div className="p-4 border-t border-slate-100 dark:border-dark-border flex items-center gap-3 bg-slate-50/50 dark:bg-slate-950/10">
            <div className="h-10 w-10 rounded-xl bg-brand-100 dark:bg-brand-950/50 text-brand-600 dark:text-brand-400 font-extrabold flex items-center justify-center premium-shadow">
              {fullName?.substring(0, 2).toUpperCase() || 'ST'}
            </div>
            <div className="flex-1 min-w-0">
              <p className="text-sm font-bold text-slate-850 dark:text-slate-200 truncate">{fullName || 'Staff'}</p>
              <p className="text-[10px] text-slate-450 dark:text-slate-500 truncate font-semibold uppercase">{t('common.role')}</p>
            </div>
            <button 
              onClick={handleLogout}
              className="text-slate-400 hover:text-red-500 hover:bg-red-50 dark:hover:bg-red-950/20 p-2 rounded-xl transition-all cursor-pointer"
              title={t('common.logout')}
            >
              <LogOut className="h-4.5 w-4.5" />
            </button>
          </div>
        </div>
      </aside>

      {/* Mobile Drawer (Overlay and Menu) */}
      {mobileMenuOpen && (
        <div className="md:hidden fixed inset-0 z-50 flex">
          <div 
            className="fixed inset-0 bg-slate-900/40 backdrop-blur-sm transition-opacity" 
            onClick={() => setMobileMenuOpen(false)}
          />
          <div className="relative flex-1 flex flex-col max-w-xs w-full bg-white dark:bg-dark-surface border-r border-slate-250 dark:border-dark-border animate-scale-in">
            <div className={`absolute top-4 ${isRtl ? 'left-4' : 'right-4'} z-50`}>
              <button
                type="button"
                className="flex items-center justify-center h-10 w-10 rounded-xl border border-slate-200 dark:border-dark-border bg-white dark:bg-slate-900 text-slate-500 dark:text-slate-400"
                onClick={() => setMobileMenuOpen(false)}
              >
                <X className="h-5 w-5" />
              </button>
            </div>

            <div className="h-20 flex items-center px-6 border-b border-slate-100 dark:border-dark-border gap-3">
              <div className="p-1 bg-brand-50 dark:bg-brand-950/40 rounded-lg">
                <img src={curexmindLogo} alt="CurexMind Logo" className="h-9 w-9 rounded-md object-contain" />
              </div>
              <div>
                <h1 className="text-base font-extrabold text-slate-900 dark:text-white font-cairo">CurexMind</h1>
                <p className="text-[9px] text-slate-400 dark:text-slate-500 font-bold uppercase">{t('nav.staffPanel')}</p>
              </div>
            </div>

            <nav className="mt-6 px-3 space-y-1.5 flex-1 overflow-y-auto">
              {navItems.map((item) => {
                const Icon = item.icon;
                return (
                  <NavLink
                    key={item.to}
                    to={item.to}
                    end={item.end}
                    onClick={() => setMobileMenuOpen(false)}
                    className={({ isActive }) => {
                      const baseClass = "flex items-center px-4 py-3 text-sm font-bold transition-all gap-3 rounded-xl";
                      const activeClass = isActive
                        ? `bg-brand-50/50 dark:bg-brand-950/20 text-brand-600 dark:text-brand-400 border-l-4 border-brand-500 ${isRtl ? 'border-l-0 border-r-4' : ''}`
                        : "text-slate-550 dark:text-slate-400 hover:bg-slate-550/5 dark:hover:bg-slate-900 hover:text-slate-900 dark:hover:text-white";
                      return `${baseClass} ${activeClass}`;
                    }}
                  >
                    <Icon className="h-5 w-5 flex-shrink-0" />
                    <span className="font-cairo">{item.label}</span>
                  </NavLink>
                );
              })}
            </nav>

            <div className="p-4 border-t border-slate-100 dark:border-dark-border flex items-center gap-3 bg-slate-50 dark:bg-[#090e18]">
              <div className="h-10 w-10 rounded-xl bg-brand-100 dark:bg-brand-950/50 text-brand-600 dark:text-brand-400 font-extrabold flex items-center justify-center">
                {fullName?.substring(0, 2).toUpperCase() || 'ST'}
              </div>
              <div className="flex-1 min-w-0">
                <p className="text-sm font-bold text-slate-850 dark:text-slate-200 truncate">{fullName || 'Staff'}</p>
                <p className="text-[10px] text-slate-450 dark:text-slate-500 uppercase font-semibold">{t('common.role')}</p>
              </div>
              <button 
                onClick={handleLogout}
                className="text-slate-400 hover:text-red-500 p-2 hover:bg-red-50 dark:hover:bg-red-950/10 rounded-xl"
              >
                <LogOut className="h-4.5 w-4.5" />
              </button>
            </div>
          </div>
        </div>
      )}

      {/* Right Content Area */}
      <div className="flex-1 flex flex-col overflow-hidden">
        {/* Top Header */}
        <header className="bg-white dark:bg-dark-surface border-b border-slate-200/60 dark:border-dark-border h-20 flex items-center justify-between px-6 z-10 transition-colors duration-300">
          <div className="flex items-center gap-4">
            <button
              onClick={() => setMobileMenuOpen(true)}
              className="md:hidden text-slate-500 dark:text-slate-400 hover:text-slate-700 dark:hover:text-white p-2 border border-slate-250 dark:border-dark-border rounded-xl bg-slate-50/50 dark:bg-slate-900"
            >
              <Menu className="h-5 w-5" />
            </button>
            <div className="hidden sm:block text-left">
              <h2 className="text-sm font-extrabold text-slate-800 dark:text-slate-250 font-cairo">{t('dashboard.welcome')} {fullName || 'Staff'}</h2>
              <p className="text-[9px] text-slate-400 dark:text-slate-500 font-bold uppercase tracking-wider mt-0.5">
                {new Date().toLocaleDateString(language === 'ar' ? 'ar-EG' : 'en-US', { weekday: 'long', year: 'numeric', month: 'long', day: 'numeric' })}
              </p>
            </div>
          </div>

          <div className="flex items-center gap-3">
            {/* Language Switcher Button */}
            <button
              onClick={() => setLanguage(language === 'ar' ? 'en' : 'ar')}
              className="px-3.5 py-2 text-xs font-bold rounded-xl border border-slate-200 dark:border-dark-border hover:bg-slate-150/5 dark:hover:bg-slate-800 text-slate-700 dark:text-slate-350 cursor-pointer transition-all hover:scale-105"
            >
              {language === 'ar' ? 'English' : 'العربية'}
            </button>

            {/* Dark Mode Toggle */}
            <button
              onClick={toggleTheme}
              className="text-slate-500 dark:text-slate-400 hover:text-slate-700 dark:hover:text-white p-2.5 rounded-xl border border-slate-200 dark:border-dark-border hover:bg-slate-100 dark:hover:bg-slate-800 cursor-pointer transition-all hover:scale-105"
              title={theme === 'light' ? t('common.dark') : t('common.light')}
            >
              {theme === 'light' ? <Moon className="h-4.5 w-4.5" /> : <Sun className="h-4.5 w-4.5" />}
            </button>

            <button className="relative text-slate-500 dark:text-slate-400 hover:text-slate-700 dark:hover:text-white p-2.5 rounded-xl border border-slate-200 dark:border-dark-border hover:bg-slate-100 dark:hover:bg-slate-800 cursor-pointer transition-all">
              <Bell className="h-4.5 w-4.5" />
              <span className="absolute top-2.5 right-2.5 h-2 w-2 bg-red-500 rounded-full animate-pulse"></span>
            </button>
          </div>
        </header>

        {/* Dynamic Inner Page Content */}
        <main className="flex-1 overflow-y-auto p-6 md:p-8 bg-slate-50 dark:bg-dark-bg transition-colors duration-300">
          <Outlet />
        </main>
      </div>
    </div>
  );
};
