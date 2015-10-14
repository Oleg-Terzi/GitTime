using System.Threading.Tasks;
using System.Web.Mvc;

using GitTime.Web.Infrastructure.GitHub;

namespace GitTime.Web.Controllers
{
    public class HomeController : BaseController
    {
        #region Actions

        public async Task<ActionResult> Index()
        {
            var gitHubUser = await GitHubUser.GetCurrentAsync();

            return gitHubUser.IsAuthenticated
                ? RedirectToAction("Find", "Issue")
                : RedirectToAction("Find", "Timecard");
        }

        #endregion
    }
}