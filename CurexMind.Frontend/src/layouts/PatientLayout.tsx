import React, { useState } from 'react';
import { Link, NavLink, Outlet, useNavigate } from 'react-router-dom';
import { useAuthStore } from '../store/authStore';
import { useSettingsStore } from '../store/settingsStore';
import { Bell, Menu, X, LogOut, User, Sun, Moon } from 'lucide-react';
import curexmindLogo from '../assets/curexmind_logo.png';

export const PatientLayout: React.FC = () => {
  const { logout, fullName } = useAuthStore();
  const { theme, language, toggleTheme, setLanguage, t } = useSettingsStore();
  const navigate = useNavigate();
  const [mobileMenuOpen, setMobileMenuOpen] = useState(false);

  const handleLogout = () => {
    logout();
    navigate('/login');
  };

  const handleScrollToSection = (e: React.MouseEvent<HTMLAnchorElement>, targetId: string) => {
    e.preventDefault();
    const element = document.getElementById(targetId.replace('#', ''));
    if (element) {
      element.scrollIntoView({ behavior: 'smooth' });
    }
  };

  const navItems = [
    { to: '/patient', label: t('nav.home') },
    { to: '/patient/appointments', label: t('nav.myAppointments') },
    { to: '#about', label: t('nav.aboutUs') },
    { to: '#contact', label: t('nav.contactUs') },
  ];

  const isRtl = language === 'ar';
  const dir = isRtl ? 'rtl' : 'ltr';
  const textAlignClass = isRtl ? 'text-right' : 'text-left';
  const flexDirClass = isRtl ? 'md:flex-row' : 'md:flex-row-reverse';

  return (
    <div className={`min-h-screen bg-slate-50 dark:bg-dark-bg flex flex-col font-sans ${textAlignClass} transition-colors duration-300`} dir={dir}>
      
      {/* Patient Navbar Redesigned */}
      <nav className="bg-white dark:bg-dark-surface border-b border-slate-200/50 dark:border-dark-border sticky top-0 z-50 shadow-sm transition-all duration-300">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
          <div className="flex justify-between h-20 items-center">
            
            {/* Logo Section */}
            <div className="flex items-center gap-3">
              <div className="p-1 bg-brand-50 dark:bg-brand-950/40 rounded-lg">
                <img src={curexmindLogo} alt="CurexMind Logo" className="h-9 w-9 rounded-md object-contain" />
              </div>
              <Link to="/patient" className="text-xl font-extrabold text-slate-900 dark:text-white font-cairo tracking-tight bg-gradient-to-r from-brand-600 to-indigo-500 dark:from-brand-400 dark:to-indigo-300 bg-clip-text text-transparent">
                CurexMind
              </Link>
            </div>

            {/* Desktop Navigation Links */}
            <div className="hidden md:flex items-center gap-8 font-bold text-slate-555 dark:text-slate-400">
              {navItems.map((item) => (
                item.to.startsWith('#') ? (
                  <a 
                    key={item.to}
                    href={item.to} 
                    onClick={(e) => handleScrollToSection(e, item.to)}
                    className="hover:text-brand-600 dark:hover:text-brand-400 transition-colors py-2 text-sm font-cairo cursor-pointer"
                  >
                    {item.label}
                  </a>
                ) : (
                  <NavLink
                    key={item.to}
                    to={item.to}
                    end
                    className={({ isActive }) => 
                      `hover:text-brand-600 dark:hover:text-brand-400 transition-all py-2 text-sm font-cairo relative ${
                        isActive ? 'text-brand-600 dark:text-brand-400 after:absolute after:bottom-0 after:left-0 after:right-0 after:h-0.5 after:bg-brand-500 after:rounded-full' : ''
                      }`
                    }
                  >
                    {item.label}
                  </NavLink>
                )
              ))}
            </div>

            {/* Right Action Icons */}
            <div className="hidden md:flex items-center gap-4">
              {/* Language Selector */}
              <button
                onClick={() => setLanguage(language === 'ar' ? 'en' : 'ar')}
                className="px-3.5 py-2 text-xs font-bold rounded-xl border border-slate-200 dark:border-dark-border hover:bg-slate-50 dark:hover:bg-slate-800 text-slate-700 dark:text-slate-350 cursor-pointer transition-all hover:scale-105"
              >
                {language === 'ar' ? 'English' : 'العربية'}
              </button>

              {/* Theme Selector */}
              <button
                onClick={toggleTheme}
                className="text-slate-500 dark:text-slate-400 hover:text-slate-700 dark:hover:text-white p-2.5 rounded-xl border border-slate-200 dark:border-dark-border hover:bg-slate-50 dark:hover:bg-slate-800 cursor-pointer transition-all hover:scale-105"
                title={theme === 'light' ? t('common.dark') : t('common.light')}
              >
                {theme === 'light' ? <Moon className="h-4.5 w-4.5" /> : <Sun className="h-4.5 w-4.5" />}
              </button>

              {/* Notification icon */}
              <button className="text-slate-500 dark:text-slate-400 hover:text-slate-700 dark:hover:text-white relative p-2.5 rounded-xl border border-slate-200 dark:border-dark-border hover:bg-slate-50 dark:hover:bg-slate-800 cursor-pointer transition-all">
                <Bell className="h-4.5 w-4.5" />
                <span className="absolute top-2.5 right-2.5 h-2 w-2 bg-red-500 rounded-full animate-pulse"></span>
              </button>

              {/* Profile dropdown / name */}
              <div className={`flex items-center gap-3.5 ${language === 'ar' ? 'border-r pr-5' : 'border-l pl-5'} border-slate-200 dark:border-dark-border`}>
                <div className="h-9 w-9 rounded-xl bg-brand-50 dark:bg-brand-950/40 border border-brand-100/10 flex items-center justify-center text-brand-600 dark:text-brand-400 premium-shadow">
                  <User className="h-4.5 w-4.5" />
                </div>
                <div className={textAlignClass}>
                  <p className="text-[9px] text-slate-400 dark:text-slate-500 font-bold uppercase tracking-wider">{t('common.welcome')}</p>
                  <p className="text-sm font-bold text-slate-850 dark:text-slate-200 truncate max-w-[120px] font-cairo">{fullName || t('auth.patient')}</p>
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

            {/* Mobile Hamburger menu */}
            <div className="md:hidden flex items-center gap-3">
              <button
                onClick={() => setLanguage(language === 'ar' ? 'en' : 'ar')}
                className="px-2.5 py-1 text-xs font-bold rounded-lg border border-slate-200 dark:border-dark-border text-slate-700 dark:text-slate-350"
              >
                {language === 'ar' ? 'EN' : 'عربي'}
              </button>
              <button
                onClick={toggleTheme}
                className="text-slate-550 dark:text-slate-400 p-1.5 rounded-lg border border-slate-200 dark:border-dark-border"
              >
                {theme === 'light' ? <Moon className="h-4.5 w-4.5" /> : <Sun className="h-4.5 w-4.5" />}
              </button>
              <button
                onClick={() => setMobileMenuOpen(!mobileMenuOpen)}
                className="text-slate-550 dark:text-slate-400 hover:text-slate-900 p-1.5 rounded-lg border border-slate-250 dark:border-dark-border bg-slate-50 dark:bg-slate-900"
              >
                {mobileMenuOpen ? <X className="h-5 w-5" /> : <Menu className="h-5 w-5" />}
              </button>
            </div>

          </div>
        </div>

        {/* Mobile Navigation Drawer */}
        {mobileMenuOpen && (
          <div className="md:hidden bg-white dark:bg-dark-surface border-t border-slate-100 dark:border-dark-border py-4 px-4 space-y-2 shadow-inner animate-scale-in">
            {navItems.map((item) => (
              item.to.startsWith('#') ? (
                <a
                  key={item.to}
                  href={item.to}
                  onClick={(e) => {
                    setMobileMenuOpen(false);
                    handleScrollToSection(e, item.to);
                  }}
                  className="block px-3 py-2.5 rounded-xl text-base font-bold text-slate-700 dark:text-slate-350 hover:bg-slate-50 dark:hover:bg-slate-900 hover:text-brand-600 font-cairo cursor-pointer"
                >
                  {item.label}
                </a>
              ) : (
                <NavLink
                  key={item.to}
                  to={item.to}
                  end
                  onClick={() => setMobileMenuOpen(false)}
                  className={({ isActive }) => 
                    `block px-3 py-2.5 rounded-xl text-base font-bold font-cairo ${
                      isActive 
                        ? 'bg-brand-50/50 dark:bg-brand-950/20 text-brand-600 dark:text-brand-400' 
                        : 'text-slate-700 dark:text-slate-350 hover:bg-slate-50 dark:hover:bg-slate-900 hover:text-brand-600'
                    }`
                  }
                >
                  {item.label}
                </NavLink>
              )
            ))}
            <div className="border-t border-slate-100 dark:border-dark-border pt-4 flex items-center justify-between">
              <div className="flex items-center gap-3">
                <div className="h-9 w-9 rounded-xl bg-brand-100 dark:bg-brand-950/50 flex items-center justify-center text-brand-600 dark:text-brand-400 font-bold">
                  {fullName?.substring(0, 1).toUpperCase() || 'P'}
                </div>
                <div>
                  <p className="text-sm font-bold text-slate-800 dark:text-slate-200 font-cairo">{fullName || t('auth.patient')}</p>
                </div>
              </div>
              <button
                onClick={handleLogout}
                className="flex items-center gap-2 px-3 py-2 rounded-xl text-sm font-bold text-red-500 hover:bg-red-50 dark:hover:bg-red-950/20 font-cairo"
              >
                <LogOut className="h-4 w-4" />
                <span>{t('common.logout')}</span>
              </button>
            </div>
          </div>
        )}
      </nav>

      {/* Main Page Area */}
      <main className="flex-grow max-w-7xl w-full mx-auto px-4 sm:px-6 lg:px-8 py-8 bg-slate-50 dark:bg-dark-bg transition-colors duration-300">
        <Outlet />
      </main>

      {/* Shared Footer */}
      <footer className="bg-slate-900 dark:bg-[#04060b] text-slate-400 py-12 mt-auto border-t border-slate-850 dark:border-dark-border" id="about">
        <div className={`max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 flex flex-col ${flexDirClass} justify-between gap-8 text-center ${isRtl ? 'md:text-right' : 'md:text-left'}`}>
          <div>
            <div className={`flex items-center gap-3 justify-center ${isRtl ? 'md:justify-start' : 'md:justify-end'} mb-3`}>
              <div className="bg-slate-800 p-1.5 rounded-lg border border-slate-700">
                <img src={curexmindLogo} alt="CurexMind Logo" className="h-7 w-7 rounded-md object-contain" />
              </div>
              <span className="text-white font-extrabold text-lg font-cairo">CurexMind</span>
            </div>
            <p className="text-sm max-w-md font-cairo leading-relaxed">
              {t('patient.aboutClinic')}
            </p>
          </div>

          <div id="contact">
            <h3 className="text-white font-bold mb-3 font-cairo">{t('nav.contactUs')}</h3>
            <p className="text-sm font-cairo">{isRtl ? 'البريد الإلكتروني: support@curexmind.com' : 'Email: support@curexmind.com'}</p>
            <p className="text-sm mt-1 font-cairo">{isRtl ? 'رقم الهاتف: 19999' : 'Phone: 19999'}</p>
            <p className="text-xs text-slate-655 mt-4 font-cairo">
              &copy; {new Date().getFullYear()} CurexMind. {t('landing.rightsReserved')}
            </p>
          </div>
        </div>
      </footer>
    </div>
  );
};
