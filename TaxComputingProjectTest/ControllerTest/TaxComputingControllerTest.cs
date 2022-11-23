using Microsoft.AspNetCore.Mvc;
using Moq;
using TaxComputingProject.Controllers;
using TaxComputingProject.Services;

namespace TaxComputingProjectTest.ControllerTest;

public class TaxComputingControllerTest
{
    [Fact]
    public void Should_Return_Ok_When_Get_Tax_By_Month()
    {
        var mockService = new Mock<ITaxComputingService>();
        mockService.Setup(user => user.GetTaxOfMonth(It.IsAny<int>())).Returns(It.IsAny<double>());
        var taxComputingController = new TaxComputingController(mockService.Object);
        var result = taxComputingController.GetMonthOfTax(It.IsIn(1, 12));
        Assert.IsType<OkObjectResult>(result);
    }
}