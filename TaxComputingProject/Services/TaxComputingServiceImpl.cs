namespace TaxComputingProject.Services;

public class TaxComputingServiceImpl : ITaxComputingService
{
    const int SalaryThreshold = 5000;
    public double ComputeTaxBySalaryAndMonth(double salary, int month)
    {
        if (salary <= SalaryThreshold)
            return 0;
        double tax = (salary * month - SalaryThreshold * month) * 0.03;
        return tax;
    }

    public double MatchTaxRateByTotalSalaryRemovedSalaryThreshold(double salary)
    {
        return salary switch
        {
            <= 36000 => 0.03,
            > 36000 and <= 144000 => 0.1,
            > 144000 and <= 300000 => 0.2,
            > 300000 and <= 420000 => 0.25,
            > 420000 and <= 660000 => 0.3,
            > 660000 and <= 960000 => 0.35,
            _ => 0.45
        };
    }
}