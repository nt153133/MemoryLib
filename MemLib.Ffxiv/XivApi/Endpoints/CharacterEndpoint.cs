using System.Net;

namespace MemLib.Ffxiv.XivApi.Endpoints {
    public class CharacterEndpoint {
        private readonly WebClient m_Client;
        private const string Endpoint = "/character";

        internal CharacterEndpoint(WebClient client) {
            m_Client = client;
        }

        public void GetCharacter(int lodestoneId) {
            if(lodestoneId == 0) return;
            var json = m_Client.DownloadString($"{Endpoint}/{lodestoneId}");
        }

        public void GetAchievements(int lodestoneId) {
            if (lodestoneId == 0) return;
            var json = m_Client.DownloadString($"{Endpoint}/{lodestoneId}?data=AC&columns=Achievements.List");
        }
    }
}