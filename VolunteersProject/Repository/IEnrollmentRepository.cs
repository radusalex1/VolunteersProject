using System.Collections.Generic;
using VolunteersProject.Models;

namespace VolunteersProject.Repository
{
    public interface IEnrollmentRepository
    {
        Enrollment GetEnrollmentById(int id);
        List<Enrollment> GetVolunteers();
    }
}