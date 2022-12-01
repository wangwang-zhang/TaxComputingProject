using System.ComponentModel.DataAnnotations;

namespace TaxComputingProject.Model;

public class UserInfo
{
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
    [RegularExpression("^((13[0-9])|(15[1-3,5-9])|(18[0-9]))\\d{8}$", ErrorMessage = "Can't identify phone number")]
    public string Phone { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string Job { get; set; } = string.Empty;
}