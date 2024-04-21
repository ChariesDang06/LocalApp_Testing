namespace LocalEdu_App.Models
{
    public class UserProgress
    {
        public string UserId { get; set; }
        public string TopicId { get; set; }
        public int Score { get; set; }

        public AppUser AppUser { get; set; }
        public Topic Topic { get; set; }
    }
}
