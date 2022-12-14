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

    public UserServiceImpl( IUserDao userDao, IConfiguration configuration )
    {
        _userDao = userDao;
        _configuration = configuration;
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
        if (user == null) throw new Exception("The user is not exist!");
        user.VerifiedAt = DateTime.Now;
        _userDao.SaveChanges();
        return true;

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

    public void UserUpdate(int id, UserInfo userUpdateInfo)
    {
        var currentUser = _userDao.GetUserById(id);
        if (currentUser == null)
        {
            throw new Exception("The user is not existed!");
        }

        if (userUpdateInfo.Email != null)
        {
            var userWithSameEmail = _userDao.FindUserByEmail(userUpdateInfo.Email);
            if (userUpdateInfo.Email != currentUser.Email && userWithSameEmail != null)
            {
                throw new Exception("This user email have already existed!");
            }
        }

        MapUserInfo(userUpdateInfo, currentUser);
        _userDao.UpdateUserInfo(id, userUpdateInfo);
    }

    private static void MapUserInfo(UserInfo userUpdateInfo, User currentUser)
    {
        userUpdateInfo.Email ??= currentUser.Email;
        userUpdateInfo.Phone ??= currentUser.Phone;
        userUpdateInfo.Address ??= currentUser.Address;
        userUpdateInfo.Job ??= currentUser.Job;
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