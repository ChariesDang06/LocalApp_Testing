using LocalEdu_App.Models;
using System.Diagnostics.Eventing.Reader;

namespace LocalEdu_App.Interfaces
{
    public interface ITopicRepository
    {
        ICollection<Topic> GetTopics();
        Topic GetTopicById(string id);
        ICollection<Part> GetPartByTopic(string topicId);
        bool TopicExist(string topicId);
        bool CreateTopic(Topic topic);
        bool UpdateTopic(Topic topic);
        bool DeleteTopic(Topic topic);
        bool Save();

    }
}
