namespace XPMTest.API.Controllers
{
    
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }
       
        [HttpPost("all-users")]
        [ProducesResponseType(typeof(IEnumerable<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AllUser(QueryModel queryModel)
        {
            var users = await _userService.GetUsers(queryModel);
            return Ok(users);
        }

        [HttpGet("get-user-by-Id/{id}")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetUserById(string id)
        {
            var user = await _userService.GetUserbyId(id);
            if (user == null)
            {
                return NotFound(new { Message = $"User with ID {id} not found." });
            }
            return Ok(user);
        }

        [HttpPost("create-user")]
        [ProducesResponseType(typeof(ApiResponse<UserRequestDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateUser([FromBody] UserCreateDto dto)
        {
            var result = await _userService.CreateUser(dto);
            if (!result.Succeeded)
                return StatusCode(result.StatusCode, result);

            return CreatedAtAction(nameof(GetUserById), new { id = result.Data.Id }, result);
        }

    }
}