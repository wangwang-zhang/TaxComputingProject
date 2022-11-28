namespace TaxComputingProject.Model;

public class UserTax
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public List<TaxOfMonth> Taxes { get; set; }
}