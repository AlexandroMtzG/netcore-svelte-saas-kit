using NetcoreSaas.Application.Dtos.App.Employees;

namespace NetcoreSaas.Application.Contracts.App.Employees
{
    public class CreateEmployeesRequest
    {
        public EmployeeDto[] Employees { get; set; }
    }
}