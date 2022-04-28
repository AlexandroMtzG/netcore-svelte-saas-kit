using System;
using System.Collections.Generic;
using NetcoreSaas.Application.Dtos.Core.Workspaces;
using NetcoreSaas.Domain.Enums.Core.Users;

namespace NetcoreSaas.Application.Dtos.Core.Users
{
    public class UserSimpleDto : MasterEntityDto
    {
        public string Email { get; set; }
        public UserType Type { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }
        public UserLoginType LoginType { get; set; }
        public Guid Token { get; set; }
        public int? DefaultTenantId { get; set; }
        public List<WorkspaceUserDto> Workspaces { get; set; }

        public string GetFullName()
        {
            if (string.IsNullOrEmpty(FirstName))
                return Email;
            return string.IsNullOrEmpty(LastName) ? FirstName : $"{FirstName} {LastName}";
        }
    }
}
