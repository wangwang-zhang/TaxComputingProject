using TaxComputingProject.Model;

namespace TaxComputingProject.Services;

public interface IUserService
{
    public string AddUser(UserRegisterRequest request);
    public bool AddVerify(string token);
    public string UserLogin(UserLoginRequest request);
    public void UserUpdate(int id, UserInfo userUpdateInfo);
}