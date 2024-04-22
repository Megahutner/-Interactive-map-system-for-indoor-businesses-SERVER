using Microsoft.AspNetCore.Mvc;
using sl_APIs.Controller.Controller;
using UWEServer.Data;
using UWEServer.Entities;
using UWEServer.Responses;
using UWEServer.Requests;
using Serilog;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using UWEServer.Model;
using Newtonsoft.Json;
using UWEServer.Services;
using Microsoft.EntityFrameworkCore;
namespace UWEServer.Controllers
{
    [Route("api/category")]
    [ApiController]
    public class CategoryController : APIBaseController
    {
        #region Private variables
        private readonly DbApiContext _dbApiContext;
        #endregion

        #region Public variables

        #endregion

        #region Contructor
        public CategoryController(DbApiContext dbApiContext)
        {
            _dbApiContext = dbApiContext;
        }



        [Route("create")]
        [HttpPost]
        public ApiJsonResult CreateCategory([FromBody] CategoryTransfer categoryCreate)
        {
            try
            {
                if (string.IsNullOrEmpty(categoryCreate.Name))
                {
                    return new ApiJsonResult()
                    {
                        code = 422,
                        message = "Category name is null"
                    };
                }
              
                //Check duplicate name
                var categoryCheck = _dbApiContext.Categories.FirstOrDefault(x => x.Name == categoryCreate.Name && !x.DelFlag);
                if (categoryCheck != null)
                {
                    return new ApiJsonResult()
                    {
                        code = 424,
                        message = "Category already exists"
                    };
                }
                var newCategory = new Category()
                {
                    CreatedTime = DateTime.Now,
                    UpdatedTime = DateTime.Now,
                    DelFlag = false,
                    Name = categoryCreate.Name,
                };
                _dbApiContext.Add(newCategory);
                _dbApiContext.SaveChanges();
                return new ApiJsonResult()
                {
                    code = 200,
                    message = "Ok",
                    data = new CategoryResponse()
                    {
                        Id = newCategory.Id,
                        Name = newCategory.Name,
                        CreatedTime = newCategory.CreatedTime,
                        UpdatedTime = newCategory.UpdatedTime

                    }
                };
            }
            catch (Exception e)
            {
                Log.Error(e.ToString(), "CreateCategory fail");
                return APIException(e);
            }

        }


        /// <summary>
        /// Get all categories
        /// </summary>
        /// <returns></returns>
        [Route("get")]
        [HttpGet]
        public ApiJsonResult GetCategories([FromQuery] PaginationFilter queryData)
        {
            {
                try
                {
                    var categoryQuery = _dbApiContext.Categories.Where(m => !m.DelFlag);
                    var route = Request.Path.Value;
                    var filter = PaginationHelperResponse.GetDataFilter(queryData.Filter);
                    var sorts = string.IsNullOrEmpty(queryData.Sort) ? new List<DataSort>() :
                        JsonConvert.DeserializeObject<List<DataSort>>(queryData.Sort);
                    categoryQuery = PaginationHelperResponse.QueryInTable<Category>(categoryQuery, filter, sorts);
                    categoryQuery = categoryQuery.OrderBy(m => m.Id);
                    var pagedResponse = PaginationHelperResponse.CreatePagedReponse<Category>(categoryQuery, queryData, 0, route);
                    return new ApiJsonResult()
                    {
                        code = 200,
                        message = "Ok",
                        data = new
                        {
                            category = pagedResponse.Data.Select(m => ConvertService.ConvertCategoryToBasicData(m)).ToList(),
                            meta = new
                            {
                                pagedResponse.PageNumber,
                                pagedResponse.PageSize,
                                pagedResponse.TotalPages,
                                pagedResponse.TotalRecords,
                                pagedResponse.Succeeded,
                                pagedResponse.Errors,
                                pagedResponse.Message,

                            }
                        }
                    };
                }
                catch (Exception e)
                {
                    Log.Error(e.ToString(), "GetCategories failed");
                    return APIException(e);
                }
            }

        }

        /// <summary>
        /// Get all categories
        /// </summary>
        /// <returns></returns>
        [Route("enum")]
        [HttpGet]
        public ApiJsonResult GetCategoryEnum()
        {
            {
                try
                {
                    var listCategories = _dbApiContext.Categories.Where( m=> m.DelFlag == false).ToList().Select(m =>
                    new { id = m.Id, name = m.Name }).ToList();
                    return new ApiJsonResult()
                    {
                        code = 200,
                        message = "OK",
                        data = listCategories
                    };
                }
                catch (Exception e)
                {
                    Log.Error(e.ToString(), "GetCategories failed");
                    return APIException(e);
                }
            }

        }


        /// <summary>
        /// Delete Category
        /// </summary>
        /// <returns></returns>
        [Route("delete")]
        [HttpDelete]
        public ApiJsonResult DeleteCategory(int id)
        {
            try
            {
                var category = _dbApiContext.Categories.FirstOrDefault(m => m.Id == id && !m.DelFlag);
                if (category == null)
                {
                    return new ApiJsonResult()
                    {
                        code = 400,
                        message = " Category not found"
                    };
                }
                category.DelFlag = true;
                category.UpdatedTime = DateTime.Now;
                _dbApiContext.Update(category);
                _dbApiContext.SaveChanges();
                return new ApiJsonResult()
                {
                    code = 200,
                    message = "Category deleted successfully"
                };
            }
            catch (Exception e)
            {
                Log.Error(e.ToString(), "DeleteCategory failed");
                return APIException(e);
            }
        }
        #endregion
    }

}
