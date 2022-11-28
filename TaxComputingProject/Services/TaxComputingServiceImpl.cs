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
        int id = GetId();
        UserTax? userTax = _userDao.GetUserTaxById(id);

        var salariesOrderedByMonth = salaries.OrderBy(monthSalary => monthSalary.Month).ToList();
        salariesOrderedByMonth = salariesOrderedByMonth.DistinctBy(monthSalary => monthSalary.Month).ToList();

        if (userTax == null)
        {
            return FirstSaveSalary(month, salariesOrderedByMonth);
        }

        return LaterSaveSalary(month, userTax, salariesOrderedByMonth);
    }

    private double LaterSaveSalary(int month, UserTax userTax, List<MonthSalary> salariesOrderedByMonth)
    {
        List<TaxOfMonth> existedTaxOfMonths = userTax.Taxes.ToList();
        int existedCount = userTax.Taxes.ToList().Count();
        double existedTaxableSalary = existedTaxOfMonths.Select(taxOfMonth => taxOfMonth.Salary).Sum() -
                                      existedCount * SalaryThreshold;
        double existedTax = existedTaxOfMonths.Select(taxOfMonth => taxOfMonth.Tax).Sum();

        int existedMaxMonth = existedTaxOfMonths.Select(taxOfMonth => taxOfMonth.Month).Max();
        List<MonthSalary> filteredSalaries =
            salariesOrderedByMonth.Where(monthSalary => monthSalary.Month > existedMaxMonth).ToList();

        ComputeTaxOfMonth(filteredSalaries, existedTaxableSalary, existedTax);
        SaveRecord(filteredSalaries);
        return GetTaxOfMonth(month);
    }

    private double FirstSaveSalary(int month, List<MonthSalary> salariesOrderedByMonth)
    {
        ComputeTaxOfMonth(salariesOrderedByMonth, 0, 0);
        SaveRecord(salariesOrderedByMonth);
        return salariesOrderedByMonth.Where(monthSalary => monthSalary.Month == month)
            .Select(monthSalary => monthSalary.Tax).FirstOrDefault();
    }

    private void ComputeTaxOfMonth(List<MonthSalary> monthSalaries, double existedTaxableSalary, double existedTax)
    {
        for (int count = 0; count < monthSalaries.Count; count++)
        {
            double taxableSalary = 0;
            taxableSalary += existedTaxableSalary;
            for (int pre = 0; pre <= count; pre++)
            {
                taxableSalary += monthSalaries[pre].Salary;
                taxableSalary -= SalaryThreshold;
            }

            TaxLevel taxLevel = MatchTaxRateAndDeductionBySalary(taxableSalary);
            double tax = taxableSalary * taxLevel.TaxRate - taxLevel.Deduction;
            double preTaxes = monthSalaries.Take(count).Select(monthSalary => monthSalary.Tax).Sum();
            tax -= preTaxes;
            tax -= existedTax;
            monthSalaries[count].Tax = tax;
        }
    }

    private void SaveRecord(List<MonthSalary> salaries)
    {
        int userid = GetId();
        UserTax? userTax = _userDao.GetUserTaxById(userid);
        if (userTax == null)
        {
            userTax = CreateUserTax(salaries, userid);
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

    private static UserTax CreateUserTax(List<MonthSalary> salaries, int userId)
    {
        var userTax = new UserTax
        {
            UserId = userId
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
        int userid = GetId();
        UserTax? userTax = _userDao.GetUserTaxById(userid);
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

    public int GetId()
    {
        string result = string.Empty;
        if (_httpContextAccessor.HttpContext != null)
        {
            result = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Sid);
        }
        int.TryParse(result, out var userId);
        return userId;
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