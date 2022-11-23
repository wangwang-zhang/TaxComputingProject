using TaxComputingProject.Model;

namespace TaxComputingProject.Services;

public interface ITaxComputingService
{
    public double ComputeTaxBySalaryAndMonth(List<MonthSalary> salaries, int month);
    public TaxLevel MatchTaxRateAndDeductionBySalary(double salary);
    public string GetEmail();
    public double GetTaxOfMonth(int month);
}