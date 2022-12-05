using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Moq;
using TaxComputingProject.Dao;
using TaxComputingProject.DBContext;
using TaxComputingProject.Model;
using TaxComputingProject.Services;
using TaxComputingProjectTest.MockData;
using Microsoft.Extensions.Configuration;
using TaxComputingProject.Utils;

namespace TaxComputingProjectTest.ServicesTest;

public class TaxComputingServiceTest
{
    private readonly DataContext _context;

    public TaxComputingServiceTest()
    {
        DbContextOptionsBuilder dbOptions = new DbContextOptionsBuilder()
            .UseInMemoryDatabase(
                Guid.NewGuid().ToString()
            );
        _context = new DataContext(dbOptions.Options);
    }

    [Theory]
    [InlineData(5000, 0)]
    [InlineData(36000, 930)]
    [InlineData(41000, 1080)]
    [InlineData(149000, 11880)]
    [InlineData(305000, 43080)]
    [InlineData(425000, 73080)]
    [InlineData(665000, 145080)]
    [InlineData(965000, 250080)]
    [InlineData(1000000, 265830)]
    public void Should_Return_Correct_Tax_With_One_Month_Of_Salary(double salary, double tax)
    {
        List<MonthSalary> testSalaries = new List<MonthSalary>
        {
            new()
            {
                Month = 1,
                Salary = salary
            }
        };
        var userDao = new UserDaoImpl(_context);
        var accessorMock = MockHttpContextAccessor();
        var httpContextAccessor = new HttpContextAccessorUtil(accessorMock.Object);
        var taxComputingService = new TaxComputingServiceImpl(userDao, httpContextAccessor);
        taxComputingService.ComputeTaxBySalaryAndMonth(testSalaries);
        Assert.Equal(tax, taxComputingService.GetTaxOfMonth(1));
    }

    [Theory]
    [InlineData(36000, 0.03, 0)]
    [InlineData(144000, 0.1, 2520)]
    [InlineData(300000, 0.2, 16920)]
    [InlineData(420000, 0.25, 31920)]
    [InlineData(660000, 0.3, 52920)]
    [InlineData(960000, 0.35, 85920)]
    [InlineData(1000000, 0.45, 181920)]
    public void Should_Return_Correct_Tax_Rate_And_Deduction_By_Taxable_Salary(double salary, double taxRate,
        double deduction)
    {
        var taxComputingService = MockService();
        var result = taxComputingService.MatchTaxRateAndDeductionBySalary(salary);
        Assert.Equal(taxRate, result.TaxRate);
        Assert.Equal(deduction, result.Deduction);
    }

    [Theory]
    [InlineData(new double[] { 41000 }, 1, 1080)]
    [InlineData(new double[] { 41000, 113000 }, 2, 10800)]
    [InlineData(new double[] { 41000, 113000, 161000 }, 3, 31200)]
    [InlineData(new double[] { 41000, 113000, 161000, 125000 }, 4, 30000)]
    [InlineData(new double[] { 41000, 113000, 161000, 125000, 245000 }, 5, 72000)]
    [InlineData(new double[] { 41000, 113000, 161000, 125000, 245000, 305000 }, 6, 105000)]
    [InlineData(new double[] { 41000, 113000, 161000, 125000, 245000, 305000, 45000 }, 7, 18000)]
    [InlineData(new[] { 39003.7, 41200.8, 50987.7 }, 3, 4598.77)]
    public void Should_Return_Correct_Tax_With_Multiple_Month_Of_Salary(double[] salaries, int month, double tax)
    {
        List<MonthSalary> testSalaries = new List<MonthSalary>();
        for (int i = 0; i < salaries.Length; i++)
        {
            testSalaries.Add(new MonthSalary()
            {
                Month = i + 1,
                Salary = salaries[i]
            });
        }

        var userDao = new UserDaoImpl(_context);
        var accessorMock = MockHttpContextAccessor();
        var httpContextAccessor = new HttpContextAccessorUtil(accessorMock.Object);
        var taxComputingService = new TaxComputingServiceImpl(userDao, httpContextAccessor);
        taxComputingService.ComputeTaxBySalaryAndMonth(testSalaries);
        Assert.Equal(tax, taxComputingService.GetTaxOfMonth(month));
    }

