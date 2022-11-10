using TaxComputingProject.Services;

namespace TaxComputingProjectTest.ServicesTest;

public class TaxComputingServiceTest
{
    private readonly ITaxComputingService _taxComputingService = new TaxComputingServiceImpl();

    [Theory]
    [InlineData(new Double[]{3000, 6000, 15000, 30000}, 1)]
    public void Should_Return_Correct_Tax_With_One_Month_Of_Salary(Double[] salaries, int month)
    {
        var ResultOfCaseOne = _taxComputingService.ComputeTaxBySalaryAndMonth(salaries[0], month);
        var ResultOfCaseTwo = _taxComputingService.ComputeTaxBySalaryAndMonth(salaries[1], month);
        var ResultOfCaseThree = _taxComputingService.ComputeTaxBySalaryAndMonth(salaries[2], month);
        var ResultOfCaseFour = _taxComputingService.ComputeTaxBySalaryAndMonth(salaries[3], month);
        Assert.Equal(0, ResultOfCaseOne);
        Assert.Equal(30, ResultOfCaseTwo);
        Assert.Equal(300, ResultOfCaseThree);
        Assert.Equal(750, ResultOfCaseFour);
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