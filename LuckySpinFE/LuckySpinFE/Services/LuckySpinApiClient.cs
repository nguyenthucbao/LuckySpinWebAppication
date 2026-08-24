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


    public async Task<GetAccountByPhoneRespone?> GetCustomerByPhoneAsync(string phone)
    {
        var response = await http.GetAsync($"api/WinnerSessions/GetByPhoneNumber/{phone}");

        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            return null;

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new Exception($"API lỗi ({(int)response.StatusCode}): {error}");
        }

        return await response.Content.ReadFromJsonAsync<GetAccountByPhoneRespone>();
    }

    public async Task<string?> GetKey(string keyId)
    {
        var response = await http.GetAsync($"api/WinnerSessions/getkey/{keyId}");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }

    public async Task<bool?> CheckCampaignEndDate(string sprizeId)
    {
        var response = await http.GetAsync($"api/WinnerSessions/CheckEndDate/{sprizeId}");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<bool>();
    }


    public async Task<List<GetCustomerPrize>?> GetCustomerPrizeByIdAsync(string winnerId)
    {
        var response = await http.GetAsync($"api/WinnerSessions/GetCustomerPrizeById/{winnerId}");

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new Exception($"API lỗi ({(int)response.StatusCode}): {error}");
        }

        return await response.Content.ReadFromJsonAsync<List<GetCustomerPrize>>();
    }

    public async Task<string?> GetQRCode(string code)
    {
        var response = await http.GetAsync($"api/QrCode/generate/{Uri.EscapeDataString(code)}");

        if (!response.IsSuccessStatusCode)
            return null;

        // Đọc binary bytes của file ảnh PNG
        var bytes = await response.Content.ReadAsByteArrayAsync();

        // Convert sang Base64 Data URL
        var base64 = Convert.ToBase64String(bytes);
        return $"data:image/png;base64,{base64}";
    }


}