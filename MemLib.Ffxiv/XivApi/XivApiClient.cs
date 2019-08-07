using System;
using System.Net.Http;
using System.Threading.Tasks;
using MemLib.Ffxiv.XivApi.Models.Character;
using MemLib.Ffxiv.XivApi.Models.Search;

namespace MemLib.Ffxiv.XivApi {
    public static class XivApiClient {
        internal static HttpClient Client = new HttpClient();

        static XivApiClient() {
            Client.BaseAddress = new Uri("https://xivapi.com");
        }

        public static async Task<T> Request<T>(string requestUri) {
            var result = await Client.GetStringAsync(requestUri);
            return result.FromJson<T>();
        }

        public static async Task<CharacterRequest> GetCharacter(int lodestoneId) {
            return await Request<CharacterRequest>($"character/{lodestoneId}?data=AC");
        }

        public static async Task<SearchResult<CharacterSearchResult>> SearchCharacter(string name, string server = "") {
            var searchStr = $"character/search?name={name}";
            if (!string.IsNullOrEmpty(server))
                searchStr += $"&server={server}";
            var result = await Request<SearchResult<CharacterSearchResult>>($"{searchStr}");
            result.RequestUri = searchStr;
            return result;
        }
    }
}