    [Theory]
    [InlineData(1, 1080)]
    [InlineData(2, 3600)]
    [InlineData(5, 7200)]
    public void Should_Return_Correct_TaxOfMonth_By_Month(int month, double tax)
    {
        ITaxComputingService taxComputingService = MockUserTax();
        double taxOfMonth = taxComputingService.GetTaxOfMonth(month);
        Assert.Equal(tax, taxOfMonth);
    }

    [Theory]
    [InlineData(6)]
    [InlineData(8)]
    [InlineData(10)]
    public void Should_Throw_Exception_When_Month_Tax_Not_Exist(int month)
    {
        ITaxComputingService taxComputingService = MockUserTax();
        Assert.Throws<BadHttpRequestException>(() => taxComputingService.GetTaxOfMonth(month));
    }

    [Fact]
    public void Should_Return_Exception_When_There_Are_Duplicate_Months()
    {
        var monthSalaries = new List<MonthSalary>
        {
            new()
            {
                Month = 1,
                Salary = 41000,
                Tax = 1080
            },
            new()
            {
                Month = 1,
                Salary = 41000,
                Tax = 1080
            }
        };
        var taxComputingService = MockService();
        Assert.Throws<ArgumentException>(() => taxComputingService.ComputeTaxBySalaryAndMonth(monthSalaries));
    }

    [Fact]
    public void Should_Return_NotNull_Object_When_Get_AnnualTaxRecords_Successfully()
    {
        var userDao = new UserDaoImpl(_context);
        var accessorMock = MockHttpContextAccessor();
        var httpContextAccessor = new HttpContextAccessorUtil(accessorMock.Object);
        var userService = new UserServiceImpl(userDao, MockConfiguration(), httpContextAccessor);
        var taxComputingService = new TaxComputingServiceImpl(userDao, httpContextAccessor);
        var user = new UserRegisterRequest
        {
            Email = "initial@example.com",
            Phone = "13812344321",
            Job = "teacher",
            Address = "Xi'an",
            Password = "123456789",
            ConfirmPassword = "123456789"
        };
        userService.AddUser(user);
        List<MonthSalary> monthSalaries = new List<MonthSalary>
        {
            new() { Month = 1, Salary = 41000, Tax = 1080 },
            new() { Month = 2, Salary = 41000, Tax = 3600 }
        };
        taxComputingService.ComputeTaxBySalaryAndMonth(monthSalaries);
        var records = taxComputingService.GetAnnualTaxRecords();
        var monthTaxes = records?.MonthTaxes?.ToList();
        Assert.NotNull(records);
        Assert.Equal(82000, records?.TotalSalary);
        Assert.Equal(4680, records?.TotalTax);
        if (monthTaxes != null)
        {
            Assert.Equal(41000, monthTaxes[0].Salary);
            Assert.Equal(41000, monthTaxes[1].Salary);
        }
    }

    [Fact]
    public void Should_Return_Null_If_User_Not_Existed_When_Get_AnnualTaxRecords()
    {
        var userDao = new UserDaoImpl(_context);
        var accessorMock = MockHttpContextAccessor();
        var httpContextAccessor = new HttpContextAccessorUtil(accessorMock.Object);
        var taxComputingService = new TaxComputingServiceImpl(userDao, httpContextAccessor);
        var records = taxComputingService.GetAnnualTaxRecords();
        Assert.Null(records);
    }
    
    [Fact]
    public void Should_Return_Null_If_UserTax_Not_Existed_When_Get_AnnualTaxRecords()
    {
        var userDao = new UserDaoImpl(_context);
        var accessorMock = MockHttpContextAccessor();
        var httpContextAccessor = new HttpContextAccessorUtil(accessorMock.Object);
        var userService = new UserServiceImpl(userDao, MockConfiguration(), httpContextAccessor);
        var taxComputingService = new TaxComputingServiceImpl(userDao, httpContextAccessor);
        var user = new UserRegisterRequest
        {
            Email = "initial@example.com",
            Phone = "13812344321",
            Job = "teacher",
            Address = "Xi'an",
            Password = "123456789",
            ConfirmPassword = "123456789"
        };
        userService.AddUser(user);
        var records = taxComputingService.GetAnnualTaxRecords();
        Assert.Null(records);
    }

