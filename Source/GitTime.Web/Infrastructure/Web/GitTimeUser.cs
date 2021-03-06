﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Web;

using GitTime.Web.Models;
using GitTime.Web.Models.Database;

namespace GitTime.Web.Infrastructure.Web
{
    public class GitTimeUser : IIdentity, IPrincipal
    {
        #region Properties

        public static GitTimeUser Current
        {
            get { return (GitTimeUser)HttpContext.Current.User; }
        }

        public Int32 ID { get; private set; }
        public String Email { get; private set; }
        public String FirstName { get; private set; }
        public String LastName { get; private set; }
        public HashSet<String> Roles { get; private set; }

        // IIdentity

        public String AuthenticationType
        {
            get { return "Custom Authentication"; }
        }

        public Boolean IsAuthenticated { get; private set; }

        public String Name
        {
            get { return Email; }
        }

        // IPrincipal

        public IIdentity Identity
        {
            get { return this; }
        }

        #endregion

        #region Construction

        private GitTimeUser(Person p)
        {
            if (p == null)
                return;

            ID = p.ID;
            Email = p.Email;
            FirstName = p.FirstName;
            LastName = p.LastName;
            Roles = new HashSet<String>(p.Roles.Select(r => r.Name).ToArray());
            IsAuthenticated = true;
        }

        #endregion

        #region Public methods

        public Boolean IsInRole(String roleName)
        {
            return Roles.Contains(roleName);
        }

        #endregion

        #region Public static methods

        public async static Task<GitTimeUser> Open(String email)
        {
            GitTimeUser user = null;

            if (!String.IsNullOrEmpty(email))
            {
                using (var db = new GitTimeContext())
                {
                    var contact = await (
                        db.Persons
                            .Where(c => c.Email == email)
                            .Select(c => c)
                        ).AsNoTracking().FirstOrDefaultAsync();

                    user = new GitTimeUser(contact);
                }
            }

            return user ?? new GitTimeUser(null);
        }

        #endregion
    }
}