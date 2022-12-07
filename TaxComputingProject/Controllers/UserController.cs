using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaxComputingProject.Model;
using TaxComputingProject.Services;

namespace TaxComputingProject.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost("register")]
    public IActionResult Register(UserRegisterRequest request)
    {
        try
        {
            var result = _userService.AddUser(request);
            return Ok($"User successfully created! Your activation code is\n {result}");
        }
        catch (Exception e)
        {
            return BadRequest(new { errorMessage = e.Message });
        }
    }

    [HttpPost("verify")]
    public IActionResult Verify(string activationCode)
    {
        _userService.AddVerify(activationCode);
        return Ok("User verified! :)");
    }

    [HttpPost("login")]
    public IActionResult Login(UserLoginRequest request)
    {
        var result = _userService.UserLogin(request);
        if (result.Length == 0)
        {
            return BadRequest("User not found or not verified");
        }

        return Ok($"Welcome back, {request.Email}! :), Your Token is\n {result}");
    }

    [HttpPut("userInfoUpdate"), Authorize]
    public IActionResult Update(UserInfo userInfo)
    {
        try
        {
            _userService.UserUpdate(userInfo);
        }
        catch (Exception e)
        {
            return BadRequest(new { errorMessage = e.Message });
        }
        return Ok("User information updated successfully!");
    }
}