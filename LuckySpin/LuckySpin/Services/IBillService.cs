using LuckySpin.Dto;
using LuckySpin.Models;

namespace LuckySpin.Services;

public interface IBillService
{
    Task<GetBillResponseDto> CreateBillAsync(PostBillRequest request);
    Task<DbRewardCodeDto> GenerateCodeOnBillAsync(Bill input);
}