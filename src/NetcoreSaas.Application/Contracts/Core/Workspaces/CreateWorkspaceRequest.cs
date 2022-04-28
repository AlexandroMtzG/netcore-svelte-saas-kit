using System;
using System.Collections.Generic;
using NetcoreSaas.Application.Dtos.Core.Users;
using NetcoreSaas.Domain.Enums.Core.Tenants;

namespace NetcoreSaas.Application.Contracts.Core.Workspaces
{
    public class CreateWorkspaceRequest
    {
        public string Name { get; set; }
        // TODO: Add your own workspace properties below
        public WorkspaceType Type { get; set; }
        public string BusinessMainActivity { get; set; }
        public string RegistrationNumber { get; set; }
        public DateTime? RegistrationDate { get; set; }
        public List<UserDto> Users { get; set; }
    }
}