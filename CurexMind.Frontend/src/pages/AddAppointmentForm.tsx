import React, { useEffect, useState } from 'react';
import { useNavigate, useLocation } from 'react-router-dom';
import Swal from 'sweetalert2';
import { apiClient } from '../api/apiClient';

interface Patient {
  patientId: string;
  fullName: string;
  phoneNumber: string;
}

interface Doctor {
  id: string;
  fullName: string;
  specialization: string;
  clinicName: string;
}

interface Clinic {
  id: string;
  name: string;
}

const CLINICS: Clinic[] = [
  { id: '33333333-3333-3333-3333-333333333333', name: 'Main Clinic' },
  { id: '33333333-3333-3333-3333-333333333002', name: 'Downtown Clinic' },
  { id: '33333333-3333-3333-3333-333333333003', name: 'Uptown Clinic' },
  { id: '33333333-3333-3333-3333-333333333004', name: 'Eastside Clinic' },
  { id: '33333333-3333-3333-3333-333333333005', name: 'West End Clinic' },
  { id: '33333333-3333-3333-3333-333333333006', name: 'Riverside Clinic' },
  { id: '33333333-3333-3333-3333-333333333007', name: 'Greenfield Clinic' },
];

export const AddAppointmentForm: React.FC = () => {
  const navigate = useNavigate();
  const location = useLocation();

  const [patients, setPatients] = useState<Patient[]>([]);
  const [allDoctors, setAllDoctors] = useState<Doctor[]>([]);
  const [filteredDoctors, setFilteredDoctors] = useState<Doctor[]>([]);
  const [selectedClinicId, setSelectedClinicId] = useState('');
  const [isSubmitting, setIsSubmitting] = useState(false);

  const [form, setForm] = useState({
    patientId: '',
    doctorId: '',
    appointmentType: '1',
    status: '1',
    date: '',
    time: '',
    durationMinutes: '30',
    notes: '',
  });

  useEffect(() => {
    apiClient.get('/api/Patients?page=1&pageSize=10').then((res: any) => {
      setPatients(res.data?.data ?? res.data?.items ?? (Array.isArray(res.data) ? res.data : []));
    }).catch(() => {});

    apiClient.get('/api/Doctors?page=1&pageSize=10').then((res: any) => {
      setAllDoctors(res.data?.data ?? res.data?.items ?? (Array.isArray(res.data) ? res.data : []));
    }).catch(() => {});
  }, []);

  useEffect(() => {
    if (location.state?.patientId) {
      setForm(f => ({ ...f, patientId: location.state.patientId }));
    }
  }, [location.state]);

  const handleClinicChange = (clinicId: string) => {
    setSelectedClinicId(clinicId);
    const clinic = CLINICS.find((c) => c.id === clinicId);
    const clinicName = clinic?.name ?? '';
    setFilteredDoctors(
      allDoctors.filter((d) => d.clinicName?.trim().toLowerCase() === clinicName.trim().toLowerCase())
    );
    setForm((f) => ({ ...f, doctorId: '' }));
  };

  const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement | HTMLTextAreaElement>) => {
    setForm((f) => ({ ...f, [e.target.name]: e.target.value }));
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setIsSubmitting(true);

    const dateISO = form.date ? new Date(form.date).toISOString() : undefined;
    const payload = {
      patientId: form.patientId,
      doctorId: form.doctorId,
      clinicId: selectedClinicId,
      appointmentType: parseInt(form.appointmentType),
      status: parseInt(form.status),
      date: dateISO,
      time: form.time,
      durationMinutes: parseInt(form.durationMinutes),
      notes: form.notes,
    };

    try {
      await apiClient.post('/api/Appointments', payload);
      await Swal.fire({ title: 'Success!', text: 'Appointment scheduled successfully.', icon: 'success', confirmButtonText: 'Go to List' });
      navigate('/management/appointments');
    } catch (err: any) {
      const msg = err.response?.data?.errors?.[0]?.message ?? err.response?.data?.title ?? 'Failed to schedule appointment.';
      Swal.fire({ title: 'Error!', text: msg, icon: 'error' });
    } finally {
      setIsSubmitting(false);
    }
  };

  return (
    <div className="space-y-6">
      {/* Header */}
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-2xl font-bold text-slate-800">Add New Appointment</h1>
          <p className="text-sm text-slate-500 mt-1">Complete the form below to schedule an appointment</p>
        </div>
        <button onClick={() => navigate('/management/appointments')} className="flex items-center gap-2 px-4 py-2 border border-slate-200 rounded-lg text-slate-600 text-sm font-medium hover:bg-slate-50 transition-colors">
          <i className="fa-solid fa-arrow-left text-xs"></i> Back
        </button>
      </div>

      <form onSubmit={handleSubmit} className="space-y-6">
        {/* Patient & Doctor Info */}
        <div className="bg-white rounded-2xl border border-slate-200 shadow-sm overflow-hidden">
          <div className="px-6 py-4 bg-slate-50 border-b border-slate-200 font-semibold text-slate-700">
            <i className="fa-regular fa-user mr-2 text-blue-500"></i> Patient &amp; Doctor Info
          </div>
          <div className="p-6 md:p-8">
            <div className="grid grid-cols-1 md:grid-cols-2 gap-x-8 gap-y-5">
              <div className="flex flex-col gap-1.5">
                <label className="text-sm font-medium text-slate-700">Patient <span className="text-red-500">*</span></label>
                <div className="flex gap-2">
                  <select name="patientId" value={form.patientId} onChange={handleChange} required className="flex-1 px-3 py-2.5 border border-slate-200 rounded-lg text-sm text-slate-800 outline-none focus:border-blue-500 focus:ring-2 focus:ring-blue-100 transition bg-white">
                    <option value="" disabled>Search and select patient</option>
                    {patients.map((p) => (
                      <option key={p.patientId} value={p.patientId}>{p.fullName} ({p.phoneNumber})</option>
                    ))}
                  </select>
                  <button type="button" onClick={() => navigate('/management/patients/add')} title="Add new patient"
                    className="w-11 border border-dashed border-blue-400 text-blue-500 rounded-lg hover:bg-blue-50 transition-colors flex items-center justify-center text-sm">
                    <i className="fa-solid fa-plus"></i>
                  </button>
                </div>
              </div>

              <div className="flex flex-col gap-1.5">
                <label className="text-sm font-medium text-slate-700">Clinic <span className="text-red-500">*</span></label>
                <select value={selectedClinicId} onChange={(e) => handleClinicChange(e.target.value)} required className="px-3 py-2.5 border border-slate-200 rounded-lg text-sm text-slate-800 outline-none focus:border-blue-500 focus:ring-2 focus:ring-blue-100 transition bg-white">
                  <option value="">Select Clinic</option>
                  {CLINICS.map((c) => <option key={c.id} value={c.id}>{c.name}</option>)}
                </select>
              </div>

              <div className="flex flex-col gap-1.5 md:col-span-2">
                <label className="text-sm font-medium text-slate-700">Doctor <span className="text-red-500">*</span></label>
                <select name="doctorId" value={form.doctorId} onChange={handleChange} required disabled={!selectedClinicId}
                  className="px-3 py-2.5 border border-slate-200 rounded-lg text-sm text-slate-800 outline-none focus:border-blue-500 focus:ring-2 focus:ring-blue-100 transition bg-white disabled:opacity-50 disabled:cursor-not-allowed">
                  <option value="">{selectedClinicId ? 'Select Doctor' : 'Select Clinic First'}</option>
                  {filteredDoctors.map((d) => <option key={d.id} value={d.id}>{d.fullName} ({d.specialization})</option>)}
                  {selectedClinicId && filteredDoctors.length === 0 && <option disabled>No doctors in this clinic</option>}
                </select>
              </div>
            </div>
          </div>
        </div>

        {/* Appointment Details */}
        <div className="bg-white rounded-2xl border border-slate-200 shadow-sm overflow-hidden">
          <div className="px-6 py-4 bg-slate-50 border-b border-slate-200 font-semibold text-slate-700">
            <i className="fa-regular fa-calendar-check mr-2 text-blue-500"></i> Appointment Details
          </div>
          <div className="p-6 md:p-8">
            <div className="grid grid-cols-1 md:grid-cols-2 gap-x-8 gap-y-5">
              <div className="flex flex-col gap-1.5">
                <label className="text-sm font-medium text-slate-700">Appointment Type</label>
                <select name="appointmentType" value={form.appointmentType} onChange={handleChange} className="px-3 py-2.5 border border-slate-200 rounded-lg text-sm text-slate-800 outline-none focus:border-blue-500 focus:ring-2 focus:ring-blue-100 transition bg-white">
                  <option value="1">First Visit</option>
                  <option value="2">Follow Up</option>
                  <option value="3">Checkup</option>
                </select>
              </div>

              <div className="flex flex-col gap-1.5">
                <label className="text-sm font-medium text-slate-700">Status</label>
                <select name="status" value={form.status} onChange={handleChange} className="px-3 py-2.5 border border-slate-200 rounded-lg text-sm text-slate-800 outline-none focus:border-blue-500 focus:ring-2 focus:ring-blue-100 transition bg-white">
                  <option value="1">Confirmed</option>
                  <option value="0">Pending</option>
                  <option value="2">Cancelled</option>
                </select>
              </div>

              <div className="flex flex-col gap-1.5">
                <label className="text-sm font-medium text-slate-700">Date <span className="text-red-500">*</span></label>
                <input name="date" type="date" value={form.date} onChange={handleChange} required className="px-3 py-2.5 border border-slate-200 rounded-lg text-sm text-slate-800 outline-none focus:border-blue-500 focus:ring-2 focus:ring-blue-100 transition" />
              </div>

              <div className="flex flex-col gap-1.5">
                <label className="text-sm font-medium text-slate-700">Time <span className="text-red-500">*</span></label>
                <input name="time" type="time" value={form.time} onChange={handleChange} required className="px-3 py-2.5 border border-slate-200 rounded-lg text-sm text-slate-800 outline-none focus:border-blue-500 focus:ring-2 focus:ring-blue-100 transition" />
              </div>

              <div className="flex flex-col gap-1.5 md:col-span-2">
                <label className="text-sm font-medium text-slate-700">Duration</label>
                <select name="durationMinutes" value={form.durationMinutes} onChange={handleChange} className="px-3 py-2.5 border border-slate-200 rounded-lg text-sm text-slate-800 outline-none focus:border-blue-500 focus:ring-2 focus:ring-blue-100 transition bg-white">
                  <option value="15">15 Minutes</option>
                  <option value="30">30 Minutes</option>
                  <option value="45">45 Minutes</option>
                  <option value="60">1 Hour</option>
                </select>
              </div>

              <div className="flex flex-col gap-1.5 md:col-span-2">
                <label className="text-sm font-medium text-slate-700">Notes</label>
                <textarea name="notes" value={form.notes} onChange={handleChange} rows={3} placeholder="Any additional notes about this appointment..." className="px-3 py-2.5 border border-slate-200 rounded-lg text-sm text-slate-800 outline-none focus:border-blue-500 focus:ring-2 focus:ring-blue-100 transition resize-y" />
              </div>
            </div>
          </div>
        </div>

        {/* Actions */}
        <div className="flex justify-end gap-3 pb-8">
          <button type="button" onClick={() => navigate('/management/appointments')} className="px-6 py-2.5 border border-slate-200 rounded-lg text-slate-700 text-sm font-medium hover:bg-slate-50 transition-colors">
            Cancel
          </button>
          <button type="submit" disabled={isSubmitting} className="px-6 py-2.5 bg-blue-600 hover:bg-blue-700 disabled:bg-blue-300 text-white rounded-lg text-sm font-semibold transition-colors flex items-center gap-2">
            {isSubmitting ? <><i className="fa-solid fa-circle-notch fa-spin"></i> Saving...</> : <><i className="fa-regular fa-calendar-check"></i> Save Appointment</>}
          </button>
        </div>
      </form>
    </div>
  );
};
