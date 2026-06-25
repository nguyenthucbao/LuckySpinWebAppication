using System.Net.Http.Json;
using LuckySpinFE.Dto;

namespace LuckySpinFE.Services;

public class LuckySpinApiClient(HttpClient http)
{
    public async Task<SpinResponse?> SpinAsync(SpinRequest request)
    {
        var response = await http.PostAsJsonAsync("/api/spin/spinaction", request);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<SpinResponse>();
    }
    public async Task<List<CampaignDto>?> GetCampaignsByRewardCodeAsync(string rewardCode)
    {
        var response = await http.GetAsync($"/api/spin/{rewardCode}");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<List<CampaignDto>>();
    }

    //public async Task CreateWinnerSessionAsync(PostCustomerRequest request)
    //{
    //    var response = await http.PostAsJsonAsync("/api/WinnerSessions", request);
    //    response.EnsureSuccessStatusCode();
    //}

    public async Task<string> CreateWinnerSessionAsync(PostCustomerRequest request)
    {
        var response = await http.PostAsJsonAsync("api/winner-sessions", request);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new Exception($"API lỗi ({(int)response.StatusCode}): {error}");
        }

        var result = await response.Content.ReadFromJsonAsync<CreateWinnerSessionResponse>();
        return result?.SessionId ?? throw new Exception("API không trả về sessionId.");



    }
}