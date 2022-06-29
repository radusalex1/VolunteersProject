using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VolunteersProject.Models;

namespace VolunteersProject.Repository
{
    public interface IEnrollmentRepository
    {
        Enrollment GetEnrollmentById(int id);
        IQueryable<Enrollment> GetEnrollments();
        IQueryable<Enrollment> GetEnrollments_With_Data();
        void Save(Enrollment enrollment);
        void UpdateEnrollment(Enrollment enrollment);
        bool EnrollmentExists(Enrollment enrollment);
        void DeleteEnrollment(Enrollment enrollment);
    }
}