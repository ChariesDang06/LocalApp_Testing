using AutoMapper;
using LocalEdu_App.Dto;
using LocalEdu_App.Interfaces;
using LocalEdu_App.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace LocalEdu_App.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserProgressController : Controller
    {
        private readonly IUserProgressRepository _userProgressRepository;
        private readonly IMapper _mapper;
        private readonly IAppUserRopsitory _appUserRopsitory;
        private readonly ITopicRepository _topicRepository;

        public UserProgressController(IUserProgressRepository userProgressRepository, IMapper mapper, IAppUserRopsitory appUserRopsitory, ITopicRepository topicRepository)
        {
            _userProgressRepository = userProgressRepository;
            _mapper = mapper;
            _appUserRopsitory = appUserRopsitory;
            _topicRepository = topicRepository;
        }
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<UserProgress>))]
        public IActionResult GetUserProgress()
        {
            var userProgressses = _mapper.Map<List<UserProgressDto>>(_userProgressRepository.GetUserProgresses());
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(userProgressses);
        }
        [HttpGet("{userId}/userProgresses")]
        [ProducesResponseType(200, Type = typeof(UserProgress))]
        [ProducesResponseType(400)]
        public IActionResult GetUserProgressOfUser (string userId)
        {
            var userProgresses = _mapper.Map<List<UserProgressDto>>(_userProgressRepository.GetUserProgressOfUser(userId));
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(userProgresses);
        }

        [HttpPost]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public IActionResult CreateUserProgress([FromQuery] string  userId, [FromQuery] string topicId, [FromBody] UserProgressDto userProgressCreate)
        {
            if (userProgressCreate == null)
                return BadRequest(ModelState);
            if (!_appUserRopsitory.AppUserExist(userId))
            {
                ModelState.AddModelError("", "user does not exist");
                return StatusCode(422, ModelState);
            }
            if (!_topicRepository.TopicExist(topicId))
            {
                ModelState.AddModelError("", "topic does not exist");
                return StatusCode(422, ModelState);
            }
            var userProgresses = _userProgressRepository.GetUserProgresses().FirstOrDefault(up => up.UserId == userId && up.TopicId == topicId);
            if (userProgresses != null)
            {
                ModelState.AddModelError("", "user's progress already exists");
                return StatusCode(422, ModelState);
            }
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var userProgressMap = _mapper.Map<UserProgress>(userProgressCreate);
            userProgressMap.AppUser = _appUserRopsitory.GetAppUserById(userId);
            userProgressMap.Topic = _topicRepository.GetTopicById(topicId);
            if(!_userProgressRepository.CreateUserProgress(userProgressMap))
            {
                ModelState.AddModelError("", "Something went wrong while savin");
                return StatusCode(500, ModelState);
            }

            return Ok("Successfully created");

        }
        [HttpPut("{userId}/{topicId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public IActionResult UpdateUserProgress(string userId, string topicId, [FromBody] UserProgressDto userProgressUpdated)
        {
            if (userProgressUpdated == null)
                return BadRequest(ModelState);
            if (!_appUserRopsitory.AppUserExist(userId))
            {
                ModelState.AddModelError("", "user does not exist");
                return StatusCode(422, ModelState);
            }
            if (!_topicRepository.TopicExist(topicId))
            {
                ModelState.AddModelError("", "topic does not exist");
                return StatusCode(422, ModelState);
            }
            if (!ModelState.IsValid)
                return BadRequest();
            var userProgressMap = _mapper.Map<UserProgress>(userProgressUpdated);
           

            if (!_userProgressRepository.UpdateUserProgress(userProgressMap))
            {
                ModelState.AddModelError("", "Something went wrong updating category");
                return StatusCode(500, ModelState);
            }

            return Ok("Successfully updated");

        }
        [HttpDelete("/DeleteTopicProgressOfUser/{userId}/{topicId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public IActionResult DeleteTopicProgressOfUser(string userId, string topicId)
        {
            if(!_userProgressRepository.UserProgressExist(userId, topicId))
                return BadRequest(ModelState);
            UserProgress topicProgressToDelete = _userProgressRepository.GetUserProgressOfUser(userId).FirstOrDefault(up => up.TopicId == topicId);
            if (!ModelState.IsValid)
                return BadRequest();
            if (!_userProgressRepository.DeleteUserProgress(topicProgressToDelete))
            {
                ModelState.AddModelError("", "error deleting reviews");
                return StatusCode(500, ModelState);
            }
            return NoContent();

        }
    }
}
