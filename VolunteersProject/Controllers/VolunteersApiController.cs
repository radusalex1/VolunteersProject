using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using VolunteersProject.Models;
using VolunteersProject.Repository;

namespace VolunteersProject.Controllers
{
    [Route("api/[controller]")]   
    [ApiController]
    [ApiExplorerSettings(GroupName = "Volunteer")]
    public class VolunteersApiController : ControllerBase
    {
        private IVolunteerRepository volunteerRepository;

        public VolunteersApiController(IVolunteerRepository repository)
        {
            this.volunteerRepository = repository;
        }

        // GET: api/VolunteersApi
        [HttpGet]
        //[ApiExplorerSettings(GroupName = "Volunteer > GetVolunteers")]
        public ActionResult<IEnumerable<Volunteer>> GetVolunteers()
        {
            //return await _context.Volunteers.ToListAsync();

            var volunteers = this.volunteerRepository.GetVolunteers();

            return Ok(volunteers);
        }

        //todo cia - implement this: GetVolunteerWithEnrollmentsById

        //GET: api/VolunteersApi/5
        [HttpGet("{id}")]
        //[ApiExplorerSettings(GroupName = "Volunteer > GetVolunteer")]
        public ActionResult<Volunteer> GetVolunteer(int id)
        {
            //var volunteer = await _context.Volunteers.FindAsync(id);

            var volunteer = this.volunteerRepository.GetVolunteerById(id);

            if (volunteer == null)
            {
                return NotFound();
            }

            return volunteer;
        }

        //PUT: api/VolunteersApi/5
        //To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        //[ApiExplorerSettings(GroupName = "Volunteer > UpdateVolunteer")]
        //public async Task<IActionResult> PutVolunteer(int id, Volunteer volunteer)
        public ActionResult PutVolunteer(Volunteer volunteer)
        {           
            //context.Entry(volunteer).State = EntityState.Modified;

            try
            {
                //await _context.SaveChangesAsync();
                volunteerRepository.UpdateVolunteer(volunteer);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!volunteerRepository.VolunteerExists(volunteer.ID))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            //return NoContent();
            return Ok();
        }

        // POST: api/VolunteersApi
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        //[ApiExplorerSettings(GroupName = "Volunteer > CreateVolunteer")]
        [HttpPost]
        public ActionResult PostVolunteer(Volunteer volunteer)
        {
            //_context.Volunteers.Add(volunteer);
            //await _context.SaveChangesAsync();
            volunteerRepository.AddVolunteer(volunteer);

            //return CreatedAtAction("GetVolunteer", new { id = volunteer.ID }, volunteer);
            return Ok();
        }

        // DELETE: api/VolunteersApi/5
        [HttpDelete("{id}")]
        //[ApiExplorerSettings(GroupName = "Volunteer > DeleteVolunteer")]
        public ActionResult DeleteVolunteer(int id)
        {
            //var volunteer = await _context.Volunteers.FindAsync(id);
            var volunteer = volunteerRepository.GetVolunteerById(id);

            if (volunteer == null)
            {
                return NotFound();
            }

            //_context.Volunteers.Remove(volunteer);
            //await _context.SaveChangesAsync();
            volunteerRepository.DeleteVolunteer(volunteer);

            //return NoContent();
            return Ok();
        }        
    }
}
