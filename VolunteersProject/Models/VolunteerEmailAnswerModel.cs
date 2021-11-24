using System;

namespace VolunteersProject.Models
{
    public class VolunteerEmailAnswerModel
    {
        public int VolunteerId;

        public int ContributionId;

        public string ContributionName;

        public string StartDate { get; set; }

        public string FinishDate { get; set; }
    }
}
