namespace DevKnowledgeBase.UI.Models
{
    public class UserProfileModel
    {
        public string Id { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? ProfilePicture { get; set; } = string.Empty;
    }
}
