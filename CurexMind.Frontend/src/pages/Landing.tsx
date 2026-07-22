import React from 'react';
import { Link } from 'react-router-dom';
import { useSettingsStore } from '../store/settingsStore';
import { Heart, Calendar, Shield, Users, ArrowLeft, ArrowRight, Sun, Moon, ArrowUpRight, Sparkles, Check } from 'lucide-react';
import curexmindLogo from '../assets/curexmind_logo.png';

export const Landing: React.FC = () => {
  const { theme, language, toggleTheme, setLanguage, t } = useSettingsStore();
  const dir = language === 'ar' ? 'rtl' : 'ltr';
  const textAlignment = language === 'ar' ? 'text-right' : 'text-left';

  const isRtl = language === 'ar';

  return (
    <div className={`min-h-screen bg-slate-50 dark:bg-dark-bg font-sans ${textAlignment} transition-colors duration-300`} dir={dir}>
      
      {/* Premium Header/Navbar with Glassmorphism */}
      <header className="sticky top-0 z-50 w-full glass-effect border-b border-slate-200/50 dark:border-dark-border premium-shadow transition-all">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 h-20 flex items-center justify-between">
          <div className="flex items-center gap-3">
            <div className="relative p-1 bg-white/80 dark:bg-slate-800 rounded-xl premium-shadow border border-slate-100 dark:border-slate-700">
              <img src={curexmindLogo} alt="CurexMind Logo" className="h-10 w-10 rounded-lg object-contain" />
              <span className="absolute -top-1 -right-1 flex h-3 w-3">
                <span className="animate-ping absolute inline-flex h-full w-full rounded-full bg-brand-400 opacity-75"></span>
                <span className="relative inline-flex rounded-full h-3 w-3 bg-brand-500"></span>
              </span>
            </div>
            <div>
              <span className="text-2xl font-extrabold text-slate-900 dark:text-white tracking-tight font-cairo bg-gradient-to-r from-brand-600 to-indigo-500 dark:from-brand-400 dark:to-indigo-300 bg-clip-text text-transparent">
                CurexMind
              </span>
              <p className="text-[9px] text-slate-400 dark:text-slate-500 font-bold uppercase tracking-widest">{t('common.appName')}</p>
            </div>
          </div>

          <div className="flex items-center gap-3">
            {/* Language Switcher */}
            <button
              onClick={() => setLanguage(language === 'ar' ? 'en' : 'ar')}
              className="px-3.5 py-2 text-xs font-bold rounded-xl border border-slate-200 dark:border-dark-border hover:bg-slate-100 dark:hover:bg-slate-800 text-slate-700 dark:text-slate-300 cursor-pointer transition-all duration-200 hover:scale-105"
            >
              {language === 'ar' ? 'English' : 'العربية'}
            </button>

            {/* Theme Switcher */}
            <button
              onClick={toggleTheme}
              className="text-slate-500 dark:text-slate-400 hover:text-slate-800 dark:hover:text-white p-2.5 rounded-xl border border-slate-200 dark:border-dark-border hover:bg-slate-100 dark:hover:bg-slate-800 cursor-pointer transition-all duration-200 hover:scale-105"
              title={theme === 'light' ? t('common.dark') : t('common.light')}
            >
              {theme === 'light' ? <Moon className="h-4.5 w-4.5" /> : <Sun className="h-4.5 w-4.5" />}
            </button>

            <Link 
              to="/login" 
              className="text-slate-700 dark:text-slate-300 hover:text-brand-600 dark:hover:text-brand-400 font-bold text-sm px-4 py-2.5 rounded-xl transition-colors font-cairo"
            >
              {t('auth.login')}
            </Link>
            
            <Link 
              to="/register" 
              className="bg-brand-600 hover:bg-brand-700 text-white font-bold text-sm px-5 py-2.5 rounded-xl transition-all shadow-md shadow-brand-500/10 hover:shadow-lg hover:shadow-brand-500/20 font-cairo flex items-center gap-2"
            >
              <span>{t('auth.staffSignup')}</span>
              <ArrowUpRight className="h-4 w-4" />
            </Link>
          </div>
        </div>
      </header>

      {/* Hero Section with Vibrant Gradients & Micro-Animations */}
      <section className="relative overflow-hidden bg-gradient-to-br from-brand-600 via-brand-750 to-indigo-900 dark:from-slate-900 dark:via-brand-950 dark:to-indigo-950 text-white py-24 md:py-36">
        
        {/* Glow Effects in Background */}
        <div className="absolute top-1/4 left-1/4 w-96 h-96 bg-brand-400/20 rounded-full blur-[120px] animate-pulse-slow"></div>
        <div className="absolute bottom-1/4 right-1/4 w-96 h-96 bg-indigo-500/15 rounded-full blur-[120px] animate-pulse-slow"></div>

        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 relative z-10 flex flex-col items-center text-center">
          
          <div className="animate-fade-in-up flex items-center gap-2 bg-white/10 backdrop-blur-md px-4 py-2 rounded-full text-xs font-bold font-cairo tracking-wide mb-8 border border-white/10">
            <Sparkles className="h-4 w-4 text-brand-350 fill-brand-350" />
            <span>{t('landing.tagline')}</span>
          </div>

          <h1 className="animate-fade-in-up [animation-delay:100ms] text-4xl md:text-7xl font-extrabold font-cairo tracking-tight leading-[1.15] max-w-4xl mb-8">
            {isRtl ? (
              <>
                نظام رعاية ذكي مصمم <span className="bg-gradient-to-r from-brand-200 to-brand-100 bg-clip-text text-transparent">لتسهيل وإدارة عيادتك</span>
              </>
            ) : (
              <>
                A Smart Care System <span className="bg-gradient-to-r from-brand-200 to-brand-100 bg-clip-text text-transparent">Built to Elevate Your Clinic</span>
              </>
            )}
          </h1>

          <p className="animate-fade-in-up [animation-delay:200ms] text-lg md:text-xl text-brand-100/90 max-w-3xl font-cairo mb-12 leading-relaxed">
            {t('landing.description')}
          </p>

          <div className="animate-fade-in-up [animation-delay:300ms] flex flex-col sm:flex-row gap-5 justify-center w-full max-w-md">
            <Link 
              to="/login" 
              className="bg-white hover:bg-slate-50 text-brand-700 font-extrabold px-8 py-4 rounded-2xl shadow-xl hover:shadow-2xl transition-all duration-300 font-cairo flex items-center justify-center gap-3 hover:-translate-y-0.5 group"
            >
              <span>{t('landing.enterAsPatient')}</span>
              {isRtl ? (
                <ArrowLeft className="h-5 w-5 transition-transform group-hover:-translate-x-1" />
              ) : (
                <ArrowRight className="h-5 w-5 transition-transform group-hover:translate-x-1" />
              )}
            </Link>
            
            <Link 
              to="/login" 
              className="bg-brand-500/30 hover:bg-brand-500/40 text-white font-bold px-8 py-4 rounded-2xl shadow-lg border border-white/20 hover:border-white/40 backdrop-blur-md transition-all duration-300 font-cairo flex items-center justify-center hover:-translate-y-0.5"
            >
              {t('landing.enterAsReceptionist')}
            </Link>
          </div>

          {/* Quick value props list */}
          <div className="animate-fade-in-up [animation-delay:400ms] grid grid-cols-2 md:grid-cols-3 gap-6 mt-16 text-sm text-brand-200 border-t border-white/10 pt-8 w-full max-w-3xl">
            <div className="flex items-center justify-center gap-2">
              <div className="h-5 w-5 rounded-full bg-emerald-500/20 text-emerald-350 flex items-center justify-center text-xs">
                <Check className="h-3 w-3" />
              </div>
              <span className="font-semibold">{isRtl ? 'حجز فوري ومؤمن' : 'Instant & Secure Booking'}</span>
            </div>
            <div className="flex items-center justify-center gap-2">
              <div className="h-5 w-5 rounded-full bg-emerald-500/20 text-emerald-350 flex items-center justify-center text-xs">
                <Check className="h-3 w-3" />
              </div>
              <span className="font-semibold">{isRtl ? 'إدارة ذكية للفواتير' : 'Smart Billing Engine'}</span>
            </div>
            <div className="flex items-center justify-center gap-2 col-span-2 md:col-span-1">
              <div className="h-5 w-5 rounded-full bg-emerald-500/20 text-emerald-350 flex items-center justify-center text-xs">
                <Check className="h-3 w-3" />
              </div>
              <span className="font-semibold">{isRtl ? 'تقييمات موثقة للأطباء' : 'Verified Reviews'}</span>
            </div>
          </div>

        </div>

        {/* Decorative Grid Lines Overlay */}
        <div className="absolute inset-0 bg-[linear-gradient(to_right,rgba(255,255,255,0.03)_1px,transparent_1px),linear-gradient(to_bottom,rgba(255,255,255,0.03)_1px,transparent_1px)] bg-[size:4rem_4rem] [mask-image:radial-gradient(ellipse_60%_50%_at_50%_0%,#000_70%,transparent_100%)]"></div>
      </section>

      {/* Features Grid Redesigned to feel premium */}
      <section className="py-24 max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 relative">
        <div className="text-center mb-20">
          <span className="text-xs font-bold text-brand-600 dark:text-brand-400 uppercase tracking-widest bg-brand-50 dark:bg-brand-950/40 px-3 py-1.5 rounded-full">{isRtl ? 'ميزات النظام' : 'SYSTEM HIGHLIGHTS'}</span>
          <h2 className="text-3xl md:text-5xl font-extrabold text-slate-900 dark:text-white font-cairo mt-4 mb-5">{t('landing.featuresTitle')}</h2>
          <p className="text-slate-500 dark:text-slate-400 max-w-2xl mx-auto font-cairo text-sm md:text-base leading-relaxed">
            {t('landing.featuresSubtitle')}
          </p>
        </div>

        <div className="grid grid-cols-1 md:grid-cols-3 gap-8">
          {/* Feature 1 */}
          <div className="bg-white dark:bg-dark-surface border border-slate-200/50 dark:border-dark-border p-10 rounded-[2rem] premium-shadow hover:shadow-xl dark:hover:shadow-brand-950/20 hover:-translate-y-2 transition-all duration-300 group">
            <div className="bg-brand-50 dark:bg-brand-950/40 text-brand-600 dark:text-brand-400 p-4.5 rounded-2xl w-fit mb-8 group-hover:scale-110 transition-transform duration-300">
              <Calendar className="h-7 w-7" />
            </div>
            <h3 className="text-2xl font-bold text-slate-900 dark:text-white font-cairo mb-4">{t('landing.feature1Title')}</h3>
            <p className="text-slate-500 dark:text-slate-400 font-cairo text-sm leading-relaxed">
              {t('landing.feature1Desc')}
            </p>
          </div>

          {/* Feature 2 */}
          <div className="bg-white dark:bg-dark-surface border border-slate-200/50 dark:border-dark-border p-10 rounded-[2rem] premium-shadow hover:shadow-xl dark:hover:shadow-brand-950/20 hover:-translate-y-2 transition-all duration-300 group">
            <div className="bg-emerald-50 dark:bg-emerald-950/40 text-emerald-600 dark:text-emerald-400 p-4.5 rounded-2xl w-fit mb-8 group-hover:scale-110 transition-transform duration-300">
              <Users className="h-7 w-7" />
            </div>
            <h3 className="text-2xl font-bold text-slate-900 dark:text-white font-cairo mb-4">{t('landing.feature2Title')}</h3>
            <p className="text-slate-500 dark:text-slate-400 font-cairo text-sm leading-relaxed">
              {t('landing.feature2Desc')}
            </p>
          </div>

          {/* Feature 3 */}
          <div className="bg-white dark:bg-dark-surface border border-slate-200/50 dark:border-dark-border p-10 rounded-[2rem] premium-shadow hover:shadow-xl dark:hover:shadow-brand-950/20 hover:-translate-y-2 transition-all duration-300 group">
            <div className="bg-rose-50 dark:bg-rose-950/40 text-rose-600 dark:text-rose-450 p-4.5 rounded-2xl w-fit mb-8 group-hover:scale-110 transition-transform duration-300">
              <Shield className="h-7 w-7" />
            </div>
            <h3 className="text-2xl font-bold text-slate-900 dark:text-white font-cairo mb-4">{t('landing.feature3Title')}</h3>
            <p className="text-slate-500 dark:text-slate-400 font-cairo text-sm leading-relaxed">
              {t('landing.feature3Desc')}
            </p>
          </div>
        </div>
      </section>

      {/* Footer Section */}
      <footer className="bg-slate-900 dark:bg-[#04060b] text-slate-400 py-16 border-t border-slate-800/80 dark:border-dark-border relative overflow-hidden">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
          <div className="flex flex-col md:flex-row justify-between items-center gap-8 border-b border-slate-800 pb-10 mb-10">
            <div className="flex items-center gap-3">
              <div className="bg-slate-800 p-2 rounded-xl border border-slate-700">
                <img src={curexmindLogo} alt="CurexMind Logo" className="h-8 w-8 rounded-md object-contain" />
              </div>
              <div>
                <span className="text-white font-bold text-xl font-cairo">CurexMind</span>
                <p className="text-[10px] text-slate-500 uppercase tracking-widest">{t('common.appName')}</p>
              </div>
            </div>
            
            <div className="flex items-center gap-6 text-sm font-semibold">
              <Link to="/login" className="hover:text-white transition-colors">{t('auth.login')}</Link>
              <Link to="/register" className="hover:text-white transition-colors">{t('auth.staffSignup')}</Link>
              <a href="https://github.com/mohamed68909/ClincManagement" target="_blank" rel="noreferrer" className="hover:text-white transition-colors">GitHub</a>
            </div>
          </div>
          
          <div className="flex flex-col md:flex-row justify-between items-center gap-4">
            <p className="text-xs font-cairo">
              &copy; {new Date().getFullYear()} CurexMind. {t('landing.rightsReserved')}
            </p>
            <p className="text-[10px] text-slate-600">
              Developed by Mohamed Ashraf &bull; Clean Architecture Clinique System
            </p>
          </div>
        </div>
      </footer>

    </div>
  );
};
