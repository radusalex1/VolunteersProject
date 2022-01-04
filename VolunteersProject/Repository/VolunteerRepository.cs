﻿using Microsoft.EntityFrameworkCore;
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
                return null;
            }

            return _context.Volunteers
                .Include(e => e.Enrollments)
                .ThenInclude(c => c.contribution)
                     .FirstOrDefault(m => m.Id == id);
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
            //todo Radu - check if this volunteer already exist (not by id)

            _context.Add(volunteer);
            _context.SaveChanges();
        }

        /// <summary>
        /// Update volunteer.
        /// </summary>
        /// <param name="volunteer">Volunteer.</param>
        public void UpdateVolunteer(Volunteer volunteer)
        {
            //todo Radu - check if this volunteer already exist (not by id)

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
    }
}
