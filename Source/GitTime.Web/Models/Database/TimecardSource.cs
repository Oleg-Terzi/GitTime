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
    public static class TimecardSource
    {
        #region SELECT

        public static async Task<ICollection<TimecardFinderRow>> SelectFinderRowsAsync(this DbSet<Timecard> dbSet, Int32 startRow, Int32 endRow, TimecardFilter filter, String sortExpression)
        {
            const String query = @"
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

            if (String.IsNullOrEmpty(sortExpression))
                sortExpression = "EntryDate DESC, TimecardID DESC";

            String rowNumberOrderBy = sortExpression
                .Replace("TimecardID", "Timecard.pk_ID")
                ;

            String where = CreateWhere(filter);
            String curQuery = String.Format(query, where, rowNumberOrderBy);

            ICollection<SqlParameter> parameters = CreateParameters(filter);
            GitTimeContext.AddParameter("@StartRow", SqlDbType.Int, startRow, parameters);
            GitTimeContext.AddParameter("@EndRow", SqlDbType.Int, endRow, parameters);

            return await GitTimeContext.GetContext(dbSet).Database.SqlQuery<TimecardFinderRow>(curQuery, parameters.ToArray()).ToListAsync();

        }

        public static async Task<Int32> CountAsync(this DbSet<Timecard> dbSet, TimecardFilter filter)
        {
            const String query = @"SELECT CAST(COUNT(*) AS INT) FROM t.Timecard WHERE {0}";

            String where = CreateWhere(filter);
            String curQuery = String.Format(query, where);

            ICollection<SqlParameter> parameters = CreateParameters(filter);

            return await GitTimeContext.GetContext(dbSet).Database.SqlQuery<Int32>(curQuery, parameters.ToArray()).FirstOrDefaultAsync();
        }

        public static async Task<Decimal> SumHoursAsync(this DbSet<Timecard> dbSet, TimecardFilter filter)
        {
            const String query = @"SELECT SUM(Hours) FROM t.Timecard WHERE {0}";

            String where = CreateWhere(filter);
            String curQuery = String.Format(query, where);

            ICollection<SqlParameter> parameters = CreateParameters(filter);

            return await GitTimeContext.GetContext(dbSet).Database.SqlQuery<Decimal?>(curQuery, parameters.ToArray()).FirstOrDefaultAsync() ?? 0;
        }

        #endregion

        #region DELETE

        public static async Task DeleteAsync(this DbSet<Timecard> dbSet, Int32 id)
        {
            const String query = "DELETE t.Timecard WHERE pk_ID = @ID";

            await GitTimeContext.GetContext(dbSet).Database.ExecuteSqlCommandAsync(query, new SqlParameter("@ID", id));
        }

        #endregion

        #region Helper methods

        private static String CreateWhere(TimecardFilter filter)
        {
            var where = new StringBuilder("1 = 1");

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
            var parameters = new List<SqlParameter>();

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