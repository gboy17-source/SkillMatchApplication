using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillMatchApplication.Models
{
    public class FakeDatabase
    {
        public static List<SessionCard> GetUpcomingSessions()
        {
            return new List<SessionCard>
        {
            new SessionCard { Day = "05", Month = "Dec", Skill = "React Basics", PartnerName = "Jane Smith", Time = "2:00 PM", IsTeaching = true },
            new SessionCard { Day = "08", Month = "Dec", Skill = "Python", PartnerName = "Mike Johnson", Time = "10:00 AM", IsTeaching = false },
            new SessionCard { Day = "15", Month = "Dec", Skill = "C# Advanced", PartnerName = "Alex Chen", Time = "4:00 PM", IsTeaching = true },
            new SessionCard { Day = "20", Month = "Dec", Skill = "UI/UX", PartnerName = "Sarah Kim", Time = "6:00 PM", IsTeaching = false }
        };
        }

        public static List<SessionCard> GetPastSessions()
        {
            return new List<SessionCard>
        {
            new SessionCard { Day = "28", Month = "Nov", Skill = "Java OOP", PartnerName = "Anna Lee", Time = "3:00 PM", IsTeaching = true }
        };
        }
    }
}
