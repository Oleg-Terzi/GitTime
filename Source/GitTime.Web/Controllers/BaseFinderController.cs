using System;
using System.IO;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;
using System.Web.Mvc;

using GitTime.Web.Models.View;

namespace GitTime.Web.Controllers
{
    public abstract class BaseFinderController<ModelType, FilterType>: BaseController
        where ModelType: new()
    {
        #region Enums

        protected enum SaveResult { NotSaved, Added, Edited }

        #endregion

        #region Properties

        protected abstract string SingleEntityName { get; }
        protected abstract string MultiEntityName { get; }

        #endregion

        #region Actions

        public ActionResult Find()
        {
            var filter = GetInitFilter();
            var model = new ModelType();

            var searchResults = new BaseSearchResultsModel
            {
                PageIndex = 0,
                SerializedFilter = SerializeFilter(filter)
            };

            InitModel(model, searchResults, filter);

            LoadData(searchResults, filter);

            return View(model);
        }

        [HttpPost]
        public ActionResult Search(ModelType model)
        {
            FilterType filter = GetFilterBySearchCriteria(model);

            var searchResults = new BaseSearchResultsModel
            {
                PageIndex = 0,
                SerializedFilter = SerializeFilter(filter)
            };

            InitModel(model, searchResults, filter);

            LoadData(searchResults, filter);

            ModelState.Clear();//Allows to re-bind strongly typed controls from model rather than from model state

            return View("Find", model);
        }

        [HttpPost]
        public ActionResult ChangePage(ModelType model)
        {
            var searchResults = GetSearchResults(model);
            var filter = DeserializeFilter(searchResults.SerializedFilter);

            LoadData(searchResults, filter);

            return PartialView("SearchResults", model);
        }

        [HttpPost]
        public ActionResult Create(ModelType model)
        {
            InitCreate(model);

            return PartialView("Edit", model);
        }

        [HttpPost]
        public ActionResult Edit(ModelType model)
        {
            InitEdit(model);

            return PartialView("Edit", model);
        }

        [HttpPost]
        public ActionResult Save(ModelType model)
        {
            SaveResult result;

            if (!ModelState.IsValid || (result = SaveData(model)) == SaveResult.NotSaved)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;

                return PartialView("Edit", model);
            }

            var searchResults = GetSearchResults(model);
            var filter = DeserializeFilter(searchResults.SerializedFilter);

            LoadData(searchResults, filter);

            ViewBag.Operation = result == SaveResult.Edited ? "Edited" : "Added";

            return PartialView("SearchResults", model);
        }

        [HttpPost]
        public ActionResult Delete(ModelType model)
        {
            DeleteData(model);

            var searchResults = GetSearchResults(model);
            var filter = DeserializeFilter(searchResults.SerializedFilter);

            LoadData(searchResults, filter);

            ViewBag.Operation = "Deleted";

            return PartialView("SearchResults", model);
        }

        #endregion

        #region Initialization

        protected abstract FilterType GetInitFilter();
        protected abstract FilterType GetFilterBySearchCriteria(ModelType model);
        protected abstract BaseSearchResultsModel GetSearchResults(ModelType model);

        protected abstract void InitModel(ModelType model, BaseSearchResultsModel searchResults, FilterType filter);
        protected abstract void InitCreate(ModelType model);
        protected abstract void InitEdit(ModelType model);

        #endregion

        #region Database methods

        protected abstract int Count(FilterType filter);
        protected abstract object Select(int startRow, int endRow, FilterType filter);

        protected abstract SaveResult SaveData(ModelType model);
        protected abstract void DeleteData(ModelType model);

        private void LoadData(BaseSearchResultsModel searchResults, FilterType filter)
        {
            int startRow, endRow, rowCount, pageCount;

            rowCount = Count(filter);

            pageCount = rowCount / Constants.PageSize;

            if (rowCount % Constants.PageSize != 0)
                pageCount++;

            if (searchResults.PageIndex < 0 || searchResults.PageIndex >= pageCount)
                searchResults.PageIndex = 0;

            startRow = searchResults.PageIndex * Constants.PageSize + 1;
            endRow = startRow + Constants.PageSize - 1;

            if (endRow > rowCount)
                endRow = rowCount;

            ViewBag.DataSource = Select(startRow, endRow, filter);
            ViewBag.SingleEntityName = SingleEntityName;
            ViewBag.MultiEntityName = MultiEntityName;
            ViewBag.SerializedFilter = searchResults.SerializedFilter;
            ViewBag.StartRow = startRow;
            ViewBag.EndRow = endRow;
            ViewBag.RowCount = rowCount;
            ViewBag.PageIndex = searchResults.PageIndex;
            ViewBag.PageCount = pageCount;
        }

        #endregion

        #region Serialization

        private string SerializeFilter(FilterType filter)
        {
            byte[] buffer;

            using (MemoryStream stream = new MemoryStream())
            {
                new BinaryFormatter().Serialize(stream, filter);

                buffer = new byte[stream.Length];

                stream.Position = 0;
                stream.Read(buffer, 0, (int)stream.Length);
            }

            return Convert.ToBase64String(buffer);
        }

        private FilterType DeserializeFilter(string serializedFilter)
        {
            byte[] buffer = Convert.FromBase64String(serializedFilter);

            using (MemoryStream stream = new MemoryStream(buffer))
            {
                return (FilterType)(new BinaryFormatter().Deserialize(stream));
            }
        }

        #endregion
    }
}