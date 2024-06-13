namespace WebApplication1.Models;

public class PrescriptionMedicament
{
    public int PrescriptionId { get; set; }
    public Prescription Prescription { get; set; }
    public int MedicamentId { get; set; }
    public Medicament Medicament { get; set; }
    public int Dose { get; set; }
}