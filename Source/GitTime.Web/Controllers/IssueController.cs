using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using GitTime.Web.Infrastructure.GitHub;

namespace GitTime.Web.Controllers
{
    public class IssueController : Controller
    {
        // GET: Issue
        public async Task<ActionResult> Find()
        {
            var user = await GitHubUser.GetCurrentAsync();
            var context = GitHubHelper.RequestIssuesAsync(user.AccessToken);

            return View();
        }
    }
}