    [Fact]
    public void Should_Return_Correct_Tax_When_Salary_Updated()
    {
        var userDao = new UserDaoImpl(_context);
        var accessorMock = MockHttpContextAccessor();
        var httpContextAccessor = new HttpContextAccessorUtil(accessorMock.Object);
        var userService = new UserServiceImpl(userDao, MockConfiguration(), httpContextAccessor);
        var taxComputingService = new TaxComputingServiceImpl(userDao, httpContextAccessor);
        var user = new UserRegisterRequest
        {
            Email = "initial@example.com",
            Phone = "13812344321",
            Job = "teacher",
            Address = "Xi'an",
            Password = "123456789",
            ConfirmPassword = "123456789"
        };
        userService.AddUser(user);
        var monthSalaries = new List<MonthSalary>
        {
            new(){Month = 1, Salary = 41000},
            new(){Month = 2, Salary = 41000},
            new(){Month = 5, Salary = 41000}
        };
        taxComputingService.ComputeTaxBySalaryAndMonth(monthSalaries);
        var monthSalariesLater = new List<MonthSalary>
        {
            new(){Month = 3, Salary = 41000},
            new(){Month = 4, Salary = 41000},
        };
        taxComputingService.ComputeTaxBySalaryAndMonth(monthSalariesLater);
        Assert.Equal(7200, taxComputingService.GetTaxOfMonth(5));
    }

    private static Mock<IHttpContextAccessor> MockHttpContextAccessor()
    {
        var accessorMock = new Mock<IHttpContextAccessor>();
        var userClaimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.Sid, "1"),
        }, "mock"));
        HttpContext httpContext = new DefaultHttpContext { User = userClaimsPrincipal };
        accessorMock.Setup(accessor => accessor.HttpContext).Returns(httpContext);
        return accessorMock;
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

    private static ITaxComputingService MockService()
    {
        var accessorMock = MockHttpContextAccessor();
        Mock<DataContext> mockDbContext = MockDbContext();
        var mockUserDao = new UserDaoImpl(mockDbContext.Object);
        var httpContextAccessor = new HttpContextAccessorUtil(accessorMock.Object);
        ITaxComputingService taxComputingService = new TaxComputingServiceImpl(mockUserDao, httpContextAccessor);
        return taxComputingService;
    }

    private static ITaxComputingService MockUserTax()
    {
        var accessorMock = MockHttpContextAccessor();
        var httpContextAccessor = new HttpContextAccessorUtil(accessorMock.Object);
        Mock<IUserDao> mockUserDao = new Mock<IUserDao>();
        mockUserDao.Setup(user => user.GetUserTaxById(It.IsAny<int>())).Returns(UserTax);
        ITaxComputingService taxComputingService = new TaxComputingServiceImpl(mockUserDao.Object, httpContextAccessor);
        return taxComputingService;
    }

    private static Mock<DataContext> MockDbContext()
    {
        var mockSet = new Mock<DbSet<UserTax>>();
        mockSet.As<IQueryable<UserTax>>().Setup(m => m.Provider).Returns(TestMockData.UserTaxes.Provider);
        mockSet.As<IQueryable<UserTax>>().Setup(m => m.Expression).Returns(TestMockData.UserTaxes.Expression);
        mockSet.As<IQueryable<UserTax>>().Setup(m => m.ElementType).Returns(TestMockData.UserTaxes.ElementType);
        mockSet.As<IQueryable<UserTax>>().Setup(m => m.GetEnumerator())
            .Returns(() => TestMockData.UserTaxes.GetEnumerator());
        var mockContext = new Mock<DataContext>();
        mockContext.Setup(dataContext => dataContext.UserTaxes).Returns(mockSet.Object);
        return mockContext;
    }

    private static readonly UserTax UserTax = new()
    {
        Id = 1,
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
}