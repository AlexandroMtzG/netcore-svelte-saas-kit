using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace NetcoreSaas.WebApi.Service
{
    public abstract class GlobalHubService : Hub
    {
        public async Task JoinGroup(Guid tenant)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, tenant.ToString());
            if (Context.User != null)
            {
                var user = Context.User.Identity.Name;
                await Clients.Group(tenant.ToString()).SendAsync("UserConnected", user + " joined.");
            }
        }
        public async Task LeaveGroup(Guid tenant)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, tenant.ToString());
        }
        public async Task SendMessage(Guid tenant, string user, string message)
        {
            await Clients.Group(tenant.ToString()).SendAsync("ReceiveMessage", user, message);
        }

        public async Task AddObject(Guid tenant, object obj)
        {
            await Clients.Group(tenant.ToString()).SendAsync("AddedObject", obj);
        }
    }

}
