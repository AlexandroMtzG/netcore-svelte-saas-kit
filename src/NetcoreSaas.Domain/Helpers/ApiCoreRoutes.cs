namespace NetcoreSaas.Domain.Helpers
{
    public static class ApiCoreRoutes
    {
        private const string Base = "api/";
        public static class Authentication
        {
            private const string Controller = nameof(Authentication);

            public const string AdminImpersonate = Base + Controller + "/Admin/Impersonate/{userId}";

            public const string Login = Base + Controller + "/Login";
            public const string Register = Base + Controller + "/Register";
            public const string Verify = Base + Controller + "/Verify";
            public const string Reset = Base + Controller + "/Reset/{email}";
        }

        public static class Tenant
        {
            private const string Controller = nameof(Tenant);

            public const string AdminGetAll = Base + Controller + "/Admin/GetAll";
            public const string AdminDelete = Base + Controller + "/Admin/Delete/{id}";
            public const string AdminGetFeatures = Base + Controller + "/GetFeatures/{id}";
            public const string AdminGetProducts = Base + Controller + "/GetProducts/{id}";

            public const string GetAll = Base + Controller + "/GetAll";
            public const string Get = Base + Controller + "/Get/{id}";
            public const string GetFeatures = Base + Controller + "/GetFeatures";
            public const string GetCurrentUsage = Base + Controller + "/GetCurrentUsage/{type}";
            public const string GetCurrent = Base + Controller + "/GetCurrent";
            public const string Create = Base + Controller + "/Create";
            public const string Update = Base + Controller + "/Update/{id}";
            public const string UpdateImage = Base + Controller + "/UpdateImage/{id}";
            public const string Delete = Base + Controller + "/Delete/{id}";
        }

        public static class User
        {
            private const string Controller = nameof(User);
            
            public const string AdminGetAll = Base + Controller + "/Admin/GetAll";
            public const string AdminDelete = Base + Controller + "/Admin/Delete/{id}";
            public const string AdminUpdatePassword = Base + Controller + "/Admin/UpdatePassword/{userId}/{password}";

            public const string GetClaims = Base + Controller + "/GetClaims";
            public const string GetUser = Base + Controller + "/GetUser/{id}";
            public const string GetUserAvatar = Base + Controller + "/GetUserAvatar/{id}";
            public const string GetCurrent = Base + Controller + "/GetCurrent";
            public const string Update = Base + Controller + "/Update/{id}";
            public const string UpdateAvatar = Base + Controller + "/UpdateAvatar";
            public const string UpdateLocale = Base + Controller + "/UpdateLocale";
            public const string UpdatePassword = Base + Controller + "/UpdatePassword";
            public const string UpdateDefaultTenant = Base + Controller + "/UpdateDefaultTenant/{userId}/{tenantId}";
            public const string DeleteMe = Base + Controller + "/DeleteMe";
        }

        public static class TenantUsers
        {
            private const string Controller = nameof(TenantUsers);

            public const string AdminGetAll = Base + Controller + "/Admin/GetAll";
            public const string AdminDelete = Base + Controller + "/Admin/Delete/{id}";

            public const string GetAll = Base + Controller + "/GetAll";
            public const string Get = Base + Controller + "/Get/{id}";
            public const string Update = Base + Controller + "/Update/{id}";
            public const string ResetToken = Base + Controller + "/ResetToken/{id}";
            public const string Delete = Base + Controller + "/Delete/{id}";
        }

        public static class TenantUserInvitation
        {
            private const string Controller = nameof(TenantUserInvitation);

            public const string GetInvitation = Base + Controller + "/GetInvitation/{invitationLink}";
            public const string GetInviteUrl = Base + Controller + "/GetInviteURL/{linkUuid}";
            public const string GetInvitationSettings = Base + Controller + "/GetInvitationSettings/{tenantId}";
            public const string InviteUser = Base + Controller + "/InviteUser";
            public const string RequestAccess = Base + Controller + "/RequestAccess/{linkUuid}";
            public const string AcceptInvitation = Base + Controller + "/AcceptInvitation/{invitationLink}";
            public const string AcceptUser = Base + Controller + "/AcceptUser/{tenantUserId}";
            public const string UpdateInvitationSettings = Base + Controller + "/UpdateInvitationSettings";
        }

        public static class SubscriptionManager
        {
            private const string Controller = nameof(SubscriptionManager);
            
            public const string GetCurrentSubscription = Base + Controller + "/GetCurrentSubscription";
            public const string GetUpcomingInvoice = Base + Controller + "/GetUpcomingInvoice";
            public const string GetCoupon = Base + Controller + "/GetCoupon/{couponId}/{currency}";
            public const string CreateCustomerPortalSession = Base + Controller + "/CreateCustomerPortalSession";
            public const string UpdateSubscription = Base + Controller + "/UpdateSubscription";
            public const string UpdateCardToken = Base + Controller + "/UpdateCardToken/{cardToken}";
            public const string CreateCardToken = Base + Controller + "/CreateCardToken";
            public const string UpdateCard = Base + Controller + "/UpdateCard";
            public const string CancelSubscription = Base + Controller + "/CancelSubscription";
        }

        public static class SubscriptionProduct
        {
            private const string Controller = nameof(SubscriptionProduct);

            public const string CreateProduct = Base + Controller + "/CreateProduct";
            public const string CreatePrice = Base + Controller + "/CreatePrice";
            public const string CreateFeature = Base + Controller + "/CreateFeature";
            public const string UpdateProduct = Base + Controller + "/UpdateProduct/{id}";
            public const string UpdatePrice = Base + Controller + "/UpdatePrice/{id}";
            public const string UpdateFeature = Base + Controller + "/UpdateFeature/{id}";
            public const string DeleteProduct = Base + Controller + "/DeleteProduct/{id}";
            public const string DeletePrice = Base + Controller + "/DeletePrice/{id}";
            public const string DeleteFeature = Base + Controller + "/DeleteFeature/{id}";

            public const string GetAll = Base + Controller + "/GetAll";
            public const string GetProduct = Base + Controller + "/GetProduct/{id}";
            public const string GetPrice = Base + Controller + "/GetPrice/{id}";
            public const string GetFeature = Base + Controller + "/GetFeature/{id}";
        }

        public static class Workspace
        {
            private const string Controller = nameof(Workspace);

            public const string AdminGetAll = Base + Controller + "/Admin/GetAll";
            public const string AdminDelete = Base + Controller + "/Admin/Delete/{id}";

            public const string GetAll = Base + Controller + "/GetAll";
            public const string Get = Base + Controller + "/Get/{id}";
            public const string Create = Base + Controller + "/Create";
            public const string Update = Base + Controller + "/Update/{id}";
            public const string UpdateType = Base + Controller + "/UpdateType/{id}/{type}";
            public const string Delete = Base + Controller + "/Delete/{id}";
            public const string AddUser = Base + Controller + "/AddUser/{id}";
            public const string RemoveUser = Base + Controller + "/RemoveUser/{id}";
        }

        public static class Link
        {
            private const string Controller = nameof(Link);

            public const string GetAllLinked = Base + Controller + "/GetAllLinked";
            public const string GetAllPending = Base + Controller + "/GetAllPending";
            public const string GetAllProviders = Base + Controller + "/GetAllProviders";
            public const string GetAllClients = Base + Controller + "/GetAllClients";
            public const string GetLinkUsers = Base + Controller + "/GetLinkUsers/{linkId}";
            public const string GetInvitation = Base + Controller + "/GetInvitation/{id}";
            public const string CreateInvitation = Base + Controller + "/CreateInvitation";
            public const string RejectInvitation = Base + Controller + "/RejectInvitation/{id}";
            public const string SearchUser = Base + Controller + "/SearchUser/{email}";
            public const string SearchMember = Base + Controller + "/SearchMember/{email}/{workspaceName}";
            public const string GetMember = Base + Controller + "/GetMember/{linkId}/{email}";
            public const string GetWorkspace = Base + Controller + "/GetWorkspace/{linkId}";
            public const string Get = Base + Controller + "/Get/{id}";
            public const string Create = Base + Controller + "/Create";
            public const string AcceptOrReject = Base + Controller + "/AcceptOrReject/{id}";
            public const string Delete = Base + Controller + "/Delete/{id}";
        }

        public static class Setup
        {
            private const string Controller = nameof(Setup);

            public const string GetPostmarkTemplates = Base + Controller + "/GetPostmarkTemplates";
            public const string CreatePostmarkTemplates = Base + Controller + "/CreatePostmarkTemplates";
        }
    }
}
