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
    public static class ContactSource
    {
        #region SELECT

        public static async Task<ICollection<ContactFinderRow>> SelectFinderRowsAsync(this DbSet<Contact> dbSet, Int32 startRow, Int32 endRow, ContactFilter filter, String sortExpression)
        {
            const String query = @"
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

            if (String.IsNullOrEmpty(sortExpression))
                sortExpression = "ContactID";

            String rowNumberOrderBy = sortExpression
                .Replace("ContactID", "Contact.pk_ID")
                ;

            String where = CreateWhere(filter);
            String curQuery = String.Format(query, where, rowNumberOrderBy);

            ICollection<SqlParameter> parameters = CreateParameters(filter);
            GitTimeContext.AddParameter("@StartRow", SqlDbType.Int, startRow, parameters);
            GitTimeContext.AddParameter("@EndRow", SqlDbType.Int, endRow, parameters);

            return await GitTimeContext.GetContext(dbSet).Database.SqlQuery<ContactFinderRow>(curQuery, parameters.ToArray()).ToListAsync();

        }

        public static async Task<Int32> CountAsync(this DbSet<Contact> dbSet, ContactFilter filter)
        {
            const String query = @"SELECT CAST(COUNT(*) AS INT) FROM c.Contact WHERE {0}";

            String where = CreateWhere(filter);
            String curQuery = String.Format(query, where);

            ICollection<SqlParameter> parameters = CreateParameters(filter);

            return await GitTimeContext.GetContext(dbSet).Database.SqlQuery<Int32>(curQuery, parameters.ToArray()).FirstOrDefaultAsync();
        }

        #endregion

        #region DELETE

        public static async Task DeleteAsync(this DbSet<Contact> dbSet, Int32 id)
        {
            const String query = "DELETE c.Contact WHERE pk_ID = @ID";

            await GitTimeContext.GetContext(dbSet).Database.ExecuteSqlCommandAsync(query, new SqlParameter("@ID", id));
        }

        #endregion

        #region Helper methods

        private static String CreateWhere(ContactFilter filter)
        {
            var where = new StringBuilder("1 = 1");

            if (!String.IsNullOrEmpty(filter.Subtype))
                where.Append(" AND Contact.Subtype = @Subtype");

            if (!String.IsNullOrEmpty(filter.Name))
                where.Append(" AND Contact.Name LIKE @Name");

            if (!String.IsNullOrEmpty(filter.PersonName))
                where.Append(" AND (Contact.FirstName + ' ' + Contact.LastName LIKE @PersonName OR Contact.LastName + ', ' + Contact.FirstName LIKE @PersonName)");

            return where.ToString();
        }

        private static ICollection<SqlParameter> CreateParameters(ContactFilter filter)
        {
            var parameters = new List<SqlParameter>();

            if (!String.IsNullOrEmpty(filter.Subtype))
                GitTimeContext.AddParameter("@Subtype", SqlDbType.NVarChar, filter.Subtype, parameters);

            if (!String.IsNullOrEmpty(filter.Name))
                GitTimeContext.AddParameterForLike("@Name", filter.Name, parameters);

            if (!String.IsNullOrEmpty(filter.PersonName))
                GitTimeContext.AddParameterForLike("@PersonName", filter.PersonName, parameters);

            return parameters;
        }

        #endregion
    }
}