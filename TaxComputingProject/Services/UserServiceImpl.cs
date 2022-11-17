using System.Security.Cryptography;
using TaxComputingProject.Dao;
using TaxComputingProject.Model;

namespace TaxComputingProject.Services;

public class UserServiceImpl : IUserService
{
    private readonly IUserDao _userDao;

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

    public bool UserLogin(UserLoginRequest request)
    {
        User? user = _userDao.FindUserByEmail(request.Email);
        if(user == null)
        {
            return false;
        }
        if (!VerifyPasswordHash(request.Password, user.PasswordHash, user.PasswordSalt))
        {
            return false;
        }
        if (user.VerifiedAt == null)
        {
            return false;
        }
        return true;
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
    
}