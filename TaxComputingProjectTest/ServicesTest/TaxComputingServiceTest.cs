using TaxComputingProject.Services;

namespace TaxComputingProjectTest.ServicesTest;

public class TaxComputingServiceTest
{
    private readonly ITaxComputingService _taxComputingService = new TaxComputingServiceImpl();

    [Theory]
    [InlineData(3000, 1, 0)]
    [InlineData(6000, 1, 30)]
    [InlineData(15000, 1, 300)]
    [InlineData(30000, 1, 750)]
    public void Should_Return_Correct_Tax_With_One_Month_Of_Salary(Double salary, int month, double tax)
    {
        var result = _taxComputingService.ComputeTaxBySalaryAndMonth(salary, month);
        Assert.Equal(tax, result);
    }

    [Theory]
    [InlineData(30000, 0.03)]
    [InlineData(45000, 0.1)]
    [InlineData(220000, 0.2)]
    [InlineData(350000, 0.25)]
    [InlineData(500000, 0.3)]
    [InlineData(800000, 0.35)]
    [InlineData(1000000, 0.45)]
    public void Should_Return_Correct_Tax_Rate_By_Total_Salary_Removed_Threshold(double salary, double taxRate)
    {
        var result = _taxComputingService.MatchTaxRateByTotalSalaryRemovedSalaryThreshold(salary);
        Assert.Equal(taxRate, result);
        
    }
}