using PostmarkDotNet.Model;

namespace NetcoreSaas.Application.Dtos.Core.Emails
{
    public class EmailTemplateDto
    {
        public string Alias { get; set; }

        public string Subject { get; set; }

        public string Name { get; set; }

        public string HtmlBody { get; set; }

        public bool Created { get; set; }
        public bool Active { get; set; }
        public TemplateType Type { get; set; }
        public long TemplateId { get; set; }
        public int AssociatedServerId { get; set; }
    }
}

