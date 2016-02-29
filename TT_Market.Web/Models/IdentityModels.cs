﻿using System.Data.Entity;
using Microsoft.AspNet.Identity.EntityFramework;
using TT_Market.Core.Domains;
using TT_Market.Web.Migrations;

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
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<ApplicationDbContext, Configuration>());
        }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //one-to-many 
            //modelBuilder.Entity<PriceList>()
            //            .HasOptional<Agent>(s => s.Agent)
            //            .WithMany(s => s.PriceLists)
            //            .HasForeignKey(s => s.AgentId);

            base.OnModelCreating(modelBuilder);

        }
        public virtual DbSet<Brand> Brands { get; set; }
        public virtual DbSet<PriceLanguage> PriceLanguages { get; set; }
        public virtual DbSet<PriceList> PriceLists { get; set; }
        public virtual DbSet<Agent> Agents { get; set; }
        public virtual DbSet<AutoType> AutoTypes { get; set; } 
        public virtual DbSet<ConvSign> ConvSigns { get; set; }
        public virtual DbSet<Country> Countrys { get; set; }
        public virtual DbSet<Currency> Currencys { get; set; }
        public virtual DbSet<Diameter> Diameters { get; set; }
        public virtual DbSet<Height> Heights { get; set; }
        public virtual DbSet<HomolAttribute> Homols { get; set; }
        public virtual DbSet<Model> Models { get; set; }
        public virtual DbSet<MPT> Mpts { get; set; }
        public virtual DbSet<PressIndex> PressIndixs { get; set; }
        public virtual DbSet<Season> Seasons { get; set; }
        public virtual DbSet<SpeedIndex> SpeedIndexs { get; set; }
        public virtual DbSet<StockCity> Citys { get; set; }
        public virtual DbSet<Width> Widths { get; set; }
    }
}