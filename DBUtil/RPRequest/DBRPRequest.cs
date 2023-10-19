using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Leaf_Village_Bot.DBUtil.RPRequest
{
    public class DBRPRequest
    {
        public ulong RequestID { get; set; }
        public string UserName { get; set; }
        public ulong MemberID { get; set; }
        public ulong ServerID { get; set; }
        public string IngameName { get; set; }
        public string RPMission { get; set; }
        public string Attendees { get; set; }
        public string Timezone { get; set; }
    }
}
