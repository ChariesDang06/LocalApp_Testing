using LocalEdu_App.Data;
using LocalEdu_App.Interfaces;
using LocalEdu_App.Models;
using Microsoft.EntityFrameworkCore;

namespace LocalEdu_App.Repository
{
    public class PartRepository : IPartRepository
    {
        private readonly DataContext _context;

        public PartRepository(DataContext context)
        {
            _context = context;
        }
        public bool CreatePart(Part part)
        {
            _context.Add(part);
            return Save();
        }

        public bool DeletePart(Part part)
        {
            _context.Remove(part);
            return Save();
        }

        public Part GetPartById(string id)
        {
            return _context.Parts.Where(p => p.Id == id).FirstOrDefault();
        }

        public ICollection<Part> GetParts()
        {
            return _context.Parts.OrderBy(p => p.Id).ToList();
        }

        public ICollection<Part> GetPartsOfTopic(string topicId)
        {
            return _context.Parts.Where(p => p.TopicId == topicId).ToList();
        }

        public bool PartExist(string partId)
        {
            return _context.Parts.Any(p => p.Id == partId);
        }

        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0 ? true : false;
        }

        public bool UpdatePart(Part part)
        {
            _context.Update(part);
            return Save(); 
        }
    }
}
