using System.Security.Claims;
using TaxComputingProject.Dao;
using TaxComputingProject.Model;

namespace TaxComputingProject.Services;

public class TaxComputingServiceImpl : ITaxComputingService
{
    private readonly IUserDao _userDao;
    private readonly IHttpContextAccessor _httpContextAccessor;
    const int SalaryThreshold = 5000;

    public TaxComputingServiceImpl(IHttpContextAccessor httpContextAccessor, IUserDao userDao)
    {
        _httpContextAccessor = httpContextAccessor;
        _userDao = userDao;
    }

    public double ComputeTaxBySalaryAndMonth(double[] salaries, int month)
    {
        double[] taxPerMonth = new double[salaries.Length + 1];
        taxPerMonth[0] = 0;
        for (int monthForTax = 1; monthForTax <= salaries.Length; monthForTax++)
        {
            double salaryForTax = 0;
            for (int preMonth = 1; preMonth <= monthForTax; preMonth++)
            {
                salaryForTax += salaries[preMonth - 1];
            }
            salaryForTax -= SalaryThreshold * monthForTax;
            TaxLevel taxLevel = MatchTaxRateAndDeductionBySalary(salaryForTax);
            double tax = salaryForTax * taxLevel.TaxRate - taxLevel.Deduction;
            for (int preMonth = 0; preMonth < monthForTax; preMonth++)
            {
                tax -= taxPerMonth[preMonth];
            }
            taxPerMonth[monthForTax] = tax;
        }
        SaveRecord(salaries,taxPerMonth);
        return taxPerMonth[month];
    }

    private bool SaveRecord(double[] salaries, double[] taxPerMonth)
    {
        string email = GetEmail();
        UserTax? userTax = _userDao.GetUserTax(email);
        if (userTax == null)
        {
            userTax = new UserTax();
            userTax.Email = email;
            List<TaxOfMonth> taxes = new List<TaxOfMonth>();
            for (int month = 1; month <= salaries.Length; month++)
            {
               taxes.Add(new TaxOfMonth
               {
                   Month = month,
                   Salary = salaries[month - 1],
                   Tax = taxPerMonth[month]
               }); 
            }
            userTax.Taxes = taxes;
            _userDao.AddUserTax(userTax);
            _userDao.SaveChanges();
        }
        return true;
    }
    public double GetTaxOfMonth(int month)
    {
        string email = GetEmail();
        UserTax? userTax = _userDao.GetUserTax(email);
        TaxOfMonth? taxOfMonth = userTax?.Taxes.FirstOrDefault(tax => tax.Month == month);
        if (taxOfMonth != null)
        {
            var tax = taxOfMonth.Tax;
            return tax;
        }
        return -1;
    } 
    public TaxLevel MatchTaxRateAndDeductionBySalary(double salary)
    {
        return salary switch
        {
            <= (double)TotalSalary.FirstLevel => new TaxLevel(TaxRates.FirstLevel,(double)Deduction.FirstLevel),
            > (double)TotalSalary.FirstLevel and <= (double)TotalSalary.SecondLevel => new TaxLevel(TaxRates.SecondLevel,(double)Deduction.SecondLevel),
            > (double)TotalSalary.SecondLevel and <= (double)TotalSalary.ThirdLevel => new TaxLevel(TaxRates.ThirdLevel,(double)Deduction.ThirdLevel),
            > (double)TotalSalary.ThirdLevel and <= (double)TotalSalary.FourthLevel => new TaxLevel(TaxRates.FourthLevel,(double)Deduction.FourthLevel),
            > (double)TotalSalary.FourthLevel and <= (double)TotalSalary.FifthLevel => new TaxLevel(TaxRates.FifthLevel,(double)Deduction.FifthLevel),
            > (double)TotalSalary.FifthLevel and <= (double)TotalSalary.SixthLevel => new TaxLevel(TaxRates.SixthLevel,(double)Deduction.SixthLevel),
            _ => new TaxLevel(TaxRates.SeventhLevel,(double)Deduction.SeventhLevel)
        };
    }
    public string GetEmail()
    {
        var result = string.Empty;
        if (_httpContextAccessor.HttpContext != null)
        {
            result = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Email);
        }
        return result;
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