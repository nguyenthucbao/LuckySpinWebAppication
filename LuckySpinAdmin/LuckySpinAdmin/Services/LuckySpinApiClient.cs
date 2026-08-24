using LuckySpinAdmin.Dto;
using Microsoft.AspNetCore.Components.Forms;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using static System.Net.WebRequestMethods;


namespace LuckySpinAdmin.Service;

public class LuckySpinApiClient(HttpClient http)
{

    /// /////////////////////////////////////////////// STORE API ////////////////////////////////

    public async Task<List<GetStores>?> GetStoresAsync()
    {
        var response = await http.GetAsync("api/stores/admin");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<List<GetStores>>();
    }

    public async Task<List<GetCampaignInfo>?> GetAllCampaignsAsync()
    {
        var response = await http.GetAsync("api/campaigns/admin");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<List<GetCampaignInfo>>();
    }

    public async Task<bool> AddCampaignToStoreAsync(string storeId, string campaignId)
    {
        var response = await http.PostAsync($"api/stores/admin/addcampaigntostore/{storeId}/{campaignId}", null);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteCampaignFromStoreAsync(string storeId, string campaignId)
    {
        var response = await http.DeleteAsync($"api/stores/admin/removecampaignfromstore/{storeId}/{campaignId}");
        return response.IsSuccessStatusCode;
    }


    public async Task<GetStoresInfo?> GetStoreInfoAsync(string storeId)
    {
        var response = await http.GetAsync($"api/stores/admin/getstorebyid/{storeId}");
        if (response.StatusCode == System.Net.HttpStatusCode.NotFound) return null;
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<GetStoresInfo>();
    }

    public async Task<bool> AddStoreAsync(CreateStoreDto store)
    {
        var response = await http.PostAsJsonAsync("api/stores/admin/addstore", store);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteStoreAsync(string storeId)
    {
        var response = await http.DeleteAsync($"api/stores/admin/deletestore/{storeId}");
        return response.IsSuccessStatusCode;
    }

    public async Task<List<GroupedPrizeAdmin>?> GetStoreCampaignPrizesAsync(string storeId, string campaignId)
    {
        var response = await http.GetAsync($"api/stores/admin/storecampaignprize/{storeId}/{campaignId}");
        if (!response.IsSuccessStatusCode) return null;
        return await response.Content.ReadFromJsonAsync<List<GroupedPrizeAdmin>>();
    }

    public async Task<bool> ChangeProbabilityWeightAsync(ProbabilityChangeDto dto)
    {
        var response = await http.PostAsJsonAsync("api/stores/admin/changeprobability", dto);
        return response.IsSuccessStatusCode;
    }


   


    /// /////////////////////////////////////////////// CAMPAIGN API ////////////////////////////////
    /// 

    // Thêm vào LuckySpinApiClient
    public async Task<List<GetCampaignInfo>?> GetCampaignsAsync()
    {
        var response = await http.GetAsync("api/campaigns/admin");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<List<GetCampaignInfo>>();
    }

    public async Task<CampaignWithPrize?> GetCampaignByIdAsync(string campaignid)
    {
        var response = await http.GetAsync($"api/campaigns/admin/getcambyid/{campaignid}");
        if (response.StatusCode == System.Net.HttpStatusCode.NotFound) return null;
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<CampaignWithPrize>();
    }

    public async Task<bool> AddCampaignAsync(GetCampaignInfo campaign)
    {
        var res = await http.PostAsJsonAsync("api/campaigns/admin/addcampaign", campaign);
        return res.IsSuccessStatusCode;
    }
   


    public async Task<bool> DeleteCampaignAsync(string campaignId)
    {
        var res = await http.DeleteAsync($"api/campaigns/admin/deletecampaign/{campaignId}");
        return res.IsSuccessStatusCode;
    }

    public async Task<bool> ChangeCampaignInfoAsync(GetCampaignInfo dto)
    {
        var res = await http.PostAsJsonAsync("api/campaigns/admin/changecampaigninfo", dto);
        return res.IsSuccessStatusCode;
    }


   

}
