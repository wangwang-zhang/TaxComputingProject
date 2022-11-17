using Microsoft.EntityFrameworkCore;
using Moq;
using TaxComputingProject.Dao;
using TaxComputingProject.DBContext;
using TaxComputingProject.Model;

namespace TaxComputingProjectTest.DaoTest;

public class UserDaoTest
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
    public void Should_Return_True_When_User_Exist()
    {
        var mockContext = MockContext();
        var userDao = new UserDaoImpl(mockContext.Object);
        Assert.True(userDao.FindUserByEmail("Tom@email.com") != null);
        Assert.True(userDao.FindUserByEmail("Lucas@email.com") == null);
    }
    
    [Fact]
    public void Should_Return_Correct_Count_When_Add_One_User()
    {
        DataContext dataContext = new DataContext();
        var userDao = new UserDaoImpl(dataContext);
        User user = new User
        {
            Email = "Hello2@example.com",
            PasswordHash = new byte[32],
            PasswordSalt = new byte[32],
            VerificationToken = "",
            VerifiedAt = null
        };
        userDao.AddUser(user);
        Assert.Equal(2, dataContext.Users.Count());
    }
    
    [Fact]
    public void Should_Return_Correct_User_When_Find_User_By_token()
    {
        var mockContext = MockContext();
        var userDao = new UserDaoImpl(mockContext.Object);
        var user = userDao.FindUserByToken("testToken");
        Assert.Equal("Tom@email.com", user.Email);
    }

    private Mock<DataContext> MockContext()
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