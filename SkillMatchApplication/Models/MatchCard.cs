using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillMatchApplication.Models
{
    public class MatchCard
    {
        public string Name { get; set; } = "";            //e.g. "Louigie Tipsay"
        public string Skill { get; set; } = "";           //e.g. "Python"
        public double Rating { get; set; } = 0;           //e.g. 4.9
    }
}
