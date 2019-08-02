using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;

namespace MemLib.Ffxiv.XivApi.Endpoints {
    public class ItemsEndpoint {
        private readonly WebClient m_Client;
        private const string Endpoint = "/item";

        internal ItemsEndpoint(WebClient client) {
            m_Client = client;
        }

        public Dictionary<int, string> GetItemsById(params uint[] itemIds) {
            if (itemIds.Length == 0) return null;
            var json = m_Client.DownloadString($"{Endpoint}?columns=Name,ID&limit={itemIds.Length}&ids={string.Join(",", itemIds)}");
            var result = json.FromJson<Dictionary<string, List<Dictionary<string, object>>>>();
            var retDict = new Dictionary<int, string>(itemIds.Length);
            foreach (var item in result["Results"]) {
                retDict.Add(int.Parse(item["ID"].ToString()), item["Name"].ToString());
            }
            return retDict;
        }
    }
}