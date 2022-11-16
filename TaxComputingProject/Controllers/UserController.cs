using System.Security.Cryptography;
using Microsoft.AspNetCore.Mvc;
using TaxComputingProject.DBContext;
using TaxComputingProject.Model;
using TaxComputingProject.Services;

namespace TaxComputingProject.Controllers;

public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController( IUserService userService )
    {
        _userService = userService;
    }
    
    [HttpPost("register")]
    public IActionResult Register(UserRegisterRequest request)
    {
        if (request.Password != request.ConfirmPassword)
            return BadRequest("Password and ConfirmPassword do not match");
        var result = _userService.AddUser(request);
        if (!result)
        {
            return BadRequest("User already exists.");
        }
        return Ok("User successfully created!");
    }
    [HttpPost("verify")]
    public IActionResult Verify(string token)
    {
        _userService.AddVerify(token);
        return Ok("User verified! :)");
    }
    [HttpPost("login")]
    public IActionResult Login(UserLoginRequest request)
    {
        var result = _userService.UserLogin(request);
        if (result == false)
        {
            return BadRequest("User not found or not verified");
        }
        return Ok($"Welcome back, {request.Email}! :)");
    }

}