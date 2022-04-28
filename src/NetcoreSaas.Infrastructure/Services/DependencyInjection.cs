using NetcoreSaas.Domain.Settings;
using NetcoreSaas.Infrastructure.Data;
using NetcoreSaas.Infrastructure.Middleware.Tenancy.Store;
using NetcoreSaas.Infrastructure.Middleware.Tenancy.Strategy;
using NetcoreSaas.Infrastructure.UnitOfWork;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using NetcoreSaas.Application.UnitOfWork;
using NetcoreSaas.Infrastructure.Services.Subscription;
using NetcoreSaas.Application.Services.Images;
using NetcoreSaas.Application.Services.Messages;
using NetcoreSaas.Infrastructure.Middleware.Tenancy;
using NetcoreSaas.Infrastructure.Services.Images;
using NetcoreSaas.Infrastructure.Services.Messages;
using NetcoreSaas.Application.Services.App;
using NetcoreSaas.Application.Services.Core;
using NetcoreSaas.Application.Services.Core.Subscriptions;
using NetcoreSaas.Application.Services.Core.Tenants;
using NetcoreSaas.Application.Services.Core.Users;
using NetcoreSaas.Infrastructure.Services.App;
using NetcoreSaas.Infrastructure.Services.Core;

namespace NetcoreSaas.Infrastructure.Services
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            //Authentication
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = configuration["Jwt:Issuer"],
                        ValidAudience = configuration["Jwt:Issuer"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]))
                    };
                });

            //Automapper
            //MapperBuilder.Build();
            //Mapper.Reset(); // Otherwise throws an exception when testing
            //Mapper.Initialize(cfg => cfg.AddProfile<ApplicationProfile>());
            //services.AddAutoMapper();

            ProjectConfiguration.GlobalConfiguration = ProjectConfiguration.CreateConfiguration(configuration);
            //Sql Database
            switch (ProjectConfiguration.GlobalConfiguration.DatabaseProvider)
            {
                case DatabaseProvider.PostgreSql:
                    services.AddDbContext<MasterDbContext>(options => options.UseNpgsql(ProjectConfiguration.GlobalConfiguration.MasterDbContext));
                    services.AddDbContext<AppDbContext>(options => options.UseNpgsql(ProjectConfiguration.GlobalConfiguration.MasterDbContext));
                    break;
                case DatabaseProvider.MySql:
                    services.AddDbContext<MasterDbContext>();
                    services.AddDbContext<AppDbContext>();
                    break;
            }

            //var sqlConnectionConfiguration = new SqlConnectionConfiguration(ProjectConfiguration.GlobalConfiguration.MasterDbContext);
            //services.AddSingleton(sqlConnectionConfiguration);

            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            //ORM -> EF Core, Dapper or ADO.NET
            //services.AddScoped<IUserService, UserService_ADO>();
            //services.AddScoped<IUserService, UserService_Dapper>();
            //services.AddScoped<IUserService, IRepository<User>>();
            // services.AddScoped<IRepository<Tenant>, TenantService>();
            // services.AddScoped<IRepository<TenantUser>, UserService_EF>();

            services.AddSingleton<ITenantResolutionStrategy, HeaderResolutionStrategy>();
            services.AddTransient<ITenantStore, ClaimsTenantStore>();
            // services.AddTransient<ITenantAccessService, TenantAccessService>();

            // services.AddMultiTenancy()
            // .WithResolutionStrategy<HeaderResolutionStrategy>()
            // .WithStore<InDatabaseTenantStore>();
            // .WithStore<InMemoryTenantStore>();
            // services.AddMvc().AddGenericControllers<CoreDbContext, IEntity>(typeof(GenericController<>));

            //SignalR
            services.AddSignalR();

            //GraphQL
            //services.AddSingleton<ContextServiceLocator>();
            //services.AddSingleton<IDocumentExecuter, DocumentExecuter>();
            //services.AddSingleton<ExpenseItemQuery>();
            //services.AddSingleton<ExpenseItemType>();

            //var sp = services.BuildServiceProvider();
            //services.AddSingleton<ISchema>(new MyGraphQLSchemas(new FuncDependencyResolver(type => sp.GetService(type))));

            services.AddHttpContextAccessor();
            services.AddTransient<IMasterUnitOfWork, MasterUnitOfWork>();
            services.AddTransient<ITenantAccessService, TenantAccessService>();
            services.AddScoped<IAppUnitOfWork, AppUnitOfWork>();

            services.AddApplicationServices(configuration);

            return services;
        }
 
        private static void AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Master application services
            services.AddTransient<ITenantService, TenantService>();
            services.AddTransient<IUserService, UserService>();

            // App services
            services.AddTransient<IWorkspaceService, WorkspaceService>();
            services.AddSingleton<IOpticalCharacterRecognitionService, OpticalCharacterRecognitionMicrosoft>();

            services.AddTransient<IContractService, ContractService>();
            services.AddTransient<ILinkService, LinkService>();


            // Helper services
            services.Configure<EmailSettings>(configuration.GetSection("EmailSettings"));
            services.Configure<OpticalCharacterRecognitionSettings>(configuration.GetSection("OpticalCharacterRecognitionSettings"));
            
            if (ProjectConfiguration.GlobalConfiguration.EmailProvider == EmailProvider.Postmark)
                services.AddScoped<IEmailService, EmailPostmarkService>();
            
            // services.Configure<LandbotSettings>(configuration.GetSection("LandbotSettings"));
            // services.AddTransient<IChatbotService, ChatbotLandbotService>();

            // Subscription
            services.Configure<SubscriptionSettings>(configuration.GetSection("SubscriptionSettings"));
            if (ProjectConfiguration.GlobalConfiguration.SubscriptionProvider == SubscriptionProvider.Stripe)
                services.AddScoped<ISubscriptionService, SubscriptionStripeService>();
            //else if (ProjectConfiguration.GlobalConfiguration.SubscriptionProvider == SubscriptionProvider.Paddle)
            //    services.AddScoped<ISubscriptionService, SubscriptionPaddleService>();
        }
    }
}
