namespace TT_Market.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Agents",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AgentTitle = c.String(),
                        City = c.String(),
                        Phone = c.String(),
                        Email = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.PriceReadSettings",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FileName = c.String(),
                        TransformMask = c.String(),
                        Agent_Id = c.Int(),
                        PriceLanguage_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Agents", t => t.Agent_Id)
                .ForeignKey("dbo.PriceLanguages", t => t.PriceLanguage_Id)
                .Index(t => t.Agent_Id)
                .Index(t => t.PriceLanguage_Id);
            
            CreateTable(
                "dbo.PriceLanguages",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        LanguageName = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Prices",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DownLoadDate = c.DateTime(nullable: false),
                        InsertDate = c.DateTime(nullable: false),
                        PriceReadSetting_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.PriceReadSettings", t => t.PriceReadSetting_Id)
                .Index(t => t.PriceReadSetting_Id);
            
            CreateTable(
                "dbo.MPTs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TireTitle = c.String(),
                        ExtendedData = c.String(),
                        Stock = c.Int(nullable: false),
                        RegularPrice = c.Double(nullable: false),
                        DiscountPrice = c.Double(nullable: false),
                        SpecialPrice = c.Double(nullable: false),
                        AutoType_Id = c.Int(),
                        Country_Id = c.Int(),
                        Model_Id = c.Int(),
                        Brand_Id = c.Int(),
                        ConvSign_Id = c.Int(),
                        Currency_Id = c.Int(),
                        Diameter_Id = c.Int(),
                        Height_Id = c.Int(),
                        Homol_Id = c.Int(),
                        PressIndex_Id = c.Int(),
                        Price_Id = c.Int(),
                        Season_Id = c.Int(),
                        SpeedIndex_Id = c.Int(),
                        StockCity_Id = c.Int(),
                        Width_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AutoTypes", t => t.AutoType_Id)
                .ForeignKey("dbo.Countries", t => t.Country_Id)
                .ForeignKey("dbo.Models", t => t.Model_Id)
                .ForeignKey("dbo.Brands", t => t.Brand_Id)
                .ForeignKey("dbo.ConvSigns", t => t.ConvSign_Id)
                .ForeignKey("dbo.Currencies", t => t.Currency_Id)
                .ForeignKey("dbo.Diameters", t => t.Diameter_Id)
                .ForeignKey("dbo.Heights", t => t.Height_Id)
                .ForeignKey("dbo.HomolAttributes", t => t.Homol_Id)
                .ForeignKey("dbo.PressIndexes", t => t.PressIndex_Id)
                .ForeignKey("dbo.Prices", t => t.Price_Id)
                .ForeignKey("dbo.Seasons", t => t.Season_Id)
                .ForeignKey("dbo.SpeedIndexes", t => t.SpeedIndex_Id)
                .ForeignKey("dbo.StockCities", t => t.StockCity_Id)
                .ForeignKey("dbo.Widths", t => t.Width_Id)
                .Index(t => t.AutoType_Id)
                .Index(t => t.Country_Id)
                .Index(t => t.Model_Id)
                .Index(t => t.Brand_Id)
                .Index(t => t.ConvSign_Id)
                .Index(t => t.Currency_Id)
                .Index(t => t.Diameter_Id)
                .Index(t => t.Height_Id)
                .Index(t => t.Homol_Id)
                .Index(t => t.PressIndex_Id)
                .Index(t => t.Price_Id)
                .Index(t => t.Season_Id)
                .Index(t => t.SpeedIndex_Id)
                .Index(t => t.StockCity_Id)
                .Index(t => t.Width_Id);
            
            CreateTable(
                "dbo.AutoTypes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TypeValue = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Brands",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        BrandTitle = c.String(),
                        Country_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Countries", t => t.Country_Id)
                .Index(t => t.Country_Id);
            
            CreateTable(
                "dbo.Countries",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CountryTitle = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Models",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ModelTitle = c.String(),
                        AgentId = c.Int(nullable: false),
                        Brand_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Brands", t => t.Brand_Id)
                .Index(t => t.Brand_Id);
            
            CreateTable(
                "dbo.ConvSigns",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SignValue = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Currencies",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CurrencyTitle = c.String(),
                        CurrentExchangeRate = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Diameters",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DSize = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Heights",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Value = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.HomolAttributes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        HomolTitle = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.PressIndexes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Value = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Seasons",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SeasonTitle = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.SpeedIndexes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Value = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.StockCities",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CityTitle = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Widths",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Value = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        UserName = c.String(),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        Discriminator = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                        User_Id = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.User_Id, cascadeDelete: true)
                .Index(t => t.User_Id);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.LoginProvider, t.ProviderKey })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.RoleId)
                .Index(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUserClaims", "User_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Prices", "PriceReadSetting_Id", "dbo.PriceReadSettings");
            DropForeignKey("dbo.MPTs", "Width_Id", "dbo.Widths");
            DropForeignKey("dbo.MPTs", "StockCity_Id", "dbo.StockCities");
            DropForeignKey("dbo.MPTs", "SpeedIndex_Id", "dbo.SpeedIndexes");
            DropForeignKey("dbo.MPTs", "Season_Id", "dbo.Seasons");
            DropForeignKey("dbo.MPTs", "Price_Id", "dbo.Prices");
            DropForeignKey("dbo.MPTs", "PressIndex_Id", "dbo.PressIndexes");
            DropForeignKey("dbo.MPTs", "Homol_Id", "dbo.HomolAttributes");
            DropForeignKey("dbo.MPTs", "Height_Id", "dbo.Heights");
            DropForeignKey("dbo.MPTs", "Diameter_Id", "dbo.Diameters");
            DropForeignKey("dbo.MPTs", "Currency_Id", "dbo.Currencies");
            DropForeignKey("dbo.MPTs", "ConvSign_Id", "dbo.ConvSigns");
            DropForeignKey("dbo.MPTs", "Brand_Id", "dbo.Brands");
            DropForeignKey("dbo.MPTs", "Model_Id", "dbo.Models");
            DropForeignKey("dbo.Models", "Brand_Id", "dbo.Brands");
            DropForeignKey("dbo.MPTs", "Country_Id", "dbo.Countries");
            DropForeignKey("dbo.Brands", "Country_Id", "dbo.Countries");
            DropForeignKey("dbo.MPTs", "AutoType_Id", "dbo.AutoTypes");
            DropForeignKey("dbo.PriceReadSettings", "PriceLanguage_Id", "dbo.PriceLanguages");
            DropForeignKey("dbo.PriceReadSettings", "Agent_Id", "dbo.Agents");
            DropIndex("dbo.AspNetUserClaims", new[] { "User_Id" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.Prices", new[] { "PriceReadSetting_Id" });
            DropIndex("dbo.MPTs", new[] { "Width_Id" });
            DropIndex("dbo.MPTs", new[] { "StockCity_Id" });
            DropIndex("dbo.MPTs", new[] { "SpeedIndex_Id" });
            DropIndex("dbo.MPTs", new[] { "Season_Id" });
            DropIndex("dbo.MPTs", new[] { "Price_Id" });
            DropIndex("dbo.MPTs", new[] { "PressIndex_Id" });
            DropIndex("dbo.MPTs", new[] { "Homol_Id" });
            DropIndex("dbo.MPTs", new[] { "Height_Id" });
            DropIndex("dbo.MPTs", new[] { "Diameter_Id" });
            DropIndex("dbo.MPTs", new[] { "Currency_Id" });
            DropIndex("dbo.MPTs", new[] { "ConvSign_Id" });
            DropIndex("dbo.MPTs", new[] { "Brand_Id" });
            DropIndex("dbo.MPTs", new[] { "Model_Id" });
            DropIndex("dbo.Models", new[] { "Brand_Id" });
            DropIndex("dbo.MPTs", new[] { "Country_Id" });
            DropIndex("dbo.Brands", new[] { "Country_Id" });
            DropIndex("dbo.MPTs", new[] { "AutoType_Id" });
            DropIndex("dbo.PriceReadSettings", new[] { "PriceLanguage_Id" });
            DropIndex("dbo.PriceReadSettings", new[] { "Agent_Id" });
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.Widths");
            DropTable("dbo.StockCities");
            DropTable("dbo.SpeedIndexes");
            DropTable("dbo.Seasons");
            DropTable("dbo.PressIndexes");
            DropTable("dbo.HomolAttributes");
            DropTable("dbo.Heights");
            DropTable("dbo.Diameters");
            DropTable("dbo.Currencies");
            DropTable("dbo.ConvSigns");
            DropTable("dbo.Models");
            DropTable("dbo.Countries");
            DropTable("dbo.Brands");
            DropTable("dbo.AutoTypes");
            DropTable("dbo.MPTs");
            DropTable("dbo.Prices");
            DropTable("dbo.PriceLanguages");
            DropTable("dbo.PriceReadSettings");
            DropTable("dbo.Agents");
        }
    }
}
