import React from 'react';
import { Routes, Route, Navigate } from 'react-router-dom';
import { ProtectedRoute } from '../components/ProtectedRoute';

// Layouts
import { ManagementLayout } from '../layouts/ManagementLayout';
import { PatientLayout } from '../layouts/PatientLayout';
import { AuthLayout } from '../layouts/AuthLayout';

// Pages
import { Landing } from '../pages/Landing';
import { Login } from '../pages/Login';
import { Register } from '../pages/Register';
import { Dashboard } from '../pages/Dashboard';
import { PatientsList } from '../pages/PatientsList';
import { AddPatientForm } from '../pages/AddPatientForm';
import { EditPatientForm } from '../pages/EditPatientForm';
import { PatientDetails } from '../pages/PatientDetails';
import { AppointmentsList } from '../pages/AppointmentsList';
import { AddAppointmentForm } from '../pages/AddAppointmentForm';
import { InvoicesList } from '../pages/InvoicesList';
import { AddInvoiceForm } from '../pages/AddInvoiceForm';
import { StaysList } from '../pages/StaysList';
import { AddStayForm } from '../pages/AddStayForm';
import { PatientHome } from '../pages/PatientHome';
import { PatientAppointments } from '../pages/PatientAppointments';
import { BookAppointment } from '../pages/BookAppointment';
import { DoctorProfile } from '../pages/DoctorProfile';
import { Payment } from '../pages/Payment';
import { RateDoctor } from '../pages/RateDoctor';
import { Unauthorized } from '../pages/Unauthorized';

export const AppRoutes: React.FC = () => {
  return (
    <Routes>
      {/* Public Pages */}
      <Route path="/" element={<Landing />} />
      <Route path="/unauthorized" element={<Unauthorized />} />

      {/* Guest/Auth Layout */}
      <Route element={<AuthLayout />}>
        <Route path="/login" element={<Login />} />
        <Route path="/register" element={<Register />} />
      </Route>

      {/* Receptionist/Admin Routes */}
      <Route element={<ProtectedRoute allowedRoles={['Receptionist', 'Admin']} />}>
        <Route path="/management" element={<ManagementLayout />}>
          <Route index element={<Dashboard />} />
          <Route path="patients" element={<PatientsList />} />
          <Route path="patients/add" element={<AddPatientForm />} />
          <Route path="patients/edit/:id" element={<EditPatientForm />} />
          <Route path="patients/:id" element={<PatientDetails />} />
          <Route path="appointments" element={<AppointmentsList />} />
          <Route path="appointments/add" element={<AddAppointmentForm />} />
          <Route path="invoices" element={<InvoicesList />} />
          <Route path="invoices/add" element={<AddInvoiceForm />} />
          <Route path="stays" element={<StaysList />} />
          <Route path="stays/add" element={<AddStayForm />} />
        </Route>
      </Route>

      {/* Patient Portal Routes */}
      <Route element={<ProtectedRoute allowedRoles={['Patient', 'Admin', 'Receptionist']} />}>
        <Route path="/patient" element={<PatientLayout />}>
          <Route index element={<PatientHome />} />
          <Route path="appointments" element={<PatientAppointments />} />
          <Route path="book/:doctorId" element={<BookAppointment />} />
          <Route path="doctors/:id" element={<DoctorProfile />} />
          <Route path="payment/:appointmentId" element={<Payment />} />
          <Route path="rate/:id" element={<RateDoctor />} />
        </Route>
      </Route>

      {/* Fallback to root */}
      <Route path="*" element={<Navigate to="/" replace />} />
    </Routes>
  );
};
