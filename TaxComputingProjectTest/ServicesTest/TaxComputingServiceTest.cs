using TaxComputingProject.Services;

namespace TaxComputingProjectTest.ServicesTest;

public class TaxComputingServiceTest
{
    private readonly ITaxComputingService _taxComputingService = new TaxComputingServiceImpl();

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
        var result = _taxComputingService.ComputeTaxBySalaryAndMonth(salary, 1);
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
        var result = _taxComputingService.MatchTaxRateAndDeductionBySalary(salary);
        Assert.Equal(taxRate, result.TaxRate);
        Assert.Equal(deduction,result.Deduction);
    }

    [Theory]
    [InlineData(41000, 2, 3600)]
    [InlineData(41000, 3, 3600)]
    [InlineData(41000, 4, 3600)]
    [InlineData(41000, 5, 7200)]
    [InlineData(41000, 6, 7200)]
    [InlineData(41000, 7, 7200)]
    [InlineData(41000, 8, 7200)]
    [InlineData(41000, 9, 8400)]
    [InlineData(41000, 10, 9000)]
    [InlineData(41000, 11, 9000)]
    [InlineData(41000, 12, 9600)]
    public void Should_Return_Correct_Tax_With_Multiple_Month_Of_Salary(double salary, int month, double tax)
    {
        var result = _taxComputingService.ComputeTaxBySalaryAndMonth(salary, month);
        Assert.Equal(tax, result);
    }
}