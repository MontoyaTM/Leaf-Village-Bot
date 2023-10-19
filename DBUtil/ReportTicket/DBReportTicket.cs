using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Leaf_Village_Bot.DBUtil.ReportTicket
{
    public class DBReportTicket
    {
        public ulong TicketID { get; set; }
        public string UserName { get; set; }
        public ulong MemberID { get; set; }
        public ulong ServerID { get; set; }
        public string Plantiff { get; set; }
        public string Defendant { get; set; }
        public string Date { get; set; }
        public string Details { get; set; }
    }
}
