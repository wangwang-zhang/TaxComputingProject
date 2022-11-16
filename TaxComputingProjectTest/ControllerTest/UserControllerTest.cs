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
            Id = 1, Email = "Tom@email.com", PasswordHash = new byte[32], PasswordSalt = new byte[32], VerificationToken = "",
            VerifiedAt = null
        },
        new()
        {
            Id = 2, Email = "Amy@email.com", PasswordHash = new byte[32], PasswordSalt = new byte[32], VerificationToken = "",
            VerifiedAt = null
        },
        new()
        {
            Id = 3, Email = "Bob@email.com", PasswordHash = new byte[32], PasswordSalt = new byte[32], VerificationToken = "",
            VerifiedAt = null
        },
    }.AsQueryable();
    
    [Fact]
    public void Should_Return_BadRequest_When_User_Existed_Already()
    {
        var userServiceImpl = MockContext();
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
        var userServiceImpl = MockContext();
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
    private UserServiceImpl MockContext()
    {
        var mockSet = new Mock<DbSet<User>>();
        mockSet.As<IQueryable<User>>().Setup(m => m.Provider).Returns(_users.Provider);
        mockSet.As<IQueryable<User>>().Setup(m => m.Expression).Returns(_users.Expression);
        mockSet.As<IQueryable<User>>().Setup(m => m.ElementType).Returns(_users.ElementType);
        mockSet.As<IQueryable<User>>().Setup(m => m.GetEnumerator()).Returns(() => _users.GetEnumerator());
        var mockContext = new Mock<DataContext>();
        mockContext.Setup(dataContext => dataContext.Users).Returns(mockSet.Object);
        var userDaoImpl = new UserDaoImpl(mockContext.Object);
        var userServiceImpl = new UserServiceImpl(userDaoImpl);
        return userServiceImpl;
    }
}