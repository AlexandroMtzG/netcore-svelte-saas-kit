using NetcoreSaas.Domain.Models.Core;

namespace NetcoreSaas.Domain.Models.App.Employees
{
    public class Employee : AppWorkspaceEntity
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
    }
}