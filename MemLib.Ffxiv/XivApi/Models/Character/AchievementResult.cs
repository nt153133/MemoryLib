using System;
using System.Collections;
using System.Collections.Generic;

namespace MemLib.Ffxiv.XivApi.Models.Character {
    public class AchievementResult : IEnumerable<KeyValuePair<int, DateTime>> {
        public List<Dictionary<string, int>> List = new List<Dictionary<string, int>>();
        public int Points;

        private IEnumerable<KeyValuePair<int, DateTime>> Values {
            get {
                foreach (var entry in List) {
                    if (entry.TryGetValue("ID", out var id) && entry.TryGetValue("Date", out var date))
                        yield return new KeyValuePair<int, DateTime>(id, DateTimeOffset.FromUnixTimeSeconds(date).DateTime);
                }
            }
        }
        
        #region Implementation of IEnumerable

        public IEnumerator<KeyValuePair<int, DateTime>> GetEnumerator() {
            return Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        #endregion
    }
}