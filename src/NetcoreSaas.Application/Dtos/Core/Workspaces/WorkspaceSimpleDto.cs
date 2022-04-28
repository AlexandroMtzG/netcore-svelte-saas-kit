using System;
using NetcoreSaas.Domain.Enums.Core.Tenants;

namespace NetcoreSaas.Application.Dtos.Core.Workspaces
{
    public class WorkspaceSimpleDto : AppEntityDto
    {
        public string Name { get; set; }
        // TODO: Add your own workspace properties below
        public WorkspaceType Type { get; set; }
        public string BusinessMainActivity { get; set; }
        public string RegistrationNumber { get; set; }
        public DateTime? RegistrationDate { get; set; }
        public string AddressStreet { get; set; }
        public string AddressExterior { get; set; }
        public string AddressInterior { get; set; }
        public string AddressNeighborhood { get; set; }
        public string AddressCity { get; set; }
        public string AddressZip { get; set; }
        public string AddressState { get; set; }
        public string AddressCountry { get; set; }
        public bool Default { get; set; }
    }
}