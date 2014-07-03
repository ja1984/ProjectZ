using System;

namespace ProjectZ.Web.Models
{
    public class IssueComment
    {
        public int Id { get; set; }
        public string Comment { get; set; }
        public DateTime Posted { get; set; }
        public IssueCommentUser User { get; set; }
    }

    public class IssueCommentUser
    {
        public IssueCommentUser()
        {

        }

        public IssueCommentUser(User user)
        {
            UserId = user.Id;
            DisplayName = user.UserName;
            Image = user.GetImage();
        }

        public string UserId { get; set; }
        public string DisplayName { get; set; }
        public string Image { get; set; }
    }
}