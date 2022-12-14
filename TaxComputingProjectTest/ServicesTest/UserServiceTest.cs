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
    private const int MockUserId = 1;

    public UserServiceTest()
    {
        DbContextOptionsBuilder dbOptions = new DbContextOptionsBuilder()
            .UseInMemoryDatabase(
                Guid.NewGuid().ToString()
            );
        _context = new DataContext(dbOptions.Options);
    }

    [Fact]
    public void Should_Throw_Exception_If_User_Existed_Already_When_AddUser()
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
    public void Should_Return_Non_Empty_String_If_User_Not_Existed_When_AddUser()
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
        var result = userService.AddVerify("testActivationCode");
        Assert.True(result);
    }

    [Fact]
    public void Should_Throw_Exception_If_User_Not_Existed_When_Login()
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
    public void Should_Throw_Exception_If_User_Not_Activated_When_Login()
    {
        var userDao = new UserDaoImpl(_context);
        var mockConfiguration = MockConfiguration();
        var userService = new UserServiceImpl(userDao, mockConfiguration);
        userService.AddUser(TestMockData.MockRegisterUser);
        var userLoginRequest = new UserLoginRequest
        {
            Email = TestMockData.MockRegisterUser.Email,
            Password = TestMockData.MockRegisterUser.Password
        };
        Assert.Throws<Exception>(() => userService.UserLogin(userLoginRequest));
    }

    [Fact]
    public void Should_Throw_Exception_If_User_Not_Exist_When_Update()
    {
        var userService = SetupService();
        var userInfo = new UserInfo();
        var id = TestMockData.Users.Count() + 1;
        Assert.Throws<Exception>(() => userService.UserUpdate(id, userInfo));
    }

    [Fact]
    public void Should_Return_NotNull_User_When_Find_User_By_Updated_Email_Successfully()
    {
        var userDao = new UserDaoImpl(_context);
        var userService = new UserServiceImpl(userDao, MockConfiguration());
        userService.AddUser(TestMockData.MockRegisterUser);
        userService.UserUpdate(MockUserId, TestMockData.UserUpdateMockInfo);
        if (TestMockData.UserUpdateMockInfo.Email != null)
        {
            var user = userDao.FindUserByEmail(TestMockData.UserUpdateMockInfo.Email);
            Assert.NotNull(user);
        }
    }

    [Fact]
    public void Should_Throw_Exception_When_Updated_Email_Have_Already_Existed()
    {
        var userDao = new UserDaoImpl(_context);
        var userService = new UserServiceImpl(userDao, MockConfiguration());
        userService.AddUser(TestMockData.MockRegisterUser);
        TestMockData.MockRegisterUser.Email = "same@example.com";
        userService.AddUser(TestMockData.MockRegisterUser);
        TestMockData.UserUpdateMockInfo.Email = "same@example.com";
        Assert.Throws<Exception>(() => userService.UserUpdate(MockUserId, TestMockData.UserUpdateMockInfo));
    }
    
    [Fact]
    public void Should_Throw_Exception_If_User_Not_Exist_When_Verify_User()
    {
        var userService = SetupService();
        Assert.Throws<Exception>(() => userService.AddVerify("fake"));
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