using LocalEdu_App.Models;

namespace LocalEdu_App.Interfaces
{
    public interface IUserProgressRepository
    {
        ICollection<UserProgress> GetUserProgresses();
        ICollection<UserProgress> GetUserProgressOfUser(string userId);
        UserProgress GetUserProgress(string userId, string topicId);

        bool UserProgressExist(string userId, string topicId);
        bool CreateUserProgress( UserProgress userProgress);
        bool UpdateUserProgress( UserProgress userProgress);
        bool DeleteUserProgress( UserProgress userProgress);
        bool Save();
    }
}
