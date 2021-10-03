using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using VolunteersProject.Models;
using VolunteersProject.Repository;

namespace VolunteersProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VolunteersApiController : ControllerBase
    {
        //private readonly VolunteersContext _context;
        private IVolunteerRepository repository;

        //public VolunteersApiController(VolunteersContext context)
        public VolunteersApiController(IVolunteerRepository repository)
        {
            this.repository = repository;
        }

        // GET: api/VolunteersApi
        [HttpGet]
        //public async Task<ActionResult<IEnumerable<Volunteer>>> GetVolunteers()
        public ActionResult<IEnumerable<Volunteer>> GetVolunteers()
        {
            //return await _context.Volunteers.ToListAsync();

            var volunteers = this.repository.GetVolunteers();

            return Ok(volunteers);
        }

        //GET: api/VolunteersApi/5
        [HttpGet("{id}")]
        //public async Task<ActionResult<Volunteer>> GetVolunteer(int id)
        public ActionResult<Volunteer> GetVolunteer(int id)
        {
            //var volunteer = await _context.Volunteers.FindAsync(id);

            var volunteer = this.repository.GetVolunteerById(id);

            if (volunteer == null)
            {
                return NotFound();
            }

            return volunteer;
        }

        // PUT: api/VolunteersApi/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        //[HttpPut("{id}")]
        //public async Task<IActionResult> PutVolunteer(int id, Volunteer volunteer)
        //{
        //    if (id != volunteer.ID)
        //    {
        //        return BadRequest();
        //    }

        //    _context.Entry(volunteer).State = EntityState.Modified;

        //    try
        //    {
        //        await _context.SaveChangesAsync();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!VolunteerExists(id))
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return NoContent();
        //}

        // POST: api/VolunteersApi
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        //[HttpPost]
        //public async Task<ActionResult<Volunteer>> PostVolunteer(Volunteer volunteer)
        //{
        //    _context.Volunteers.Add(volunteer);
        //    await _context.SaveChangesAsync();

        //    return CreatedAtAction("GetVolunteer", new { id = volunteer.ID }, volunteer);
        //}

        // DELETE: api/VolunteersApi/5
        //[HttpDelete("{id}")]
        //public async Task<IActionResult> DeleteVolunteer(int id)
        //{
        //    var volunteer = await _context.Volunteers.FindAsync(id);
        //    if (volunteer == null)
        //    {
        //        return NotFound();
        //    }

        //    _context.Volunteers.Remove(volunteer);
        //    await _context.SaveChangesAsync();

        //    return NoContent();
        //}

        //private bool VolunteerExists(int id)
        //{
        //    return _context.Volunteers.Any(e => e.ID == id);
        //}
    }
}
