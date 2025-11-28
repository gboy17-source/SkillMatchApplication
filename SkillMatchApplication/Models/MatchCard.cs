using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillMatchApplication.Models
{
    public class MatchCard
    {
        public string CurrentUserId { get; set; } = "";   //e.g. 8bsdf7iu2q34-435234-efdhb-435
        public string MatchedUserId { get; set; } = "";   //e.g. 123563456-4567-48545684568-435
        public string Name { get; set; } = "";            //e.g. "Louigie Tipsay"

        // Legacy single string used for display
        public string Skill { get; set; } = "";           // e.g. "Python"

        // New: explicit list of offered skill names (preserves original API data)
        public List<string> SkillsOffered { get; set; } = new List<string>();

        // New: corresponding skill ids (from overlapSkillIds or other id arrays in API)
        public List<int> SkillIds { get; set; } = new List<int>();

        public double Rating { get; set; } = 0;           //e.g. 4.9
    }
}
