import React, { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import { apiClient } from '../api/apiClient';
import { useAuthStore } from '../store/authStore';
import { Calendar, Clock, MapPin, Loader2, AlertCircle, Trash2, CheckCircle2, AlertTriangle } from 'lucide-react';
import Swal from 'sweetalert2';

interface Appointment {
  id: string;
  doctorId?: string;
  doctorName: string;
  doctorSpecialization: string;
  appointmentDate: string;
  status: string;
  isPaid: boolean;
  clinicAddress?: string;
}

export const PatientAppointments: React.FC = () => {
  const { patientId } = useAuthStore();
  const [appointments, setAppointments] = useState<Appointment[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [activeTab, setActiveTab] = useState<'all' | 'upcoming' | 'past' | 'cancelled'>('all');

  const fetchAppointments = async () => {
    if (!patientId) {
      setError('لم يتم العثور على معرف المريض. يرجى تسجيل الدخول مجدداً.');
      setLoading(false);
      return;
    }
    try {
      setLoading(true);
      setError(null);
      const response = await apiClient.get(`/api/Appointments/patient/${patientId}`);
      setAppointments(response.data || []);
    } catch (err: any) {
      console.error('Failed to load patient appointments:', err);
      setError('تعذر تحميل كشف مواعيدك. يرجى التحقق من اتصالك بالسيرفر.');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchAppointments();
  }, [patientId]);

  const handleCancel = (id: string) => {
    Swal.fire({
      title: 'إلغاء الموعد؟',
      text: 'هل أنت متأكد من رغبتك في إلغاء هذا الموعد؟',
      icon: 'warning',
      showCancelButton: true,
      confirmButtonColor: '#ef4444',
      cancelButtonColor: '#94a3b8',
      confirmButtonText: 'نعم، قم بالإلغاء',
      cancelButtonText: 'تراجع'
    }).then(async (result) => {
      if (result.isConfirmed) {
        try {
          // Send request to cancel
          await apiClient.delete(`/api/Appointments/${id}`);
          Swal.fire('تم الإلغاء!', 'تم إلغاء الموعد بنجاح.', 'success');
          fetchAppointments();
        } catch (err) {
          console.error('Failed to cancel appointment:', err);
          Swal.fire('خطأ!', 'فشلت عملية إلغاء الموعد.', 'error');
        }
      }
    });
  };

  const getFilteredAppointments = () => {
    const now = new Date();
    return appointments.filter(app => {
      const appDate = new Date(app.appointmentDate);
      if (activeTab === 'upcoming') {
        return appDate >= now && app.status !== 'Cancelled';
      }
      if (activeTab === 'past') {
        return appDate < now && app.status !== 'Cancelled';
      }
      if (activeTab === 'cancelled') {
        return app.status === 'Cancelled';
      }
      return true; // 'all'
    });
  };

  const filtered = getFilteredAppointments();

  return (
    <div className="space-y-8 text-right font-sans" dir="rtl">
      
      {/* Page Header */}
      <div className="flex flex-col sm:flex-row justify-between items-start sm:items-center gap-4">
        <div>
          <h1 className="text-3xl font-extrabold text-slate-900 font-cairo">جدول مواعيدي</h1>
          <p className="text-sm text-slate-500 font-cairo">تتبع وإدارة مواعيد كشوفاتك الطبية بكل سهولة</p>
        </div>
      </div>

      {/* Tabs Container */}
      <div className="flex border-b border-slate-200 overflow-x-auto pr-1">
        {(['all', 'upcoming', 'past', 'cancelled'] as const).map((tab) => {
          const labels = {
            all: 'الكل',
            upcoming: 'القادمة',
            past: 'السابقة',
            cancelled: 'ملغاة'
          };
          return (
            <button
              key={tab}
              onClick={() => setActiveTab(tab)}
              className={`py-3 px-6 text-sm font-bold font-cairo border-b-2 transition-all shrink-0 ${
                activeTab === tab
                  ? 'border-blue-600 text-blue-600'
                  : 'border-transparent text-slate-400 hover:text-slate-600'
              }`}
            >
              {labels[tab]}
            </button>
          );
        })}
      </div>

      {/* Main List View */}
      {loading ? (
        <div className="flex flex-col items-center justify-center min-h-[300px] gap-3">
          <Loader2 className="h-8 w-8 text-blue-600 animate-spin" />
          <p className="text-slate-500 text-xs font-semibold font-cairo">جاري تحميل كشف المواعيد...</p>
        </div>
      ) : error ? (
        <div className="bg-red-50 border border-red-200 text-red-700 p-6 rounded-2xl flex items-start gap-4 shadow-sm">
          <AlertCircle className="h-6 w-6 flex-shrink-0 mt-0.5" />
          <div>
            <h3 className="font-bold text-sm font-cairo">فشل الاتصال</h3>
            <p className="text-xs mt-1 font-cairo">{error}</p>
          </div>
        </div>
      ) : (
        <div className="space-y-4">
          {filtered.length === 0 ? (
            <div className="bg-white border border-slate-200 p-12 text-center rounded-2xl text-slate-400 font-cairo shadow-sm">
              لا توجد مواعيد حالية مسجلة في هذا التبويب.
            </div>
          ) : (
            filtered.map((app) => {
              const appDate = new Date(app.appointmentDate);
              
              let statusLabel = "معلق";
              let statusClass = "bg-amber-50 text-amber-700 border-amber-100";
              
              if (app.status === 'Confirmed') {
                statusLabel = "مؤكد";
                statusClass = "bg-green-50 text-green-700 border-green-100";
              } else if (app.status === 'Cancelled') {
                statusLabel = "ملغى";
                statusClass = "bg-red-50 text-red-700 border-red-100";
              }

              return (
                <div 
                  key={app.id} 
                  className="bg-white border border-slate-200 rounded-2xl p-5 md:p-6 shadow-sm hover:shadow-md transition-all flex flex-col md:flex-row justify-between items-start md:items-center gap-6"
                >
                  
                  {/* Doctor Info (Right Side) */}
                  <div className="flex gap-4 items-start w-full md:w-1/3">
                    <div className="h-12 w-12 rounded-full bg-blue-50 text-blue-600 flex items-center justify-center text-lg font-bold shrink-0 border border-blue-100">
                      👨‍⚕️
                    </div>
                    <div className="text-right space-y-1">
                      <h3 className="font-bold text-slate-900 text-md font-cairo flex items-center gap-2">
                        <span>{app.doctorName || 'طبيب CurexMind'}</span>
                        <span className="text-[10px] font-bold text-blue-600 bg-blue-50 border border-blue-100 px-2 py-0.5 rounded-full font-cairo">
                          {app.doctorSpecialization || 'تخصص عام'}
                        </span>
                      </h3>
                      <p className="text-xs text-slate-500 font-cairo flex items-center gap-1.5">
                        <MapPin className="h-3.5 w-3.5 text-slate-400" />
                        <span>{app.clinicAddress || 'مركز العيادات الرئيسي'}</span>
                      </p>
                      <span className="text-[10px] text-slate-400 font-mono block">رقم الحجز: #{app.id.substring(0, 8).toUpperCase()}</span>
                    </div>
                  </div>

                  {/* Time info (Middle) */}
                  <div className="flex flex-col gap-2 w-full md:w-1/4">
                    <div className="text-xs text-slate-500 font-semibold flex items-center gap-2">
                      <Calendar className="h-4 w-4 text-slate-400" />
                      <span>{appDate.toLocaleDateString('ar-EG', { weekday: 'long', year: 'numeric', month: 'long', day: 'numeric' })}</span>
                    </div>
                    <div className="text-xs text-slate-500 font-semibold flex items-center gap-2">
                      <Clock className="h-4 w-4 text-slate-400" />
                      <span>{appDate.toLocaleTimeString('ar-EG', { hour: '2-digit', minute: '2-digit' })}</span>
                    </div>
                  </div>

                  {/* Status & Actions (Left Side) */}
                  <div className="flex flex-col items-end gap-3 w-full md:w-1/3">
                    <div className="flex gap-2">
                      <span className={`px-2.5 py-1 rounded-full text-[10px] font-bold border font-cairo ${statusClass}`}>
                        {statusLabel}
                      </span>
                      <span className={`px-2.5 py-1 rounded-full text-[10px] font-bold border font-cairo ${
                        app.isPaid 
                          ? 'bg-blue-50 text-blue-700 border-blue-100' 
                          : 'bg-red-50 text-red-700 border-red-100'
                      }`}>
                        {app.isPaid ? 'مدفوع' : 'غير مدفوع'}
                      </span>
                    </div>

                    <div className="flex gap-2 flex-wrap justify-end">
                      <button className="bg-white border border-slate-200 hover:bg-slate-50 text-slate-700 font-bold text-xs px-4 py-2 rounded-xl transition-colors font-cairo">
                        عرض التفاصيل
                      </button>
                      {!app.isPaid && app.status !== 'Cancelled' && (
                        <Link 
                          to={`/patient/payment/${app.id}`}
                          className="bg-blue-600 hover:bg-blue-700 text-white font-bold text-xs px-4 py-2 rounded-xl transition-all shadow-sm font-cairo flex items-center justify-center"
                        >
                          ادفع الآن
                        </Link>
                      )}
                      {app.status === 'Confirmed' && (
                        <Link 
                          to={`/patient/rate/${app.doctorId || '0ad98dc6-ec33-4d5a-96e1-0ae8ef619ce6'}`}
                          className="bg-amber-500 hover:bg-amber-600 text-white font-bold text-xs px-4 py-2 rounded-xl transition-all shadow-sm font-cairo flex items-center justify-center"
                        >
                          تقييم الطبيب
                        </Link>
                      )}
                      {app.status !== 'Cancelled' && (
                        <button 
                          onClick={() => handleCancel(app.id)}
                          className="border border-red-200 hover:bg-red-50 text-red-600 font-bold text-xs px-4 py-2 rounded-xl transition-colors font-cairo flex items-center gap-1.5"
                        >
                          <Trash2 className="h-3.5 w-3.5" />
                          <span>إلغاء</span>
                        </button>
                      )}
                    </div>
                  </div>

                </div>
              );
            })
          )}
        </div>
      )}

    </div>
  );
};
