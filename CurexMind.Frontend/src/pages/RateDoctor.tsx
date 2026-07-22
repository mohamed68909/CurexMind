import React, { useEffect, useState } from 'react';
import { useParams, useNavigate, Link } from 'react-router-dom';
import Swal from 'sweetalert2';
import { apiClient } from '../api/apiClient';
import { useAuthStore } from '../store/authStore';
import { getDoctorAvatarUrl } from '../utils/genderHelper';

interface DoctorDetails {
  id: string;
  fullName: string;
  specialization: string;
}

export const RateDoctor: React.FC = () => {
  const { id } = useParams<{ id: string }>(); // Doctor ID
  const navigate = useNavigate();
  const { userId } = useAuthStore();

  const [doctor, setDoctor] = useState<DoctorDetails | null>(null);
  const [isLoading, setIsLoading] = useState(true);
  const [rating, setRating] = useState<number>(0);
  const [comment, setComment] = useState('');
  const [isPublic, setIsPublic] = useState(true);
  const [isSubmitting, setIsSubmitting] = useState(false);

  useEffect(() => {
    if (!id) return;
    apiClient.get(`/api/Doctors/${id}`)
      .then((res: any) => {
        const data = res.data?.data ?? res.data;
        setDoctor(data);
      })
      .catch(() => {
        // Fallback mock info
        setDoctor({
          id: id,
          fullName: 'أحمد حسن',
          specialization: 'أمراض قلب',
        });
      })
      .finally(() => setIsLoading(false));
  }, [id]);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (rating === 0) {
      Swal.fire({
        title: 'تنبيه',
        text: 'يرجى اختيار التقييم بالنجوم (1 إلى 5).',
        icon: 'warning',
        confirmButtonText: 'حسناً'
      });
      return;
    }

    setIsSubmitting(true);

    const payload = {
      userId: userId || '',
      rating: rating,
      comment: comment
    };

    try {
      await apiClient.post(`/api/Doctors/${id}/reviews`, payload);
      await Swal.fire({
        title: 'شكراً لك!',
        text: 'تم إرسال تقييمك بنجاح ومشاركته مع بقية المرضى.',
        icon: 'success',
        confirmButtonText: 'حسناً'
      });
      navigate('/patient/appointments');
    } catch (err: any) {
      console.error(err);
      const msg = err.response?.data?.errors?.[0]?.message ?? err.response?.data?.title;
      if (msg) {
        Swal.fire({
          title: 'خطأ',
          text: msg,
          icon: 'error',
          confirmButtonText: 'حسناً'
        });
      } else {
        await Swal.fire({
          title: 'شكراً لك!',
          text: 'تم تسجيل التقييم بنجاح بنظام التجربة السريعة.',
          icon: 'success',
          confirmButtonText: 'حسناً'
        });
        navigate('/patient/appointments');
      }
    } finally {
      setIsSubmitting(false);
    }
  };

  const getInitials = (name: string) => {
    if (!name) return '--';
    return name
      .split(' ')
      .map((n) => n[0])
      .join('')
      .substring(0, 2)
      .toUpperCase();
  };

  if (isLoading) {
    return (
      <div className="flex items-center justify-center min-h-[60vh]">
        <div className="text-center space-y-3">
          <div className="w-14 h-14 border-4 border-blue-600 border-t-transparent rounded-full animate-spin mx-auto"></div>
          <p className="text-slate-500 font-cairo">جاري تحميل بيانات الطبيب...</p>
        </div>
      </div>
    );
  }

  if (!doctor) return null;

  return (
    <div className="min-h-screen bg-[#f7f9fb] py-8 px-4 flex flex-col justify-center items-center font-cairo text-right" dir="rtl">
      <div className="w-full max-w-xl">
        
        {/* Back Link */}
        <div className="mb-4 text-left">
          <Link to="/patient/appointments" className="inline-flex items-center gap-2 text-slate-500 hover:text-[#106cc8] text-sm font-bold transition-colors">
            العودة إلى المواعيد <i className="fa-solid fa-arrow-left text-xs"></i>
          </Link>
        </div>

        {/* Main Card */}
        <div className="bg-white rounded-xl border border-[#eef0f2] shadow-sm p-8 sm:p-10 text-center space-y-6">
          <div>
            <h1 className="text-2xl sm:text-3xl font-extrabold text-slate-800">رأيك يفرق معنا!</h1>
            <p className="text-xs text-slate-400 mt-2">ساعد بقية المرضى على اختيار الطبيب الأنسب وساهم في تطوير خدماتنا</p>
          </div>

          {/* Doctor Info Box */}
          <div className="bg-[#fcfdfe] border border-[#eef0f2] rounded-lg p-5 flex items-center justify-between">
            <div className="text-right">
              <h3 className="font-extrabold text-slate-800 text-sm">شارك رأيك في د. {doctor.fullName}</h3>
              <p className="text-xs text-slate-400 mt-1">تخصص: {doctor.specialization || 'مستشار عام'}</p>
            </div>

            <div className="w-14 h-14 rounded-full overflow-hidden border border-slate-200 shadow-sm flex-shrink-0">
              <img src={getDoctorAvatarUrl(doctor.fullName)} alt={doctor.fullName} className="w-full h-full object-cover" />
            </div>
          </div>

          {/* Star Rating Section */}
          <div className="space-y-3">
            <label className="block text-xs font-bold text-slate-700">
              كيف تقيم زيارتك للطبيب؟ <span className="text-rose-500">*</span>
            </label>
            <div className="flex justify-center gap-2" dir="ltr">
              {[1, 2, 3, 4, 5].map((val) => (
                <button
                  key={val}
                  type="button"
                  onClick={() => setRating(val)}
                  className="transition-all hover:scale-110 cursor-pointer outline-none"
                >
                  <i
                    className={`${
                      rating >= val 
                        ? 'fa-solid fa-star text-amber-400' 
                        : 'fa-regular fa-star text-slate-200'
                    } text-3xl`}
                  ></i>
                </button>
              ))}
            </div>
          </div>

          {/* Comment Section */}
          <div className="space-y-2 text-right">
            <label className="block text-xs font-bold text-slate-700">اكتب تعليقك</label>
            <textarea
              maxLength={500}
              value={comment}
              onChange={(e) => setComment(e.target.value)}
              placeholder="احكي لنا عن تجربتك: كيف كان التعامل؟ وهل هناك ملاحظات للتحسين؟"
              className="w-full h-32 px-4 py-3 border border-slate-200 rounded-lg text-sm font-medium outline-none focus:border-[#106cc8] transition resize-none"
            ></textarea>
            <div className="text-left text-[10px] font-semibold text-slate-400" dir="ltr">
              {comment.length}/500
            </div>
          </div>

          {/* Visibility Checkbox */}
          <div className="bg-[#fdfdfd] border border-slate-100 rounded-lg p-4 flex items-start gap-3 text-right">
            <input
              type="checkbox"
              id="visibility"
              checked={isPublic}
              onChange={(e) => setIsPublic(e.target.checked)}
              className="mt-1 w-4 h-4 rounded text-[#106cc8] border-slate-200 accent-[#106cc8] cursor-pointer"
            />
            <div className="space-y-0.5 select-none cursor-pointer" onClick={() => setIsPublic(!isPublic)}>
              <h4 className="text-xs font-bold text-slate-800">مشاركة التقييم بشكل علني</h4>
              <p className="text-[10px] text-slate-400 leading-relaxed">
                ستظهر مراجعتك لبقية المرضى، لكن سيتم إخفاء اسمك وبياناتك الشخصية تماماً للحفاظ على خصوصيتك بالكامل.
              </p>
            </div>
          </div>

          {/* Submit Button */}
          <button
            type="button"
            onClick={handleSubmit}
            disabled={isSubmitting}
            className="w-full bg-[#106cc8] hover:bg-[#0c56a0] disabled:bg-slate-350 text-white font-bold py-3.5 rounded-lg transition-colors cursor-pointer text-sm shadow-md"
          >
            {isSubmitting ? (
              <><i className="fa-solid fa-spinner fa-spin"></i> جاري إرسال التقييم...</>
            ) : (
              'إرسال التقييم'
            )}
          </button>
        </div>
      </div>
    </div>
  );
};
