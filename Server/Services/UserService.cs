using DentistryManagement.Server.Data;
using DentistryManagement.Server.Mappers;
using DentistryManagement.Server.Models;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using System;
using DentistryManagement.Server.DataTransferObjects;
using Microsoft.EntityFrameworkCore;
using DentistryManagement.Server.Services.Interfaces;
using DentistryManagement.Server.Helpers;

namespace DentistryManagement.Server.Services
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserProviderService _userProviderService;

        public UserService(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            IUserProviderService userProviderService
            )
        {
            _context = context;
            _userManager = userManager;
            _userProviderService = userProviderService;
        }

        public List<UserDTO> GetAll()
        {
            var users = _context.ApplicationUsers
                .Include(ur => ur.UserRoles)
                .ThenInclude(r => r.Role)
                .Include(a => a.Affiliate)
                .Select(x => UserMapper.ApplicationUserToDTO(x))
                .ToList();
            
            return users;
        }

        public UserDTO Get(string id)
        {
            var applicationUser = _context.ApplicationUsers
             .Include(ur => ur.UserRoles)
             .ThenInclude(r => r.Role)
             .Include(a => a.Affiliate)
             .SingleOrDefault(x => x.Id.Equals(id));

            if (applicationUser == null)
            {
                return null;
            }

            return UserMapper.ApplicationUserToDTO(applicationUser);
        }

        public UserDTO GetByUsername(string username)
        {
            var applicationUser = _context.ApplicationUsers
                .Include(ur => ur.UserRoles)
                .ThenInclude(r => r.Role)
                .Include(a => a.Affiliate)
                .SingleOrDefault(x => x.UserName.Equals(username));
           
            if (applicationUser == null)
            {
                return null;
            }

            return UserMapper.ApplicationUserToDTO(applicationUser);
        }

        public async Task CreateUser(UserDTO userDTO)
        {
            var applicationUser = UserMapper.DTOtoApplicationUser(userDTO);
          
            var adsa = await _userManager.CreateAsync(applicationUser, userDTO.Password);
            
            var role = _context.ApplicationRole.Find(userDTO.RoleId);

            applicationUser.UserRoles.Add(new ApplicationUserRole()
            {
                User = applicationUser,
                Role = role,
            });

            var affiliate = _context.Affiliates.Find(userDTO.AffiliateId);
            applicationUser.Affiliate = affiliate;

            _context.SaveChanges();
        }

        public async Task UpdateUser(UserDTO userDTO)
        {
            var applicationUser = _context.ApplicationUsers
                .Include(ur => ur.UserRoles)
                .Include(a => a.Affiliate)
                .SingleOrDefault(x => x.Id.Equals(userDTO.Id));

            if (userDTO.RoleId != null)
            {
                applicationUser.UserRoles.Clear();

                var role = _context.ApplicationRole.Find(userDTO.RoleId);

                applicationUser.UserRoles.Add(new ApplicationUserRole()
                {
                    User = applicationUser,
                    Role = role,
                });
            }

            if (userDTO.AffiliateId != 0)
            {
                var affiliate = _context.Affiliates.Find(userDTO.AffiliateId);
                applicationUser.Affiliate = affiliate;
            }

            applicationUser.FirstName = userDTO.FirstName;
            applicationUser.LastName = userDTO.LastName;
            applicationUser.PhoneNumber = userDTO.PhoneNumber;

           await _context.SaveChangesAsync();
        }

        public async Task UpdatePassword(UserDTO userDTO)
        {
            var applicationUser = await _userManager.FindByIdAsync(userDTO.Id);
            await _userManager.ChangePasswordAsync(applicationUser, userDTO.Password, userDTO.NewPassword);
        }

        public async Task DeleteUser(string id)
        {
            var applicationUser = _context.ApplicationUsers.Find(id);
            await _userManager.DeleteAsync(applicationUser);
        }

        public bool Exist(string id)
        {
            return _context.ApplicationUsers.Any(x => x.Id.Equals(id));
        }

        public async Task<bool> CheckPassword(string id, string password)
        {
            var applicationUser =  await _userManager.FindByIdAsync(id);
            return await _userManager.CheckPasswordAsync(applicationUser, password);
        }

        public List<UserDTO> GetAffiliateUsers(int affiliateId)
        {
            var users = _context.ApplicationUsers
                .Include(ur => ur.UserRoles)
                .ThenInclude(r => r.Role)
                .Where(u => u.AffiliateId.Equals(affiliateId))
                .Select(x => UserMapper.ApplicationUserToDTO(x))
                .ToList();

            return users;
        }

        public UserDTO GetCurrent()
        {
            var applicationUser = _context.ApplicationUsers
                 .Include(ur => ur.UserRoles)
                 .ThenInclude(r => r.Role)
                 .Include(a => a.Affiliate)
                 .SingleOrDefault(x => x.Id.Equals(_userProviderService.GetUserId()));

            if (applicationUser == null)
            {
                return null;
            }

            return UserMapper.ApplicationUserToDTO(applicationUser);
        }
    }
}
