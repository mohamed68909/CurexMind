import React, { useEffect, useState } from 'react';
import { useParams, useNavigate, useSearchParams, Link } from 'react-router-dom';
import Swal from 'sweetalert2';
import { apiClient } from '../api/apiClient';
import { useAuthStore } from '../store/authStore';

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
  { value: '1', label: 'First Visit' },
  { value: '2', label: 'Follow Up' },
  { value: '3', label: 'Checkup' },
];

export const BookAppointment: React.FC = () => {
  const { doctorId: urlDoctorId } = useParams<{ doctorId: string }>();
  const doctorId = urlDoctorId;
  const [searchParams] = useSearchParams();
  const navigate = useNavigate();
  // Read patientId from JWT claims via Zustand store
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
    // Convert "4:30 PM" → "16:30"
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

  useEffect(() => {
    if (!doctorId) return;
    apiClient.get(`/api/Doctors/${doctorId}`)
      .then((res: any) => setDoctor(res.data?.data ?? res.data))
      .catch(() => {/* silent */})
      .finally(() => setIsLoading(false));
  }, [doctorId]);

  const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement | HTMLTextAreaElement>) => {
    setForm((f) => ({ ...f, [e.target.name]: e.target.value }));
  };

  const handleNext = (e: React.FormEvent) => {
    e.preventDefault();
    if (!form.date || !form.time) {
      Swal.fire({ title: 'Required', text: 'Please select a date and time.', icon: 'warning' });
      return;
    }
    setStep('confirm');
  };

  const handleConfirm = async () => {
    if (!patientId) {
      Swal.fire({ title: 'Error', text: 'Patient ID not found. Please log in again.', icon: 'error' });
      return;
    }

    setIsSubmitting(true);

    // Build date+time as a Date object to send separately
    const [h, m] = form.time.split(':').map(Number);
    const appointmentDate = new Date(form.date);
    appointmentDate.setHours(0, 0, 0, 0);

    const payload = {
      doctorId,
      consultationType: form.appointmentType === '1' ? 'In_Person' : 'Video_Consultation',
      date: appointmentDate.toISOString(),
      time: `${String(h).padStart(2, '0')}:${String(m).padStart(2, '0')}:00`,
      reasonForVisit: form.notes || 'General consultation',
    };

    try {
      // Correct endpoint: POST /api/Appointments/patient/{patientId}/book
      const res = await apiClient.post(`/api/Appointments/patient/${patientId}/book`, payload);
      const newId = res.data?.appointmentId ?? res.data?.data?.appointmentId ?? 'N/A';
      setBookingId(newId);
      setStep('success');
    } catch (err: any) {
      const msg =
        err.response?.data?.errors?.[0]?.message ??
        err.response?.data?.title ??
        'Booking failed. Please try again.';
      Swal.fire({ title: 'Error!', text: msg, icon: 'error' });
    } finally {
      setIsSubmitting(false);
    }
  };

  const formatDate = (d: string) => d ? new Date(d).toLocaleDateString('en-GB', { weekday: 'long', year: 'numeric', month: 'long', day: 'numeric' }) : '—';
  const formatTime = (t: string) => {
    if (!t) return '—';
    const [h, m] = t.split(':').map(Number);
    const period = h >= 12 ? 'PM' : 'AM';
    const h12 = h % 12 || 12;
    return `${h12}:${String(m).padStart(2, '0')} ${period}`;
  };

  if (isLoading) {
    return (
      <div className="flex items-center justify-center min-h-[60vh]">
        <div className="text-center space-y-3">
          <div className="w-14 h-14 border-4 border-blue-600 border-t-transparent rounded-full animate-spin mx-auto"></div>
          <p className="text-slate-500">Loading...</p>
        </div>
      </div>
    );
  }

  return (
    <div className="min-h-screen bg-slate-50 py-8 px-4">
      <div className="max-w-2xl mx-auto space-y-6">
        {/* Header */}
        <div className="text-center space-y-1">
          <h1 className="text-2xl font-bold text-slate-800">Book an Appointment</h1>
          {doctor && (
            <p className="text-slate-500 text-sm">with <span className="font-semibold text-blue-600">Dr. {doctor.fullName}</span> · {doctor.specialization}</p>
          )}
        </div>

        {/* Progress Steps */}
        {step !== 'success' && (
          <div className="flex items-center justify-center gap-0">
            {(['datetime', 'confirm'] as const).map((s, idx) => {
              const active = step === s;
              const done = (step === 'confirm' && s === 'datetime');
              return (
                <React.Fragment key={s}>
                  <div className={`flex items-center gap-2 px-4 py-2 rounded-full text-sm font-semibold transition-all ${active ? 'bg-blue-600 text-white' : done ? 'bg-emerald-100 text-emerald-700' : 'bg-slate-100 text-slate-400'}`}>
                    {done ? <i className="fa-solid fa-check text-xs"></i> : <span>{idx + 1}</span>}
                    <span>{s === 'datetime' ? 'Date & Time' : 'Confirm'}</span>
                  </div>
                  {idx < 1 && <div className="w-8 h-0.5 bg-slate-200"></div>}
                </React.Fragment>
              );
            })}
          </div>
        )}

        {/* STEP 1: Date & Time */}
        {step === 'datetime' && (
          <div className="bg-white rounded-2xl border border-slate-200 shadow-sm overflow-hidden">
            {doctor && (
              <div className="flex items-center gap-4 px-6 py-4 bg-slate-50 border-b border-slate-200">
                <div className="w-12 h-12 rounded-full overflow-hidden bg-blue-100 flex-shrink-0">
                  {doctor.profileImageUrl
                    ? <img src={doctor.profileImageUrl} alt="" className="w-full h-full object-cover" />
                    : <div className="w-full h-full flex items-center justify-center text-blue-500 font-bold">{doctor.fullName?.[0]}</div>
                  }
                </div>
                <div>
                  <p className="font-semibold text-slate-800">Dr. {doctor.fullName}</p>
                  <p className="text-xs text-slate-500">{doctor.clinicName}</p>
                </div>
                <div className="ml-auto text-right">
                  <p className="text-xl font-extrabold text-blue-600">EGP {doctor.price || 200}</p>
                  <p className="text-xs text-slate-400">per visit</p>
                </div>
              </div>
            )}

            <form onSubmit={handleNext} className="p-6 space-y-5">
              <div className="grid grid-cols-1 sm:grid-cols-2 gap-5">
                <div className="flex flex-col gap-1.5">
                  <label className="text-sm font-medium text-slate-700">Select Date <span className="text-red-500">*</span></label>
                  <input
                    name="date" type="date"
                    min={new Date().toISOString().split('T')[0]}
                    value={form.date} onChange={handleChange} required
                    className="px-3 py-2.5 border border-slate-200 rounded-xl text-sm text-slate-800 outline-none focus:border-blue-500 focus:ring-2 focus:ring-blue-100 transition"
                  />
                </div>

                <div className="flex flex-col gap-1.5">
                  <label className="text-sm font-medium text-slate-700">Select Time <span className="text-red-500">*</span></label>
                  <input
                    name="time" type="time"
                    value={form.time} onChange={handleChange} required
                    className="px-3 py-2.5 border border-slate-200 rounded-xl text-sm text-slate-800 outline-none focus:border-blue-500 focus:ring-2 focus:ring-blue-100 transition"
                  />
                </div>
              </div>

              <div className="flex flex-col gap-1.5">
                <label className="text-sm font-medium text-slate-700">Appointment Type</label>
                <select name="appointmentType" value={form.appointmentType} onChange={handleChange}
                  className="px-3 py-2.5 border border-slate-200 rounded-xl text-sm text-slate-800 outline-none focus:border-blue-500 focus:ring-2 focus:ring-blue-100 transition bg-white">
                  {APPOINTMENT_TYPES.map((t) => <option key={t.value} value={t.value}>{t.label}</option>)}
                </select>
              </div>

              <div className="flex flex-col gap-1.5">
                <label className="text-sm font-medium text-slate-700">Notes <span className="text-slate-400 text-xs">(Optional)</span></label>
                <textarea
                  name="notes" value={form.notes} onChange={handleChange}
                  rows={3} placeholder="Describe your symptoms or reason for visit..."
                  className="px-3 py-2.5 border border-slate-200 rounded-xl text-sm text-slate-800 outline-none focus:border-blue-500 focus:ring-2 focus:ring-blue-100 transition resize-y"
                />
              </div>

              <div className="flex justify-between pt-2">
                <Link to={doctorId ? `/patient/doctors/${doctorId}` : '/patient'}
                  className="px-5 py-2.5 border border-slate-200 rounded-xl text-slate-600 text-sm font-medium hover:bg-slate-50 transition-colors">
                  Back
                </Link>
                <button type="submit"
                  className="px-6 py-2.5 bg-blue-600 hover:bg-blue-700 text-white rounded-xl text-sm font-semibold transition-colors flex items-center gap-2">
                  Continue <i className="fa-solid fa-arrow-right text-xs"></i>
                </button>
              </div>
            </form>
          </div>
        )}

        {/* STEP 2: Confirm */}
        {step === 'confirm' && (
          <div className="bg-white rounded-2xl border border-slate-200 shadow-sm overflow-hidden">
            <div className="px-6 py-4 bg-slate-50 border-b border-slate-200">
              <h2 className="font-semibold text-slate-700 flex items-center gap-2"><i className="fa-regular fa-circle-check text-blue-500"></i> Confirm Your Booking</h2>
            </div>
            <div className="p-6 space-y-4">
              {/* Summary */}
              <div className="bg-blue-50 border border-blue-100 rounded-xl p-5 space-y-3">
                {doctor && (
                  <div className="flex items-center gap-3 pb-3 border-b border-blue-100">
                    <div className="w-10 h-10 rounded-full overflow-hidden bg-blue-200 flex-shrink-0">
                      {doctor.profileImageUrl
                        ? <img src={doctor.profileImageUrl} alt="" className="w-full h-full object-cover" />
                        : <div className="w-full h-full flex items-center justify-center text-blue-600 font-bold text-sm">{doctor.fullName?.[0]}</div>
                      }
                    </div>
                    <div>
                      <p className="font-bold text-slate-800 text-sm">Dr. {doctor.fullName}</p>
                      <p className="text-xs text-slate-500">{doctor.specialization}</p>
                    </div>
                  </div>
                )}
                {[
                  { icon: 'fa-calendar-day', label: 'Date', value: formatDate(form.date) },
                  { icon: 'fa-clock', label: 'Time', value: formatTime(form.time) },
                  { icon: 'fa-stethoscope', label: 'Type', value: APPOINTMENT_TYPES.find((t) => t.value === form.appointmentType)?.label ?? '—' },
                  { icon: 'fa-location-dot', label: 'Clinic', value: doctor?.clinicName ?? '—' },
                  { icon: 'fa-file-invoice-dollar', label: 'Fee', value: `EGP ${doctor?.price || 200}` },
                ].map((item) => (
                  <div key={item.label} className="flex items-center justify-between text-sm">
                    <span className="text-slate-500 flex items-center gap-2">
                      <i className={`fa-solid ${item.icon} text-blue-400 w-4`}></i>{item.label}
                    </span>
                    <span className="font-semibold text-slate-800">{item.value}</span>
                  </div>
                ))}
                {form.notes && (
                  <div className="pt-2 border-t border-blue-100">
                    <p className="text-xs text-slate-500 mb-1">Notes:</p>
                    <p className="text-sm text-slate-700 italic">"{form.notes}"</p>
                  </div>
                )}
              </div>

              <p className="text-xs text-slate-400 text-center">By confirming, you agree to our cancellation policy. You may cancel up to 24 hours before your appointment.</p>

              <div className="flex justify-between pt-2">
                <button onClick={() => setStep('datetime')}
                  className="px-5 py-2.5 border border-slate-200 rounded-xl text-slate-600 text-sm font-medium hover:bg-slate-50 transition-colors">
                  Back
                </button>
                <button onClick={handleConfirm} disabled={isSubmitting}
                  className="px-6 py-2.5 bg-emerald-600 hover:bg-emerald-700 disabled:bg-emerald-300 text-white rounded-xl text-sm font-bold transition-colors flex items-center gap-2">
                  {isSubmitting ? <><i className="fa-solid fa-circle-notch fa-spin"></i> Confirming...</> : <><i className="fa-solid fa-check"></i> Confirm Booking</>}
                </button>
              </div>
            </div>
          </div>
        )}

        {/* STEP 3: Success */}
        {step === 'success' && (
          <div className="bg-white rounded-2xl border border-slate-200 shadow-sm overflow-hidden text-center p-10 space-y-6">
            <div className="w-20 h-20 rounded-full bg-emerald-100 flex items-center justify-center mx-auto">
              <i className="fa-solid fa-circle-check text-4xl text-emerald-500"></i>
            </div>
            <div>
              <h2 className="text-2xl font-bold text-slate-800">Booking Confirmed!</h2>
              <p className="text-slate-500 mt-2 text-sm">Your appointment has been successfully scheduled.</p>
            </div>

            <div className="bg-slate-50 rounded-xl p-4 text-sm space-y-2 text-left max-w-xs mx-auto">
              {doctor && <p className="font-semibold text-slate-800">Dr. {doctor.fullName}</p>}
              <p className="text-slate-600"><i className="fa-regular fa-calendar mr-2 text-blue-500"></i>{formatDate(form.date)}</p>
              <p className="text-slate-600"><i className="fa-regular fa-clock mr-2 text-blue-500"></i>{formatTime(form.time)}</p>
              {bookingId && bookingId !== 'N/A' && <p className="text-xs text-slate-400">Ref: #{bookingId.slice(0, 8).toUpperCase()}</p>}
            </div>

            <div className="flex gap-3 justify-center flex-wrap">
              <Link to="/patient/appointments"
                className="px-5 py-2.5 bg-blue-600 hover:bg-blue-700 text-white rounded-xl text-sm font-semibold transition-colors">
                View My Appointments
              </Link>
              <Link to="/patient"
                className="px-5 py-2.5 border border-slate-200 hover:bg-slate-50 text-slate-600 rounded-xl text-sm font-medium transition-colors">
                Browse More Doctors
              </Link>
            </div>
          </div>
        )}
      </div>
    </div>
  );
};
