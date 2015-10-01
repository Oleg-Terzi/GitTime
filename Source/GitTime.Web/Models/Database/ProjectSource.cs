using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitTime.Web.Models.Database
{
    public static class ProjectSource
    {
        #region SELECT

        public static async Task<ICollection<ProjectFinderRow>> SelectFinderRowsAsync(this DbSet<Project> dbSet, Int32 startRow, Int32 endRow, ProjectFilter filter, String sortExpression)
        {
            const String query = @"
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

            if (String.IsNullOrEmpty(sortExpression))
                sortExpression = "ProjectName";

            String rowNumberOrderBy = sortExpression
                .Replace("ProjectName", "Project.Name")
                ;

            String where = CreateWhere(filter);
            String curQuery = String.Format(query, where, rowNumberOrderBy);

            ICollection<SqlParameter> parameters = CreateParameters(filter);
            GitTimeContext.AddParameter("@StartRow", SqlDbType.Int, startRow, parameters);
            GitTimeContext.AddParameter("@EndRow", SqlDbType.Int, endRow, parameters);

            return await GitTimeContext.GetContext(dbSet).Database.SqlQuery<ProjectFinderRow>(curQuery, parameters.ToArray()).ToListAsync();

        }

        public static async Task<Int32> CountAsync(this DbSet<Project> dbSet, ProjectFilter filter)
        {
            const String query = @"SELECT CAST(COUNT(*) AS INT) FROM p.Project WHERE {0}";

            String where = CreateWhere(filter);
            String curQuery = String.Format(query, where);

            ICollection<SqlParameter> parameters = CreateParameters(filter);

            return await GitTimeContext.GetContext(dbSet).Database.SqlQuery<Int32>(curQuery, parameters.ToArray()).FirstOrDefaultAsync();
        }

        #endregion

        #region DELETE

        public static async Task DeleteAsync(this DbSet<Project> dbSet, Int32 id)
        {
            const String query = "DELETE p.Project WHERE pk_ID = @ID";

            await GitTimeContext.GetContext(dbSet).Database.ExecuteSqlCommandAsync(query, new SqlParameter("@ID", id));
        }

        #endregion

        #region Helper methods

        private static String CreateWhere(ProjectFilter filter)
        {
            var where = new StringBuilder("1 = 1");

            if (filter.CompanyContactID.HasValue)
                where.Append(" AND Project.fk_CompanyContactID = @CompanyContactID");

            return where.ToString();
        }

        private static ICollection<SqlParameter> CreateParameters(ProjectFilter filter)
        {
            var parameters = new List<SqlParameter>();

            if (filter.CompanyContactID.HasValue)
                GitTimeContext.AddParameter("@CompanyContactID", SqlDbType.Int, filter.CompanyContactID, parameters);

            return parameters;
        }

        #endregion
    }
}