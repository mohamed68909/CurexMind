import React from 'react';
import { Link } from 'react-router-dom';
import { ShieldAlert, Home } from 'lucide-react';

export const Unauthorized: React.FC = () => {
  return (
    <div className="min-h-screen flex flex-col justify-center items-center bg-slate-50 text-center px-4 font-sans" dir="rtl">
      <div className="bg-red-50 text-red-600 p-4 rounded-full mb-6">
        <ShieldAlert className="h-12 w-12" />
      </div>
      <h1 className="text-3xl font-extrabold text-slate-900 font-cairo mb-3">غير مصرح لك بالدخول!</h1>
      <p className="text-slate-500 max-w-md font-cairo mb-8 leading-relaxed">
        عذراً، الحساب الحالي لا يملك الصلاحيات الكافية لتصفح هذه الصفحة. يرجى تسجيل الدخول بحساب يملك الصلاحية المناسبة.
      </p>
      <Link 
        to="/" 
        className="bg-blue-600 hover:bg-blue-700 text-white font-bold px-6 py-3 rounded-xl transition-all shadow-md flex items-center gap-2 font-cairo"
      >
        <Home className="h-5 w-5" />
        <span>العودة للرئيسية</span>
      </Link>
    </div>
  );
};
