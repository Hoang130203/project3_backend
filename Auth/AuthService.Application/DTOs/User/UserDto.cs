using AuthService.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Application.DTOs.User
{
    public record UserDto
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public UserType UserType { get; set; }
        public string Password { get; set; }
        public string AvatarUrl { get; set; }
    }
}
