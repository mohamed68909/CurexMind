import React, { useEffect, useState } from 'react';
import { apiClient } from '../api/apiClient';
import { Plus, Calendar, Clock, User, Loader2, AlertCircle, Trash2, CheckCircle2 } from 'lucide-react';
import { Link } from 'react-router-dom';
import Swal from 'sweetalert2';

interface Appointment {
  id: string;
  patientName: string;
  doctorName: string;
  appointmentDate: string;
  status: string;
}

export const AppointmentsList: React.FC = () => {
  const [appointments, setAppointments] = useState<Appointment[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const fetchAppointments = async () => {
    try {
      setLoading(true);
      setError(null);
      
      const response = await apiClient.get('/api/Appointments');
      const data = response.data?.items ?? response.data ?? [];
      setAppointments(data);
    } catch (err: any) {
      console.error('Failed to fetch appointments:', err);
      setError('تعذر تحميل جدول المواعيد من الخادم. يرجى التحقق من اتصالك.');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchAppointments();
  }, []);

  const handleCancel = (id: string) => {
    Swal.fire({
      title: 'إلغاء الموعد؟',
      text: 'هل أنت متأكد من رغبتك في إلغاء هذا الموعد؟',
      icon: 'warning',
      showCancelButton: true,
      confirmButtonColor: '#ef4444',
      cancelButtonColor: '#94a3b8',
      confirmButtonText: 'نعم، قم بالإلغاء',
      cancelButtonText: 'تراجع'
    }).then(async (result) => {
      if (result.isConfirmed) {
        try {
          await apiClient.delete(`/api/Appointments/${id}`);
          Swal.fire('تم الإلغاء!', 'تم إلغاء الموعد بنجاح.', 'success');
          fetchAppointments();
        } catch (err) {
          console.error('Failed to cancel appointment:', err);
          Swal.fire('خطأ!', 'تعذر إلغاء الموعد في الوقت الحالي.', 'error');
        }
      }
    });
  };

  return (
    <div className="space-y-6 text-left" dir="ltr">
      <div className="flex flex-col sm:flex-row justify-between items-start sm:items-center gap-4">
        <div>
          <h1 className="text-2xl font-bold text-slate-900 font-cairo">Appointments Calendar</h1>
          <p className="text-xs text-slate-500 font-cairo">Manage scheduling, booking request approvals, and patient visits</p>
        </div>
        <Link 
          to="/management/appointments/add" 
          className="w-full sm:w-auto bg-emerald-600 hover:bg-emerald-700 text-white text-xs font-bold px-4 py-2.5 rounded-xl shadow-md transition-all flex items-center justify-center gap-2"
        >
          <Plus className="h-4 w-4" />
          <span>Book Appointment</span>
        </Link>
      </div>

      {loading ? (
        <div className="flex flex-col items-center justify-center min-h-[300px] gap-3 bg-white border border-slate-200 rounded-2xl">
          <Loader2 className="h-8 w-8 text-emerald-600 animate-spin" />
          <p className="text-slate-500 text-xs font-semibold">Loading appointments schedule...</p>
        </div>
      ) : error ? (
        <div className="bg-red-50 border border-red-200 text-red-700 p-6 rounded-2xl flex items-start gap-4 shadow-sm">
          <AlertCircle className="h-6 w-6 flex-shrink-0 mt-0.5" />
          <div>
            <h3 className="font-bold text-sm">Failed to retrieve data</h3>
            <p className="text-xs mt-1">{error}</p>
          </div>
        </div>
      ) : (
        <div className="bg-white rounded-2xl border border-slate-200 overflow-hidden shadow-sm">
          <div className="overflow-x-auto">
            <table className="w-full text-sm text-left border-collapse">
              <thead>
                <tr className="bg-blue-50/50 text-slate-800 text-xs font-semibold normal-case border-b border-slate-100">
                  <th className="p-4 text-left font-semibold">Patient</th>
                  <th className="p-4 text-left font-semibold">Doctor</th>
                  <th className="p-4 text-left font-semibold">Date & Time</th>
                  <th className="p-4 text-left font-semibold">Status</th>
                  <th className="p-4 text-right font-semibold">Actions</th>
                </tr>
              </thead>
              <tbody className="divide-y divide-slate-50 text-slate-700">
                {appointments.length === 0 ? (
                  <tr>
                    <td colSpan={5} className="text-center py-8 text-slate-400">
                      No appointments scheduled.
                    </td>
                  </tr>
                ) : (
                  appointments.map((app) => {
                    let statusClass = "bg-slate-100 text-slate-700 border-slate-200";
                    if (app.status === 'Confirmed') statusClass = "bg-emerald-50 text-emerald-700 border-emerald-100";
                    else if (app.status === 'Waiting' || app.status === 'Pending') statusClass = "bg-amber-50 text-amber-700 border-amber-100";
                    else if (app.status === 'Cancelled' || app.status === 'Missed') statusClass = "bg-rose-50 text-rose-700 border-rose-100";

                    return (
                      <tr key={app.id} className="hover:bg-slate-50/50 transition-colors">
                        <td className="p-4">
                          <div className="flex items-center gap-3">
                            <div className="h-8 w-8 bg-slate-50 border border-slate-200 rounded-full flex items-center justify-center text-slate-600">
                              <User className="h-4 w-4" />
                            </div>
                            <span className="font-bold text-slate-900 capitalize">{app.patientName || 'Unknown Patient'}</span>
                          </div>
                        </td>
                        <td className="p-4 text-slate-600 font-semibold">{app.doctorName || 'Dr. Specialist'}</td>
                        <td className="p-4">
                          <div className="flex flex-col text-xs text-slate-500 gap-1">
                            <span className="flex items-center gap-1.5 font-bold text-slate-700">
                              <Calendar className="h-3.5 w-3.5 text-slate-400" />
                              {new Date(app.appointmentDate).toLocaleDateString('en-US', { dateStyle: 'medium' })}
                            </span>
                            <span className="flex items-center gap-1.5">
                              <Clock className="h-3.5 w-3.5 text-slate-400" />
                              {new Date(app.appointmentDate).toLocaleTimeString('en-US', { timeStyle: 'short' })}
                            </span>
                          </div>
                        </td>
                        <td className="p-4">
                          <span className={`px-2.5 py-1 rounded-full text-[10px] font-bold border uppercase ${statusClass}`}>
                            {app.status || 'Scheduled'}
                          </span>
                        </td>
                        <td className="p-4 text-right">
                          <div className="flex items-center justify-end gap-2">
                            <button 
                              onClick={() => handleCancel(app.id)}
                              disabled={app.status === 'Cancelled'}
                              className="p-1.5 text-slate-400 hover:text-red-600 rounded-lg hover:bg-slate-50 transition-colors disabled:opacity-50 disabled:cursor-not-allowed"
                              title="Cancel Appointment"
                            >
                              <Trash2 className="h-4 w-4" />
                            </button>
                          </div>
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
    </div>
  );
};
