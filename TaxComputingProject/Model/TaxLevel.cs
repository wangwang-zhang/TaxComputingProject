namespace TaxComputingProject.Model;

public class TaxLevel
{
    public double TaxRate { get; }
    public double Deduction { get; }

    public TaxLevel(double taxRate, double deduction)
    {
        TaxRate = taxRate;
        Deduction = deduction;
    }
}