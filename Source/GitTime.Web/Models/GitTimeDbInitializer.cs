using System.Data.Entity;

using GitTime.Web.Models.Database;

namespace GitTime.Web.Models
{
    public class GitTimeDbInitializer : CreateDatabaseIfNotExists<GitTimeContext>
    {
        protected override void Seed(GitTimeContext db)
        {
            Person adminUser;

            db.Persons.Add(adminUser = new Person { Email = "admin", Password = "admin", FirstName = "Admin", LastName = "User" });

            //Company company1, company2, company3;

            //db.Companies.Add(company1 = new Company { Name = "Company #1" });
            //db.Companies.Add(company2 = new Company { Name = "Company #2" });
            //db.Companies.Add(company3 = new Company { Name = "Company #3" });

            //db.Projects.Add(new Project { Company = company1, Name = "Project #1", Repository = "Company_1/Project_1" });
            //db.Projects.Add(new Project { Company = company2, Name = "Project #2", Repository = "Company_2/Project_2" });
            //db.Projects.Add(new Project { Company = company3, Name = "Project #3", Repository = "Company_3/Project_3" });

            base.Seed(db);
        }
    }
}