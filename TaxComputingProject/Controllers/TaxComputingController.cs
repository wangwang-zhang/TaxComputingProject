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
    private readonly int _id;

    public TaxComputingController(ITaxComputingService taxComputingService)
    {
        _taxComputingService = taxComputingService;
        _id = HttpContextAccessorUtil.GetId();
    }

    [HttpPost("recordTax"), Authorize]
    public IActionResult SaveTaxByAccumulatedSalary([FromBody] List<MonthSalary> monthSalaries)
    {
        if (monthSalaries.Count == 0)
            return BadRequest(new { errorMessage = "The input is empty!" });
        try
        {
            _taxComputingService.ComputeAndSaveTax(_id, monthSalaries);
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
        try
        {
            var taxOfMonth = _taxComputingService.GetTaxOfMonth(_id, month);
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
        try
        {
            var taxRecords = _taxComputingService.GetAnnualTaxRecords(_id);
            return Ok(taxRecords);
        }
        catch (Exception e)
        {
            return BadRequest(new { errorMessage = e.Message });
        }
    }
}