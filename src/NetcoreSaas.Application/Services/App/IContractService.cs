using System.Threading.Tasks;
using NetcoreSaas.Application.Contracts.App.Contracts;
using NetcoreSaas.Domain.Models.App.Contracts;
using NetcoreSaas.Domain.Models.Core.Links;
using NetcoreSaas.Domain.Models.Core.Users;

namespace NetcoreSaas.Application.Services.App
{
    public interface IContractService
    {
        Task<Contract> Create(User currentUser, Link link, CreateContractRequest request);
    }
}