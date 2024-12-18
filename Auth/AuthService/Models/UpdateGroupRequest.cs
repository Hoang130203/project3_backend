﻿using AuthService.Domain.Enums;

namespace AuthService.Models
{
    public class UpdateGroupRequest
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public bool Visibility { get; set; }
    }
}
