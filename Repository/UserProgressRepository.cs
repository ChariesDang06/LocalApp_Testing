using LocalEdu_App.Data;
using LocalEdu_App.Interfaces;
using LocalEdu_App.Models;

namespace LocalEdu_App.Repository
{
    public class UserProgressRepository  : IUserProgressRepository
    {
        private readonly DataContext _context;

        public UserProgressRepository( DataContext context)
        {
            _context = context;
        }

        public bool CreateUserProgress(UserProgress userProgress)
        {
            _context.Add(userProgress); return Save();
        }

        public bool DeleteUserProgress(UserProgress userProgress)
        {
            _context.Remove(userProgress); return Save();
        }

        public UserProgress GetUserProgress(string userId, string topicId)
        {
            return _context.UserProgresses.FirstOrDefault(u => u.UserId == userId && u.TopicId == topicId);
        }

        public ICollection<UserProgress> GetUserProgresses()
        {
            return _context.UserProgresses.ToList();
        }

        public ICollection<UserProgress> GetUserProgressOfUser(string userId)
        {
            return _context.UserProgresses.Where(up=>up.UserId == userId).ToList();
        }

        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0 ? true : false;
        }

        public bool UpdateUserProgress(UserProgress userProgress)
        {
            _context.Update(userProgress);
            return Save();
        }

        public bool UserProgressExist(string userId, string topicId)
        {
            return _context.UserProgresses.Any( u => u.UserId ==userId && u.TopicId == topicId);
        }
    }
}
