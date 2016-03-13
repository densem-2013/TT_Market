using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Claims;
using System.Threading.Tasks;

namespace TT_Market.Core.Domains
{

    public partial class User : IdentityUser
    {
        [Required]
        public virtual string FirstName { get; set; }
        [Required]
        public virtual string LastName { get; set; }
        [Required]
        [DataType(DataType.DateTime)]
        public virtual DateTime LastLoginTime { get; set; }
        [Required]
        [DataType(DataType.DateTime)]
        public virtual DateTime RegistrationDate { get; set; }

        [JsonIgnore]
        public virtual Agent Agent { get; set; }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<User, string> manager)
        {
            // Обратите внимание, что authenticationType должен совпадать с типом, определенным в CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Здесь добавьте утверждения пользователя
            return userIdentity;
        }
    }
}
