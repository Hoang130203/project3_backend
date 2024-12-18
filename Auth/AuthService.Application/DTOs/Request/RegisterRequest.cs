using AuthService.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Application.DTOs.Request
{
    public record RegisterRequest(string Username, string Email, string Password, string FullName, string AvatarUrl, UserType UserType);
}
