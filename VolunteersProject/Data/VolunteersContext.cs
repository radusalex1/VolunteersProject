using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VolunteersProject.Models;

namespace VolunteersProject.Data
{
    public class VolunteersContext : DbContext
    {
        public VolunteersContext(DbContextOptions<VolunteersContext> options):base(options)
        {
        }

        public DbSet<Contribution> Contributions { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<Volunteer> Volunteers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Contribution>().ToTable("Contributions");
            modelBuilder.Entity<Enrollment>().ToTable("Enrollments");
            modelBuilder.Entity<Volunteer>().ToTable("Volunteers");

            modelBuilder.Entity<User>().ToTable("Users");
        }
    }
}
