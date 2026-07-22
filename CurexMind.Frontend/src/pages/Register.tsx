import React, { useState } from 'react';
import { useNavigate, Link } from 'react-router-dom';
import { useSettingsStore } from '../store/settingsStore';
import { apiClient } from '../api/apiClient';
import { 
  User, 
  Mail, 
  Lock, 
  Phone, 
  MapPin, 
  Eye, 
  EyeOff, 
  Sparkles, 
  CheckCircle, 
  AlertCircle,
  UserPlus
} from 'lucide-react';
import Swal from 'sweetalert2';

export const Register: React.FC = () => {
  const navigate = useNavigate();
  const { language, t } = useSettingsStore();

  const [formData, setFormData] = useState({
    fullname: '',
    phone: '',
    email: '',
    username: '',
    address: '',
    password: '',
    password2: '',
  });

  const [showPassword, setShowPassword] = useState(false);
  const [showPassword2, setShowPassword2] = useState(false);
  const [termsAccepted, setTermsAccepted] = useState(false);
  const [loading, setLoading] = useState(false);

  const [alert, setAlert] = useState<{ message: string; type: 'success' | 'error' | null }>({
    message: '',
    type: null
  });

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    setFormData({
      ...formData,
      [e.target.name]: e.target.value
    });
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    if (formData.password !== formData.password2) {
      setAlert({ 
        message: language === 'ar' ? 'كلمات المرور غير متطابقة!' : 'Passwords do not match!', 
        type: 'error' 
      });
      return;
    }

    if (!termsAccepted) {
      setAlert({ 
        message: language === 'ar' ? 'يجب الموافقة على شروط الاستخدام والخصوصية!' : 'You must accept the terms of use and privacy policy!', 
        type: 'error' 
      });
      return;
    }

    setLoading(true);
    setAlert({ message: '', type: null });

    try {
      await apiClient.post('/api/Auth/sign-up', {
        fullName: formData.fullname.trim(),
        email: formData.email.trim(),
        userName: formData.username.trim(),
        password: formData.password.trim(),
        confirmPassword: formData.password2.trim(),
        address: formData.address.trim(),
        phoneNumber: formData.phone.trim()
      });

      setAlert({
        message: language === 'ar' ? 'تم التسجيل بنجاح! جاري تحويلك لصفحة الدخول...' : 'Registered successfully! Redirecting to login...',
        type: 'success'
      });

      Swal.fire({
        icon: 'success',
        title: language === 'ar' ? 'تم إنشاء الحساب بنجاح' : 'Account created successfully',
        text: language === 'ar' ? 'يمكنك الآن تسجيل الدخول للنظام.' : 'You can now sign in to the system.',
        timer: 2000,
        showConfirmButton: false
      });

      setTimeout(() => {
        navigate('/login');
      }, 2500);

    } catch (error: any) {
      console.error('Sign up error:', error);
      let errMsg = language === 'ar' ? 'خطأ في الاتصال بالخادم!' : 'Server connection error!';
      
      if (error.response && error.response.data) {
        const err = error.response.data;
        if (err.errors && Array.isArray(err.errors)) {
          errMsg = err.errors.map((e: any) => e.message).join(' - ');
        } else {
          errMsg = err.message || (language === 'ar' ? 'فشل التسجيل. يرجى مراجعة البيانات المدخلة.' : 'Registration failed. Please check input values.');
        }
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
          <div className={`mb-5 p-4 rounded-xl flex items-center gap-3 font-semibold text-sm ${
            alert.type === 'success' 
              ? 'bg-green-50 dark:bg-green-950/20 text-green-700 dark:text-green-400 border border-green-200/30 dark:border-green-900/40' 
              : 'bg-red-50 dark:bg-red-950/20 text-red-700 dark:text-red-400 border border-red-200/30 dark:border-red-900/40'
          }`}>
            {alert.type === 'success' ? <CheckCircle className="h-5 w-5 flex-shrink-0" /> : <AlertCircle className="h-5 w-5 flex-shrink-0" />}
            <span className="font-cairo text-xs">{alert.message}</span>
          </div>
        )}

        <div className="flex justify-between items-center mb-6">
          <div>
            <h1 className="text-2xl font-bold text-slate-900 dark:text-white font-cairo tracking-tight">{t('auth.register')}</h1>
            <p className="text-slate-450 dark:text-slate-500 text-xs mt-1.5 font-cairo">{t('auth.receptionistRegister')}</p>
          </div>
          <div className="bg-brand-50 dark:bg-brand-950/40 text-brand-600 dark:text-brand-400 p-2.5 rounded-2xl">
            <UserPlus className="h-6 w-6" />
          </div>
        </div>

        <form onSubmit={handleSubmit} className="space-y-4">
          <div>
            <label className="block text-xs font-bold text-slate-750 dark:text-slate-300 mb-1.5 font-cairo">{t('auth.fullName')}</label>
            <div className="relative">
              <input
                type="text"
                name="fullname"
                required
                value={formData.fullname}
                onChange={handleChange}
                placeholder={isRtl ? 'مثال: سارة محمد' : 'e.g. Sarah Connor'}
                className="w-full border border-slate-200 dark:border-dark-border bg-white dark:bg-slate-900 text-slate-900 dark:text-white focus:border-brand-500 focus:ring-2 focus:ring-brand-100 dark:focus:ring-brand-950/25 rounded-xl p-3 text-xs outline-none pr-9 pl-4 transition-all"
              />
              <User className={`absolute inset-y-0 ${isRtl ? 'right-0 pr-3.5' : 'left-0 pl-3.5'} flex items-center h-full text-slate-400 pointer-events-none`} size={16} />
            </div>
          </div>

          <div>
            <label className="block text-xs font-bold text-slate-750 dark:text-slate-300 mb-1.5 font-cairo">{t('common.phone')}</label>
            <div className="relative">
              <input
                type="tel"
                name="phone"
                required
                value={formData.phone}
                onChange={handleChange}
                placeholder="01xxxxxxxxx"
                className="w-full border border-slate-200 dark:border-dark-border bg-white dark:bg-slate-900 text-slate-900 dark:text-white focus:border-brand-500 focus:ring-2 focus:ring-brand-100 dark:focus:ring-brand-950/25 rounded-xl p-3 text-xs outline-none pr-9 pl-4 transition-all font-sans text-left"
                dir="ltr"
              />
              <Phone className={`absolute inset-y-0 ${isRtl ? 'right-0 pr-3.5' : 'left-0 pl-3.5'} flex items-center h-full text-slate-400 pointer-events-none`} size={16} />
            </div>
          </div>

          <div>
            <label className="block text-xs font-bold text-slate-750 dark:text-slate-300 mb-1.5 font-cairo">{t('auth.email')}</label>
            <div className="relative">
              <input
                type="email"
                name="email"
                required
                value={formData.email}
                onChange={handleChange}
                placeholder="example@domain.com"
                className="w-full border border-slate-200 dark:border-dark-border bg-white dark:bg-slate-900 text-slate-900 dark:text-white focus:border-brand-500 focus:ring-2 focus:ring-brand-100 dark:focus:ring-brand-950/25 rounded-xl p-3 text-xs outline-none pr-9 pl-4 transition-all font-sans text-left"
                dir="ltr"
              />
              <Mail className={`absolute inset-y-0 ${isRtl ? 'right-0 pr-3.5' : 'left-0 pl-3.5'} flex items-center h-full text-slate-400 pointer-events-none`} size={16} />
            </div>
          </div>

          <div className="grid grid-cols-2 gap-3">
            <div>
              <label className="block text-xs font-bold text-slate-750 dark:text-slate-300 mb-1.5 font-cairo">{isRtl ? 'اسم المستخدم' : 'Username'}</label>
              <input
                type="text"
                name="username"
                required
                value={formData.username}
                onChange={handleChange}
                placeholder="username"
                className="w-full border border-slate-200 dark:border-dark-border bg-white dark:bg-slate-900 text-slate-900 dark:text-white focus:border-brand-500 focus:ring-2 focus:ring-brand-100 dark:focus:ring-brand-950/25 rounded-xl p-3 text-xs outline-none px-3 transition-all font-sans text-left"
                dir="ltr"
              />
            </div>
            <div>
              <label className="block text-xs font-bold text-slate-750 dark:text-slate-300 mb-1.5 font-cairo">{t('auth.address')}</label>
              <div className="relative">
                <input
                  type="text"
                  name="address"
                  required
                  value={formData.address}
                  onChange={handleChange}
                  placeholder={isRtl ? 'القاهرة، مصر' : 'Cairo, Egypt'}
                  className="w-full border border-slate-200 dark:border-dark-border bg-white dark:bg-slate-900 text-slate-900 dark:text-white focus:border-brand-500 focus:ring-2 focus:ring-brand-100 dark:focus:ring-brand-950/25 rounded-xl p-3 text-xs outline-none pr-9 pl-3 transition-all"
                />
                <MapPin className={`absolute inset-y-0 ${isRtl ? 'right-0 pr-3.5' : 'left-0 pl-3.5'} flex items-center h-full text-slate-400 pointer-events-none`} size={16} />
              </div>
            </div>
          </div>

          <div>
            <label className="block text-xs font-bold text-slate-750 dark:text-slate-300 mb-1.5 font-cairo">{t('auth.password')}</label>
            <div className="relative">
              <input
                type={showPassword ? 'text' : 'password'}
                name="password"
                required
                value={formData.password}
                onChange={handleChange}
                placeholder="••••••••"
                className="w-full border border-slate-200 dark:border-dark-border bg-white dark:bg-slate-900 text-slate-900 dark:text-white focus:border-brand-500 focus:ring-2 focus:ring-brand-100 dark:focus:ring-brand-950/25 rounded-xl p-3 text-xs outline-none pr-9 pl-9 transition-all font-sans"
              />
              <Lock className={`absolute inset-y-0 ${isRtl ? 'right-0 pr-3.5' : 'left-0 pl-3.5'} flex items-center h-full text-slate-400 pointer-events-none`} size={16} />
              <button
                type="button"
                onClick={() => setShowPassword(!showPassword)}
                className={`absolute inset-y-0 ${isRtl ? 'left-0 pl-3.5' : 'right-0 pr-3.5'} flex items-center text-slate-400 hover:text-slate-655 dark:hover:text-slate-350`}
              >
                {showPassword ? <EyeOff size={14} /> : <Eye size={14} />}
              </button>
            </div>
          </div>

          <div>
            <label className="block text-xs font-bold text-slate-750 dark:text-slate-300 mb-1.5 font-cairo">{t('auth.confirmPassword')}</label>
            <div className="relative">
              <input
                type={showPassword2 ? 'text' : 'password'}
                name="password2"
                required
                value={formData.password2}
                onChange={handleChange}
                placeholder="••••••••"
                className="w-full border border-slate-200 dark:border-dark-border bg-white dark:bg-slate-900 text-slate-900 dark:text-white focus:border-brand-500 focus:ring-2 focus:ring-brand-100 dark:focus:ring-brand-950/25 rounded-xl p-3 text-xs outline-none pr-9 pl-9 transition-all font-sans"
              />
              <Lock className={`absolute inset-y-0 ${isRtl ? 'right-0 pr-3.5' : 'left-0 pl-3.5'} flex items-center h-full text-slate-400 pointer-events-none`} size={16} />
              <button
                type="button"
                onClick={() => setShowPassword2(!showPassword2)}
                className={`absolute inset-y-0 ${isRtl ? 'left-0 pl-3.5' : 'right-0 pr-3.5'} flex items-center text-slate-400 hover:text-slate-655 dark:hover:text-slate-350`}
              >
                {showPassword2 ? <EyeOff size={14} /> : <Eye size={14} />}
              </button>
            </div>
          </div>

          <div className="flex items-center gap-2 pt-1">
            <input
              type="checkbox"
              id="terms"
              checked={termsAccepted}
              onChange={(e) => setTermsAccepted(e.target.checked)}
              className="h-4 w-4 text-brand-600 border-slate-300 rounded focus:ring-brand-500 cursor-pointer"
            />
            <label htmlFor="terms" className="text-xs text-slate-600 dark:text-slate-400 cursor-pointer font-cairo select-none">
              {isRtl ? 'أوافق على' : 'I agree to the'}{' '}
              <a href="#" className="text-brand-600 dark:text-brand-400 hover:underline font-bold">
                {isRtl ? 'الشروط وسياسة الخصوصية' : 'Terms & Privacy Policy'}
              </a>
            </label>
          </div>

          <button
            type="submit"
            disabled={loading}
            className="w-full bg-brand-600 hover:bg-brand-700 text-white font-bold py-3 rounded-xl transition-all shadow-md hover:shadow-lg focus:ring-2 focus:ring-brand-100 dark:focus:ring-brand-950/30 font-cairo disabled:bg-brand-400 disabled:cursor-not-allowed flex items-center justify-center gap-2 mt-2 text-sm cursor-pointer"
          >
            {loading ? t('auth.signingUp') : t('auth.register')}
          </button>

          <div className="flex items-center my-4">
            <div className="flex-1 h-px bg-slate-200/50 dark:bg-slate-800"></div>
            <span className="text-[10px] text-slate-400 dark:text-slate-500 px-2 font-cairo uppercase font-bold">{isRtl ? 'أو' : 'Or'}</span>
            <div className="flex-1 h-px bg-slate-200/50 dark:bg-slate-800"></div>
          </div>

          <p className="text-xs text-center text-slate-550 dark:text-slate-400 font-cairo">
            {isRtl ? 'لديك حساب بالفعل؟' : 'Already have an account?'}{' '}
            <Link to="/login" className="text-brand-600 dark:text-brand-400 hover:underline font-extrabold ml-1">
              {t('auth.login')}
            </Link>
          </p>
        </form>
      </div>

      {/* Promo Section */}
      <div className="max-w-md hidden md:block animate-fade-in-up">
        <div className={`bg-white dark:bg-dark-surface shadow-xl rounded-[2rem] p-8 mb-8 w-64 ${isRtl ? 'rotate-[-4deg]' : 'rotate-[4deg]'} mx-auto border border-slate-200/40 dark:border-dark-border relative overflow-hidden transition-colors duration-300`}>
          <div className={`absolute top-0 ${isRtl ? 'right-0 rounded-bl-3xl' : 'left-0 rounded-br-3xl'} bg-brand-50 dark:bg-brand-950/50 p-2.5 text-brand-600 dark:text-brand-400`}>
            <Sparkles className="h-4.5 w-4.5" />
          </div>
          <div className="text-4xl text-center mb-6">👩‍⚕️</div>
          <div className="grid grid-cols-2 gap-4 justify-items-center">
            <div className="w-14 h-14 bg-brand-50 dark:bg-brand-950/30 text-brand-600 dark:text-brand-400 flex items-center justify-center rounded-2xl text-2xl shadow-sm border border-brand-100/10 hover:scale-105 transition-transform">📅</div>
            <div className="w-14 h-14 bg-emerald-50 dark:bg-emerald-950/30 text-emerald-600 dark:text-emerald-400 flex items-center justify-center rounded-2xl text-2xl shadow-sm border border-emerald-100/10 hover:scale-105 transition-transform">📋</div>
            <div className="w-14 h-14 bg-rose-50 dark:bg-rose-950/30 text-rose-600 dark:text-rose-455 flex items-center justify-center rounded-2xl text-2xl shadow-sm border border-rose-100/10 hover:scale-105 transition-transform">❤️</div>
            <div className="w-14 h-14 bg-indigo-50 dark:bg-indigo-950/30 text-indigo-600 dark:text-indigo-400 flex items-center justify-center rounded-2xl text-2xl shadow-sm border border-indigo-100/10 hover:scale-105 transition-transform">👩‍⚕️</div>
          </div>
        </div>

        <h2 className="text-3xl font-extrabold text-slate-900 dark:text-white mb-4 font-cairo leading-tight">
          {isRtl ? 'نظم العمل بسهولة مع CurexMind' : 'Organize your work with CurexMind'}
        </h2>
        <p className="text-slate-550 dark:text-slate-400 leading-relaxed font-cairo text-sm">
          {isRtl 
            ? 'انضم لموظفي الاستقبال الذين يعتمدون على CurexMind يومياً لإدارة المواعيد وتسهيل استقبال المرضى وزيادة جودة الرعاية الطبية.'
            : 'Join receptionist teams that rely on CurexMind daily to manage appointments, welcome patients, and improve healthcare delivery.'}
        </p>
      </div>

    </div>
  );
};
