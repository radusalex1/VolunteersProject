using System.Collections.Generic;
using System.Linq;
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
        public List<Enrollment> GetEnrollments()
        {
            return _context.Enrollments.ToList();
        }

        public void Save(Enrollment enrollment)
        {
            _context.Add(enrollment);
            _context.SaveChanges();
        }
    }
}
