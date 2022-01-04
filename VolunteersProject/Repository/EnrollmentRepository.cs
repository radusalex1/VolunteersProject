using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using VolunteersProject.Common;
using VolunteersProject.Data;
using VolunteersProject.Models;

namespace VolunteersProject.Repository
{
    public class EnrollmentRepository : IEnrollmentRepository
    {
        private readonly VolunteersContext _context;

        /// <summary>
        /// Contructor
        /// </summary>
        /// <param name="context"></param>
        public EnrollmentRepository(VolunteersContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get volunteer by id.
        /// </summary>
        /// <param name="id">Volunteer id.</param>
        /// <returns></returns>
        public Enrollment GetEnrollmentById(int id)
        {
            return _context.Enrollments.FirstOrDefault(i => i.EnrollmentID.Equals(id));
        }

        /// <summary>
        /// Get all volunteers.
        /// </summary>
        /// <returns>List of all volunteers.</returns>
        public IQueryable<Enrollment> GetEnrollments()
        {
            return _context.Enrollments;
        }
        public IQueryable<Enrollment> GetEnrollments_With_Data()
        {
            IQueryable<Enrollment> enrollments = _context.Enrollments
                .Include(e => e.volunteer)
                .Include(c => c.contribution);

            return enrollments;
        }
        public void Save(Enrollment enrollment)
        {
            _context.Add(enrollment);
            _context.SaveChanges();
        }

        //todo Radu - rename this method to something that can be understand
        public void Update(Enrollment enrollment)
        {
            //e1 => e1.contributionId == enrollment.contributionId
            var result = _context.Enrollments.FirstOrDefault(e => e.VolunteerID == enrollment.VolunteerID &&  e.contributionId == enrollment.contributionId);
            
            result.VolunteerStatus = (int)VolunteerEnrollmentStatusEnum.Declined;
            _context.SaveChanges();
        }

        //todo Radu - rename this method to something that can be understand
        public bool IfExist(Enrollment enrollment)
        {
            var result = _context.Enrollments.FirstOrDefault(e => e.VolunteerID == enrollment.VolunteerID &&
                                                                e.contributionId==enrollment.contributionId);
            if(result==null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public void DeleteEnrollment(Enrollment enrollment)
        {
            _context.Enrollments.Remove(enrollment);
            _context.SaveChanges();
        }
    }
}
