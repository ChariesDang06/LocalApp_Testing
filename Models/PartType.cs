namespace LocalEdu_App.Models
{
    public class PartType
    {
        public string Id { get; set; }
        public string PartId { get; set; }
        public string MediaPath { get; set; }
        public string Content { get; set; }
        public string ViewStatus { get; set; }

        public Part Part { get; set; }
    }
}
