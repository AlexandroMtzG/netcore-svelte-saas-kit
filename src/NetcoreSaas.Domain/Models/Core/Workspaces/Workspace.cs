using System;
using System.Collections.Generic;
using NetcoreSaas.Domain.Enums.Core.Tenants;

namespace NetcoreSaas.Domain.Models.Core.Workspaces
{
    public class Workspace : AppEntity
    {
        public string Name { get; set; }
        // TODO: Add your own workspace properties below
        public WorkspaceType Type { get; set; }
        public string BusinessMainActivity { get; set; }
        public string RegistrationNumber { get; set; }
        public DateTime? RegistrationDate { get; set; }
        public bool Default { get; set; }
        public ICollection<WorkspaceUser> Users { get; set; }
        
        public Workspace()
        {
            Users = new List<WorkspaceUser>();
        }
    }
}