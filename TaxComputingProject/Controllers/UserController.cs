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
}