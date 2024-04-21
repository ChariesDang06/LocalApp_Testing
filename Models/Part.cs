namespace LocalEdu_App.Models
{
    public class Part
    {
        public string Id { get; set; }
        public string TopicId { get; set; }
        public string ViewStatus { get; set; }
        public string Title { get; set; }

        public Topic Topic { get; set; }
        public ICollection<PartType> PartTypes { get; set; }
    }
}
