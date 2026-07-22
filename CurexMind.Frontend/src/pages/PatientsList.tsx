import React, { useEffect, useState } from 'react';
import { apiClient } from '../api/apiClient';
import { 
  Plus, 
  Search, 
  Loader2, 
  AlertCircle, 
  Download, 
  User, 
  Trash2, 
  Edit 
} from 'lucide-react';
import { Link } from 'react-router-dom';
import Swal from 'sweetalert2';

interface Patient {
  id: string;
  fullName: string;
  phoneNumber: string;
  gender: number;
  age: number;
  address: string;
}

export const PatientsList: React.FC = () => {
  const [patients, setPatients] = useState<Patient[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  // Pagination & Search States
  const [currentPage, setCurrentPage] = useState(1);
  const [totalPages, setTotalPages] = useState(1);
  const [totalCount, setTotalCount] = useState(0);
  const [searchTerm, setSearchTerm] = useState('');
  const [genderFilter, setGenderFilter] = useState('');
  
  const pageSize = 10;

  const fetchPatients = async (page = 1) => {
    try {
      setLoading(true);
      setError(null);
      
      const response = await apiClient.get('/api/Patients', {
        params: {
          page: page,
          pageSize: pageSize
        }
      });
      
      const result = response.data;
      const list = result.items || result.data || (Array.isArray(result) ? result : []);
      setPatients(list);
      setTotalPages(result.totalPages || 1);
      setTotalCount(result.totalCount || list.length);
    } catch (err: any) {
      console.error('Failed to fetch patients:', err);
      setError('تعذر تحميل سجل المرضى من الخادم. يرجى التحقق من اتصالك بالإنترنت.');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchPatients(currentPage);
  }, [currentPage]);

  const handleDelete = (id: string, name: string) => {
    Swal.fire({
      title: 'هل أنت متأكد؟',
      text: `سيتم حذف سجل المريض ${name} بشكل نهائي.`,
      icon: 'warning',
      showCancelButton: true,
      confirmButtonColor: '#ef4444',
      cancelButtonColor: '#94a3b8',
      confirmButtonText: 'نعم، احذفه',
      cancelButtonText: 'إلغاء'
    }).then(async (result) => {
      if (result.isConfirmed) {
        try {
          await apiClient.delete(`/api/Patients/${id}`);
          Swal.fire('تم الحذف!', 'تم حذف ملف المريض بنجاح.', 'success');
          // Refresh list
          fetchPatients(currentPage);
        } catch (err) {
          console.error('Failed to delete patient:', err);
          Swal.fire('خطأ!', 'فشلت عملية الحذف، يرجى المحاولة لاحقاً.', 'error');
        }
      }
    });
  };

  // Client-side filtering wrapper for search terms
  const filteredPatients = patients.filter((patient) => {
    const matchesSearch = 
      (patient.fullName && patient.fullName.toLowerCase().includes(searchTerm.toLowerCase())) ||
      (patient.phoneNumber && patient.phoneNumber.includes(searchTerm));
    
    const matchesGender = 
      genderFilter === '' || 
      patient.gender.toString() === genderFilter;

    return matchesSearch && matchesGender;
  });

  return (
    <div className="space-y-6 text-left" dir="ltr">
      {/* Page Header */}
      <div className="flex flex-col sm:flex-row justify-between items-start sm:items-center gap-4">
        <div>
          <h1 className="text-2xl font-bold text-slate-900 font-cairo">Patients Directory</h1>
          <p className="text-xs text-slate-500 font-cairo">Manage clinic records, medical history, and invoicing details</p>
        </div>
        <div className="flex gap-2 w-full sm:w-auto">
          <button className="flex-1 sm:flex-initial bg-white border border-slate-200 hover:bg-slate-50 text-slate-700 text-xs font-bold px-4 py-2.5 rounded-xl shadow-sm transition-all flex items-center justify-center gap-2">
            <Download className="h-4 w-4" />
            <span>Export CSV</span>
          </button>
          <Link 
            to="/management/patients/add" 
            className="flex-1 sm:flex-initial bg-blue-600 hover:bg-blue-700 text-white text-xs font-bold px-4 py-2.5 rounded-xl shadow-md transition-all flex items-center justify-center gap-2"
          >
            <Plus className="h-4 w-4" />
            <span>Add New Patient</span>
          </Link>
        </div>
      </div>

      {/* Filter and Search Bar */}
      <div className="bg-white p-5 rounded-2xl border border-slate-200 shadow-sm grid grid-cols-1 sm:grid-cols-3 gap-4 items-end">
        <div className="space-y-1.5">
          <label className="text-xs font-bold text-slate-700">Search</label>
          <div className="relative">
            <input 
              type="text" 
              placeholder="Search by name or phone..."
              value={searchTerm}
              onChange={(e) => setSearchTerm(e.target.value)}
              className="w-full border border-slate-200 focus:border-blue-500 focus:ring-2 focus:ring-blue-100 rounded-xl p-2.5 text-xs outline-none pl-9 transition-all"
            />
            <Search className="absolute inset-y-0 left-0 flex items-center pl-3 h-full text-slate-400 pointer-events-none" size={16} />
          </div>
        </div>

        <div className="space-y-1.5">
          <label className="text-xs font-bold text-slate-700">Gender</label>
          <select 
            value={genderFilter}
            onChange={(e) => setGenderFilter(e.target.value)}
            className="w-full border border-slate-200 focus:border-blue-500 focus:ring-2 focus:ring-blue-100 rounded-xl p-2.5 text-xs outline-none bg-white transition-all"
          >
            <option value="">All Genders</option>
            <option value="1">Male</option>
            <option value="2">Female</option>
          </select>
        </div>

        <button 
          onClick={() => { setSearchTerm(''); setGenderFilter(''); }}
          className="border border-slate-200 hover:bg-slate-50 text-slate-500 hover:text-slate-700 font-bold p-2.5 rounded-xl text-xs transition-colors h-[38px]"
        >
          Clear Filters
        </button>
      </div>

      {/* Patients Data Table Container */}
      {loading ? (
        <div className="flex flex-col items-center justify-center min-h-[300px] gap-3 bg-white border border-slate-200 rounded-2xl">
          <Loader2 className="h-8 w-8 text-blue-600 animate-spin" />
          <p className="text-slate-500 text-xs font-semibold">Loading patients...</p>
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
                <tr className="bg-blue-50/50 text-slate-800 text-xs font-bold normal-case border-b border-slate-100">
                  <th className="p-4 text-left font-semibold">Full Name</th>
                  <th className="p-4 text-left font-semibold">ID</th>
                  <th className="p-4 text-left font-semibold">Phone</th>
                  <th className="p-4 text-left font-semibold">Gender</th>
                  <th className="p-4 text-left font-semibold">Age</th>
                  <th className="p-4 text-left font-semibold">Address</th>
                  <th className="p-4 text-right font-semibold">Actions</th>
                </tr>
              </thead>
              <tbody className="divide-y divide-slate-50 text-slate-700">
                {filteredPatients.length === 0 ? (
                  <tr>
                    <td colSpan={7} className="text-center py-8 text-slate-400">
                      No patients found matching the criteria.
                    </td>
                  </tr>
                ) : (
                  filteredPatients.map((patient) => (
                    <tr key={patient.id} className="hover:bg-slate-50/50 transition-colors">
                      <td className="p-4">
                        <div className="flex items-center gap-3">
                          <div className="h-8 w-8 bg-blue-50 border border-blue-100 rounded-full flex items-center justify-center text-blue-600">
                            <User className="h-4 w-4" />
                          </div>
                          <Link 
                            to={`/management/patients/${patient.id}`} 
                            className="font-bold text-slate-900 capitalize hover:text-blue-600 transition-colors"
                          >
                            {patient.fullName}
                          </Link>
                        </div>
                      </td>
                      <td className="p-4 font-mono text-xs text-slate-400">
                        #{patient.id.substring(0, 5)}
                      </td>
                      <td className="p-4 text-slate-600 font-sans">{patient.phoneNumber || 'N/A'}</td>
                      <td className="p-4">
                        <span className={`px-2 py-0.5 rounded text-[10px] font-bold border ${
                          patient.gender === 1 
                            ? 'bg-sky-50 text-sky-700 border-sky-100' 
                            : 'bg-rose-50 text-rose-700 border-rose-100'
                        }`}>
                          {patient.gender === 1 ? 'Male' : 'Female'}
                        </span>
                      </td>
                      <td className="p-4 font-medium">{patient.age} y/o</td>
                      <td className="p-4 text-slate-500 truncate max-w-[150px]">{patient.address || 'N/A'}</td>
                      <td className="p-4 text-right">
                        <div className="flex items-center justify-end gap-2">
                          <Link 
                            to={`/management/patients/edit/${patient.id}`} 
                            className="p-1.5 text-slate-400 hover:text-blue-600 rounded-lg hover:bg-slate-50 transition-colors"
                            title="Edit"
                          >
                            <Edit className="h-4 w-4" />
                          </Link>
                          <button 
                            onClick={() => handleDelete(patient.id, patient.fullName)}
                            className="p-1.5 text-slate-400 hover:text-red-600 rounded-lg hover:bg-slate-50 transition-colors"
                            title="Delete"
                          >
                            <Trash2 className="h-4 w-4" />
                          </button>
                        </div>
                      </td>
                    </tr>
                  ))
                )}
              </tbody>
            </table>
          </div>

          {/* Pagination Footer */}
          <div className="bg-slate-50 border-t border-slate-100 p-4 flex flex-col sm:flex-row justify-between items-center gap-4 text-xs font-semibold text-slate-500">
            <span>Showing {filteredPatients.length} of {totalCount} patients</span>
            <div className="flex gap-2">
              <button 
                onClick={() => setCurrentPage(prev => Math.max(prev - 1, 1))}
                disabled={currentPage === 1 || loading}
                className="px-3 py-1.5 bg-white border border-slate-200 rounded-lg hover:bg-slate-50 text-slate-700 disabled:opacity-50 disabled:cursor-not-allowed"
              >
                Previous
              </button>
              <span className="px-3 py-1.5 bg-blue-50 border border-blue-100 rounded-lg text-blue-600 font-bold">
                Page {currentPage} of {totalPages}
              </span>
              <button 
                onClick={() => setCurrentPage(prev => Math.min(prev + 1, totalPages))}
                disabled={currentPage === totalPages || loading}
                className="px-3 py-1.5 bg-white border border-slate-200 rounded-lg hover:bg-slate-50 text-slate-700 disabled:opacity-50 disabled:cursor-not-allowed"
              >
                Next
              </button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
};
