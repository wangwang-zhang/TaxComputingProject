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
            <= (double)TotalSalary.FirstLevel => TaxRates.FirstLevel,
            > (double)TotalSalary.FirstLevel and <= (double)TotalSalary.SecondLevel => TaxRates.SecondLevel,
            > (double)TotalSalary.SecondLevel and <= (double)TotalSalary.ThirdLevel => TaxRates.ThirdLevel,
            > (double)TotalSalary.ThirdLevel and <= (double)TotalSalary.FourthLevel => TaxRates.FourthLevel,
            > (double)TotalSalary.FourthLevel and <= (double)TotalSalary.FifthLevel => TaxRates.FifthLevel,
            > (double)TotalSalary.FifthLevel and <= (double)TotalSalary.SixthLevel => TaxRates.SixthLevel,
            _ => TaxRates.SeventhLevel
        };
    }

    private enum TotalSalary
    {
        FirstLevel = 36000,
        SecondLevel = 144000,
        ThirdLevel = 300000,
        FourthLevel = 420000,
        FifthLevel = 660000,
        SixthLevel = 960000
    }

    struct TaxRates
    {
         public const double FirstLevel = 0.03;
         public const double SecondLevel = 0.1;
         public const double ThirdLevel = 0.2;
         public const double FourthLevel = 0.25;
         public const double FifthLevel = 0.3;
         public const double SixthLevel = 0.35;
         public const double SeventhLevel = 0.45;
    }
}