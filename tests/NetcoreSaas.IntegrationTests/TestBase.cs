using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using NetcoreSaas.Infrastructure.Data;
using NetcoreSaas.Infrastructure.Middleware.Tenancy.Strategy;
using NetcoreSaas.WebApi;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NetcoreSaas.Domain.Helpers;
using Microsoft.AspNetCore.Hosting;
using NetcoreSaas.Application.Contracts.Core.Users;
using NetcoreSaas.Application.Dtos.Core.Subscriptions;
using NetcoreSaas.Application.Dtos.Core.Tenants;
using NetcoreSaas.Application.Dtos.Core.Users;
using NetcoreSaas.Domain.Enums.Core.Users;

namespace NetcoreSaas.IntegrationTests
{
    public class TestBase : IDisposable
    {
        protected readonly HttpClient TestClient;
        protected readonly IConfiguration Configuration;
        protected UserDto CurrentUser;
        protected TenantSimpleDto CurrentTenant;
        protected List<SubscriptionProductDto> TestProducts { get; set; }

        private const string AdminEmail = "admin@testing.com";
        private const string TenantEmail = "company1@saasfrontends.com";

        public TestBase()
        {
            // Setup

            var projectDir = Directory.GetCurrentDirectory();
            var configPath = Path.Combine(projectDir, "appsettings.Testing.json");
            Configuration = new ConfigurationBuilder().AddJsonFile(configPath).Build();

            var applicationFactory = new WebApplicationFactory<Startup>()
                .WithWebHostBuilder(builder =>
                {
                    builder.UseEnvironment("Testing");
                    builder.ConfigureAppConfiguration((_, conf) =>
                    {
                        conf.AddJsonFile(configPath);
                    });
                    builder.ConfigureServices(services =>
                    {
                        services.RemoveAll(typeof(MasterDbContext));
                        var sp = services.BuildServiceProvider();
                        services.AddDbContext<MasterDbContext>(options => {
                            options.UseInMemoryDatabase(Guid.NewGuid().ToString());
                            options.UseInternalServiceProvider(sp);
                        });

                        services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
                        services.AddScoped<ITenantResolutionStrategy, HeaderResolutionStrategy>();
                    });
                });

            TestClient = applicationFactory.CreateClient();
            CleanDatabaseTests();
        }

        protected void AuthenticateAsAdmin()
        {
            AuthenticateAs(UserType.Admin);
        }

        protected void AuthenticateAsTenant()
        {
            AuthenticateAs(UserType.Tenant);
        }

        protected void AuthenticateWithUser(string email, string password)
        {
            AuthenticateAs(UserType.Tenant, email, password);
        }

        protected void Logout()
        {
            CurrentTenant = null;
            CurrentUser = null;

            TestClient.DefaultRequestHeaders.Remove("Authorization");
            TestClient.DefaultRequestHeaders.Remove("X-Tenant-Key");
        }

        private void AuthenticateAs(UserType userType, string email = null, string password = null)
        {
            var user = new UserLoginRequest()
            {
                Email = userType == UserType.Admin ? AdminEmail : TenantEmail,
                Password = "password",
            };
            if(!string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(password))
            {
                user.Email = email;
                user.Password = password;
            }
            var response = TestClient.PostAsJsonAsync(ApiCoreRoutes.Authentication.Login, user).Result;
            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                var error = response.Content.ReadAsStringAsync().Result;
                throw new Exception(error);
            }
            response.Content.ReadAsStringAsync().Wait();
            var responseContent = response.Content.ReadAsAsync<UserLoggedResponse>().Result;
            
            CurrentUser = responseContent.User;
            TestClient.DefaultRequestHeaders.Remove("Authorization");
            TestClient.DefaultRequestHeaders.Remove("X-Tenant-Key");
            TestClient.DefaultRequestHeaders.Remove("X-Workspace-Id");
            TestClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", (responseContent.Token));

            CurrentTenant = responseContent.User.CurrentTenant;
            TestClient.DefaultRequestHeaders.Add("X-Tenant-Key", CurrentTenant.Uuid.ToString());
            
            // Logged
            // var getwWorkspaces = TestClient.GetAsync(ApiRoutes.Workspace.GetAll).Result;
            // var tenantWorkspaces = getwWorkspaces.Content.ReadAsAsync<IEnumerable<WorkspaceDto>>().Result.ToList();
            // var currentWorkspace = tenantWorkspaces.FirstOrDefault(f => f.Default) ?? tenantWorkspaces.FirstOrDefault();
            if (responseContent.DefaultWorkspace != null)
                TestClient.DefaultRequestHeaders.Add("X-Workspace-Id", responseContent.DefaultWorkspace.Id.ToString());
        }

        public void Dispose()
        {
            // Teardown
            CleanDatabaseTests();
        }

        private void CleanDatabaseTests()
        {
            AuthenticateAsAdmin();

            // Delete created tenants
            var createdTenants = TestClient.GetAsync(ApiCoreRoutes.Tenant.GetAll).Result.Content.ReadAsAsync<List<TenantDto>>().Result;
            foreach (var item in createdTenants)
            {
                if (new[] { "Admin", "Tenant1" }.Contains(item.Name) == false)
                {
                    TestClient.DeleteAsync(ApiCoreRoutes.Tenant.Delete.Replace("{id}", item.Id.ToString())).Wait();
                }
            }

            // Delete created users
            var createdUsers = TestClient.GetAsync(ApiCoreRoutes.User.AdminGetAll).Result.Content.ReadAsAsync<List<UserDto>>().Result;
            foreach (var item in createdUsers)
            {
                if (!new[] { AdminEmail, TenantEmail }.Contains(item.Email))
                {
                    TestClient.DeleteAsync(ApiCoreRoutes.User.AdminDelete.Replace("{id}", item.Id.ToString())).Wait();
                }
            }

            // Delete all created products but main test products
            if (TestProducts != null && TestProducts.Count > 0)
            {
                var createdProducts = TestClient.GetAsync(ApiCoreRoutes.SubscriptionProduct.GetAll).Result;
                if (createdProducts.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    foreach (var item in createdProducts.Content.ReadAsAsync<List<SubscriptionProductDto>>().Result)
                    {
                        if (!TestProducts.Exists(f => f.Id == item.Id))
                        {
                            TestClient.DeleteAsync(ApiCoreRoutes.SubscriptionProduct.DeleteProduct.Replace("{id}", item.Id.ToString())).Wait();
                        }
                    }
                }
            }

            Logout();
        }
    }
}
