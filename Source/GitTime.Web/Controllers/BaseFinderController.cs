using System;
using System.IO;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using System.Web.Mvc;
using GitTime.Web.Infrastructure.Helpers;
using GitTime.Web.Models.View;

namespace GitTime.Web.Controllers
{
    public abstract class BaseFinderController<ModelType, FilterType> : BaseController
        where ModelType : new()
    {
        #region Enums

        protected enum SaveResult { NotSaved, Added, Edited }

        #endregion

        #region Properties

        protected abstract String SingleEntityName { get; }
        protected abstract String MultiEntityName { get; }

        #endregion

        #region Actions

        public async Task<ActionResult> Find()
        {
            var filter = GetInitFilter();
            var model = new ModelType();

            var searchResults = new BaseSearchResultsModel
            {
                PageIndex = 0,
                SerializedFilter = SerializeFilter(filter)
            };

            InitModel(model, searchResults, filter);

            await LoadData(searchResults, filter);

            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> Search(ModelType model)
        {
            FilterType filter = GetFilterBySearchCriteria(model);

            var searchResults = new BaseSearchResultsModel
            {
                PageIndex = 0,
                SerializedFilter = SerializeFilter(filter)
            };

            InitModel(model, searchResults, filter);

            await LoadData(searchResults, filter);

            ModelState.Clear(); //Allows to re-bind strongly typed controls from model rather than from model state

            return View("Find", model);
        }

        [HttpPost]
        public async Task<ActionResult> ChangePage(ModelType model)
        {
            var searchResults = GetSearchResults(model);
            var filter = DeserializeFilter(searchResults.SerializedFilter);

            await LoadData(searchResults, filter);

            return PartialView("SearchResults", model);
        }

        [HttpPost]
        public async Task<ActionResult> Create(ModelType model)
        {
            await InitCreate(model);

            return PartialView("Edit", model);
        }

        [HttpPost]
        public async Task<ActionResult> Edit(ModelType model)
        {
            await InitEdit(model);

            return PartialView("Edit", model);
        }

        [HttpPost]
        public async Task<ActionResult> Save(ModelType model)
        {
            SaveResult result;

            if (!ModelState.IsValid || (result = await SaveData(model)) == SaveResult.NotSaved)
            {
                Response.StatusCode = (Int32)HttpStatusCode.BadRequest;

                return PartialView("Edit", model);
            }

            var searchResults = GetSearchResults(model);
            var filter = DeserializeFilter(searchResults.SerializedFilter);

            await LoadData(searchResults, filter);

            ViewBag.Operation = result == SaveResult.Edited ? "Edited" : "Added";

            return PartialView("SearchResults", model);
        }

        [HttpPost]
        public async Task<ActionResult> Delete(ModelType model)
        {
            await DeleteData(model);

            var searchResults = GetSearchResults(model);
            var filter = DeserializeFilter(searchResults.SerializedFilter);

            await LoadData(searchResults, filter);

            ViewBag.Operation = "Deleted";

            return PartialView("SearchResults", model);
        }

        #endregion

        #region Initialization

        protected abstract FilterType GetInitFilter();
        protected abstract FilterType GetFilterBySearchCriteria(ModelType model);
        protected abstract BaseSearchResultsModel GetSearchResults(ModelType model);

        protected abstract void InitModel(ModelType model, BaseSearchResultsModel searchResults, FilterType filter);
        protected abstract Task InitCreate(ModelType model);
        protected abstract Task InitEdit(ModelType model);

        #endregion

        #region Database methods

        protected abstract Task<Int32> Count(FilterType filter);
        protected abstract Task<Object> Select(Int32 startRow, Int32 endRow, FilterType filter);

        protected abstract Task<SaveResult> SaveData(ModelType model);
        protected abstract Task DeleteData(ModelType model);

        private async Task LoadData(BaseSearchResultsModel searchResults, FilterType filter)
        {
            Int32 startRow, endRow, rowCount, pageCount;

            rowCount = await Count(filter);

            pageCount = rowCount / Constants.PageSize;

            if (rowCount % Constants.PageSize != 0)
                pageCount++;

            if (searchResults.PageIndex < 0 || searchResults.PageIndex >= pageCount)
                searchResults.PageIndex = 0;

            startRow = searchResults.PageIndex * Constants.PageSize + 1;
            endRow = startRow + Constants.PageSize - 1;

            if (endRow > rowCount)
                endRow = rowCount;

            ViewBag.DataSource = await Select(startRow, endRow, filter);
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

        private String SerializeFilter(FilterType filter)
        {
            byte[] buffer;

            using (var stream = new MemoryStream())
            {
                new BinaryFormatter().Serialize(stream, filter);

                buffer = new byte[stream.Length];

                stream.Position = 0;
                stream.Read(buffer, 0, (Int32)stream.Length);
            }

            return Convert.ToBase64String(buffer);
        }

        private FilterType DeserializeFilter(String serializedFilter)
        {
            Byte[] buffer = Convert.FromBase64String(serializedFilter);

            using (var stream = new MemoryStream(buffer))
            {
                return (FilterType)(new BinaryFormatter().Deserialize(stream));
            }
        }

        #endregion
    }
}