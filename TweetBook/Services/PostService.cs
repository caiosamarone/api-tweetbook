using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using TweetBook.Data;
using TweetBook.Domain;

namespace TweetBook.Services
{
    public class PostService : IPostService
    {
        private readonly DataContext _dataContext;

        public PostService(DataContext dataContext)
        {
            _dataContext = dataContext;
        }
        public async Task<List<Post>> GetPosts()
        {
            return await _dataContext.Posts.ToListAsync();
        }

        public async Task<Post> GetPostById(Guid postId)
        {
            return await _dataContext.Posts.SingleOrDefaultAsync(x => x.Id == postId);
        }

        public async Task<bool> UpdatePost(Post postToUpdate)
        {
             _dataContext.Posts.Update(postToUpdate);
            var updated = await _dataContext.SaveChangesAsync();
            //se tiver algo para dar update, retorna true
            return updated > 0;            
        }   

        public async Task<bool> DeletePost(Guid postId)
        {   
            var post = await GetPostById(postId);

            if (post == null) return false;

             _dataContext.Remove(post);
            var deleted = await _dataContext.SaveChangesAsync();
            return deleted > 0;

        }

        public async Task<bool> CreatePost(Post post)
        {
            await _dataContext.Posts.AddAsync(post);
            var created = await _dataContext.SaveChangesAsync();
            return created > 0;
            
        }
    }
}
