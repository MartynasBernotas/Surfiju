namespace DevKnowledgeBase.API.Models
{
    public class UserProfileModel
    {
        public string Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string? ProfilePicture { get; set; }  // If you use a profile image URL
    }
}
