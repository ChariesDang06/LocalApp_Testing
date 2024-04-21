using AutoMapper;
using LocalEdu_App.Dto;
using LocalEdu_App.Interfaces;
using LocalEdu_App.Models;
using LocalEdu_App.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LocalEdu_App.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class TopicController : Controller
    {
        private readonly ITopicRepository _topicRepository;
        private readonly IMapper _mapper;

        public TopicController(ITopicRepository topicRepository, IMapper mapper)
        {
            _topicRepository = topicRepository;
            _mapper = mapper;
        }
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<AppUser>))]
        public IActionResult GetTopic()
        {
            var users = _mapper.Map<List<TopicDto>>(_topicRepository.GetTopics());
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok(users);
        }

        [HttpGet("{topicId}")]
        [ProducesResponseType(200, Type = typeof(Topic))]
        [ProducesResponseType(400)]

        public IActionResult GetAppUser(string topicId)
        {
            if (!_topicRepository.TopicExist(topicId)) { return NotFound(); }
            var toipc = _mapper.Map<TopicDto>(_topicRepository.GetTopicById(topicId));

            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            return Ok(toipc);
        }

        [HttpGet("{topicId}/parts")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Part>))]
        [ProducesResponseType(400)]
        public IActionResult GetPartByTopic(string topicId)
        {
            if (!_topicRepository.TopicExist(topicId))
            {
                return NotFound();
            }

            var parts = _mapper.Map<List<PartDto>>(
                _topicRepository.GetPartByTopic(topicId));

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(parts);
        }
        [HttpPost]
        [ProducesResponseType(200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public IActionResult CreateAppUser([FromBody] TopicDto topicCreate)
        {
            if (topicCreate == null)
                return BadRequest(ModelState);

            var topic = _topicRepository.GetTopics()
                .Where(c => c.Id == topicCreate.Id.Trim())
                .FirstOrDefault();

            if (topic != null)
            {
                ModelState.AddModelError("", "Topic already exists");
                return StatusCode(422, ModelState);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var topicMap = _mapper.Map<Topic>(topicCreate);

            if (!_topicRepository.CreateTopic(topicMap))
            {
                ModelState.AddModelError("", "Something went wrong while saving");
                return StatusCode(500, ModelState);
            }

            return Ok("Successfully created");
        }

        [HttpPut("{topicId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public IActionResult UpdateCategory(string topicId, [FromBody] TopicDto updateTopic)
        {
            if (updateTopic == null)
                return BadRequest(ModelState);

            if (topicId != updateTopic.Id)
                return BadRequest(ModelState);

            if (!_topicRepository.TopicExist(topicId))
                return NotFound();

            if (!ModelState.IsValid)
                return BadRequest();

            var topicMap = _mapper.Map<Topic>(updateTopic);

            if (!_topicRepository.UpdateTopic(topicMap))
            {
                ModelState.AddModelError("", "Something went wrong updating category");
                return StatusCode(500, ModelState);
            }

            return Ok("Successfully updated");
        }
        [HttpDelete("{topicId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public IActionResult DeleteTopic(string topicId)
        {
            if (!_topicRepository.TopicExist(topicId))
            {
                return NotFound();
            }

            var topicToDelete = _topicRepository.GetTopicById(topicId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!_topicRepository.DeleteTopic(topicToDelete))
            {
                ModelState.AddModelError("", "Something went wrong deleting category");
            }

            return Ok("Successfully deleted");
        }
    }
}
