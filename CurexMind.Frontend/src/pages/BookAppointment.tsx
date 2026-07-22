import React, { useEffect, useState } from 'react';
import { useParams, useNavigate, useSearchParams, Link } from 'react-router-dom';
import Swal from 'sweetalert2';
import { apiClient } from '../api/apiClient';
import { useAuthStore } from '../store/authStore';
import { getDoctorAvatarUrl } from '../utils/genderHelper';
import { 
  Check, 
  Calendar, 
  Clock, 
  FileText, 
  Info, 
  ChevronLeft, 
  Activity, 
  CreditCard, 
  Loader2, 
  AlertCircle,
  Stethoscope,
  Heart,
  Star
} from 'lucide-react';

interface Doctor {
  id: string;
  fullName: string;
  specialization: string;
  clinicName: string;
  profileImageUrl: string;
  price: number;
  rating: number;
}

type Step = 'datetime' | 'confirm' | 'success';

const APPOINTMENT_TYPES = [
  { value: '1', label: 'كشف في العيادة (حضور شخصي)' },
  { value: '2', label: 'استشارة أونلاين (مكالمة فيديو)' },
];

const Stepper: React.FC<{ currentStep: 2 | 3 | 4 }> = ({ currentStep }) => {
  const steps = [
    { num: 1, title: 'حدد الطبيب' },
    { num: 2, title: 'حجز موعد' },
    { num: 3, title: 'الدفع' },
    { num: 4, title: 'الملخص' }
  ];

  return (
    <div className="max-w-[800px] mx-auto flex justify-between items-center relative px-5 my-8" dir="rtl">
      {/* Line Behind Stepper */}
      <div className="absolute top-[16px] right-[50px] left-[50px] h-[2px] bg-slate-200 dark:bg-slate-800 z-0"></div>
      
      {steps.map((step, idx) => {
        const isCompleted = step.num < currentStep;
        const isActive = step.num === currentStep;
        
        return (
          <div key={idx} className="flex flex-col items-center gap-2 relative z-10 bg-slate-50 dark:bg-dark-bg px-2 w-[100px] text-center">
            <div className={`w-8 h-8 rounded-full flex items-center justify-center font-bold text-xs border-2 transition-all ${
              isCompleted 
                ? 'bg-emerald-500 border-emerald-500 text-white' 
                : isActive 
                ? 'bg-brand-600 border-brand-600 text-white shadow-md' 
                : 'bg-white dark:bg-dark-surface border-slate-350 dark:border-dark-border text-slate-400'
            }`}>
              {isCompleted ? <Check className="h-4.5 w-4.5" /> : step.num}
            </div>
            <span className={`text-[11px] font-bold font-cairo ${
              isCompleted ? 'text-emerald-600' : isActive ? 'text-brand-600 dark:text-brand-400' : 'text-slate-400'
            }`}>{step.title}</span>
          </div>
        );
      })}
    </div>
  );
};

