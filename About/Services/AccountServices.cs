using About.Controllers;
using About.Data;
using About.Interface;
using About.ViewModel;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace About.Services
{
    public class AccountServices
    {

        private readonly AccountContext _ctx;
        private readonly IHashService _hashService;

        public AccountServices(AccountContext context, IHashService hashService)
        {
            _ctx = context;
            _hashService = hashService;
        }

        public async Task<ApplicationUser> AuthenticateUser(LoginViewModel loginVM)
        {
            //find user
            // _hashService.MD5Hash(loginVM.Password)
            var user = await _ctx.Users
                .FirstOrDefaultAsync(u => u.Email.ToUpper() == loginVM.Email && u.Password == _hashService.MD5Hash( loginVM.Password));

            if (user != null)
            {
                //讀取第一個Role
                var roleName = await _ctx.Users
                                .Where(u => u.Email == loginVM.Email)
                                .SelectMany(u => u.UserRoles)
                                .Select(ur => ur.Role.Name)
                                .FirstOrDefaultAsync();

                //讀取所有Role角色
                List<string> roleNames = await _ctx.Users
                        .Where(u => u.Email == loginVM.Email)
                        .SelectMany(u => u.UserRoles)
                        .Select(ur => ur.Role.Name)
                        .ToListAsync();

                var userInfo = new ApplicationUser
                {
                    Name = user.Name,
                    Email = user.Email,
                    Role = roleName ?? "",
                    Roles = roleNames.ToArray()
                };

                return userInfo;
            }
            else
            {
                return null;
            }
        }
    }
}
