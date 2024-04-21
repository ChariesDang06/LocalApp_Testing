using AutoMapper;
using LocalEdu_App.Dto;
using LocalEdu_App.Interfaces;
using LocalEdu_App.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LocalEdu_App.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class PartController : Controller
    {
        private readonly IPartRepository _partRepository;
        private readonly ITopicRepository _topicRepository;
        private readonly IMapper _mapper;

        public PartController(IPartRepository partRepository, ITopicRepository topicRepository, IMapper mapper)
        {
            _partRepository = partRepository;
            _topicRepository = topicRepository;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Part>))]
        public IActionResult GetParts()
        {
            var parts = _mapper.Map<List<PartDto>>(_partRepository.GetParts());

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(parts);
        }
    
    
        [HttpPost]
        [ProducesResponseType(200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public IActionResult CreateAppUser([FromQuery] string topicId, [FromBody] PartDto partCreated)
        {
            if (partCreated == null)
                return BadRequest(ModelState);

            var part = _partRepository.GetParts()
                .Where(c => c.Id == partCreated.Id.Trim())
                .FirstOrDefault();

            if (part != null)
            {
                ModelState.AddModelError("", "Part already exists");
                return StatusCode(422, ModelState);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var partMap = _mapper.Map<Part>(partCreated);
            partMap.Topic = _topicRepository.GetTopicById(topicId);

            if (!_partRepository.CreatePart(partMap))
            {
                ModelState.AddModelError("", "Something went wrong while saving");
                return StatusCode(500, ModelState);
            }

            return Ok("Successfully created");
        }
        [HttpGet("{partId}")]
        [ProducesResponseType(200, Type = typeof(Part))]
        [ProducesResponseType(400)]
        public IActionResult GetPart(string partId)
        {
            if (!_partRepository.PartExist(partId))
                return NotFound();

            var part = _mapper.Map<PartDto>(_partRepository.GetPartById(partId));

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(part);
        }
        [HttpPut("{partId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public IActionResult UpdateCategory(string partId, [FromBody] PartDto updatePart)
        {
            if (updatePart == null)
                return BadRequest(ModelState);

            if (partId != updatePart.Id)
                return BadRequest(ModelState);

            if (!_partRepository.PartExist(partId))
            {
                ModelState.AddModelError("", "Part does not exist");
                return StatusCode(422, ModelState);
            }

            if (!ModelState.IsValid)
                return BadRequest();

            var partMap = _mapper.Map<Part>(updatePart);

            if (!_partRepository.UpdatePart(partMap))
            {
                ModelState.AddModelError("", "Something went wrong updating category");
                return StatusCode(500, ModelState);
            }

            return Ok("Successfully updated");
        }
        [HttpDelete("{partId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public IActionResult DeletePart(string partId)
        {
            if (!_partRepository.PartExist(partId))
            {
                return NotFound();
            }

            var partToDelete = _partRepository.GetPartById(partId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!_partRepository.DeletePart(partToDelete))
            {
                ModelState.AddModelError("", "Something went wrong deleting category");
            }

            return Ok("Successfully deleted");
        }

    }
}
