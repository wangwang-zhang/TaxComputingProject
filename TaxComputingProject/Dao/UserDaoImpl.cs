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
}