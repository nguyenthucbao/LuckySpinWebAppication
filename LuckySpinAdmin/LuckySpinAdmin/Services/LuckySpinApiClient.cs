using System.Net.Http.Json;
using LuckySpinAdmin.Dto;

namespace LuckySpinAdmin.Service;

public class LuckySpinApiClient(HttpClient http)
{

    public async Task<List<StoreDto>> GetStoresAsync()
    {
        return await http.GetFromJsonAsync<List<StoreDto>>("api/stores") ?? new();
    }

    public async Task<bool> AddStoreAsync(CreateStoreDto store)
    {
        var response = await http.PostAsJsonAsync("api/stores/addstore", store);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> UpdateStoreLocationAsync(string storeId, string newLocation)
    {
        var body = new UpdateStoreLocationDto { StoreLocate = newLocation };
        var response = await http.PutAsJsonAsync($"api/stores/{storeId}", body);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteStoreAsync(string storeId)
    {
        var response = await http.DeleteAsync($"api/stores/{storeId}");
        return response.IsSuccessStatusCode;
    }

    // ================= CAMPAIGNS APIS =================
    // Endpoint bổ sung theo yêu cầu để lấy toàn bộ danh sách Campaign hệ thống có
    public async Task<List<CampaignDto>> GetAllCampaignsAsync()
    {
        return await http.GetFromJsonAsync<List<CampaignDto>>("api/campaigns") ?? new();
    }

    public async Task<List<CampaignDto>> GetCampaignsByStoreAsync(string storeId)
    {
        return await http.GetFromJsonAsync<List<CampaignDto>>($"api/stores/{storeId}/campaigns") ?? new();
    }

    public async Task<bool> AssignCampaignsToStoreAsync(string storeId, List<string> campaignIds)
    {
        var body = new AssignCampaignsDto { CampaignIds = campaignIds };
        var response = await http.PostAsJsonAsync($"api/stores/{storeId}/campaigns", body);
        return response.IsSuccessStatusCode;
    }

    // ================= PRIZES APIS =================
    public async Task<List<PrizeDto>> GetPrizesByCampaignAsync(string campaignId)
    {
        var prizes = await http.GetFromJsonAsync<List<PrizeDto>>($"api/campaigns/{campaignId}/prizes") ?? new();
        // Gán giá trị ban đầu cho EditedWeight để thuận tiện cho việc chỉnh sửa trên FE
        foreach (var prize in prizes)
        {
            prize.EditedWeight = prize.ProbabilityWeight;
        }
        return prizes;
    }

    public async Task<bool> UpdatePrizeWeightAsync(string prizeId, double weight)
    {
        var body = new UpdatePrizeWeightDto { Weight = weight };
        var response = await http.PutAsJsonAsync($"api/prizes/{prizeId}/weight", body);
        return response.IsSuccessStatusCode;
    }
}
