namespace TaxComputingProject.Services;

public class TaxComputingServiceImpl : ITaxComputingService
{
    public double ComputeTaxBySalaryAndMonth(double salary, int month)
    {
        if (salary <= 5000)
            return 0;
        double tax = (salary * month - 5000 * month) * 0.03;
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