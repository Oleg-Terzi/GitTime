using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

using GitTime.Web.Models;
using GitTime.Web.Models.Database;
using GitTime.Web.Models.View.Timecard;

namespace GitTime.Web.Controllers
{
    public class TimecardController : BaseController
    {
        #region Actions

        public ActionResult Find()
        {
            LoadData();

            return View();
        }

        [HttpPost]
        public ActionResult Find(FindModel model)
        {
            LoadData();

            return View();
        }

        [HttpPost]
        public ActionResult Create()
        {
            var model = new EditModel() { EntryDate = DateTime.Now };

            using (var db = new TimeTrackerContext())
            {
                model.PersonFullName = db.Persons
                    .Where(p => p.Email == User.Identity.Name)
                    .Select(p => new { FullName = p.FirstName + " " + p.LastName }).First().FullName;
            }

            return PartialView("Edit", model);
        }

        [HttpPost]
        public ActionResult Save(EditModel model)
        {
            if (!ModelState.IsValid || !SaveData(model))
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;

                return PartialView("Edit", model);
            }

            LoadData();

            ViewBag.IsAdded = true;

            return PartialView("SearchResults");
        }

        #endregion

        #region Database methods

        private void LoadData()
        {
            using (var db = new TimeTrackerContext())
            {
                int count = db.Timecards.Count(new TimecardFilter { });

                ViewBag.DataSource = db.Timecards.SelectFinderRows(1, 100, new TimecardFilter { }, null);
            }
        }

        private bool SaveData(EditModel model)
        {
            using (var db = new TimeTrackerContext())
            {
                Timecard row = model.ID.HasValue
                    ? db.Timecards.Find(model.ID.Value)
                    : new Timecard();

                if (row == null)
                {
                    ViewBag.ErrorMessage = "This timecard doesn't exist anymore. Please create new one.";
                    return false;
                }

                int? personContactID = FindPerson(model, db);

                if (personContactID == null)
                    return false;

                int? projectID = FindProject(model, db);

                if (projectID == null)
                    return false;

                row.EntryDate = model.EntryDate;
                row.PersonContactID = personContactID.Value;
                row.IssueNumber = model.IssueNumber;
                row.Hours = model.Hours.Value;
                row.ProjectID = projectID.Value;
                row.IssueDescription = model.IssueDescription;

                if (model.ID.HasValue)
                    db.Entry(row).State = EntityState.Modified;
                else
                    db.Timecards.Add(row);

                db.SaveChanges();
            }

            return true;
        }

        private int? FindPerson(EditModel model, TimeTrackerContext db)
        {
            var persons = db.Persons
                .Where(p => p.FirstName + " " + p.LastName == model.PersonFullName)
                .Select(p => p.ID ).ToList();

            if (persons.Count == 1)
                return persons[0];

            ModelState.AddModelError("PersonFullName", "Specified person doesn't exist.");

            return null;
        }

        private int? FindProject(EditModel model, TimeTrackerContext db)
        {
            var projects = db.Projects
                .Where(p => p.Name == model.ProjectName)
                .Select(p => p.ID).ToList();

            if (projects.Count == 1)
                return projects[0];

            ModelState.AddModelError("ProjectName", "Specified project doesn't exist.");

            return null;
        }

        #endregion
    }
}