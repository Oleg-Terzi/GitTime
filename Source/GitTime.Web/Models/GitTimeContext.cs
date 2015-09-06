using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;

using GitTime.Web.Models.Database;

namespace GitTime.Web.Models
{
    public class GitTimeContext: DbContext
    {
        #region Construction

        public GitTimeContext()
            : base("GitTime")
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
            modelBuilder.Conventions.Remove<ManyToManyCascadeDeleteConvention>();

            modelBuilder.Entity<Contact>()
                .Map<Contact>(m => m.Requires("Subtype").HasValue("Contact"))
                .Map<Company>(m => m.Requires("Subtype").HasValue(Constants.ContactTypes.Company))
                .Map<Person>(m => m.Requires("Subtype").HasValue(Constants.ContactTypes.Person))
                ;

            base.OnModelCreating(modelBuilder);
        }

        #endregion

        #region Tables

        public DbSet<Contact> Contacts { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<Person> Persons { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<Timecard> Timecards { get; set; }

        #endregion

        #region Helper methods

        public static void AddParameter(string name, SqlDbType dbType, object value, ICollection<SqlParameter> parameters)
        {
            var param = new SqlParameter(name, dbType);
            param.Value = value != null ? value: DBNull.Value;

            parameters.Add(param);
        }

        public static void AddParameterForLike(string name, object value, ICollection<SqlParameter> parameters)
        {
            var param = new SqlParameter(name, SqlDbType.NVarChar);
            param.Value = value != null && !string.IsNullOrEmpty(value.ToString()) ? (object)string.Format("%{0}%", value) : DBNull.Value;

            parameters.Add(param);
        }

        /// <summary>
        /// http://stackoverflow.com/questions/17710769/can-you-get-the-dbcontext-from-a-dbset
        /// </summary>
        public static GitTimeContext GetContext<TEntity>(DbSet<TEntity> dbSet)
                where TEntity : class
        {
            object internalSet = dbSet
                .GetType()
                .GetField("_internalSet", BindingFlags.NonPublic | BindingFlags.Instance)
                .GetValue(dbSet);

            object internalContext = internalSet
                .GetType()
                .BaseType
                .GetField("_internalContext", BindingFlags.NonPublic | BindingFlags.Instance)
                .GetValue(internalSet);

            return (GitTimeContext)internalContext
                .GetType()
                .GetProperty("Owner", BindingFlags.Instance | BindingFlags.Public)
                .GetValue(internalContext, null);
        }

        #endregion
    }
}