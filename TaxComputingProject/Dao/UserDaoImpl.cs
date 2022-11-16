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

    public bool FindUserByEmail(string email)
    {
        return  _context.Users.Any(user => user.Email == email);
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

    public User? FindUser(string email)
    {
        return _context.Users.FirstOrDefault(user => user.Email == email);
    }
}