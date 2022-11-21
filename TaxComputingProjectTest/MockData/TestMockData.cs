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
}