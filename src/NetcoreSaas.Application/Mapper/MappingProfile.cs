using AutoMapper;
using NetcoreSaas.Application.Contracts.Core.Subscriptions;
using NetcoreSaas.Application.Dtos.App.Contracts;
using NetcoreSaas.Application.Dtos.App.Employees;
using NetcoreSaas.Application.Dtos.Core.Links;
using NetcoreSaas.Application.Dtos.Core.Subscriptions;
using NetcoreSaas.Application.Dtos.Core.Tenants;
using NetcoreSaas.Application.Dtos.Core.Users;
using NetcoreSaas.Application.Dtos.Core.Workspaces;
using NetcoreSaas.Domain.Models.App.Contracts;
using NetcoreSaas.Domain.Models.App.Employees;
using NetcoreSaas.Domain.Models.Core.Links;
using NetcoreSaas.Domain.Models.Core.Subscriptions;
using NetcoreSaas.Domain.Models.Core.Tenants;
using NetcoreSaas.Domain.Models.Core.Users;
using NetcoreSaas.Domain.Models.Core.Workspaces;

namespace NetcoreSaas.Application.Mapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Master Models
            CreateMap<User, UserDto>();
            CreateMap<User, UserSimpleDto>();
            CreateMap<Tenant, TenantDto>();
            CreateMap<Tenant, TenantSimpleDto>();
            CreateMap<TenantDto, TenantSimpleDto>();
            
            CreateMap<TenantUser, TenantUserDto>();
            CreateMap<TenantProduct, TenantProductDto>();
            CreateMap<TenantProduct, TenantProductSimpleDto>();
            CreateMap<TenantJoinSettings, TenantJoinSettingsDto>();
            
            CreateMap<SubscriptionProduct, SubscriptionProductDto>();
            CreateMap<SubscriptionPrice, SubscriptionPriceDto>();
            CreateMap<SubscriptionFeature, SubscriptionFeatureDto>();
            CreateMap<SubscriptionProductDto, SubscriptionProduct>();
            CreateMap<SubscriptionPriceDto, SubscriptionPrice>();
            CreateMap<SubscriptionFeatureDto, SubscriptionFeature>();

            CreateMap<SubscriptionUpdateProductRequest, SubscriptionProduct>();
            CreateMap<SubscriptionUpdatePriceRequest, SubscriptionPrice>();

            // App Models
            
            // Workspaces
            CreateMap<Workspace, WorkspaceSimpleDto>();
            CreateMap<Workspace, WorkspaceDto>();
            
            CreateMap<WorkspaceUserDto, WorkspaceUser>();
            CreateMap<WorkspaceUser, WorkspaceUserDto>();
            CreateMap<WorkspaceDto, Workspace>();
            
            // Contracts - Model to Dto
            CreateMap<ContractActivity, ContractActivityDto>();
            CreateMap<Contract, ContractDto>()
                .ForMember(f=>f.HasFile,
                    m => m.MapFrom(d=>!string.IsNullOrEmpty(d.File)));
            CreateMap<ContractEmployee, ContractEmployeeDto>();
            CreateMap<ContractMember, ContractMemberDto>();
            // Contracts - Dto to Model
            CreateMap<ContractActivityDto, ContractActivity>();
            CreateMap<ContractDto, Contract>();
            CreateMap<ContractEmployeeDto, ContractEmployee>();
            CreateMap<ContractMemberDto, ContractMember>();

            // Employees - Model to Dto
            CreateMap<Employee, EmployeeDto>();
            // Employees - Dto to Model
            CreateMap<EmployeeDto, Employee>();
            
            // Links - Model to Dto
            CreateMap<Link, LinkDto>();
            CreateMap<LinkInvitation, LinkInvitationDto>();
            // Links - Dto to Model
            CreateMap<LinkDto, Link>();
            CreateMap<LinkInvitationDto, LinkInvitation>();
        }
    }
}
