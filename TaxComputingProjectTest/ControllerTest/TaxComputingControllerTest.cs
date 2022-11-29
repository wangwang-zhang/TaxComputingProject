using Microsoft.AspNetCore.Mvc;
using Moq;
using TaxComputingProject.Controllers;
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

    [Fact]
    public void Should_Return_Ok_When_Get_MonthOfTax_Correctly()
    {
        var mockService = new Mock<ITaxComputingService>();
        mockService.Setup(user => user.GetTaxOfMonth(It.IsIn(1,12))).Returns(It.IsAny<double>());
        var taxComputingController = new TaxComputingController(mockService.Object);
        var result = taxComputingController.GetMonthOfTax(It.IsIn(1,12));
        Assert.IsType<OkObjectResult>(result);
    }
}