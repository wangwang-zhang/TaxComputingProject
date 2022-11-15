using Microsoft.EntityFrameworkCore;
using TaxComputingProject.Model;

namespace TaxComputingProject.DBContext;

public class DataContext : DbContext
{
    public DataContext(DbContextOptions<DataContext> options) : base(options)
    {
    }
    public DataContext(){}
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        optionsBuilder.UseMySql("Server=Localhost;Database=TaxDB;user=root;pwd=123456789",
            new MySqlServerVersion("8.0.29"));
    }
    public virtual DbSet<User> Users => Set<User>();
}