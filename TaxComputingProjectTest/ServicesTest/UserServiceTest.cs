using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using TaxComputingProject.Dao;
using TaxComputingProject.DBContext;
using TaxComputingProject.Model;
using TaxComputingProject.Services;
using TaxComputingProjectTest.MockData;

namespace TaxComputingProjectTest.ServicesTest;

public class UserServiceTest
{
    private readonly DataContext _context;

    public UserServiceTest()
    {
        DbContextOptionsBuilder dbOptions = new DbContextOptionsBuilder()
            .UseInMemoryDatabase(
                Guid.NewGuid().ToString()
            );
        _context = new DataContext(dbOptions.Options);
    }

    [Fact]
    public void Should_Return_Empty_String_When_User_Existed_Already()
    {
        var userService = SetupService();
        var userRegisterRequest = new UserRegisterRequest
        {
            Email = "Tom@email.com",
            Password = "password",
            ConfirmPassword = "password"
        };
        Assert.Throws<Exception>(() => userService.AddUser(userRegisterRequest));
    }

    [Fact]
    public void Should_Return_String_NotEmpty_When_User_Not_Existed()
    {
        var userService = SetupService();
        var userRegisterRequest = new UserRegisterRequest
        {
            Email = "Henry@email.com",
            Password = "password",
            ConfirmPassword = "password"
        };
        var result = userService.AddUser(userRegisterRequest);
        Assert.True(result.Length != 0);
    }

    [Fact]
    public void Should_Return_True_When_Verified_Correctly()
    {
        var userService = SetupService();
        var result = userService.AddVerify("testToken");
        Assert.True(result);
    }

    [Fact]
    public void Should_Return_False_When_Login_User_Not_Existed()
    {
        var userService = SetupService();
        var userLoginRequest = new UserLoginRequest
        {
            Email = "Hello@example.email",
            Password = "password"
        };
        Assert.Throws<Exception>(() => userService.UserLogin(userLoginRequest));
    }

    [Fact]
    public void Should_Return_NotNull_When_Find_User_By_Updated_Email()
    {
        var userDao = new UserDaoImpl(_context);
        var userService = new UserServiceImpl(userDao, MockConfiguration());
        UserRegisterRequest user = new UserRegisterRequest
        {
            Email = "initial@example.com",
            Phone = "13812344321",
            Job = "teacher",
            Address = "Xi'an",
            Password = "123456789",
            ConfirmPassword = "123456789"
        };
        userService.AddUser(user);
        UserInfo userInfo = new UserInfo()
        {
            Email = "Updated@example.com",
            Address = "New York",
            Job = "doctor",
            Phone = "15524367856"
        };
        userService.UserUpdate(1, userInfo);
        User? result = userDao.FindUserByEmail(userInfo.Email);
        Assert.NotNull(result);
    }
    
    [Fact]
    public void Should_Throw_Exception_When_Updated_Email_Have_Already_Existed()
    {
        var userDao = new UserDaoImpl(_context);
        var userService = new UserServiceImpl(userDao, MockConfiguration());
        var firstUser = new UserRegisterRequest
        {
            Email = "initial@example.com",
            Phone = "13812344321",
            Job = "teacher",
            Address = "Xi'an",
            Password = "123456789",
            ConfirmPassword = "123456789"
        };
        var secondUser = new UserRegisterRequest
        {
            Email = "same@example.com",
            Phone = "13812344321",
            Job = "teacher",
            Address = "Xi'an",
            Password = "123456789",
            ConfirmPassword = "123456789"
        };
        userService.AddUser(firstUser);
        userService.AddUser(secondUser);
        var userInfo = new UserInfo
        {
            Email = "same@example.com",
            Address = "New York",
            Job = "doctor",
            Phone = "15524367856"
        };
        Assert.Throws<Exception>(() => userService.UserUpdate(1, userInfo));
    }

    private UserServiceImpl SetupService()
    {
        var mockContext = MockDbContext();
        var userDaoImpl = new UserDaoImpl(mockContext.Object);
        var configuration = MockConfiguration();
        var userServiceImpl = new UserServiceImpl(userDaoImpl, configuration);
        return userServiceImpl;
    }

    private static IConfiguration MockConfiguration()
    {
        var inMemorySettings = new Dictionary<string, string>
        {
            { "AppSettings:Token", "My Json Web Token Key" },
        };
        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();
        return configuration;
    }

    private Mock<DataContext> MockDbContext()
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