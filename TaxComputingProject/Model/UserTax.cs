namespace TaxComputingProject.Model;
public class UserTax
{
    public int Id { get; set; }
    public string Email { get; set; }
    public virtual List<TaxOfMonth> Taxes { get; set; }
}