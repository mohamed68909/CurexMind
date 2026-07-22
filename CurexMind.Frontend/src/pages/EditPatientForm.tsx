import React, { useEffect, useRef, useState } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import Swal from 'sweetalert2';
import { apiClient } from '../api/apiClient';

interface PatientData {
  patientId: string;
  fullName: string;
  gender: number;
  socialStatus: number;
  dateOfBirth: string;
  phoneNumber: string;
  email: string;
  nationalId: string;
  address: string;
  notes: string;
  profileImageUrl: string;
}

export const EditPatientForm: React.FC = () => {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const fileInputRef = useRef<HTMLInputElement>(null);

  const [imagePreview, setImagePreview] = useState<string | null>(null);
  const [imageFile, setImageFile] = useState<File | null>(null);
  const [isLoading, setIsLoading] = useState(true);
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
  });

  useEffect(() => {
    if (!id) return;
    apiClient.get(`/api/Patients/${id}`).then((res: any) => {
      const p: PatientData = res.data?.data ?? res.data;
      if (!p) return;
      setForm({
        FullName: p.fullName ?? '',
        Gender: String(p.gender ?? ''),
        SocialStatus: String(p.socialStatus ?? ''),
        DateOfBirth: p.dateOfBirth ? p.dateOfBirth.split('T')[0] : '',
        PhoneNumber: p.phoneNumber ?? '',
        Email: p.email ?? '',
        NationalId: p.nationalId ?? '',
        Address: p.address ?? '',
        Notes: p.notes ?? '',
      });
      if (p.profileImageUrl) setImagePreview(p.profileImageUrl);
    }).catch(() => {
      Swal.fire({ title: 'Error', text: 'Failed to load patient data.', icon: 'error' });
      navigate('/management/patients');
    }).finally(() => setIsLoading(false));
  }, [id, navigate]);

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
    formData.append('PatientId', id!);
    formData.append('FullName', form.FullName);
    formData.append('Gender', form.Gender);
    if (form.SocialStatus) formData.append('SocialStatus', form.SocialStatus);
    formData.append('DateOfBirth', new Date(form.DateOfBirth).toISOString());
    formData.append('PhoneNumber', form.PhoneNumber);
    formData.append('Email', form.Email);
    if (form.NationalId) formData.append('NationalId', form.NationalId);
    if (form.Address) formData.append('Address', form.Address);
    if (form.Notes) formData.append('Notes', form.Notes);
    if (imageFile) formData.append('ProfileImageUrl', imageFile);

    try {
      await apiClient.put(`/api/Patients/${id}`, formData, {
        headers: { 'Content-Type': 'multipart/form-data' },
      });
      await Swal.fire({ title: 'Updated!', text: 'Patient information saved.', icon: 'success' });
      navigate('/management/patients');
    } catch (err: any) {
      const msg = err.response?.data?.errors?.[0]?.message ?? err.response?.data?.title ?? 'Update failed.';
      Swal.fire({ title: 'Error', text: msg, icon: 'error' });
    } finally {
      setIsSubmitting(false);
    }
  };

  if (isLoading) {
    return (
      <div className="flex items-center justify-center h-64">
        <div className="text-center space-y-3">
          <div className="w-12 h-12 border-4 border-blue-600 border-t-transparent rounded-full animate-spin mx-auto"></div>
          <p className="text-slate-500 text-sm">Loading patient data...</p>
        </div>
      </div>
    );
  }

  return (
    <div className="space-y-6">
      {/* Header */}
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-2xl font-bold text-slate-800">Edit Patient</h1>
          <p className="text-sm text-slate-500 mt-1">Update the patient's registration information</p>
        </div>
        <button onClick={() => navigate('/management/patients')} className="flex items-center gap-2 px-4 py-2 border border-slate-200 rounded-lg text-slate-600 text-sm font-medium hover:bg-slate-50 transition-colors">
          <i className="fa-solid fa-arrow-left text-xs"></i> Back to Patients
        </button>
      </div>

      <form onSubmit={handleSubmit} className="space-y-6">
        <div className="bg-white rounded-2xl border border-slate-200 shadow-sm overflow-hidden">
          <div className="px-6 py-4 bg-slate-50 border-b border-slate-200 font-semibold text-slate-700">
            <i className="fa-regular fa-pen-to-square mr-2 text-blue-500"></i> Patient Information
          </div>
          <div className="p-6 md:p-8">
            {/* Avatar */}
            <div className="flex justify-center mb-8">
              <div className="relative">
                <div
                  onClick={() => fileInputRef.current?.click()}
                  className="w-24 h-24 rounded-full border-2 border-dashed border-slate-300 bg-slate-50 flex items-center justify-center cursor-pointer overflow-hidden hover:border-blue-400 transition-colors"
                  style={imagePreview ? { backgroundImage: `url(${imagePreview})`, backgroundSize: 'cover', backgroundPosition: 'center' } : {}}
                >
                  {!imagePreview && <i className="fa-regular fa-user text-3xl text-slate-300"></i>}
                </div>
                <button type="button" onClick={() => fileInputRef.current?.click()}
                  className="absolute bottom-0 right-0 w-7 h-7 rounded-full bg-blue-600 text-white flex items-center justify-center border-2 border-white shadow hover:bg-blue-700 transition-colors">
                  <i className="fa-solid fa-pen text-xs"></i>
                </button>
                <input ref={fileInputRef} type="file" accept="image/*" className="hidden" onChange={handleImageChange} />
              </div>
            </div>

            <div className="grid grid-cols-1 md:grid-cols-2 gap-x-8 gap-y-5">
              {/* Reusable form fields */}
              {[
                { label: 'Patient Name', name: 'FullName', type: 'text', required: true, placeholder: 'Enter full name' },
                { label: 'Phone Number', name: 'PhoneNumber', type: 'tel', required: true, placeholder: 'e.g. 01012345678' },
                { label: 'Email Address', name: 'Email', type: 'email', required: true, placeholder: 'example@email.com' },
                { label: 'National ID', name: 'NationalId', type: 'text', placeholder: 'Enter national ID' },
                { label: 'Address', name: 'Address', type: 'text', placeholder: 'Enter full address' },
              ].map((field) => (
                <div key={field.name} className="flex flex-col gap-1.5">
                  <label className="text-sm font-medium text-slate-700">{field.label}{field.required && <span className="text-red-500 ml-1">*</span>}</label>
                  <input
                    name={field.name}
                    type={field.type}
                    value={(form as any)[field.name]}
                    onChange={handleChange}
                    required={field.required}
                    placeholder={field.placeholder}
                    className="px-3 py-2.5 border border-slate-200 rounded-lg text-sm text-slate-800 outline-none focus:border-blue-500 focus:ring-2 focus:ring-blue-100 transition"
                  />
                </div>
              ))}

              <div className="flex flex-col gap-1.5">
                <label className="text-sm font-medium text-slate-700">Date of Birth <span className="text-red-500">*</span></label>
                <input name="DateOfBirth" type="date" value={form.DateOfBirth} onChange={handleChange} required className="px-3 py-2.5 border border-slate-200 rounded-lg text-sm text-slate-800 outline-none focus:border-blue-500 focus:ring-2 focus:ring-blue-100 transition" />
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

              <div className="flex flex-col gap-1.5 md:col-span-2">
                <label className="text-sm font-medium text-slate-700">Notes</label>
                <textarea name="Notes" value={form.Notes} onChange={handleChange} rows={3} placeholder="Enter any relevant patient notes..." className="px-3 py-2.5 border border-slate-200 rounded-lg text-sm text-slate-800 outline-none focus:border-blue-500 focus:ring-2 focus:ring-blue-100 transition resize-y" />
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
            {isSubmitting ? <><i className="fa-solid fa-circle-notch fa-spin"></i> Saving...</> : <><i className="fa-solid fa-floppy-disk"></i> Save Changes</>}
          </button>
        </div>
      </form>
    </div>
  );
};
