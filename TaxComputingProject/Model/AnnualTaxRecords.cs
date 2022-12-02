namespace TaxComputingProject.Model;

public class AnnualTaxRecords
{
    public string Email { get; set; }
    public double TotalSalary { get; set; }
    public double TotalTax { get; set; }
    public List<MonthTax>? MonthTaxes{ get; set; }
}
