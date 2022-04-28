namespace NetcoreSaas.Application.Contracts.Core.Users
{
    public class UserUpdatePasswordRequest
    {
        public string PasswordCurrent { get; set; }
        public string PasswordNew { get; set; }
        public string PasswordConfirm { get; set; }
    }
}
