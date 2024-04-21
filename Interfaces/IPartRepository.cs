using LocalEdu_App.Models;

namespace LocalEdu_App.Interfaces
{
    public interface IPartRepository
    {
        ICollection<Part> GetParts();
        ICollection<Part> GetPartsOfTopic(string topicId);
        Part GetPartById(string id);
        bool PartExist(string partId);
        bool CreatePart(Part part);
        bool UpdatePart(Part part);
        bool DeletePart(Part part);
        bool Save();
    }
}
