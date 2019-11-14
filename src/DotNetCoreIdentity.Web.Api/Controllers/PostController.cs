using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using DotNetCoreIdentity.Application;
using DotNetCoreIdentity.Application.BlogServices;
using DotNetCoreIdentity.Application.BlogServices.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DotNetCoreIdentity.Web.Api.Controllers
{
    [Route("api/[controller]")]

    [ApiController]
    public class PostController : ControllerBase
    {
         private readonly IPostService _postService;
         public PostController(IPostService postService)
        {
            _postService = postService;
        }

         // GET: api/Category
        [HttpGet]
        public async Task<ApplicationResult<List<PostDto>>> Get()
        {
            return await _postService.GetAll();
        }
         // GET: api/Category/5
        [HttpGet("{id}", Name = "GetPost")]
        public async Task<ActionResult<ApplicationResult<PostDto>>> Get(Guid id)
        {
            var result = await _postService.Get(id);
            if (result.Succeeded)
                return result;
            return NotFound(result);
        }

        // POST: api/Category
        [HttpPost]
        public async Task<ActionResult<ApplicationResult<PostDto>>> Post([FromBody] CreatePostInput input)
        {
            var result = await _postService.Create(input);
            if (result.Succeeded)
                return result;
            return NotFound(result);
        }

        // PUT: api/Category/5
        [HttpPut("{id}")]
        public async Task<ActionResult<ApplicationResult<PostDto>>> Put(Guid id, [FromBody] UpdatePostInput input)
        {
            if (ModelState.IsValid)
            {
                var getService = await _postService.Get(id);
                input.CreatedById = getService.Result.CreatedById;
                input.CreatedBy = getService.Result.CreatedBy;

                input.Id = getService.Result.Id;
                input.ModifiedById = User.FindFirst(ClaimTypes.NameIdentifier).Value;
                input.ModifiedBy = User.FindFirst(ClaimTypes.Name).Value;

                var updateService = await _postService.Update(input);
                if (updateService.Succeeded)
                {
                    return Ok(updateService);
                }
                ModelState.AddModelError("Error", updateService.ErrorMessage);
            }
            return BadRequest(new ApplicationResult<PostDto>
            {
                Result = new PostDto(),
                Succeeded = false,
                ErrorMessage = string.Join(", ", ModelState.Values)
            });
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApplicationResult>> Delete(Guid id)
        {
            var deleteService = await _postService.Delete(id);
            if (deleteService.Succeeded)
            {
                return Ok(deleteService);
            }
            return BadRequest(deleteService);
        }

        //TODO: upload image api
    }
}