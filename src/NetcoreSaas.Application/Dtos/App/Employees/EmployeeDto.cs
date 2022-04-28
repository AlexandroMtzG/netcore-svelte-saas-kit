using NetcoreSaas.Application.Dtos.Core;

namespace NetcoreSaas.Application.Dtos.App.Employees
{
    public class EmployeeDto : AppWorkspaceEntityDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
    }
}