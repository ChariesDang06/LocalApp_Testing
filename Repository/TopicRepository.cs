using LocalEdu_App.Data;
using LocalEdu_App.Interfaces;
using LocalEdu_App.Models;

namespace LocalEdu_App.Repository
{
    public class TopicRepository : ITopicRepository
    {
        private readonly DataContext _context;
        public TopicRepository(DataContext context)
        {
            _context = context;

        }
        public bool CreateTopic(Topic topic)
        {
            _context.Add(topic);
            return Save();
        }

        public bool DeleteTopic(Topic topic)
        {
            _context.Remove(topic);
            return Save();
        }

        public ICollection<Part> GetPartByTopic(string topicId)
        {
            return _context.Parts.Where(p => p.TopicId == topicId).ToList();
        }

        public Topic GetTopicById(string id)
        {
            return _context.Topics.Where(p => p.Id == id).FirstOrDefault();
        }

        public ICollection<Topic> GetTopics()
        {
            return _context.Topics.OrderBy(p => p.Id).ToList();
        }

        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0 ? true : false;
        }

        public bool TopicExist(string topicId)
        {
            return _context.Topics.Any(p => p.Id == topicId);
        }

        public bool UpdateTopic(Topic topic)
        {
            _context.Update(topic);
            return Save();
        }
    }
}
