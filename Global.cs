using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Leaf_Village_Bot
{
    public static class Global
    {
        public static string LeafSymbol_URL = "https://i.imgur.com/spjhOGb.png";
        public static string CharacterProfile_URL = "https://i.imgur.com/bBsKHPn.png";
        public static string LMPFSymbol_URL = "https://i.imgur.com/QbBzNiR.png";
        public static string VillagerApplication_URL = "https://i.imgur.com/4S7reOU.png";
        public static string RaidDashboard_URL = "https://i.imgur.com/kKP3M5F.png";
        public static string LMPFDashboard_URL = "https://i.imgur.com/T9m0bwD.png";

        public static string HokageNPC_URL = "https://i.imgur.com/8XpWbYE.png";
        public static string RankedNPC_URL = "https://i.imgur.com/XbTXm3E.png";
        public static string LeafGate_URL = "https://i.imgur.com/oVtf0GT.png";
        public static string LeafClans_URL = "https://i.imgur.com/4Udc9ml.png";
        
        

        public static ulong CreateID()
        {
            var random = new Random();

            ulong minValue = 10000000000000000;
            ulong maxValue = 99999999999999999;

            ulong randomNumber = ((ulong)random.Next((int)(minValue >> 32), int.MaxValue) << 32) | ((ulong)random.Next());
            ulong result = randomNumber % (maxValue - minValue + 1) + minValue;

            return result;

        }
    }
}
