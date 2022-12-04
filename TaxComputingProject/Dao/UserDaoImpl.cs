using Microsoft.EntityFrameworkCore;
using TaxComputingProject.DBContext;
using TaxComputingProject.Model;

namespace TaxComputingProject.Dao;

public class UserDaoImpl : IUserDao
{
    private readonly DataContext _context;

    public UserDaoImpl(DataContext context)
    {
        _context = context;
    }

    public UserDaoImpl()
    {
    }

    public User? FindUserByEmail(string email)
    {
        return _context.Users.FirstOrDefault(user => user.Email == email);
    }

    public void AddUser(User user)
    {
        _context.Users.Add(user);
        _context.SaveChanges();
    }

    public User? FindUserByToken(string token)
    {
        return _context.Users.FirstOrDefault(user => user.VerificationToken == token);
    }

    public void SaveChanges()
    {
        _context.SaveChanges();
    }

    public void AddUserTax(UserTax userTax)
    {
        _context.UserTaxes.Add(userTax);
        _context.SaveChanges();
    }

    public virtual UserTax? GetUserTaxById(int id)
    {
        return _context.UserTaxes.Where(user => user.UserId == id)
            .Include(userTax => userTax.Taxes)
            .FirstOrDefault();
    }

    public void RemoveTaxItem(int id)
    {
        UserTax? userTax = _context.UserTaxes.Where(user => user.Id == id)
            .Include(userTax => userTax.Taxes)
            .FirstOrDefault();
        if (userTax != null) _context.Taxes.RemoveRange(userTax.Taxes);
        _context.SaveChanges();
    }

    public void UpdateUserInfo(int id, UserInfo userInfo)
    {
        var user = _context.Users.FirstOrDefault(user => user.Id == id);
        if (user != null)
        {
            if (userInfo.Email != "default@example.com")
                user.Email = userInfo.Email;

            if (userInfo.Address != string.Empty)
            {
                user.Address = userInfo.Address;
            }

            if (userInfo.Job != string.Empty)
            {
                user.Job = userInfo.Job;
            }

            if (userInfo.Phone != string.Empty)
            {
                user.Phone = userInfo.Phone;
            }
        }

        _context.SaveChanges();
    }

    public User? GetUserById(int id)
    {
        return _context.Users.FirstOrDefault(user => user.Id == id);
    }
}