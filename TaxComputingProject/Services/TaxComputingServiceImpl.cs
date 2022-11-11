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

    public double[] MatchTaxRateAndDeductionBySalary(double salary)
    {
        return salary switch
        {
            <= (double)TotalSalary.FirstLevel => new []{TaxRates.FirstLevel, (double)Deduction.FirstLevel},
            > (double)TotalSalary.FirstLevel and <= (double)TotalSalary.SecondLevel => new []{TaxRates.SecondLevel,(double)Deduction.SecondLevel},
            > (double)TotalSalary.SecondLevel and <= (double)TotalSalary.ThirdLevel => new []{TaxRates.ThirdLevel, (double)Deduction.ThirdLevel},
            > (double)TotalSalary.ThirdLevel and <= (double)TotalSalary.FourthLevel => new []{TaxRates.FourthLevel, (double)Deduction.FourthLevel},
            > (double)TotalSalary.FourthLevel and <= (double)TotalSalary.FifthLevel => new []{TaxRates.FifthLevel, (double)Deduction.FifthLevel},
            > (double)TotalSalary.FifthLevel and <= (double)TotalSalary.SixthLevel => new []{TaxRates.SixthLevel, (double)Deduction.SixthLevel},
            _ => new []{TaxRates.SeventhLevel, (double)Deduction.SeventhLevel}
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
    private enum Deduction
    {
        FirstLevel = 0,
        SecondLevel = 2520,
        ThirdLevel = 16920,
        FourthLevel = 31920,
        FifthLevel = 52920,
        SixthLevel = 85920,
        SeventhLevel = 181920
    }
}