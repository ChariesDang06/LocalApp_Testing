namespace LocalEdu_App.Models
{
    public class Topic
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Brief { get; set; }

        public ICollection<Part> Parts { get; set; }
        public ICollection<UserProgress> UserProgresses { get; set; }
    }
}
