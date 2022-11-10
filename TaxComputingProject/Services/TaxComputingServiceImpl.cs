namespace TaxComputingProject.Services;

public class TaxComputingServiceImpl : ITaxComputingService
{
    public double ComputeTaxBySalaryAndMonth(double salary, int month)
    {
        if (salary <= 5000)
            return 0;
        double tax = (salary * month - 5000 * month) * 0.03;
        return tax;
    }
}