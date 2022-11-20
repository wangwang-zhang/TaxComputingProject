using TaxComputingProject.Model;

namespace TaxComputingProject.Services;

public interface ITaxComputingService
{
    public double ComputeTaxBySalaryAndMonth(double[] salaries, int month);
    public TaxLevel MatchTaxRateAndDeductionBySalary(double salary);
    public string GetEmail();
}