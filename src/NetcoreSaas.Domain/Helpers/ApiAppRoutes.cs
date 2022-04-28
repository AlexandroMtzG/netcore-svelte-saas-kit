namespace NetcoreSaas.Domain.Helpers
{
    public static class ApiAppRoutes
    {
        private const string Base = "api/";
        public static class Contract
        {
            private const string Controller = nameof(Contract);

            public const string GetAllByStatusFilter = Base + Controller + "/GetAllByStatusFilter/{filter}";
            public const string GetAllByLink = Base + Controller + "/GetAllByLink/{linkId}";
            public const string Get = Base + Controller + "/Get/{id}";
            public const string Create = Base + Controller + "/Create";
            public const string Download = Base + Controller + "/Download/{id}";
            public const string Send = Base + Controller + "/Send/{id}";
            public const string Update = Base + Controller + "/Update/{id}";
            public const string Delete = Base + Controller + "/Delete/{id}";
        }
        public static class Employee
        {
            private const string Controller = nameof(Employee);

            public const string GetAll = Base + Controller + "/GetAll";
            public const string Get = Base + Controller + "/Get/{id}";
            public const string Create = Base + Controller + "/Create";
            public const string CreateMultiple = Base + Controller + "/CreateMultiple";
            public const string Update = Base + Controller + "/Update/{id}";
            public const string Delete = Base + Controller + "/Delete/{id}";
        }
    }
}
