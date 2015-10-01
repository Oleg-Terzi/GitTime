namespace GitTime.Web.Migrations
{
    using System.Data.Entity.Migrations;
    using System.Linq;
    using GitTime.Web.Models.Database;

    internal sealed class Configuration : DbMigrationsConfiguration<GitTime.Web.Models.GitTimeContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            ContextKey = "GitTime.Web.Models.GitTimeContext";
        }

        protected override void Seed(GitTime.Web.Models.GitTimeContext context)
        {
            context.Roles.AddOrUpdate(
                r => r.Name,
                new Role { Name = GitTime.Web.Constants.RoleType.Administrator },
                new Role { Name = GitTime.Web.Constants.RoleType.Developer }
            );
            
            var adminUser = context.Persons.Where(p => p.Email == "admin").SingleOrDefault();
            if (adminUser != null && !adminUser.Roles.Any(r => r.Name == GitTime.Web.Constants.RoleType.Administrator))
            {
                var adminRole = context.Roles.Where(r => r.Name == GitTime.Web.Constants.RoleType.Administrator).SingleOrDefault();
                adminUser.Roles.Add(adminRole);

                context.SaveChanges();
            }
        }
    }
}
