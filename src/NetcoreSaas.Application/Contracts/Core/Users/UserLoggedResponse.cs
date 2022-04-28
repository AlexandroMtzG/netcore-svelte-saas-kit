using NetcoreSaas.Application.Dtos.Core.Users;
using NetcoreSaas.Application.Dtos.Core.Workspaces;

namespace NetcoreSaas.Application.Contracts.Core.Users
{
    public class UserLoggedResponse
    {
        public UserDto User { get; set; }
        public WorkspaceDto DefaultWorkspace { get; set; }
        public string Token { get; set; }
    }
}
