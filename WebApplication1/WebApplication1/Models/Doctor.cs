namespace WebApplication1.Models;

public class Doctor
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public ICollection<Prescription> Prescriptions { get; set; } = new List<Prescription>();
}