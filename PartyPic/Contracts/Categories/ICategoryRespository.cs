using PartyPic.DTOs.Categories;
using PartyPic.Models.Categories;
using PartyPic.Models.Common;

namespace PartyPic.Contracts.Categories
{
    public interface ICategoryRespository
    {
        AllCategoriesResponse GetAllCategories();
        Category GetCategoryById(int id);
        Category CreateCategory(Category category);
        bool SaveChanges();
        void DeleteCategory(int id);
        Category UpdateCategory(int id, CategoryUpdateDTO category);
        void PartiallyUpdate(int id, CategoryUpdateDTO category);
        CategoryGrid GetAllCategoriesForGrid(GridRequest gridRequest);
    }
}
