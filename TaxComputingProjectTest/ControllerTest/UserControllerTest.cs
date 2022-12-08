using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using TaxComputingProject.Controllers;
using TaxComputingProject.Dao;
using TaxComputingProject.DBContext;
using TaxComputingProject.Model;
using TaxComputingProject.Services;
using TaxComputingProjectTest.MockData;

namespace TaxComputingProjectTest.ControllerTest;

public class UserControllerTest
{
    [Fact]
    public void Should_Return_BadRequest_When_User_Existed_Already()
    {
        var userService = SetupService();
        var userRegisterRequest = new UserRegisterRequest
        {
            Email = "Tom@email.com",
            Password = "password",
            ConfirmPassword = "password"
        };
        var controller = new UserController(userService);
        var result = controller.Register(userRegisterRequest);
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void Should_Return_OK_When_User_Not_Existed()
    {
        var userService = SetupService();
        var userRegisterRequest = new UserRegisterRequest
        {
            Email = "Henry@email.com",
            Password = "password",
            ConfirmPassword = "password"
        };
        var controller = new UserController(userService);
        var result = controller.Register(userRegisterRequest);
        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public void should_Return_BadRequest_If_User_Existed_Already_When_User_Login()
    {
        var userService = SetupService();
        var userLoginRequest = new UserLoginRequest
        {
            Email = "Henry@email.com",
            Password = "password",
        };
        var controller = new UserController(userService);
        var result = controller.Login(userLoginRequest);
        Assert.IsType<BadRequestObjectResult>(result);
        var objectResult = result as BadRequestObjectResult;
        var value = objectResult?.Value;
        Assert.Equal("{ errorMessage = The user is not existed! }", value?.ToString());
    }
    
    [Fact]
    public void should_Return_BadRequest_If_Password_Is_Not_Correct_When_User_Login()
    {
        var userService = SetupService();
        var userLoginRequest = new UserLoginRequest
        {
            Email = "Tom@email.com",
            Password = "",
        };
        var controller = new UserController(userService);
        var result = controller.Login(userLoginRequest);
        Assert.IsType<BadRequestObjectResult>(result);
        var objectResult = result as BadRequestObjectResult;
        var value = objectResult?.Value;
        Assert.Equal("{ errorMessage = The password is not correct! }", value?.ToString());
    }

    [Fact]
    public void Should_Return_OK_When_Token_Is_Verified()
    {
        var userService = SetupService();
        var controller = new UserController(userService);
        var result = controller.Verify("testToken");
        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public void Should_Return_OK_When_Update_Successfully()
    {
        var userService = SetupService();
        var controller = new UserController(userService);
        UserInfo userInfo = new UserInfo
        {
            Email = "update@email.com",
            Address = "Xi'an",
            Job = "doctor",
            Phone = "13526758976"
        };
        var result = controller.Update(userInfo);
        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public void Should_Return_BadRequest_When_Updated_Email_Already_Existed()
    {
        var userService = SetupService();
        var controller = new UserController(userService);
        UserInfo userInfo = new UserInfo
        {
            Email = "Amy@email.com",
            Address = "Xi'an",
            Job = "doctor",
            Phone = "13526758976"
        };
        var result = controller.Update(userInfo);
        Assert.IsType<BadRequestObjectResult>(result);
    }
    
    private UserServiceImpl SetupService()
    {
        var mockContext = MockDbContext();
        var userDao = new UserDaoImpl(mockContext.Object);
        var configuration = MockConfiguration();
        var userService = new UserServiceImpl(userDao, configuration);
        return userService;
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