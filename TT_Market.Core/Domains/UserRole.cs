using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.EntityFramework;

namespace TT_Market.Core.Domains
{
    public partial class UserRole : IdentityRole
    {
        public UserRole() : base() { }
        public UserRole(string name) : base(name) { }
        public UserRole(string name, string descr)
            : base(name)
        {
            Description = descr;
        }
        public string Description { get; set; }

    }
}
