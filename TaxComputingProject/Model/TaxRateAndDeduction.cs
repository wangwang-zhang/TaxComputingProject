namespace TaxComputingProject.Model;

public class TaxRateAndDeduction
{
    public double TaxRate { get; }
    public double Deduction { get; }

    public TaxRateAndDeduction(double taxRate, double deduction)
    {
        TaxRate = taxRate;
        Deduction = deduction;
    }
}