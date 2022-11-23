using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace TaxComputingProject.Model;

public class MonthSalary
{
    public int Month { get; set; }
    public double Salary { get; set; }
    
    [JsonIgnore]
    public double Tax { get; set; } = 0;
}