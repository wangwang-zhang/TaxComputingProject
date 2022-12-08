using TaxComputingProject.Dao;
using TaxComputingProject.Model;
using TaxComputingProject.Utils;

namespace TaxComputingProject.Services;

public class TaxComputingServiceImpl : ITaxComputingService
{
    private readonly IUserDao _userDao;
    
    const int SalaryThreshold = 5000;

    public TaxComputingServiceImpl( IUserDao userDao )
    {
        _userDao = userDao;
    }

    public void ComputeAndSaveTax(int id, List<MonthSalary> salaries)
    {
        JudgeRepetitionMonth(salaries);
        var prepareMonthSalaries = PrepareMonthSalaries(id, salaries);
        ComputeTaxOfMonth(prepareMonthSalaries);
        SaveRecord(id, prepareMonthSalaries);
    }

    private static void JudgeRepetitionMonth(List<MonthSalary> salaries)
    {
        var groupWithRepeatingMonth = salaries.GroupBy(monthSalary => monthSalary.Month)
            .Where(group => group.Count() > 1)
            .Select(group => group.Key)
            .ToList();
        if (groupWithRepeatingMonth.Count > 0)
        {
            throw new ArgumentException("There are duplicate months!");
        }
    }

    private List<MonthSalary> PrepareMonthSalaries(int id, List<MonthSalary> salaries)
    {
        var userTax = _userDao.GetUserTaxById(id);
        if (userTax == null)
        {
            return salaries.OrderBy(monthSalary => monthSalary.Month).ToList();
        }
        var taxOfMonthsInDatabase = userTax.Taxes.ToList();
        var monthsNewlyInput = salaries.Select(monthSalary => monthSalary.Month).ToList();
        taxOfMonthsInDatabase = monthsNewlyInput.Aggregate(taxOfMonthsInDatabase,
            (current, month) => current.Where(taxOfMonth => taxOfMonth.Month != month).ToList());

        salaries.AddRange(taxOfMonthsInDatabase.Select(existedItem =>
            new MonthSalary { Month = existedItem.Month, Salary = existedItem.Salary, Tax = 0 }));
        _userDao.RemoveTaxItem(userTax.Id);
        return salaries.OrderBy(monthSalary => monthSalary.Month).ToList();
    }

    private void ComputeTaxOfMonth(List<MonthSalary> monthSalaries)
    {
        for (int count = 0; count < monthSalaries.Count; count++)
        {
            double taxableSalary = 0;
            for (int pre = 0; pre <= count; pre++)
            {
                double salary = monthSalaries[pre].Salary;
                taxableSalary += salary;
                taxableSalary -= salary < SalaryThreshold ? salary : SalaryThreshold;
            }

            TaxLevel taxLevel = MatchTaxRateAndDeductionBySalary(taxableSalary);
            double tax = taxableSalary * taxLevel.TaxRate - taxLevel.Deduction;
            double preTaxes = monthSalaries.Take(count).Select(monthSalary => monthSalary.Tax).Sum();
            tax -= preTaxes;
            monthSalaries[count].Tax = MyRound(tax,2);
        }
    }

    private void SaveRecord(int id, List<MonthSalary> salaries)
    {
        var userTax = _userDao.GetUserTaxById(id);
        if (userTax == null)
        {
            userTax = CreateUserTax(salaries, id);
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

    public double GetTaxOfMonth(int id, int month)
    {
        var userTax = _userDao.GetUserTaxById(id);
        if (userTax == null)
        {
            throw new BadHttpRequestException("The user has no tax record");
        }
        var taxOfMonth = userTax.Taxes.FirstOrDefault(tax => tax.Month == month);
        if (taxOfMonth == null) throw new BadHttpRequestException("The month of tax is not existed!");
        {
            var tax = taxOfMonth.Tax;
            return tax;
        }
    }

    public AnnualTaxRecords? GetAnnualTaxRecords(int id)
    {
        var user = _userDao.GetUserById(id);
        if (user == null) return null;
        var userTax = _userDao.GetUserTaxById(id);
        if (userTax == null) return null;
        var totalSalary = userTax.Taxes.Select(tax => tax.Salary).Sum();
        var totalTax = userTax.Taxes.Select(tax => tax.Tax).Sum();
        var monthTaxes = userTax.Taxes
            .Select(taxOfMonth => new MonthTax { Month = taxOfMonth.Month, Salary = taxOfMonth.Salary, Tax = taxOfMonth.Tax }).ToList();
        var records = new AnnualTaxRecords
        {
            Email = user.Email,
            TotalSalary = totalSalary,
            TotalTax = MyRound(totalTax, 2),
            MonthTaxes = monthTaxes
        };
        return records;
    }

    private static double MyRound(double input, int digits)
    {
        return Math.Round(input, digits);
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