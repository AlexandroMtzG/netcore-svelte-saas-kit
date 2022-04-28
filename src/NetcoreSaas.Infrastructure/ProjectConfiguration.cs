using System;
using Microsoft.Extensions.Configuration;

namespace NetcoreSaas.Infrastructure
{
    public class ProjectConfiguration
    {
        public static ProjectConfiguration GlobalConfiguration;
        public DatabaseProvider DatabaseProvider { get; set; }
        public MultiTenancy MultiTenancy { get; set; }
        public EmailProvider EmailProvider { get; set; }
        public SubscriptionProvider SubscriptionProvider { get; set; }
        public string MasterDbContext { get { return DbContext.Replace("[DATABASE]", Database); } }
        public string Database { get; set; }
        public bool RequiresVerification { get; set; }

        private IConfiguration _configuration;

        public static ProjectConfiguration CreateConfiguration(IConfiguration configuration)
        {
            ProjectConfiguration conf = new ProjectConfiguration
            {
                _configuration = configuration
            };
            conf.DatabaseProvider = conf.GetDatabaseProvider();
            conf.MultiTenancy = conf.GetMultiTenancy();
            conf.EmailProvider = conf.GetEmailProvider();
            conf.SubscriptionProvider = conf.GetSubscriptionProvider();
            conf.Database = conf.GetMasterDatabase();
            conf.RequiresVerification = Convert.ToBoolean(configuration["ProjectConfiguration:RequiresVerification"]);
            return conf;
        }

        public string GetTenantContext(Guid tenantUuid)
        {
            return MultiTenancy switch
            {
                MultiTenancy.SingleDatabase => MasterDbContext,
                MultiTenancy.DatabasePerTenant => DbContext.Replace("[DATABASE]", _configuration["App:Name"] + "-" + tenantUuid.ToString()),
                _ => MasterDbContext,
            };
        }

        private string DbContext
        {
            get
            {
                return DatabaseProvider switch
                {
                    DatabaseProvider.PostgreSql => _configuration.GetConnectionString(Constants.DbContextPostgreSql),
                    DatabaseProvider.MySql => _configuration.GetConnectionString(Constants.DbContextMySql),
                    _ => _configuration.GetConnectionString(Constants.DbContextPostgreSql),
                };
            }
        }

        private DatabaseProvider GetDatabaseProvider()
        {
            return (_configuration["ProjectConfiguration:DatabaseProvider"]) switch
            {
                "PostgreSQL" => DatabaseProvider.PostgreSql,
                "MySQL" => DatabaseProvider.MySql,
                _ => DatabaseProvider.PostgreSql,
            };
        }

        private MultiTenancy GetMultiTenancy()
        {
            return (_configuration["ProjectConfiguration:MultiTenancy"]) switch
            {
                "SingleDatabase" => MultiTenancy.SingleDatabase,
                "DatabasePerTenant" => MultiTenancy.DatabasePerTenant,
                _ => MultiTenancy.SingleDatabase,
            };
        }

        private EmailProvider GetEmailProvider()
        {
            return (_configuration["ProjectConfiguration:EmailProvider"]) switch
            {
                "Postmark" => EmailProvider.Postmark,
                _ => EmailProvider.Postmark,
            };
        }

        private SubscriptionProvider GetSubscriptionProvider()
        {
            return (_configuration["ProjectConfiguration:SubscriptionProvider"]) switch
            {
                "Stripe" => SubscriptionProvider.Stripe,
                "Paddle" => SubscriptionProvider.Paddle,
                _ => SubscriptionProvider.Stripe,
            };
        }

        private string GetMasterDatabase()
        {
            return _configuration["ProjectConfiguration:MasterDatabase"];
        }
    }

    public static class Constants
    {
        public static string DbContextPostgreSql = "DbContext_PostgreSQL";
        public static string DbContextMySql = "DbContext_MySQL";
    }

    public enum DatabaseProvider
    {
        PostgreSql, MySql
    }

    public enum MultiTenancy
    {
        SingleDatabase, DatabasePerTenant
    }

    public enum EmailProvider
    {
        Postmark
    }

    public enum SubscriptionProvider
    {
        Stripe, Paddle
    }
}