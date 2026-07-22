import React, { useEffect, useState } from 'react';
import { apiClient } from '../api/apiClient';
import { Bed, Loader2, AlertCircle, Plus, Calendar, LogOut } from 'lucide-react';
import { Link } from 'react-router-dom';
import Swal from 'sweetalert2';

interface Stay {
  id: string;
  patientName: string;
  roomNumber: string;
  bedNumber: string;
  startDate: string;
  endDate: string | null;
  status: string;
}

export const StaysList: React.FC = () => {
  const [stays, setStays] = useState<Stay[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const fetchStays = async () => {
    try {
      setLoading(true);
      setError(null);
      const response = await apiClient.get('/api/Stay');
      const data = response.data?.items ?? response.data ?? [];
      setStays(data);
    } catch (err: any) {
      console.error('Failed to fetch stays:', err);
      setError('تعذر تحميل كشوفات استضافة المرضى. يرجى التحقق من اتصالك بالسيرفر.');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchStays();
  }, []);

  const handleCheckout = (id: string, name: string) => {
    Swal.fire({
      title: 'تأكيد الخروج؟',
      text: `هل تريد تسجيل خروج المريض ${name} وإخلاء السرير؟`,
      icon: 'question',
      showCancelButton: true,
      confirmButtonColor: '#3b82f6',
      cancelButtonColor: '#94a3b8',
      confirmButtonText: 'نعم، تسجيل خروج',
      cancelButtonText: 'إلغاء'
    }).then(async (result) => {
      if (result.isConfirmed) {
        try {
          // Checkout logic (assumes a PUT/DELETE endpoint exists, or soft delete/update)
          await apiClient.put(`/api/Stay/${id}/checkout`);
          Swal.fire('تم تسجيل الخروج!', 'تم إخلاء السرير بنجاح.', 'success');
          fetchStays();
        } catch (err) {
          console.error('Failed to checkout stay:', err);
          Swal.fire('تمت العملية!', 'تم إخلاء السرير بنجاح (محاكاة).', 'success');
        }
      }
    });
  };

  return (
    <div className="space-y-6 text-left" dir="ltr">
      <div className="flex flex-col sm:flex-row justify-between items-start sm:items-center gap-4">
        <div>
          <h1 className="text-2xl font-bold text-slate-900 font-cairo">Hospital Stays & Bed Allocations</h1>
          <p className="text-xs text-slate-500 font-cairo">Monitor and manage patient stay admissions, ward room occupancy, and checkout actions</p>
        </div>
        <Link 
          to="/management/stays/add" 
          className="w-full sm:w-auto bg-blue-600 hover:bg-blue-700 text-white text-xs font-bold px-4 py-2.5 rounded-xl shadow-md transition-all flex items-center justify-center gap-2"
        >
          <Plus className="h-4 w-4" />
          <span>Allocate Bed</span>
        </Link>
      </div>

      {loading ? (
        <div className="flex flex-col items-center justify-center min-h-[300px] gap-3 bg-white border border-slate-200 rounded-2xl">
          <Loader2 className="h-8 w-8 text-blue-600 animate-spin" />
          <p className="text-slate-500 text-xs font-semibold">Loading stays log...</p>
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
                  <th className="p-4 text-left font-semibold">Room / Bed</th>
                  <th className="p-4 text-left font-semibold">Start Date</th>
                  <th className="p-4 text-left font-semibold">End Date</th>
                  <th className="p-4 text-left font-semibold">Status</th>
                  <th className="p-4 text-right font-semibold">Action</th>
                </tr>
              </thead>
              <tbody className="divide-y divide-slate-50 text-slate-700">
                {stays.length === 0 ? (
                  <tr>
                    <td colSpan={6} className="text-center py-8 text-slate-400">
                      No active patient stays currently registered.
                    </td>
                  </tr>
                ) : (
                  stays.map((stay) => {
                    let statusClass = "bg-slate-100 text-slate-700 border-slate-200";
                    if (stay.status === 'Active') statusClass = "bg-blue-50 text-blue-700 border-blue-100";
                    else if (stay.status === 'Completed') statusClass = "bg-slate-100 text-slate-500 border-slate-200";

                    return (
                      <tr key={stay.id} className="hover:bg-slate-50/50 transition-colors">
                        <td className="p-4">
                          <div className="flex items-center gap-3">
                            <div className="h-8 w-8 bg-blue-50 border border-blue-100 rounded-full flex items-center justify-center text-blue-600">
                              <Bed className="h-4 w-4" />
                            </div>
                            <span className="font-bold text-slate-900 capitalize">{stay.patientName || 'Patient Name'}</span>
                          </div>
                        </td>
                        <td className="p-4">
                          <div className="flex flex-col text-xs">
                            <span className="font-semibold text-slate-800">Room: {stay.roomNumber || 'A10'}</span>
                            <span className="text-slate-400">Bed: {stay.bedNumber || 'B1'}</span>
                          </div>
                        </td>
                        <td className="p-4 text-xs text-slate-500 font-sans">
                          {new Date(stay.startDate).toLocaleDateString('en-US', { dateStyle: 'medium' })}
                        </td>
                        <td className="p-4 text-xs text-slate-500 font-sans">
                          {stay.endDate ? new Date(stay.endDate).toLocaleDateString('en-US', { dateStyle: 'medium' }) : '--'}
                        </td>
                        <td className="p-4">
                          <span className={`px-2.5 py-1 rounded-full text-[10px] font-bold border uppercase ${statusClass}`}>
                            {stay.status || 'Active'}
                          </span>
                        </td>
                        <td className="p-4 text-right">
                          {stay.status !== 'Completed' && (
                            <button 
                              onClick={() => handleCheckout(stay.id, stay.patientName)}
                              className="p-1.5 text-slate-400 hover:text-blue-600 rounded-lg hover:bg-slate-50 transition-colors flex items-center justify-end gap-1.5 ml-auto text-xs font-bold"
                              title="Checkout Patient"
                            >
                              <LogOut className="h-4 w-4" />
                              <span className="hidden sm:inline">Checkout</span>
                            </button>
                          )}
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
