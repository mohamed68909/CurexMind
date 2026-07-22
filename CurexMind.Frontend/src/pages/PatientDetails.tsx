import React, { useEffect, useState } from 'react';
import { useParams, useNavigate, Link } from 'react-router-dom';
import { apiClient } from '../api/apiClient';
import Swal from 'sweetalert2';
import { 
  ArrowLeft, 
  Edit, 
  Plus, 
  FileText, 
  Bed, 
  FileSpreadsheet, 
  Download, 
  Clock, 
  Calendar, 
  DollarSign, 
  Loader2 
} from 'lucide-react';

interface PatientProfile {
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

interface Invoice {
  invoiceId: string;
  invoiceNumber: string;
  totalAmount: number;
  status: number;
  issueDate: string;
  dueDate: string;
  doctorName: string;
}

interface Appointment {
  appointmentId: string;
  appointmentDate: string;
  doctorName: string;
  clinicName: string;
  status: number;
  appointmentType: number;
}

interface Stay {
  stayId: string;
  department: string;
  roomNumber: string;
  bedNumber: string;
  startDate: string;
  endDate: string | null;
  status: number;
  notes: string;
}

const genderLabel = (g: number) => (g === 1 ? 'Male' : g === 2 ? 'Female' : '—');
const socialLabel = (s: number) => (s === 1 ? 'Single' : s === 2 ? 'Married' : '—');

const appointmentStatusStyle = (s: number) => {
  if (s === 1) return 'bg-emerald-50 text-emerald-700 border border-emerald-200';
  if (s === 2) return 'bg-red-50 text-red-700 border border-red-200';
  return 'bg-amber-50 text-amber-700 border border-amber-200';
};
const appointmentStatusLabel = (s: number) => {
  if (s === 1) return 'Confirmed';
  if (s === 2) return 'Cancelled';
  return 'Pending';
};

const invoiceStatusStyle = (s: number) => {
  if (s === 1) return 'bg-emerald-50 text-emerald-700 border border-emerald-200';
  if (s === 2) return 'bg-red-50 text-red-700 border border-red-200';
  return 'bg-amber-50 text-amber-700 border border-amber-200';
};
const invoiceStatusLabel = (s: number) => {
  if (s === 1) return 'Paid';
  if (s === 2) return 'Overdue';
  return 'Pending';
};

const appointmentTypeLabel = (t: number) => {
  if (t === 1) return 'First Visit';
  if (t === 2) return 'Follow Up';
  if (t === 3) return 'Checkup';
  return '—';
};

export const PatientDetails: React.FC = () => {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();

  const [patient, setPatient] = useState<PatientProfile | null>(null);
  const [appointments, setAppointments] = useState<Appointment[]>([]);
  const [invoices, setInvoices] = useState<Invoice[]>([]);
  const [stays, setStays] = useState<Stay[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [activeTab, setActiveTab] = useState<'basic-info' | 'appointments' | 'invoices' | 'stays'>('basic-info');

  useEffect(() => {
    if (!id) return;
    setIsLoading(true);
    Promise.all([
      apiClient.get(`/api/Patients/${id}`),
      apiClient.get(`/api/Appointments/patient/${id}`).catch(() => ({ data: [] })),
      apiClient.get(`/api/Patients/${id}/invoices`).catch(() => ({ data: [] })),
      apiClient.get(`/api/Patients/${id}/stays`).catch(() => ({ data: [] })),
    ]).then(([patRes, apptRes, invRes, stayRes]) => {
      setPatient(patRes.data?.data ?? patRes.data);
      setAppointments(apptRes.data?.data ?? apptRes.data ?? []);
      setInvoices(invRes.data?.data ?? invRes.data ?? []);
      setStays(stayRes.data?.data ?? stayRes.data ?? []);
    }).catch((err) => {
      console.error('Failed to load patient details:', err);
      navigate('/management/patients');
    }).finally(() => setIsLoading(false));
  }, [id, navigate]);

  const exportPDF = () => {
    window.print();
  };

  const exportExcel = () => {
    if (!patient) return;
    if (invoices.length === 0) {
      Swal.fire({ title: 'No Invoices', text: 'There are no invoices to export for this patient.', icon: 'info' });
      return;
    }
    const headers = 'Date,Invoice #,Doctor,Amount,Paid,Remaining,Status\n';
    const rows = invoices.map(inv => {
      const remaining = inv.status === 1 ? 0 : inv.totalAmount;
      const paid = inv.status === 1 ? inv.totalAmount : 0;
      const statusLabel = inv.status === 1 ? 'Paid' : 'Pending';
      return `"${new Date(inv.issueDate).toLocaleDateString('en-GB')}","${inv.invoiceNumber || inv.invoiceId.slice(0, 8)}","Dr. ${inv.doctorName}",${inv.totalAmount},${paid},${remaining},"${statusLabel}"`;
    }).join('\n');

    const blob = new Blob([headers + rows], { type: 'text/csv;charset=utf-8;' });
    const url = URL.createObjectURL(blob);
    const link = document.createElement('a');
    link.setAttribute('href', url);
    link.setAttribute('download', `${patient.fullName.replace(/\s+/g, '_')}_Invoices.csv`);
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
  };

  if (isLoading) {
    return (
      <div className="flex flex-col items-center justify-center min-h-[400px] gap-3">
        <Loader2 className="h-10 w-10 text-blue-600 animate-spin" />
        <p className="text-slate-500 font-semibold text-sm">Loading patient profile...</p>
      </div>
    );
  }

  if (!patient) return null;

  const age = patient.dateOfBirth
    ? Math.floor((Date.now() - new Date(patient.dateOfBirth).getTime()) / (365.25 * 24 * 3600 * 1000))
    : null;

  // Financial calculations
  const unpaidInvoices = invoices.filter(inv => inv.status !== 1);
  const outstandingBalance = unpaidInvoices.reduce((sum, inv) => sum + inv.totalAmount, 0);
  const paidInvoices = invoices.filter(inv => inv.status === 1);
  const lastPaymentDate = paidInvoices.length > 0
    ? new Date(Math.max(...paidInvoices.map(inv => new Date(inv.issueDate).getTime()))).toLocaleDateString('en-GB')
    : '—';

  return (
    <div className="space-y-6 text-left" dir="ltr" id="print-area">
      {/* Print styles */}
      <style>{`
        @media print {
          body * {
            visibility: hidden;
          }
          #print-area, #print-area * {
            visibility: visible;
          }
          #print-area {
            position: absolute;
            left: 0;
            top: 0;
            width: 100%;
          }
          .hide-on-pdf {
            display: none !important;
          }
        }
      `}</style>

      {/* Header */}
      <div className="flex flex-col sm:flex-row justify-between items-start sm:items-center gap-4">
        <div>
          <h1 className="text-2xl font-bold text-slate-900 font-cairo">Patient Details</h1>
          <p className="text-xs text-slate-500 font-cairo">
            Home &gt; Patients &gt; View &gt; <span className="text-slate-800 font-semibold capitalize">{patient.fullName}</span>
          </p>
        </div>
        <div className="flex gap-2 w-full sm:w-auto hide-on-pdf">
          <button 
            onClick={exportPDF}
            className="flex-1 sm:flex-initial bg-white border border-slate-200 hover:bg-slate-50 text-slate-700 text-xs font-bold px-4 py-2.5 rounded-xl shadow-sm transition-all flex items-center justify-center gap-2"
          >
            <FileText className="h-4 w-4 text-rose-500" />
            <span>Export PDF</span>
          </button>
          <button 
            onClick={exportExcel}
            className="flex-1 sm:flex-initial bg-white border border-slate-200 hover:bg-slate-50 text-slate-700 text-xs font-bold px-4 py-2.5 rounded-xl shadow-sm transition-all flex items-center justify-center gap-2"
          >
            <FileSpreadsheet className="h-4 w-4 text-emerald-500" />
            <span>Export Excel</span>
          </button>
          <button 
            onClick={() => navigate('/management/patients')}
            className="flex-1 sm:flex-initial bg-white border border-slate-200 hover:bg-slate-50 text-slate-700 text-xs font-bold px-4 py-2.5 rounded-xl shadow-sm transition-all flex items-center justify-center gap-2"
          >
            <ArrowLeft className="h-4 w-4" />
            <span>Back</span>
          </button>
        </div>
      </div>

      {/* Main Profile Info Card */}
      <div className="bg-white rounded-2xl border border-slate-200 shadow-sm p-6">
        <div className="grid grid-cols-1 lg:grid-cols-3 gap-6 divide-y lg:divide-y-0 lg:divide-x divide-slate-100">
          
          {/* Patient Details Grid (Left 2 columns on desktop) */}
          <div className="lg:col-span-2 flex flex-col md:flex-row gap-6">
            <div className="flex-shrink-0 mx-auto md:mx-0">
              {patient.profileImageUrl ? (
                <img 
                  src={patient.profileImageUrl} 
                  alt={patient.fullName} 
                  className="w-24 h-24 rounded-full object-cover border border-slate-100 shadow-sm" 
                />
              ) : (
                <div className="w-24 h-24 rounded-full bg-blue-50 border border-blue-100 flex items-center justify-center text-3xl text-blue-500 font-bold capitalize shadow-sm">
                  {patient.fullName.charAt(0)}
                </div>
              )}
            </div>
            
            <div className="flex-grow space-y-4 text-center md:text-left">
              <div className="flex flex-col md:flex-row justify-between items-center md:items-start gap-2">
                <div>
                  <h2 className="text-xl font-bold text-slate-900 capitalize">{patient.fullName}</h2>
                  <p className="text-xs text-slate-400 font-mono mt-0.5">#{patient.patientId.substring(0, 8).toUpperCase()}</p>
                </div>
                <Link
                  to={`/management/patients/edit/${patient.patientId}`}
                  className="px-3 py-1.5 border border-slate-200 rounded-lg text-xs font-semibold text-slate-600 hover:bg-slate-50 transition-colors flex items-center gap-1.5 hide-on-pdf"
                >
                  <Edit className="h-3.5 w-3.5" />
                  <span>Edit Patient Info</span>
                </Link>
              </div>

              <div className="grid grid-cols-1 sm:grid-cols-2 gap-x-6 gap-y-3 text-sm text-left">
                <div className="flex items-center gap-2">
                  <span className="text-slate-400 w-24">Phone:</span> 
                  <span className="font-bold text-slate-800">{patient.phoneNumber || '—'}</span>
                </div>
                <div className="flex items-center gap-2">
                  <span className="text-slate-400 w-24">Age:</span> 
                  <span className="font-bold text-slate-800">{age !== null ? `${age} years` : '—'}</span>
                </div>
                <div className="flex items-center gap-2">
                  <span className="text-slate-400 w-24">National ID:</span> 
                  <span className="font-bold text-slate-800 font-mono text-xs">{patient.nationalId || '—'}</span>
                </div>
                <div className="flex items-center gap-2">
                  <span className="text-slate-400 w-24">Patient ID:</span> 
                  <span className="font-bold text-slate-800 text-xs font-mono">#{patient.patientId.substring(0, 8).toUpperCase()}</span>
                </div>
                <div className="flex items-center gap-2">
                  <span className="text-slate-400 w-24">Gender:</span> 
                  <span className="font-bold text-slate-800">{genderLabel(patient.gender)}</span>
                </div>
                <div className="flex items-center gap-2">
                  <span className="text-slate-400 w-24">Status:</span> 
                  <span className="bg-emerald-50 text-emerald-700 border border-emerald-100 px-2 py-0.5 rounded-full text-[10px] font-bold">Active</span>
                </div>
                <div className="col-span-1 sm:col-span-2 flex items-start gap-2">
                  <span className="text-slate-400 w-24 flex-shrink-0">Address:</span> 
                  <span className="font-bold text-slate-800">{patient.address || '—'}</span>
                </div>
              </div>
            </div>
          </div>

          {/* Quick Actions Panel (Right column on desktop) */}
          <div className="lg:col-span-1 pt-6 lg:pt-0 lg:pl-6 flex flex-col justify-center space-y-3 hide-on-pdf text-left">
            <h4 className="font-bold text-slate-400 text-[10px] uppercase tracking-wider mb-1">Quick Actions</h4>
            <button
              onClick={() => navigate('/management/appointments/add', { state: { patientId: patient.patientId } })}
              className="flex items-center justify-start gap-2.5 px-4 py-2.5 bg-blue-600 text-white rounded-lg text-xs font-bold hover:bg-blue-700 transition-colors shadow-sm w-full"
            >
              <Plus className="h-4 w-4" />
              <span>Add Appointment</span>
            </button>
            <button
              onClick={() => navigate('/management/invoices/add', { state: { patientId: patient.patientId } })}
              className="flex items-center justify-start gap-2.5 px-4 py-2.5 bg-emerald-600 text-white rounded-lg text-xs font-bold hover:bg-emerald-700 transition-colors shadow-sm w-full"
            >
              <FileText className="h-4 w-4" />
              <span>Add Invoice</span>
            </button>
            <button
              onClick={() => navigate('/management/stays/add', { state: { patientId: patient.patientId } })}
              className="flex items-center justify-start gap-2.5 px-4 py-2.5 bg-purple-600 text-white rounded-lg text-xs font-bold hover:bg-purple-700 transition-colors shadow-sm w-full"
            >
              <Bed className="h-4 w-4" />
              <span>Add Stay/Admission</span>
            </button>
          </div>
        </div>
      </div>

      {/* Tabs Container */}
      <div className="bg-white rounded-2xl border border-slate-200 overflow-hidden shadow-sm">
        <div className="flex border-b border-slate-200 bg-slate-50/50 hide-on-pdf">
          {[
            { id: 'basic-info', label: 'Basic Info' },
            { id: 'appointments', label: `Appointments (${appointments.length})` },
            { id: 'invoices', label: `Invoices (${invoices.length})` },
            { id: 'stays', label: `Stays (${stays.length})` }
          ].map((tab) => (
            <button
              key={tab.id}
              onClick={() => setActiveTab(tab.id as any)}
              className={`px-6 py-4 text-xs font-bold capitalize transition-colors border-b-2 ${
                activeTab === tab.id
                  ? 'text-blue-600 border-blue-600 bg-white'
                  : 'text-slate-500 hover:text-slate-700 hover:bg-slate-50/50 border-transparent'
              }`}
            >
              {tab.label}
            </button>
          ))}
        </div>

        <div className="p-6">
          {activeTab === 'basic-info' && (
            <div className="space-y-6">
              <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
                
                {/* Identity Card */}
                <div className="p-5 bg-slate-50 rounded-xl border border-slate-100 space-y-3 text-sm text-left">
                  <h4 className="font-bold text-slate-800 text-sm border-b pb-2 mb-2 flex items-center gap-2">
                    <i className="fa-solid fa-circle-info text-blue-500"></i> Identity & Status
                  </h4>
                  <div className="flex justify-between items-center text-xs">
                    <span className="text-slate-400">Social Status:</span> 
                    <span className="font-bold text-slate-800">{socialLabel(patient.socialStatus)}</span>
                  </div>
                  <div className="flex justify-between items-center text-xs">
                    <span className="text-slate-400">Date of Birth:</span> 
                    <span className="font-bold text-slate-800">
                      {patient.dateOfBirth ? new Date(patient.dateOfBirth).toLocaleDateString('en-GB') : '—'}
                    </span>
                  </div>
                  <div className="flex justify-between items-center text-xs">
                    <span className="text-slate-400">Email:</span> 
                    <span className="font-bold text-slate-800 font-mono">{patient.email || '—'}</span>
                  </div>
                </div>

                {/* Contact Card */}
                <div className="p-5 bg-slate-50 rounded-xl border border-slate-100 space-y-3 text-sm text-left">
                  <h4 className="font-bold text-slate-800 text-sm border-b pb-2 mb-2 flex items-center gap-2">
                    <i className="fa-solid fa-address-book text-blue-500"></i> Contact Details
                  </h4>
                  <div className="flex justify-between items-center text-xs">
                    <span className="text-slate-400">Phone:</span> 
                    <span className="font-bold text-slate-800">{patient.phoneNumber || '—'}</span>
                  </div>
                  <div className="flex justify-between items-center text-xs">
                    <span className="text-slate-400">Address:</span> 
                    <span className="font-bold text-slate-800">{patient.address || '—'}</span>
                  </div>
                </div>
              </div>

              {/* Patient Notes */}
              {patient.notes && (
                <div className="bg-amber-50 border border-amber-200 rounded-xl p-4 text-xs text-amber-800 text-left">
                  <h4 className="font-bold text-sm mb-1.5 flex items-center gap-1.5">
                    <i className="fa-solid fa-note-sticky text-amber-600"></i> Patient Notes
                  </h4>
                  <p className="leading-relaxed">{patient.notes}</p>
                </div>
              )}
            </div>
          )}

          {activeTab === 'appointments' && (
            <div className="overflow-x-auto">
              <table className="w-full text-sm text-left border-collapse">
                <thead>
                  <tr className="bg-blue-50/50 text-slate-800 text-xs font-semibold normal-case border-b border-slate-100">
                    <th className="p-4 font-semibold text-left">Doctor</th>
                    <th className="p-4 font-semibold text-left">Clinic</th>
                    <th className="p-4 font-semibold text-left">Type</th>
                    <th className="p-4 font-semibold text-left">Date & Time</th>
                    <th className="p-4 font-semibold text-right">Status</th>
                  </tr>
                </thead>
                <tbody className="divide-y divide-slate-50 text-slate-700">
                  {appointments.length === 0 ? (
                    <tr>
                      <td colSpan={5} className="text-center py-8 text-slate-400">
                        No appointments found.
                      </td>
                    </tr>
                  ) : (
                    appointments.map((a) => (
                      <tr key={a.appointmentId} className="hover:bg-slate-50/50 transition-colors">
                        <td className="p-4 font-semibold text-slate-900 capitalize">Dr. {a.doctorName}</td>
                        <td className="p-4 text-slate-600">{a.clinicName}</td>
                        <td className="p-4">
                          <span className="px-2 py-0.5 bg-slate-100 text-slate-600 text-[10px] font-bold rounded">
                            {appointmentTypeLabel(a.appointmentType)}
                          </span>
                        </td>
                        <td className="p-4">
                          <div className="flex flex-col text-xs text-slate-500 gap-0.5">
                            <span className="flex items-center gap-1.5 font-bold text-slate-700">
                              <Calendar className="h-3 w-3 text-slate-400" />
                              {new Date(a.appointmentDate).toLocaleDateString('en-US', { dateStyle: 'medium' })}
                            </span>
                            <span className="flex items-center gap-1.5">
                              <Clock className="h-3 w-3 text-slate-400" />
                              {new Date(a.appointmentDate).toLocaleTimeString('en-US', { timeStyle: 'short' })}
                            </span>
                          </div>
                        </td>
                        <td className="p-4 text-right">
                          <span className={`px-2.5 py-1 rounded-full text-[10px] font-bold border uppercase ${appointmentStatusStyle(a.status)}`}>
                            {appointmentStatusLabel(a.status)}
                          </span>
                        </td>
                      </tr>
                    ))
                  )}
                </tbody>
              </table>
            </div>
          )}

          {activeTab === 'invoices' && (
            <div>
              {/* Financial Summary */}
              <div className="mb-6 p-5 bg-blue-50/30 rounded-xl border border-blue-50 text-left">
                <h3 className="font-bold text-slate-800 text-xs uppercase tracking-wider mb-4 flex items-center gap-1.5">
                  <DollarSign className="h-4 w-4 text-blue-600" />
                  <span>Financial Summary</span>
                </h3>
                <div className="grid grid-cols-2 gap-4">
                  <div>
                    <span className="text-[10px] text-slate-400 block mb-0.5">Last Payment Date</span>
                    <span className="font-bold text-slate-700 text-sm">{lastPaymentDate}</span>
                  </div>
                  <div className="text-right">
                    <span className="text-[10px] text-slate-400 block mb-0.5 text-right">Outstanding Balance</span>
                    <span className="font-extrabold text-rose-600 text-lg">{outstandingBalance.toLocaleString()} EGP</span>
                  </div>
                </div>
              </div>

              {/* Invoices Table */}
              <div className="overflow-x-auto">
                <table className="w-full text-sm text-left border-collapse">
                  <thead>
                    <tr className="bg-blue-50/50 text-slate-800 text-xs font-semibold normal-case border-b border-slate-100">
                      <th className="p-4 font-semibold text-left">Date</th>
                      <th className="p-4 font-semibold text-left">Invoice #</th>
                      <th className="p-4 font-semibold text-left">Doctor</th>
                      <th className="p-4 font-semibold text-left">Amount</th>
                      <th className="p-4 font-semibold text-left">Paid</th>
                      <th className="p-4 font-semibold text-left">Remaining</th>
                      <th className="p-4 font-semibold text-right">Status</th>
                    </tr>
                  </thead>
                  <tbody className="divide-y divide-slate-50 text-slate-700">
                    {invoices.length === 0 ? (
                      <tr>
                        <td colSpan={7} className="text-center py-8 text-slate-400">
                          No invoices found.
                        </td>
                      </tr>
                    ) : (
                      invoices.map((inv) => {
                        const remaining = inv.status === 1 ? 0 : inv.totalAmount;
                        const paid = inv.status === 1 ? inv.totalAmount : 0;
                        return (
                          <tr key={inv.invoiceId} className="hover:bg-slate-50/50 transition-colors">
                            <td className="p-4 text-xs text-slate-500 font-sans">
                              {new Date(inv.issueDate).toLocaleDateString('en-GB')}
                            </td>
                            <td className="p-4 font-mono text-xs font-bold text-slate-500">
                              #{inv.invoiceNumber || inv.invoiceId.slice(0, 8).toUpperCase()}
                            </td>
                            <td className="p-4 text-slate-600 font-semibold capitalize">Dr. {inv.doctorName}</td>
                            <td className="p-4 font-bold text-slate-900">{inv.totalAmount.toLocaleString()} EGP</td>
                            <td className="p-4 text-slate-500 font-sans">{paid.toLocaleString()} EGP</td>
                            <td className="p-4 font-bold text-slate-700 font-sans">{remaining.toLocaleString()} EGP</td>
                            <td className="p-4 text-right">
                              <span className={`px-2.5 py-1 rounded-full text-[10px] font-bold border uppercase ${invoiceStatusStyle(inv.status)}`}>
                                {invoiceStatusLabel(inv.status)}
                              </span>
                            </td>
                          </tr>
                        );
                      })
                    )}
                  </tbody>
                </table>
              </div>
            </div>
          )}

          {activeTab === 'stays' && (
            <div className="overflow-x-auto">
              <table className="w-full text-sm text-left border-collapse">
                <thead>
                  <tr className="bg-blue-50/50 text-slate-800 text-xs font-semibold normal-case border-b border-slate-100">
                    <th className="p-4 font-semibold text-left">Department</th>
                    <th className="p-4 font-semibold text-left">Room / Bed</th>
                    <th className="p-4 font-semibold text-left">Start Date</th>
                    <th className="p-4 font-semibold text-left">End Date</th>
                    <th className="p-4 font-semibold text-left">Notes</th>
                    <th className="p-4 font-semibold text-right">Status</th>
                  </tr>
                </thead>
                <tbody className="divide-y divide-slate-50 text-slate-700">
                  {stays.length === 0 ? (
                    <tr>
                      <td colSpan={6} className="text-center py-8 text-slate-400">
                        No hospital stays registered.
                      </td>
                    </tr>
                  ) : (
                    stays.map((stay) => {
                      const statusStyle = stay.status === 0
                        ? 'bg-blue-50 text-blue-700 border-blue-100'
                        : 'bg-slate-100 text-slate-500 border-slate-200';
                      const statusLabel = stay.status === 0 ? 'Active' : 'Completed';
                      return (
                        <tr key={stay.stayId} className="hover:bg-slate-50/50 transition-colors">
                          <td className="p-4 font-semibold text-slate-900">{stay.department}</td>
                          <td className="p-4">
                            <div className="flex flex-col text-xs gap-0.5">
                              <span className="font-semibold text-slate-800">Room: {stay.roomNumber}</span>
                              <span className="text-slate-400">Bed: {stay.bedNumber}</span>
                            </div>
                          </td>
                          <td className="p-4 text-xs text-slate-500 font-sans">
                            {new Date(stay.startDate).toLocaleDateString('en-GB')}
                          </td>
                          <td className="p-4 text-xs text-slate-500 font-sans">
                            {stay.endDate ? new Date(stay.endDate).toLocaleDateString('en-GB') : '--'}
                          </td>
                          <td className="p-4 text-xs text-slate-500 max-w-[150px] truncate" title={stay.notes}>
                            {stay.notes || '—'}
                          </td>
                          <td className="p-4 text-right">
                            <span className={`px-2.5 py-1 rounded-full text-[10px] font-bold border uppercase ${statusStyle}`}>
                              {statusLabel}
                            </span>
                          </td>
                        </tr>
                      );
                    })
                  )}
                </tbody>
              </table>
            </div>
          )}
        </div>
      </div>
    </div>
  );
};
