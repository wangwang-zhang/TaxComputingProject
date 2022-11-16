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
        if (_userDao.FindUserByEmail(request.Email))
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
}