namespace TaxComputingProject.Services;

public interface ITaxComputingService
{
    public double ComputeTaxBySalaryAndMonth(double salary, int month);
    public double[] MatchTaxRateAndDeductionBySalary(double salary);
}