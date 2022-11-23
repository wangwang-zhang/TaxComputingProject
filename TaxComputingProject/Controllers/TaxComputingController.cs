using Microsoft.AspNetCore.Authorization;
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

    [HttpPost("taxOfMonth"), Authorize]
    public IActionResult SaveTaxByAccumulatedSalary([FromBody] AccumulatedSalary? accumulatedSalary)
    {
        if (accumulatedSalary == null)
            return BadRequest();
        double tax = _taxComputingService.ComputeTaxBySalaryAndMonth(accumulatedSalary.Salary, accumulatedSalary.Month);
        return Ok(tax);
    }
    
    [HttpGet("UserEmail"), Authorize]
    public ActionResult<string> GetUserEmail()
    {
        var userEmail = _taxComputingService.GetEmail();
        return Ok(userEmail);
    }

    [HttpGet("taxByMonth"), Authorize]
    public ActionResult GetMonthOfTax(int month)
    {
        var taxOfMonth = _taxComputingService.GetTaxOfMonth(month);
        return Ok(taxOfMonth);
    }
}