using System;

namespace NetcoreSaas.Application.Contracts.Core.Workspaces
{
    public class CreateWorkspaceCredentialCiecRequest
    {
        public Guid Id { get; set; }
        public string Password { get; set; }
    }
}