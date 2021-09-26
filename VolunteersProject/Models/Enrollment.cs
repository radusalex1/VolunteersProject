namespace VolunteersProject.Models
{
    public class Enrollment
    { 
        public int EnrollmentID { get; set; }
        public int EventID { get; set; }
        public int VolunteerID { get; set; }
        public Volunteer volunteer { get; set; }
        public Contribution contribution { get;set; }
    }
}