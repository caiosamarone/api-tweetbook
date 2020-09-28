using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using TweetBook.Contracts.V1;
using TweetBook.Contracts.V1.Requests;
using TweetBook.Contracts.V1.Responses;
using TweetBook.Domain;
using TweetBook.Extensions;
using TweetBook.Services;

namespace TweetBook.Controllers.V1
{
    //da o privilegio de acessar os endpoints se estiver autenticado - conexao com linha 33 da classe Installers/MvcInstaller   
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class PostController : Controller
    {
        private readonly IPostService _postService;

        public PostController(IPostService postService)
        {
            _postService = postService;
        }


        ///api/v1/posts
        [HttpGet(ApiRoutes.Posts.GetAll)]
        public async Task<IActionResult> GetAll()
        {
            
            return Ok(await _postService.GetPosts());
        }

        //api/v1/posts/{postId}
        [HttpGet(ApiRoutes.Posts.Get)]
        public async Task<IActionResult> Get([FromRoute] Guid postId)
        {
            var post =  await _postService.GetPostById(postId);

            if(post == null)
            {
                return NotFound();
            }
            return Ok(post);
        }

        //api/v1/posts
        [HttpPost(ApiRoutes.Posts.Create)]
        public async Task<IActionResult> Create([FromBody]CreatePostRequest postRequest)
        {
            
            var post = new Post 
            { 
                Name = postRequest.Name,
                UserId = HttpContext.GetUserId()   //HttpContext vem da classe do controller
            };
           
            await _postService.CreatePost(post);

            var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.ToUriComponent()}";
            var locationUri = baseUrl + "/" + ApiRoutes.Posts.Get.Replace("{postId}", post.Id.ToString());

            var response = new PostResponse { Id = post.Id};    
            return Created(locationUri, response);
                      
           
        }

        //api/v1/posts{postId}
        [HttpPut(ApiRoutes.Posts.Update)]
        public async Task<IActionResult> Update([FromRoute] Guid postId, [FromBody] UpdatePostRequest request)
        {
            var userOwnsPost = await _postService.UserOwnsPost(postId, HttpContext.GetUserId());    

            if(!userOwnsPost)
            {
                return BadRequest(new
                {
                    error = "Você não tem permissão para atualizar este post."
                });
            }

            var post = await _postService.GetPostById(postId);
            //atualizando o nome que veio do Body da request 
            post.Name = request.Name;

            var updated = await _postService.UpdatePost(post);
            if (!updated)
            {
                return NotFound();
            }
            return Ok(post);
        }

        //api/v1/posts{postId}
        [HttpDelete(ApiRoutes.Posts.Delete)]
        public async Task<IActionResult> Delete([FromRoute] Guid postId)
        {
            var userOwnsPost = await _postService.UserOwnsPost(postId, HttpContext.GetUserId());

            if (!userOwnsPost)
            {
                return BadRequest(new
                {
                    error = "Você não tem permissão para deletar este post."
                });
            }

            var deleted = await _postService.DeletePost(postId);
            if(!deleted)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}
