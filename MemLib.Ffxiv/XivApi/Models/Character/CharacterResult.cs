using System;
using System.Collections.Generic;

namespace MemLib.Ffxiv.XivApi.Models.Character {
    public class CharacterResult {
        public ClassJobResult ActiveClassJob;
        public List<ClassJobResult> ClassJobs = new List<ClassJobResult>();
        public string Name;
        public string FreeCompanyId;
        public string DC;
        public string Server;
        public string Nameday;
        public string Portrait;
        public string Avatar;
        public int ParseDate;
        public int Race;
        public int Gender;
        public int ID;
        public int GuardianDeity;
        public int Title;
        public int Town;
        public int Tribe;
    }
}