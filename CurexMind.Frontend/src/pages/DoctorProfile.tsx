import React, { useEffect, useState } from 'react';
import { useParams, useNavigate, Link } from 'react-router-dom';
import { apiClient } from '../api/apiClient';
import { getDoctorAvatarUrl } from '../utils/genderHelper';

interface DoctorProfile {
  id: string;
  fullName: string;
  specialization: string;
  clinicName: string;
  profileImageUrl: string;
  bio: string;
  languages: string;
  yearsOfExperience: number;
  price: number;
  rating: number;
  reviewsCount: number;
  reviews?: Review[];
}

interface Review {
  reviewerName: string;
  comment: string;
  rating: number;
  date: string;
}

const MOCK_BIO = 'طبيب ذو خبرة عالية ملتزم بتقديم رعاية صحية شاملة للمرضى باستخدام أحدث التقنيات الطبية والعلاجات القائمة على الأدلة العلمية في مركز CurexMind الطبي.';

const MOCK_REVIEWS: Review[] = [
  { reviewerName: 'سارة أحمد', comment: 'طبيبة ممتازة جداً وشرحت لي كل شيء بالتفصيل. العيادة نظيفة للغاية والمواعيد دقيقة.', rating: 5, date: '15 يناير 2026' },
  { reviewerName: 'محمد علي', comment: 'تجربة رائعة للغاية. الطبيب استمع لشكواي باهتمام والتشخيص كان دقيقاً جداً.', rating: 4, date: '10 يناير 2026' },
  { reviewerName: 'نور حسن', comment: 'مهني للغاية ومعاملة راقية جداً. أنصح به بشدة لكل من يحتاج لرعاية متخصصة.', rating: 5, date: '3 فبراير 2026' },
];

const StarRating: React.FC<{ rating: number; size?: string }> = ({ rating, size = 'text-sm' }) => (
  <div className={`flex gap-0.5 ${size}`}>
    {[1, 2, 3, 4, 5].map((i) => (
      <i
        key={i}
        className={i <= Math.round(rating) ? 'fa-solid fa-star text-amber-400' : 'fa-regular fa-star text-slate-200'}
      />
    ))}
  </div>
);

const getInitials = (name: string) => {
  if (!name) return '--';
  return name.split(' ').map((n) => n[0]).join('').slice(0, 2).toUpperCase();
};

