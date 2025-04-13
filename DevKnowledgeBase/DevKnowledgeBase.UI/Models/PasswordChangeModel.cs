namespace DevKnowledgeBase.UI.Models
{
    public class PasswordChangeModel
    {
        public string CurrentPassword { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
