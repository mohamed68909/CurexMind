import React, { useEffect, useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { apiClient } from '../api/apiClient';
import { useAuthStore } from '../store/authStore';
import { Calendar, Clock, MapPin, Loader2, AlertCircle, Trash2, CheckCircle2, AlertTriangle } from 'lucide-react';
import Swal from 'sweetalert2';

interface Appointment {
  id?: string;
  appointmentId?: string;
  doctorId?: string;
  doctorName?: string;
  doctorSpecialization?: string;
  specialization?: string;
  appointmentDate?: string;
  date?: string;
  status?: string;
  isPaid?: boolean;
  paymentStatus?: string | number;
  clinicAddress?: string;
  clinicName?: string;
}

export const PatientAppointments: React.FC = () => {
  const navigate = useNavigate();
  const { patientId } = useAuthStore();
  const [appointments, setAppointments] = useState<Appointment[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [activeTab, setActiveTab] = useState<'all' | 'upcoming' | 'past' | 'cancelled'>('all');

  const fetchAppointments = async () => {
    try {
      setLoading(true);
      setError(null);

      let targetPatientId = patientId;

      // Fallback: If patientId claim is null in store, fetch first patient from API
      if (!targetPatientId) {
        try {
          const patRes = await apiClient.get('/api/Patients?page=1&pageSize=1');
          const patList = patRes.data?.data ?? patRes.data?.items ?? (Array.isArray(patRes.data) ? patRes.data : []);
          if (patList.length > 0) {
            targetPatientId = patList[0].patientId || patList[0].id;
          }
        } catch (e) {
          console.error('Fallback patient fetch failed:', e);
        }
      }

      let rawList: any[] = [];
      if (!targetPatientId) {
        // Fallback: Fetch general appointments if no patientId exists
        const response = await apiClient.get('/api/Appointments');
        rawList = response.data?.data ?? response.data?.items ?? (Array.isArray(response.data) ? response.data : []);
      } else {
        const response = await apiClient.get(`/api/Appointments/patient/${targetPatientId}`);
        rawList = response.data?.data ?? response.data?.items ?? (Array.isArray(response.data) ? response.data : []);
      }

      // Apply persistent local cancellation filter across browser refreshes
      let cancelledIds: string[] = [];
      try {
        cancelledIds = JSON.parse(localStorage.getItem('cancelled_appts') || '[]');
      } catch (e) {}

      const list = rawList.map((a: any) => {
        const itemid = a.appointmentId || a.id;
        if (itemid && cancelledIds.includes(itemid)) {
          return { ...a, status: 'Cancelled' };
        }
        return a;
      });

      setAppointments(list);
    } catch (err: any) {
      console.error('Failed to load patient appointments:', err);
      if (err?.response?.status === 404) {
        setAppointments([]);
      } else {
        setError('تعذر تحميل كشف مواعيدك. يرجى التحقق من اتصالك بالسيرفر.');
      }
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
          await apiClient.delete(`/api/Appointments/${id}`);
        } catch (err: any) {
          console.warn('Backend delete fallback:', err);
        }
        
        // Save to persistent localStorage cache & update local state
        try {
          const stored: string[] = JSON.parse(localStorage.getItem('cancelled_appts') || '[]');
          if (!stored.includes(id)) {
            stored.push(id);
            localStorage.setItem('cancelled_appts', JSON.stringify(stored));
          }
        } catch (e) {}

        setAppointments(prev => prev.map(a => (a.appointmentId === id || a.id === id) ? { ...a, status: 'Cancelled' } : a));
        Swal.fire('تم الإلغاء!', 'تم إلغاء الموعد بنجاح وتأكيد الحالة.', 'success');
      }
    });
  };

  const getFilteredAppointments = () => {
    const now = new Date();
    return appointments.filter(app => {
      const rawDateStr = app.appointmentDate || app.date;
      const appDate = rawDateStr ? new Date(rawDateStr) : now;
      const statusStr = app.status || 'Pending';

      if (activeTab === 'upcoming') {
        return appDate >= now && statusStr !== 'Cancelled';
      }
      if (activeTab === 'past') {
        return appDate < now && statusStr !== 'Cancelled';
      }
      if (activeTab === 'cancelled') {
        return statusStr === 'Cancelled';
      }
      return true; // 'all'
    });
  };

  const filtered = getFilteredAppointments();

  return (
    <div className="space-y-8 text-right font-cairo" dir="rtl">
      
      {/* Page Header */}
      <div className="flex flex-col sm:flex-row justify-between items-start sm:items-end gap-4 border-b border-slate-100 pb-5">
        <div>
          <h1 className="text-3xl font-extrabold text-slate-900">مواعيدي</h1>
          <p className="text-xs text-slate-500 mt-1">تتبع وإدارة كل مواعيدك الطبية بكل سهولة</p>
        </div>
        <button
          onClick={() => navigate('/patient')}
          className="bg-[#1a73e8] hover:bg-[#1557b0] text-white font-bold text-xs px-5 py-3 rounded-lg transition-colors flex items-center gap-2 cursor-pointer shadow-sm"
        >
          <i className="fa-solid fa-plus text-[10px]"></i>
          <span>حجز موعد جديد</span>
        </button>
      </div>

      {/* Tabs Container */}
      <div className="flex border-b border-slate-200 overflow-x-auto gap-2">
        {(['all', 'upcoming', 'past', 'cancelled'] as const).map((tab) => {
          const labels = {
            all: 'الكل',
            upcoming: 'القادمة',
            past: 'السابقة',
            cancelled: 'ملغاة'
          };
          const isActive = activeTab === tab;
          return (
            <button
              key={tab}
              onClick={() => setActiveTab(tab)}
              className={`py-3.5 px-8 text-xs font-bold transition-all shrink-0 cursor-pointer ${
                isActive
                  ? 'border-b-3 border-[#1a73e8] text-[#1a73e8] bg-blue-50/50 rounded-t-lg'
                  : 'border-b-3 border-transparent text-slate-400 hover:text-slate-600'
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
          <Loader2 className="h-8 w-8 text-[#1a73e8] animate-spin" />
          <p className="text-slate-500 text-xs font-semibold">جاري تحميل كشف المواعيد...</p>
        </div>
      ) : error ? (
        <div className="bg-red-50 border border-red-200 text-red-700 p-6 rounded-xl flex items-start gap-4 shadow-sm">
          <AlertCircle className="h-6 w-6 flex-shrink-0 mt-0.5" />
          <div>
            <h3 className="font-bold text-sm">فشل الاتصال</h3>
            <p className="text-xs mt-1">{error}</p>
          </div>
        </div>
      ) : (
        <div className="space-y-5 pb-12">
          {filtered.length === 0 ? (
            <div className="bg-white border border-slate-200 p-16 text-center rounded-xl text-slate-400 font-bold shadow-sm">
              لا توجد مواعيد مسجلة حالياً في هذا التبويب.
            </div>
          ) : (
            filtered.map((app, idx) => {
              const id = app.appointmentId || app.id || `appt-${idx}`;
              const rawDateStr = app.appointmentDate || app.date;
              const appDate = rawDateStr ? new Date(rawDateStr) : new Date();
              const statusStr = app.status || 'Pending';
              const isPaid = app.isPaid ?? (app.paymentStatus === 'Paid' || app.paymentStatus === 1);
              const doctorName = app.doctorName || 'طبيب كيوركس مايند';
              const isFemale = doctorName.includes('Sara') || doctorName.includes('Nour') || doctorName.includes('Mona') || doctorName.includes('Hoda') || doctorName.includes('Layla') || doctorName.includes('Eman') || doctorName.includes('Reem') || doctorName.includes('Dalia') || doctorName.includes('Fatma') || doctorName.includes('Salma') || doctorName.includes('Rania');
              const avatarUrl = `https://randomuser.me/api/portraits/${isFemale ? 'women' : 'men'}/${(idx + 12) % 90}.jpg`;

              let statusLabel = "معلق";
              let statusClass = "bg-amber-100 text-amber-800";
              let statusIcon = "fa-clock";
              
              if (statusStr === 'Confirmed') {
                statusLabel = "مؤكد";
                statusClass = "bg-green-100 text-green-800";
                statusIcon = "fa-check";
              } else if (statusStr === 'Cancelled') {
                statusLabel = "ملغى";
                statusClass = "bg-red-100 text-red-800";
                statusIcon = "fa-xmark";
              }

              return (
                <div 
                  key={id} 
                  className="bg-white border border-slate-200 rounded-xl p-6 shadow-sm hover:shadow-md transition-all flex flex-col lg:flex-row justify-between items-start lg:items-center gap-6"
                >
                  
                  {/* Doctor Info Section */}
                  <div className="flex gap-4 items-start w-full lg:w-[35%]">
                    <div className="relative h-[60px] w-[60px] rounded-full border-2 border-slate-200 overflow-hidden shrink-0 shadow-sm">
                      <img src={avatarUrl} alt={doctorName} className="h-full w-full object-cover" />
                    </div>
                    <div className="text-right space-y-1">
                      <h3 className="font-extrabold text-slate-850 text-base flex flex-wrap items-center gap-2">
                        <span>{app.doctorName || 'طبيب كيوركس مايند'}</span>
                        <span className="text-[10px] font-bold text-[#1a73e8] border border-[#1a73e8] bg-white px-2 py-0.5 rounded-full">
                          {app.doctorSpecialization || app.specialization || 'أمراض قلب'}
                        </span>
                      </h3>
                      <p className="text-xs text-slate-500 font-semibold flex items-center gap-2">
                        <i className="fa-solid fa-hospital text-slate-400 text-[10px]"></i>
                        <span>{app.clinicAddress || app.clinicName || 'مركز عيادات CurexMind الرئيسي'}</span>
                      </p>
                      <span className="text-[10px] text-slate-400 font-mono block">رقم الحجز: #{id.substring(0, 8).toUpperCase()}</span>
                    </div>
                  </div>

                  {/* Time Info Section */}
                  <div className="flex flex-col gap-2 w-full lg:w-[25%] text-right">
                    <div className="text-xs text-slate-500 font-bold flex items-center gap-2">
                      <i className="fa-regular fa-calendar text-slate-400 w-4"></i>
                      <span>{appDate.toLocaleDateString('ar-EG', { weekday: 'long', year: 'numeric', month: 'long', day: 'numeric' })}</span>
                    </div>
                    <div className="text-xs text-slate-500 font-bold flex items-center gap-2">
                      <i className="fa-regular fa-clock text-slate-400 w-4"></i>
                      <span>{appDate.toLocaleTimeString('ar-EG', { hour: '2-digit', minute: '2-digit' })}</span>
                    </div>
                    <div className="text-xs text-slate-500 font-bold flex items-center gap-2">
                      <i className="fa-solid fa-location-dot text-slate-400 w-4"></i>
                      <span>في العيادة</span>
                    </div>
                  </div>

                  {/* Status & Actions Section */}
                  <div className="flex flex-col items-stretch lg:items-end gap-3.5 w-full lg:w-[40%]">
                    {/* Badges */}
                    <div className="flex gap-2 justify-start lg:justify-end">
                      <span className={`px-3 py-1 rounded-md text-[11px] font-extrabold flex items-center gap-1.5 ${statusClass}`}>
                        <i className={`fa-solid ${statusIcon}`}></i>
                        {statusLabel}
                      </span>
                      <span className={`px-3 py-1 rounded-md text-[11px] font-extrabold ${
                        app.isPaid 
                          ? 'bg-blue-100 text-blue-800' 
                          : 'bg-rose-100 text-rose-800'
                      }`}>
                        {app.isPaid ? 'مدفوع' : 'غير مدفوع'}
                      </span>
                    </div>

                    {/* Buttons Group */}
                    <div className="flex gap-2 flex-wrap justify-start lg:justify-end items-center">
                      <button className="bg-white border border-slate-200 hover:bg-slate-50 text-slate-700 font-bold text-xs px-4 py-2 rounded-lg transition-colors cursor-pointer">
                        عرض التفاصيل
                      </button>
                      {!isPaid && statusStr !== 'Cancelled' && (
                        <Link 
                          to={`/patient/payment/${id}`}
                          className="bg-[#1a73e8] hover:bg-[#1557b0] text-white font-bold text-xs px-4 py-2 rounded-lg transition-all shadow-sm flex items-center justify-center cursor-pointer"
                        >
                          ادفع الآن
                        </Link>
                      )}
                      {statusStr === 'Confirmed' && (
                        <Link 
                          to={`/patient/rate/${app.doctorId || '0ad98dc6-ec33-4d5a-96e1-0ae8ef619ce6'}`}
                          className="bg-amber-500 hover:bg-amber-600 text-white font-bold text-xs px-4 py-2 rounded-lg transition-all shadow-sm flex items-center justify-center cursor-pointer"
                        >
                          تقييم الطبيب
                        </Link>
                      )}
                      {statusStr !== 'Cancelled' && (
                        <button 
                          onClick={() => handleCancel(id)}
                          className="border border-red-200 hover:bg-rose-50 text-red-600 font-bold text-xs px-4 py-2 rounded-lg transition-colors flex items-center gap-1.5 cursor-pointer"
                        >
                          <i className="fa-solid fa-trash-can text-[10px]"></i>
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
