using TT_Market.Web.Models;

namespace TT_Market.Web.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<TT_Market.Web.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(TT_Market.Web.Models.ApplicationDbContext context)
        {
            ApplicationDbInitializer.InitializeIdentityForEF(context);
        }
    }
}
