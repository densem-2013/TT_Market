using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web;
using TT_Market.Core.Domains;

namespace TT_Market.Web.Models
{
    public class ApplicationDbInitializer : DropCreateDatabaseIfModelChanges<ApplicationDbContext>
    {
        protected override void Seed(ApplicationDbContext context)
        {
            InitializeIdentityForEF(context);
            base.Seed(context);
        }

        public static void InitializeIdentityForEF(ApplicationDbContext context)
        {
            context.PriceColumns.AddOrUpdate(p => p.ColumnName,
                new PriceTitleCell { OrderNumber = 0,ColumnName = "Описание", ColSpan = 1, RowSpan = 2 },
                new PriceTitleCell { OrderNumber = 9, ColumnName = "Спеццена грн", ColSpan = 1, RowSpan = 2 }
                );
        }
    }
}