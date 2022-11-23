using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TaxComputingProject.Model;

public class MonthSalary
{
    [Range(1, 12)]
    public int Month { get; set; }
    
    [Range(0, double.MaxValue, ErrorMessage = "The salary can't be less than 0.")]
    public double Salary { get; set; }
    
    [JsonIgnore]
    public double Tax { get; set; } = 0;
}