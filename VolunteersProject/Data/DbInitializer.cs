using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VolunteersProject.Models;

namespace VolunteersProject.Data
{
    public class DbInitializer
    {
        public static void Initialize(VolunteersContext context)
        {
            context.Database.EnsureCreated();
            if(context.Volunteers.Any())
            {
                return;
            }
            var volunteers = new Volunteer[]
            {
                new Volunteer{Name="Carson",Surname="Alexander",City="Brasov",JoinHubDate=DateTime.Parse("2005-09-01"),BirthDate=DateTime.Parse("2001-01-01")},
                new Volunteer{Name="Meredith",Surname="Alonso",City="Brasov",JoinHubDate=DateTime.Parse("2002-09-01"),BirthDate=DateTime.Parse("2001-02-02")},
                new Volunteer{Name="Arturo",Surname="Anand",City="Brasov",JoinHubDate=DateTime.Parse("2003-09-01"),BirthDate=DateTime.Parse("2003-03-03")},
                new Volunteer{Name="Gytis",Surname="Barzdukas",City="Brasov",JoinHubDate=DateTime.Parse("2003-09-01"),BirthDate=DateTime.Parse("2002-05-28")},
                new Volunteer{Name="Yan",Surname="Li",City="Brasov",JoinHubDate=DateTime.Parse("2003-09-01"),BirthDate=DateTime.Parse("2001-04-04")},
                new Volunteer{Name="Peggy",Surname="Justice",City="Brasov",JoinHubDate=DateTime.Parse("2002-09-01"),BirthDate=DateTime.Parse("2002-05-05")},
                new Volunteer{Name="Laura",Surname="Norman",City="Brasov",JoinHubDate=DateTime.Parse("2002-09-01"),BirthDate=DateTime.Parse("2001-06-06")},
                new Volunteer{Name="Nino",Surname="Olivetto",City="Brasov",JoinHubDate=DateTime.Parse("2002-09-01"),BirthDate=DateTime.Parse("2002-12-12")}
            };
            foreach(Volunteer v in volunteers)
            {
                context.Volunteers.Add(v);
            }
            context.SaveChanges();

            var contributions = new Contribution[]
            {
                new Contribution{Name="Voluntar Tabara",Credits=5},
                new Contribution{Name="Participare Sedinta",Credits=2},
                new Contribution{Name="Participare Proiect",Credits=6},
                new Contribution{Name="Participare Concurs",Credits=4},
                new Contribution{Name="Contributie Hub",Credits=7},
                new Contribution{Name="Sef Departament",Credits=6},
                new Contribution{Name="Presedinte",Credits=7},
                new Contribution{Name="Vice Presedinte",Credits=8},
            };
            foreach (Contribution c in contributions)
            {
              
                context.Contributions.Add(c);
            }
            context.SaveChanges();

            var enrollments = new Enrollment[]
                {
                    new Enrollment{EventID=1,VolunteerID=1},
                    new Enrollment{VolunteerID=2,EventID=1},
                    new Enrollment{VolunteerID=3,EventID=1},
                    new Enrollment{VolunteerID=1,EventID=2},
                    new Enrollment{VolunteerID=5,EventID=3},
                    new Enrollment{VolunteerID=6,EventID=3},
                    new Enrollment{VolunteerID=7,EventID=4},
                    new Enrollment{VolunteerID=8,EventID=4},
                    new Enrollment{VolunteerID=4,EventID=7},
                    new Enrollment{VolunteerID=5,EventID=1},
                    new Enrollment{VolunteerID=6,EventID=1},
                };
            foreach(Enrollment e in enrollments)
            {
                context.Enrollments.Add(e);
            }
            context.SaveChanges();

        }
    }
}
