using System.Linq;

namespace NetcoreSaas.Domain.Helpers
{
    public static class StringHelper
    {
        public static bool IsDigitsOnly(string str)
        {
            return str.All(c => c >= '0' && c <= '9');
        }
    }
}