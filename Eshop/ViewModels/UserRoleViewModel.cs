using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Eshop.ViewModels
{
    public class UserRoleViewModel
    {
        public IEnumerable<User> Users { get; set; }
        public IEnumerable<Role> Roles { get; set; }
        public IEnumerable<UserRoleMapping> UserRoleMappings { get; set; }
        public User User { get; set; }

        public virtual Role Role { get; set; }
        
    }
}