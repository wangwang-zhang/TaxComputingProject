using TaxComputingProject.Services;

namespace TaxComputingProjectTest.ServicesTest;

public class TaxComputingServiceTest
{
    private readonly ITaxComputingService _taxComputingService = new TaxComputingServiceImpl();

    [Theory]
    [InlineData(new double[]{5000}, 0)]
    [InlineData(new double[]{36000},930)]
    [InlineData(new double[]{41000},1080)]
    [InlineData(new double[]{149000},11880)]
    [InlineData(new double[]{305000},43080)]
    [InlineData(new double[]{425000},73080)]
    [InlineData(new double[]{665000},145080)]
    [InlineData(new double[]{965000},250080)]
    [InlineData(new double[]{1000000},265830)]
    public void Should_Return_Correct_Tax_With_One_Month_Of_Salary(double[] salary, double tax)
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
    [InlineData(new double[]{41000}, 1, 1080)]
    [InlineData(new double[]{41000, 113000}, 2, 10800)]
    [InlineData(new double[]{41000, 113000, 161000}, 3, 31200)]
    [InlineData(new double[]{41000, 113000, 161000, 125000}, 4, 30000)]
    public void Should_Return_Correct_Tax_With_Multiple_Month_Of_Salary(double[] salary, int month, double tax)
    {
        var result = _taxComputingService.ComputeTaxBySalaryAndMonth(salary, month);
        Assert.Equal(tax, result);
    }
}