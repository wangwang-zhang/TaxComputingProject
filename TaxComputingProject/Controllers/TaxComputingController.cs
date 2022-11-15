using Microsoft.AspNetCore.Mvc;
using TaxComputingProject.Model;
using TaxComputingProject.Services;

namespace TaxComputingProject.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TaxComputingController : ControllerBase
{
    private readonly ITaxComputingService _taxComputingService;

    public TaxComputingController(ITaxComputingService taxComputingService)
    {
        _taxComputingService = taxComputingService;
    }

    [HttpGet("taxOfMonth")]
    public IActionResult GetTaxByAccumulatedSalary([FromHeader] AccumulatedSalary? accumulatedSalary)
    {
        if (accumulatedSalary == null)
            return BadRequest();
        double tax = _taxComputingService.ComputeTaxBySalaryAndMonth(accumulatedSalary.Salary, accumulatedSalary.Month);
        return Ok(tax);
    }
}