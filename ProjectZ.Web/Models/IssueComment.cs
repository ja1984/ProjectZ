namespace ProjectZ.Web.Models
{
    public class IssueComment
    {
        public string Id { get; set; }
        public string IssueId { get; set; }
        public string UserId { get; set; }
        public string Comment { get; set; }
    }
}