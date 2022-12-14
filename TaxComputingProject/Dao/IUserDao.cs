using TaxComputingProject.Model;

namespace TaxComputingProject.Dao;

public interface IUserDao
{
    public User? FindUserByEmail(string email);
    public void AddUser(User user);
    public User? FindUserByToken(string token);
    public void SaveChanges();
    public void AddUserTax(UserTax userTax);
    public UserTax? GetUserTaxById(int id);
    public void RemoveTaxItem(int id);
    public void UpdateUserInfo(int id, UserInfo userInfo);
    public User? GetUserById(int id);
}