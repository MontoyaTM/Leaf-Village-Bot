using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Leaf_Village_Bot.DBUtil.Profile
{
    public class DBProfile
    {
        public string UserName { get; set; }
        public ulong MemberID { get; set; }
        public ulong ServerID { get; set; } 
        public string AvatarURL { get; set; }
        public string ProfileImage { get; set; }
        public string InGameName { get; set; }
        public int Level { get; set; }
        public string Clan { get; set; }
        public string Organization { get; set; } = "—";
        public string OrgRank { get; set; } = "—";
        public int Raids { get; set; } = 0;
        public int Fame { get; set; } = 0;
        public string Masteries { get; set; }
        public string Alts { get; set; }
        public int ProctoredMissions { get; set; } = 0;
    }
}
