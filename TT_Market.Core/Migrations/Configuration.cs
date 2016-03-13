using System;
using System.Data.Entity.Migrations;
using TT_Market.Core.Identity;

namespace TT_Market.Core.Migrations
{
    internal sealed class Configuration : DbMigrationsConfiguration<ApplicationDbContext>
    {
        private string _path = AppDomain.CurrentDomain.BaseDirectory.Replace(@"TT_Market.Core\bin\Debug", "") +
                               @"TT_Market.Core\DBinitial\Read";

        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(ApplicationDbContext context)
        {
            ApplicationDbInitializer.InitializeIdentityForEf(_path, context);
        }
    }
}
