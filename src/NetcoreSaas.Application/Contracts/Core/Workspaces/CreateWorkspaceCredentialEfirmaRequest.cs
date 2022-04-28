using System;

namespace NetcoreSaas.Application.Contracts.Core.Workspaces
{
    public class CreateWorkspaceCredentialEfirmaRequest
    {
        public Guid Id { get; set; }
        public string Certificate { get; set; }
        public string PrivateKey { get; set; }
        public string Password { get; set; }
    }
}