using TaxComputingProject.Model;

namespace TaxComputingProject.Services;

public interface ITaxComputingService
{
    public void ComputeAndSaveTax(List<MonthSalary> salaries);
    public TaxLevel MatchTaxRateAndDeductionBySalary(double salary);
    public double GetTaxOfMonth(int month);
    public AnnualTaxRecords? GetAnnualTaxRecords();
}