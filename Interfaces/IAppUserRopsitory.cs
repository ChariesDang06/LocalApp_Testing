using LocalEdu_App.Models;
using System.Diagnostics.Eventing.Reader;

namespace LocalEdu_App.Interfaces
{
    public interface IAppUserRopsitory
    {
        ICollection<AppUser> GetAppUsers();

        AppUser GetAppUserById(string id);
        AppUser GetAppUserByEmail(string email);
        AppUser GetAppUserByPassword(string pass);
        AppUser GetAppUserByUserName(string appUserName);
        AppUser GetAppUserByFirstName(string firstName);
        AppUser GetAppUserByLastName(string lastName);
        AppUser GetAppUserByInstitution(string institution);
        AppUser GetAppUserByAvtSrc(string avtSrc);
        bool AppUserExist(string appUserId);
        bool CreateAppUser(AppUser appUser);
        bool UpdateAppUser(AppUser appUser);
        bool DeleteAppUser(AppUser appUser);
        bool Save();
    }
}
