using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace StudentCoursePlatform.Application.DTOs.Users;

public class UserUpdateDto
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}
