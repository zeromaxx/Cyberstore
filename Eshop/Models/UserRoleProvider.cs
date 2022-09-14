using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Security;

namespace Eshop.Models
{
    

    public class UserRoleProvider : RoleProvider
    {
        public override string ApplicationName
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public override void CreateRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            throw new NotImplementedException();
        }

        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            throw new NotImplementedException();
        }

        public override string[] GetAllRoles()
        {
            using (Entities _Context = new Entities())
            {
                return _Context.Roles.Select(r => r.RoleName).ToArray();
            }
        }

        public override string[] GetRolesForUser(string username)
        {
            using (Entities _Context = new Entities())
            {
                
                var userRoles = (from user in _Context.Users
                                 join roleMapping in _Context.UserRoleMappings
                                 on user.UserId equals roleMapping.UserId
                                 join role in _Context.Roles
                                 on roleMapping.RoleId equals role.Id
                                 where user.Email == username
                                 select role.RoleName).ToArray();
                return userRoles.ToArray() ;
            }
        }

        public override string[] GetUsersInRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override bool IsUserInRole(string username, string roleName)
        {

            using (Entities _Context = new Entities())
            {
                var user = _Context.Users.SingleOrDefault(u => u.Username == username);
                var userRoles = _Context.Roles.Select(r => r.RoleName);
                if (user == null)
                    return false;
                foreach (var role in GetRolesForUser(user.Username))
                {
                    if (role == roleName)
                    {
                        return true;
                    }
                }
            }
            
            return false;
        }

        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public override bool RoleExists(string roleName)
        {
            throw new NotImplementedException();
        }
    }
}