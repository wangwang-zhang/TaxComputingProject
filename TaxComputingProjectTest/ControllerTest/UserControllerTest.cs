using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using TaxComputingProject.Controllers;
using TaxComputingProject.Dao;
using TaxComputingProject.DBContext;
using TaxComputingProject.Model;
using TaxComputingProject.Services;

namespace TaxComputingProjectTest.ControllerTest;

public class UserControllerTest
{
    private readonly IQueryable<User> _users = new List<User>
    {
        new()
        {
            Id = 1, Email = "Tom@email.com", PasswordHash = new byte[32], PasswordSalt = new byte[32], VerificationToken = "testToken",
            VerifiedAt = null
        },
        new()
        {
            Id = 2, Email = "Amy@email.com", PasswordHash = new byte[32], PasswordSalt = new byte[32], VerificationToken = "testTokenTwo",
            VerifiedAt = null
        },
        new()
        {
            Id = 3, Email = "Bob@email.com", PasswordHash = new byte[32], PasswordSalt = new byte[32], VerificationToken = "testTokenThree",
            VerifiedAt = null
        },
    }.AsQueryable();
    
    [Fact]
    public void Should_Return_BadRequest_When_User_Existed_Already()
    {
        var userServiceImpl = ServiceSetup();
        var userRegisterRequest = new UserRegisterRequest
        {
            Email = "Tom@email.com",
            Password = "password",
            ConfirmPassword = "password"
        };
        var controller = new UserController(userServiceImpl);
        var result = controller.Register(userRegisterRequest); 
        Assert.IsType<BadRequestObjectResult>(result);
    }
    [Fact]
    public void Should_Return_OK_When_User_Not_Existed()
    {
        var userServiceImpl = ServiceSetup();
        var userRegisterRequest = new UserRegisterRequest
        {
            Email = "Henry@email.com",
            Password = "password",
            ConfirmPassword = "password"
        };
        var controller = new UserController(userServiceImpl);
        var result = controller.Register(userRegisterRequest); 
        Assert.IsType<OkObjectResult>(result);
    }
    [Fact]
    public void Should_Return_OK_When_Token_Is_Verified()
    {
        var userService = ServiceSetup();
        var controller = new UserController(userService);
        var result = controller.Verify("testToken"); 
        Assert.IsType<OkObjectResult>(result);
    }
    
    private UserServiceImpl ServiceSetup()
    {
        var mockContext = MockDbContext();
        var userDao = new UserDaoImpl(mockContext.Object);
        var userService = new UserServiceImpl(userDao);
        return userService;
    }
    
    private Mock<DataContext> MockDbContext()
    {
        var mockSet = new Mock<DbSet<User>>();
        mockSet.As<IQueryable<User>>().Setup(m => m.Provider).Returns(_users.Provider);
        mockSet.As<IQueryable<User>>().Setup(m => m.Expression).Returns(_users.Expression);
        mockSet.As<IQueryable<User>>().Setup(m => m.ElementType).Returns(_users.ElementType);
        mockSet.As<IQueryable<User>>().Setup(m => m.GetEnumerator()).Returns(() => _users.GetEnumerator());
        var mockContext = new Mock<DataContext>();
        mockContext.Setup(dataContext => dataContext.Users).Returns(mockSet.Object);
        return mockContext;
    }
}