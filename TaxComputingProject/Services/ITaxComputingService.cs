using TaxComputingProject.Model;

namespace TaxComputingProject.Services;

public interface ITaxComputingService
{
    public double ComputeTaxBySalaryAndMonth(double salary, int month);
    public TaxLevel MatchTaxRateAndDeductionBySalary(double salary);
}