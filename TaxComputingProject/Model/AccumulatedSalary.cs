namespace TaxComputingProject.Model;

public class AccumulatedSalary
{
    public AccumulatedSalary(double[] salary, int month)
    {
        Salary = salary;
        Month = month;
    }
    public AccumulatedSalary()
    {
        
    }
    public double[] Salary { get; set; } = { 0 };
    public int Month { get; set; } = 0;
}