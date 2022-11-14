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

    [HttpPost("taxOfMonth")]
    public IActionResult GetTaxBySalaryAndMonth([FromBody] SalaryAndMonth? salaryAndMonth)
    {
        if (salaryAndMonth == null)
            return BadRequest();
        double tax = _taxComputingService.ComputeTaxBySalaryAndMonth(salaryAndMonth.Salary, salaryAndMonth.Month);
        return Ok(tax);
    }
}