using XPMTest.Application.Service.Constants;

namespace XPMTest.Application.Service.Implementation
{
    public class AuthenticationServices : IAuthenticationServices
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ILogger<AuthenticationServices> _logger;
        private readonly IMapper _mapper;
        private readonly IConfiguration _config;
        private readonly IUserRepository _userRepository;

        public AuthenticationServices(IConfiguration config,
                                        UserManager<AppUser> userManager,
                                        SignInManager<AppUser> signInManager,
                                        IMapper mapper,
                                        ILogger<AuthenticationServices> logger,
                                        IUserRepository userRepository)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _mapper = mapper;
            _config = config;
            _userRepository = userRepository;
        }

        public async Task<BaseResponse<RegisterResponseDto>> RegisterAsync(AppUserCreateDto appUserCreateDto)
        {

            var user = await _userManager.FindByEmailAsync(appUserCreateDto.Email);
            if (user != null)
            {
                return BaseResponse<RegisterResponseDto>.Failure("User with this email already exists.", StatusCodes.DuplicateRecord);
            }
            var appUser = new AppUser()
            {
                FirstName = appUserCreateDto.FirstName,
                LastName = appUserCreateDto.LastName,
                Email = appUserCreateDto.Email,
                PhoneNumber = appUserCreateDto.PhoneNumber,
                UserName = appUserCreateDto.Email,
            };

            try
            {
                var result = await _userManager.CreateAsync(appUser, appUserCreateDto.Password);
                var userr = await _userManager.FindByEmailAsync(appUser.Email);
                if (result.Succeeded)
                {
                    var role = await _userManager.AddToRoleAsync(appUser, "User");

                    var response = new RegisterResponseDto
                    {
                        Id = appUser.Id,
                        Email = appUser.Email,
                        PhoneNumber = appUser.PhoneNumber,
                        FirstName = appUser.FirstName,
                        LastName = appUser.LastName,

                    };

                    return BaseResponse<RegisterResponseDto>.Success("User registered successfully. Please click on the link sent to your email to confirm your account", StatusCodes.Successful, response);
                }
                return BaseResponse<RegisterResponseDto>.Failure("Unable to create wallet", StatusCodes.FatalError);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while adding a manager " + ex.InnerException);
                return BaseResponse<RegisterResponseDto>.Failure("Error creating user.", StatusCodes.GeneralError);
            }

        }

        public async Task<BaseResponse<LoginResponseDto>> LoginAsync(AppUserLoginDto loginDTO)
        {
            try
            {
                var response = new LoginResponseDto();
                var userr = await _userManager.FindByEmailAsync(loginDTO.Email);
                if (userr == null)
                {
                    return BaseResponse<LoginResponseDto>
                       .Failure(ResponseMessages.UserNotFound, StatusCodes.NoRecordFound);
                }
                var result = await _signInManager.CheckPasswordSignInAsync(userr, loginDTO.Password, lockoutOnFailure: false);

                switch (result)
                {
                    case { Succeeded: true }:
                        var role = (await _userManager.GetRolesAsync(userr)).FirstOrDefault();

                        response.JWToken = GenerateJwtToken(userr, role);
                        response.RefreshToken = GenerateRefreshToken();
                        response.IsLoggedIn = true;
                        response.Name = userr.FirstName + " " + userr.LastName;

                        userr.RefreshToken = response.RefreshToken;
                        await _userManager.UpdateAsync(userr);

                        return BaseResponse<LoginResponseDto>
                   .Success(ResponseMessages.OperationSuccessful, StatusCodes.Successful, response);
                    case { IsLockedOut: true }:
                        return BaseResponse<LoginResponseDto>.Failure($"Account is locked out. Please try again later or contact support." +
                          $" You can unlock your account after {_userManager.Options.Lockout.DefaultLockoutTimeSpan.TotalMinutes} minutes.", StatusCodes.GeneralError);

                    case { IsNotAllowed: true }:
                        return BaseResponse<LoginResponseDto>.Failure("Login failed.", StatusCodes.BadRequest);

                    default:
                        return BaseResponse<LoginResponseDto>.Failure("Login failed. Invalid Phonenumber or password.", StatusCodes.BadRequest);
                }
            }
            catch (Exception ex)
            {
                return BaseResponse<LoginResponseDto>.Failure("Some error occurred while logging in." + ex.InnerException, StatusCodes.FatalError);
            }
        }
        private string GenerateJwtToken(AppUser contact, string roles)
        {
            var jwtSettings = _config.GetSection("JwtSettings:Secret").Value;
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings ?? "jwtSettings cannot be null here"));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Create claims list instead of array to handle conditional claims
            var claims = new List<Claim>
    {
        new Claim(JwtRegisteredClaimNames.Sub, contact.Id),
        new Claim(JwtRegisteredClaimNames.Email, contact.Email ?? "Email cannot be null here"),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        new Claim(JwtRegisteredClaimNames.GivenName, contact.FirstName + " " + contact.LastName),
        new Claim(ClaimTypes.Name, contact.UserName)
    };

            if (!string.IsNullOrEmpty(roles))
            {
                claims.Add(new Claim(ClaimTypes.Role, roles));
            }
            else
            {
                claims.Add(new Claim(ClaimTypes.Role, "User"));
            }

            var token = new JwtSecurityToken(
                issuer: _config["JwtSettings:ValidIssuer"],
                audience: _config["JwtSettings:ValidAudience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(int.Parse(_config["JwtSettings:AccessTokenExpiration"] ?? "30")),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private static string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using (var numberGenerator = RandomNumberGenerator.Create())
            {
                numberGenerator.GetBytes(randomNumber);
            }
            return Convert.ToBase64String(randomNumber);
        }

        public async Task<BaseResponse<string>> ValidateTokenAsync(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(_config.GetSection("JwtSettings:Secret").Value);

                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = _config["JwtSettings:ValidIssuer"],
                    ValidAudience = _config["JwtSettings:ValidAudience"],
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                };

                var principal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken securityToken);
                var emailClaim = principal.FindFirst(JwtRegisteredClaimNames.Email)?.Value;

                return BaseResponse<string>.Success("token is valid", StatusCodes.NoRecordFound, emailClaim);
            }
            catch (SecurityTokenExpiredException)
            {
                _logger.LogWarning("Token has expired");
                return BaseResponse<string>.Failure("Token has expired", StatusCodes.BadRequest);
            }
            catch (SecurityTokenException ex)
            {
                _logger.LogError(ex, "Token validation failed");
                var errorList = new List<string> { ex.Message };
                return BaseResponse<string>.Failure("Token validation failed.", StatusCodes.NoRecordFound);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during token validation");
                // var errorList = new List<string> { ex.Message };
                return BaseResponse<string>.Failure("Error occurred during token validation", StatusCodes.FatalError);
            }
        }
    }
}