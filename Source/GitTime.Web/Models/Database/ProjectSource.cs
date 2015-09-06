using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace GitTime.Web.Models.Database
{
    public static class ProjectSource
    {
        #region SELECT

        public static ICollection<ProjectFinderRow> SelectFinderRows(this DbSet<Project> dbSet, int startRow, int endRow, ProjectFilter filter, string sortExpression)
        {
            const string query = @"
WITH Project AS
(
    SELECT
        Project.pk_ID AS ProjectID
       ,Project.Name AS ProjectName
       ,Project.Repository
       ,Company.Name AS CompanyName
       ,ROW_NUMBER() OVER (ORDER BY {1}) AS RowNumber
    FROM
        p.Project
        INNER JOIN c.Contact AS Company ON Company.pk_ID = Project.fk_CompanyContactID
    WHERE
        {0}
)
SELECT * FROM Project
WHERE RowNumber BETWEEN @StartRow AND @EndRow
ORDER BY RowNumber
";

            if (string.IsNullOrEmpty(sortExpression))
                sortExpression = "ProjectName";

            string rowNumberOrderBy = sortExpression
                .Replace("ProjectName", "Project.Name")
                ;

            string where = CreateWhere(filter);
            string curQuery = string.Format(query, where, rowNumberOrderBy);

            ICollection<SqlParameter> parameters = CreateParameters(filter);
            GitTimeContext.AddParameter("@StartRow", SqlDbType.Int, startRow, parameters);
            GitTimeContext.AddParameter("@EndRow", SqlDbType.Int, endRow, parameters);

            return GitTimeContext.GetContext(dbSet).Database.SqlQuery<ProjectFinderRow>(curQuery, parameters.ToArray()).ToList();

        }

        public static int Count(this DbSet<Project> dbSet, ProjectFilter filter)
        {
            const string query = @"SELECT CAST(COUNT(*) AS INT) FROM p.Project WHERE {0}";

            string where = CreateWhere(filter);
            string curQuery = string.Format(query, where);

            ICollection<SqlParameter> parameters = CreateParameters(filter);

            return GitTimeContext.GetContext(dbSet).Database.SqlQuery<int>(curQuery, parameters.ToArray()).FirstOrDefault();
        }

        #endregion

        #region DELETE

        public static void Delete(this DbSet<Project> dbSet, int id)
        {
            const string query = "DELETE p.Project WHERE pk_ID = @ID";

            GitTimeContext.GetContext(dbSet).Database.ExecuteSqlCommand(query, new SqlParameter("@ID", id));
        }

        #endregion

        #region Helper methods

        private static string CreateWhere(ProjectFilter filter)
        {
            StringBuilder where = new StringBuilder("1 = 1");

            if (filter.CompanyContactID.HasValue)
                where.Append(" AND Project.fk_CompanyContactID = @CompanyContactID");

            return where.ToString();
        }

        private static ICollection<SqlParameter> CreateParameters(ProjectFilter filter)
        {
            List<SqlParameter> parameters = new List<SqlParameter>();

            if (filter.CompanyContactID.HasValue)
                GitTimeContext.AddParameter("@CompanyContactID", SqlDbType.Int, filter.CompanyContactID, parameters);

            return parameters;
        }

        #endregion
    }
}