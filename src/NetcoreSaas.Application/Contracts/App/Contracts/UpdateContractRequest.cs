using NetcoreSaas.Domain.Enums.App.Contracts;

namespace NetcoreSaas.Application.Contracts.App.Contracts
{
    public class UpdateContractRequest
    {
        public string Name { get; set; }
        public ContractStatus Status { get; set; }
        public string Description { get; set; }
        public string File { get; set; }
    }
}
