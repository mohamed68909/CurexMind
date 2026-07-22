import React, { useEffect, useState } from 'react';
import { useNavigate, useLocation } from 'react-router-dom';
import Swal from 'sweetalert2';
import { apiClient } from '../api/apiClient';
import { 
  FileText, 
  User, 
  Stethoscope, 
  DollarSign, 
  CreditCard, 
  ArrowLeft, 
  Save, 
  Loader2 
} from 'lucide-react';

interface Patient {
  patientId: string;
  fullName: string;
  phoneNumber: string;
}

interface Doctor {
  id: string;
  fullName: string;
  clinicName: string;
}

const CLINICS = [
  'Main Clinic',
  'Downtown Clinic',
  'Uptown Clinic',
  'Eastside Clinic',
  'West End Clinic',
  'Riverside Clinic',
  'Greenfield Clinic'
];

const SERVICE_TYPES = [
  { id: '44444444-4444-4444-4444-444444444444', name: 'Consultation' },
  { id: '55555555-5555-5555-5555-555555555555', name: 'Installation' },
  { id: '66666666-6666-6666-6666-666666666666', name: 'Maintenance' },
  { id: '77777777-7777-7777-7777-777777777777', name: 'Emergency Repair' }
];

export const AddInvoiceForm: React.FC = () => {
  const navigate = useNavigate();
  const location = useLocation();
  const [patients, setPatients] = useState<Patient[]>([]);
  const [allDoctors, setAllDoctors] = useState<Doctor[]>([]);
  const [filteredDoctors, setFilteredDoctors] = useState<Doctor[]>([]);
  
  const [loadingPatients, setLoadingPatients] = useState(true);
  const [isSubmitting, setIsSubmitting] = useState(false);

  const [form, setForm] = useState({
    patientId: '',
    clinicName: '',
    doctorId: '',
    serviceTypeId: '',
    visitDate: '',
    totalAmountEGP: '',
    discountEGP: '0',
    paymentMethod: 'Cash',
    paymentStatus: 'Paid',
    amountPaidEGP: '',
    notes: ''
  });

  // Initialize visit date to now
  useEffect(() => {
    const now = new Date();
    now.setMinutes(now.getMinutes() - now.getTimezoneOffset());
    setForm(prev => ({ ...prev, visitDate: now.toISOString().slice(0, 16) }));
  }, []);

  useEffect(() => {
    if (location.state?.patientId) {
      setForm(prev => ({ ...prev, patientId: location.state.patientId }));
    }
  }, [location.state]);

  // Fetch Patients and Doctors
  useEffect(() => {
    const loadData = async () => {
      try {
        setLoadingPatients(true);
        // Get Patients
        const patRes = await apiClient.get('/api/Patients?page=1&pageSize=10');
        const patData = patRes.data?.data ?? patRes.data?.items ?? (Array.isArray(patRes.data) ? patRes.data : []);
        setPatients(patData);

        // Get Doctors
        const docRes = await apiClient.get('/api/Doctors?page=1&pageSize=10');
        const docData = docRes.data?.data ?? docRes.data?.items ?? (Array.isArray(docRes.data) ? docRes.data : []);
        setAllDoctors(docData);
      } catch (err) {
        console.error('Failed to load form lookup data:', err);
        Swal.fire({
          title: 'خطأ في التحميل',
          text: 'تعذر تحميل بيانات المرضى أو الأطباء من الخادم.',
          icon: 'error',
          confirmButtonColor: '#3b82f6'
        });
      } finally {
        setLoadingPatients(false);
      }
    };
    loadData();
  }, []);

  // Filter doctors when clinic changes
  useEffect(() => {
    if (form.clinicName) {
      const filtered = allDoctors.filter(
        doc => doc.clinicName?.trim().toLowerCase() === form.clinicName.trim().toLowerCase()
      );
      setFilteredDoctors(filtered);
      // Reset doctorId if not in filtered list
      if (!filtered.some(d => d.id === form.doctorId)) {
        setForm(prev => ({ ...prev, doctorId: '' }));
      }
    } else {
      setFilteredDoctors([]);
      setForm(prev => ({ ...prev, doctorId: '' }));
    }
  }, [form.clinicName, allDoctors]);

  // Handle Payment Status and Amounts Autofill
  const finalAmount = Math.max(0, (parseFloat(form.totalAmountEGP) || 0) - (parseFloat(form.discountEGP) || 0));

  useEffect(() => {
    if (form.paymentStatus === 'Paid') {
      setForm(prev => ({ ...prev, amountPaidEGP: finalAmount.toString() }));
    } else if (form.paymentStatus === 'Unpaid') {
      setForm(prev => ({ ...prev, amountPaidEGP: '0' }));
    }
  }, [form.paymentStatus, form.totalAmountEGP, form.discountEGP]);

  const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement | HTMLTextAreaElement>) => {
    const { name, value } = e.target;
    setForm(prev => ({ ...prev, [name]: value }));
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    if (!form.patientId || !form.doctorId || !form.clinicName || !form.serviceTypeId) {
      Swal.fire('تنبيه', 'يرجى ملء جميع الحقول المطلوبة.', 'warning');
      return;
    }

    const paidVal = parseFloat(form.amountPaidEGP) || 0;
    if (paidVal > finalAmount) {
      Swal.fire('خطأ', 'القيمة المدفوعة لا يمكن أن تتخطى القيمة الإجمالية للفاتورة.', 'warning');
      return;
    }

    try {
      setIsSubmitting(true);
      const payload = {
        patientId: form.patientId,
        doctorId: form.doctorId,
        serviceDetails: {
          serviceTypeId: form.serviceTypeId,
          visitDate: form.visitDate ? new Date(form.visitDate).toISOString() : new Date().toISOString(),
          clinicName: form.clinicName
        },
        amountDetails: {
          totalAmountEGP: parseFloat(form.totalAmountEGP) || 0,
          discountEGP: parseFloat(form.discountEGP) || 0,
          finalAmountEGP: finalAmount
        },
        paymentInformation: {
          paymentMethod: form.paymentMethod,
          paymentStatus: form.paymentStatus,
          amountPaidEGP: paidVal
        },
        notes: form.notes || ''
      };

      await apiClient.post('/api/invoices', payload);

      Swal.fire({
        title: 'تم بنجاح!',
        text: 'تم إصدار الفاتورة للمريض بنجاح.',
        icon: 'success',
        confirmButtonColor: '#3b82f6',
        confirmButtonText: 'الذهاب لقائمة الفواتير'
      }).then(() => {
        navigate('/management/invoices');
      });

    } catch (err: any) {
      console.error('Failed to save invoice:', err);
      const serverMsg = err.response?.data?.errors?.[0]?.message || err.response?.data?.title || 'تعذر إصدار الفاتورة.';
      Swal.fire('فشلت العملية', serverMsg, 'error');
    } finally {
      setIsSubmitting(false);
    }
  };

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
          <h1 className="text-2xl font-bold text-slate-900 font-cairo">Create New Invoice</h1>
          <p className="text-xs text-slate-500 font-cairo">Issue a new medical service fee or billing record</p>
        </div>
      </div>

      {loadingPatients ? (
        <div className="flex flex-col items-center justify-center min-h-[300px] gap-3 bg-white border border-slate-200 rounded-2xl">
          <Loader2 className="h-8 w-8 text-blue-600 animate-spin" />
          <p className="text-slate-500 text-xs font-semibold">Loading patient and clinic list...</p>
        </div>
      ) : (
        <form onSubmit={handleSubmit} className="space-y-6 max-w-5xl">
          <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
            
            {/* Card 1: Patient & Doctor */}
            <div className="bg-white rounded-2xl border border-slate-200 overflow-hidden shadow-sm">
              <div className="bg-slate-50 px-6 py-4 border-b border-slate-200 flex items-center gap-2">
                <User className="h-5 w-5 text-blue-600" />
                <h3 className="font-bold text-slate-800 text-sm">Patient & Clinic</h3>
              </div>

              <div className="p-6 space-y-3">
                <div>
                  <label className="block text-xs font-bold text-slate-700 mb-1.5">Patient Name *</label>
                  <select
                    name="patientId"
                    value={form.patientId}
                    onChange={handleChange}
                    className="w-full px-3 py-2 border border-slate-200 rounded-lg text-sm bg-white focus:outline-none focus:ring-2 focus:ring-blue-100 focus:border-blue-500"
                    required
                  >
                    <option value="" disabled>Select Patient</option>
                    {patients.map(p => (
                      <option key={p.patientId} value={p.patientId}>
                        {p.fullName} ({p.phoneNumber})
                      </option>
                    ))}
                  </select>
                </div>

                <div>
                  <label className="block text-xs font-bold text-slate-700 mb-1.5">Clinic *</label>
                  <select
                    name="clinicName"
                    value={form.clinicName}
                    onChange={handleChange}
                    className="w-full px-3 py-2 border border-slate-200 rounded-lg text-sm bg-white focus:outline-none focus:ring-2 focus:ring-blue-100 focus:border-blue-500"
                    required
                  >
                    <option value="" disabled>Select Clinic</option>
                    {CLINICS.map(c => (
                      <option key={c} value={c}>{c}</option>
                    ))}
                  </select>
                </div>

                <div>
                  <label className="block text-xs font-bold text-slate-700 mb-1.5">Doctor *</label>
                  <select
                    name="doctorId"
                    value={form.doctorId}
                    onChange={handleChange}
                    className="w-full px-3 py-2 border border-slate-200 rounded-lg text-sm bg-white focus:outline-none focus:ring-2 focus:ring-blue-100 focus:border-blue-500 disabled:bg-slate-50 disabled:cursor-not-allowed"
                    disabled={!form.clinicName}
                    required
                  >
                    <option value="" disabled>
                      {form.clinicName ? 'Select Doctor' : 'Select Clinic First'}
                    </option>
                    {filteredDoctors.map(d => (
                      <option key={d.id} value={d.id}>{d.fullName}</option>
                    ))}
                  </select>
                </div>
              </div>
            </div>

            {/* Card 2: Service Details */}
            <div className="bg-white rounded-2xl border border-slate-200 overflow-hidden shadow-sm">
              <div className="bg-slate-50 px-6 py-4 border-b border-slate-200 flex items-center gap-2">
                <Stethoscope className="h-5 w-5 text-emerald-600" />
                <h3 className="font-bold text-slate-800 text-sm">Service Details</h3>
              </div>

              <div className="p-6 space-y-3">
                <div>
                  <label className="block text-xs font-bold text-slate-700 mb-1.5">Service Type *</label>
                  <select
                    name="serviceTypeId"
                    value={form.serviceTypeId}
                    onChange={handleChange}
                    className="w-full px-3 py-2 border border-slate-200 rounded-lg text-sm bg-white focus:outline-none focus:ring-2 focus:ring-blue-100 focus:border-blue-500"
                    required
                  >
                    <option value="" disabled>Select Service Type</option>
                    {SERVICE_TYPES.map(s => (
                      <option key={s.id} value={s.id}>{s.name}</option>
                    ))}
                  </select>
                </div>

                <div>
                  <label className="block text-xs font-bold text-slate-700 mb-1.5">Date of Service *</label>
                  <input
                    type="datetime-local"
                    name="visitDate"
                    value={form.visitDate}
                    onChange={handleChange}
                    className="w-full px-3 py-2 border border-slate-200 rounded-lg text-sm bg-white focus:outline-none focus:ring-2 focus:ring-blue-100 focus:border-blue-500"
                    required
                  />
                </div>
              </div>
            </div>

            {/* Card 3: Financials */}
            <div className="bg-white rounded-2xl border border-slate-200 overflow-hidden shadow-sm">
              <div className="bg-slate-50 px-6 py-4 border-b border-slate-200 flex items-center gap-2">
                <DollarSign className="h-5 w-5 text-amber-600" />
                <h3 className="font-bold text-slate-800 text-sm">Financials</h3>
              </div>

              <div className="p-6 space-y-3">
                <div>
                  <label className="block text-xs font-bold text-slate-700 mb-1.5">Total Amount (EGP) *</label>
                  <input
                    type="number"
                    name="totalAmountEGP"
                    value={form.totalAmountEGP}
                    onChange={handleChange}
                    placeholder="0.00"
                    className="w-full px-3 py-2 border border-slate-200 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-blue-100 focus:border-blue-500"
                    min="0"
                    step="0.01"
                    required
                  />
                </div>

                <div>
                  <label className="block text-xs font-bold text-slate-700 mb-1.5">Discount (EGP)</label>
                  <input
                    type="number"
                    name="discountEGP"
                    value={form.discountEGP}
                    onChange={handleChange}
                    placeholder="0.00"
                    className="w-full px-3 py-2 border border-slate-200 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-blue-100 focus:border-blue-500"
                    min="0"
                    step="0.01"
                  />
                </div>

                <div>
                  <label className="block text-xs font-bold text-slate-700 mb-1.5">Final Amount (EGP)</label>
                  <div className="w-full px-3 py-2 bg-blue-50 border border-blue-100 rounded-lg text-sm font-extrabold text-blue-700">
                    {finalAmount.toFixed(2)} EGP
                  </div>
                </div>
              </div>
            </div>

            {/* Card 4: Payment Details */}
            <div className="bg-white rounded-2xl border border-slate-200 overflow-hidden shadow-sm">
              <div className="bg-slate-50 px-6 py-4 border-b border-slate-200 flex items-center gap-2">
                <CreditCard className="h-5 w-5 text-rose-600" />
                <h3 className="font-bold text-slate-800 text-sm">Payment Details</h3>
              </div>

              <div className="p-6 space-y-3">
                <div>
                  <label className="block text-xs font-bold text-slate-700 mb-1.5">Payment Method *</label>
                  <div className="flex gap-4">
                    {['Cash', 'Credit Card', 'Wallet'].map(m => (
                      <label key={m} className="flex items-center gap-2 text-sm text-slate-700 font-semibold cursor-pointer">
                        <input
                          type="radio"
                          name="paymentMethod"
                          value={m}
                          checked={form.paymentMethod === m}
                          onChange={handleChange}
                          className="accent-blue-600 h-4 w-4"
                        />
                        <span>{m}</span>
                      </label>
                    ))}
                  </div>
                </div>

                <div>
                  <label className="block text-xs font-bold text-slate-700 mb-1.5">Payment Status *</label>
                  <div className="flex gap-4">
                    {['Paid', 'Unpaid', 'Partial'].map(s => (
                      <label key={s} className="flex items-center gap-2 text-sm text-slate-700 font-semibold cursor-pointer">
                        <input
                          type="radio"
                          name="paymentStatus"
                          value={s}
                          checked={form.paymentStatus === s}
                          onChange={handleChange}
                          className="accent-blue-600 h-4 w-4"
                        />
                        <span>{s}</span>
                      </label>
                    ))}
                  </div>
                </div>

                <div>
                  <label className="block text-xs font-bold text-slate-700 mb-1.5">Amount Paid (EGP)</label>
                  <input
                    type="number"
                    name="amountPaidEGP"
                    value={form.amountPaidEGP}
                    onChange={handleChange}
                    placeholder="0.00"
                    className="w-full px-3 py-2 border border-slate-200 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-blue-100 focus:border-blue-500 disabled:bg-slate-50 disabled:text-slate-500 disabled:cursor-not-allowed"
                    disabled={form.paymentStatus !== 'Partial'}
                    min="0"
                    step="0.01"
                    required
                  />
                </div>
              </div>
            </div>

            {/* Card 5: Notes (Full Width) */}
            <div className="bg-white rounded-2xl border border-slate-200 overflow-hidden shadow-sm md:col-span-2">
              <div className="bg-slate-50 px-6 py-4 border-b border-slate-200 flex items-center gap-2">
                <FileText className="h-5 w-5 text-slate-600" />
                <h3 className="font-bold text-slate-800 text-sm">Invoice Notes</h3>
              </div>
              <div className="p-6">
                <label className="block text-xs font-bold text-slate-700 mb-1.5">Notes</label>
                <textarea
                  name="notes"
                  value={form.notes}
                  onChange={handleChange}
                  placeholder="Enter any diagnostic, medical codes, billing comments or instructions..."
                  className="w-full px-3 py-2 border border-slate-200 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-blue-100 focus:border-blue-500 h-24 resize-none"
                />
              </div>
            </div>

          </div>

          {/* Form Actions */}
          <div className="flex justify-end gap-3 pt-3">
            <button
              type="button"
              onClick={() => navigate('/management/invoices')}
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
                  <span>Issuing...</span>
                </>
              ) : (
                <>
                  <Save className="h-4 w-4" />
                  <span>Issue Invoice</span>
                </>
              )}
            </button>
          </div>
        </form>
      )}
    </div>
  );
};
