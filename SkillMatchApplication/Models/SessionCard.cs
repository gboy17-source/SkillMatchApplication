using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillMatchApplication.Models
{
    public class SessionCard
    {
        public string SessionId { get; set; }
        public string Day { get; set; }
        public string Month { get; set; }
        public string Skill { get; set; }
        public string PartnerName { get; set; }
        public string Time { get; set; }
        public bool IsTeaching { get; set; }   // true = TEACH, false = LEARN
    }
}
