using AutoMapper;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using PartyPic.Contracts.Categories;
using PartyPic.DTOs.Categories;
using PartyPic.Models.Categories;
using PartyPic.Models.Common;
using PartyPic.Models.Exceptions;

namespace PartyPic.Controllers
{
    [EnableCors("CorsApi")]
    [Route("api/categories")]
    [ApiController]
    public class CategoryController : PartyPicControllerBase
    {
        private readonly ICategoryRespository _categoryRepository;
        private readonly IMapper _mapper;
        private readonly Contracts.Logger.ILoggerManager _logger;

        public CategoryController(ICategoryRespository categoryRepository, IMapper mapper, IConfiguration config, Contracts.Logger.ILoggerManager logger) : base(mapper, config, logger)
        {
            _categoryRepository = categoryRepository;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpGet]
        public ActionResult GetAllCategories()
        {
            return ExecuteMethod<CategoryController, GetAllCategoriesApiResponse, AllCategoriesResponse>(() => _categoryRepository.GetAllCategories());
        }

        [HttpGet]
        [Route("~/api/categories/grid")]
        public ActionResult<CategoryGrid> GetAllCategoriesForGrid([FromQuery] int current, [FromQuery] int rowCount, [FromQuery] string searchPhrase, [FromQuery] string sortBy, string orderBy)
        {
            GridRequest gridRequest = new GridRequest
            {
                Current = current,
                RowCount = rowCount,
                SearchPhrase = searchPhrase,
                SortBy = sortBy,
                OrderBy = orderBy
            };

            return ExecuteMethod<CategoryController, GridCategoryApiResponse, CategoryGrid>(() => _categoryRepository.GetAllCategoriesForGrid(gridRequest));
        }

        [HttpGet("{id}", Name = "GetCategoryById")]
        public ActionResult<Category> GetCategoryById(int id)
        {
            return ExecuteMethod<CategoryController, CategoryApiResponse, Category>(() => _categoryRepository.GetCategoryById(id));
        }

        [HttpPost]
        public ActionResult<Category> CreateCategory(CategoryCreateDTO categoryCreateDTO)
        {
            var categoryModel = _mapper.Map<Category>(categoryCreateDTO);

            return ExecuteMethod<CategoryController, CategoryApiResponse, Category>(() => _categoryRepository.CreateCategory(categoryModel));
        }

        [HttpPut("{id}")]
        public ActionResult UpdateCategory(int id, CategoryUpdateDTO categoryUpdateDto)
        {
            return ExecuteMethod<CategoryController, CategoryApiResponse, Category>(() => _categoryRepository.UpdateCategory(id, categoryUpdateDto));
        }

        [HttpPatch("{id}")]
        public ActionResult PartialCategoryUpdate(int id, JsonPatchDocument<CategoryUpdateDTO> patchDoc)
        {
            var categoryModelFromRepo = _categoryRepository.GetCategoryById(id);
            if (categoryModelFromRepo == null)
            {
                return ExecuteMethod<CategoryController>(() => new NotCategoryFoundException());
            }

            var categoryToPatch = _mapper.Map<CategoryUpdateDTO>(categoryModelFromRepo);
            patchDoc.ApplyTo(categoryToPatch);

            if (!TryValidateModel(categoryToPatch))
            {
                return ValidationProblem(ModelState);
            }

            _mapper.Map(categoryToPatch, categoryModelFromRepo);

            return ExecuteMethod<CategoryController>(() => _categoryRepository.PartiallyUpdate(id, categoryToPatch));
        }

        [HttpDelete("{id}")]
        public ActionResult DeleteCategory(int id)
        {
            return ExecuteMethod<CategoryController>(() => _categoryRepository.DeleteCategory(id));
        }
    }
}
