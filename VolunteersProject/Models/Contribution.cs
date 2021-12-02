using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace VolunteersProject.Models
{
    /// <summary>
    /// Contribution model.
    /// </summary>
    public class Contribution
    {
        public int ID { get; set; }
        
        public string Name { get; set; }
       [Display(Name="Happy Points")]
        public int Credits { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime StartDate { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime FinishDate { get; set; }

        public string Description { get; set; }
        public ICollection<Enrollment> Enrollments { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime VolunteerDeadlineConfirmation { get; set; }
    }
}
