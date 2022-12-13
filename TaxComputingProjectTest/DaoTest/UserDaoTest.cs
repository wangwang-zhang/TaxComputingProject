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

    private readonly User _testUser = new()
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

    private readonly UserTax _testUserTax = new()
    {
        UserId = 1,
        Taxes = new List<TaxOfMonth>
        {
            new() { Id = 1, Month = 1, Salary = 41000, Tax = 1080 },
            new() { Id = 2, Month = 2, Salary = 41000, Tax = 3600 },
            new() { Id = 3, Month = 3, Salary = 41000, Tax = 3600 },
            new() { Id = 4, Month = 4, Salary = 41000, Tax = 3600 },
            new() { Id = 5, Month = 5, Salary = 41000, Tax = 7200 },
        }
    };

    [Fact]
    public void Should_Return_NotNull_If_User_Exist_When_FindUserByEmail()
    {
        var mockContext = MockContext();
        var userDao = new UserDaoImpl(mockContext.Object);
        Assert.NotNull(userDao.FindUserByEmail("Tom@email.com"));
    }
    
    [Fact]
    public void Should_Return_Null_If_User_Not_Exist_When_FindUserByEmail()
    {
        var mockContext = MockContext();
        var userDao = new UserDaoImpl(mockContext.Object);
        Assert.Null(userDao.FindUserByEmail("Lucas@email.com"));
    }

    [Fact]
    public void Should_Return_Count_1_When_Added_One_User()
    {
        var userDao = new UserDaoImpl(_context);
        userDao.AddUser(_testUser);
        Assert.Equal(1, _context.Users.Count());
    }

    [Fact]
    public void Should_Return_Correct_User_When_Find_User_By_token()
    {
        var mockContext = MockContext();
        var userDao = new UserDaoImpl(mockContext.Object);
        var user = userDao.FindUserByToken("testActivationCode");
        Assert.Equal("Tom@email.com", user?.Email);
    }

    [Fact]
    public void Should_Return_Correct_Count_When_Add_One_UserTax()
    {
        var userDao = new UserDaoImpl(_context);
        userDao.AddUserTax(_testUserTax);
        Assert.Equal(1, _context.UserTaxes.Count());
    }

    [Fact]
    public void Should_Return_Correct_Tax_Count_When_GetUserTaxById()
    {
        var userDao = new UserDaoImpl(_context);
        userDao.AddUserTax(_testUserTax);
        UserTax? userTax = userDao.GetUserTaxById(1);
        Assert.Equal(5, userTax?.Taxes.Count);
    }

    [Fact]
    public void Should_Return_Empty_Taxes_When_Removed_Tax_Items_By_Id()
    {
        var userDao = new UserDaoImpl(_context);
        userDao.AddUserTax(_testUserTax);
        userDao.RemoveTaxItem(1);
        var userTax = userDao.GetUserTaxById(1);
        if (userTax != null) Assert.Empty(userTax.Taxes);
    }

    [Fact]
    public void Should_Return_NotNull_When_Find_User_By_Updated_Info()
    {
        UserDaoImpl userDao = new UserDaoImpl(_context);
        userDao.AddUser(_testUser);
        var userInfo = new UserInfo
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
        userDao.AddUser(_testUser);
        var userInfo = new UserInfo
        {
            Address = "New York",
            Job = "doctor",
            Phone = "15524367856"
        };
        userDao.UpdateUserInfo(1, userInfo);
        var result = _context.Users
            .Where(user => user.Email == _testUser.Email)
            .Where(user => user.Address == userInfo.Address)
            .Where(user => user.Job == userInfo.Job)
            .FirstOrDefault(user => user.Phone == userInfo.Phone);
        Assert.NotNull(result);
    }

    [Fact]
    public void Should_Return_NotNull_When_Find_User_By_Id_Successfully()
    {
        UserDaoImpl userDao = new UserDaoImpl(_context);
        userDao.AddUser(_testUser);
        var user = userDao.GetUserById(1);
        Assert.NotNull(user);
    }

    private static Mock<DataContext> MockContext()
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