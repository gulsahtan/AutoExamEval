using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace AutoExamEval.Entities;

public class AppUser : IdentityUser
{
    [Required]
    [StringLength(150)]
    public string FullName { get; set; } = string.Empty;
}
