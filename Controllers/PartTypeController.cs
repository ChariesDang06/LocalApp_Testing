using AutoMapper;
using LocalEdu_App.Dto;
using LocalEdu_App.Interfaces;
using LocalEdu_App.Models;
using Microsoft.AspNetCore.Mvc;

namespace LocalEdu_App.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PartTypeController : Controller
    {
        private readonly IPartTypeRepository _partTypeRepository;
        private readonly IMapper _mapper;
        private readonly IPartRepository _partRepository;

        public PartTypeController(IPartTypeRepository partTypeRepository,IPartRepository partRepository, IMapper mapper)
        {
            _partTypeRepository = partTypeRepository;
            _partRepository = partRepository;
            _mapper = mapper;
        }
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<PartType>))]
        public IActionResult GetPartTypes()
        {
            var partTypes = _mapper.Map<List<PartTypeDto>>(_partTypeRepository.GetPartTypes());
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok(partTypes);
        }
        [HttpGet("{partTypeId}")]
        [ProducesResponseType(200, Type = typeof(PartType))]
        [ProducesResponseType(400)]
        public IActionResult GetPartType(string partTypeId)
        {
            if (!_partTypeRepository.PartTypeExist(partTypeId))
                return NotFound();

            var partType = _mapper.Map<PartTypeDto>(_partTypeRepository.GetPartTypeById(partTypeId));

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(partType);
        }
        [HttpPost]
        [ProducesResponseType(200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public IActionResult CreatePartType([FromQuery] string partId, [FromBody] PartTypeDto partTypeCreate)
        {
            if (partTypeCreate == null)
                return BadRequest(ModelState);
            var partTypes = _partTypeRepository.GetPartTypes().Where(c => c.Id == partTypeCreate.Id).FirstOrDefault();

            if (partTypes != null)
            {
                ModelState.AddModelError("", "Part's type already exists");
                return StatusCode(422, ModelState);
            }
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var partTypeMap = _mapper.Map<PartType>(partTypeCreate);
            partTypeMap.Part = _partRepository.GetPartById(partId);

            if (!_partTypeRepository.CreatePartType(partTypeMap))
            {
                ModelState.AddModelError("", "Something went wrong while savin");
                return StatusCode(500, ModelState);
            }

            return Ok("Successfully created");
        }
        [HttpDelete("{partTypeId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public IActionResult DeletePartType(string partTypeId)
        {
            if (!_partTypeRepository.PartTypeExist(partTypeId))
            {
                return NotFound();
            }

            var partTypeToDelete = _partTypeRepository.GetPartTypeById(partTypeId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!_partTypeRepository.DeletePartType(partTypeToDelete))
            {
                ModelState.AddModelError("", "Something went wrong deleting owner");
            }

            return NoContent();
        }
    }
}
