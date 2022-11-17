using TaxComputingProject.Model;

namespace TaxComputingProject.Dao;

public interface IUserDao
{
    public User? FindUserByEmail(string email);
    public void AddUser(User user);
    public User? FindUserByToken(string token);
    public void SaveChanges();
}