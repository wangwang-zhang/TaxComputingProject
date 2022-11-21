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
        Assert.True(userDao.FindUserByEmail("Tom@email.com") != null);
        Assert.True(userDao.FindUserByEmail("Lucas@email.com") == null);
    }
    
    [Fact]
    public void Should_Return_Correct_Count_When_Add_One_User()
    {
        var userDao = new UserDaoImpl(_context);
        User user = new User
        {
            Email = "Hello2@example.com",
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