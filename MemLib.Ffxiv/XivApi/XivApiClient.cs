using System;
using System.Net;
using MemLib.Ffxiv.XivApi.Endpoints;

namespace MemLib.Ffxiv.XivApi {
    public class XivApiClient : IDisposable {
        private readonly WebClient m_Client;
        public string BaseUrl { get; } = "https://xivapi.com";

        private ItemsEndpoint m_Item;
        public ItemsEndpoint Item => m_Item ?? (m_Item = new ItemsEndpoint(m_Client));
        private CharacterEndpoint m_Character;
        public CharacterEndpoint Character => m_Character ?? (m_Character = new CharacterEndpoint(m_Client));

        public XivApiClient() {
            m_Client = new WebClient {BaseAddress = BaseUrl};
        }

        public void Dispose() {
            m_Client.Dispose();
        }
    }
}