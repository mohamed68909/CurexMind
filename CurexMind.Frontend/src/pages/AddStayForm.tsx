import React, { useEffect, useState } from 'react';
import { useNavigate, useLocation } from 'react-router-dom';
import Swal from 'sweetalert2';
import { apiClient } from '../api/apiClient';
import { 
  Bed, 
  User, 
  Calendar, 
  ArrowLeft, 
  Save, 
  Loader2, 
  Info 
} from 'lucide-react';

interface Patient {
  patientId: string;
  fullName: string;
  phoneNumber: string;
}

interface PatientDetails {
  gender: number;
  nationalId: string;
  phoneNumber: string;
  dateOfBirth: string;
}

const HOSPITAL_STRUCTURE: Record<string, Record<string, string[]>> = {
  'Cardiology': { 'Room 101': ['Bed A', 'Bed B'], 'Room 102': ['Bed A'] },
  'Orthopedics': { 'Room 201': ['Bed A', 'Bed B', 'Bed C'], 'Room 202': ['Bed A', 'Bed B'] },
  'General Surgery': { 'Room 301': ['Bed A'], 'Room 302': ['Bed A', 'Bed B'] },
  'ICU': { 'ICU-01': ['Bed 1'], 'ICU-02': ['Bed 2'], 'ICU-03': ['Bed 3'] },
  'Pediatrics': { 'Room 401': ['Crib A', 'Crib B'], 'Room 402': ['Bed A'] }
};

