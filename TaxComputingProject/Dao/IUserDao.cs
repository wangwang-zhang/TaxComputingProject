using TaxComputingProject.Model;

namespace TaxComputingProject.Dao;

public interface IUserDao
{
    public bool FindUserByEmail(string email);
    public void AddUser(User user);
    public User FindUserByToken(string token);
}