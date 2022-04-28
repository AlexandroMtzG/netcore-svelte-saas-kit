using Microsoft.AspNetCore.Http;

namespace NetcoreSaas.Infrastructure.Extensions
{
    public class ContextServiceLocator
    {
        public IHttpContextAccessor HttpContextAccessor { get; }
        public ContextServiceLocator(IHttpContextAccessor httpContextAccessor)
        {
            HttpContextAccessor = httpContextAccessor;
        }
    }
}