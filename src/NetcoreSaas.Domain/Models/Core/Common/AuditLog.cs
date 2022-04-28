using System;

namespace NetcoreSaas.Domain.Models.Core.Common
{
    public class AuditLog : MasterEntity
    {
        public DateTime Date { get; set; }
        public string Table { get; set; }
        public string KeyValues { get; set; }
        public string OldValues { get; set; }
        public string NewValues { get; set; }
    }
}
