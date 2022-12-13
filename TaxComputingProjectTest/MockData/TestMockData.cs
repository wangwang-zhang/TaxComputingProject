using TaxComputingProject.Model;

namespace TaxComputingProjectTest.MockData;

public static class TestMockData
{
    public static readonly IQueryable<User> Users = new List<User>
    {
        new()
        {
            Id = 1, Email = "Tom@email.com", PasswordHash = new byte[32], PasswordSalt = new byte[32],
            VerificationToken = "testActivationCode",
            VerifiedAt = null
        },
        new()
        {
            Id = 2, Email = "Amy@email.com", PasswordHash = new byte[32], PasswordSalt = new byte[32],
            VerificationToken = "testActivationCodeTwo",
            VerifiedAt = null
        },
        new()
        {
            Id = 3, Email = "Bob@email.com", PasswordHash = new byte[32], PasswordSalt = new byte[32],
            VerificationToken = "testActivationCodeThree",
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
    
    public static readonly List<MonthSalary> MonthSalariesWithDuplicateMonths = new List<MonthSalary>
    {
        new()
        {
            Month = 1,
            Salary = 41000,
            Tax = 1080
        },
        new()
        {
            Month = 1,
            Salary = 41000,
            Tax = 1080
        }
    };
    
    public static readonly UserRegisterRequest MockRegisterUser = new()
    {
        Email = "initial@example.com",
        Phone = "13812344321",
        Job = "teacher",
        Address = "Xi'an",
        Password = "123456789",
        ConfirmPassword = "123456789"
    };
    
    public static readonly UserInfo UserUpdateMockInfo = new()
    {
        Email = "Updated@example.com",
        Address = "New York",
        Job = "doctor",
        Phone = "15524367856"
    };
}