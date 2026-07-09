using System.Net.Http.Json;
using LuckySpinFE.Dto;

namespace LuckySpinFE.Services;

public class LuckySpinApiClient(HttpClient http)
{
    public async Task<SpinResponse?> SpinAsync(SpinRequest request)
    {
        var response = await http.PostAsJsonAsync("api/spin/spinaction", request);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<SpinResponse>();
    }

    public async Task<List<CampaignDto>?> GetCampaignsByRewardCodeAsync(string rewardCode)
    {
        var response = await http.GetAsync($"api/spin/getcampaign/{rewardCode}");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<List<CampaignDto>>();
    }

    private record WinnerSessionResponse(string winner_id);

    public async Task<string> CreateWinnerSessionAsync(PostCustomerRequest request)
    {
        var response = await http.PostAsJsonAsync("api/WinnerSessions", request);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new Exception($"API lỗi ({(int)response.StatusCode}): {error}");
        }

        var result = await response.Content.ReadFromJsonAsync<WinnerSessionResponse>();

        return result?.winner_id
            ?? throw new Exception("API không trả về winner_id.");
    }

    private class AssignPrizeWinnerRequest
    {
        public string WinnerId { get; set; } = string.Empty;
        public string PrizeId { get; set; } = string.Empty;
    }

    /// <summary>
    /// Gán 1 winner (customerId) vào 1 prize cụ thể để xác nhận nhận thưởng.
    /// Chỉ xử lý được 1 prize mỗi lần gọi - nếu khách có nhiều phần thưởng, gọi hàm này nhiều lần.
    /// </summary>
    public async Task AssignWinnerToPrizeAsync(string winnerId, string prizeId)
    {
        var response = await http.PostAsJsonAsync("api/WinnerSessions/AssignWinnerToPrize", new AssignPrizeWinnerRequest
        {
            WinnerId = winnerId,
            PrizeId = prizeId
        });

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new Exception($"API lỗi ({(int)response.StatusCode}): {error}");
        }
    }
}