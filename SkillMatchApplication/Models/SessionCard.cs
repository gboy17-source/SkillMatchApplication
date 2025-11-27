using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillMatchApplication.Models
{
    public class SessionCard
    {
        public string Day { get; set; } = "";
        public string Month { get; set; } = "";
        public string Skill { get; set; } = "";
        public string Partner { get; set; } = "";
        public string Time { get; set; } = "";
        public string Status { get; set; } = "";
        public string StatusColor { get; set; } = "";
        public bool IsIncomingRequest { get; set; } = false;
    }
}
