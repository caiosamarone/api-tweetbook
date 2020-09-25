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
using TweetBook.Services;

namespace TweetBook.Controllers.V1
{
    public class PostController : Controller
    {
        private readonly IPostService _postService;

        public PostController(IPostService postService)
        {
            _postService = postService;
        }


        ///api/v1/posts
        [HttpGet(ApiRoutes.Posts.GetAll)]
        public IActionResult GetAll()
        {
            
            return Ok(_postService.GetPosts());
        }

        //api/v1/posts/{postId}
        [HttpGet(ApiRoutes.Posts.Get)]
        public IActionResult Get([FromRoute] Guid postId)
        {
            var post = _postService.GetPostById(postId);

            if(post == null)
            {
                return NotFound();
            }
            return Ok(post);
        }

        //api/v1/posts
        [HttpPost(ApiRoutes.Posts.Create)]
        public IActionResult Create([FromBody]CreatePostRequest postRequest)
        {

            var post = new Post { Id = postRequest.Id };
            if (post.Id != Guid.Empty)
            {
                //add um id aleatorio
                post.Id = Guid.NewGuid();
                _postService.GetPosts().Add(post);

                var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.ToUriComponent()}";
                var locationUri = baseUrl + "/" + ApiRoutes.Posts.Get.Replace("{postId}", post.Id.ToString());

                var response = new PostResponse { Id = post.Id};
                return Created(locationUri, response);
            }

            return NotFound();
           
        }

        //api/v1/posts
        [HttpPut(ApiRoutes.Posts.Update)]
        public IActionResult Update([FromRoute] Guid postId, [FromBody] UpdatePostRequest request)
        {
            var post = new Post
            {
                Id = postId,
                Name = request.Name
            };
            var updated = _postService.UpdatePost(post);
            if (!updated)
            {
                return NotFound();
            }
            return Ok(post);
        }
    }
}
