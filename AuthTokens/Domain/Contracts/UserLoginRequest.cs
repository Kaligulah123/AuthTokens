﻿namespace AuthTokens.Domain.Contracts
{
    public class UserLoginRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
