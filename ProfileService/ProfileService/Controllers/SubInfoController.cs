using Microsoft.AspNetCore.Mvc;
using ProfileService.Data;

namespace ProfileService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SubInfoController : ControllerBase
    {
        private readonly IProfileRepository _profileRepository;
        public SubInfoController(IProfileRepository profileRepository)
        {
            _profileRepository = profileRepository;
        }

        [HttpGet("{Id}")]
        public async Task<IActionResult> Get(Guid Id)
        {
            var subInfo = await _profileRepository.GetUserNameAndAvatarByIdAsync(Id);
            if (subInfo is null)
            {
                subInfo = await _profileRepository.GetGroupNameAndAvatarByIdAsync(Id);
            }
            if (subInfo is null)
            {
                return NotFound();
            }
            return Ok(subInfo);
        }
        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string name)
        {
            var subInfos = await _profileRepository.SearchUserByName(name);
            return Ok(subInfos);
        }
    }
}
