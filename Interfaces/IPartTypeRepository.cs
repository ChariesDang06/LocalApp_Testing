using LocalEdu_App.Models;

namespace LocalEdu_App.Interfaces
{
    public interface IPartTypeRepository
    {
        ICollection<PartType> GetPartTypes();
        PartType GetPartTypeById(string partTypeId);
        ICollection<PartType> GetPartTypeByPartId(string partId);
        bool PartTypeExist(string partTypeId);
        bool CreatePartType(PartType partType);
        bool UpdatePartType(PartType partType);
        bool DeletePartType(PartType partType);
        bool Save();
    }
}
