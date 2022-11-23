using Microsoft.AspNetCore.Http;
using Moq;
using TaxComputingProject.Dao;
using TaxComputingProject.Model;
using TaxComputingProject.Services;

namespace TaxComputingProjectTest.ServicesTest;

public class TaxComputingServiceTest
{
    [Theory]
    [InlineData(5000, 0)]
    [InlineData(36000,930)]
    [InlineData(41000,1080)]
    [InlineData(149000,11880)]
    [InlineData(305000,43080)]
    [InlineData(425000,73080)]
    [InlineData(665000,145080)]
    [InlineData(965000,250080)]
    [InlineData(1000000,265830)]
    public void Should_Return_Correct_Tax_With_One_Month_Of_Salary(double salary, double tax)
    {
        List<MonthSalary> testSalaries = new List<MonthSalary>
        {
            new()
            {
                Month = 1,
                Salary = salary
            }
        };
        var taxComputingService = MockService();
        var result = taxComputingService.ComputeTaxBySalaryAndMonth(testSalaries, 1);
        Assert.Equal(tax, result);
    }

    [Theory]
    [InlineData(36000, 0.03, 0)]
    [InlineData(144000, 0.1, 2520)]
    [InlineData(300000, 0.2, 16920)]
    [InlineData(420000, 0.25, 31920)]
    [InlineData(660000, 0.3, 52920)]
    [InlineData(960000, 0.35, 85920)]
    [InlineData(1000000, 0.45, 181920)]
    public void Should_Return_Correct_Tax_Rate_And_Deduction_By_Taxable_Salary(double salary, double taxRate, double deduction)
    {
        var taxComputingService = MockService();
        var result = taxComputingService.MatchTaxRateAndDeductionBySalary(salary);
        Assert.Equal(taxRate, result.TaxRate);
        Assert.Equal(deduction,result.Deduction);
    }

    [Theory]
    [InlineData(new double[]{41000}, 1, 1080)]
    [InlineData(new double[]{41000, 113000}, 2, 10800)]
    [InlineData(new double[]{41000, 113000, 161000}, 3, 31200)]
    [InlineData(new double[]{41000, 113000, 161000, 125000}, 4, 30000)]
    [InlineData(new double[]{41000, 113000, 161000, 125000, 245000}, 5, 72000)]
    [InlineData(new double[]{41000, 113000, 161000, 125000, 245000, 305000}, 6, 105000)]
    [InlineData(new double[]{41000, 113000, 161000, 125000, 245000, 305000, 45000}, 7, 18000)]
    public void Should_Return_Correct_Tax_With_Multiple_Month_Of_Salary(double[] salaries, int month, double tax)
    {
        List<MonthSalary> testSalaries = new List<MonthSalary>();
        for (int i = 0; i < salaries.Length; i++)
        {
            testSalaries.Add(new MonthSalary()
            {
                Month = i + 1,
                Salary = salaries[i]
            });
        }
        var taxComputingService = MockService();
        var result = taxComputingService.ComputeTaxBySalaryAndMonth(testSalaries, month);
        Assert.Equal(tax, result);
    }
    
    [Theory]
    [InlineData(1, 1080)]
    [InlineData(2, 3600)]
    [InlineData(5, 7200)]
    public void Should_Return_Correct_TaxOfMonth_By_Month(int month, double tax)
    {
        ITaxComputingService taxComputingService = MockService();
        double taxOfMonth = taxComputingService.GetTaxOfMonth(month);
        Assert.Equal(tax, taxOfMonth);
    }
    
    private static ITaxComputingService MockService()
    {
        var accessorMock = new Mock<IHttpContextAccessor>();
        var context = new DefaultHttpContext();
        accessorMock.Setup(a => a.HttpContext).Returns(context);
        Mock<UserDaoImpl> mockUserDao = new Mock<UserDaoImpl>();
        mockUserDao.Setup(user => user.GetUserTax(It.IsAny<string>())).Returns(UserTax);
        ITaxComputingService taxComputingService = new TaxComputingServiceImpl(accessorMock.Object, mockUserDao.Object);
        return taxComputingService;
    }
    
    private static readonly UserTax UserTax = new UserTax
    {
        Id = 1,
        Email = "Tom@email.com",
        Taxes = new List<TaxOfMonth>()
        {
            new TaxOfMonth() { Id = 1, Month = 1, Salary = 41000, Tax = 1080 },
            new TaxOfMonth() { Id = 2, Month = 2, Salary = 41000, Tax = 3600 },
            new TaxOfMonth() { Id = 3, Month = 3, Salary = 41000, Tax = 3600 },
            new TaxOfMonth() { Id = 4, Month = 4, Salary = 41000, Tax = 3600 },
            new TaxOfMonth() { Id = 5, Month = 5, Salary = 41000, Tax = 7200 },
        }
    };
}