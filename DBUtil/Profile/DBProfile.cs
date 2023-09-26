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
        public string InGameName { get; set; }
        public int Level { get; set; }
        public string Masteries { get; set; }
        public string Clan { get; set; }
        public string Organization { get; set; }
        public string OrgRank { get; set; }
        public string Alts { get; set; }
        public int Raids { get; set; }
        public int Fame { get; set; }
    }
}
