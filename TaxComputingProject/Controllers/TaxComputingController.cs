using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaxComputingProject.Model;
using TaxComputingProject.Services;
using TaxComputingProject.Utils;

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

    [HttpPost("recordTax"), Authorize]
    public IActionResult SaveTaxByAccumulatedSalary([FromBody] List<MonthSalary> monthSalaries)
    {
        if (monthSalaries.Count == 0)
            return BadRequest(new { errorMessage = "The input is empty!" });
        try
        {
            var id = HttpContextAccessorUtil.GetId();
            _taxComputingService.ComputeAndSaveTax(id, monthSalaries);
            return Ok("Saved taxes successfully!");
        }
        catch (ArgumentException e)
        {
            return BadRequest(new { errorMessage = e.Message });
        }
    }

    [HttpGet("taxByMonth"), Authorize]
    public ActionResult GetMonthOfTax(int month)
    {
        if (month is < 1 or > 12)
        {
            return BadRequest(new { errorMessage = "Month is not valid!" });
        }

        try
        {
            var id = HttpContextAccessorUtil.GetId();
            var taxOfMonth = _taxComputingService.GetTaxOfMonth(id, month);
            return Ok(taxOfMonth);
        }
        catch (BadHttpRequestException e)
        {
            return BadRequest(new { errorMessage = e.Message });
        }
    }

    [HttpGet("AnnualTaxRecords"), Authorize]
    public ActionResult GetAnnualTaxRecords()
    {
        var id = HttpContextAccessorUtil.GetId();
        var taxRecords = _taxComputingService.GetAnnualTaxRecords(id);
        return Ok(taxRecords);
    }
}