using Microsoft.EntityFrameworkCore;
using Moq;
using TaxComputingProject.Dao;
using TaxComputingProject.DBContext;
using TaxComputingProject.Model;
using TaxComputingProjectTest.MockData;

namespace TaxComputingProjectTest.DaoTest;

public class UserDaoTest
{
    private readonly DataContext _context;

    public UserDaoTest()
    {
        DbContextOptionsBuilder dbOptions = new DbContextOptionsBuilder()
            .UseInMemoryDatabase(
                Guid.NewGuid().ToString()
            );
        _context = new DataContext(dbOptions.Options);
    }

    [Fact]
    public void Should_Return_True_When_User_Exist()
    {
        var mockContext = MockContext();
        var userDao = new UserDaoImpl(mockContext.Object);
        Assert.NotNull(userDao.FindUserByEmail("Tom@email.com"));
        Assert.Null(userDao.FindUserByEmail("Lucas@email.com"));
    }

    [Fact]
    public void Should_Return_Correct_Count_When_Add_One_User()
    {
        var userDao = new UserDaoImpl(_context);
        User user = new User
        {
            Email = "Hello2@example.com",
            Phone = "13812344321",
            Job = "teacher",
            Address = "Xi'an",
            PasswordHash = new byte[32],
            PasswordSalt = new byte[32],
            VerificationToken = "",
            VerifiedAt = null
        };
        userDao.AddUser(user);
        Assert.Equal(1, _context.Users.Count());
    }

    [Fact]
    public void Should_Return_Correct_User_When_Find_User_By_token()
    {
        var mockContext = MockContext();
        var userDao = new UserDaoImpl(mockContext.Object);
        var user = userDao.FindUserByToken("testToken");
        Assert.Equal("Tom@email.com", user?.Email);
    }

    [Fact]
    public void Should_Return_Correct_Count_When_Add_One_UserTax()
    {
        UserTax userTax = new()
        {
            UserId = 1,
            Taxes = new List<TaxOfMonth>()
            {
                new() { Id = 1, Month = 1, Salary = 41000, Tax = 1080 },
                new() { Id = 2, Month = 2, Salary = 41000, Tax = 3600 },
                new() { Id = 3, Month = 3, Salary = 41000, Tax = 3600 },
                new() { Id = 4, Month = 4, Salary = 41000, Tax = 3600 },
                new() { Id = 5, Month = 5, Salary = 41000, Tax = 7200 },
            }
        };
        var userDao = new UserDaoImpl(_context);
        userDao.AddUserTax(userTax);
        Assert.Equal(1, _context.UserTaxes.Count());
    }

    [Fact]
    public void Should_Return_Correct_UserTax_By_Id()
    {
        UserTax testUserTax = new()
        {
            UserId = 1,
            Taxes = new List<TaxOfMonth>()
            {
                new() { Id = 1, Month = 1, Salary = 41000, Tax = 1080 },
                new() { Id = 2, Month = 2, Salary = 41000, Tax = 3600 },
                new() { Id = 3, Month = 3, Salary = 41000, Tax = 3600 },
                new() { Id = 4, Month = 4, Salary = 41000, Tax = 3600 },
                new() { Id = 5, Month = 5, Salary = 41000, Tax = 7200 },
            }
        };
        var userDao = new UserDaoImpl(_context);
        userDao.AddUserTax(testUserTax);
        UserTax? userTax = userDao.GetUserTaxById(1);
        Assert.Equal(5, userTax?.Taxes.Count);
    }

    [Fact]
    public void Should_Return_Zero_When_Remove_Tax_Items_By_Id()
    {
        UserTax testUserTax = new()
        {
            UserId = 1,
            Taxes = new List<TaxOfMonth>()
            {
                new() { Id = 1, Month = 1, Salary = 41000, Tax = 1080 },
                new() { Id = 2, Month = 2, Salary = 41000, Tax = 3600 },
            }
        };
        var userDao = new UserDaoImpl(_context);
        userDao.AddUserTax(testUserTax);
        userDao.RemoveTaxItem(1);
        UserTax? userTax = userDao.GetUserTaxById(1);
        if (userTax != null) Assert.Empty(userTax.Taxes);
    }

    [Fact]
    public void Should_Return_Non_Empty_User_When_Find_User_By_Updated_Info()
    {
        UserDaoImpl userDao = new UserDaoImpl(_context);
        User testUser = new User
        {
            Email = "Hello@example.com",
            Phone = "13812344321",
            Job = "teacher",
            Address = "Xi'an",
            PasswordHash = new byte[32],
            PasswordSalt = new byte[32],
            VerificationToken = "",
            VerifiedAt = null
        };
        userDao.AddUser(testUser);
        UserInfo userInfo = new UserInfo()
        {
            Email = "test@example.com",
            Address = "New York",
            Job = "doctor",
            Phone = "15524367856"
        };
        userDao.UpdateUserInfo(1, userInfo);
        var result = _context.Users
            .Where(user => user.Email == userInfo.Email)
            .Where(user => user.Address == userInfo.Address)
            .Where(user => user.Job == userInfo.Job)
            .FirstOrDefault(user => user.Phone == userInfo.Phone);
        Assert.NotNull(result);
    }
    
    [Fact]
    public void Should_Still_Return_Non_Empty_User_When_Find_User_By_Previous_Email()
    {
        UserDaoImpl userDao = new UserDaoImpl(_context);
        User testUser = new User
        {
            Email = "Hello@example.com",
            Phone = "13812344321",
            Job = "teacher",
            Address = "Xi'an",
            PasswordHash = new byte[32],
            PasswordSalt = new byte[32],
            VerificationToken = "",
            VerifiedAt = null
        };
        userDao.AddUser(testUser);
        UserInfo userInfo = new UserInfo()
        {
            Address = "New York",
            Job = "doctor",
            Phone = "15524367856"
        };
        userDao.UpdateUserInfo(1, userInfo);
        var result = _context.Users
            .Where(user => user.Email == testUser.Email)
            .Where(user => user.Address == userInfo.Address)
            .Where(user => user.Job == userInfo.Job)
            .FirstOrDefault(user => user.Phone == userInfo.Phone);
        Assert.NotNull(result);
    }

    [Fact]
    public void Should_Return_User_When_Find_User_By_Id_Successfully()
    {
        UserDaoImpl userDao = new UserDaoImpl(_context);
        User testUser = new User
        {
            Email = "Hello@example.com",
            Phone = "13812344321",
            Job = "teacher",
            Address = "Xi'an",
            PasswordHash = new byte[32],
            PasswordSalt = new byte[32],
            VerificationToken = "",
            VerifiedAt = null
        };
        userDao.AddUser(testUser);
        User? user = userDao.GetUserById(1);
        Assert.NotNull(user);
    }
    
    private Mock<DataContext> MockContext()
    {
        var mockSet = new Mock<DbSet<User>>();
        mockSet.As<IQueryable<User>>().Setup(m => m.Provider).Returns(TestMockData.Users.Provider);
        mockSet.As<IQueryable<User>>().Setup(m => m.Expression).Returns(TestMockData.Users.Expression);
        mockSet.As<IQueryable<User>>().Setup(m => m.ElementType).Returns(TestMockData.Users.ElementType);
        mockSet.As<IQueryable<User>>().Setup(m => m.GetEnumerator()).Returns(() => TestMockData.Users.GetEnumerator());
        var mockContext = new Mock<DataContext>();
        mockContext.Setup(dataContext => dataContext.Users).Returns(mockSet.Object);
        return mockContext;
    }
}