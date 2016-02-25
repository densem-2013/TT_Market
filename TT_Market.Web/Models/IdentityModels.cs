using System.Data.Entity;
using Microsoft.AspNet.Identity.EntityFramework;
using TT_Market.Core.Domains;

namespace TT_Market.Web.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection")
        {
        }
        static ApplicationDbContext()
        {
            Database.SetInitializer<ApplicationDbContext>(new ApplicationDbInitializer());
        }

        public virtual DbSet<ReadSetting> ReadSettinges { get; set; }
        public virtual DbSet<PriceTitleCell> PriceColumns { get; set; }
        public virtual DbSet<PriceLanguage> PriceLanguages { get; set; }
        public virtual DbSet<PriceList> PriceLists { get; set; }
        public virtual DbSet<Provider> Providers { get; set; }
        public virtual DbSet<CellValue> ColumnValues { get; set; }
        public virtual DbSet<ChildCellColumnName> ChildColumnNames { get; set; }
        public virtual DbSet<WorkSheet> WorkSheets { get; set; }
    }
}