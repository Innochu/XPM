namespace XPMTest.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthenticationServices _authenticationService;
        private readonly SignInManager<AppUser> _signInManager;
        public AuthController(SignInManager<AppUser> signInManager, UserManager<AppUser> userManager, IAuthenticationServices authenticationService)
        {
            _authenticationService = authenticationService;
            _signInManager = signInManager;
        }

        [HttpPost("Register")]
        [ProducesResponseType(typeof(BaseResponse<RegisterResponseDto>), Microsoft.AspNetCore.Http.StatusCodes.Status200OK)]
        public async Task<IActionResult> Register([FromBody] AppUserCreateDto appUserCreateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(BaseResponse<string>.Failure("Invalid model state.", Microsoft.AspNetCore.Http.StatusCodes.Status400BadRequest.ToString()));
            }
            var callBackUrl = Request.Scheme + "://";
            var registrationResult = await _authenticationService.RegisterAsync(appUserCreateDto);

            if (registrationResult.Data != null)
            {
                return Ok(registrationResult);
            }
            else
            {
                return BadRequest(new { Message = registrationResult.Message });
            }
        }

        [HttpPost("Login")]
        [ProducesResponseType(typeof(BaseResponse<LoginResponseDto>), Microsoft.AspNetCore.Http.StatusCodes.Status200OK)]
        //[ProducesResponseType(Microsoft.AspNetCore.Http.StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Login([FromBody] AppUserLoginDto loginDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<string>.Failed("Invalid model state.", Microsoft.AspNetCore.Http.StatusCodes.Status400BadRequest, ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage).ToList()));
            }
            return Ok(await _authenticationService.LoginAsync(loginDTO));
        }
    }
}
