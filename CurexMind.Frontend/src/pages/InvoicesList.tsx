import React, { useEffect, useState } from 'react';
import { apiClient } from '../api/apiClient';
import { FileText, Loader2, AlertCircle, Plus, DollarSign, Download } from 'lucide-react';
import { Link } from 'react-router-dom';

interface Invoice {
  id: string;
  patientName: string;
  doctorName: string;
  amountEGP: number;
  invoiceDate: string;
  status: string;
}

export const InvoicesList: React.FC = () => {
  const [invoices, setInvoices] = useState<Invoice[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    const fetchInvoices = async () => {
      try {
        setLoading(true);
        setError(null);
        const response = await apiClient.get('/api/invoices');
        setInvoices(response.data || []);
      } catch (err: any) {
        console.error('Failed to fetch invoices:', err);
        setError('تعذر تحميل كشف الفواتير من الخادم. يرجى التحقق من اتصالك بالشبكة.');
      } finally {
        setLoading(false);
      }
    };

    fetchInvoices();
  }, []);

  return (
    <div className="space-y-6 text-left" dir="ltr">
      <div className="flex flex-col sm:flex-row justify-between items-start sm:items-center gap-4">
        <div>
          <h1 className="text-2xl font-bold text-slate-900 font-cairo">Billing & Invoices</h1>
          <p className="text-xs text-slate-500 font-cairo">Issue and manage patient bills, consult fee logs, and payment statuses</p>
        </div>
        <Link 
          to="/management/invoices/add" 
          className="w-full sm:w-auto bg-amber-600 hover:bg-amber-700 text-white text-xs font-bold px-4 py-2.5 rounded-xl shadow-md transition-all flex items-center justify-center gap-2"
        >
          <Plus className="h-4 w-4" />
          <span>Create Invoice</span>
        </Link>
      </div>

      {loading ? (
        <div className="flex flex-col items-center justify-center min-h-[300px] gap-3 bg-white border border-slate-200 rounded-2xl">
          <Loader2 className="h-8 w-8 text-amber-600 animate-spin" />
          <p className="text-slate-500 text-xs font-semibold">Loading billing registry...</p>
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
                  <th className="p-4 text-left font-semibold">Invoice ID</th>
                  <th className="p-4 text-left font-semibold">Patient</th>
                  <th className="p-4 text-left font-semibold">Amount</th>
                  <th className="p-4 text-left font-semibold">Date</th>
                  <th className="p-4 text-left font-semibold">Status</th>
                  <th className="p-4 text-right font-semibold">Action</th>
                </tr>
              </thead>
              <tbody className="divide-y divide-slate-50 text-slate-700">
                {invoices.length === 0 ? (
                  <tr>
                    <td colSpan={6} className="text-center py-8 text-slate-400">
                      No invoices issued yet.
                    </td>
                  </tr>
                ) : (
                  invoices.map((inv) => {
                    let statusClass = "bg-slate-100 text-slate-700 border-slate-200";
                    if (inv.status === 'Paid') statusClass = "bg-emerald-50 text-emerald-700 border-emerald-100";
                    else if (inv.status === 'Partial') statusClass = "bg-amber-50 text-amber-700 border-amber-100";
                    else if (inv.status === 'Unpaid') statusClass = "bg-rose-50 text-rose-700 border-rose-100";

                    return (
                      <tr key={inv.id} className="hover:bg-slate-50/50 transition-colors">
                        <td className="p-4 font-mono text-xs font-bold text-slate-500">
                          #{inv.id.substring(0, 8).toUpperCase()}
                        </td>
                        <td className="p-4">
                          <div className="flex flex-col gap-0.5">
                            <span className="font-bold text-slate-900 capitalize">{inv.patientName || 'Unknown Patient'}</span>
                            <span className="text-[10px] text-slate-400">Doctor: {inv.doctorName}</span>
                          </div>
                        </td>
                        <td className="p-4 font-bold text-slate-900">
                          {(inv.amountEGP || 0).toLocaleString()} EGP
                        </td>
                        <td className="p-4 text-xs text-slate-500 font-sans">
                          {new Date(inv.invoiceDate).toLocaleDateString('en-US', { dateStyle: 'medium' })}
                        </td>
                        <td className="p-4">
                          <span className={`px-2.5 py-1 rounded-full text-[10px] font-bold border uppercase ${statusClass}`}>
                            {inv.status || 'Unpaid'}
                          </span>
                        </td>
                        <td className="p-4 text-right">
                          <button 
                            className="p-1.5 text-slate-400 hover:text-slate-600 rounded-lg hover:bg-slate-50 transition-colors"
                            title="Export PDF"
                          >
                            <Download className="h-4 w-4" />
                          </button>
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
