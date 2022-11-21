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
    [Fact]
    public void Should_Return_False_When_User_Existed_Already()
    {
        var userService = SetupService();
        var userRegisterRequest = new UserRegisterRequest
        {
            Email = "Tom@email.com",
            Password = "password",
            ConfirmPassword = "password"
        };
        var result = userService.AddUser(userRegisterRequest);
        Assert.False(result);
    }
    
    [Fact]
    public void Should_Return_True_When_User_Not_Existed()
    {
        var userService = SetupService();
        var userRegisterRequest = new UserRegisterRequest
        {
            Email = "Henry@email.com",
            Password = "password",
            ConfirmPassword = "password"
        };
        var result = userService.AddUser(userRegisterRequest);
        Assert.True(result);
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