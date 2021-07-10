using AutoMapper;
using PartyPic.DTOs.Categories;
using PartyPic.Helpers;
using PartyPic.Models.Categories;
using PartyPic.Models.Common;
using PartyPic.Models.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PartyPic.Contracts.Categories
{
    public class SqlCategoryRepository : ICategoryRespository
    {
        private readonly CategoryContext _categoryContext;
        private readonly IMapper _mapper;

        public SqlCategoryRepository(CategoryContext categoryContext, IMapper mapper)
        {
            _categoryContext = categoryContext;
            _mapper = mapper;
        }

        public Category CreateCategory(Category category)
        {
            this.ThrowExceptionIfArgumentIsNull(category);
            this.ThrowExceptionIfPropertyAlreadyExists(category, true, 0);

            category.CreatedDatetime = DateTime.Now;

            _categoryContext.Categories.Add(category);

            this.SaveChanges();

            var addedCategory = _categoryContext.Categories.OrderByDescending(u => u.CreatedDatetime).FirstOrDefault();

            return addedCategory;
        }

        public void DeleteCategory(int id)
        {
            var category = this.GetCategoryById(id);

            if (category == null)
            {
                throw new NotCategoryFoundException();
            }

            _categoryContext.Categories.Remove(category);

            this.SaveChanges();
        }

        public AllCategoriesResponse GetAllCategories()
        {
            return new AllCategoriesResponse
            {
                Categories = _categoryContext.Categories.ToList()
            };
        }

        public CategoryGrid GetAllCategoriesForGrid(GridRequest gridRequest)
        {
            var categoryRows = new List<Category>();

            categoryRows = _categoryContext.Categories.ToList();

            if (!string.IsNullOrEmpty(gridRequest.SearchPhrase))
            {
                categoryRows = _categoryContext.Categories.Where(cat => cat.Description.Contains(gridRequest.SearchPhrase)).ToList();
            }

            if (gridRequest.RowCount != -1 && _categoryContext.Categories.Count() > gridRequest.RowCount && gridRequest.Current > 0 && categoryRows.Count > 0)
            {
                var offset = gridRequest.RowCount;
                var index = (gridRequest.Current - 1) * gridRequest.RowCount;

                if ((categoryRows.Count % gridRequest.RowCount) != 0 && (categoryRows.Count / gridRequest.RowCount) < gridRequest.Current)
                {
                    offset = categoryRows.Count % gridRequest.RowCount;
                }

                categoryRows = categoryRows.GetRange(index, offset);
            }

            if (!string.IsNullOrEmpty(gridRequest.SortBy) && !string.IsNullOrEmpty(gridRequest.OrderBy))
            {
                gridRequest.SortBy = WordingHelper.FirstCharToUpper(gridRequest.SortBy);

                categoryRows = categoryRows
                                .OrderBy(m => m.GetType()
                                                .GetProperties()
                                                .First(n => n.Name == gridRequest.SortBy)
                                .GetValue(m, null))
                                .ToList();

                if (gridRequest.OrderBy.ToLowerInvariant() == "desc")
                {
                    categoryRows.Reverse();
                }
            }

            var categoriesGrid = new CategoryGrid
            {
                Rows = categoryRows,
                Total = _categoryContext.Categories.Count(),
                Current = gridRequest.Current,
                RowCount = gridRequest.RowCount
            };

            return categoriesGrid;
        }

        public Category GetCategoryById(int id)
        {
            var category = _categoryContext.Categories.FirstOrDefault(cat => cat.CategoryId == id);

            if (category == null)
            {
                throw new NotCategoryFoundException();
            }

            return category;
        }

        public void PartiallyUpdate(int id, CategoryUpdateDTO category)
        {
            this.UpdateCategory(id, category);

            this.SaveChanges();
        }

        public bool SaveChanges()
        {
            return (_categoryContext.SaveChanges() >= 0);
        }

        public Category UpdateCategory(int id, CategoryUpdateDTO categoryUpdateDto)
        {
            var category = _mapper.Map<Category>(categoryUpdateDto);

            var retrievedCategory = this.GetCategoryById(id);

            if (retrievedCategory == null)
            {
                throw new NotCategoryFoundException();
            }

            this.ThrowExceptionIfArgumentIsNull(category);
            this.ThrowExceptionIfPropertyIsIncorrect(category, false, id);

            _mapper.Map(categoryUpdateDto, retrievedCategory);

            _categoryContext.Categories.Update(retrievedCategory);

            this.SaveChanges();

            return this.GetCategoryById(id);
        }

        private void ThrowExceptionIfPropertyIsIncorrect(Category category, bool isNew, int id)
        {
            if (_categoryContext.Categories.ToList().Any(cat => cat.Description == category.Description))
            {
                throw new PropertyIncorrectException();
            }
        }

        private void ThrowExceptionIfPropertyAlreadyExists(Category category, bool isNew, int id)
        {
            if (!isNew)
            {
                if (category.CreatedDatetime != _categoryContext.Categories.FirstOrDefault(e => e.CategoryId == id).CreatedDatetime)
                {
                    throw new PropertyIncorrectException();
                }
            }

            if (_categoryContext.Categories.ToList().Any(cat => cat.Description == category.Description))
            {
                throw new PropertyIncorrectException();
            }
        }

        private void ThrowExceptionIfArgumentIsNull(Category category)
        {
            if (category == null)
            {
                throw new ArgumentNullException(nameof(category));
            }

            if (string.IsNullOrEmpty(category.Description))
            {
                throw new ArgumentNullException(nameof(category.Description));
            }
        }
    }
}
