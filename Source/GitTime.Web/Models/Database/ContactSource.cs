using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace GitTime.Web.Models.Database
{
    public static class ContactSource
    {
        #region SELECT

        public static ICollection<ContactFinderRow> SelectFinderRows(this DbSet<Contact> dbSet, int startRow, int endRow, ContactFilter filter, string sortExpression)
        {
            const string query = @"
WITH Contact AS
(
    SELECT
        Contact.pk_ID AS ContactID
       ,Contact.Name
       ,Contact.Email
       ,Contact.FirstName
       ,Contact.LastName
       ,Contact.Subtype
       ,ROW_NUMBER() OVER (ORDER BY {1}) AS RowNumber
    FROM
        c.Contact
    WHERE
        {0}
)
SELECT * FROM Contact
WHERE RowNumber BETWEEN @StartRow AND @EndRow
ORDER BY RowNumber
";

            if (string.IsNullOrEmpty(sortExpression))
                sortExpression = "ContactID";

            string rowNumberOrderBy = sortExpression
                .Replace("ContactID", "Contact.pk_ID")
                ;

            string where = CreateWhere(filter);
            string curQuery = string.Format(query, where, rowNumberOrderBy);

            ICollection<SqlParameter> parameters = CreateParameters(filter);
            GitTimeContext.AddParameter("@StartRow", SqlDbType.Int, startRow, parameters);
            GitTimeContext.AddParameter("@EndRow", SqlDbType.Int, endRow, parameters);

            return GitTimeContext.GetContext(dbSet).Database.SqlQuery<ContactFinderRow>(curQuery, parameters.ToArray()).ToList();

        }

        public static int Count(this DbSet<Contact> dbSet, ContactFilter filter)
        {
            const string query = @"SELECT CAST(COUNT(*) AS INT) FROM c.Contact WHERE {0}";

            string where = CreateWhere(filter);
            string curQuery = string.Format(query, where);

            ICollection<SqlParameter> parameters = CreateParameters(filter);

            return GitTimeContext.GetContext(dbSet).Database.SqlQuery<int>(curQuery, parameters.ToArray()).FirstOrDefault();
        }

        #endregion

        #region DELETE

        public static void Delete(this DbSet<Contact> dbSet, int id)
        {
            const string query = "DELETE c.Contact WHERE pk_ID = @ID";

            GitTimeContext.GetContext(dbSet).Database.ExecuteSqlCommand(query, new SqlParameter("@ID", id));
        }

        #endregion

        #region Helper methods

        private static string CreateWhere(ContactFilter filter)
        {
            StringBuilder where = new StringBuilder("1 = 1");

            if (!string.IsNullOrEmpty(filter.Subtype))
                where.Append(" AND Contact.Subtype = @Subtype");

            if (!string.IsNullOrEmpty(filter.Name))
                where.Append(" AND Contact.Name LIKE @Name");

            if (!string.IsNullOrEmpty(filter.PersonName))
                where.Append(" AND (Contact.FirstName + ' ' + Contact.LastName LIKE @PersonName OR Contact.LastName + ', ' + Contact.FirstName LIKE @PersonName)");

            return where.ToString();
        }

        private static ICollection<SqlParameter> CreateParameters(ContactFilter filter)
        {
            List<SqlParameter> parameters = new List<SqlParameter>();

            if (!string.IsNullOrEmpty(filter.Subtype))
                GitTimeContext.AddParameter("@Subtype", SqlDbType.NVarChar, filter.Subtype, parameters);

            if (!string.IsNullOrEmpty(filter.Name))
                GitTimeContext.AddParameterForLike("@Name", filter.Name, parameters);

            if (!string.IsNullOrEmpty(filter.PersonName))
                GitTimeContext.AddParameterForLike("@PersonName", filter.PersonName, parameters);

            return parameters;
        }

        #endregion
    }
}