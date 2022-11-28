namespace TaxComputingProject.Model;

public class AccumulatedSalary
{
    public AccumulatedSalary(List<MonthSalary> salary)
    {
        Salary = salary;
    }

    public AccumulatedSalary()
    {
    }

    public List<MonthSalary> Salary { get; set; }
}