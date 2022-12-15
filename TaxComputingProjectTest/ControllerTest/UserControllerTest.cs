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
    private readonly DataContext _context;

    public UserControllerTest()
    {
        DbContextOptionsBuilder dbOptions = new DbContextOptionsBuilder()
            .UseInMemoryDatabase(
                Guid.NewGuid().ToString()
            );
        _context = new DataContext(dbOptions.Options);
    }
    
    [Fact]
    public void Should_Return_BadRequest_If_User_Existed_Already_When_Register_Integration()
    {
        var userDao = new UserDaoImpl(_context);
        var mockConfiguration = MockConfiguration();
        var userService = new UserServiceImpl(userDao, mockConfiguration);
        var userRegisterRequest = new UserRegisterRequest
        {
            Email = "Tom@email.com",
            Password = "password",
            ConfirmPassword = "password"
        };
        userService.AddUser(userRegisterRequest);
        var userController = new UserController(userService);
        var result = userController.Register(userRegisterRequest);
        Assert.IsType<BadRequestObjectResult>(result);
        var objectResult = result as BadRequestObjectResult;
        Assert.Equal("{ errorMessage = User already exists. }",  objectResult?.Value?.ToString());
    }

    [Fact]
    public void Should_Return_OK_When_Register_Successfully()
    {
        var mockUserService = new Mock<IUserService>();
        mockUserService.Setup(user => user.AddUser(It.IsAny<UserRegisterRequest>()))
            .Returns("success");
        var userController = new UserController(mockUserService.Object);
        var result = userController.Register(new UserRegisterRequest());
        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public void Should_Return_BadRequest_If_User_Existed_Already_When_Register()
    {
        var mockUserService = new Mock<IUserService>();
        mockUserService.Setup(user => user.AddUser(It.IsAny<UserRegisterRequest>()))
            .Throws(() => new Exception("User already exists."));
        var userController = new UserController(mockUserService.Object);
        var result = userController.Register(new UserRegisterRequest());
        Assert.IsType<BadRequestObjectResult>(result);
        var objectResult = result as BadRequestObjectResult;
        Assert.Equal("{ errorMessage = User already exists. }",  objectResult?.Value?.ToString());
    }

    [Theory]
    [InlineData("The user is not existed!")]
    [InlineData("The user is not activated")]
    [InlineData("The password is not correct!")]
    public void Should_Return_BadRequest_When_Login_Failed(string exceptionMessage)
    {
        var mockUserService = new Mock<IUserService>();
        mockUserService.Setup(user => user.UserLogin(It.IsAny<UserLoginRequest>()))
            .Throws(() => new Exception(exceptionMessage));
        var userController = new UserController(mockUserService.Object);
        var result = userController.Login(new UserLoginRequest());
        Assert.IsType<BadRequestObjectResult>(result);
        var objectResult = result as BadRequestObjectResult;
        Assert.Equal("{ errorMessage = " + exceptionMessage +" }", objectResult?.Value?.ToString());
    }

    [Fact]
    public void Should_Return_OK_When_ActivationCode_Is_Verified()
    {
        var mockUserService = new Mock<IUserService>();
        mockUserService.Setup(user => user.AddVerify(It.IsAny<string>()))
            .Returns(true);
        var userController = new UserController(mockUserService.Object);
        var result = userController.Verify("testActivationCode");
        Assert.IsType<OkObjectResult>(result);
    }
    
    [Fact]
    public void Should_Return_BadRequest_If_User_Not_Exist_When_Verify_User()
    {
        var mockUserService = new Mock<IUserService>();
        mockUserService.Setup(user => user.AddVerify(It.IsAny<string>()))
            .Throws(() => new Exception("The user is not exist!"));
        var userController = new UserController(mockUserService.Object);
        var result = userController.Verify("");
        Assert.IsType<BadRequestObjectResult>(result);
        var objectResult = result as BadRequestObjectResult;
        Assert.Equal("{ errorMessage = The user is not exist! }", objectResult?.Value?.ToString());
    }

    [Theory]
    [InlineData("The user is not existed!")]
    [InlineData("This user email have already existed!")]
    public void Should_Return_BadRequest_When_Update_Failed(string exceptionMessage)
    {
        var mockUserService = new Mock<IUserService>();
        mockUserService.Setup(user => user.UserUpdate(It.IsAny<int>(), It.IsAny<UserInfo>()))
            .Throws<Exception>(() => throw new Exception(exceptionMessage));
        var userController = new UserController(mockUserService.Object);
        var result = userController.Update(new UserInfo());
        Assert.IsType<BadRequestObjectResult>(result);
        var objectResult = result as BadRequestObjectResult;
        Assert.Equal("{ errorMessage = " + exceptionMessage + " }", objectResult?.Value?.ToString());
    }
    
    [Fact]
    public void Should_Return_OK_When_Update_Successfully()
    {
        var mockUserService = new Mock<IUserService>();
        mockUserService.Setup(user => user.UserUpdate(It.IsAny<int>(), It.IsAny<UserInfo>()));
        var userController = new UserController(mockUserService.Object);
        var result = userController.Update(new UserInfo());
        Assert.IsType<OkObjectResult>(result);
    }

    private static UserServiceImpl SetupService()
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

    private static Mock<DataContext> MockDbContext()
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