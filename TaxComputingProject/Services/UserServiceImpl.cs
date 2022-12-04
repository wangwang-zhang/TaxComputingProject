using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;
using TaxComputingProject.Dao;
using TaxComputingProject.Model;

namespace TaxComputingProject.Services;

public class UserServiceImpl : IUserService
{
    private readonly IUserDao _userDao;
    private readonly IConfiguration _configuration;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserServiceImpl(IUserDao userDao, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
    {
        _userDao = userDao;
        _configuration = configuration;
        _httpContextAccessor = httpContextAccessor;
    }

    public string AddUser(UserRegisterRequest request)
    {
        if (_userDao.FindUserByEmail(request.Email) != null)
        {
            throw new Exception("User already exists.");
        }

        CreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);
        var user = new User
        {
            Email = request.Email,
            Phone = request.Phone,
            Address = request.Address,
            Job = request.Job,
            PasswordHash = passwordHash,
            PasswordSalt = passwordSalt,
            VerificationToken = CreateActivationCode()
        };
        _userDao.AddUser(user);
        return user.VerificationToken;
    }

    public bool AddVerify(string token)
    {
        var user = _userDao.FindUserByToken(token);
        if (user != null)
        {
            user.VerifiedAt = DateTime.Now;
            _userDao.SaveChanges();
            return true;
        }

        return false;
    }

    public string UserLogin(UserLoginRequest request)
    {
        User? user = _userDao.FindUserByEmail(request.Email);
        if (user == null)
        {
            throw new Exception("The user is not existed!");
        }

        if (!VerifyPasswordHash(request.Password, user.PasswordHash, user.PasswordSalt))
        {
            throw new Exception("The password is not correct!");
        }

        if (user.VerifiedAt == null)
        {
            throw new Exception("The user is not activated");
        }

        return CreateToken(user);
    }

    public void UserUpdate(UserInfo userInfo)
    {
        int id = GetId();
        _userDao.UpdateUserInfo(id, userInfo);
    }

    private int GetId()
    {
        string result = string.Empty;
        if (_httpContextAccessor.HttpContext != null)
        {
            result = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Sid);
        }

        int.TryParse(result, out var userId);
        return userId;
    }

    private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
    {
        using (var hmac = new HMACSHA512())
        {
            passwordSalt = hmac.Key;
            passwordHash = hmac
                .ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
        }
    }

    private string CreateActivationCode()
    {
        return Convert.ToHexString(RandomNumberGenerator.GetBytes(64));
    }

    private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
    {
        using (var hmac = new HMACSHA512(passwordSalt))
        {
            var computedHash = hmac
                .ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            return computedHash.SequenceEqual(passwordHash);
        }
    }

    private string CreateToken(User user)
    {
        List<Claim> claims = new List<Claim>
        {
            new(ClaimTypes.Sid, user.Id.ToString()),
        };
        var key = new SymmetricSecurityKey(
            System.Text.Encoding.UTF8.GetBytes(_configuration.GetSection("AppSettings:Token").Value));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.Now.AddDays(50),
            signingCredentials: credentials);
        var jwt = new JwtSecurityTokenHandler().WriteToken(token);
        return jwt;
    }
}