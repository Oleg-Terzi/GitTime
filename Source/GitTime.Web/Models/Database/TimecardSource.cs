using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace GitTime.Web.Models.Database
{
    public static class TimecardSource
    {
        #region SELECT

        public static ICollection<TimecardFinderRow> SelectFinderRows(this DbSet<Timecard> dbSet, int startRow, int endRow, TimecardFilter filter, string sortExpression)
        {
            const string query = @"
WITH Timecard AS
(
    SELECT
        Timecard.pk_ID AS TimecardID
       ,Timecard.EntryDate
       ,Person.FirstName AS PersonName
       ,Project.Repository
       ,Company.Name AS CompanyName
       ,Timecard.IssueDescription
       ,Timecard.[Hours]
       ,ROW_NUMBER() OVER (ORDER BY {1}) AS RowNumber
    FROM
        t.Timecard
        INNER JOIN c.Contact AS Person ON Person.pk_ID = Timecard.fk_PersonContactID
        INNER JOIN p.Project ON Project.pk_ID = Timecard.fk_ProjectID
        INNER JOIN c.Contact AS Company ON Company.pk_ID = Project.fk_CompanyContactID
    WHERE
        {0}
)
SELECT * FROM Timecard
WHERE RowNumber BETWEEN @StartRow AND @EndRow
ORDER BY RowNumber
";

            if (string.IsNullOrEmpty(sortExpression))
                sortExpression = "EntryDate DESC, TimecardID DESC";

            string rowNumberOrderBy = sortExpression
                .Replace("TimecardID", "Timecard.pk_ID")
                ;

            string where = CreateWhere(filter);
            string curQuery = string.Format(query, where, rowNumberOrderBy);

            ICollection<SqlParameter> parameters = CreateParameters(filter);
            GitTimeContext.AddParameter("@StartRow", SqlDbType.Int, startRow, parameters);
            GitTimeContext.AddParameter("@EndRow", SqlDbType.Int, endRow, parameters);

            return GitTimeContext.GetContext(dbSet).Database.SqlQuery<TimecardFinderRow>(curQuery, parameters.ToArray()).ToList();

        }

        public static int Count(this DbSet<Timecard> dbSet, TimecardFilter filter)
        {
            const string query = @"
SELECT
    CAST(COUNT(*) AS INT)
FROM
    t.Timecard
WHERE
    {0}
";

            string where = CreateWhere(filter);
            string curQuery = string.Format(query, where);

            ICollection<SqlParameter> parameters = CreateParameters(filter);

            return GitTimeContext.GetContext(dbSet).Database.SqlQuery<int>(curQuery, parameters.ToArray()).FirstOrDefault();

        }

        #endregion

        #region DELETE

        public static void Delete(this DbSet<Timecard> dbSet, int id)
        {
            const string query = "DELETE t.Timecard WHERE pk_ID = @ID";

            GitTimeContext.GetContext(dbSet).Database.ExecuteSqlCommand(query, new SqlParameter("@ID", id));
        }

        #endregion

        #region Helper methods

        private static string CreateWhere(TimecardFilter filter)
        {
            StringBuilder where = new StringBuilder("1 = 1");

            if (filter.ProjectID.HasValue)
                where.Append(" AND Timecard.fk_ProjectID = @ProjectID");

            if (filter.PersonContactID.HasValue)
                where.Append(" AND Timecard.fk_PersonContactID = @PersonContactID");

            if (filter.EntryDateFrom.HasValue)
                where.Append(" AND Timecard.EntryDate >= @EntryDateFrom");

            if (filter.EntryDateThru.HasValue)
                where.Append(" AND Timecard.EntryDate < @EntryDateThru");

            return where.ToString();
        }

        private static ICollection<SqlParameter> CreateParameters(TimecardFilter filter)
        {
            List<SqlParameter> parameters = new List<SqlParameter>();

            if (filter.ProjectID.HasValue)
                GitTimeContext.AddParameter("@ProjectID", SqlDbType.Int, filter.ProjectID, parameters);

            if (filter.PersonContactID.HasValue)
                GitTimeContext.AddParameter("@PersonContactID", SqlDbType.Int, filter.PersonContactID, parameters);

            if (filter.EntryDateFrom.HasValue)
                GitTimeContext.AddParameter("@EntryDateFrom", SqlDbType.DateTime, filter.EntryDateFrom.Value.Date, parameters);

            if (filter.EntryDateThru.HasValue)
                GitTimeContext.AddParameter("@EntryDateThru", SqlDbType.DateTime, filter.EntryDateThru.Value.Date.AddDays(1), parameters);

            return parameters;
        }

        #endregion
    }
}