export const DoctorProfile: React.FC = () => {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();

  const [doctor, setDoctor] = useState<DoctorProfile | null>(null);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState(false);
  const [selectedSlot, setSelectedSlot] = useState<string | null>('04:30 مساءً');

  const AVAILABLE_SLOTS = ['09:00 صباحاً', '11:00 صباحاً', '02:00 مساءً', '03:00 مساءً', '04:30 مساءً', '05:00 مساءً'];

  useEffect(() => {
    if (!id) return;
    apiClient.get(`/api/Doctors/${id}`).then((res: any) => {
      setDoctor(res.data?.data ?? res.data);
    }).catch(() => setError(true)).finally(() => setIsLoading(false));
  }, [id]);

  if (isLoading) {
    return (
      <div className="flex items-center justify-center min-h-[60vh]" dir="rtl">
        <div className="text-center space-y-3">
          <div className="w-14 h-14 border-4 border-[#0e6cc9] border-t-transparent rounded-full animate-spin mx-auto"></div>
          <p className="text-slate-500 font-tajawal">جاري تحميل ملف الطبيب...</p>
        </div>
      </div>
    );
  }

  if (error || !doctor) {
    return (
      <div className="flex flex-col items-center justify-center min-h-[60vh] text-center gap-4" dir="rtl">
        <i className="fa-regular fa-face-sad-tear text-5xl text-slate-300"></i>
        <p className="text-slate-600 font-semibold font-tajawal">ملف الطبيب غير موجود حالياً.</p>
        <button onClick={() => navigate('/patient')} className="px-6 py-2.5 bg-[#0e6cc9] hover:bg-[#0b59a8] text-white rounded-lg text-sm font-bold font-tajawal transition-colors cursor-pointer">
          تصفح الأطباء
        </button>
      </div>
    );
  }

  const rating = doctor.rating || 4.8;
  const reviewsCount = doctor.reviewsCount || 53;
  const reviews = (doctor.reviews && doctor.reviews.length > 0) ? doctor.reviews : MOCK_REVIEWS;
  const bio = (doctor.bio && doctor.bio.trim() !== '') ? doctor.bio : MOCK_BIO;
  const price = doctor.price || 200;

  // Render
  return (
    <div className="min-h-screen bg-[#f7f9fb] font-tajawal" dir="rtl">
      
      {/* Hero Header Wrapper */}
      <div className="bg-[#0e6cc9] text-white py-10 pb-16">
        <div className="max-w-6xl mx-auto px-4">
          <Link to="/patient" className="inline-flex items-center gap-2 text-blue-100 hover:text-white text-sm mb-6 transition-colors">
            <i className="fa-solid fa-arrow-right text-xs"></i> العودة للأطباء
          </Link>
          
          <div className="flex flex-col md:flex-row items-center justify-between gap-8">
            <div className="flex flex-col gap-3 text-right">
              <h1 className="text-3xl md:text-4xl font-extrabold">{doctor.fullName}</h1>
              <p className="text-blue-100 text-lg font-medium">{doctor.specialization || 'مستشار عام'}</p>
              
              <div className="flex flex-wrap items-center gap-4 text-sm text-blue-50 mt-1">
                <span><i className="fa-solid fa-location-dot ml-1.5"></i>{doctor.clinicName || 'عيادة CurexMind'}</span>
              </div>
              
              <div className="flex items-center gap-2 bg-black/10 px-3 py-1.5 rounded-full w-fit mt-2">
                <span className="font-bold text-white text-base">{Number(rating).toFixed(1)}</span>
                <i className="fa-solid fa-star text-amber-400"></i>
                <span className="text-xs text-blue-100 font-semibold">({reviewsCount} تقييم)</span>
              </div>
              
              <button
                onClick={() => navigate(`/patient/book/${id}`)}
                className="mt-4 inline-flex items-center justify-center gap-2 bg-white text-[#0e6cc9] font-bold px-6 py-2.5 rounded-lg hover:bg-slate-50 transition-colors shadow-md w-fit cursor-pointer text-sm"
              >
                <i className="fa-regular fa-calendar-check"></i> احجز موعداً
              </button>
            </div>

            <div className="w-[150px] h-[150px] rounded-full border-4 border-white/20 overflow-hidden bg-slate-100 shadow-lg shrink-0">
              <img src={getDoctorAvatarUrl(doctor.fullName)} alt={doctor.fullName} className="w-full h-full object-cover" />
            </div>
          </div>
        </div>
      </div>

      {/* Main Layout Container */}
      <div className="max-w-6xl mx-auto px-4 -mt-10 grid grid-cols-1 lg:grid-cols-3 gap-6 pb-12 relative z-10">
        
        {/* Right column: Bio & Reviews (Desktop matches Main col on right side for RTL) */}
        <div className="lg:col-span-2 space-y-6">
          
          {/* Bio card */}
          <div className="bg-white rounded-xl border border-[#eef1f5] shadow-sm p-6 sm:p-8">
            <h2 className="text-lg font-bold text-slate-900 mb-4 flex items-center gap-2 border-b border-slate-50 pb-2">
              <i className="fa-regular fa-address-card text-[#0e6cc9]"></i> نبذة عن الطبيب
            </h2>
            <p className="text-slate-600 leading-relaxed text-sm">{bio}</p>
            
            <div className="flex flex-wrap gap-3 mt-6">
              <span className="flex items-center gap-2 bg-[#f8f9fa] border border-slate-100 px-4 py-2 rounded-full text-xs font-semibold text-slate-700">
                <i className="fa-solid fa-globe text-[#2386ea]"></i>
                اللغات: {doctor.languages || 'العربية، الإنجليزية'}
              </span>
              <span className="flex items-center gap-2 bg-[#f8f9fa] border border-slate-100 px-4 py-2 rounded-full text-xs font-semibold text-slate-700">
                <i className="fa-solid fa-briefcase-medical text-[#2386ea]"></i>
                الخبرة: {doctor.yearsOfExperience || 10}+ سنوات خبرة
              </span>
            </div>
          </div>

          {/* Reviews card */}
          <div className="bg-white rounded-xl border border-[#eef1f5] shadow-sm p-6 sm:p-8">
            <div className="flex items-center justify-between mb-6 border-b border-slate-50 pb-2">
              <h2 className="text-lg font-bold text-slate-900 flex items-center gap-2">
                <i className="fa-solid fa-users-viewfinder text-[#0e6cc9]"></i> آراء المرضى
              </h2>
              <button onClick={() => navigate(`/patient/rate/${id}`)} className="bg-[#2386ea] hover:bg-[#1a75d2] text-white text-xs font-bold px-4 py-2 rounded-lg cursor-pointer transition-colors">
                اكتب تعليقاً
              </button>
            </div>

            <div className="flex items-center gap-2 text-sm text-slate-500 mb-5">
              <strong className="font-extrabold text-slate-900 text-2xl">{Number(rating).toFixed(1)}</strong>
              <StarRating rating={rating} />
              <span>من {reviewsCount} تقييم</span>
            </div>

            <div className="divide-y divide-slate-100">
              {reviews.map((rev, idx) => (
                <div key={idx} className="py-5">
                  <div className="flex items-start justify-between mb-2">
                    <div className="flex items-center gap-3">
                      <div className="w-10 h-10 rounded-full bg-slate-100 flex items-center justify-center text-[#2386ea] font-extrabold text-sm flex-shrink-0 border border-slate-200">
                        {getInitials(rev.reviewerName)}
                      </div>
                      <div>
                        <p className="text-sm font-bold text-slate-800">{rev.reviewerName || 'مريض'}</p>
                        <StarRating rating={rev.rating || 5} size="text-xs" />
                      </div>
                    </div>
                    <span className="text-xs text-slate-400 font-medium">{rev.date || 'مؤخراً'}</span>
                  </div>
                  <p className="text-sm text-slate-600 leading-relaxed mt-2 pr-13">{rev.comment || 'لا توجد تعليقات.'}</p>
                </div>
              ))}
            </div>

            <a href="#" onClick={(e) => e.preventDefault()} className="block w-fit mx-auto mt-6 border border-slate-200 hover:border-[#2386ea] hover:text-[#2386ea] bg-white px-8 py-2.5 rounded-lg text-sm font-bold text-slate-600 transition-colors">
              عرض جميع المراجعات
            </a>
          </div>
        </div>

        {/* Left sidebar: Booking Widget (Matches sidebar on left side for RTL) */}
        <div className="lg:col-span-1">
          <div className="bg-white rounded-xl border border-[#eef1f5] shadow-sm p-6 sticky top-22">
            <h3 className="text-base font-bold text-slate-800 text-center mb-5 flex items-center justify-center gap-2 border-b border-slate-50 pb-2">
              <i className="fa-regular fa-clock text-[#0e6cc9]"></i> احجز موعداً
            </h3>

            {/* Price display */}
            <div className="text-center mb-5">
              <span className="text-3xl font-extrabold text-[#2386ea]">{price} جنيه</span>
              <span className="text-xs text-slate-400 mr-1.5 font-semibold">/ الكشف</span>
            </div>

            {/* Next Available Gradient Box */}
            <div className="bg-gradient-to-r from-[#6ab0f3] to-[#2386ea] text-white text-center py-3 px-4 rounded-lg text-xs font-bold mb-5 shadow-sm">
              <i className="fa-regular fa-calendar-check ml-1.5"></i>
              الموعد القادم المتاح: اليوم في {selectedSlot || '04:30 مساءً'}
            </div>

            {/* Time slots */}
            <p className="text-sm font-bold text-slate-800 mb-3 text-right">المواعيد المتاحة</p>
            <div className="grid grid-cols-2 gap-2 mb-5">
              {AVAILABLE_SLOTS.map((slot) => (
                <button
                  key={slot}
                  onClick={() => setSelectedSlot(slot)}
                  className={`py-2.5 px-2 rounded-lg text-xs font-bold transition-all border cursor-pointer ${
                    selectedSlot === slot
                      ? 'bg-[#2386ea] border-[#2386ea] text-white shadow-md shadow-[#2386ea]/20'
                      : 'bg-[#f0f7ff] border-transparent text-[#2386ea] hover:bg-[#e0f0ff]'
                  }`}
                >
                  {slot}
                </button>
              ))}
            </div>

            {/* Booking Meta */}
            <div className="border-t border-slate-100 pt-4 mb-5 space-y-2.5 text-xs text-slate-500 font-semibold">
              <div className="flex justify-between">
                <span>مدة الكشف:</span>
                <span className="text-slate-800">30 دقيقة</span>
              </div>
              <div className="flex justify-between">
                <span>نوع الاستشارة:</span>
                <span className="text-slate-800">في العيادة / كشف عام</span>
              </div>
            </div>

            <button
              onClick={() => navigate(`/patient/book/${id}${selectedSlot ? `?slot=${encodeURIComponent(selectedSlot)}` : ''}`)}
              className="w-full bg-[#0e6cc9] hover:bg-[#0b59a8] text-white font-extrabold py-3.5 rounded-lg transition-colors cursor-pointer text-sm shadow-md"
            >
              احجز الآن {selectedSlot ? `— ${selectedSlot}` : ''}
            </button>
          </div>
        </div>

      </div>
    </div>
  );
};
