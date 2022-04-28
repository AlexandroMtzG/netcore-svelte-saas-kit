using System;
using System.ComponentModel.DataAnnotations;

namespace NetcoreSaas.Application.Contracts.Core.Users
{
    public class UserVerifyRequest
    {
        [Required]
        public string Email { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }
        public Guid Token { get; set; }
    }
}
