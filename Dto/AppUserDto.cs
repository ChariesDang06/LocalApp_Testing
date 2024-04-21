using System.ComponentModel.DataAnnotations.Schema;

namespace LocalEdu_App.Dto
{
    public class AppUserDto
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string Pass { get; set; }
        public string AppUserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Institution { get; set; }
        public string AvtSrc { get; set; }

        [NotMapped]
        public string AccessToken { get; set; }


    }
}
