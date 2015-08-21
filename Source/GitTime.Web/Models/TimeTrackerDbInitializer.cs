using System.Data.Entity;

using GitTime.Web.Models.Database;

namespace GitTime.Web.Models
{
    public class TimeTrackerDbInitializer: CreateDatabaseIfNotExists<TimeTrackerContext>
    {
        protected override void Seed(TimeTrackerContext db)
        {
            db.Persons.Add(new Person { Email = "a", Password = "a", FirstName = "Aleksey", LastName = "Terzi" });

            Company insite, keyera, millerDatabases;

            db.Companies.Add(insite = new Company { Name = "InSite" });
            db.Companies.Add(keyera = new Company { Name = "Keyera" });
            db.Companies.Add(millerDatabases = new Company { Name = "Miller Databases" });

            db.Projects.Add(new Project { Company = insite, Name = "Iris", Repository = "InSite/Iris" });
            db.Projects.Add(new Project { Company = keyera, Name = "Cmds", Repository = "InSite/Cmds" });
            db.Projects.Add(new Project { Company = millerDatabases, Name = "Lemar", Repository = "MillerDatabases/Lemar" });

            base.Seed(db);
        }
    }
}