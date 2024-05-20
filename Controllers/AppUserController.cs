using AutoMapper;
using LocalEdu_App.Dto;
using LocalEdu_App.Interfaces;
using LocalEdu_App.Models;
using LocalEdu_App.Repository;
using Microsoft.AspNetCore.Mvc;

namespace LocalEdu_App.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppUserController : Controller
    {
        private readonly IAppUserRopsitory _appUserRopsitory;
        private readonly IMapper _mapper;

        public AppUserController(IAppUserRopsitory appUserRopsitory, IMapper mapper)
        {
            _appUserRopsitory = appUserRopsitory;
            _mapper = mapper;
        }
        
        [HttpGet]
        [ProducesResponseType(200, Type =typeof(IEnumerable<AppUser>))]
        public IActionResult GetAppUser() {
            var users = _mapper.Map<List<AppUserDto>>(_appUserRopsitory.GetAppUsers());
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok(users);
        }

        [HttpGet("{appUserId}")]
        [ProducesResponseType(200, Type =typeof(AppUser))]
        [ProducesResponseType(400)]

        public IActionResult GetAppUser(string appUserId)
        {
            if (!_appUserRopsitory.AppUserExist(appUserId)) { return NotFound(); }
            var appUser = _mapper.Map<AppUserDto>(_appUserRopsitory.GetAppUserById(appUserId));

            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            return Ok(appUser);
        }
        [HttpPost]
        [ProducesResponseType(200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public IActionResult CreateAppUser( [FromBody] AppUserDto appUserCreate)
        {
            if (appUserCreate == null)
                return BadRequest(ModelState);

            var appUser = _appUserRopsitory.GetAppUsers()
                .Where(c => c.Email.Trim() == appUserCreate.Email.Trim())
                .FirstOrDefault();

            if (appUser != null)
            {
                ModelState.AddModelError("", "User already exists");
                return StatusCode(422, ModelState);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var appUserMap = _mapper.Map<AppUser>(appUserCreate);

            if (!_appUserRopsitory.CreateAppUser(appUserMap))
            {
                ModelState.AddModelError("", "Something went wrong while saving");
                return StatusCode(500, ModelState);
            }

            return Ok("Successfully created");
        }

        [HttpPut("{appUserId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public IActionResult UpdateAppUser(string appUserId, [FromBody] AppUserDto updateAppUser)
        {
            if (updateAppUser == null)
                return BadRequest(ModelState);

            if (appUserId != updateAppUser.Id)
                return BadRequest(ModelState);

            if (!_appUserRopsitory.AppUserExist(appUserId))
                return NotFound();

            if (!ModelState.IsValid)
                return BadRequest();

            var appUserMap = _mapper.Map<AppUser>(updateAppUser);

            if (!_appUserRopsitory.UpdateAppUser(appUserMap))
            {
                ModelState.AddModelError("", "Something went wrong updating category");
                return StatusCode(500, ModelState);
            }

            return Ok("Successfully updated");
        }

        [HttpDelete("{appUserId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public IActionResult DeleteAppUser(string appUserId)
        {
            if (!_appUserRopsitory.AppUserExist(appUserId))
            {
                return NotFound();
            }

            var appUserToDelete = _appUserRopsitory.GetAppUserById(appUserId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!_appUserRopsitory.DeleteAppUser(appUserToDelete))
            {
                ModelState.AddModelError("", "Something went wrong deleting category");
                return StatusCode(500, ModelState); // Ensure this line is present
            }

            return Ok("Successfully deleted");
        }
    }
}
