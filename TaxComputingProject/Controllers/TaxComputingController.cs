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
        try
        {
            _taxComputingService.ComputeTaxBySalaryAndMonth(monthSalaries);
            return Ok("Saved taxes successfully!");
        }
        catch (ArgumentException e)
        {
            return BadRequest(new { errorMessage = e.Message });
        }
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
        try
        {
            var taxOfMonth = _taxComputingService.GetTaxOfMonth(month);
            return Ok(taxOfMonth);
        }
        catch (BadHttpRequestException e)
        {
            return BadRequest(new { errorMessage = e.Message });
        }
    }
}