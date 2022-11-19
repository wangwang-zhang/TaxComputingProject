namespace TaxComputingProject.Model;

public class AccumulatedSalary
{
    public AccumulatedSalary(double[] salary, int month)
    {
        Salary = salary;
        Month = month;
    }
    public double[] Salary { get; set; }
    public int Month { get; set; }
}