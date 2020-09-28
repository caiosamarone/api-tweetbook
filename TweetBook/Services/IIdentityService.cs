using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TweetBook.Domain;

namespace TweetBook.Services
{
    public interface IIdentityService
    {
        Task<AuthenticationResult> Register(string email, string password);
        Task<AuthenticationResult> Login(string email, string password);

    }
}
    