export const AddStayForm: React.FC = () => {
  const navigate = useNavigate();
  const location = useLocation();
  const [patients, setPatients] = useState<Patient[]>([]);
  const [selectedPatientDetails, setSelectedPatientDetails] = useState<PatientDetails | null>(null);
  
  const [loadingPatients, setLoadingPatients] = useState(true);
  const [loadingDetails, setLoadingDetails] = useState(false);
  const [isSubmitting, setIsSubmitting] = useState(false);

  const [form, setForm] = useState({
    patientId: '',
    department: '',
    roomNumber: '',
    bedNumber: '',
    stayType: '0',
    startDate: '',
    endDate: '',
    notes: ''
  });

  // Initialize start date to now
  useEffect(() => {
    const now = new Date();
    now.setMinutes(now.getMinutes() - now.getTimezoneOffset());
    setForm(prev => ({ ...prev, startDate: now.toISOString().slice(0, 16) }));
  }, []);

  useEffect(() => {
    if (location.state?.patientId) {
      setForm(prev => ({ ...prev, patientId: location.state.patientId }));
    }
  }, [location.state]);

  // Fetch Patients List
  useEffect(() => {
    const loadPatients = async () => {
      try {
        setLoadingPatients(true);
        const res = await apiClient.get('/api/Patients?page=1&pageSize=100');
        const data = res.data?.items ?? res.data?.data ?? res.data ?? [];
        setPatients(data);
      } catch (err) {
        console.error('Failed to load patients list:', err);
        Swal.fire({
          title: 'خطأ في التحميل',
          text: 'تعذر تحميل قائمة المرضى من الخادم.',
          icon: 'error',
          confirmButtonColor: '#3b82f6'
        });
      } finally {
        setLoadingPatients(false);
      }
    };
    loadPatients();
  }, []);

  // Fetch patient details when selected patient changes
  useEffect(() => {
    if (!form.patientId) {
      setSelectedPatientDetails(null);
      return;
    }

    const loadPatientDetails = async () => {
      try {
        setLoadingDetails(true);
        const res = await apiClient.get(`/api/Patients/${form.patientId}`);
        setSelectedPatientDetails(res.data);
      } catch (err) {
        console.error('Failed to load patient details:', err);
        setSelectedPatientDetails(null);
      } finally {
        setLoadingDetails(false);
      }
    };
    loadPatientDetails();
  }, [form.patientId]);

  // Handle department change - reset room and bed
  const handleDepartmentChange = (e: React.ChangeEvent<HTMLSelectElement>) => {
    const dept = e.target.value;
    setForm(prev => ({
      ...prev,
      department: dept,
      roomNumber: '',
      bedNumber: ''
    }));
  };

  // Handle room change - reset bed
  const handleRoomChange = (e: React.ChangeEvent<HTMLSelectElement>) => {
    const room = e.target.value;
    setForm(prev => ({
      ...prev,
      roomNumber: room,
      bedNumber: ''
    }));
  };

  const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement | HTMLTextAreaElement>) => {
    const { name, value } = e.target;
    setForm(prev => ({ ...prev, [name]: value }));
  };

  // Calculate duration dynamically
  const calculateDuration = () => {
    if (form.startDate && form.endDate) {
      const start = new Date(form.startDate);
      const end = new Date(form.endDate);
      if (end < start) return 'Invalid dates (End is before Start)';
      const diffTime = Math.abs(end.getTime() - start.getTime());
      const diffDays = Math.ceil(diffTime / (1000 * 60 * 60 * 24));
      return `${diffDays} Day${diffDays > 1 ? 's' : ''}`;
    }
    return 'Auto-calculated';
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    if (!form.patientId || !form.department || !form.roomNumber || !form.bedNumber) {
      Swal.fire('تنبيه', 'يرجى ملء جميع الحقول المطلوبة.', 'warning');
      return;
    }

    const start = new Date(form.startDate);
    const end = new Date(form.endDate);

    if (form.endDate && end <= start) {
      Swal.fire('خطأ في تاريخ الخروج', 'تاريخ الخروج المتوقع يجب أن يكون بعد تاريخ الدخول.', 'warning');
      return;
    }

    try {
      setIsSubmitting(true);
      const payload = {
        patientId: form.patientId,
        department: form.department,
        roomNumber: form.roomNumber,
        bedNumber: form.bedNumber,
        stayType: parseInt(form.stayType),
        startDate: start.toISOString(),
        endDate: form.endDate ? end.toISOString() : null,
        notes: form.notes || 'No notes'
      };

      await apiClient.post('/api/Stay', payload);

      Swal.fire({
        title: 'تم بنجاح!',
        text: 'تم حجز الغرفة وتخصيص السرير للمريض بنجاح.',
        icon: 'success',
        confirmButtonColor: '#3b82f6',
        confirmButtonText: 'الذهاب لكشف الاستضافة'
      }).then(() => {
        navigate('/management/stays');
      });

    } catch (err: any) {
      console.error('Failed to create stay:', err);
      let errorMsg = 'تعذر حجز الغرفة وتخصيص الإقامة.';
      const serverMsg = err.response?.data?.errors?.[0]?.message;
      if (serverMsg) {
        errorMsg = serverMsg;
        if (serverMsg.includes('Failed to create')) {
          errorMsg += '<br><br><b>ملاحظة:</b> قد يكون هذا المريض مسجلاً بالفعل في إقامة نشطة حالية. يرجى تسريح المريض أولاً أو التحقق من حالته.';
        }
      } else if (err.response?.data?.title) {
        errorMsg = err.response.data.title;
      }
      
      Swal.fire({
        title: 'فشلت العملية',
        html: errorMsg,
        icon: 'error',
        confirmButtonColor: '#3b82f6'
      });
    } finally {
      setIsSubmitting(false);
    }
  };

  const rooms = form.department ? Object.keys(HOSPITAL_STRUCTURE[form.department] || {}) : [];
  const beds = (form.department && form.roomNumber) ? HOSPITAL_STRUCTURE[form.department][form.roomNumber] || [] : [];

  return (
    <div className="space-y-6 text-left" dir="ltr">
      {/* Header */}
      <div className="flex items-center gap-4">
        <button 
          onClick={() => navigate(-1)} 
          className="p-2 bg-white hover:bg-slate-50 border border-slate-200 rounded-xl transition-colors text-slate-600 shadow-sm"
        >
          <ArrowLeft className="h-5 w-5" />
        </button>
        <div>
          <h1 className="text-2xl font-bold text-slate-900 font-cairo">Allocate Bed & Stay</h1>
          <p className="text-xs text-slate-500 font-cairo">Register a new patient stay admission and allocate a ward bed</p>
        </div>
      </div>

      {loadingPatients ? (
        <div className="flex flex-col items-center justify-center min-h-[300px] gap-3 bg-white border border-slate-200 rounded-2xl">
          <Loader2 className="h-8 w-8 text-blue-600 animate-spin" />
          <p className="text-slate-500 text-xs font-semibold">Loading patient list...</p>
        </div>
      ) : (
        <form onSubmit={handleSubmit} className="space-y-6 max-w-5xl">
          
          {/* Card 1: Patient Information */}
          <div className="bg-white rounded-2xl border border-slate-200 overflow-hidden shadow-sm">
            <div className="bg-slate-50 px-6 py-4 border-b border-slate-200 flex items-center justify-between">
              <div className="flex items-center gap-2">
                <User className="h-5 w-5 text-blue-600" />
                <h3 className="font-bold text-slate-800 text-sm">Patient Selection</h3>
              </div>
              <button
                type="button"
                onClick={() => navigate('/management/patients/add')}
                className="text-xs font-bold text-blue-600 hover:text-blue-700 bg-white border border-slate-200 px-3 py-1.5 rounded-lg transition-colors shadow-sm"
              >
                + Add New Patient
              </button>
            </div>

            <div className="p-6 space-y-5">

            <div>
              <label className="block text-xs font-bold text-slate-700 mb-1.5">Select Patient *</label>
              <select
                name="patientId"
                value={form.patientId}
                onChange={handleChange}
                className="w-full px-3 py-2 border border-slate-200 rounded-lg text-sm bg-white focus:outline-none focus:ring-2 focus:ring-blue-100 focus:border-blue-500"
                required
              >
                <option value="" disabled>Search and select patient...</option>
                {patients.map(p => (
                  <option key={p.patientId} value={p.patientId}>
                    {p.fullName} ({p.phoneNumber})
                  </option>
                ))}
              </select>
            </div>

            {/* Auto-filled details */}
            {selectedPatientDetails && (
              <div className="grid grid-cols-2 md:grid-cols-4 gap-4 p-4 bg-slate-50 rounded-xl border border-slate-100 text-xs">
                <div>
                  <span className="text-slate-400 block mb-0.5">Gender</span>
                  <span className="font-bold text-slate-800">
                    {selectedPatientDetails.gender === 1 ? 'Male' : selectedPatientDetails.gender === 2 ? 'Female' : 'Not Specified'}
                  </span>
                </div>
                <div>
                  <span className="text-slate-400 block mb-0.5">National ID</span>
                  <span className="font-bold text-slate-800">{selectedPatientDetails.nationalId || 'N/A'}</span>
                </div>
                <div>
                  <span className="text-slate-400 block mb-0.5">Phone Number</span>
                  <span className="font-bold text-slate-800">{selectedPatientDetails.phoneNumber || 'N/A'}</span>
                </div>
                <div>
                  <span className="text-slate-400 block mb-0.5">Birthdate</span>
                  <span className="font-bold text-slate-800">
                    {selectedPatientDetails.dateOfBirth ? new Date(selectedPatientDetails.dateOfBirth).toLocaleDateString('en-GB') : 'N/A'}
                  </span>
                </div>
              </div>
            )}
            
            {loadingDetails && (
              <div className="flex items-center gap-2 text-xs text-slate-500">
                <Loader2 className="h-4 w-4 animate-spin text-blue-600" />
                <span>Loading patient profile...</span>
              </div>
            )}
          </div>
        </div>

          {/* Card 2: Stay & Bed Details */}
          <div className="bg-white rounded-2xl border border-slate-200 overflow-hidden shadow-sm">
            <div className="bg-slate-50 px-6 py-4 border-b border-slate-200 flex items-center gap-2">
              <Bed className="h-5 w-5 text-emerald-600" />
              <h3 className="font-bold text-slate-800 text-sm">Stay Details</h3>
            </div>

            <div className="p-6 space-y-4">
              <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
              
              <div>
                <label className="block text-xs font-bold text-slate-700 mb-1.5">Department *</label>
                <select
                  name="department"
                  value={form.department}
                  onChange={handleDepartmentChange}
                  className="w-full px-3 py-2 border border-slate-200 rounded-lg text-sm bg-white focus:outline-none focus:ring-2 focus:ring-blue-100 focus:border-blue-500"
                  required
                >
                  <option value="" disabled>Select department...</option>
                  {Object.keys(HOSPITAL_STRUCTURE).map(d => (
                    <option key={d} value={d}>{d}</option>
                  ))}
                </select>
              </div>

              <div>
                <label className="block text-xs font-bold text-slate-700 mb-1.5">Room Number *</label>
                <select
                  name="roomNumber"
                  value={form.roomNumber}
                  onChange={handleRoomChange}
                  className="w-full px-3 py-2 border border-slate-200 rounded-lg text-sm bg-white focus:outline-none focus:ring-2 focus:ring-blue-100 focus:border-blue-500 disabled:bg-slate-50 disabled:cursor-not-allowed"
                  disabled={!form.department}
                  required
                >
                  <option value="" disabled>
                    {form.department ? 'Select room...' : 'Select department first'}
                  </option>
                  {rooms.map(r => (
                    <option key={r} value={r}>{r}</option>
                  ))}
                </select>
              </div>

              <div>
                <label className="block text-xs font-bold text-slate-700 mb-1.5">Bed Number *</label>
                <select
                  name="bedNumber"
                  value={form.bedNumber}
                  onChange={handleChange}
                  className="w-full px-3 py-2 border border-slate-200 rounded-lg text-sm bg-white focus:outline-none focus:ring-2 focus:ring-blue-100 focus:border-blue-500 disabled:bg-slate-50 disabled:cursor-not-allowed"
                  disabled={!form.roomNumber}
                  required
                >
                  <option value="" disabled>
                    {form.roomNumber ? 'Select bed...' : 'Select room first'}
                  </option>
                  {beds.map(b => (
                    <option key={b} value={b}>{b}</option>
                  ))}
                </select>
              </div>

              <div>
                <label className="block text-xs font-bold text-slate-700 mb-1.5">Stay Type *</label>
                <select
                  name="stayType"
                  value={form.stayType}
                  onChange={handleChange}
                  className="w-full px-3 py-2 border border-slate-200 rounded-lg text-sm bg-white focus:outline-none focus:ring-2 focus:ring-blue-100 focus:border-blue-500"
                  required
                >
                  <option value="0">General Admission</option>
                  <option value="1">Surgery</option>
                  <option value="2">Observation</option>
                  <option value="3">Emergency</option>
                </select>
              </div>

              <div>
                <label className="block text-xs font-bold text-slate-700 mb-1.5">Start Date *</label>
                <input
                  type="datetime-local"
                  name="startDate"
                  value={form.startDate}
                  onChange={handleChange}
                  className="w-full px-3 py-2 border border-slate-200 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-blue-100 focus:border-blue-500"
                  required
                />
              </div>

              <div>
                <label className="block text-xs font-bold text-slate-700 mb-1.5">End Date (Expected)</label>
                <input
                  type="datetime-local"
                  name="endDate"
                  value={form.endDate}
                  onChange={handleChange}
                  className="w-full px-3 py-2 border border-slate-200 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-blue-100 focus:border-blue-500"
                />
              </div>

              <div>
                <label className="block text-xs font-bold text-slate-700 mb-1.5">Estimated Duration</label>
                <div className="w-full px-3 py-2 bg-slate-50 border border-slate-200 rounded-lg text-sm font-semibold text-slate-600">
                  {calculateDuration()}
                </div>
              </div>

            </div>

            <div className="pt-2">
              <label className="block text-xs font-bold text-slate-700 mb-1.5">Notes</label>
              <textarea
                name="notes"
                value={form.notes}
                onChange={handleChange}
                placeholder="Write stay admission description, symptoms, doctor directives or room requirements..."
                className="w-full px-3 py-2 border border-slate-200 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-blue-100 focus:border-blue-500 h-24 resize-none"
              />
            </div>
          </div>
        </div>

          {/* Form Actions */}
          <div className="flex justify-end gap-3 pt-3">
            <button
              type="button"
              onClick={() => navigate('/management/stays')}
              className="px-5 py-2.5 bg-white border border-slate-300 text-slate-700 text-sm font-semibold rounded-xl hover:bg-slate-50 transition-colors shadow-sm"
            >
              Cancel
            </button>
            <button
              type="submit"
              disabled={isSubmitting}
              className="px-6 py-2.5 bg-blue-600 text-white text-sm font-bold rounded-xl hover:bg-blue-700 transition-colors shadow-md hover:shadow-lg disabled:bg-blue-300 disabled:cursor-not-allowed flex items-center gap-2"
            >
              {isSubmitting ? (
                <>
                  <Loader2 className="h-4 w-4 animate-spin" />
                  <span>Saving...</span>
                </>
              ) : (
                <>
                  <Save className="h-4 w-4" />
                  <span>Save Stay Admission</span>
                </>
              )}
            </button>
          </div>
        </form>
      )}
    </div>
  );
};
