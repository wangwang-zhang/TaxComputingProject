using TaxComputingProject.Model;

namespace TaxComputingProject.Services;

public interface IUserService
{
    public bool AddUser(UserRegisterRequest request);
}