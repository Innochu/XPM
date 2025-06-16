
using Microsoft.AspNetCore.Http;

namespace XPMTest.Application.Service.Implementation
{
    public class UserService : IUserService
    {
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepository;
        public UserService(IMapper mapper, IUserRepository userRepository)
        {
            _mapper = mapper;
            _userRepository = userRepository;
        }

        public async Task<ApiResponse<UserRequestDto>> CreateUser(UserCreateDto dto)
        {
            
            var existingUser = await _userRepository.FindAsync(u => u.Email == dto.Email);
            if (existingUser != null)
            {
                return ApiResponse<UserRequestDto>.Failed(
                    "A user with this email already exists.",
                    StatusCodes.Status409Conflict,
                    new List<string> { "Duplicate email." });
            }

            var user = _mapper.Map<Customer>(dto);
            user.Id = Guid.NewGuid().ToString();
            user.Status =  XPMTest.Application.Service.Constants.ResponseMessages.Active;

            var result = await _userRepository.AddAsync(user);

            if (result == null)
            {
                return ApiResponse<UserRequestDto>.Failed(
                    "User creation failed.",
                    StatusCodes.Status500InternalServerError,
                    new List<string> { "Database insert failed." });
            }

            var responseDto = _mapper.Map<UserRequestDto>(result);
            return ApiResponse<UserRequestDto>.Success(responseDto, "User created successfully", StatusCodes.Status201Created);
        }

        public async Task<ApiResponse<PaginatedList<UserRequestDto>>> GetUsers(QueryModel queryModel)
        {
            var users = await _userRepository.GetAllAsync();
            if (!users.Any())
                return ApiResponse<PaginatedList<UserRequestDto>>.Failed("No user was obtained from the database", StatusCodes.Status404NotFound, new List<string>());

            var userDtos = users.Select(user => _mapper.Map<UserRequestDto>(user)).AsQueryable();

            var pagedUsers = userDtos.Paginate(queryModel.PageIndex, queryModel.PageSize);

            var paginatedList = new PaginatedList<UserRequestDto>(pagedUsers.ToList(), userDtos.Count(), queryModel.PageIndex, queryModel.PageSize);

            return ApiResponse<PaginatedList<UserRequestDto>>.Success(paginatedList, "Users retrieved successfully", StatusCodes.Status200OK);
        }


        public async Task<ApiResponse<UserRequestDto>> GetUserbyId(string id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user is null)
                return ApiResponse<UserRequestDto>.Failed("User with the given id not found", StatusCodes.Status404NotFound, new List<string> { });
            var reponseDto = _mapper.Map<UserRequestDto>(user);

            return new ApiResponse<UserRequestDto>(reponseDto, "User retrieved successfully");
        }

    }
}