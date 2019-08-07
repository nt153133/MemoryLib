using System;
using System.Net.Http;
using System.Threading.Tasks;
using MemLib.Ffxiv.XivApi.Models.Character;
using MemLib.Ffxiv.XivApi.Models.Item;
using MemLib.Ffxiv.XivApi.Models.Search;

namespace MemLib.Ffxiv.XivApi {
    public static class XivApiClient {
        internal static HttpClient Client = new HttpClient();

        static XivApiClient() {
            Client.BaseAddress = new Uri("https://xivapi.com");
        }

        public static T Request<T>(string requestUri) => RequestAsync<T>(requestUri).Result;

        public static async Task<T> RequestAsync<T>(string requestUri) {
            var result = await Client.GetStringAsync(requestUri);
            return result.FromJson<T>();
        }

        public static CharacterRequest GetCharacter(int lodestoneId) => GetCharacterAsync(lodestoneId).Result;

        public static async Task<CharacterRequest> GetCharacterAsync(int lodestoneId) {
            return await RequestAsync<CharacterRequest>($"character/{lodestoneId}?data=AC");
        }

        public static SearchResult<CharacterSearchResult> SearchCharacter(string name, string server = "") => SearchCharacterAsync(name, server).Result;

        public static async Task<SearchResult<CharacterSearchResult>> SearchCharacterAsync(string name, string server = "") {
            if (string.IsNullOrEmpty(name))
                return null;
            var searchStr = $"character/search?name={name}";
            if (!string.IsNullOrEmpty(server))
                searchStr += $"&server={server}";
            var result = await RequestAsync<SearchResult<CharacterSearchResult>>(searchStr);
            result.RequestUri = searchStr;
            return result;
        }

        public static SearchResult<ItemSearchResult> SearchItems(params uint[] itemIds) => SearchItemsAsync(itemIds).Result;

        public static async Task<SearchResult<ItemSearchResult>> SearchItemsAsync(params uint[] itemIds) {
            var searchStr = "item/";
            if (itemIds.Length > 0)
                searchStr += $"?ids={string.Join(",", itemIds)}";
            var result = await RequestAsync<SearchResult<ItemSearchResult>>(searchStr);
            result.RequestUri = searchStr;
            return result;
        }

        public static ItemResult GetItem(uint itemId) => GetItemAsync(itemId).Result;

        public static async Task<ItemResult> GetItemAsync(uint itemId) {
            if (itemId <= 0) return null;
            return await RequestAsync<ItemResult>($"item/{itemId}");
        }
    }
}