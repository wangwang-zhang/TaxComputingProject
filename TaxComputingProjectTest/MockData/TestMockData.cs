using TaxComputingProject.Model;

namespace TaxComputingProjectTest.MockData;

public static class TestMockData
{
    public static readonly IQueryable<User> Users = new List<User>
    {
        new()
        {
            Id = 1, Email = "Tom@email.com", PasswordHash = new byte[32], PasswordSalt = new byte[32],
            VerificationToken = "testToken",
            VerifiedAt = null
        },
        new()
        {
            Id = 2, Email = "Amy@email.com", PasswordHash = new byte[32], PasswordSalt = new byte[32],
            VerificationToken = "testTokenTwo",
            VerifiedAt = null
        },
        new()
        {
            Id = 3, Email = "Bob@email.com", PasswordHash = new byte[32], PasswordSalt = new byte[32],
            VerificationToken = "testTokenThree",
            VerifiedAt = null
        },
    }.AsQueryable();

    public static readonly IQueryable<UserTax> UserTaxes = new List<UserTax>
    {
        new()
        {
            Id = 1,
            UserId = 1,
            Taxes = new List<TaxOfMonth>
            {
                new() { Id = 1, Month = 1, Salary = 41000, Tax = 1080 },
                new() { Id = 2, Month = 2, Salary = 41000, Tax = 3600 },
                new() { Id = 3, Month = 3, Salary = 41000, Tax = 3600 },
                new() { Id = 4, Month = 4, Salary = 41000, Tax = 3600 },
                new() { Id = 5, Month = 5, Salary = 41000, Tax = 7200 },
            }
        }
    }.AsQueryable();
}