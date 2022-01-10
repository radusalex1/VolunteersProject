using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using VolunteersProject.Data;
using VolunteersProject.Models;

namespace VolunteersProject.Repository
{
    /// <summary>
    /// Repository for volunteers.
    /// </summary>
    public class VolunteerRepository : IVolunteerRepository
    {
        private readonly VolunteersContext _context;

        /// <summary>
        /// Contructor
        /// </summary>
        /// <param name="context"></param>
        public VolunteerRepository(VolunteersContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get all not assigned volunteers to the current contribution id.
        /// </summary>
        /// <param name="contributionId">Contribution id.</param>
        /// <returns>List of not assigned volunteers.</returns>
        public List<Volunteer> GetAvailableVolunteers(int contributionId)
        {
            var volunteers = _context.Volunteers
             .Include(e => e.Enrollments)
             .ThenInclude(c => c.contribution);
            

            var volunteersAssigned = volunteers.Where(v => v.Enrollments.Any(c => c.contribution.Id == contributionId));

            var volunteersAvailable = volunteers.Where(v => v.Enrollments.Any(c => c.contribution.Id != contributionId) && !volunteersAssigned.Contains(v));

            var volunteersWithNoAnyAssignments = volunteers.Where(v => v.Enrollments.Count == 0);

            var test =  volunteersAvailable.Union(volunteersWithNoAnyAssignments).ToList();

            return test;
        }

        /// <summary>
        /// Get volunteer by id.
        /// </summary>
        /// <param name="id">Volunteer id.</param>
        /// <returns>Return volunteer.</returns>
        public Volunteer GetVolunteerById(int? id)
        {
            if (id == null)
            {
                return null;
            }

            return _context.Volunteers.FirstOrDefault(i => i.Id.Equals(id));
        }

        /// <summary>
        /// Get volunteer with related enrollments.
        /// </summary>
        /// <param name="id">Volunteer id.</param>
        /// <returns>Return colunteer with related enrollments.</returns>
        public Volunteer GetVolunteerWithEnrollmentsById(int? id)
        {
            if (id == null)
            {
                Volunteer v = new Volunteer();
                return v;
            }

            var result = _context.Enrollments
                .Include(v => v.volunteer)
                .Include(c => c.contribution)
                    .FirstOrDefault(r => r.VolunteerID == id && r.VolunteerStatus == 2);

            return result.volunteer;

        }

        /// <summary>
        /// Get all volunteers.
        /// </summary>
        /// <returns>List of all volunteers.</returns>
        public IQueryable<Volunteer> GetVolunteers()
        {
            return _context.Volunteers;
        }

        /// <summary>
        /// Add volunteer.
        /// </summary>
        /// <param name="volunteer"></param>
        public void AddVolunteer(Volunteer volunteer)
        {
            _context.Add(volunteer);
            _context.SaveChanges();
        }

        /// <summary>
        /// Update volunteer.
        /// </summary>
        /// <param name="volunteer">Volunteer.</param>
        public void UpdateVolunteer(Volunteer volunteer)
        {
            _context.Update(volunteer);
            _context.SaveChanges();
        }

        /// <summary>
        /// Check by id if volunteer exist.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>True if exist, otherwise false.</returns>
        public bool VolunteerExists(int id)
        {
            return _context.Volunteers.Any(e => e.Id == id);
        }

        /// <summary>
        /// Delete volunteer.
        /// </summary>
        /// <param name="volunteer">Volunteer.</param>
        public void DeleteVolunteer(Volunteer volunteer)
        {
            _context.Volunteers.Remove(volunteer);
            _context.SaveChanges();
        }

        /// <summary>
        /// Search by phone, email if volunteer exist.
        /// </summary>
        /// <param name="volunteer"></param>
        /// <returns>True if exist, otherwise false.</returns>
        public bool CheckVolunteerExistByPhoneOrEmail(Volunteer volunteer)
        {
            if (volunteer.Email != null && volunteer.Phone != null)
            {
                var result = _context.Volunteers.AsNoTracking().FirstOrDefault(
                    v => v.Phone == volunteer.Phone || 
                    v.Email == volunteer.Email);

                return (result == null || result.Id == volunteer.Id) ? false : true;
            }

            return false;
        }

        /// <summary>
        /// Return points of volunteer
        /// </summary>
        /// <param name="volunteer"></param>
        /// <returns></returns>
        public int GetVolunteerTotalPoints(Volunteer volunteer)
        {

            if(volunteer==null)
            {
                return 0;
            }
            
            var result = _context.Enrollments
                .Include(e => e.volunteer)
                .Include(c => c.contribution)
                .Where(e => e.volunteer.Id == volunteer.Id).ToList().ToArray();

            var totalPoints = 0;
            
            for(int i=0;i<result.Length;i++)
            {
                if(result.ElementAt(i).VolunteerStatus==2)
                {
                    totalPoints += result[i].contribution.Credits;
                }
               
            }

            return totalPoints;
            /*select sum(c.Credits) as totalPoints from Volunteers v
              inner join Enrollments e on e.VolunteerID=v.Id
              inner join Contributions c on c.Id=e.contributionId
              where Surname='Radu - Serban';*/
            
        }

        /// <summary>
        /// Return the volunteer with the useriD given as parameter
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public Volunteer GetVolunteerByUserId(int Id)
        {
            if(Id==null)
            {
                Volunteer v = new Volunteer();
                v.Id = 0;
                return v;
            }
            return _context.Volunteers
                .FirstOrDefault(v => v.User.Id == Id);

        }

        /// <summary>
        /// return list of contributions of volunteer given as parameter to be displayed on HomePage;
        /// </summary>
        /// <param name="volunteer"></param>
        /// <returns></returns>
        public List<Contribution> GetContributionsByVolunteer(Volunteer volunteer)
        {
            List<Contribution> result = new List<Contribution>();

            if(volunteer==null)
            {
                return result;
            }

            var tempEnrollemts = _context.Enrollments
                .Include(v => v.volunteer)
                .Include(c => c.contribution)
                .Where(v => v.volunteer.Id == volunteer.Id && v.VolunteerStatus==2).AsNoTracking().ToList();

            foreach(Enrollment e in tempEnrollemts)
            {
               result.Add(e.contribution);
            }

            return result;
           
        }
    }
}
