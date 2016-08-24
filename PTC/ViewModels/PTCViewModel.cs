using PTC.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PTC
{
    public class PTCViewModel
    {
        #region Constructor
        public PTCViewModel()
        {
            Init();
        }
        #endregion

        #region Public Properties
        public List<Product> Products { get; set; }
        public List<Category> Categories { get; set; }
        public ProductSearch SearchEntity { get; set; }
        public List<Category> SearchCategories { get; set; }
        public Product Entity { get; set; }

        public string EventCommand { get; set; }
        public string EventArgument { get; set; }
        public bool IsValid { get; set; }
        public string PageMode { get; set; }
        public bool IsDetailAreaVisible { get; set; }
        public bool IsListAreaVisible { get; set; }
        public bool IsSearchAreaVisible { get; set; }
        public ModelStateDictionary Messages { get; set; }
        #endregion

        #region Init Method
        public void Init()
        {
            Products = new List<Product>();
            Categories = new List<Category>();
            Entity = new Product();

            SearchEntity = new ProductSearch();
            SearchCategories = new List<Category>();

            EventCommand = string.Empty;
            EventArgument = string.Empty;
            IsValid = true;
            IsDetailAreaVisible = false;
            IsListAreaVisible = true;
            IsSearchAreaVisible = true;
            PageMode = PageConstants.LIST;
            Messages = new ModelStateDictionary();

        }
        #endregion

        #region HandleRequest Method
        public void HandleRequest()
        {
            LoadSearchCategories();
            LoadCategories();

            switch (EventCommand.ToLower())
            {
                case "":
                case "list":
                    Get();
                    break;
            }
        }
        #endregion

        #region LoadCategories Method
        private void LoadCategories()
        {
            PTCEntities db = new PTCEntities();
            Categories.AddRange(db.Categories);
        }
        #endregion

        #region LoadSearchCategories Method
        private void LoadSearchCategories()
        {
            PTCEntities db = new PTCEntities();

            if (Categories.Count == 0)
            {
                SearchCategories.AddRange(db.Categories);
            }
            else
            {
                SearchCategories.AddRange(Categories);
            }

            Category entity = new Category();
            entity.CategoryId = 0;
            entity.CategoryName = "--- Search All Categories ---";
            SearchCategories.AddRange(Categories);
            SearchCategories.Insert(0, entity);
        }
        #endregion

        #region Get Methods
        public void Get()
        {
            PTCEntities db = new PTCEntities();
            Products = db.Products.OrderBy(p => p.ProductName).ToList();
            SetUIState(PageConstants.LIST);
        }

        public Product Get(int productId)
        {
            PTCEntities db = new PTCEntities();
            Entity = db.Products.Find(productId);
            return Entity;
        }
        #endregion

        #region Search Method
        public void Search()
        {
            PTCEntities db = new PTCEntities();

            // Perform Search
            Products = db.Products.Where(p =>
                (SearchEntity.CategoryId == 0 ? true :
                    p.Category.CategoryId == SearchEntity.CategoryId) &&
                (string.IsNullOrEmpty(SearchEntity.ProductName) ? true :
                    p.ProductName.Contains(SearchEntity.ProductName))).
                OrderBy(p => p.ProductName).ToList();

            SetUIState(PageConstants.LIST);                
        }        
        #endregion

        #region ResetSearch Method
        public void ResetSearch()
        {
            SearchEntity = new ProductSearch();

            Get();
        }
        #endregion

        #region AddMode Method
        public void AddMode()
        {
            // Initialize Entity Object
            Entity = new Product();
            Entity.IntroductionDate = DateTime.Now;
            Entity.Url = string.Empty;
            Entity.Price = 0;

            SetUIState(PageConstants.ADD);
        }
        #endregion

        #region EditMode Method
        public void EditMode(int productId)
        {
            Entity = Get(productId);

            SetUIState(PageConstants.EDIT);
        }
        #endregion

        #region Save Method
        public void Save()
        {
            Messages.Clear();
            PTCEntities db = new PTCEntities();

            Entity.Category = db.Categories.Find(Entity.Category.CategoryId);

            try
            {
                if (PageMode == PageConstants.EDIT)
                {
                    db.Entry(Entity).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                }
                else if (PageMode == PageConstants.ADD){
                    db.Products.Add(Entity);
                    db.SaveChanges();
                }

                Get();
            }
            catch (DbEntityValidationException ex)
            {
                IsValid = false;
                foreach (var errors in ex.EntityValidationErrors)
                {
                    foreach(var item in errors.ValidationErrors)
                    {
                        Messages.AddModelError(item.PropertyName, item.ErrorMessage);
                    }
                }

                SetUIState(PageMode);
            }
        }
        #endregion

        #region Delete Method
        public void Delete(int productId)
        {
            PTCEntities db = new PTCEntities();

            Product product = db.Products.Find(productId);
            db.Products.Remove(product);
            db.SaveChanges();

            Get();
        }
        #endregion

        #region SetUIState Method
        protected void SetUIState(string state)
        {
            PageMode = state;

            IsDetailAreaVisible = (PageMode == "Add" || PageMode == "Edit");
            IsListAreaVisible = (PageMode == "List");
            IsSearchAreaVisible = (PageMode == "List");
        }
        #endregion
    }
}