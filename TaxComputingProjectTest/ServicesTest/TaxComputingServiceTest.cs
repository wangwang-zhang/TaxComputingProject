using TaxComputingProject.Services;

namespace TaxComputingProjectTest.ServicesTest;

public class TaxComputingServiceTest
{
    private readonly ITaxComputingService _taxComputingService = new TaxComputingServiceImpl();

    [Theory]
    [InlineData(5000, 1, 0)]
    [InlineData(36000, 1, 930)]
    [InlineData(41000, 1, 1080)]
    public void Should_Return_Correct_Tax_With_One_Month_Of_Salary(Double salary, int month, double tax)
    {
        var result = _taxComputingService.ComputeTaxBySalaryAndMonth(salary, month);
        Assert.Equal(tax, result);
    }

    [Theory]
    [InlineData(36000, 0.03)]
    [InlineData(144000, 0.1)]
    [InlineData(300000, 0.2)]
    [InlineData(420000, 0.25)]
    [InlineData(660000, 0.3)]
    [InlineData(960000, 0.35)]
    [InlineData(1000000, 0.45)]
    public void Should_Return_Correct_Tax_Rate_By_Total_Salary_Removed_Threshold(double salary, double taxRate)
    {
        var result = _taxComputingService.MatchTaxRateByTotalSalaryRemovedSalaryThreshold(salary);
        Assert.Equal(taxRate, result);
        
    }
}