import React, { useEffect, useState } from 'react';
import { apiClient } from '../api/apiClient';
import { useSettingsStore } from '../store/settingsStore';
import { useAuthStore } from '../store/authStore';
import { 
  Users, 
  CalendarCheck, 
  DollarSign, 
  UserPlus, 
  Plus, 
  FileText,
  Loader2,
  AlertCircle,
  ArrowRight,
  TrendingUp,
  Activity
} from 'lucide-react';
import { Link } from 'react-router-dom';

interface Appointment {
  time: string;
  patient: string;
  doctor: string;
  status: string;
}

interface Invoice {
  patient: string;
  amountEGP: number;
  status: string;
}

interface SummaryData {
  totalPatients: number;
  todayAppointmentsCount: number;
  unpaidInvoicesCount: number;
  unpaidInvoicesAmountEGP: number;
  newPatientsToday: number;
  todayAppointments: Appointment[];
  invoicesToFollowUp: Invoice[];
}

export const Dashboard: React.FC = () => {
  const [data, setData] = useState<SummaryData | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const { language, t } = useSettingsStore();

  const { userId } = useAuthStore();

  useEffect(() => {
    const fetchSummary = async () => {
      try {
        setLoading(true);
        const currentUserId = userId || '4E14506C-D3C0-4AE3-8616-5EB95A764358';
        const response = await apiClient.get(`/api/Dashboard/receptionist-summary/${currentUserId}`);
        setData(response.data);
      } catch (err: any) {
        console.error('Failed to load receptionist dashboard:', err);
        setError(language === 'ar' 
          ? 'تعذر تحميل بيانات لوحة التحكم. يرجى التحقق من اتصالك بالإنترنت.' 
          : 'Failed to load dashboard data. Please check your internet connection.');
      } finally {
        setLoading(false);
      }
    };

    fetchSummary();
  }, [language]);

  if (loading) {
    return (
      <div className="flex flex-col items-center justify-center min-h-[450px] gap-3">
        <div className="relative flex items-center justify-center">
          <Loader2 className="h-10 w-10 text-brand-650 animate-spin" />
          <Activity className="absolute h-5 w-5 text-brand-400 animate-pulse-slow" />
        </div>
        <p className="text-slate-500 dark:text-slate-400 font-bold text-xs font-cairo">{t('common.loading')}</p>
      </div>
    );
  }

  if (error) {
    return (
      <div className="bg-red-50 dark:bg-red-950/20 border border-red-200/50 dark:border-red-900/40 text-red-700 dark:text-red-400 p-6 rounded-2xl flex items-start gap-4 shadow-sm animate-scale-in">
        <AlertCircle className="h-6 w-6 flex-shrink-0 mt-0.5" />
        <div>
          <h3 className="font-extrabold text-sm font-cairo">{language === 'ar' ? 'فشل جلب البيانات' : 'Failed to fetch data'}</h3>
          <p className="text-xs mt-1 font-cairo">{error}</p>
        </div>
      </div>
    );
  }

  const isRtl = language === 'ar';
  const textAlignment = isRtl ? 'text-right' : 'text-left';

  return (
    <div className={`space-y-8 ${textAlignment} animate-fade-in-up`} dir={isRtl ? 'rtl' : 'ltr'}>
      {/* Welcome Card Row for Mobile/Tablet */}
      <div className="sm:hidden bg-gradient-to-br from-brand-600 to-indigo-850 text-white p-6 rounded-2xl shadow-md">
        <h2 className="text-xl font-bold font-cairo">{t('dashboard.welcome')}</h2>
        <p className="text-[10px] text-brand-100 font-semibold mt-1.5 uppercase">
          {new Date().toLocaleDateString(isRtl ? 'ar-EG' : 'en-US', { weekday: 'long', year: 'numeric', month: 'long', day: 'numeric' })}
        </p>
      </div>

      {/* Stats Cards Grid Redesigned to match Figma */}
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
        
        {/* Total Patients Card */}
        <div className="bg-white dark:bg-dark-surface p-6 rounded-2xl border border-slate-200/40 dark:border-dark-border premium-shadow flex justify-between items-start hover:-translate-y-1 hover:shadow-lg transition-all duration-300">
          <div className="space-y-2">
            <p className="text-[10px] font-extrabold text-brand-600 dark:text-brand-400 uppercase tracking-widest font-cairo">{t('dashboard.totalPatients')}</p>
            <h3 className="text-3xl font-extrabold text-slate-900 dark:text-white">{data?.totalPatients ?? 0}</h3>
            <div className="flex items-center gap-1 text-[10px] text-emerald-600 font-bold">
              <TrendingUp className="h-3 w-3" />
              <span>+12% {isRtl ? 'هذا الشهر' : 'this month'}</span>
            </div>
          </div>
          <div className="text-brand-600 bg-brand-50 dark:bg-brand-950/40 p-3 rounded-xl">
            <Users className="h-5 w-5" />
          </div>
        </div>

        {/* Today's Appointments Card */}
        <div className="bg-white dark:bg-dark-surface p-6 rounded-2xl border border-slate-200/40 dark:border-dark-border premium-shadow flex justify-between items-start hover:-translate-y-1 hover:shadow-lg transition-all duration-300">
          <div className="space-y-2">
            <p className="text-[10px] font-extrabold text-emerald-600 dark:text-emerald-450 uppercase tracking-widest font-cairo">{t('dashboard.todayAppointments')}</p>
            <h3 className="text-3xl font-extrabold text-slate-900 dark:text-white">{data?.todayAppointmentsCount ?? 0}</h3>
            <p className="text-[10px] text-slate-400 dark:text-slate-500 font-semibold">{isRtl ? 'المواعيد المجدولة اليوم' : 'Scheduled for today'}</p>
          </div>
          <div className="text-emerald-600 bg-emerald-50 dark:bg-emerald-950/40 p-3 rounded-xl">
            <CalendarCheck className="h-5 w-5" />
          </div>
        </div>

        {/* Unpaid Invoices Card */}
        <div className="bg-white dark:bg-dark-surface p-6 rounded-2xl border border-slate-200/40 dark:border-dark-border premium-shadow flex justify-between items-start hover:-translate-y-1 hover:shadow-lg transition-all duration-300">
          <div className="space-y-2">
            <p className="text-[10px] font-extrabold text-rose-600 dark:text-rose-450 uppercase tracking-widest font-cairo">{t('dashboard.totalInvoices')}</p>
            <h3 className="text-3xl font-extrabold text-slate-900 dark:text-white">{data?.unpaidInvoicesCount ?? 0}</h3>
            <p className="text-[10px] text-rose-600 dark:text-rose-400 font-bold bg-rose-50 dark:bg-rose-950/20 px-2 py-0.5 rounded w-fit">
              {(data?.unpaidInvoicesAmountEGP ?? 0).toLocaleString()} {t('patient.currency')}
            </p>
          </div>
          <div className="text-rose-600 bg-rose-50 dark:bg-rose-950/40 p-3 rounded-xl">
            <DollarSign className="h-5 w-5" />
          </div>
        </div>

        {/* New Patients Today Card */}
        <div className="bg-white dark:bg-dark-surface p-6 rounded-2xl border border-slate-200/40 dark:border-dark-border premium-shadow flex justify-between items-start hover:-translate-y-1 hover:shadow-lg transition-all duration-300">
          <div className="space-y-2">
            <p className="text-[10px] font-extrabold text-indigo-600 dark:text-indigo-400 uppercase tracking-widest font-cairo">{isRtl ? 'مرضى جدد اليوم' : 'New Patients Today'}</p>
            <h3 className="text-3xl font-extrabold text-slate-900 dark:text-white">{data?.newPatientsToday ?? 0}</h3>
            <p className="text-[10px] text-slate-450 dark:text-slate-500 font-semibold">{isRtl ? 'المسجلين اليوم فقط' : 'Registered today'}</p>
          </div>
          <div className="text-indigo-600 bg-indigo-50 dark:bg-indigo-950/40 p-3 rounded-xl">
            <UserPlus className="h-5 w-5" />
          </div>
        </div>

      </div>

      {/* Main Content Layout */}
      <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">
        
        {/* Appointments Table Section */}
        <div className="lg:col-span-2 bg-white dark:bg-dark-surface rounded-2xl border border-slate-200/40 dark:border-dark-border p-6 premium-shadow transition-colors duration-300">
          <div className="flex justify-between items-center mb-6">
            <div className="flex items-center gap-2">
              <span className="h-4 w-1 bg-brand-500 rounded-full"></span>
              <h3 className="font-extrabold text-slate-800 dark:text-white text-base font-cairo">{t('dashboard.todayAppointments')}</h3>
            </div>
            <Link 
              to="/management/appointments" 
              className="text-xs font-bold text-brand-600 dark:text-brand-400 bg-brand-50 dark:bg-brand-950/40 px-3 py-1.5 rounded-xl hover:bg-brand-100 dark:hover:bg-brand-950/60 transition-all"
            >
              {t('dashboard.viewAll')}
            </Link>
          </div>
          
          <div className="overflow-x-auto rounded-xl">
            <table className="w-full text-sm border-collapse">
              <thead>
                <tr className="bg-slate-50 dark:bg-slate-900/60 text-slate-550 dark:text-slate-400 text-xs font-bold border-b border-slate-100 dark:border-dark-border">
                  <th className={`p-4 ${isRtl ? 'text-right' : 'text-left'} font-semibold font-cairo`}>{isRtl ? 'الوقت' : 'Time'}</th>
                  <th className={`p-4 ${isRtl ? 'text-right' : 'text-left'} font-semibold font-cairo`}>{isRtl ? 'المريض' : 'Patient'}</th>
                  <th className={`p-4 ${isRtl ? 'text-right' : 'text-left'} font-semibold font-cairo`}>{isRtl ? 'الطبيب' : 'Doctor'}</th>
                  <th className={`p-4 ${isRtl ? 'text-left' : 'text-right'} font-semibold font-cairo`}>{t('common.status')}</th>
                </tr>
              </thead>
              <tbody className="text-slate-700 dark:text-slate-350 divide-y divide-slate-100 dark:divide-slate-800">
                {!data?.todayAppointments || data.todayAppointments.length === 0 ? (
                  <tr>
                    <td colSpan={4} className="text-center py-10 text-slate-400 dark:text-slate-500 font-bold font-cairo">
                      {t('dashboard.noAppointments')}
                    </td>
                  </tr>
                ) : (
                  data.todayAppointments.map((app, idx) => {
                    let statusClass = "bg-slate-100 text-slate-750 dark:bg-slate-800 dark:text-slate-400 border-slate-200 dark:border-slate-700";
                    if (app.status === 'Confirmed') statusClass = "bg-emerald-50 text-emerald-700 dark:bg-emerald-950/20 dark:text-emerald-400 border-emerald-100/30 dark:border-emerald-900/40";
                    else if (app.status === 'Waiting') statusClass = "bg-brand-50 text-brand-700 dark:bg-brand-950/20 dark:text-brand-400 border-brand-100/30 dark:border-brand-900/40";
                    else if (app.status === 'Missed' || app.status === 'Cancelled') statusClass = "bg-rose-50 text-red-700 dark:bg-rose-950/20 dark:text-red-400 border-red-100/30 dark:border-red-900/40";

                    return (
                      <tr key={idx} className="hover:bg-slate-50/50 dark:hover:bg-slate-900/20 transition-colors">
                        <td className="p-4 font-bold text-slate-900 dark:text-white font-sans">{app.time}</td>
                        <td className="p-4 font-medium capitalize font-cairo">{app.patient}</td>
                        <td className="p-4 text-slate-500 dark:text-slate-450 font-cairo">{app.doctor}</td>
                        <td className={`p-4 ${isRtl ? 'text-left' : 'text-right'}`}>
                          <span className={`px-2.5 py-1 rounded-full text-[10px] font-extrabold border ${statusClass} font-cairo`}>
                            {app.status}
                          </span>
                        </td>
                      </tr>
                    );
                  })
                )}
              </tbody>
            </table>
          </div>
        </div>

        {/* Invoices to Follow Up Section */}
        <div className="bg-white dark:bg-dark-surface rounded-2xl border border-slate-200/40 dark:border-dark-border p-6 premium-shadow flex flex-col justify-between transition-colors duration-300">
          <div>
            <div className="flex justify-between items-center mb-6">
              <div className="flex items-center gap-2">
                <span className="h-4 w-1 bg-brand-500 rounded-full"></span>
                <h3 className="font-extrabold text-slate-800 dark:text-white text-base font-cairo">{isRtl ? 'متابعة الفواتير' : 'Invoices to Follow Up'}</h3>
              </div>
              <Link 
                to="/management/invoices" 
                className="text-xs font-bold text-brand-600 dark:text-brand-400 bg-brand-50 dark:bg-brand-950/40 px-3 py-1.5 rounded-xl hover:bg-brand-100 dark:hover:bg-brand-950/60 transition-all"
              >
                {t('dashboard.viewAll')}
              </Link>
            </div>
            
            <div className="space-y-4 max-h-[300px] overflow-y-auto pr-1">
              {!data?.invoicesToFollowUp || data.invoicesToFollowUp.length === 0 ? (
                <p className="text-center text-slate-400 dark:text-slate-500 text-sm font-semibold py-8 font-cairo">{isRtl ? 'لا توجد فواتير معلقة.' : 'No pending invoices.'}</p>
              ) : (
                data.invoicesToFollowUp.map((inv, idx) => {
                  let statusColor = "text-slate-600 bg-slate-50 dark:bg-slate-900 dark:text-slate-400 border-slate-200/50 dark:border-dark-border";
                  let amountColor = "text-slate-850 dark:text-slate-200";
                  
                  if (inv.status === 'Unpaid') {
                    statusColor = "text-red-700 bg-red-50 dark:bg-red-950/20 dark:text-red-400 border-red-100/30 dark:border-red-900/40";
                    amountColor = "text-red-650 dark:text-red-400";
                  } else if (inv.status === 'Partial') {
                    statusColor = "text-amber-700 bg-amber-50 dark:bg-amber-950/20 dark:text-amber-400 border-amber-100/30 dark:border-amber-900/40";
                    amountColor = "text-amber-600 dark:text-amber-400";
                  }

                  return (
                    <div key={idx} className="flex justify-between items-center pb-4 border-b border-slate-100 dark:border-slate-800 last:border-0 last:pb-0">
                      <div className="space-y-1">
                        <h4 className="text-sm font-bold text-slate-900 dark:text-white capitalize font-cairo">{inv.patient}</h4>
                        <p className={`text-xs font-extrabold ${amountColor}`}>
                          {(inv.amountEGP || 0).toLocaleString()} {t('patient.currency')}
                        </p>
                      </div>
                      <span className={`px-2 py-0.5 rounded-lg text-[9px] font-extrabold border uppercase ${statusColor} font-cairo`}>
                        {inv.status}
                      </span>
                    </div>
                  );
                })
              )}
            </div>
          </div>
        </div>

      </div>

      {/* Quick Actions Row Redesigned with custom icons & gradients */}
      <div className="bg-white dark:bg-dark-surface rounded-2xl border border-slate-200/40 dark:border-dark-border p-6 premium-shadow transition-colors duration-300">
        <div className="flex items-center gap-2 mb-6">
          <span className="h-4 w-1 bg-brand-500 rounded-full"></span>
          <h3 className="font-extrabold text-slate-800 dark:text-white text-base font-cairo">{isRtl ? 'إجراءات سريعة' : 'Quick Actions'}</h3>
        </div>
        <div className="grid grid-cols-1 sm:grid-cols-3 gap-5">
          
          <Link 
            to="/management/patients/add" 
            className="flex flex-col items-center justify-center p-6 rounded-2xl bg-gradient-to-br from-brand-500 to-brand-600 hover:from-brand-600 hover:to-brand-700 text-white hover:scale-[1.02] hover:shadow-lg hover:shadow-brand-500/10 transition-all duration-300 gap-3 group"
          >
            <div className="p-3 bg-white/10 rounded-xl group-hover:scale-110 transition-transform">
              <Plus className="h-6 w-6" />
            </div>
            <span className="text-sm font-extrabold font-cairo">{isRtl ? 'إضافة مريض جديد' : 'Add New Patient'}</span>
          </Link>

          <Link 
            to="/management/appointments/add" 
            className="flex flex-col items-center justify-center p-6 rounded-2xl bg-gradient-to-br from-emerald-500 to-teal-650 hover:from-emerald-600 hover:to-teal-700 text-white hover:scale-[1.02] hover:shadow-lg hover:shadow-emerald-500/10 transition-all duration-300 gap-3 group"
          >
            <div className="p-3 bg-white/10 rounded-xl group-hover:scale-110 transition-transform">
              <CalendarCheck className="h-6 w-6" />
            </div>
            <span className="text-sm font-extrabold font-cairo">{isRtl ? 'حجز موعد طبي' : 'Book Appointment'}</span>
          </Link>

          <Link 
            to="/management/invoices/add" 
            className="flex flex-col items-center justify-center p-6 rounded-2xl bg-gradient-to-br from-indigo-500 to-indigo-650 hover:from-indigo-600 hover:to-indigo-700 text-white hover:scale-[1.02] hover:shadow-lg hover:shadow-indigo-500/10 transition-all duration-300 gap-3 group"
          >
            <div className="p-3 bg-white/10 rounded-xl group-hover:scale-110 transition-transform">
              <FileText className="h-6 w-6" />
            </div>
            <span className="text-sm font-extrabold font-cairo">{isRtl ? 'إنشاء فاتورة' : 'Create Invoice'}</span>
          </Link>

        </div>
      </div>
    </div>
  );
};
