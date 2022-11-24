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

    public double ComputeTaxBySalaryAndMonth(List<MonthSalary> salaries, int month)
    {
        string email = GetEmail();
        UserTax? userTax = _userDao.GetUserTax(email);
        var salariesOrdered = salaries.OrderBy(s => s.Month).ToList();
        salariesOrdered.DistinctBy(s => s.Month);
        if (userTax == null)
        {
            FirstComputeTaxOfMonth(salariesOrdered);
            SaveRecord(salariesOrdered);
            return salariesOrdered.Where(s => s.Month == month).Select(s => s.Tax).FirstOrDefault();
        }

        List<TaxOfMonth> existedTaxOfMonths = userTax.Taxes.ToList();
        int existedCount = userTax.Taxes.ToList().Count();
        double existedTaxableSalary = existedTaxOfMonths.Select(s => s.Salary).Sum() - existedCount * SalaryThreshold;
        double existedTax = existedTaxOfMonths.Select(s => s.Tax).Sum();

        int existedMaxMonth = existedTaxOfMonths.Select(s => s.Month).Max();
        List<MonthSalary> filteredSalaries = salariesOrdered.Where(s => s.Month > existedMaxMonth).ToList();

        LaterComputeTaxOfMonth(filteredSalaries, existedTaxableSalary, existedTax);
        SaveRecord(filteredSalaries);
        return GetTaxOfMonth(month);
    }

    private void LaterComputeTaxOfMonth(List<MonthSalary> monthSalariesFiltered, double existedTaxableSalary, double existedTax)
    {
        for (int count = 0; count < monthSalariesFiltered.Count; count++)
        {
            double taxableSalary = 0;
            taxableSalary += existedTaxableSalary;
            for (int pre = 0; pre <= count; pre++)
            {
                taxableSalary += monthSalariesFiltered[pre].Salary;
                taxableSalary -= SalaryThreshold;
            }

            TaxLevel taxLevel = MatchTaxRateAndDeductionBySalary(taxableSalary);
            double tax = taxableSalary * taxLevel.TaxRate - taxLevel.Deduction;
            double preTaxes = monthSalariesFiltered.Take(count).Select(monthSalary => monthSalary.Tax).Sum();
            tax -= preTaxes;
            tax -= existedTax;
            monthSalariesFiltered[count].Tax = tax;
        }
    }

    private void FirstComputeTaxOfMonth(List<MonthSalary> salariesOrdered)
    {
        for (int count = 0; count < salariesOrdered.Count; count++)
        {
            double taxableSalary = 0;
            for (int pre = 0; pre <= count; pre++)
            {
                taxableSalary += salariesOrdered[pre].Salary;
                taxableSalary -= SalaryThreshold;
            }

            TaxLevel taxLevel = MatchTaxRateAndDeductionBySalary(taxableSalary);
            double tax = taxableSalary * taxLevel.TaxRate - taxLevel.Deduction;
            double preTaxes = salariesOrdered.Take(count).Select(monthSalary => monthSalary.Tax).Sum();
            tax -= preTaxes;
            salariesOrdered[count].Tax = tax;
        }
    }

    private void SaveRecord(List<MonthSalary> salaries)
    {
        string email = GetEmail();
        UserTax? userTax = _userDao.GetUserTax(email);
        if (userTax == null)
        {
            userTax = CreateUserTax(salaries, email);
            _userDao.AddUserTax(userTax);
        }
        else
        {
            foreach (var monthSalary in salaries)
            {
                AddTaxItems(userTax.Taxes, monthSalary);
            }
        }
        _userDao.SaveChanges();
    }

    private static void AddTaxItems(List<TaxOfMonth> taxes, MonthSalary monthSalary)
    {
        taxes.Add(new TaxOfMonth
        {
            Month = monthSalary.Month,
            Salary = monthSalary.Salary,
            Tax = monthSalary.Tax
        });
    }
    private static UserTax CreateUserTax(List<MonthSalary> salaries, string email)
    {
        var userTax = new UserTax
        {
            Email = email
        };
        List<TaxOfMonth> taxes = new List<TaxOfMonth>();
        foreach (var monthSalary in salaries)
        {
            AddTaxItems(taxes, monthSalary);
        }

        userTax.Taxes = taxes;
        return userTax;
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
            <= (double)TotalSalary.FirstLevel => new TaxLevel(TaxRates.FirstLevel, (double)Deduction.FirstLevel),
            > (double)TotalSalary.FirstLevel and <= (double)TotalSalary.SecondLevel => new TaxLevel(
                TaxRates.SecondLevel, (double)Deduction.SecondLevel),
            > (double)TotalSalary.SecondLevel and <= (double)TotalSalary.ThirdLevel => new TaxLevel(TaxRates.ThirdLevel,
                (double)Deduction.ThirdLevel),
            > (double)TotalSalary.ThirdLevel and <= (double)TotalSalary.FourthLevel => new TaxLevel(
                TaxRates.FourthLevel, (double)Deduction.FourthLevel),
            > (double)TotalSalary.FourthLevel and <= (double)TotalSalary.FifthLevel => new TaxLevel(TaxRates.FifthLevel,
                (double)Deduction.FifthLevel),
            > (double)TotalSalary.FifthLevel and <= (double)TotalSalary.SixthLevel => new TaxLevel(TaxRates.SixthLevel,
                (double)Deduction.SixthLevel),
            _ => new TaxLevel(TaxRates.SeventhLevel, (double)Deduction.SeventhLevel)
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