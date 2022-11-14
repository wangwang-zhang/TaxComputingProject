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
    public IActionResult GetTaxBySalaryAndMonth([FromHeader] SalaryAndMonth? salaryAndMonth)
    {
        if (salaryAndMonth == null)
            return BadRequest();
        double tax = _taxComputingService.ComputeTaxBySalaryAndMonth(salaryAndMonth.Salary, salaryAndMonth.Month);
        return Ok(tax);
    }
}