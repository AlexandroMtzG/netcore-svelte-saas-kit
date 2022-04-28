using NetcoreSaas.Domain.Enums.Core.Users;

namespace NetcoreSaas.Application.Contracts.Core.Users
{
    public class UserLoginRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public UserLoginType LoginType { get; set; }
    }
}
