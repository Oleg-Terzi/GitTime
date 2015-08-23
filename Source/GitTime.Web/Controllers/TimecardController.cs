﻿using System;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;
using System.Web.Mvc;

using GitTime.Web.Models;
using GitTime.Web.Models.Database;
using GitTime.Web.Models.View;

namespace GitTime.Web.Controllers
{
    public class TimecardController : BaseController
    {
        #region Actions

        public ActionResult Find()
        {
            TimecardFilter filter = GetInitFilter();

            FindModel model = new FindModel();

            model.SearchResults = new FindModel.SearchResultsModel
            {
                PageIndex = 0,
                Filter = SerializeFilter(filter)
            };

            model.SearchCriteria = GetSearchCriteria(filter);

            LoadData(model.SearchResults);

            return View(model);
        }

        [HttpPost]
        public ActionResult Search(FindModel model)
        {
            TimecardFilter filter;

            if (model.SearchCriteria.Clear)
            {
                filter = GetInitFilter();
            }
            else
            {
                filter = new TimecardFilter
                {
                    ProjectName = model.SearchCriteria.ProjectName,
                    PersonName = model.SearchCriteria.PersonName,
                    EntryDateFrom = model.SearchCriteria.EntryDateFrom,
                    EntryDateThru = model.SearchCriteria.EntryDateThru
                };
            }

            model.SearchResults.Filter = SerializeFilter(filter);

            model.SearchCriteria = GetSearchCriteria(filter);

            LoadData(model.SearchResults);

            return View("Find", model);
        }

        [HttpPost]
        public ActionResult Create(FindModel model)
        {
            model.Edit = new FindModel.EditModel() { EntryDate = DateTime.Now };

            using (var db = new GitTimeContext())
            {
                model.Edit.PersonContactID = db.Persons
                    .Where(p => p.Email == User.Identity.Name)
                    .Select(p => p.ID).First();
            }

            return PartialView("Edit", model);
        }

        [HttpPost]
        public ActionResult Edit(FindModel model)
        {
            using (var db = new GitTimeContext())
            {
                model.Edit = db.Timecards
                    .Where(p => p.ID == model.Key.Value)
                    .Select(p => new FindModel.EditModel
                    {
                        ID = p.ID,
                        EntryDate = p.EntryDate,
                        PersonContactID = p.PersonContactID,
                        ProjectID = p.ProjectID,
                        IssueNumber = p.IssueNumber,
                        IssueDescription = p.IssueDescription,
                        Hours = p.Hours
                    }).First();
            }

            return PartialView("Edit", model);
        }

        [HttpPost]
        public ActionResult Save(FindModel model)
        {
            if (!ModelState.IsValid || !SaveData(model.Edit))
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;

                return PartialView("Edit", model);
            }

            LoadData(model.SearchResults);

            ViewBag.Operation = model.Edit.ID.HasValue ? "Edited" : "Added";

            return PartialView("SearchResults", model);
        }

        [HttpPost]
        public ActionResult Delete(FindModel model)
        {
            using (GitTimeContext db = new GitTimeContext())
            {
                db.Timecards.Delete(model.Key.Value);
            }

            LoadData(model.SearchResults);

            ViewBag.Operation = "Deleted";

            return PartialView("SearchResults", model);
        }

        #endregion

        #region Database methods

        private void LoadData(FindModel.SearchResultsModel model)
        {
            using (var db = new GitTimeContext())
            {
                int count = db.Timecards.Count(new TimecardFilter { });

                ViewBag.DataSource = db.Timecards.SelectFinderRows(1, 100, new TimecardFilter { }, null);
            }
        }

        private bool SaveData(FindModel.EditModel model)
        {
            using (var db = new GitTimeContext())
            {
                Timecard row = model.ID.HasValue
                    ? db.Timecards.Find(model.ID.Value)
                    : new Timecard();

                if (row == null)
                {
                    ViewBag.ErrorMessage = "This timecard doesn't exist anymore. Please create new one.";
                    return false;
                }

                row.EntryDate = model.EntryDate;
                row.PersonContactID = model.PersonContactID.Value;
                row.IssueNumber = model.IssueNumber;
                row.Hours = model.Hours.Value;
                row.ProjectID = model.ProjectID.Value;
                row.IssueDescription = model.IssueDescription;

                if (model.ID.HasValue)
                    db.Entry(row).State = EntityState.Modified;
                else
                    db.Timecards.Add(row);

                db.SaveChanges();
            }

            return true;
        }

        private string SerializeFilter(TimecardFilter filter)
        {
            byte[] buffer;

            using(MemoryStream stream = new MemoryStream())
            {
                new BinaryFormatter().Serialize(stream, filter);

                buffer = new byte[stream.Length];

                stream.Position = 0;
                stream.Read(buffer, 0, (int)stream.Length);
            }

            return Convert.ToBase64String(buffer);
        }

        private TimecardFilter DeserializeFilter(string filter)
        {
            byte[] buffer = Convert.FromBase64String(filter);

            using(MemoryStream stream = new MemoryStream(buffer))
            {
                return (TimecardFilter)(new BinaryFormatter().Deserialize(stream));
            }
        }

        private TimecardFilter GetInitFilter()
        {
            return new TimecardFilter();
        }

        private FindModel.SearchCriteriaModel GetSearchCriteria(TimecardFilter filter)
        {
            return new FindModel.SearchCriteriaModel
            {
                ProjectName = filter.ProjectName,
                PersonName = filter.PersonName,
                EntryDateFrom = filter.EntryDateFrom,
                EntryDateThru = filter.EntryDateThru
            };
        }

        #endregion
    }
}