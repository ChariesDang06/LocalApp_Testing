using LocalEdu_App.Data;
using LocalEdu_App.Interfaces;
using LocalEdu_App.Models;
using Microsoft.EntityFrameworkCore;

namespace LocalEdu_App.Repository
{
    public class PartTypeRepository : IPartTypeRepository
    {
        private readonly DataContext _context;

        public PartTypeRepository(DataContext context)
        {
            _context = context;

        }

        public bool CreatePartType(PartType partType)
        {
            _context.Add(partType);
            return Save();
        }

        public bool DeletePartType(PartType partType)
        {
            _context.Remove(partType);
            return Save();
        }

        public PartType GetPartTypeById(string partTypeId)
        {
            return _context.PartTypes.Where(p => p.Id == partTypeId).FirstOrDefault();
        }

        public ICollection<PartType> GetPartTypeByPartId(string partId)
        {
            throw new NotImplementedException();
        }

        public ICollection<PartType> GetPartTypes()
        {
            return _context.PartTypes.ToList();
        }

        public bool PartTypeExist(string partTypeId)
        {
            return _context.PartTypes.Any(p => p.Id == partTypeId);
        }

        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0 ? true : false;
        }

        public bool UpdatePartType(PartType partType)
        {
            _context.Update(partType);
            return Save();
        }
    }

       
    
}
