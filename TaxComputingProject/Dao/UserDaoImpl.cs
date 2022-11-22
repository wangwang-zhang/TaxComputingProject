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
        return  _context.Users.FirstOrDefault(user => user.Email == email);
    }
    public void AddUser(User user)
    {
        _context.Users.Add(user);
        _context.SaveChanges();
    }

    public User? FindUserByToken(string token)
    {
        return  _context.Users.FirstOrDefault(user => user.VerificationToken == token);
    }

    public void SaveChanges()
    {
        _context.SaveChanges();
    }

    public bool AddUserTax(UserTax userTax)
    {
        _context.UserTaxes.Add(userTax);
        _context.SaveChanges();
        return true;
    }
    
    public virtual UserTax? GetUserTax(string email)
    {
        return _context.UserTaxes.Where(user => user.Email == email)
            .Include(userTax => userTax.Taxes)
            .FirstOrDefault();
    }
}