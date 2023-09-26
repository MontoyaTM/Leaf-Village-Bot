using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Leaf_Village_Bot.DBUtil.ReportTicket
{
    public class DBReportTicket
    {
        public int TicketNo { get; set; }
        public string UserName { get; set; }
        public string Plantiff { get; set; }
        public string Defendant { get; set; }
        public string Date { get; set; }
        public string Details { get; set; }
        public string Screenshots { get; set; }
    }
}
