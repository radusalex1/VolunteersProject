namespace VolunteersProject.Models
{
    public class Enrollment
    { 
        public int EnrollmentID { get; set; }      
        public int contributionId { get; set; }
        public int VolunteerID { get; set; }
        public Volunteer volunteer { get; set; }
        public Contribution contribution { get;set; }
        
        public int VolunteerStatus { get; set; } 
    }
}