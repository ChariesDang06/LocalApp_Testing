using LocalEdu_App.Data;
using LocalEdu_App.Interfaces;
using LocalEdu_App.Models;
using System.Diagnostics.Metrics;

namespace LocalEdu_App.Repository
{
    public class AppUserRepository : IAppUserRopsitory
    {
        private readonly DataContext _context;
        public AppUserRepository(DataContext context)
        {
            _context = context;
            
        }

        public bool AppUserExist(string appUserId)
        {
            return _context.AppUsers.Any(p => p.Id == appUserId);
        }

        public bool CreateAppUser(AppUser appUser)
        {
            _context.Add(appUser);
            return Save();
        }

        public bool DeleteAppUser(AppUser appUser)
        {
            _context.Remove(appUser);
            return Save();
        }

        

        public AppUser GetAppUserByAvtSrc(string avtSrc)
        {
            return _context.AppUsers.Where(p => p.AvtSrc == avtSrc).FirstOrDefault(); ;
        }

        public AppUser GetAppUserByEmail(string email)
        {
            return _context.AppUsers.Where(p => p.Email == email).FirstOrDefault(); ;
        }

        public AppUser GetAppUserByFirstName(string firstName)
        {
            return _context.AppUsers.Where(p => p.FirstName == firstName).FirstOrDefault(); ;
        }

        public AppUser GetAppUserById(string id)
        {
            return _context.AppUsers.Where(p => p.Id == id).FirstOrDefault();
        }

        public AppUser GetAppUserByInstitution(string institution)
        {
            return _context.AppUsers.Where(p => p.Institution == institution).FirstOrDefault(); ;
        }

        public AppUser GetAppUserByLastName(string lastName)
        {
            return _context.AppUsers.Where(p => p.LastName == lastName).FirstOrDefault(); ;
        }

        public AppUser GetAppUserByPassword(string pass)
        {
            return _context.AppUsers.Where(p => p.Pass == pass).FirstOrDefault(); ;
        }

        public AppUser GetAppUserByUserName(string appUserName)
        {
            return _context.AppUsers.Where(p => p.AppUserName == appUserName).FirstOrDefault(); ;
        }

        public ICollection<AppUser> GetAppUsers()
        {
            return _context.AppUsers.OrderBy(p => p.Id).ToList();
        }

        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0 ? true : false;
        }

        public bool UpdateAppUser(AppUser appUser)
        {
            _context.Update(appUser);
            return Save();
        }
    }
}
