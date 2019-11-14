using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using DotNetCoreIdentity.Application;
using DotNetCoreIdentity.Application.CategoryServices.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DotNetCoreIdentity.Web.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }
        // GET: api/Category
        [HttpGet]
        public async Task<ApplicationResult<List<CategoryDto>>> Get()
        {
            return await _categoryService.GetAll();
        }

        // GET: api/Category/5
        [HttpGet("{id}", Name = "Get")]
        public async Task<ActionResult<ApplicationResult<CategoryDto>>> Get(int id)
        {
            var result = await _categoryService.Get(id);
            if (result.Succeeded)
                return result;
            return NotFound(result);
        }

        // POST: api/Category
        [HttpPost]
        public async Task<ActionResult<ApplicationResult<CategoryDto>>> Post([FromBody] CreateCategoryInput input)
        {
            var result = await _categoryService.Create(input);
            if (result.Succeeded)
                return result;
            return NotFound(result);
        }

        // PUT: api/Category/5
        [HttpPut("{id}")]
        public async Task<ActionResult<ApplicationResult<CategoryDto>>> Put(int id, [FromBody] UpdateCategoryInput input)
        {
            if (ModelState.IsValid)
            {
                var getService = await _categoryService.Get(id);
                input.CreatedById = getService.Result.CreatedById;
                input.CreatedBy = getService.Result.CreatedBy;

                input.Id = getService.Result.Id;
                input.ModifiedById = User.FindFirst(ClaimTypes.NameIdentifier).Value;
                input.ModifiedBy = User.FindFirst(ClaimTypes.Name).Value;

                var updateService = await _categoryService.Update(input);
                if (updateService.Succeeded)
                {
                    return Ok(updateService);
                }
                ModelState.AddModelError("Error", updateService.ErrorMessage);
            }
            return BadRequest(new ApplicationResult<CategoryDto>
            {
                Result = new CategoryDto(),
                Succeeded = false,
                ErrorMessage = string.Join(", ", ModelState.Values)
            });
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApplicationResult>> Delete(int id)
        {
            var deleteService = await _categoryService.Delete(id);
            if (deleteService.Succeeded)
            {
                return Ok(deleteService);
            }
            return BadRequest(deleteService);
        }
    }
}
