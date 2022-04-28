using System;
using System.Collections.Generic;
using NetcoreSaas.Domain.Enums.Core.Tenants;

namespace NetcoreSaas.Application.Dtos.Core.Workspaces
{
    public class WorkspaceDto : AppEntityDto
    {
        public string Name { get; set; }
        // TODO: Add your own workspace properties below
        public WorkspaceType Type { get; set; }
        public string BusinessMainActivity { get; set; }
        public string RegistrationNumber { get; set; }
        public DateTime? RegistrationDate { get; set; }
        public bool Default { get; set; }
        public ICollection<WorkspaceUserDto> Users { get; set; }
    }
}