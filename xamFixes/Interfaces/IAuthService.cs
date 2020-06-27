using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using xamFixes.Models;

namespace xamFixes.Interfaces
{
    public interface IAuthService
    {

        Task<bool> LoginAsync(UserLogin credentials);
        Task<bool> RegisterAsync(UserRegister credentials);

    }
}
