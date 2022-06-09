using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using VolunteerOpenXmlReports;
using VolunteersProject.Models;
using VolunteersProject.Repository;
using Microsoft.AspNetCore.Authorization;

namespace VolunteersProject.Controllers
{
    [Route("api/[controller]")]
    //[RoutePrefix("api/[controller]")]
    [AllowAnonymous]
    [ApiController]
    [ApiExplorerSettings(GroupName = "Volunteer")]
    public class VolunteersApiController : ControllerBase
    {
        private IVolunteerRepository volunteerRepository;
        private IReportManager reportManager;


        public VolunteersApiController(IVolunteerRepository repository, IReportManager reportManager)
        {
            this.volunteerRepository = repository;
            this.reportManager = reportManager;
        }

        // GET: api/VolunteersApi
        [HttpGet("getVolunteers")]
        public ActionResult<IEnumerable<Volunteer>> GetVolunteers()
        {
            var volunteers = this.volunteerRepository.GetVolunteers();

            return Ok(volunteers);
        }


        //GET: api/VolunteersApi/5
        [HttpGet("getVolunteer/{id}")]
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
        public ActionResult PutVolunteer(Volunteer volunteer)
        {
            //context.Entry(volunteer).State = EntityState.Modified;

            try
            {
                volunteerRepository.UpdateVolunteer(volunteer);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!volunteerRepository.VolunteerExists(volunteer.Id))
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
        //[HttpPost("InsertEmployeeDetails")]
        [HttpPost]
        [Route("insertVolunteer")]
        public ActionResult PostVolunteer(Volunteer volunteer)
        {
            volunteerRepository.AddVolunteer(volunteer);

            return Ok();
        }

        // DELETE: api/VolunteersApi/5
        [HttpDelete("{id}")]
        public ActionResult DeleteVolunteer(int id)
        {
            var volunteer = volunteerRepository.GetVolunteerById(id);

            if (volunteer == null)
            {
                return NotFound();
            }

            volunteerRepository.DeleteVolunteer(volunteer);

            return Ok();
        }

        // GET: api/VolunteersApi
        [HttpGet("getVolunteersReport")]
        public ActionResult<IEnumerable<Volunteer>> GetVolunteersReport()
        {
            var volunteers = this.volunteerRepository.GetVolunteers();

            var result = reportManager.ProcessData<Volunteer>(volunteers.ToList(), "VolunteerReport", "VolunteerReport");

            return Ok(volunteers);
        }
    }
}
