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
    private readonly string _tokenKey = "My token key";

    public UserServiceImpl(IUserDao userDao)
    {
        _userDao = userDao;
    }
    
    public bool AddUser(UserRegisterRequest request)
    {
        if (_userDao.FindUserByEmail(request.Email) != null)
        {
            return false;
        }
        CreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);
        var user = new User
        {
            Email = request.Email,
            PasswordHash = passwordHash,
            PasswordSalt = passwordSalt,
            VerificationToken = CreateRandomToken()
        };
        _userDao.AddUser(user);
        return true;
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
        if(user == null)
        {
            throw new Exception("The user is not existed!");
        }
        if (!VerifyPasswordHash(request.Password, user.PasswordHash, user.PasswordSalt))
        {
            throw new Exception("The password is not correct!");
        }
        if (user.VerifiedAt == null)
        {
            throw new Exception("The user is not activated");;
        }
        return CreateToken(user);
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
    
    private string CreateRandomToken()
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
            new(ClaimTypes.Email, user.Email),
        };
        var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_tokenKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.Now.AddDays(50),
            signingCredentials: credentials);
        var jwt = new JwtSecurityTokenHandler().WriteToken(token);
        return jwt;
    }
    
}