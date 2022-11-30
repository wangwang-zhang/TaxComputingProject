using TaxComputingProject.Model;

namespace TaxComputingProject.Services;

public interface ITaxComputingService
{
    public bool ComputeTaxBySalaryAndMonth(List<MonthSalary> salaries);
    public TaxLevel MatchTaxRateAndDeductionBySalary(double salary);
    public int GetId();
    public double GetTaxOfMonth(int month);
}