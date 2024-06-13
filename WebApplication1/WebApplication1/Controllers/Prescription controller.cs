using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.DTOs;
using WebApplication1.Models;

namespace WebApplication1.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PrescriptionsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public PrescriptionsController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<IActionResult> CreatePrescription([FromBody] PrescriptionDto prescriptionDto)
    {
        var patient = await _context.Patients.FindAsync(prescriptionDto.PatientId);
        if (patient == null)
        {
            patient = new Patient { FirstName = prescriptionDto.PatientFirstName, LastName = prescriptionDto.PatientLastName };
            _context.Patients.Add(patient);
        }

        var doctor = await _context.Doctors.FindAsync(prescriptionDto.DoctorId);
        if (doctor == null)
        {
            return BadRequest("Doctor not found");
        }

        var prescription = new Prescription
        {
            Date = prescriptionDto.Date,
            DueDate = prescriptionDto.DueDate,
            Patient = patient,
            Doctor = doctor
        };

        if (prescriptionDto.Medicaments.Count > 10)
        {
            return BadRequest("Cannot include more than 10 medicaments");
        }

        foreach (var med in prescriptionDto.Medicaments)
        {
            var medicament = await _context.Medicaments.FindAsync(med.Id);
            if (medicament == null)
            {
                return BadRequest($"Medicament with ID {med.Id} not found");
            }

            prescription.PrescriptionMedicaments.Add(new PrescriptionMedicament
            {
                Medicament = medicament,
                Dose = med.Dose
            });
        }

        _context.Prescriptions.Add(prescription);
        await _context.SaveChangesAsync();

        return Ok(prescription);
    }

    [HttpGet("{patientId}")]
    public async Task<IActionResult> GetPatientPrescriptions(int patientId)
    {
        var patient = await _context.Patients
            .Include(p => p.Prescriptions)
                .ThenInclude(pr => pr.Doctor)
            .Include(p => p.Prescriptions)
                .ThenInclude(pr => pr.PrescriptionMedicaments)
                    .ThenInclude(pm => pm.Medicament)
            .FirstOrDefaultAsync(p => p.Id == patientId);

        if (patient == null)
        {
            return NotFound();
        }

        return Ok(patient);
    }
}
