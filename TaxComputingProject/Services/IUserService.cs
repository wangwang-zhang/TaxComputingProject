using TaxComputingProject.Model;

namespace TaxComputingProject.Services;

public interface IUserService
{
    public bool AddUser(UserRegisterRequest request);
    public bool AddVerify(string token);
    public bool UserLogin(UserLoginRequest request);

}