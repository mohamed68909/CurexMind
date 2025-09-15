
using ClincManagement.API.Abstractions;
using ClincManagement.API.Contracts.Patient.Requests;
using ClincManagement.API.Contracts.Patient.Respones;
using ClincManagement.API.Contracts.Patient.Responses;
using ClincManagement.API.Errors;
using ClincManagement.API.Services;
using ClincManagement.API.Services.Interface;
using Mapster;
using Microsoft.AspNetCore.Identity;
using System;

public class PatientService : IPatientService
{
    private readonly ApplicationDbContext _context;
    

    public PatientService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<PatientCreateResponseDto>> CreatePatientAsync(PatientRequestDto request)
    { 
        
    }



    public async Task<Result<IEnumerable<ResponseAllAppointmentPatient>>> GetAllAppointmentsByPatientIdAsync(Guid patientId)
    {
        var patient = await _context.Patients
            .Include(p => p.Appointments)
                .ThenInclude(a => a.Doctor)
            .Include(p => p.Invoice)
            .FirstOrDefaultAsync(p => p.PatientId == patientId);

        if (patient == null)
        {
            return Result.Failure<IEnumerable<ResponseAllAppointmentPatient>>(PatientErrors.NotFound);
        }

        if (patient.Appointments == null || !patient.Appointments.Any())
        {
            return Result.Failure<IEnumerable<ResponseAllAppointmentPatient>>(PatientErrors.NoAppointments);
        }

     
        var appointments = patient.Appointments.Adapt<IEnumerable<ResponseAllAppointmentPatient>>();

        return Result.Success(appointments);
    }





}
