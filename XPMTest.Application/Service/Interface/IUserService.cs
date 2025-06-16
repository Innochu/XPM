namespace XPMTest.Application.Service.Interface
{
    public interface IUserService
    {
        Task<ApiResponse<PaginatedList<UserRequestDto>>> GetUsers(QueryModel queryModel);
        Task<ApiResponse<UserRequestDto>> GetUserbyId(string id);
        Task<ApiResponse<UserRequestDto>> CreateUser(UserCreateDto dto);
    }
}
