import React, { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { apiClient } from '../api/apiClient';
import { useSettingsStore } from '../store/settingsStore';
import { Search, Star, Loader2, AlertCircle, Calendar, Shield, Activity } from 'lucide-react';
import { useAuthStore } from '../store/authStore';
import { isFemaleDoctorName, getDoctorAvatarUrl } from '../utils/genderHelper';

interface Doctor {
  id: string;
  fullName: string;
  specialization: string;
  rating: number;
  reviewsCount: number;
  price: number;
  clinicName: string;
}

export const PatientHome: React.FC = () => {
  const navigate = useNavigate();
  const { fullName } = useAuthStore();
  const { language, t } = useSettingsStore();
  
  const [doctors, setDoctors] = useState<Doctor[]>([]);
  const [filteredDoctors, setFilteredDoctors] = useState<Doctor[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  // Filters State
  const [searchQuery, setSearchQuery] = useState('');
  const [specialty, setSpecialty] = useState('');
  const [rating, setRating] = useState('');
  const [gender, setGender] = useState('');
  const [sortBy, setSortBy] = useState('');

  const [page, setPage] = useState(1);
  const [totalPages, setTotalPages] = useState(1);
  const [totalCount, setTotalCount] = useState(0);

  const fetchDoctors = async (pageNumber = 1) => {
    try {
      setLoading(true);
      setError(null);
      
      const response = await apiClient.get('/api/Doctors', {
        params: {
          page: pageNumber,
          pageSize: 10
        }
      });

      const resObj = response.data;
      const list = Array.isArray(resObj?.data)
        ? resObj.data
        : (Array.isArray(resObj?.items)
          ? resObj.items
          : (Array.isArray(resObj) ? resObj : []));

      setDoctors(list);
      setFilteredDoctors(list);
      setPage(resObj?.page || pageNumber);
      setTotalPages(resObj?.totalPages || Math.ceil((resObj?.totalCount || list.length) / 10) || 1);
      setTotalCount(resObj?.totalCount || list.length);
    } catch (err: any) {
      console.error('Error fetching doctors:', err);
      setError(language === 'ar' 
        ? 'حدث خطأ أثناء تحميل سجل الأطباء. يرجى التحقق من اتصالك بالإنترنت.' 
        : 'Failed to load doctors list. Please check your internet connection.');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchDoctors(page);
  }, [language, page]);

  const handleApplyFilters = () => {
    let result = [...doctors];

    // Search query check
    if (searchQuery.trim() !== '') {
      const q = searchQuery.toLowerCase();
      result = result.filter(doc => 
        doc.fullName.toLowerCase().includes(q) ||
        doc.specialization.toLowerCase().includes(q) ||
        doc.clinicName.toLowerCase().includes(q)
      );
    }

    // Specialty filter
    if (specialty !== '') {
      result = result.filter(doc => doc.specialization === specialty);
    }

    // Rating filter
    if (rating !== '') {
      result = result.filter(doc => doc.rating >= parseFloat(rating));
    }

    // Gender filter (detect from female name keywords)
    if (gender !== '') {
      result = result.filter(doc => {
        const isFemale = isFemaleDoctorName(doc.fullName);
        return gender === 'female' ? isFemale : !isFemale;
      });
    }

    // Sort check
    if (sortBy === 'rating_desc') {
      result.sort((a, b) => b.rating - a.rating);
    } else if (sortBy === 'price_asc') {
      result.sort((a, b) => a.price - b.price);
    }

    setFilteredDoctors(result);
  };

  useEffect(() => {
    handleApplyFilters();
  }, [specialty, rating, gender, sortBy, searchQuery]);

  const isRtl = language === 'ar';
  const textAlignment = isRtl ? 'text-right' : 'text-left';

  return (
    <div className={`space-y-8 ${textAlignment} font-cairo max-w-7xl mx-auto px-2`} dir={isRtl ? 'rtl' : 'ltr'}>
      
      {/* Greeting Banner */}
      <section className="text-center py-6 space-y-2">
        <h1 className="text-3xl sm:text-4xl font-black text-slate-850 dark:text-white tracking-tight">
          {isRtl ? `مرحبا بكم ${fullName || 'أحمد حسن'}` : `Welcome ${fullName || 'Ahmed Hassan'}`}
        </h1>
        <p className="text-slate-500 dark:text-slate-400 text-sm sm:text-base font-semibold">
          {isRtl ? 'البحث عن المواعيد وحجزها مع مقدمي الرعاية الصحية الموثوق بهم' : 'Search and book appointments with trusted healthcare providers'}
        </p>

        {/* Centered Search Bar */}
        <div className="max-w-2xl mx-auto pt-4">
          <div className="relative flex items-center bg-white dark:bg-slate-900 border border-slate-200/80 dark:border-slate-800 rounded-2xl shadow-sm hover:shadow-md transition-all p-1.5">
            <input 
              type="text" 
              placeholder={isRtl ? "البحث عن طريق الطبيب أو العيادة أو التخصص ..." : "Search by doctor, clinic, or specialty..."}
              value={searchQuery}
              onChange={(e) => setSearchQuery(e.target.value)}
              className="w-full border-0 outline-none px-4 py-2.5 text-sm font-cairo text-slate-700 dark:text-slate-200 bg-transparent"
            />
            <button 
              onClick={handleApplyFilters}
              className="p-3 text-slate-400 hover:text-[#1a73e8] transition-colors cursor-pointer"
            >
              <Search className="h-5 w-5" />
            </button>
          </div>
        </div>
      </section>

      {/* Filter Box Card */}
      <div className="bg-white dark:bg-slate-900 border border-slate-200/80 dark:border-slate-800 p-6 rounded-2xl shadow-sm space-y-4">
        <div className="flex items-center justify-between">
          <div className="flex items-center gap-2 text-[#1a73e8] font-bold text-sm">
            <i className="fa-solid fa-filter text-xs"></i>
            <span>{isRtl ? 'الفلاتر' : 'Filters'}</span>
          </div>
        </div>

        <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-5 gap-4 items-end">
          {/* Specialty */}
          <div className="space-y-1">
            <label className="text-xs font-bold text-slate-500 dark:text-slate-400">{isRtl ? 'التخصص' : 'Specialty'}</label>
            <select 
              value={specialty}
              onChange={(e) => setSpecialty(e.target.value)}
              className="w-full border border-slate-200 dark:border-slate-700 focus:border-[#1a73e8] rounded-xl px-3.5 py-2.5 text-xs outline-none bg-white dark:bg-slate-850 font-cairo text-slate-700 dark:text-slate-300 cursor-pointer"
            >
              <option value="">{isRtl ? 'جميع التخصصات' : 'All Specialties'}</option>
              <option value="Cardiology">{isRtl ? 'أمراض القلب' : 'Cardiology'}</option>
              <option value="Pediatrics">{isRtl ? 'طب الأطفال' : 'Pediatrics'}</option>
              <option value="Dermatology">{isRtl ? 'الأمراض الجلدية' : 'Dermatology'}</option>
              <option value="Orthopedics">{isRtl ? 'جراحة العظام' : 'Orthopedics'}</option>
              <option value="Neurology">{isRtl ? 'طب الأعصاب' : 'Neurology'}</option>
              <option value="General Surgery">{isRtl ? 'جراحة عامة' : 'General Surgery'}</option>
            </select>
          </div>

          {/* Visit Type Toggle */}
          <div className="space-y-1">
            <label className="text-xs font-bold text-slate-500 dark:text-slate-400">{isRtl ? 'نوع الزيارة' : 'Visit Type'}</label>
            <div className="flex bg-slate-100 dark:bg-slate-800 p-1 rounded-xl gap-1 text-xs font-bold">
              <button 
                onClick={() => setGender('')}
                className={`flex-1 py-1.5 rounded-lg transition-all cursor-pointer ${gender === '' ? 'bg-[#1a73e8] text-white shadow-sm' : 'text-slate-600 dark:text-slate-400 hover:text-slate-900'}`}
              >
                {isRtl ? 'الكل' : 'All'}
              </button>
              <button 
                onClick={() => setGender('male')}
                className={`flex-1 py-1.5 rounded-lg transition-all cursor-pointer ${gender === 'male' ? 'bg-[#1a73e8] text-white shadow-sm' : 'text-slate-600 dark:text-slate-400 hover:text-slate-900'}`}
              >
                {isRtl ? 'شخصياً' : 'In-Person'}
              </button>
              <button 
                onClick={() => setGender('female')}
                className={`flex-1 py-1.5 rounded-lg transition-all cursor-pointer ${gender === 'female' ? 'bg-[#1a73e8] text-white shadow-sm' : 'text-slate-600 dark:text-slate-400 hover:text-slate-900'}`}
              >
                {isRtl ? 'عبر الإنترنت' : 'Online'}
              </button>
            </div>
          </div>

          {/* Availability */}
          <div className="space-y-1">
            <label className="text-xs font-bold text-slate-500 dark:text-slate-400">{isRtl ? 'التوافر' : 'Availability'}</label>
            <select 
              className="w-full border border-slate-200 dark:border-slate-700 focus:border-[#1a73e8] rounded-xl px-3.5 py-2.5 text-xs outline-none bg-white dark:bg-slate-850 font-cairo text-slate-700 dark:text-slate-300 cursor-pointer"
            >
              <option value="">{isRtl ? 'في أي وقت' : 'Anytime'}</option>
              <option value="today">{isRtl ? 'اليوم' : 'Today'}</option>
              <option value="tomorrow">{isRtl ? 'غداً' : 'Tomorrow'}</option>
            </select>
          </div>

          {/* Sort By */}
          <div className="space-y-1">
            <label className="text-xs font-bold text-slate-500 dark:text-slate-400">{isRtl ? 'فرز حسب' : 'Sort By'}</label>
            <select 
              value={sortBy}
              onChange={(e) => setSortBy(e.target.value)}
              className="w-full border border-slate-200 dark:border-slate-700 focus:border-[#1a73e8] rounded-xl px-3.5 py-2.5 text-xs outline-none bg-white dark:bg-slate-850 font-cairo text-slate-700 dark:text-slate-300 cursor-pointer"
            >
              <option value="">{isRtl ? 'أعلى تصنيف' : 'Highest Rated'}</option>
              <option value="rating_desc">{isRtl ? 'الأعلى تقييماً' : 'Most Rated'}</option>
              <option value="price_asc">{isRtl ? 'الأقل سعراً' : 'Price Low to High'}</option>
            </select>
          </div>

          {/* Apply Button */}
          <button
            onClick={handleApplyFilters}
            className="w-full bg-[#1a73e8] hover:bg-[#1557b0] text-white font-bold py-2.5 rounded-xl text-xs transition-colors cursor-pointer shadow-sm"
          >
            {isRtl ? 'تطبيق الفلاتر' : 'Apply Filters'}
          </button>
        </div>
      </div>

      {/* Available Doctors Section */}
      <div className="space-y-4 pt-2">
        <h2 className="text-xl font-extrabold text-slate-850 dark:text-white">
          {isRtl ? `الأطباء المتاحون (${filteredDoctors.length})` : `Available Doctors (${filteredDoctors.length})`}
        </h2>

        {loading ? (
          <div className="flex flex-col items-center justify-center min-h-[300px] gap-3">
            <Loader2 className="h-8 w-8 text-[#1a73e8] animate-spin" />
            <p className="text-slate-500 dark:text-slate-400 text-xs font-semibold">{t('common.loading')}</p>
          </div>
        ) : error ? (
          <div className="bg-red-50 dark:bg-red-950/20 border border-red-200 dark:border-red-900/40 text-red-700 dark:text-red-400 p-6 rounded-xl flex items-start gap-4 shadow-sm">
            <AlertCircle className="h-6 w-6 flex-shrink-0 mt-0.5" />
            <div>
              <h3 className="font-bold text-sm">{isRtl ? 'فشل جلب البيانات' : 'Error'}</h3>
              <p className="text-xs mt-1">{error}</p>
            </div>
          </div>
        ) : (
          <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-6">
            {filteredDoctors.length === 0 ? (
              <div className="col-span-full bg-white border border-slate-200 p-12 text-center rounded-2xl text-slate-400 font-bold shadow-sm">
                {t('patient.noDoctorsFound')}
              </div>
            ) : (
              filteredDoctors.map((doc, idx) => {
                const imageUrl = getDoctorAvatarUrl(doc.fullName, idx);

                return (
                  <div 
                    key={doc.id} 
                    onClick={() => navigate(`/patient/doctors/${doc.id}`)}
                    className="bg-white dark:bg-slate-900 border border-slate-200/70 dark:border-slate-800 rounded-2xl p-6 shadow-sm hover:shadow-md transition-all cursor-pointer flex flex-col justify-between items-center text-center gap-4 relative group"
                  >
                    {/* Avatar Circle with Verified Checkmark */}
                    <div className="relative h-24 w-24 rounded-full border-2 border-slate-100 dark:border-slate-800 overflow-hidden shrink-0 shadow-sm">
                      <img src={imageUrl} alt={doc.fullName} className="h-full w-full object-cover" />
                      <span className="absolute bottom-1 left-1 bg-[#1a73e8] text-white p-1 rounded-full text-[10px] flex items-center justify-center border border-white">
                        <i className="fa-solid fa-check"></i>
                      </span>
                    </div>

                    {/* Doctor Info */}
                    <div className="space-y-1">
                      <h3 className="font-extrabold text-slate-850 dark:text-white text-base group-hover:text-[#1a73e8] transition-colors">{doc.fullName}</h3>
                      <p className="text-xs text-[#1a73e8] font-bold">{doc.specialization}</p>
                      <p className="text-[11px] text-slate-400 font-medium flex items-center justify-center gap-1">
                        <i className="fa-solid fa-location-dot text-[10px] text-slate-400"></i>
                        <span>{doc.clinicName || 'مركز CurexMind الطبي'}</span>
                      </p>
                    </div>

                    {/* Rating */}
                    <div className="flex items-center gap-1 justify-center text-xs">
                      <i className="fa-solid fa-star text-amber-400 text-xs"></i>
                      <span className="text-slate-800 dark:text-slate-200 font-extrabold">{doc.rating || '4.8'}</span>
                      <span className="text-[11px] text-slate-400 font-semibold">(+{doc.reviewsCount || 120} تعليقات)</span>
                    </div>

                    {/* Badges */}
                    <div className="flex items-center gap-2 justify-center flex-wrap">
                      <span className="text-[10px] font-bold bg-blue-50 text-[#1a73e8] px-2.5 py-1 rounded-lg">شخصياً</span>
                      <span className="text-[10px] font-bold bg-blue-50 text-[#1a73e8] px-2.5 py-1 rounded-lg">عبر الإنترنت</span>
                    </div>

                    {/* Next slot */}
                    <div className="text-[11px] text-slate-500 font-medium flex items-center gap-1.5 justify-center">
                      <i className="fa-regular fa-clock text-[10px]"></i>
                      <span>التالي: اليوم, 2:00 PM</span>
                    </div>

                    {/* Action Button */}
                    <button 
                      onClick={(e) => {
                        e.stopPropagation();
                        navigate(`/patient/book/${doc.id}`);
                      }}
                      className="w-full bg-[#1a73e8] hover:bg-[#1557b0] text-white font-bold text-xs py-3 rounded-xl transition-colors cursor-pointer shadow-sm mt-1"
                    >
                      احجز موعداً
                    </button>
                  </div>
                );
              })
            )}
          </div>
        )}
      </div>

      {/* Pagination Controls */}
      {!loading && !error && filteredDoctors.length > 0 && (
        <div className="flex items-center justify-between bg-white dark:bg-slate-900 border border-slate-200 dark:border-slate-800 rounded-2xl p-4 mt-8 shadow-sm">
          <button
            disabled={page <= 1}
            onClick={() => setPage(p => Math.max(1, p - 1))}
            className="px-4 py-2 bg-slate-100 dark:bg-slate-800 hover:bg-slate-200 dark:hover:bg-slate-700 disabled:opacity-40 text-slate-700 dark:text-slate-200 font-bold rounded-xl transition-colors cursor-pointer text-xs"
          >
            {language === 'ar' ? 'السابقة' : 'Previous'}
          </button>
          
          <span className="text-xs font-bold text-slate-700 dark:text-slate-300">
            {language === 'ar' 
              ? `الصفحة ${page} من ${totalPages}` 
              : `Page ${page} of ${totalPages}`}
          </span>

          <button
            disabled={page >= totalPages}
            onClick={() => setPage(p => Math.min(totalPages, p + 1))}
            className="px-4 py-2 bg-[#1a73e8] hover:bg-[#1557b0] disabled:opacity-40 text-white font-bold rounded-xl transition-colors cursor-pointer text-xs"
          >
            {language === 'ar' ? 'التالية' : 'Next'}
          </button>
        </div>
      )}
    </div>
  );
};
