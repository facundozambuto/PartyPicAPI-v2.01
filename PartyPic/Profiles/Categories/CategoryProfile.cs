using AutoMapper;
using PartyPic.DTOs.Categories;
using PartyPic.Models.Categories;

namespace PartyPic.Profiles.Categories
{
    public class CategoryProfile : Profile
    {
        public CategoryProfile()
        {
            CreateMap<Category, CategoryReadDTO>();
            CreateMap<CategoryCreateDTO, Category>();
            CreateMap<CategoryGrid, CategoryReadDTOGrid>();
            CreateMap<CategoryReadDTOGrid, CategoryGrid>();
            CreateMap<CategoryUpdateDTO, Category>();
            CreateMap<Category, CategoryUpdateDTO>();
            CreateMap<AllCategoriesResponse, GetAllCategoriesApiResponse>();
            CreateMap<GetAllCategoriesApiResponse, AllCategoriesResponse>();
            CreateMap<AllCategoriesResponse, GetAllCategoriesApiResponse>();
            CreateMap<GetAllCategoriesApiResponse, AllCategoriesResponse>();
            CreateMap<CategoryGrid, GridCategoryApiResponse>();
            CreateMap<GridCategoryApiResponse, CategoryGrid>();
            CreateMap<CategoryApiResponse, Category>();
            CreateMap<Category, CategoryApiResponse>();
        }
    }
}
