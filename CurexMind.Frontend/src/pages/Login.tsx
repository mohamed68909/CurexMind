import React, { useState } from 'react';
import { useNavigate, Link } from 'react-router-dom';
import { useAuthStore } from '../store/authStore';
import { useSettingsStore } from '../store/settingsStore';
import { apiClient } from '../api/apiClient';
import { Eye, EyeOff, Lock, Mail, Activity, Sparkles, CheckCircle, AlertCircle } from 'lucide-react';
import Swal from 'sweetalert2';

export const Login: React.FC = () => {
  const navigate = useNavigate();
  const loginStore = useAuthStore().login;
  const { language, t } = useSettingsStore();
  
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [showPassword, setShowPassword] = useState(false);
  const [loading, setLoading] = useState(false);
  
  const [alert, setAlert] = useState<{ message: string; type: 'success' | 'error' | null }>({
    message: '',
    type: null
  });

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setLoading(true);
    setAlert({ message: '', type: null });

    try {
      const response = await apiClient.post('/api/Auth/sign-in', {
        email: email.trim(),
        password: password.trim()
      });

      const data = response.data;
      
      // Save in Zustand and localstorage
      loginStore(data.token, data.refreshToken, data.fullName);

      setAlert({
        message: language === 'ar' ? 'تم تسجيل الدخول بنجاح! جاري التوجيه...' : 'Logged in successfully! Redirecting...',
        type: 'success'
      });

      // SweetAlert2 notification
      Swal.fire({
        icon: 'success',
        title: t('common.welcome') + ' in CurexMind',
        text: language === 'ar' ? 'تم تسجيل الدخول بنجاح!' : 'Logged in successfully!',
        timer: 1500,
        showConfirmButton: false,
        position: 'center'
      });

      setTimeout(() => {
        // Decode roles to redirect accordingly
        const roles = useAuthStore.getState().roles;
        const currentEmail = email.trim().toLowerCase();

        // Redirect receptionist/admin roles or fallback for the dev user
        if (roles.includes('Receptionist') || roles.includes('Admin') || currentEmail === 'dev@mohamed.com') {
          navigate('/management');
        } else {
          navigate('/patient');
        }
      }, 1500);

    } catch (error: any) {
      console.error('Sign in error:', error);
      let errMsg = language === 'ar' ? 'خطأ في الاتصال بالخادم!' : 'Error connecting to the server!';
      if (error.response && error.response.status === 400) {
        errMsg = language === 'ar' ? 'خطأ في تسجيل الدخول! يرجى التأكد من البيانات.' : 'Invalid email or password!';
      }
      setAlert({
        message: errMsg,
        type: 'error'
      });
    } finally {
      setLoading(false);
    }
  };

  const isRtl = language === 'ar';
  const flexDirClass = isRtl ? 'md:flex-row' : 'md:flex-row-reverse';
  const textAlignment = isRtl ? 'text-right' : 'text-left';

  return (
    <div className={`w-full max-w-5xl flex flex-col ${flexDirClass} items-center justify-center gap-12 p-4 md:p-8 ${textAlignment} font-sans`} dir={isRtl ? 'rtl' : 'ltr'}>
      
      {/* Form Section */}
      <div className="bg-white dark:bg-dark-surface rounded-[2rem] shadow-xl p-8 w-full max-w-md border border-slate-200/50 dark:border-dark-border transition-colors duration-350 animate-scale-in">
        
        {/* Top Alert */}
        {alert.type && (
          <div className={`mb-6 p-4 rounded-xl flex items-center gap-3 font-semibold text-sm ${
            alert.type === 'success' 
              ? 'bg-green-50 dark:bg-green-950/20 text-green-700 dark:text-green-400 border border-green-200/30 dark:border-green-900/40' 
              : 'bg-red-50 dark:bg-red-950/20 text-red-700 dark:text-red-400 border border-red-200/30 dark:border-red-900/40'
          }`}>
            {alert.type === 'success' ? <CheckCircle className="h-5 w-5 flex-shrink-0" /> : <AlertCircle className="h-5 w-5 flex-shrink-0" />}
            <span className="font-cairo text-xs">{alert.message}</span>
          </div>
        )}

        <div className="flex justify-between items-center mb-8">
          <div>
            <h1 className="text-3xl font-extrabold text-slate-900 dark:text-white font-cairo tracking-tight">{t('auth.login')}</h1>
            <p className="text-slate-450 dark:text-slate-500 text-xs mt-1.5 font-cairo">{t('auth.loginToPortal')}</p>
          </div>
          <div className="bg-brand-50 dark:bg-brand-950/40 text-brand-600 dark:text-brand-400 p-3.5 rounded-2xl">
            <Activity className="h-6 w-6" />
          </div>
        </div>

        <form onSubmit={handleSubmit} className="space-y-6">
          <div>
            <label className="block text-xs font-bold text-slate-700 dark:text-slate-350 mb-2 font-cairo">{t('auth.email')}</label>
            <div className="relative">
              <input
                type="email"
                required
                value={email}
                onChange={(e) => setEmail(e.target.value)}
                placeholder="example@domain.com"
                className="w-full border border-slate-200 dark:border-dark-border bg-white dark:bg-slate-900 text-slate-900 dark:text-white focus:border-brand-500 focus:ring-2 focus:ring-brand-100 dark:focus:ring-brand-950/25 rounded-xl p-3.5 text-sm outline-none pr-10 pl-4 transition-all placeholder:text-slate-350 dark:placeholder:text-slate-600 font-sans"
              />
              <Mail className={`absolute inset-y-0 ${isRtl ? 'right-0 pr-3.5' : 'left-0 pl-3.5'} flex items-center h-full text-slate-400 pointer-events-none`} size={18} />
            </div>
          </div>

          <div>
            <label className="block text-xs font-bold text-slate-700 dark:text-slate-350 mb-2 font-cairo">{t('auth.password')}</label>
            <div className="relative">
              <input
                type={showPassword ? 'text' : 'password'}
                required
                value={password}
                onChange={(e) => setPassword(e.target.value)}
                placeholder="••••••••"
                className="w-full border border-slate-200 dark:border-dark-border bg-white dark:bg-slate-900 text-slate-900 dark:text-white focus:border-brand-500 focus:ring-2 focus:ring-brand-100 dark:focus:ring-brand-950/25 rounded-xl p-3.5 text-sm outline-none pr-10 pl-10 transition-all placeholder:text-slate-350 dark:placeholder:text-slate-600 font-sans"
              />
              <Lock className={`absolute inset-y-0 ${isRtl ? 'right-0 pr-3.5' : 'left-0 pl-3.5'} flex items-center h-full text-slate-400 pointer-events-none`} size={18} />
              <button
                type="button"
                onClick={() => setShowPassword(!showPassword)}
                className={`absolute inset-y-0 ${isRtl ? 'left-0 pl-3.5' : 'right-0 pr-3.5'} flex items-center text-slate-400 hover:text-slate-600 dark:hover:text-slate-300`}
              >
                {showPassword ? <EyeOff size={16} /> : <Eye size={16} />}
              </button>
            </div>
          </div>

          <button
            type="submit"
            disabled={loading}
            className="w-full bg-brand-600 hover:bg-brand-700 text-white font-extrabold py-3.5 rounded-xl transition-all shadow-md hover:shadow-lg hover:shadow-brand-500/10 focus:ring-2 focus:ring-brand-100 dark:focus:ring-brand-950/30 font-cairo disabled:bg-brand-400 disabled:cursor-not-allowed flex items-center justify-center gap-2 cursor-pointer text-sm"
          >
            {loading ? t('auth.signingIn') : t('auth.login')}
          </button>

          <div className="flex items-center my-6">
            <div className="flex-1 h-px bg-slate-200/50 dark:bg-slate-800"></div>
            <span className="text-[10px] text-slate-400 dark:text-slate-500 px-3 font-cairo uppercase font-bold">{isRtl ? 'أو' : 'Or'}</span>
            <div className="flex-1 h-px bg-slate-200/50 dark:bg-slate-800"></div>
          </div>

          <p className="text-xs text-center text-slate-550 dark:text-slate-400 font-cairo">
            {isRtl ? 'ليس لديك حساب مريض؟' : "Don't have a patient account?"}{' '}
            <Link to="/register" className="text-brand-600 dark:text-brand-400 hover:underline font-extrabold ml-1">
              {isRtl ? 'إنشاء حساب جديد' : 'Register here'}
            </Link>
          </p>
        </form>
      </div>

      {/* Promo / Info Section */}
      <div className="max-w-md hidden md:block animate-fade-in-up">
        <div className={`bg-white dark:bg-dark-surface shadow-xl rounded-[2rem] p-8 mb-8 w-64 ${isRtl ? 'rotate-[-4deg]' : 'rotate-[4deg]'} mx-auto border border-slate-200/40 dark:border-dark-border relative overflow-hidden transition-colors duration-300`}>
          <div className={`absolute top-0 ${isRtl ? 'right-0 rounded-bl-3xl' : 'left-0 rounded-br-3xl'} bg-brand-50 dark:bg-brand-950/50 p-2.5 text-brand-600 dark:text-brand-400`}>
            <Sparkles className="h-4.5 w-4.5" />
          </div>
          <div className="text-4xl text-center mb-6">🔐</div>
          <div className="grid grid-cols-2 gap-4 justify-items-center">
            <div className="w-14 h-14 bg-brand-50 dark:bg-brand-950/30 text-brand-600 dark:text-brand-400 flex items-center justify-center rounded-2xl text-2xl shadow-sm border border-brand-100/10 hover:scale-105 transition-transform">💬</div>
            <div className="w-14 h-14 bg-emerald-50 dark:bg-emerald-950/30 text-emerald-600 dark:text-emerald-400 flex items-center justify-center rounded-2xl text-2xl shadow-sm border border-emerald-100/10 hover:scale-105 transition-transform">📁</div>
            <div className="w-14 h-14 bg-rose-50 dark:bg-rose-950/30 text-rose-600 dark:text-rose-400 flex items-center justify-center rounded-2xl text-2xl shadow-sm border border-rose-100/10 hover:scale-105 transition-transform">💡</div>
            <div className="w-14 h-14 bg-indigo-50 dark:bg-indigo-950/30 text-indigo-600 dark:text-indigo-400 flex items-center justify-center rounded-2xl text-2xl shadow-sm border border-indigo-100/10 hover:scale-105 transition-transform">⚙️</div>
          </div>
        </div>

        <h2 className="text-3xl font-extrabold text-slate-900 dark:text-white mb-4 font-cairo leading-tight">
          {isRtl ? 'سجل دخولك بسهولة مع CurexMind' : 'Sign in easily to CurexMind'}
        </h2>
        <p className="text-slate-550 dark:text-slate-400 leading-relaxed font-cairo text-sm">
          {isRtl 
            ? 'ادخل للإدارة الكاملة للمواعيد والمرضى وجميع أدوات الاستقبال واستمتع بواجهة سريعة وآمنة ومصممة لتسهيل رعاية المرضى.'
            : 'Access the complete system for managing appointments, patients, and reception tools. Enjoy a fast, secure, and beautiful interface designed to streamline patient care.'}
        </p>
      </div>

    </div>
  );
};
