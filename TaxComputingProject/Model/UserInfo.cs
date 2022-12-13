using System.ComponentModel.DataAnnotations;

namespace TaxComputingProject.Model;

public class UserInfo
{
    [EmailAddress]
    public string? Email { get; set; }
    [RegularExpression("^((13[0-9])|(15[1-3,5-9])|(18[0-9]))\\d{8}$", ErrorMessage = "Can't identify phone number")]
    public string? Phone { get; set; } 
    public string? Address { get; set; } 
    public string? Job { get; set; } 
}