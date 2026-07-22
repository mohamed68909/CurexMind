import React, { useEffect, useRef, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import Swal from 'sweetalert2';
import { apiClient } from '../api/apiClient';

interface Clinic {
  id: string;
  name: string;
}

interface Doctor {
  id: string;
  fullName: string;
  specialization: string;
  clinicName: string;
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

export const AddPatientForm: React.FC = () => {
  const navigate = useNavigate();
  const fileInputRef = useRef<HTMLInputElement>(null);

  const [imagePreview, setImagePreview] = useState<string | null>(null);
  const [imageFile, setImageFile] = useState<File | null>(null);

  const [allDoctors, setAllDoctors] = useState<Doctor[]>([]);
  const [filteredDoctors, setFilteredDoctors] = useState<Doctor[]>([]);
  const [selectedClinicId, setSelectedClinicId] = useState('');

  const [isSubmitting, setIsSubmitting] = useState(false);

  const [form, setForm] = useState({
    FullName: '',
    Gender: '',
    SocialStatus: '',
    DateOfBirth: '',
    PhoneNumber: '',
    Email: '',
    NationalId: '',
    Address: '',
    Notes: '',
    // Initial booking
    DoctorId: '',
    AppointmentType: '1',
    AppointmentDate: '',
    BookingNotes: '',
  });

  useEffect(() => {
    apiClient.get('/api/Doctors?page=1&pageSize=10').then((res: any) => {
      const data = res.data?.data ?? res.data?.items ?? (Array.isArray(res.data) ? res.data : []);
      setAllDoctors(data);
    }).catch(() => {/* silent */});
  }, []);

  const handleClinicChange = (clinicId: string) => {
    setSelectedClinicId(clinicId);
    const clinic = CLINICS.find((c) => c.id === clinicId);
    const clinicName = clinic?.name ?? '';
    setFilteredDoctors(
      allDoctors.filter((d) => d.clinicName?.trim().toLowerCase() === clinicName.trim().toLowerCase())
    );
    setForm((f) => ({ ...f, DoctorId: '' }));
  };

  const handleImageChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const file = e.target.files?.[0];
    if (file) {
      setImageFile(file);
      const reader = new FileReader();
      reader.onload = (ev) => setImagePreview(ev.target?.result as string);
      reader.readAsDataURL(file);
    }
  };

  const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement | HTMLTextAreaElement>) => {
    setForm((f) => ({ ...f, [e.target.name]: e.target.value }));
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setIsSubmitting(true);

    const formData = new FormData();
    formData.append('FullName', form.FullName);
    formData.append('Gender', form.Gender);
    if (form.SocialStatus) formData.append('SocialStatus', form.SocialStatus);
    formData.append('DateOfBirth', new Date(form.DateOfBirth).toISOString());
    formData.append('PhoneNumber', form.PhoneNumber);
    formData.append('Email', form.Email);
    if (form.NationalId) formData.append('NationalId', form.NationalId);
    if (form.Address) formData.append('Address', form.Address);
    if (form.Notes) formData.append('Notes', form.Notes);
    // Auto-generate username
    formData.append('UserName', form.FullName.replace(/\s+/g, '') + Math.floor(Math.random() * 1000));
    // Initial booking
    if (form.DoctorId) formData.append('InitialBooking.DoctorId', form.DoctorId);
    if (selectedClinicId) formData.append('InitialBooking.ClinicId', selectedClinicId);
    formData.append('InitialBooking.AppointmentType', form.AppointmentType);
    if (form.AppointmentDate) formData.append('InitialBooking.AppointmentDate', new Date(form.AppointmentDate).toISOString());
    if (form.BookingNotes) formData.append('InitialBooking.Notes', form.BookingNotes);
    if (imageFile) formData.append('ProfileImageUrl', imageFile);

    try {
      await apiClient.post('/api/Patients', formData, {
        headers: { 'Content-Type': 'multipart/form-data' },
      });
      await Swal.fire({ title: 'Success!', text: 'Patient registered successfully.', icon: 'success', confirmButtonText: 'Go to List' });
      navigate('/management/patients');
    } catch (err: any) {
      const errorData = err.response?.data;
      const msg = errorData?.errors?.[0]?.message ?? errorData?.title ?? 'An unexpected error occurred.';
      Swal.fire({ title: 'Warning!', text: msg, icon: 'warning' });
    } finally {
      setIsSubmitting(false);
    }
  };

  return (
    <div className="space-y-6">
      {/* Header */}
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-2xl font-bold text-slate-800">Add New Patient</h1>
          <p className="text-sm text-slate-500 mt-1">Complete the form below to register a new patient</p>
        </div>
        <button onClick={() => navigate('/management/patients')} className="flex items-center gap-2 px-4 py-2 border border-slate-200 rounded-lg text-slate-600 text-sm font-medium hover:bg-slate-50 transition-colors">
          <i className="fa-solid fa-arrow-left text-xs"></i> Back to Patients
        </button>
      </div>

      <form onSubmit={handleSubmit} className="space-y-6">
        {/* Patient Information */}
        <div className="bg-white rounded-2xl border border-slate-200 shadow-sm overflow-hidden">
          <div className="px-6 py-4 bg-slate-50 border-b border-slate-200 font-semibold text-slate-700">
            <i className="fa-regular fa-user mr-2 text-blue-500"></i> Patient Information
          </div>
          <div className="p-6 md:p-8">
            {/* Avatar Upload */}
            <div className="flex justify-center mb-8">
              <div className="relative">
                <div
                  onClick={() => fileInputRef.current?.click()}
                  className="w-24 h-24 rounded-full border-2 border-dashed border-slate-300 bg-slate-50 flex items-center justify-center cursor-pointer overflow-hidden hover:border-blue-400 transition-colors"
                  style={imagePreview ? { backgroundImage: `url(${imagePreview})`, backgroundSize: 'cover', backgroundPosition: 'center' } : {}}
                >
                  {!imagePreview && <i className="fa-regular fa-user text-3xl text-slate-300"></i>}
                </div>
                <button
                  type="button"
                  onClick={() => fileInputRef.current?.click()}
                  className="absolute bottom-0 right-0 w-7 h-7 rounded-full bg-blue-600 text-white flex items-center justify-center border-2 border-white shadow hover:bg-blue-700 transition-colors"
                >
                  <i className="fa-solid fa-pen text-xs"></i>
                </button>
                <input ref={fileInputRef} type="file" accept="image/*" className="hidden" onChange={handleImageChange} />
              </div>
            </div>

            <div className="grid grid-cols-1 md:grid-cols-2 gap-x-8 gap-y-5">
              <div className="flex flex-col gap-1.5">
                <label className="text-sm font-medium text-slate-700">Patient Name <span className="text-red-500">*</span></label>
                <input name="FullName" value={form.FullName} onChange={handleChange} required placeholder="Enter full name" className="px-3 py-2.5 border border-slate-200 rounded-lg text-sm text-slate-800 outline-none focus:border-blue-500 focus:ring-2 focus:ring-blue-100 transition" />
              </div>

              <div className="flex flex-col gap-1.5">
                <label className="text-sm font-medium text-slate-700">Gender <span className="text-red-500">*</span></label>
                <select name="Gender" value={form.Gender} onChange={handleChange} required className="px-3 py-2.5 border border-slate-200 rounded-lg text-sm text-slate-800 outline-none focus:border-blue-500 focus:ring-2 focus:ring-blue-100 transition bg-white">
                  <option value="" disabled>Select gender</option>
                  <option value="1">Male</option>
                  <option value="2">Female</option>
                </select>
              </div>

              <div className="flex flex-col gap-1.5">
                <label className="text-sm font-medium text-slate-700">Social Status</label>
                <select name="SocialStatus" value={form.SocialStatus} onChange={handleChange} className="px-3 py-2.5 border border-slate-200 rounded-lg text-sm text-slate-800 outline-none focus:border-blue-500 focus:ring-2 focus:ring-blue-100 transition bg-white">
                  <option value="">Select status</option>
                  <option value="1">Single</option>
                  <option value="2">Married</option>
                </select>
              </div>

              <div className="flex flex-col gap-1.5">
                <label className="text-sm font-medium text-slate-700">Date of Birth <span className="text-red-500">*</span></label>
                <input name="DateOfBirth" type="date" value={form.DateOfBirth} onChange={handleChange} required className="px-3 py-2.5 border border-slate-200 rounded-lg text-sm text-slate-800 outline-none focus:border-blue-500 focus:ring-2 focus:ring-blue-100 transition" />
              </div>

              <div className="flex flex-col gap-1.5">
                <label className="text-sm font-medium text-slate-700">Phone Number <span className="text-red-500">*</span></label>
                <input name="PhoneNumber" type="tel" value={form.PhoneNumber} onChange={handleChange} required placeholder="e.g. 01012345678" className="px-3 py-2.5 border border-slate-200 rounded-lg text-sm text-slate-800 outline-none focus:border-blue-500 focus:ring-2 focus:ring-blue-100 transition" />
              </div>

              <div className="flex flex-col gap-1.5">
                <label className="text-sm font-medium text-slate-700">Email Address <span className="text-red-500">*</span></label>
                <input name="Email" type="email" value={form.Email} onChange={handleChange} required placeholder="example@email.com" className="px-3 py-2.5 border border-slate-200 rounded-lg text-sm text-slate-800 outline-none focus:border-blue-500 focus:ring-2 focus:ring-blue-100 transition" />
              </div>

              <div className="flex flex-col gap-1.5">
                <label className="text-sm font-medium text-slate-700">National ID</label>
                <input name="NationalId" value={form.NationalId} onChange={handleChange} placeholder="Enter national ID" className="px-3 py-2.5 border border-slate-200 rounded-lg text-sm text-slate-800 outline-none focus:border-blue-500 focus:ring-2 focus:ring-blue-100 transition" />
              </div>

              <div className="flex flex-col gap-1.5">
                <label className="text-sm font-medium text-slate-700">Address</label>
                <input name="Address" value={form.Address} onChange={handleChange} placeholder="Enter full address" className="px-3 py-2.5 border border-slate-200 rounded-lg text-sm text-slate-800 outline-none focus:border-blue-500 focus:ring-2 focus:ring-blue-100 transition" />
              </div>

              <div className="flex flex-col gap-1.5 md:col-span-2">
                <label className="text-sm font-medium text-slate-700">Notes</label>
                <textarea name="Notes" value={form.Notes} onChange={handleChange} rows={3} placeholder="Enter any relevant patient notes..." className="px-3 py-2.5 border border-slate-200 rounded-lg text-sm text-slate-800 outline-none focus:border-blue-500 focus:ring-2 focus:ring-blue-100 transition resize-y" />
              </div>
            </div>
          </div>
        </div>

        {/* Initial Booking */}
        <div className="bg-white rounded-2xl border border-slate-200 shadow-sm overflow-hidden">
          <div className="px-6 py-4 bg-slate-50 border-b border-slate-200 font-semibold text-slate-700">
            <i className="fa-regular fa-calendar-check mr-2 text-blue-500"></i> Initial Booking Details <span className="text-xs font-normal text-slate-400 ml-2">(Optional)</span>
          </div>
          <div className="p-6 md:p-8">
            <div className="grid grid-cols-1 md:grid-cols-2 gap-x-8 gap-y-5">
              <div className="flex flex-col gap-1.5">
                <label className="text-sm font-medium text-slate-700">Clinic</label>
                <select value={selectedClinicId} onChange={(e) => handleClinicChange(e.target.value)} className="px-3 py-2.5 border border-slate-200 rounded-lg text-sm text-slate-800 outline-none focus:border-blue-500 focus:ring-2 focus:ring-blue-100 transition bg-white">
                  <option value="">Select Clinic</option>
                  {CLINICS.map((c) => <option key={c.id} value={c.id}>{c.name}</option>)}
                </select>
              </div>

              <div className="flex flex-col gap-1.5">
                <label className="text-sm font-medium text-slate-700">Doctor</label>
                <select name="DoctorId" value={form.DoctorId} onChange={handleChange} disabled={!selectedClinicId} className="px-3 py-2.5 border border-slate-200 rounded-lg text-sm text-slate-800 outline-none focus:border-blue-500 focus:ring-2 focus:ring-blue-100 transition bg-white disabled:opacity-50 disabled:cursor-not-allowed">
                  <option value="">{selectedClinicId ? 'Select Doctor' : 'Select Clinic First'}</option>
                  {filteredDoctors.map((d) => <option key={d.id} value={d.id}>{d.fullName} ({d.specialization})</option>)}
                  {selectedClinicId && filteredDoctors.length === 0 && <option disabled>No doctors in this clinic</option>}
                </select>
              </div>

              <div className="flex flex-col gap-1.5">
                <label className="text-sm font-medium text-slate-700">Appointment Type</label>
                <select name="AppointmentType" value={form.AppointmentType} onChange={handleChange} className="px-3 py-2.5 border border-slate-200 rounded-lg text-sm text-slate-800 outline-none focus:border-blue-500 focus:ring-2 focus:ring-blue-100 transition bg-white">
                  <option value="1">First Visit</option>
                  <option value="2">Follow Up</option>
                  <option value="3">Checkup</option>
                </select>
              </div>

              <div className="flex flex-col gap-1.5">
                <label className="text-sm font-medium text-slate-700">Appointment Date & Time</label>
                <input name="AppointmentDate" type="datetime-local" value={form.AppointmentDate} onChange={handleChange} className="px-3 py-2.5 border border-slate-200 rounded-lg text-sm text-slate-800 outline-none focus:border-blue-500 focus:ring-2 focus:ring-blue-100 transition" />
              </div>

              <div className="flex flex-col gap-1.5 md:col-span-2">
                <label className="text-sm font-medium text-slate-700">Booking Notes</label>
                <textarea name="BookingNotes" value={form.BookingNotes} onChange={handleChange} rows={2} placeholder="Any notes for this appointment..." className="px-3 py-2.5 border border-slate-200 rounded-lg text-sm text-slate-800 outline-none focus:border-blue-500 focus:ring-2 focus:ring-blue-100 transition resize-y" />
              </div>
            </div>
          </div>
        </div>

        {/* Actions */}
        <div className="flex justify-end gap-3 pb-8">
          <button type="button" onClick={() => navigate('/management/patients')} className="px-6 py-2.5 border border-slate-200 rounded-lg text-slate-700 text-sm font-medium hover:bg-slate-50 transition-colors">
            Cancel
          </button>
          <button type="submit" disabled={isSubmitting} className="px-6 py-2.5 bg-blue-600 hover:bg-blue-700 disabled:bg-blue-300 text-white rounded-lg text-sm font-semibold transition-colors flex items-center gap-2">
            {isSubmitting ? <><i className="fa-solid fa-circle-notch fa-spin"></i> Saving...</> : <><i className="fa-solid fa-floppy-disk"></i> Save Patient</>}
          </button>
        </div>
      </form>
    </div>
  );
};
