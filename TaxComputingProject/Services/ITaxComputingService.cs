using TaxComputingProject.Model;

namespace TaxComputingProject.Services;

public interface ITaxComputingService
{
    public void ComputeAndSaveTax(int id, List<MonthSalary> salaries);
    public TaxLevel MatchTaxRateAndDeductionBySalary(double salary);
    public double GetTaxOfMonth(int id, int month);
    public AnnualTaxRecords? GetAnnualTaxRecords(int id);
}