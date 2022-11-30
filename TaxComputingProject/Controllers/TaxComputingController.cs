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
    public IActionResult SaveTaxByAccumulatedSalary([FromBody] List<MonthSalary> monthSalaries)
    {
        if (monthSalaries.Count == 0)
            return BadRequest();
        var success = _taxComputingService.ComputeTaxBySalaryAndMonth(monthSalaries);
        if (success)
        {
            return Ok("Saved taxes successfully!");
        }

        return BadRequest(new { errorMessage = "There are duplicate months!" });
    }
    
    [HttpGet("UserId"), Authorize]
    public ActionResult<string> GetUserId()
    {
        var userId = _taxComputingService.GetId();
        return Ok(userId);
    }

    [HttpGet("taxByMonth"), Authorize]
    public ActionResult GetMonthOfTax(int month)
    {
        var taxOfMonth = _taxComputingService.GetTaxOfMonth(month);
        return Ok(taxOfMonth);
    }
}