using Microsoft.AspNetCore.Mvc;
using Moq;
using TaxComputingProject.Controllers;
using TaxComputingProject.Model;
using TaxComputingProject.Services;

namespace TaxComputingProjectTest.ControllerTest;

public class TaxComputingControllerTest
{
    [Fact]
    public void Should_Return_Ok_When_Get_UserId_Correctly()
    {
        var mockService = new Mock<ITaxComputingService>();
        mockService.Setup(user => user.GetId()).Returns(It.IsAny<int>());
        var taxComputingController = new TaxComputingController(mockService.Object);
        var result = taxComputingController.GetUserId();
        Assert.IsType<OkObjectResult>(result.Result);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(6)]
    [InlineData(12)]
    public void Should_Return_Ok_When_Get_MonthOfTax_Correctly(int month)
    {
        var mockService = new Mock<ITaxComputingService>();
        mockService.Setup(user => user.GetTaxOfMonth(It.IsIn(1,6,12))).Returns(It.IsAny<double>());
        var taxComputingController = new TaxComputingController(mockService.Object);
        var result = taxComputingController.GetMonthOfTax(month);
        Assert.IsType<OkObjectResult>(result);
    }
    
    [Fact]
    public void Should_Return_BadRequest_When_Request_Failed()
    {
        var mockService = new Mock<ITaxComputingService>();
        var taxComputingController = new TaxComputingController(mockService.Object);
        List<MonthSalary> monthSalaries = new List<MonthSalary>();
        var result = taxComputingController.SaveTaxByAccumulatedSalary(monthSalaries);
        Assert.IsType<BadRequestObjectResult>(result);
    }
    
    [Fact]
    public void Should_Return_Ok_When_Request_SaveTaxByAccumulatedSalary_Successfully()
    {
        var mockService = new Mock<ITaxComputingService>();
        var taxComputingController = new TaxComputingController(mockService.Object);
        List<MonthSalary> monthSalaries = new List<MonthSalary>
        {
            new()
            {
                Month = 1,
                Salary = 41000
            }
        };
        var result = taxComputingController.SaveTaxByAccumulatedSalary(monthSalaries);
        Assert.IsType<OkObjectResult>(result);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(13)]
    public void Should_Return_BadRequest_When_Request_Month_Not_Valid(int month)
    {
        var mockService = new Mock<ITaxComputingService>();
        var taxComputingController = new TaxComputingController(mockService.Object);
        var result = taxComputingController.GetMonthOfTax(month);
        Assert.IsType<BadRequestObjectResult>(result);
    }
    
    [Fact]
    public void Should_Return_OK_When_Get_AnnualTaxRecords_Successfully()
    {
        var mockService = new Mock<ITaxComputingService>();
        AnnualTaxRecords annualTaxRecords = new AnnualTaxRecords();
        mockService.Setup(tax => tax.GetAnnualTaxRecords()).Returns(annualTaxRecords);
        var taxComputingController = new TaxComputingController(mockService.Object);
        var result = taxComputingController.GetAnnualTaxRecords();
        Assert.IsType<OkObjectResult>(result);
    }
}