﻿namespace CarRentalIdentityServer.Models
{
    public class RegisterRequestModel
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }

    }
}