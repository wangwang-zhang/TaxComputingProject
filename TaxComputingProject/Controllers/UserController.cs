using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaxComputingProject.Model;
using TaxComputingProject.Services;
using TaxComputingProject.Utils;

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
        try
        {
            _userService.AddVerify(activationCode);
            return Ok("User verified! :)");
        }
        catch (Exception e)
        {
            return new BadRequestObjectResult(new { errorMessage = e.Message });
        }
    }

    [HttpPost("login")]
    public IActionResult Login(UserLoginRequest request)
    {
        try
        {
            var result = _userService.UserLogin(request);
            return Ok($"Welcome back, {request.Email}! :), Your Token is\n {result}");
        }
        catch (Exception e)
        {
            return BadRequest(new { errorMessage = e.Message });
        }
    }

    [HttpPut("userInfoUpdate"), Authorize]
    public IActionResult Update(UserInfo userInfo)
    {
        try
        {
            var id = HttpContextAccessorUtil.GetId();
            _userService.UserUpdate(id, userInfo);
        }
        catch (Exception e)
        {
            return BadRequest(new { errorMessage = e.Message });
        }
        return Ok("User information updated successfully!");
    }
}