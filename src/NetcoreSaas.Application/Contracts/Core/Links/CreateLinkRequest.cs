namespace NetcoreSaas.Application.Contracts.Core.Links
{
    public class CreateLinkRequest
    {
        public string Email { get; set; }
        public string WorkspaceName { get; set; }
        public bool AsProvider { get; set; }
    }
}