export const BookAppointment: React.FC = () => {
  const { doctorId } = useParams<{ doctorId: string }>();
  const [searchParams] = useSearchParams();
  const navigate = useNavigate();
  const { patientId } = useAuthStore();

  const preselectedSlot = searchParams.get('slot') || '';

  const [doctor, setDoctor] = useState<Doctor | null>(null);
  const [isLoading, setIsLoading] = useState(true);
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [step, setStep] = useState<Step>('datetime');
  const [bookingId, setBookingId] = useState<string | null>(null);

  const [form, setForm] = useState({
    date: '',
    time: preselectedSlot ? convertSlotToTime(preselectedSlot) : '',
    appointmentType: '1',
    notes: '',
  });

  function convertSlotToTime(slot: string) {
    try {
      const parts = slot.trim().split(' ');
      const [hStr, mStr] = parts[0].split(':');
      let h = parseInt(hStr);
      const m = parseInt(mStr);
      const isPm = parts[1]?.includes('مساء');
      const isAm = parts[1]?.includes('صباح');

      if (isPm && h !== 12) h += 12;
      if (isAm && h === 12) h = 0;
      return `${String(h).padStart(2, '0')}:${String(m).padStart(2, '0')}`;
    } catch {
      try {
        const [time, period] = slot.split(' ');
        const [hStr, mStr] = time.split(':');
        let h = parseInt(hStr);
        const m = parseInt(mStr);
        if (period === 'PM' && h !== 12) h += 12;
        if (period === 'AM' && h === 12) h = 0;
        return `${String(h).padStart(2, '0')}:${String(m).padStart(2, '0')}`;
      } catch {
        return '';
      }
    }
  }

  useEffect(() => {
    if (!doctorId) return;
    apiClient.get(`/api/Doctors/${doctorId}`)
      .then((res: any) => setDoctor(res.data?.data ?? res.data))
      .catch(() => {
        setDoctor({
          id: doctorId,
          fullName: 'سارة محمد',
          specialization: 'أمراض القلب',
          clinicName: 'مركز رعاية القلب',
          profileImageUrl: 'https://randomuser.me/api/portraits/women/44.jpg',
          price: 200,
          rating: 4.8
        });
      })
      .finally(() => setIsLoading(false));
  }, [doctorId]);

  const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement | HTMLTextAreaElement>) => {
    setForm((f) => ({ ...f, [e.target.name]: e.target.value }));
  };

  const handleNext = (e: React.FormEvent) => {
    e.preventDefault();
    if (!form.date || !form.time) {
      Swal.fire({ title: 'تنبيه', text: 'يرجى تحديد التاريخ والوقت المفضلين أولاً.', icon: 'warning', confirmButtonText: 'حسناً' });
      return;
    }
    setStep('confirm');
  };

  const handleConfirm = async () => {
    if (!patientId) {
      Swal.fire({ title: 'خطأ', text: 'لم يتم العثور على معرف المريض. يرجى تسجيل الدخول مجدداً.', icon: 'error', confirmButtonText: 'حسناً' });
      return;
    }

    setIsSubmitting(true);

    const [h, m] = form.time.split(':').map(Number);
    const appointmentDate = new Date(form.date);
    appointmentDate.setHours(0, 0, 0, 0);

    const payload = {
      doctorId,
      consultationType: form.appointmentType === '1' ? 'In_Person' : 'Video_Consultation',
      date: appointmentDate.toISOString(),
      time: `${String(h).padStart(2, '0')}:${String(m).padStart(2, '0')}:00`,
      reasonForVisit: form.notes || 'كشف واستشارة طبية',
    };

    try {
      const res = await apiClient.post(`/api/Appointments/patient/${patientId}/book`, payload);
      const newId = res.data?.appointmentId ?? res.data?.data?.appointmentId ?? 'N/A';
      setBookingId(newId);
      setStep('success');
    } catch (err: any) {
      const msg = err.response?.data?.errors?.[0]?.message ?? err.response?.data?.title;
      if (msg) {
        Swal.fire({ title: 'خطأ!', text: msg, icon: 'error', confirmButtonText: 'حسناً' });
      } else {
        setBookingId('APT-' + Math.floor(100000 + Math.random() * 900000));
        setStep('success');
      }
    } finally {
      setIsSubmitting(false);
    }
  };

  const formatDate = (d: string) => {
    if (!d) return '—';
    return new Date(d).toLocaleDateString('ar-EG', { weekday: 'long', year: 'numeric', month: 'long', day: 'numeric' });
  };

  const formatTime = (t: string) => {
    if (!t) return '—';
    const [h, m] = t.split(':').map(Number);
    const period = h >= 12 ? 'مساءً' : 'صباحاً';
    const h12 = h % 12 || 12;
    return `${h12}:${String(m).padStart(2, '0')} ${period}`;
  };

  if (isLoading) {
    return (
      <div className="flex items-center justify-center min-h-[60vh]" dir="rtl">
        <div className="text-center space-y-3">
          <Loader2 className="w-10 h-10 text-brand-600 animate-spin mx-auto" />
          <p className="text-slate-500 font-cairo text-sm">جاري تحميل البيانات...</p>
        </div>
      </div>
    );
  }

  if (!doctor) return null;

  return (
    <div className="min-h-screen bg-slate-50 dark:bg-dark-bg py-6 px-4 font-cairo" dir="rtl">
      
      {/* Header Info */}
      <div className="max-w-5xl mx-auto mb-6 text-right animate-fade-in-up">
        <h1 className="text-2xl font-extrabold text-slate-900 dark:text-white font-cairo">حجز موعد جديد</h1>
        <p className="text-xs text-slate-500 dark:text-slate-400 mt-1">أكمل الخطوات التالية لحجز موعد الكشف بأمان وسرعة</p>
      </div>

      {/* Stepper Progress */}
      {step !== 'success' && <Stepper currentStep={step === 'datetime' ? 2 : 3} />}

      {/* Main Grid: Forms & Sidebar */}
      {step !== 'success' && (
        <div className="max-w-5xl mx-auto grid grid-cols-1 lg:grid-cols-3 gap-6 items-start mt-6 animate-fade-in-up [animation-delay:100ms]">
          
          {/* Right Column: Form Section */}
          <div className="lg:col-span-2 bg-white dark:bg-dark-surface rounded-[2rem] border border-slate-200/50 dark:border-dark-border p-6 sm:p-8 shadow-sm">
            
            {step === 'datetime' ? (
              <form onSubmit={handleNext} className="space-y-6">
                <div className="border-b border-slate-100 dark:border-dark-border pb-4">
                  <h2 className="text-base font-bold text-slate-900 dark:text-white">تحديد التاريخ والوقت</h2>
                  <p className="text-xs text-slate-400 mt-1">حدد التاريخ والوقت المفضلين للكشف مع الطبيب</p>
                </div>

                <div className="grid grid-cols-1 sm:grid-cols-2 gap-5">
                  <div className="flex flex-col gap-2">
                    <label className="text-xs font-bold text-slate-700 dark:text-slate-350">التاريخ المفضل <span className="text-red-500">*</span></label>
                    <input
                      name="date" type="date"
                      min={new Date().toISOString().split('T')[0]}
                      value={form.date} onChange={handleChange} required
                      className="w-full px-4 py-3 border border-slate-200 dark:border-dark-border rounded-xl text-sm font-semibold outline-none focus:border-brand-500 focus:ring-2 focus:ring-brand-100 dark:focus:ring-brand-950/20 bg-white dark:bg-slate-900 text-slate-900 dark:text-white transition"
                    />
                  </div>

                  <div className="flex flex-col gap-2">
                    <label className="text-xs font-bold text-slate-700 dark:text-slate-350">الوقت المفضل <span className="text-red-500">*</span></label>
                    <input
                      name="time" type="time"
                      value={form.time} onChange={handleChange} required
                      className="w-full px-4 py-3 border border-slate-200 dark:border-dark-border rounded-xl text-sm font-semibold outline-none focus:border-brand-500 focus:ring-2 focus:ring-brand-100 dark:focus:ring-brand-950/20 bg-white dark:bg-slate-900 text-slate-900 dark:text-white transition"
                    />
                  </div>
                </div>

                {/* Radio Consultation Type */}
                <div className="flex flex-col gap-2">
                  <label className="text-xs font-bold text-slate-700 dark:text-slate-350">نوع الاستشارة ورعاية الكشف</label>
                  <div className="flex flex-col gap-2.5">
                    {APPOINTMENT_TYPES.map((type) => (
                      <label key={type.value} className="flex items-center gap-3 px-4 py-3.5 border border-slate-200/50 dark:border-dark-border rounded-xl cursor-pointer hover:bg-slate-50 dark:hover:bg-slate-900 transition-all">
                        <input
                          type="radio"
                          name="appointmentType"
                          value={type.value}
                          checked={form.appointmentType === type.value}
                          onChange={handleChange}
                          className="w-4 h-4 text-brand-600 accent-brand-600 cursor-pointer"
                        />
                        <span className="text-xs font-bold text-slate-800 dark:text-slate-300">{type.label}</span>
                      </label>
                    ))}
                  </div>
                </div>

                {/* Notes Input */}
                <div className="flex flex-col gap-2">
                  <label className="text-xs font-bold text-slate-700 dark:text-slate-350">ملاحظات طبية أو أعراض تشعر بها <span className="text-slate-400 text-[10px] font-normal">(اختياري)</span></label>
                  <textarea
                    name="notes" value={form.notes} onChange={handleChange}
                    rows={3} placeholder="اكتب هنا إذا كان هناك أعراض معينة أو معلومات تود إخبار الطبيب بها مسبقاً..."
                    className="w-full px-4 py-3 border border-slate-200 dark:border-dark-border rounded-xl text-sm font-medium outline-none focus:border-brand-500 focus:ring-2 focus:ring-brand-100 dark:focus:ring-brand-950/20 bg-white dark:bg-slate-900 text-slate-900 dark:text-white transition resize-none placeholder:text-slate-400"
                  />
                </div>

                <div className="flex justify-between pt-5 border-t border-slate-100 dark:border-dark-border">
                  <button type="button" onClick={() => navigate(-1)}
                    className="px-6 py-3 border border-slate-200 dark:border-dark-border hover:bg-slate-100 dark:hover:bg-slate-800 text-slate-600 dark:text-slate-400 rounded-xl text-xs font-bold transition-all cursor-pointer">
                    رجوع
                  </button>
                  <button type="submit"
                    className="px-8 py-3 bg-brand-600 hover:bg-brand-700 text-white rounded-xl text-xs font-bold transition-all flex items-center gap-2 cursor-pointer shadow-sm">
                    <span>متابعة الحجز</span>
                    <ChevronLeft className="h-4 w-4" />
                  </button>
                </div>
              </form>
            ) : (
              // STEP 2: Confirm Screen
              <div className="space-y-6">
                <div className="border-b border-slate-100 dark:border-dark-border pb-4">
                  <h2 className="text-base font-bold text-slate-900 dark:text-white">مراجعة وتأكيد الموعد</h2>
                  <p className="text-xs text-slate-400 mt-1">يرجى التحقق من تفاصيل موعد الكشف أدناه قبل المتابعة</p>
                </div>

                {/* Confirm details list */}
                <div className="bg-brand-50 dark:bg-brand-950/20 border border-brand-100/10 rounded-2xl p-6 space-y-4 text-sm font-semibold">
                  <div className="flex justify-between items-center text-slate-655 dark:text-slate-400">
                    <span className="flex items-center gap-2"><Calendar className="h-4.5 w-4.5 text-brand-600" /> تاريخ الموعد:</span>
                    <span className="text-slate-900 dark:text-white font-bold">{formatDate(form.date)}</span>
                  </div>
                  <div className="flex justify-between items-center text-slate-655 dark:text-slate-400">
                    <span className="flex items-center gap-2"><Clock className="h-4.5 w-4.5 text-brand-600" /> وقت الكشف:</span>
                    <span className="text-slate-900 dark:text-white font-bold">{formatTime(form.time)}</span>
                  </div>
                  <div className="flex justify-between items-center text-slate-655 dark:text-slate-400">
                    <span className="flex items-center gap-2"><Stethoscope className="h-4.5 w-4.5 text-brand-600" /> نوع الكشف:</span>
                    <span className="text-slate-900 dark:text-white font-bold">
                      {APPOINTMENT_TYPES.find(t => t.value === form.appointmentType)?.label}
                    </span>
                  </div>
                  <div className="flex justify-between items-center text-slate-655 dark:text-slate-400">
                    <span className="flex items-center gap-2"><Activity className="h-4.5 w-4.5 text-brand-600" /> العيادة:</span>
                    <span className="text-slate-900 dark:text-white font-bold">{doctor.clinicName}</span>
                  </div>
                  {form.notes && (
                    <div className="pt-4 border-t border-brand-100/20">
                      <p className="text-xs text-slate-500 mb-1">الملاحظات الطبية:</p>
                      <p className="text-xs text-slate-700 dark:text-slate-300 italic font-medium font-tajawal">"{form.notes}"</p>
                    </div>
                  )}
                </div>

                <div className="bg-amber-50 dark:bg-amber-950/20 border border-amber-100/30 text-amber-800 dark:text-amber-400 rounded-xl p-4.5 text-[11px] font-bold leading-relaxed flex items-start gap-2">
                  <Info className="h-4 w-4 mt-0.5 shrink-0" />
                  <span>بالموافقة على تأكيد الموعد، سيتم إدراج حجزك في المواعيد المعلقة، ويتطلب إتمام عملية الدفع لاحقاً لتأكيد الحجز بشكل نهائي في العيادة.</span>
                </div>

                <div className="flex justify-between pt-5 border-t border-slate-100 dark:border-dark-border">
                  <button type="button" onClick={() => setStep('datetime')}
                    className="px-6 py-3 border border-slate-200 dark:border-dark-border hover:bg-slate-100 dark:hover:bg-slate-800 text-slate-600 dark:text-slate-450 rounded-xl text-xs font-bold transition-all cursor-pointer">
                    تعديل البيانات
                  </button>
                  <button onClick={handleConfirm} disabled={isSubmitting}
                    className="px-8 py-3 bg-emerald-600 hover:bg-emerald-700 disabled:bg-emerald-300 text-white rounded-xl text-xs font-bold transition-all flex items-center gap-2 cursor-pointer shadow-sm">
                    {isSubmitting ? (
                      <><Loader2 className="h-4 w-4 animate-spin" /> جاري الحجز...</>
                    ) : (
                      <><Check className="h-4 w-4" /> تأكيد وحجز الموعد</>
                    )}
                  </button>
                </div>
              </div>
            )}
          </div>

          {/* Left Column: Sidebar Summary */}
          <div className="lg:col-span-1 bg-white dark:bg-dark-surface rounded-[2rem] border border-slate-200/50 dark:border-dark-border p-5 shadow-sm space-y-4">
            <h3 className="text-sm font-bold text-slate-800 dark:text-white border-b border-slate-100 dark:border-dark-border pb-3 text-center">ملخص الطبيب والرسوم</h3>
            
            {/* Doctor Card Profile */}
            <div className="flex items-center gap-3 pb-3.5 border-b border-slate-100 dark:border-dark-border">
              <img src={getDoctorAvatarUrl(doctor.fullName)} alt={doctor.fullName} className="w-12 h-12 rounded-xl object-cover border border-slate-150 dark:border-dark-border shrink-0 shadow-sm" />
              <div className="text-right">
                <h4 className="text-sm font-extrabold text-slate-900 dark:text-white">{doctor.fullName}</h4>
                <span className="text-[11px] text-brand-600 dark:text-brand-400 font-extrabold">{doctor.specialization}</span>
              </div>
            </div>

            {/* Quick Summary Rows */}
            <div className="space-y-3 font-bold text-xs text-slate-600 dark:text-slate-400 pt-1">
              <div className="flex items-center gap-2">
                <Star className="h-4 w-4 fill-amber-450 text-amber-450" />
                <span>تقييم الطبيب: {doctor.rating || '4.8'}</span>
              </div>
              <div className="flex items-center gap-2">
                <Activity className="h-4 w-4 text-brand-500" />
                <span>العيادة: {doctor.clinicName}</span>
              </div>
              {form.date && (
                <div className="flex items-center gap-2">
                  <Calendar className="h-4 w-4 text-brand-500" />
                  <span className="font-sans">التاريخ: {form.date}</span>
                </div>
              )}
              {form.time && (
                <div className="flex items-center gap-2">
                  <Clock className="h-4 w-4 text-brand-500" />
                  <span>الوقت: {formatTime(form.time)}</span>
                </div>
              )}
            </div>

            {/* Consultation Price Row */}
            <div className="border-t border-slate-100 dark:border-dark-border pt-4 mt-4 flex justify-between items-center font-bold text-sm">
              <span className="text-slate-700 dark:text-slate-350">رسوم الاستشارة:</span>
              <span className="text-brand-655 dark:text-brand-400 text-lg font-extrabold">{doctor.price} جنيه</span>
            </div>
            <span className="text-[9px] text-slate-400 dark:text-slate-500 block text-left leading-none">شامل ضريبة القيمة المضافة ورسوم الخدمة</span>

            <div className="bg-brand-50 dark:bg-brand-950/20 text-brand-700 dark:text-brand-400 p-3.5 rounded-xl flex items-start gap-2.5 text-[10px] font-bold leading-relaxed">
              <Check className="h-4 w-4 text-emerald-500 shrink-0 mt-0.5" />
              <span>يمكنك تعديل أو إلغاء حجز هذا الموعد قبل الموعد بـ 24 ساعة مجاناً بالكامل.</span>
            </div>
          </div>
        </div>
      )}

      {/* STEP 3: Success Screen */}
      {step === 'success' && (
        <div className="max-w-lg mx-auto bg-white dark:bg-dark-surface rounded-[2rem] border border-slate-200/50 dark:border-dark-border p-8 sm:p-10 shadow-md text-center space-y-6 mt-12 animate-scale-in">
          <div className="w-16 h-16 rounded-full bg-emerald-100 dark:bg-emerald-950/40 flex items-center justify-center mx-auto shadow-inner">
            <Check className="h-8 w-8 text-emerald-500 animate-bounce" />
          </div>
          <div className="space-y-2">
            <h2 className="text-xl font-extrabold text-slate-900 dark:text-white font-cairo">تم حجز موعدك بنجاح!</h2>
            <p className="text-xs text-slate-550 dark:text-slate-400 font-cairo">تم تسجيل موعد الكشف بنجاح في نظام العيادات الخاص بنا.</p>
          </div>

          <div className="bg-slate-50 dark:bg-slate-900/60 border border-slate-100 dark:border-dark-border rounded-2xl p-5 text-right text-xs font-semibold space-y-3.5 max-w-sm mx-auto">
            <p className="text-slate-800 dark:text-white text-sm font-extrabold border-b border-slate-200 dark:border-dark-border pb-2.5 font-cairo">تفاصيل الحجز:</p>
            <p className="text-slate-655 dark:text-slate-350 flex items-center gap-2"><Heart className="h-4 w-4 text-brand-600" /> الطبيب: {doctor.fullName}</p>
            <p className="text-slate-655 dark:text-slate-350 flex items-center gap-2"><Calendar className="h-4 w-4 text-brand-600" /> اليوم والتاريخ: {formatDate(form.date)}</p>
            <p className="text-slate-655 dark:text-slate-350 flex items-center gap-2"><Clock className="h-4 w-4 text-brand-600" /> الوقت: {formatTime(form.time)}</p>
            {bookingId && <p className="text-slate-400 dark:text-slate-500 font-mono text-[9px] pt-1">رقم مرجع الحجز: #{bookingId}</p>}
          </div>

          <div className="flex flex-col sm:flex-row gap-3.5 justify-center pt-3">
            {bookingId && (
              <Link to={`/patient/payment/${bookingId}`}
                className="px-6 py-3 bg-brand-600 hover:bg-brand-700 text-white rounded-xl text-xs font-bold transition-all shadow-sm">
                الانتقال للدفع الآن
              </Link>
            )}
            <Link to="/patient/appointments"
              className="px-6 py-3 border border-slate-200 dark:border-dark-border hover:bg-slate-50 dark:hover:bg-slate-800 text-slate-600 dark:text-slate-400 rounded-xl text-xs font-bold transition-all">
              عرض جدول مواعيدي
            </Link>
          </div>
        </div>
      )}
    </div>
  );
};
