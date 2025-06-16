namespace XPMTest.Application.Service.Interface
{
    public interface IAuthenticationServices
    {
        Task<BaseResponse<RegisterResponseDto>> RegisterAsync(AppUserCreateDto appUserCreateDto);
        Task<BaseResponse<LoginResponseDto>> LoginAsync(AppUserLoginDto loginDTO);
    }
}
