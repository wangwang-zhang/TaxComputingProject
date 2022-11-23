namespace TaxComputingProject.Model;

public class AccumulatedSalary
{
    public AccumulatedSalary(List<MonthSalary> salary, int month)
    {
        Salary = salary;
        Month = month;
    }

    public AccumulatedSalary()
    {
    }

    public List<MonthSalary> Salary { get; set; }
    public int Month { get; set; } = 0;
}